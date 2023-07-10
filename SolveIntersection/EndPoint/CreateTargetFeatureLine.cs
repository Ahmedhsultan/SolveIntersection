using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB;
using SolveIntersection.DB.Entities;
using System;

namespace SolveIntersection.EndPoint
{
    internal class CreateTargetFeatureLine
    {
        public CreateTargetFeatureLine(Database db, Transaction tr, RightTurn rightTurn, RightTurn leftTurn, Road mainRoad)
        {
            //Create polyline
            var pline = new Polyline();
            var plane = new Plane(Point3d.Origin, Vector3d.ZAxis);
            pline.AddVertexAt(0, rightTurn.alignment.StartPoint.Convert2d(plane), 0.0, 0.0, 0.0);
            pline.AddVertexAt(1, leftTurn.alignment.StartPoint.Convert2d(plane), 0.0, 0.0, 0.0);
            BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
            BlockTableRecord space = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
            space.AppendEntity(pline);
            tr.AddNewlyCreatedDBObject(pline, true);
            tr.Commit();

            //Create featureline
            FeatureLine feature = tr.GetObject(FeatureLine.Create(Guid.NewGuid().ToString(), pline.Id), OpenMode.ForWrite) as FeatureLine;

            //Take featureline elveation from mainroad surface
            feature.AssignElevationsFromSurface(mainRoad.surfaceTop.SurfaceId, true);

            //Add featureline to database
            IntersectionDB.getInstance().data.featureLineTarget = feature;

            tr.Commit();
        }
    }
}
