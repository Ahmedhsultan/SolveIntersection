using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.EndPoint
{
    internal class CutMainRoadCorridor
    {
        public CutMainRoadCorridor(Transaction ts, CivilDocument civilDocument)
        {
            Corridor corridor = IntersectionDB.getInstance().road_Main.corridor;
            Alignment alignment = IntersectionDB.getInstance().road_Main.alignment;

            Alignment rightTurn = IntersectionDB.getInstance().rightTurn_Right.alignment;
            Alignment leftTurn = IntersectionDB.getInstance().rightTurn_Left.alignment;

            /*foreach (Baseline baseline in corridor.Baselines)
                if(baseline.AlignmentId == alignment.Id)
                {
                    foreach (BaselineRegion region in baseline.BaselineRegions)
                    {
                        double startSt = region.StartStation;
                        double endSt = region.EndStation;
                        if ((startSt > station && endSt < station) || (startSt < station && endSt > station))
                        {
                            region.Split(station);
                        }
                    }
                }*/
        }
    }
}
