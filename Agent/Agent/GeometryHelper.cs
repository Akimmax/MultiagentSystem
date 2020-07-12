using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;

namespace Agent
{
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

        public static double GetDistanseBetweenPoints(Point3D point1, Point3D point2)
        {
            return new Vector3D(
                       Math.Abs(point1.X - point2.X),
                       Math.Abs(point1.Y - point2.Y),
                       Math.Abs(point1.Z - point2.Z))
                       .Length;
        }

        public static Point3D GetNextPosition(Point3D currentPos, Point3D tergetPos, double step)
        {   
            Vector3D vector = new Vector3D(tergetPos.X - currentPos.X, tergetPos.Y - currentPos.Y, tergetPos.Z - currentPos.Z);
            if (vector.Length < step) return tergetPos;
            Vector3D stepVector = Vector3D.Divide(vector, vector.Length / step);
            return new Point3D(currentPos.X + stepVector.X, currentPos.Y + stepVector.Y, currentPos.Z + stepVector.Z);
        }

        public static bool CheckIfPointCrossSpheres(Point3D point, Point3D[] sphereCentres, double sphereRadius)
        {
            foreach (var sphereCentre in sphereCentres)
            {
                double distanse = GetDistanseBetweenPoints(point, sphereCentre);
                if (sphereRadius > distanse) {
                    Console.WriteLine("cross with #{0}  #{1} . {2} ", sphereCentre.X, sphereCentre.Y, sphereCentre.Z);
                    return true;
                }
                 //sphere contains point
            }
                return false;
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
