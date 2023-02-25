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
            Transaction ts = database.TransactionManager.StartOpenCloseTransaction();
            using (ts)
            {
                try
                {
                    /*BlockTable acBlkTbl = ts.GetObject(database.BlockTableId, OpenMode.ForRead) as BlockTable;
                    BlockTableRecord acBlkTblRec = ts.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;*/
                    #region CreateDB
                    IntersectionDB intersectionDB = IntersectionDB.getInstance();
                    #endregion
                    #region Selection
                    intersectionDB.corridor.corr1 = Select.cadEntity<Corridor>(ts, editor, "Select First Corridor");
                    intersectionDB.corridor.corr2 = Select.cadEntity<Corridor>(ts, editor, "Select Secound Corridor");

                    /*Polyline pl1 = Select.cadEntity<Polyline>(ts, editor, "Select right turn");
                    Polyline pl2 = Select.cadEntity<Polyline>(ts, editor, "Select right turn");
                    Polyline pl3 = Select.cadEntity<Polyline>(ts, editor, "Select right turn");
                    Polyline pl4 = Select.cadEntity<Polyline>(ts, editor, "Select right turn");*/
                    #endregion

                    #region Algorithm
                    CreateAssembly createAssembly = new CreateAssembly(ts, database);
                    #endregion
                }
                catch (System.Exception ex)
                {
                    #region HandelException
                    editor.WriteMessage(ex.Message);
                    ts.Abort();
                    #endregion
                }
                ts.Commit();
            }
        }
    }
}