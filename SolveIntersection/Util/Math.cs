using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.Util
{
    internal class Math
    {
        public static bool ContainsPoint(Point2dCollection points, Point3d point)
        {
            int count = points.Count;
            bool inside = false;

            // Ray casting algorithm
            for (int i = 0, j = count - 1; i < count; j = i++)
            {
                if (((points[i].Y > point.Y) != (points[j].Y > point.Y)) &&
                    (point.X < (points[j].X - points[i].X) * (point.Y - points[i].Y) / (points[j].Y - points[i].Y) + points[i].X))
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}
