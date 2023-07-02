﻿#region Liberary
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB;
using SolveIntersection.DB.Entities;
using SolveIntersection.DTO;
using SolveIntersection.EndPoint;
using SolveIntersection.Util;
using System.Collections.Generic;
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
                    intersectionDB.road_Main.corridor = Select.entity<Corridor>(ts, editor, "Select Main Road Corridor");
                    intersectionDB.road_Secondary.corridor = Select.entity<Corridor>(ts, editor, "Select Secondary Road Corridor");

                    intersectionDB.road_Main.baselineRegion = Select.region("Select Main Road Corridor");
                    intersectionDB.road_Secondary.baselineRegion = Select.region("Select Secondary Road Corridor");

                    intersectionDB.selection.polyline1 = Select.entity<Polyline>(ts, editor, "Select right turn 1");
                    intersectionDB.selection.polyline2 = Select.entity<Polyline>(ts, editor, "Select right turn 2");
                    #endregion

                    #region Algorithm
                    new CreateAssembly(ts, database, intersectionDB.road_Main);
                    new CreateAssembly(ts, database, intersectionDB.road_Secondary);

                    new GetRoadsAlignmentsFromCorridors(ts, editor, intersectionDB.road_Main.corridor, intersectionDB.road_Secondary.corridor);

                    var createRightTurnAlignments = new CreateRightTurnAlignments(ts, database, civilDocument);

                    new DetectRoadComponents(ts, database, civilDocument, intersectionDB.road_Main, intersectionDB.rightTurn_Right);
                    new DetectRoadComponents(ts, database, civilDocument, intersectionDB.road_Secondary, intersectionDB.rightTurn_Right);

                    createRightTurnAlignments.adjustAlignmnetLength(ts, intersectionDB.rightTurn_Right);
                    createRightTurnAlignments.adjustAlignmnetLength(ts, intersectionDB.rightTurn_Left);

                    new AddProfileForRightTurnAL<RightTurn>(ts, civilDocument, intersectionDB.rightTurn_Right);
                    new AddProfileForRightTurnAL<RightTurn>(ts, civilDocument, intersectionDB.rightTurn_Left);

                    new CutMainRoadCorridor(ts, civilDocument);

                    new CreateRightTurnCorridors<RightTurn>(ts, civilDocument, intersectionDB.rightTurn_Right, intersectionDB.road_Secondary.assemblyList.assCL1);
                    new CreateRightTurnCorridors<RightTurn>(ts, civilDocument, intersectionDB.rightTurn_Left, intersectionDB.road_Secondary.assemblyList.assCR1);
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