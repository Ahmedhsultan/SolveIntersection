using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB.Entities.Beans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.DB.Entities
{
    public class RightTurn : RoadComponent
    {
        public Meta_Data_Righturn meta_Data { get; set; }
        public Corridor corridor { get; set; }

        public RightTurn()
        {
            meta_Data = new Meta_Data_Righturn();
        }
    }
}
