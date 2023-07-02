using SolveIntersection.DB.Entities.Beans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities
{
    public class Road : RoadComponent
    {
        public Meta_Data_Road meta_Data { get; set; }

        public Road()
        {
            meta_Data = new Meta_Data_Road();
        }
    }
}
