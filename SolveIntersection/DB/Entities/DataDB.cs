using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities
{
    public class DataDB
    {
        public FeatureLine featureLineTarget { get; set; }
        //public Point3d intersectionPoint { get; set; }
    }
}
