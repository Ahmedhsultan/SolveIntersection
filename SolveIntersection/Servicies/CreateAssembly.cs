using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Runtime;
using SolveIntersection.DB;
using System;

namespace SolveIntersection.Servicies
{
    internal class CreateAssembly
    {
        public int displacement { get; set; }
        public CreateAssembly(Transaction ts, Database database)
        {
            //Get Database
            IntersectionDB intersectionDB = IntersectionDB.getInstance();

            foreach (var baseline in intersectionDB.corridor.corr1.Baselines)
            {
                foreach (var region in baseline.BaselineRegions)
                {
                    Assembly assbly = ts.GetObject(region.AssemblyId, OpenMode.ForRead) as Assembly;

                    //Create R1
                    Assembly copiedAssemblyRight = copyAssembly(assbly, ts, "Right", database);
                    deleteSide(copiedAssemblyRight, SubassemblySideType.Left, ts, database);
                    intersectionDB.assembly.assR1 = copiedAssemblyRight;

                    //Create L1
                    Assembly copiedAssemblyLeft = copyAssembly(assbly, ts, "Left", database);
                    deleteSide(copiedAssemblyLeft, SubassemblySideType.Right, ts, database);
                    intersectionDB.assembly.assL1 = copiedAssemblyLeft;

                    //Create CL1
                    Assembly copiedAssemblyCutLeft = copyAssembly(assbly, ts, "RightTurn", database);
                    deleteSide(copiedAssemblyCutLeft, SubassemblySideType.Right, ts, database);
                    mirrorPavement(copiedAssemblyCutLeft, ts, database);
                    intersectionDB.assembly.assCL1 = copiedAssemblyCutLeft;

                    //Create CR1
                    Assembly copiedAssemblyCutRight = copyAssembly(assbly, ts, "LeftTurn", database);
                    deleteSide(copiedAssemblyCutRight, SubassemblySideType.Left, ts, database);
                    mirrorPavement(copiedAssemblyCutRight, ts, database);
                    intersectionDB.assembly.assCR1 = copiedAssemblyCutRight;
                }
            }
        }

        public void mirrorPavement (Assembly copiedAssembly, Transaction ts, Database database)
        {
            ts.GetObject(copiedAssembly.ObjectId, OpenMode.ForWrite);
            Subassembly firstSubassembly = getFirstSubassembly(copiedAssembly, ts, database);
            foreach (AssemblyGroup assemblyGroup in copiedAssembly.Groups)
            {
                foreach (ObjectId subassemblyid in assemblyGroup.GetSubassemblyIds())
                {
                    try
                    {
                        Subassembly subassembly = ts.GetObject(subassemblyid, OpenMode.ForWrite) as Subassembly;
                        if (subassembly.Name == firstSubassembly.Name)
                        {
                            //Reverse slope
                            ParamDoubleCollection paramsDouble = subassembly.ParamsDouble;
                            ParamDouble slopeKey = paramsDouble["DefaultSlope"];
                            slopeKey.Value *= (-1);

                            AssemblyGroup assemblyGroupMirrord = copiedAssembly.MirrorSubassembly(subassemblyid);

                            subassembly.Erase();
                        }
                        else
                        {
                            break;
                        }
                    }catch (Exception e) {}
                }
            }
        }
        public Subassembly getFirstSubassembly(Assembly copiedAssembly, Transaction ts, Database database)
        {
            Point3d assymbly_Origin = copiedAssembly.Location;
            Subassembly firstSubassembly = null;
            foreach (AssemblyGroup assemblyGroup in copiedAssembly.Groups)
            {
                foreach (ObjectId subassemblyid in assemblyGroup.GetSubassemblyIds())
                {
                    try
                    {
                        Subassembly subassembly = ts.GetObject(subassemblyid, OpenMode.ForWrite) as Subassembly;
                        if (assymbly_Origin == subassembly.Origin)
                            firstSubassembly = subassembly;
                    }
                    catch (Exception){}
                }
            }
            return firstSubassembly;
        }

        public void deleteSide(Assembly copiedAssembly, SubassemblySideType side, Transaction ts, Database database)
        {
            foreach (AssemblyGroup assemblyGroup in copiedAssembly.Groups)
            {
                foreach (ObjectId subassemblyid in assemblyGroup.GetSubassemblyIds())
                {
                    Subassembly subassembly = ts.GetObject(subassemblyid, OpenMode.ForWrite) as Subassembly;
                    if (subassembly.Side == side)
                        if (subassembly != null) 
                            subassembly.Erase();
                }
            }
            ts.Commit();
            ts = database.TransactionManager.StartOpenCloseTransaction();
        }

        public Assembly copyAssembly(Assembly assembly, Transaction ts, String name, Database database)
        {
            // make a deep copy of the main assembly and all its referenced subassemblies
            IdMapping mapping = new IdMapping();
            database.DeepCloneObjects(new ObjectIdCollection() { assembly.ObjectId }, database.CurrentSpaceId, mapping, false);

            Assembly copiedAss = ts.GetObject(mapping.Lookup(assembly.ObjectId).Value, OpenMode.ForWrite) as Assembly;
            copiedAss.Name = assembly.Name + " (" + name + ")";

            // Move the assembly geometry
            displacement += 3;
            Matrix3d mirrorMatrix = Matrix3d.Displacement(new Vector3d(0, -displacement, 0));
            copiedAss.TransformBy(mirrorMatrix);

            return copiedAss;
        }
    }
}
