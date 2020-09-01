using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace GtbTools
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ShowHideDock : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            App.Instance.Toggle(commandData);
            return Result.Succeeded;
        }
    }

    class ExternalEventMy : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            UIDocument uidoc = uiapp.ActiveUIDocument;
            if (null == uidoc)
            {
                return; // no document, nothing to do
            }
            Document doc = uidoc.Document;
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("MyEvent");
                // Action within valid Revit API context thread
                tx.Commit();
            }
            TaskDialog.Show("Info", "It's working");
        }
        public string GetName()
        {
            return "my event";
        }
    }
}
