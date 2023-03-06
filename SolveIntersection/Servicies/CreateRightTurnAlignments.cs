using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.Settings;
using SolveIntersection.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolveIntersection.Servicies
{
    internal class CreateRightTurnAlignments
    {
        public CreateRightTurnAlignments(Transaction ts, Database database, CivilDocument civdoc)
        {
            Polyline polyline1 = IntersectionDB.getInstance().polylinesDB.polyline1;
            Polyline polyline2 = IntersectionDB.getInstance().polylinesDB.polyline2;

            Alignment alignment1 = createAlignment(ts, database, civdoc, polyline1);
            Alignment alignment2 = createAlignment(ts, database, civdoc, polyline2);

            IntersectionDB.getInstance().alignment.al1R = alignment1;
            IntersectionDB.getInstance().alignment.al1L = alignment2;

            ts.Commit();
            ts = database.TransactionManager.StartOpenCloseTransaction();
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
    }
}
