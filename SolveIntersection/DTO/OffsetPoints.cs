using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DTO
{
    internal class OffsetPoints
    {
        public Point3d RightOffset { get; set; }
        public Point3d LeftOffset { get; set; }
    }
}
