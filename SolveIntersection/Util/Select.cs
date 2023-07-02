using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using System;

namespace SolveIntersection.Util
{
    internal class Select
    {
        public static T entity<T>(Transaction ts, Editor editor, String msg) where T : Autodesk.AutoCAD.DatabaseServices.Entity
        {
            PromptEntityOptions options = new PromptEntityOptions(msg);
            options.SetRejectMessage("\nNot a valid Type. Please try again.");
            options.AddAllowedClass(typeof(T), false);
            PromptEntityResult sourcePrompt = editor.GetEntity(options);
            if (sourcePrompt.Status != PromptStatus.OK)
            {
                ts.Abort();
                return null;
            }
            T entity = ts.GetObject(sourcePrompt.ObjectId, OpenMode.ForWrite) as T;

            return entity;
        }

        public static BaselineRegion region(String msg)
        {
            BaselineRegion baselineRegion;

            do
            {
                PromptPointOptions prompt = new PromptPointOptions(msg);
                Application.DocumentManager.MdiActiveDocument.Editor.PointMonitor += CursorStopEventHandler.doEvent;
                Application.DocumentManager.MdiActiveDocument.Editor.GetPoint(prompt);
                Application.DocumentManager.MdiActiveDocument.Editor.PointMonitor -= CursorStopEventHandler.doEvent;

                baselineRegion = CursorStopEventHandler.region;
            } while (baselineRegion == null);

            return baselineRegion;
        }

        public static Point3d selectPoint(Transaction ts, Editor ed)
        {
            Point3d selectedPoint;
            PromptPointResult ppr = ed.GetPoint("\nSelect a point: ");

            // If the prompt status is OK, continue with the selected point
            if (ppr.Status == PromptStatus.OK)
                selectedPoint = ppr.Value;
            else
            {
                ts.Abort();
                return Point3d.Origin;
            }
            return selectedPoint;
        }
    }
}
