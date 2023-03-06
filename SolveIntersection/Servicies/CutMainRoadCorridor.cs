using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.Servicies
{
    internal class CutMainRoadCorridor
    {
        public CutMainRoadCorridor(Transaction ts, CivilDocument civilDocument)
        {
            Corridor corridor = IntersectionDB.getInstance().road_Main.corridor;
            Alignment alignment = IntersectionDB.getInstance().road_Main.alignment;

            Alignment rightTurn = IntersectionDB.getInstance().rightTurn1.alignment;
            Alignment leftTurn = IntersectionDB.getInstance().rightTurn2.alignment;

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
