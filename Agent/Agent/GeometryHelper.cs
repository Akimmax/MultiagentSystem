using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Agent
{
    class GeometryHelper
    {
        //==> TODO Move code to infasructure to re-use
        //==> TODO Add tests
        //==> TODO Rework naming
        //Find the fastest way to make figure using agents, find nearest corner of figure for each agent
        public static IEnumerable<AgentInfo> MapAgentToFigurePoints(AgentInfo[] agentsInfo, Figure figure)
        {
            List<Point3D> tempPointSet = new List<Point3D>(figure.PointSet);
            List<AgentInfo> agentGoals = new List<AgentInfo>();//rework with tuples to avoid creating extra classes

            foreach (var agentInfo in agentsInfo)
            {
                List<PointInfo> distancesBetweenAgentAndFigurePoints = new List<PointInfo>();//rework with tuples

                for (int i = 0; i < tempPointSet.Count(); i++)
                {
                    var distanceToFigurePoint = new PointInfo()
                    {
                        distanseToCurrentAgent = GetDistanseBetweenTwoPoints(agentInfo.currentPosition, tempPointSet[i]),
                        pointerToPoint = i
                    };
                    distancesBetweenAgentAndFigurePoints.Add(distanceToFigurePoint);
                }
                //Get point with min distance
                PointInfo nearestPoint = distancesBetweenAgentAndFigurePoints.OrderBy(p => p.distanseToCurrentAgent).First();
                
                //Booking point by the agent
                agentGoals.Add(new AgentInfo { AgentId = agentInfo.AgentId, targetPosition = tempPointSet[nearestPoint.pointerToPoint] });
                tempPointSet.RemoveAt(nearestPoint.pointerToPoint);
            }

            return agentGoals;

        }

        //==>TODO add math formula
        public static double GetDistanseBetweenTwoPoints(Point3D point1, Point3D point2)
        {
            return new Vector3D(
                       Math.Abs(point1.X - point2.X),
                       Math.Abs(point1.Y - point2.Y),
                       Math.Abs(point1.Z - point2.Z))
                       .Length;
        }

        //==>TODO add math formula
        public static Point3D GetNextPosition(Point3D currentPos, Point3D tergetPos, double step)
        {
            Vector3D vector = new Vector3D(tergetPos.X - currentPos.X, tergetPos.Y - currentPos.Y, tergetPos.Z - currentPos.Z);
            if (vector.Length < step) return tergetPos;

            //get vector with correct direction and length which is equal to step
            Vector3D stepVector = Vector3D.Divide(vector, vector.Length / step);

            return new Point3D(currentPos.X + stepVector.X, currentPos.Y + stepVector.Y, currentPos.Z + stepVector.Z);//math vectors sum
        }

        //==>TODO add math formula
        //Check is any sphere contains point.
        //To avoid crash agent should not be close then some distance. If this this distance is radius of sphere and position of 1st agent is center, all points-positions of other agent should be always outside this spheres
        public static bool CheckIfPointCrossSpheres(Point3D point, Point3D[] sphereCentres, double sphereRadius)
        {
            foreach (var sphereCentre in sphereCentres)
            {
                double distanse = GetDistanseBetweenTwoPoints(point, sphereCentre);

                if (sphereRadius > distanse)//sphere contains point
                {
                    Console.WriteLine("cross with #{0}  #{1} . {2} ", sphereCentre.X, sphereCentre.Y, sphereCentre.Z);
                    return true;
                }

            }
            return false;
        }
    }


    class Figure
    {
        public Point3D[] PointSet;
    }

    //==>TODO rename
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
