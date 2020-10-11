using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Agent
{
    class Logger
    {
        //==>TODO#15 Rework logging. Write to file or db instead console
        //==>TODO#5 Add factory for messages
        public static void PrintServerStarted(int _agentId, IPEndPoint _endPoint)
        {
            Console.WriteLine("Agent #{0} Server Started at: Ip = {2}; Port = {3}, time =  {1}",
                   _agentId,
                   DateTime.Now.ToString("hh:mm:ss.fff"),
                   _endPoint.Address,
                   _endPoint.Port);
        }

        public static void PrintGeneratedMessage(int _agentId, TCPMesssege messsege)
        {
            Console.WriteLine("Agent #{0} goes to: ({2:N2}; {3:N2}; {4:N2}), time = {1}",
                 _agentId,
                 DateTime.Now.ToString("hh:mm:ss.fff"),
                 messsege.CurrentAgentPosition.X,
                 messsege.CurrentAgentPosition.Y,
                 messsege.CurrentAgentPosition.Z);
        }

        public static void PrintReceivedMessage(int _agentId, TCPMesssege messsege, int clientname)
        {
            Console.WriteLine("Agent #{1}(client #{2}) received message Agent #{0} goes to ({3:N2}; {4:N2}; {5:N2}) time = {6} ",
            messsege.SenderId, _agentId, clientname,
            messsege.CurrentAgentPosition.X,
            messsege.CurrentAgentPosition.Y,
            messsege.CurrentAgentPosition.Z,
            DateTime.Now.ToString("hh:mm:ss.fff"));
        }

        public static void PrintTraectoryConflict(int _agentId, Point3D pos)
        {
            Console.WriteLine("Agent #{0} CONFLICT WAIT ON ({1:N2}; {2:N2}; {3:N2}) time = {4} ",
             _agentId,
            pos.X,
            pos.Y,
            pos.Z,
            DateTime.Now.ToString("hh:mm:ss.fff"));
        }
    }
}
