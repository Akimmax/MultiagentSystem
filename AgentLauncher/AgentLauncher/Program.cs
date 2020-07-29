using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace AgentLauncher
{
    class Program
    {
        static void Main(string[] args)
        {

            //TODO Make pre-definite set of figures
            var figure = new Figure();
            figure.PointSet = new Point3D[] {
                ToPoint("0,0,0"),
                ToPoint("0,80,0"),
                ToPoint("80,0,0"),
                //ToPoint("70,70,0")
            };

            string ip = "127.0.0.1";
            string ip3 = "127.0.0.1";//ip another computer
            string port1 = "38245";
            string port2 = "38246";
            string port3 = "38244";

            Point3D p1 = ToPoint("0,0,100");
            Point3D p2 = ToPoint("50,150,0");
            Point3D p3 = ToPoint("150,40,10");


            AgentInfo[] agentsInfo = new AgentInfo[] {
                 new AgentInfo(){AgentId = 1, currentPosition = p1},
                 new AgentInfo(){AgentId = 2, currentPosition = p2},
                 new AgentInfo(){AgentId = 3, currentPosition = p3},
            };


            //==> TODO Move this logic to Agent itself, to allow an Agent decide and act absolutely independently

            var result = GeometryHelper.MapAgentToFigurePoints(agentsInfo, figure).ToArray();


            var pt1 = result[0].targetPosition;
            var pt2 = result[1].targetPosition;
            var pt3 = result[2].targetPosition;
            //var pt4 = result[3].targetPosition;

            string exeFilePath = new System.Uri(Assembly.GetEntryAssembly().CodeBase).AbsolutePath;
            string solutionDirectoryPath = Path.GetDirectoryName(exeFilePath);
            string agentFile = Path.GetFullPath(Path.Combine(solutionDirectoryPath, @"..\..\..\..\Agent\Agent\bin\Debug\Agent.exe"));

            string agent1Options = $"{ip} {port1} 1 {ip} {port2} 1 {ip3} {port3} 2 {ToStr(p1)} {ToStr(pt1)}";
            string agent2Options = $"{ip} {port2} 2 {ip} {port1} 1 {ip3} {port3} 2 {ToStr(p2)} {ToStr(pt2)}";
            string agent3Options = $"{ip3} {port3} 3 {ip} {port1} 1 {ip} {port2} 2 {ToStr(p3)} {ToStr(pt3)}";


            Process.Start(agentFile, agent1Options);
            Process.Start(agentFile, agent2Options);
            Process.Start(agentFile, agent3Options);

            Console.ReadKey();
        }

        static string ToStr(Point3D p)
        {
            return $"{p.X},{p.Y},{p.Z}";
        }

        static public Point3D ToPoint(string args)
        {
            var parameters = args.Split(',').ToList<string>();
            var coordinates = parameters.Select(str => {
                return Convert.ToDouble(int.Parse(str));
            }).ToArray();

            return new Point3D(coordinates[0], coordinates[1], coordinates[2]);
        }

    }
}
