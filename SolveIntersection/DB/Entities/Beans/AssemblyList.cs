using Autodesk.Civil.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities.Beans
{
    public class AssemblyList
    {
        public Assembly mainAss { get; set; }
        public Assembly assL1 { get; set; }
        public Assembly assR1 { get; set; }
        public Assembly assCR1 { get; set; }
        public Assembly assCL1 { get; set; }
    }
}
