using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.Util
{
    internal class CursorStopEventHandler
    {
        public static BaselineRegion region { get; set; }
        public static void doEvent(object sender, PointMonitorEventArgs e)
        {
            // Get the selected object ID
            Point3d snapPoint = e.Context.RawPoint;

            // Get the active Civil 3D document
            CivilDocument civilDoc = CivilApplication.ActiveDocument;

            // Access the corridor collection
            CorridorCollection corridors = civilDoc.CorridorCollection;
            Transaction tr;
            using (tr = HostApplicationServices.WorkingDatabase.TransactionManager.StartTransaction())
            {
                foreach (ObjectId corrId in corridors)
                {
                    var corr = (Corridor)tr.GetObject(corrId, OpenMode.ForRead);
                    foreach (Baseline bsline in corr.Baselines)
                    {
                        foreach (BaselineRegion baselineRegion in bsline.BaselineRegions)
                        {
                            List<OffsetPoints> pts = new List<OffsetPoints>();

                            foreach (AppliedAssembly assy in baselineRegion.AppliedAssemblies)
                            {
                                CalculatedLinkCollection links = assy.GetLinksByCode("Top");
                                double maxLeftOffset = 0, maxRightOffset = 0;
                                OffsetPoints offpts = new OffsetPoints();
                                bool firstpt = true;
                                foreach (CalculatedLink link in links)
                                {
                                    foreach (CalculatedPoint cpt in link.CalculatedPoints)
                                    {
                                        Point3d offPt = cpt.StationOffsetElevationToBaseline;
                                        double offset = offPt.Y;
                                        if (firstpt)
                                        {
                                            maxLeftOffset = offset;
                                            maxRightOffset = offset;
                                            offpts.LeftOffset = bsline.StationOffsetElevationToXYZ(offPt);
                                            offpts.RightOffset = bsline.StationOffsetElevationToXYZ(offPt);
                                            firstpt = false;
                                        }
                                        else
                                        {
                                            if (offset < maxLeftOffset)
                                            {
                                                maxLeftOffset = offset;
                                                offpts.LeftOffset = bsline.StationOffsetElevationToXYZ(offPt);
                                                continue;
                                            }
                                            else if (offset > maxRightOffset)
                                            {
                                                maxRightOffset = offset;
                                                offpts.RightOffset = bsline.StationOffsetElevationToXYZ(offPt);
                                            }
                                        }
                                    }
                                }
                                pts.Add(offpts);
                            }
                            Point2dCollection bndypts = new Point2dCollection();
                            for (int i = 0; i < pts.Count; i++)
                            {
                                bndypts.Add(new Point2d(pts[i].LeftOffset.X, pts[i].LeftOffset.Y));
                            }
                            for (int i = pts.Count - 1; i > -1; i--)
                            {
                                bndypts.Add(new Point2d(pts[i].RightOffset.X, pts[i].RightOffset.Y));
                            }


                            if (Math.ContainsPoint(bndypts, snapPoint))
                            {
                                Polyline pline = new Polyline(bndypts.Count);
                                for (int i = 0; i < bndypts.Count; i++)
                                {
                                    pline.AddVertexAt(i, bndypts[i], 0, .2, .2);
                                }
                                pline.Closed = true;
                                pline.Elevation = 0;
                                var retval = pline.GeometricExtents;
                                //pline.Dispose();

                                pline.ColorIndex = 5;
                                pline.Highlight();
                                IntegerCollection col = new IntegerCollection();
                                Autodesk.AutoCAD.GraphicsInterface.TransientManager.CurrentTransientManager.AddTransient(pline, Autodesk.AutoCAD.GraphicsInterface.TransientDrawingMode.DirectShortTerm, 128, col);

                                region = baselineRegion;

                                goto loop;
                            }
                        }
                    }
                }
            }
        loop:;
        }
    }
}
