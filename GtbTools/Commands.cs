using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GtbTools.GUI;

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
            App.Instance.Toggle(commandData);
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

    class ExternalEventApplyCoordsToViews : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ViewCoordsTool vct = new ViewCoordsTool(uiapp.ActiveUIDocument);
            vct.ApplyCoordsToViews();
        }
        public string GetName()
        {
            return "Applied coords to views";
        }
    }

    class ExternalEventOpenViews : IExternalEventHandler
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
            if (openViewsTool.WindowResult == WindowResult.UserApply) viewCoordsTool.ApplyCoordsToViews();
        }
        public string GetName()
        {
            return "Opened multiple views";
        }
    }

    class ExternalEventSaveCoords : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Saving coordinates to file...");
            ViewCoordsTool vct = new ViewCoordsTool(uiapp.ActiveUIDocument);
            vct.SaveCurrentCoordinatesAs();
        }
        public string GetName()
        {
            return "Saved coordinates";
        }
    }

    class ExternalEventOpenCoords : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Loading coordinates from file...");
            ViewCoordsTool vct = new ViewCoordsTool(uiapp.ActiveUIDocument);
            vct.LoadCoordinatesFrom();
        }
        public string GetName()
        {
            return "Loaded coordinates";
        }
    }

    

    class ExternalEventExcelDataImporter : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ExcelDataImport excelDataImport = new ExcelDataImport();
            excelDataImport.ShowDialog();
        }
        public string GetName()
        {
            return "Excel data import";
        }
    }
}
