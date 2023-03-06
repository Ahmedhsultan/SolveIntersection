using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Runtime;
using SolveIntersection.DB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.Servicies
{
    internal class DetectRoadComponents <T> where T : Road
    {
        public DetectRoadComponents(Transaction ts, Database database, CivilDocument civdoc, T road)
        {
            //Calculate road width
            double roadWidth = calculateRoadWidth(ts, road);
            road.meta_Data.width = roadWidth;

            //Detect slope
            road.meta_Data.rightSlop = calculateRoadSlope(ts, road, SubassemblySideType.Right);
            road.meta_Data.leftSlop = calculateRoadSlope(ts, road, SubassemblySideType.Left);

            //Detect longitudinalSlop
            road.meta_Data.longitudinalSlop = detectRoadLongitudinalSlop(ts, road);
        }

        public double detectRoadLongitudinalSlop(Transaction ts, T road)
        {
            //Side slope
            double slope = 0;

            

            return slope;
        }

        public double calculateRoadSlope(Transaction ts, T road, SubassemblySideType subassemblySideType)
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

        public double calculateRoadWidth(Transaction ts, T road)
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

/*      public Subassembly getFirstSubassembly(Assembly Assembly, Transaction ts)
        {
            Point3d assymbly_Origin = Assembly.Location;
            Subassembly firstSubassembly = null;
            foreach (AssemblyGroup assemblyGroup in Assembly.Groups)
            {
                foreach (ObjectId subassemblyid in assemblyGroup.GetSubassemblyIds())
                {
                    try
                    {
                        Subassembly subassembly = ts.GetObject(subassemblyid, OpenMode.ForWrite) as Subassembly;
                        if (assymbly_Origin == subassembly.Origin)
                            firstSubassembly = subassembly;
                    }
                    catch (Exception) { }
                }
            }
            return firstSubassembly;
        }*/
    }
}
