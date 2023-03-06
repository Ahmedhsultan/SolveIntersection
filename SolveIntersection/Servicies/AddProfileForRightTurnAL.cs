using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SolveIntersection.DB;
using SolveIntersection.DB.Entities;

namespace SolveIntersection.Servicies
{
    internal class AddProfileForRightTurnAL <T> where T : Road
    {
        public AddProfileForRightTurnAL(Transaction ts, CivilDocument civilDoc, T road)
        {
            // prepare the input parameters
            ObjectId layerId = road.alignment.LayerId;

            // let's get the 1st Profile style object in the DWG file
            ObjectId styleId = civilDoc.Styles.ProfileStyles[0];

            // let's get the 1st ProfileLabelSetStyle object in the DWG file
            ObjectId labelSetId = civilDoc.Styles.LabelSetStyles.ProfileLabelSetStyles[0];

            // Create the Profile Object
            ObjectId profileId = Profile.CreateByLayout("Profile_Created_using_API", road.alignment.ObjectId, layerId, styleId, labelSetId);
            Profile profile = ts.GetObject(profileId, OpenMode.ForWrite) as Profile;

            Point3d startPoint = new Point3d(road.alignment.StartingStation, -40, 0);
            Point3d endPoint = new Point3d(758.2, -70, 0);
            ProfileTangent oTangent1 = profile.Entities.AddFixedTangent(startPoint, endPoint);


            //profile.Entities.AddFreeSymmetricParabolaByLength(oTangent1.EntityId, oTangent2.EntityId, VerticalCurveType.Sag, 900.1, true);
        }
    }
}
