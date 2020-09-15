using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Functions;
using GtbTools.GUI;
using GUI;
using System;
using System.Windows;
using ViewModels;

namespace GtbTools
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class ShowHideDock : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
        	ErrorLog errorLog = App.Instance.ErrorLog;
        	errorLog.WriteToLog("Changing DockPanel visibility state");
            try
            {
                App.Instance.Toggle(commandData);
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten. Error log wurde gespeichert.");
                errorLog.WriteToLog(ex.ToString());
                errorLog.RemoveLog = false;
                return Result.Failed;
            }
        }
    }

    class ExternalEventApplyCoordsToViews : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Multi apply coords to view");
            try
            {
                ViewCoordsTool vct = new ViewCoordsTool(uiapp.ActiveUIDocument);
                vct.ApplyCoordsToViews();
        }
            catch (Exception ex)
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten. Error log wurde gespeichert.");
                errorLog.WriteToLog(ex.ToString());
                errorLog.RemoveLog = false;
            }
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
            try
            {
                OpenViewsTool openViewsTool = new OpenViewsTool(uiapp.ActiveUIDocument, errorLog);
                openViewsTool.CreateModelViewList();

                ZoomWindow zoomWindow = new ZoomWindow(openViewsTool);
                zoomWindow.ShowDialog();
                if (openViewsTool.WindowResult == WindowResult.UserApply) openViewsTool.OpenViews();
                if (openViewsTool.CloseInactive == true) openViewsTool.CloseInactiveViews();
                ViewCoordsTool viewCoordsTool = new ViewCoordsTool(uiapp.ActiveUIDocument);
                if (openViewsTool.WindowResult == WindowResult.UserApply) viewCoordsTool.ApplyCoordsToViews();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten. Error log wurde gespeichert.");
                errorLog.WriteToLog(ex.ToString());
                errorLog.RemoveLog = false;
            }
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
            try
            {
                ViewCoordsTool vct = new ViewCoordsTool(uiapp.ActiveUIDocument);
                vct.SaveCurrentCoordinatesAs();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten. Error log wurde gespeichert.");
                errorLog.WriteToLog(ex.ToString());
                errorLog.RemoveLog = false;
            }
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
            try
            {
                ViewCoordsTool vct = new ViewCoordsTool(uiapp.ActiveUIDocument);
                vct.LoadCoordinatesFrom();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten. Error log wurde gespeichert.");
                errorLog.WriteToLog(ex.ToString());
                errorLog.RemoveLog = false;
            }
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
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Initiated excel data import");
            try
            {
                ExcelDataImport excelDataImport = new ExcelDataImport(uiapp.MainWindowHandle);
                excelDataImport.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten. Error log wurde gespeichert.");
                errorLog.WriteToLog(ex.ToString());
                errorLog.RemoveLog = false;
            }
        }
        public string GetName()
        {
            return "Excel data import";
        }
    }

    class ExternalEventSymbolHandler : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Initiated symbol handler");
            try
            {
                OpeningWindowMainViewModel openingWindowMainViewModel = OpeningWindowMainViewModel.Initialize(uiapp.ActiveUIDocument.Document);
                OpeningsMainWindow openingsMainWindow = new OpeningsMainWindow(openingWindowMainViewModel);
                openingsMainWindow.ShowDialog();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Es ist ein Fehler aufgetreten. Error log wurde gespeichert.");
                errorLog.WriteToLog(ex.ToString());
                errorLog.RemoveLog = false;
            }
        }
        public string GetName()
        {
            return "Symbol handler";
        }
    }
}
