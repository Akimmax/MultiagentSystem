using Qactive;
using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Text.Json;//==>TODO#4 use newtosoft
using System.Windows.Media.Media3D;

namespace AgentVisualisator
{
    //==>TODO#1 Move code to infasructure to re-use
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

            //Handle messages published by this certain agent
            using (query.Subscribe(
              value => {
                  var messsege = JsonSerializer.Deserialize<TCPMessage>(value); //==>TODO#5 extract to logger and messege factory
                  var str = $"Agent #{ _agentId}, Client #{name} received message from {messsege.SenderId} :X = {messsege.CurrentAgentPosition.X} ; Y  = {messsege.CurrentAgentPosition.Y} ;  z  = {messsege.CurrentAgentPosition.Z} === {DateTime.Now.ToString("hh:mm:ss.fff")} ";
                  WriteToListBox(str);

                  DisplayPositionOnChart(messsege.CurrentAgentPosition, messsege.SenderId);
              },
              ex =>
              {
                  WriteToListBox($"Error In agent #{_agentId} in client #{name}: {ex.Message}");
              },
              () => {
                  Console.WriteLine("In agent #{0} client #{1} completed", _agentId, name);
              }
              ))
            {
                var x = Console.Read();//==>TODO#6 remove somehow
                Thread.Sleep(50000);////==>TODO#6 remove somehow //Console.ReadKey(intercept: true);
            }
        }
        public void WriteToListBox(string myString)
        {
            form.Invoke(form.writeText, new object[] { myString });
        }

        public void DisplayPositionOnChart(Point3D position, int agentId)
        {
            form.Invoke(form.drawOnChart, new object[] { position, agentId });
            string mess = "Try drow on chart " + agentId + " " + position.X + " " + position.Y;
            WriteToListBox(mess);
        }

    }

    class TCPMessage
    {
        public int SenderId { get; set; }
        public Point3D CurrentAgentPosition { get; set; }
        public string Details { get; set; }
    }
}
