using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace GtbTools
{
	//Shows and hides GTB Dock Panel
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ShowHideDock : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
        	ErrorLog errorLog = App.Instance.ErrorLog;
        	errorLog.WriteToLog("Changing DockPanel visibility state");
            App.Instance.Toggle(commandData, errorLog);
            return Result.Succeeded;
        }
    }
    
    //Opens a new window where the user can choose which views to open
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class OpenViews : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //OpenViewsTool here and Zoomwindow
            ErrorLog errorLog = App.Instance.ErrorLog;
        	errorLog.WriteToLog("Initiated open view tools...");
            OpenViewsTool openViewsTool = new OpenViewsTool(commandData.Application.ActiveUIDocument, errorLog);
            openViewsTool.CreateModelViewList();
            
            ZoomWindow zoomWindow = new ZoomWindow(openViewsTool);
            if(openViewsTool.WindowResult == WindowResult.UserApply) openViewsTool.OpenViews();
            if(openViewsTool.CloseInactive == true) openViewsTool.CloseInactiveViews();
            ViewCoordsTool viewCoordsTool = new ViewCoordsTool(commandData.Application.ActiveUIDocument);
            viewCoordsTool.ApplyCoordsToViews();
            
            return Result.Succeeded;
        }
    }
    
    //Loads coordinates from file
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class LoadCoords : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //ViewCoordsTool here
            ViewCoordsTool vct = new ViewCoordsTool(commandData.Application.ActiveUIDocument);
			vct.LoadCoordinatesFrom();
            return Result.Succeeded;
        }
    }
    
    //Saves coordinates to file
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SaveCoords : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //ViewCoordsTool here
            ViewCoordsTool vct = new ViewCoordsTool(commandData.Application.ActiveUIDocument);
			vct.SaveCurrentCoordinatesAs();
            return Result.Succeeded;
        }
    }
    
    //Copy current coordinates to all open views
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class CopyCoords : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //ViewCoordsTool here
            ViewCoordsTool vct = new ViewCoordsTool(commandData.Application.ActiveUIDocument);
			vct.ApplyCoordsToViews();
            return Result.Succeeded;
        }
    }

    class ExternalEventMy : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ViewCoordsTool vct = new ViewCoordsTool(uiapp.ActiveUIDocument);
            vct.ApplyCoordsToViews();
            TaskDialog.Show("info", "OK");
        }
        public string GetName()
        {
            return "1";
        }
    }

    class ExternalEventMy2 : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Initiated open view tools...");
            OpenViewsTool openViewsTool = new OpenViewsTool(uiapp.ActiveUIDocument, errorLog);
            openViewsTool.CreateModelViewList();

            ZoomWindow zoomWindow = new ZoomWindow(openViewsTool);
            zoomWindow.ShowDialog();
            if (openViewsTool.WindowResult == WindowResult.UserApply) openViewsTool.OpenViews();
            if (openViewsTool.CloseInactive == true) openViewsTool.CloseInactiveViews();
            ViewCoordsTool viewCoordsTool = new ViewCoordsTool(uiapp.ActiveUIDocument);
            viewCoordsTool.ApplyCoordsToViews();
            TaskDialog.Show("info", "OK2");
        }
        public string GetName()
        {
            return "1";
        }
    }
}
