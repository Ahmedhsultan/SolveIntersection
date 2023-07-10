using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.DatabaseServices.Filters;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Settings;
using SolveIntersection.DB;
using SolveIntersection.DB.Entities;
using SolveIntersection.Service;
using System;

namespace SolveIntersection.EndPoint
{
    internal class CreateRightTurnAlignments
    {
        public IntersecionService intersectionService { get; set; }

        public CreateRightTurnAlignments(Transaction ts, Database database, CivilDocument civdoc)
        {
            this.intersectionService = new IntersecionService();

            //Add to database
            Polyline polyline1 = intersectionService.getSelection().polyline1;
            Polyline polyline2 = intersectionService.getSelection().polyline2;

            //Create alignment to rightturn
            Alignment alignment1 = createAlignment(ts, database, civdoc, polyline1);
            Alignment alignment2 = createAlignment(ts, database, civdoc, polyline2);

            //Make start of alignment parallel to the secoundary road so it will start from the secoundry road 
            adjestDirection(ts, alignment1);
            adjestDirection(ts, alignment2);

            //Add to database
            //Calculate crossproduct between start vector of rightturn1 and first point in rightturn2 to detect if on right or left by z sign
            Vector3d vectorStartPointRightTurn1 = alignment1.StartPoint.GetVectorTo(alignment1.GetPointAtDist(0.1));
            Point3d p1 = alignment1.StartPoint;
            Point3d p2 = alignment2.StartPoint;
            Vector3d vectorOfPoint = new Vector3d(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);

            Vector3d crossProduct = vectorStartPointRightTurn1.CrossProduct(vectorOfPoint);
            if (crossProduct.Z > 0) // secound alignment is on left
            {
                intersectionService.getRightTurn_RighSide().alignment = alignment1;
                intersectionService.getRightTurn_LeftSide().alignment = alignment2;
            }
            else if (crossProduct.Z < 0) // secound alignment is on right
            {
                intersectionService.getRightTurn_RighSide().alignment = alignment2;
                intersectionService.getRightTurn_LeftSide().alignment = alignment1;
            }
            IntersectionDB.getInstance().rightTurn_Right.meta_Data.side = DB.Entities.Beans.Side.RIGHT;
            IntersectionDB.getInstance().rightTurn_Left.meta_Data.side = DB.Entities.Beans.Side.LEFT;

            //Reverse alignment again
            reverseAlignment(ts, alignment1.Id);
            reverseAlignment(ts, alignment2.Id);

            ts.Commit();
        }

        public void adjustAlignmnetLength(Transaction trans, RightTurn rightTurn)
        {
            Road secoundryRoad = intersectionService.getSecoundryRoad();

            double length = detectAlignmentLength(secoundryRoad, rightTurn);
            Alignment alignment = trans.GetObject(rightTurn.alignment.Id, OpenMode.ForWrite) as Alignment;
            IntegerCollection i = new IntegerCollection() { 0};
            alignment.MoveGripPointsAt(i , new Vector3d(0,5,0));
            trans.Commit();
            foreach (var entity in alignment.Entities)
            {
                if (entity.EntityType == AlignmentEntityType.Line)
                {
                    AlignmentLine l = (AlignmentLine)entity;
                    //l.Length = 0.2;
                }
            }
        }

        public double detectAlignmentLength(Road secoundryRoad, RightTurn rightTurn)
        {
            /*double mainRoadSlope = IntersectionDB.getInstance().road_Main.longitudinalSlop;
            double sideSlope = 0;
            switch (rightTurn.meta_Data.side)
            {
                case DB.Entities.Beans.Side.RIGHT:
                    switch (secoundryRoad.meta_Data.direction)
                    {
                        case DB.Entities.Beans.Direction.FORWORD: sideSlope = secoundryRoad.meta_Data.rightSlop; break;
                        case DB.Entities.Beans.Direction.BACKWORD: sideSlope = secoundryRoad.meta_Data.leftSlop; break;
                    }
                    break;
                case DB.Entities.Beans.Side.LEFT:
                    switch (secoundryRoad.meta_Data.direction)
                    {
                        case DB.Entities.Beans.Direction.FORWORD: sideSlope = secoundryRoad.meta_Data.leftSlop; break;
                        case DB.Entities.Beans.Direction.BACKWORD: sideSlope = secoundryRoad.meta_Data.rightSlop; break;
                    }
                    break;
                default: throw new Exception("The right turn side isnt assigned");
            }
            double length = ((mainRoadSlope*100) - (sideSlope*100)) * 10;

            return length;*/
            return 0;
        }

