using Qactive;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Text.Json;////==> TODO use newtosoft
using System.Text.Json.Serialization;
using System.Windows.Media.Media3D;
using System.Collections.Generic;

namespace Agent
{
    class Agent
    {
        private IPEndPoint _endPoint;
        Subject<string> subject = new Subject<string>();
        private int _agentId;
        
        Dictionary<int, Point3D> lastPositionsKnownAgents = new Dictionary<int, Point3D>();
        //==================================== ==>TODO Extract into separate class
        //Point _currentPosition = new Point(0, 0);
        Point3D _currentPosition2 = new Point3D(0, 0, 0);//==>3D
        double _step = 8;
        //==================================== ==>TODO Extract into separate class
        public Agent(IPAddress endPoint, int port, int id)
        {
            _endPoint = new IPEndPoint(endPoint, port: port);
            _agentId = id;
        }

        public void SetPosition2(Point3D position)//==>3D
        {
            _currentPosition2 = position;
        }

        public void StartService()
        {

            Logger.PrintServerStarted(_agentId, _endPoint);

            IObservable<string> source = subject.AsObservable();
            source.Subscribe(value => {
                var messsege = JsonSerializer.Deserialize<TCPMesssege3D>(value);
                Logger.PrintGeneratedMessage(_agentId, messsege);
            });
            subject.OnNext(
                JsonSerializer.Serialize(new TCPMesssege3D() { senderId = _agentId, currentPosition3D = _currentPosition2, details = "As" }
                ));

            var service = source.ServeQbservableTcp<string>(
              _endPoint,
               new QbservableServiceOptions() { SendServerErrorsToClients = true, EnableDuplex = true, AllowExpressionsUnrestricted = true });////==> TODO extract to log
    
            service.Subscribe(
              terminatedClient =>
              {
                  foreach (var ex in terminatedClient.Exceptions)
                  {
                      Console.WriteLine("Basic service error: {0}", ex.SourceException.Message);
                  }

                  Console.WriteLine("Basic client shutdown: " + terminatedClient);
              },
              ex => Console.WriteLine("Basic fatal service error: {0}", ex.Message),
              () => Console.WriteLine("This will never be printed because a service host never completes."));
        }


        public void RunClient(IPAddress endPoint, int port, int name)
        {
            var client = new TcpQbservableClient<string>(endPoint, port);

            var query =
              (from value in client.Query()
               select value);

            using (query.Subscribe(
              value => {
                  var messsege = JsonSerializer.Deserialize<TCPMesssege3D>(value); ////==>   extract to log

                  Logger.PrintReceivedMessage(_agentId, messsege, name);
                  SaveAgentPosition(_agentId, messsege.currentPosition3D);
              },
              ex => Console.WriteLine("Error In agent #{1} in client #{2}: {0}", ex.Message, _agentId, name),
              () => Console.WriteLine("In agent #{0} client #{1} completed", _agentId, name)))
            {
                Console.WriteLine("Agent #{0} client #{1} started.  Waiting for basic service notifications...", _agentId, name);
                Console.ReadKey(intercept: true); ////==> TODO remove but safely
            }
        }

        public void PublishCurrentAgentPosition2()
        {
            var obj = new TCPMesssege3D() { senderId = _agentId, currentPosition3D = _currentPosition2, details = "As" };//==> TODO rework in 1 line
            string message = JsonSerializer.Serialize<TCPMesssege3D>(obj);
            subject.OnNext(message);
        }

        public void MoveToPoint2(Point3D targetPosition)
        {
            int i = 0;
            while (_currentPosition2 != targetPosition && i < 50)
            {
                Point3D nextPosition = GeometryHelper.GetNextPosition(_currentPosition2, targetPosition, _step);

                if (!CheckIfAgentCrashOtherAgentOnNextStep(nextPosition))
                {
                    PublishNextAgentPosition2(nextPosition);
                    _currentPosition2 = nextPosition;

                    //_currentPosition2 = GeometryHelper.GetNextPosition(_currentPosition2, targetPosition, _step);
                }
                else
                {
                    PublishNextAgentPosition2(_currentPosition2);//Skip step 
                    //_currentPosition2 = GeometryHelper.GetNextPosition(_currentPosition2, targetPosition, _step);
                    Logger.PrintTraectoryConflict(_agentId, _currentPosition2);
                }

                //PublishCurrentAgentPosition2();
                i++;
                Thread.Sleep(1000);
            }

            PublishCurrentAgentPosition2();
        }

        public void PublishNextAgentPosition2(Point3D nextPosition)
        {
            var obj = new TCPMesssege3D() { senderId = _agentId, currentPosition3D = nextPosition, details = "As" };
            string message = JsonSerializer.Serialize<TCPMesssege3D>(obj);
            subject.OnNext(message);
        }


        public void SaveAgentPosition(int agentId, Point3D position)
        {

            ///==>TODO Make update tread safe using monitor
            if (lastPositionsKnownAgents.ContainsKey(agentId))
                lastPositionsKnownAgents[agentId] = position; // or throw exception        
            else
                lastPositionsKnownAgents.Add(agentId, position);
        }
        public bool CheckIfAgentCrashOtherAgentOnNextStep(Point3D nextPosition)
        {
            Point3D[] sphereCentres = lastPositionsKnownAgents.Values.ToArray();
            return GeometryHelper.CheckIfPointCrossSpheres(nextPosition, sphereCentres, _step * 2);
        }

        //==================================== ==>TODO Extract into separate class
    }

    //[Serializable]
    class TCPMesssege3D
    {
        public int senderId { get; set; } //==>TODO Name To PascalCase
        //public Point currentPosition { get; set; }

        public Point3D currentPosition3D { get; set; }
        //public Point nextPosition;
        public string details { get; set; }
    }
}
