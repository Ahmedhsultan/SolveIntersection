using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Runtime;
using SolveIntersection.DB;
using SolveIntersection.DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.EndPoint
{
    internal class DetectRoadComponents
    {
        public Road road { get; set; }
        public RightTurn rightTurn_RightSide { get; set; }

        public DetectRoadComponents(Transaction ts, Database database, CivilDocument civdoc, Road road, RightTurn rightTurn_RightSide)
        {
            this.road = road;
            this.rightTurn_RightSide = rightTurn_RightSide;

            //Calculate road width
            double roadWidth = calculateRoadWidth(ts);
            road.meta_Data.width = roadWidth;

            //Detect road direction
            detectRoadDirection();

            //Detect slope
            road.meta_Data.rightSlop = calculateRoadSideSlope(ts, SubassemblySideType.Right);
            road.meta_Data.leftSlop = calculateRoadSideSlope(ts, SubassemblySideType.Left);

            //Detect longitudinalSlop
            road.meta_Data.longitudinalSlop = detectRoadLongitudinalSlop(ts);
        }

        public void detectRoadDirection()
        {
            Point3d startPoint = road.alignment.StartPoint;
            Point3d endPoint = road.alignment.EndPoint;

            Vector3d roadVector = new Vector3d(endPoint.X - startPoint.X, endPoint.Y - startPoint.Y, endPoint.Z - startPoint.Z);

            Point3d p1 = road.alignment.StartPoint;
            Point3d p2 = rightTurn_RightSide.alignment.StartPoint;
            Vector3d vectorOfPoint = new Vector3d(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);

            Vector3d crossProduct = roadVector.CrossProduct(vectorOfPoint);

            if (crossProduct.Z > 0) // rightturn is on left
                road.meta_Data.direction = DB.Entities.Beans.Direction.BACKWORD;
            else if (crossProduct.Z < 0) // rightturn is on right
                road.meta_Data.direction = DB.Entities.Beans.Direction.FORWORD;
        }

        public double detectRoadLongitudinalSlop(Transaction ts)
        {
            //Side slope
            double slope = 0;

            

            return slope;
        }

        public double calculateRoadSideSlope(Transaction ts, SubassemblySideType subassemblySideType)
        {
            //Side slope
            double slope = 0;

            //Detect first subassembly
            ObjectId firstSubassemblyId = road.assemblyList.mainAss.Groups.ElementAt(0).GetSubassemblyIds()[0];
            Subassembly firstSubassembly = ts.GetObject(firstSubassemblyId, OpenMode.ForWrite) as Subassembly;

            //Detect all sameller subassemblies
            foreach (AssemblyGroup assemblyGroup in road.assemblyList.mainAss.Groups)
            {
                foreach (ObjectId subassemblyid in assemblyGroup.GetSubassemblyIds())
                {
                    try
                    {
                        Subassembly subassembly = ts.GetObject(subassemblyid, OpenMode.ForWrite) as Subassembly;
                        if (subassembly.Name == firstSubassembly.Name && subassembly.Side == subassemblySideType)
                        {
                            //Reverse slope
                            ParamDoubleCollection paramsDouble = subassembly.ParamsDouble;
                            ParamDouble slopeKey = paramsDouble["sideSlope"];
                            slope = slopeKey.Value;
                            break;
                        }
                    }
                    catch (Exception e) { }
                }
            }

            return slope;
        }

        public double calculateRoadWidth(Transaction ts)
        {
            //Road width
            double width = 0;

            //Detect first subassembly
            ObjectId firstSubassemblyId = road.assemblyList.mainAss.Groups.ElementAt(0).GetSubassemblyIds()[0];
            Subassembly firstSubassembly = ts.GetObject(firstSubassemblyId, OpenMode.ForWrite) as Subassembly;

            //Detect all sameller subassemblies
            foreach (AssemblyGroup assemblyGroup in road.assemblyList.mainAss.Groups)
            {
                foreach (ObjectId subassemblyid in assemblyGroup.GetSubassemblyIds())
                {
                    try
                    {
                        Subassembly subassembly = ts.GetObject(subassemblyid, OpenMode.ForWrite) as Subassembly;
                        if (subassembly.Name == firstSubassembly.Name && subassembly.Side == firstSubassembly.Side)
                        {
                            //Reverse slope
                            ParamDoubleCollection paramsDouble = subassembly.ParamsDouble;
                            ParamDouble widthKey = paramsDouble["Width"];
                            width += widthKey.Value;
                        }
                        else
                        {
                            width *= 2;
                            break;
                        }
                    }
                    catch (Exception e) { }
                }
            }

            return width;
        }
    }
}
