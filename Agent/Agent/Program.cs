using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Agent
{
    class Program
    {
        static void Main(string[] args)
        {
            AppOptions opt = new AppOptions();
            opt.initalOptions(args);//==>TODO rework to receive unlimited numbers of agents

            var tcpAgent = new Agent(System.Net.IPAddress.Parse(opt.agentIp), opt.agentPort, opt.agentId);
            tcpAgent.SetPosition2(new Point3D(opt.currentPosition.X, opt.currentPosition.Y, opt.currentPosition.Z));
            tcpAgent.StartService();
            Thread.Sleep(1000);//time span to allow start all tcp servers before clients will try to connect to them

            var t3 = Task.Run(() => tcpAgent.MoveToPoint2(new Point3D(opt.targetPosition.X, opt.targetPosition.Y, opt.targetPosition.Z)));
            var t7 = Task.Run(() => tcpAgent.RunClient(IPAddress.Parse(opt.clientIp1), opt.clientPort1, opt.clientId1));
            var t8 = Task.Run(() => tcpAgent.RunClient(IPAddress.Parse(opt.clientIp2), opt.clientPort2, opt.clientId2));

            Console.ReadKey();
        }
    }
}
