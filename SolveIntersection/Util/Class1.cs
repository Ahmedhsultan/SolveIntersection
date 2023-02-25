/*using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;


namespace SolveIntersection.Util
{
    // Define a selection filter to select regions within a corridor
    public class CorridorRegionFilter : SelectionFilter
    {
        private Corridor _corridor;

        public CorridorRegionFilter(Corridor corridor)
        {
            _corridor = corridor;
        }

        public override bool AllowEntity(Autodesk.AutoCAD.DatabaseServices.Entity entity)
        {
            if (entity is Region)
            {
                Region region = entity as Region;
                if (region.CorridorId == _corridor.ObjectId)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool AllowPickObject(PickObjectOption pickOptions, PickPointFilter pointFilter, PromptStatus status, Point3d pickPoint, Entity ent, SelectionFilter selFilter, out ObjectSelectionMethod objectSelectionMethod)
        {
            objectSelectionMethod = ObjectSelectionMethod.Window;
            return AllowEntity(ent);
        }
    }
}
*/