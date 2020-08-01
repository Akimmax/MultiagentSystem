using Qactive;
using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Text.Json;////==> TODO use newtosoft
using System.Windows.Media.Media3D;

namespace AgentVisualisator
{

    class TCPClient
    {
        private int _agentId;
        public Form1 form;

        public TCPClient(int id)
        {
            _agentId = id;
        }

        public void RunClient(IPAddress endPoint, int port, int name)
        {
            var client = new TcpQbservableClient<string>(endPoint, port);

            var query =
              (from value in client.Query()
               select value);
            using (query.Subscribe(
              value => {
                  var messsege = JsonSerializer.Deserialize<TCPMessage>(value); ////==> TODO extract to log
                  var str = $"Agent #{ _agentId}, Client #{name} received message from {messsege.SenderId} :X = {messsege.CurrentAgentPosition.X} ; Y  = {messsege.CurrentAgentPosition.Y} ;  z  = {messsege.CurrentAgentPosition.Z} === {DateTime.Now.ToString("hh:mm:ss.fff")} ";
                  PrintText(str);

                  // DisplayOnForm(messsege.currentPosition3D);
                  DisplayOnChart3D(messsege.CurrentAgentPosition, messsege.SenderId);
              },
              ex =>
              {
                  PrintText($"Error In agent #{_agentId} in client #{name}: {ex.Message}");

              },
              () => {
                  Console.WriteLine("In agent #{0} client #{1} completed", _agentId, name);
              }
              ))
            {
                // PrintText($"Agent #{_agentId} client #{name} started.  Waiting for basic service notifications...");
                //Console.ReadKey(intercept: true); ////==> TODO remove 
                var x = Console.Read();
                Thread.Sleep(50000);////==> TODO remove 
            }
        }
        public void PrintText(string myString)
        {
            form.Invoke(form.myDelegate, new object[] { myString });
        }

        public void DisplayOnForm(Point position)
        {
            form.Invoke(form.drowOnForm, new object[] { position });
        }

        public void DisplayOnChart3D(Point3D position, int agentId)
        {
            form.Invoke(form.drowOnChart3D, new object[] { position, agentId });
            string mess = "Try drow on chart " + agentId + " " + position.X + " " + position.Y;
            PrintText(mess);
        }

        public void DisplayOnChart(Point3D position, int agentId)
        {
            form.Invoke(form.drowOnChart, new object[] { position, agentId });
            string mess = "Try drow on chart " + agentId + " " + position.X + " " + position.Y + " " + position.Z;
            PrintText(mess);

        }

    }

    class TCPMessage
    {
        public int SenderId { get; set; } //==>TODO Name To PascalCase
        public Point3D CurrentAgentPosition { get; set; }
        public string Details { get; set; }
    }
}
