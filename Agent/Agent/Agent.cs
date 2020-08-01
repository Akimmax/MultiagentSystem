using Qactive;
using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Text.Json;////==> TODO use newtosoft
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace Agent
{
    class Agent
    {
        IPEndPoint _endPoint;
        Subject<string> _subject = new Subject<string>();
        int _agentId;
        readonly object _lockObj = new object();

        Dictionary<int, Point3D> lastPositionsKnownAgents = new Dictionary<int, Point3D>();// positions other Agents
        
        Point3D _currentPosition = new Point3D(0, 0, 0);//==>TODO Extract into base class MovableAgent
        double _step = 8;// speed: 8 per tick
        
        public Agent(IPAddress endPoint, int port, int id)
        {
            _endPoint = new IPEndPoint(endPoint, port: port);
            _agentId = id;
        }

        public void SetPosition(Point3D position)
        {
            _currentPosition = position;
        }

        public void StartService()//==>TODO!!! Extract into separate class
        {

            Logger.PrintServerStarted(_agentId, _endPoint);

            IObservable<string> source = _subject.AsObservable();

            //==>TODO move code to separate handlers
            //Handle messages published only by this agent
            source.Subscribe(value => {
                var messsege = JsonSerializer.Deserialize<TCPMesssege>(value);
                Logger.PrintGeneratedMessage(_agentId, messsege); 
            });

            //==>TODO Rework without serialization to be able use LINQ to QbservableTcp 
            //Publish message with initial agent status
            _subject.OnNext(
                JsonSerializer.Serialize(new TCPMesssege() { SenderId = _agentId, CurrentAgentPosition = _currentPosition, Details = "details" }
                ));

            //Start TCP server. service starts serve the source
            var service = source.ServeQbservableTcp<string>(
              _endPoint,
               new QbservableServiceOptions() { SendServerErrorsToClients = true, EnableDuplex = true, AllowExpressionsUnrestricted = true });

            //==>TODO move code to separate handlers
            //Handle errors during work service
            service.Subscribe(
              terminatedClient =>
              {
                  foreach (var ex in terminatedClient.Exceptions)
                  {
                      Console.WriteLine("Basic service error: {0}", ex.SourceException.Message);//==>TODO Extract messages to logger
                  }

                  Console.WriteLine("Basic client shutdown: " + terminatedClient);
              },
              ex => Console.WriteLine("Basic fatal service error: {0}", ex.Message),
              () => Console.WriteLine("This will never be printed because a service host never completes."));
        }


        public void RunClient(IPAddress endPoint, int port, int name)//==>TODO Extract into separate class
        {            
            var client = new TcpQbservableClient<string>(endPoint, port);

            var query =
              (from value in client.Query()
               select value);

            //==>TODO move code to separate handlers
            //Handle messages published by this certain agent
            //==>TODO Extract messeges to logger
            using (query.Subscribe(
              value => {
                  var messsege = JsonSerializer.Deserialize<TCPMesssege>(value);

                  Logger.PrintReceivedMessage(_agentId, messsege, name);
                  SaveAgentPosition(_agentId, messsege.CurrentAgentPosition);//Handle recieved info published by other agents
              },              
              ex => Console.WriteLine("Error In agent #{1} in client #{2}: {0}", ex.Message, _agentId, name), 
              () => Console.WriteLine("In agent #{0} client #{1} completed", _agentId, name)))
            {
                Console.WriteLine("Agent #{0} client #{1} started.  Waiting for basic service notifications...", _agentId, name);
                Console.ReadKey(intercept: true); //DO NOT REMOVE it. leads to ProtocolNegotiationError
            }
        }

        public void MoveToPoint(Point3D targetPosition)
        {
            int i = 0;
            while (_currentPosition != targetPosition && i < 50)//==>TODO Figure how to limit time of work
            {
                Point3D nextPosition = GeometryHelper.GetNextPosition(_currentPosition, targetPosition, _step);

                //Check that Agent is not moving to position where is any of known agens
                if (!CheckIfCrashOtherAgentOnNextStep(nextPosition))//intellectuality of Agent) 
                {
                    PublishAgentPosition(nextPosition);
                    _currentPosition = nextPosition;//moving itself
                }
                else
                {
                    PublishAgentPosition(_currentPosition);//Skip step. wait until moving is safe
                    Logger.PrintTraectoryConflict(_agentId, _currentPosition);
                }

                i++;
                Thread.Sleep(1000);////TODO Extract to config.
            }

            PublishCurrentAgentPosition();//Final message
        }

        public void PublishAgentPosition(Point3D nextPosition)
        {
            var obj = new TCPMesssege() { SenderId = _agentId, CurrentAgentPosition = nextPosition, Details = "As" };
            string message = JsonSerializer.Serialize<TCPMesssege>(obj);
            _subject.OnNext(message);//Published info
        }

        public void PublishCurrentAgentPosition()
        {
            PublishAgentPosition(_currentPosition);
        }

        //Store info (positions) from other agents in system
        public void SaveAgentPosition(int agentId, Point3D position)
        {
            lock (_lockObj)//each client works in own tread, this part shold be tread safe
            {
                if (lastPositionsKnownAgents.ContainsKey(agentId))
                    lastPositionsKnownAgents[agentId] = position; // or throw exception        
                else
                    lastPositionsKnownAgents.Add(agentId, position);
            }
        }
        public bool CheckIfCrashOtherAgentOnNextStep(Point3D nextPosition)
        {
            Point3D[] sphereCentres = lastPositionsKnownAgents.Values.ToArray();
            return GeometryHelper.CheckIfPointCrossSpheres(nextPosition, sphereCentres, _step * 2);
        }

    }

    class TCPMesssege
    {
        public int SenderId { get; set; }
        public Point3D CurrentAgentPosition { get; set; }
        public string Details { get; set; }
    }
}