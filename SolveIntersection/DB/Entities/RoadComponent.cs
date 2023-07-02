using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB.Entities.Beans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities
{
    public class RoadComponent
    {
        public Alignment alignment { get; set; }
        public Corridor corridor { get; set; }
        public AssemblyList assemblyList { get; set; }
        public Profile profile { get; set; }

        public RoadComponent()
        {
            assemblyList = new AssemblyList();
        }
    }
}
