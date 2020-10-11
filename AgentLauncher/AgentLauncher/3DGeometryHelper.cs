using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace AgentLauncher
{
    //==>TODO#1 Move code to infasructure to re-use
    class GeometryHelper
    {        
        public static IEnumerable<AgentInfo> MapAgentToFigurePoints(AgentInfo[] agentsInfo, Figure figure)
        {
            List<Point3D> tempPointSet = new List<Point3D>(figure.PointSet);
            List<AgentInfo> agentGoals = new List<AgentInfo>();

            foreach (var agentInfo in agentsInfo)
            {
                List<PointInfo> distanceToPoints = new List<PointInfo>();

                for (int i = 0; i < tempPointSet.Count(); i++)
                {
                    var pointDist = new PointInfo()
                    {
                        distanseToCurrentAgent = GetDistanseBetweenPoints(agentInfo.currentPosition, tempPointSet[i]),
                        pointerToPoint = i
                    };
                    distanceToPoints.Add(pointDist);
                }
                //Get point with min distance
                PointInfo nearestPoint = distanceToPoints.OrderBy(p => p.distanseToCurrentAgent).First();
                //Booking point by the agent
                agentGoals.Add(new AgentInfo { AgentId = agentInfo.AgentId, targetPosition = tempPointSet[nearestPoint.pointerToPoint] });
                tempPointSet.RemoveAt(nearestPoint.pointerToPoint);
            }

            return agentGoals;

        }

        static double GetDistanseBetweenPoints(Point3D point1, Point3D point2)
        {
            return new Vector3D(
                       Math.Abs(point1.X - point2.X),
                       Math.Abs(point1.Y - point2.Y),
                       Math.Abs(point1.Z - point2.Z))
                       .Length;
        }
    }


    class Figure
    {
        public Point3D[] PointSet;
    }

    class PointInfo
    {
        public double distanseToCurrentAgent;
        public int pointerToPoint;

    }

    class AgentInfo
    {
        public int AgentId;
        public Point3D currentPosition;
        public Point3D targetPosition;
    }
}
