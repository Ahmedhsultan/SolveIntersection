using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB.Entities;
using SolveIntersection.Service;
using Surface = Autodesk.Civil.DatabaseServices.Surface;

namespace SolveIntersection.EndPoint
{
    internal class AddProfileForRightTurnAL <T> where T : RightTurn
    {
        public IntersecionService intersectionService{ get; set; }

        public AddProfileForRightTurnAL(Transaction ts, CivilDocument civilDoc, T road)
        {
            this.intersectionService = new IntersecionService();

            // prepare the input parameters
            ObjectId layerId = road.alignment.LayerId;

            // let's get the 1st Profile style object in the DWG file
            ObjectId styleId = civilDoc.Styles.ProfileStyles[0];

            // let's get the 1st ProfileLabelSetStyle object in the DWG file
            ObjectId labelSetId = civilDoc.Styles.LabelSetStyles.ProfileLabelSetStyles[0];

            // Create the Profile Object
            ObjectId profileId = Profile.CreateByLayout("Profile_Created_using_API", road.alignment.ObjectId, layerId, styleId, labelSetId);
            Profile profile = ts.GetObject(profileId, OpenMode.ForWrite) as Profile;

            //Detect elevation
            double startElevation = getElevation(intersectionService.getMainRoad(), road.alignment.StartPoint);
            double endElevation = getElevation(intersectionService.getSecoundryRoad(), road.alignment.EndPoint);

            //Add start and end point to profile 
            Point3d startPoint = new Point3d(road.alignment.StartingStation, startElevation, 0);
            Point3d endPoint = new Point3d(road.alignment.EndingStation, endElevation, 0);
            profile.Entities.AddFixedTangent(startPoint, endPoint);
        }

        public double getElevation(Road road, Point3d rightturnPoint)
        {
            /*            double station = 0;
                        double offset = 0;

                        Alignment alingment = road.alignment;
                        alingment.StationOffset(rightturnPoint.X, rightturnPoint.Y, ref station, ref offset);

                        Profile profile = road.profile;
                        double elevCenterLine = profile.ElevationAt(station);

                        double dist = alingment.GetPointAtDist(station).DistanceTo(rightturnPoint);

                        foreach (Baseline baseline in road.corridor.Baselines)
                        {
                            if (baseline.AlignmentId == road.alignment.Id)
                            {
                                var featurlines = baseline.MainBaselineFeatureLines;
                                var var = featurlines.FeatureLineCollectionMap;
                                foreach (var item in featurlines)
                                {
                                    featurlines.
                                }
                            }
                        }*/

            double maxElev = double.MinValue;
            CorridorSurface maxSurface = null;
            foreach (var item in road.corridor.CorridorSurfaces)
            {
                double elev = item.FindElevationAtXY(rightturnPoint.X, rightturnPoint.Y);
                if(elev > maxElev)
                {
                    maxElev = elev;
                    maxSurface = item;
                }
            }

            if (maxElev == double.MinValue)
                throw new System.Exception("cant detect elevation of right turn");

            double elevEdge = maxElev;
            road.surfaceTop = maxSurface;

            return elevEdge;
        }
    }
}
