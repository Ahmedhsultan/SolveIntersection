using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities
{
    internal class PolylinesDB
    {
        public Polyline polyline1 { get; set; }
        public Polyline polyline2 { get; set; }
        public Polyline polyline3 { get; set; }
        public Polyline polyline4 { get; set; }
    }
}
