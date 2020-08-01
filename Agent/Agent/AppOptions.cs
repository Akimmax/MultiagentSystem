using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace Agent
{

    //==>TODO rework to receive unlimited numbers of agents
    class AppOptions
    {
        //==>TODO Rework using reflection
        public void initalOptions(string[] args)
        {
            Console.WriteLine("initalOptions " + args.ToString());
            this.agentIp = args[0];
            this.agentPort = int.Parse(args[1]);
            this.agentId = int.Parse(args[2]);

            this.clientIp1 = args[3];
            this.clientPort1 = int.Parse(args[4]);
            this.clientId1 = int.Parse(args[5]);

            this.clientIp2 = args[6];
            this.clientPort2 = int.Parse(args[7]);
            this.clientId2 = int.Parse(args[8]);

            this.currentPosition3D = get3DPoint(args[9]);
            this.targetPosition3D = get3DPoint(args[10]);

        }
        //==>TODO Rework using reflection
        public void printOptions()
        {
            Console.WriteLine("printOptions ");
            Console.WriteLine(this.agentIp);
            Console.WriteLine(this.agentPort);
            Console.WriteLine(this.agentId);

            Console.WriteLine(this.clientIp1);
            Console.WriteLine(this.clientPort1);
            Console.WriteLine(this.clientId1);

            Console.WriteLine(this.clientIp2);
            Console.WriteLine(this.clientPort2);
            Console.WriteLine(this.clientId2);

            Console.WriteLine(this.currentPosition3D);
            Console.WriteLine(this.targetPosition3D);

        }

        static public Point3D get3DPoint(string args)
        {
            var parameters = args.Split(',').ToList<string>();
            var coordinates = parameters.Select(str => {
                return Convert.ToDouble(int.Parse(str));
            }).ToArray();

            return new Point3D(coordinates[0], coordinates[1], coordinates[2]);
        }


        public string agentIp;
        public int agentPort;
        public int agentId;
        public string clientIp1;
        public int clientPort1;
        public int clientId1;
        public string clientIp2;
        public int clientPort2;
        public int clientId2;
        public Point3D currentPosition3D;//==>TODO Rename
        public Point3D targetPosition3D;//==>TODO Rename
    }
}
