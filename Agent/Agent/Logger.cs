﻿using System;
using System.Net;
using System.Windows.Media.Media3D;

namespace Agent
{

    //==> TODO Write to file or db instead console
    class Logger
    {
        public static void PrintServerStarted(int _agentId, IPEndPoint _endPoint)
        {
            Console.WriteLine("Agent #{0} Server Started at: Ip = {2}; Port = {3}, time =  {1}",
                   _agentId,
                   DateTime.Now.ToString("hh:mm:ss.fff"),
                   _endPoint.Address,
                   _endPoint.Port);
        }

        public static void PrintGeneratedMessage(int _agentId, TCPMesssege3D messsege)
        {
            Console.WriteLine("Agent #{0} goes to: ({2:N2}; {3:N2}; {4:N2}), time = {1}",
                 _agentId,
                 DateTime.Now.ToString("hh:mm:ss.fff"),
                 messsege.currentPosition3D.X,
                 messsege.currentPosition3D.Y,
                 messsege.currentPosition3D.Z);
        }

        public static void PrintReceivedMessage(int _agentId, TCPMesssege3D messsege, int clientname)
        {
            Console.WriteLine("Agent #{1}(client #{2}) received message Agent #{0} goes to ({3:N2}; {4:N2}; {5:N2}) time = {6} ",
            messsege.senderId, _agentId, clientname,
            messsege.currentPosition3D.X,
            messsege.currentPosition3D.Y,
            messsege.currentPosition3D.Z,
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
