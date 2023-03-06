#region Liberary
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB;
using SolveIntersection.Servicies;
using SolveIntersection.Util;
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
                    IntersectionDB intersectionDB = IntersectionDB.getInstance();
                    #endregion
                    #region Selection
                    intersectionDB.corridor.corr1 = Select.cadEntity<Corridor>(ts, editor, "Select First Corridor");
                    intersectionDB.corridor.corr2 = Select.cadEntity<Corridor>(ts, editor, "Select Secound Corridor");

                    intersectionDB.polylinesDB.polyline1 = Select.cadEntity<Polyline>(ts, editor, "Select right turn 1");
                    intersectionDB.polylinesDB.polyline2 = Select.cadEntity<Polyline>(ts, editor, "Select right turn 2");
                    /*intersectionDB.polylinesDB.polyline3 = Select.cadEntity<Polyline>(ts, editor, "Select right turn 3");
                    intersectionDB.polylinesDB.polyline4 = Select.cadEntity<Polyline>(ts, editor, "Select right turn 4");*/
                    #endregion

                    #region Algorithm
                    CreateAssembly createAssembly = new CreateAssembly(ts, database);

                    GetRoadsAlignmentsFromCorridors getAlignmentFromCorridor = new GetRoadsAlignmentsFromCorridors(
                        ts, editor,
                        intersectionDB.corridor.corr1,
                        intersectionDB.corridor.corr2);

                    CreateRightTurnAlignments createRightTurnAlignments = new CreateRightTurnAlignments(ts, database, civilDocument);

                    AddProfileForRightTurnAL addProfileForRightTurnAL = new AddProfileForRightTurnAL(ts, civilDocument);

                    CutMainRoadCorridor cutMainRoadCorridor = new CutMainRoadCorridor(ts, civilDocument);

                    CreateRightTurnCorridors createRightTurnCorridors = new CreateRightTurnCorridors(ts, civilDocument);
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