        public Alignment createAlignment(Transaction ts, Database database, CivilDocument civdoc, Polyline polyline)
        {
            // Set options
            PolylineOptions plos = new PolylineOptions();
            plos.AddCurvesBetweenTangents = true;
            plos.EraseExistingEntities = true;
            plos.PlineId = polyline.ObjectId;

            // Get id of layer "alignments" or use id of current layer
            ObjectId idLayer = database.Clayer;
            LayerTable lt = ts.GetObject(database.LayerTableId, OpenMode.ForRead) as LayerTable;
            if (lt.Has("alignments")) idLayer = (lt["alignments"]);


            // Get id  of the 1st Alignment style or Basic
            ObjectId idStyle = civdoc.Styles.AlignmentStyles[0];
            if (civdoc.Styles.AlignmentStyles.Contains("Basic")) idStyle = civdoc.Styles.AlignmentStyles["Basic"];

            // Get id of the 1st AlignmentLabelSetStyle or Basic
            ObjectId idLabelSet = civdoc.Styles.LabelSetStyles.AlignmentLabelSetStyles[0];
            if (civdoc.Styles.LabelSetStyles.AlignmentLabelSetStyles.Contains("Basic")) idLabelSet = civdoc.Styles.LabelSetStyles.AlignmentLabelSetStyles["Basic"];

            // Create unique name
            String nAlign = Alignment.GetNextUniqueName(civdoc.Settings.GetSettings<SettingsCmdCreateAlignmentEntities>().DefaultNameFormat.AlignmentNameTemplate.Value);
            // Create alignment
            ObjectId idAlign = Alignment.Create(civdoc, plos, nAlign, ObjectId.Null, idLayer, idStyle, idLabelSet);

            Alignment alignment = ts.GetObject(idAlign, OpenMode.ForRead) as Alignment;

            return alignment;
        }

        private void adjestDirection(Transaction trans, Alignment alignment)
        {
            //Get alginment of right turn and secoundary road
            Alignment alignmentSecoundryRoad = IntersectionDB.getInstance().road_Secondary.alignment;

            //Get vector of start secoundary road and start-end right turn road
            Vector3d vectorAlignmentSecoundryRoad = alignmentSecoundryRoad.StartPoint.GetAsVector();
            Vector3d vectorRightTurnStart = alignment.StartPoint.GetAsVector();
            Vector3d vectorRightTurnEnd = alignment.EndPoint.GetAsVector();

            //Adjust vectors to be parallel to secoundry road 
            if (vectorRightTurnStart.GetAngleTo(vectorAlignmentSecoundryRoad) > 2.967)
                vectorRightTurnStart = vectorRightTurnStart.Negate();
            if (vectorRightTurnEnd.GetAngleTo(vectorAlignmentSecoundryRoad) > 2.967)
                vectorRightTurnEnd = vectorRightTurnEnd.Negate();

            //Check if andgel of start vector bigger than end vector so its start prependacular and end is parallel and we need to reverse alignment
            if (vectorRightTurnStart.GetAngleTo(vectorAlignmentSecoundryRoad) > vectorRightTurnEnd.GetAngleTo(vectorAlignmentSecoundryRoad))
                reverseAlignment(trans, alignment.Id);
        }

        public void reverseAlignment(Transaction trans, ObjectId alignmentId)
        {
            Alignment alignment = trans.GetObject(alignmentId, OpenMode.ForWrite) as Alignment;
            alignment.Reverse();

            trans.Commit();
        }

        /*private void adjestDirection(Transaction trans, Database database, ObjectId alignmentId)
        {
            Alignment alignment = trans.GetObject(alignmentId, OpenMode.ForWrite) as Alignment;

            Alignment alignmentSecoundryRoad = IntersectionDB.getInstance().road_Secondary.alignment;
            Plane plane = new Plane(alignmentSecoundryRoad.StartPoint, alignmentSecoundryRoad.StartPoint.GetAsVector().GetNormal());

            Point3d startPointAlignmentSecoundryRoad = alignmentSecoundryRoad.StartPoint;
            Vector3d vectorAlignmentSecoundryRoad = alignmentSecoundryRoad.StartPoint.GetAsVector();

            Point3d startPointAlignmentRightTurn = alignment.StartPoint;
            Point3d projectedStartPoint = projectPoint(startPointAlignmentRightTurn, startPointAlignmentSecoundryRoad, vectorAlignmentSecoundryRoad);
            double stationStartPoint = startPointAlignmentSecoundryRoad.DistanceTo(projectedStartPoint);

            Point3d endPointAlignmentRightTurn = alignment.EndPoint;
            Point3d projectedEndPoint = projectPoint(endPointAlignmentRightTurn, startPointAlignmentSecoundryRoad, vectorAlignmentSecoundryRoad);
            double stationEndPoint = startPointAlignmentSecoundryRoad.DistanceTo(projectedEndPoint);

            if (stationStartPoint > stationEndPoint)
                alignment.Reverse();

            trans.Commit();
            trans = database.TransactionManager.StartOpenCloseTransaction();
        }*/

        /*public Point3d projectPoint(Point3d point, Point3d startVector, Vector3d vector)
        {
            double t = (vector.DotProduct(point - startVector)) / vector.DotProduct(vector);
            Point3d projectedPoint = startVector + vector.MultiplyBy(t);

            return projectedPoint;
        }*/
    }
}
