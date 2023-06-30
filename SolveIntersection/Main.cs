#region Liberary
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB;
using SolveIntersection.DB.Entities;
using SolveIntersection.Servicies;
using SolveIntersection.Util;
using SolveIntersection.DB.Entities.Beans;
#endregion

namespace SolveIntersection
{
    public class Main
    {
        [CommandMethod("dar_solveIntersection")]
        public static void main()
        {
            #region Documents
            Database database = Application.DocumentManager.MdiActiveDocument.Database;     //Defination
            CivilDocument civilDocument = CivilApplication.ActiveDocument;
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor editor = doc.Editor;
            #endregion
            using (Transaction ts = database.TransactionManager.StartOpenCloseTransaction())
            {
                try
                {
                    #region CreateDB
                    IntersectionDB.clearInstance();
                    IntersectionDB intersectionDB = IntersectionDB.getInstance();
                    #endregion
                    #region Selection
                    intersectionDB.road_Main.corridor = Select.cadEntity<Corridor>(ts, editor, "Select Main Road Corridor");
                    intersectionDB.road_Secondary.corridor = Select.cadEntity<Corridor>(ts, editor, "Select Secondary Road Corridor");

                    intersectionDB.selection.polyline1 = Select.cadEntity<Polyline>(ts, editor, "Select right turn 1");
                    intersectionDB.selection.polyline2 = Select.cadEntity<Polyline>(ts, editor, "Select right turn 2");
                    #endregion

                    #region Algorithm
                    new CreateAssembly<Road_Main>(ts, database, intersectionDB.road_Main);
                    new CreateAssembly<Road_Secondary>(ts, database, intersectionDB.road_Secondary);

                    new GetRoadsAlignmentsFromCorridors(ts, editor, intersectionDB.road_Main.corridor, intersectionDB.road_Secondary.corridor);

                    new DetectRoadComponents<Road_Main>(ts, database, civilDocument, intersectionDB.road_Main);
                    new DetectRoadComponents<Road_Secondary>(ts, database, civilDocument, intersectionDB.road_Secondary);

                    new CreateRightTurnAlignments(ts, database, civilDocument);

                    new AddProfileForRightTurnAL<RightTurn1>(ts, civilDocument, intersectionDB.rightTurn1);
                    new AddProfileForRightTurnAL<RightTurn2>(ts, civilDocument, intersectionDB.rightTurn2);

                    new CutMainRoadCorridor(ts, civilDocument);

                    new CreateRightTurnCorridors<RightTurn1>(ts, civilDocument, intersectionDB.rightTurn1, IntersectionDB.getInstance().road_Secondary.assemblyList.assCL1);
                    new CreateRightTurnCorridors<RightTurn2>(ts, civilDocument, intersectionDB.rightTurn2, IntersectionDB.getInstance().road_Secondary.assemblyList.assCR1);
                    #endregion

                    ts.Commit();
                }
                catch (System.Exception ex)
                {
                    #region HandelException
                    editor.WriteMessage(ex.Message);
                    ts.Abort();
                    #endregion
                }
            }
        }
    }
}