using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using System;

namespace SolveIntersection.Util
{
    internal class Select
    {
        public static T c3dEntity<T>(Transaction ts, Editor editor, String msg) where T : Autodesk.Civil.DatabaseServices.Entity
        {
            PromptEntityOptions options = new PromptEntityOptions(msg);
            options.SetRejectMessage("\nNot a valid Type. Please try again.");
            options.AddAllowedClass(typeof(T), false);
            PromptEntityResult sourcePrompt = editor.GetEntity(options);
            if (sourcePrompt.Status != PromptStatus.OK)
            {
                ts.Abort();
                return null;
            }
            T entity = ts.GetObject(sourcePrompt.ObjectId, OpenMode.ForWrite) as T;

            return entity;
        }
        public static T cadEntity<T>(Transaction ts, Editor editor, String msg) where T : Autodesk.AutoCAD.DatabaseServices.Entity
        {
            PromptEntityOptions options = new PromptEntityOptions(msg);
            options.SetRejectMessage("\nNot a valid Type. Please try again.");
            options.AddAllowedClass(typeof(T), false);
            PromptEntityResult sourcePrompt = editor.GetEntity(options);
            if (sourcePrompt.Status != PromptStatus.OK)
            {
                ts.Abort();
                return null;
            }
            T entity = ts.GetObject(sourcePrompt.ObjectId, OpenMode.ForWrite) as T;

            return entity;
        }
    }
}
