using SolveIntersection.DB;
using SolveIntersection.DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.Service
{
    public class IntersecionService
    {
        public IntersectionDB intersectionDB { get; set; }

        public IntersecionService()
        {
            this.intersectionDB = IntersectionDB.getInstance();
        }

        public Road getMainRoad()
        {
            var entity = this.intersectionDB.road_Main;
            if (entity == null) throw new Exception();
            return entity;
        }
        public Road getSecoundryRoad()
        {
            var entity = this.intersectionDB.road_Secondary;
            if (entity == null) throw new Exception();
            return entity;
        }
        public RightTurn getRightTurn_RighSide()
        {
            var entity = this.intersectionDB.rightTurn_Right;
            if (entity == null) throw new Exception();
            return entity;
        }
        public RightTurn getRightTurn_LeftSide()
        {
            var entity = this.intersectionDB.rightTurn_Left;
            if (entity == null) throw new Exception();
            return entity;
        }
        public Selection getSelection()
        {
            var entity = this.intersectionDB.selection;
            if (entity == null) throw new Exception();
            return entity;
        }
    }
}
