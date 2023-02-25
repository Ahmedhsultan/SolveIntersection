using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities
{
    internal class AlignmentDB
    {
        public Alignment al1R { get; set; }
        public Alignment al1L { get; set; }
        public Alignment al2R { get; set; }
        public Alignment al2L { get; set; }
    }
}
