using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities
{
    internal class CorridorDB
    {
        public Corridor corr1 { get; set; }
        public Corridor corr2 { get; set; }
        public Alignment alignment1 { get; set; }
        public Alignment alignment2 { get; set; }
    }
}
