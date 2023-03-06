using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB.Entities.Beans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities
{
    internal class Road
    {
        public Alignment alignment { get; set; }
        public Corridor corridor { get; set; }
        public AssemblyList assemblyList { get; set; }
        public Meta_Data meta_Data { get; set; }
        public Profile profile { get; set; }

        public Road()
        {
            assemblyList = new AssemblyList();
            meta_Data = new Meta_Data();
        }
    }
}
