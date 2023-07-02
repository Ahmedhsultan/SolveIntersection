using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB;
using SolveIntersection.Util;
using System.Collections.Generic;

namespace SolveIntersection.EndPoint
{
    internal class GetRoadsAlignmentsFromCorridors
    {
        public GetRoadsAlignmentsFromCorridors(Transaction ts, Editor editor, Corridor corridor1, Corridor corridor2)
        {
            Dictionary<Point3d, List<Alignment>> intersectedAlignments = new Dictionary<Point3d, List<Alignment>>();
            Point3d clickedPoint = Select.selectPoint(ts, editor);

            foreach (var basline1 in corridor1.Baselines)
            {
                Alignment alignment1 = ts.GetObject(basline1.AlignmentId, OpenMode.ForRead) as Alignment;
                foreach (var baslinge2 in corridor2.Baselines)
                {
                    Alignment alignment2 = ts.GetObject(baslinge2.AlignmentId, OpenMode.ForRead) as Alignment;
                    DetectIntersecionPoint<Alignment> detectIntersecionPoint = new DetectIntersecionPoint<Alignment>(alignment1, alignment2);
                    foreach (Point3d point in detectIntersecionPoint.intersectionPoints)
                        if (!intersectedAlignments.ContainsKey(point))
                            intersectedAlignments.Add(point, new List<Alignment>() { alignment1, alignment2 });
                }
            }

            double minDist = double.MaxValue;
            Point3d minDistPoint = new Point3d();
            foreach (var pair in intersectedAlignments)
            {
                double dist = pair.Key.DistanceTo(clickedPoint);
                if (dist < minDist)
                {
                    minDist = dist;
                    minDistPoint = pair.Key;
                }
            }

            IntersectionDB.getInstance().road_Main.alignment = intersectedAlignments[minDistPoint][0];
            IntersectionDB.getInstance().road_Secondary.alignment = intersectedAlignments[minDistPoint][1];
            IntersectionDB.getInstance().data.intersectionPoint = minDistPoint;
        }
    }
}
