using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB;
using System;

namespace SolveIntersection.Util
{
    internal class DetectIntersecionPoint<T> where T : Entity
    {
        public Point3dCollection intersectionPoints { get; set; }
        public DetectIntersecionPoint(T entity1, T entity2)
        {
            intersectionPoints = new Point3dCollection();
            entity1.IntersectWith(entity2, Autodesk.AutoCAD.DatabaseServices.Intersect.OnBothOperands, intersectionPoints, IntPtr.Zero, IntPtr.Zero);
        }
    }
}