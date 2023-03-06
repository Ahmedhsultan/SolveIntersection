using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities
{
    internal class Selection
    {
        public Polyline polyline1 { get; set; }
        public Polyline polyline2 { get; set; }
    }
}
