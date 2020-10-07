﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExStorage;
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
                View activeView = uiapp.ActiveUIDocument.ActiveView;
                if(activeView.ViewType == ViewType.CeilingPlan || activeView.ViewType == ViewType.EngineeringPlan || activeView.ViewType == ViewType.FloorPlan || activeView.ViewType == ViewType.AreaPlan)
                {
                    ViewCoordsTool vct = new ViewCoordsTool(uiapp.ActiveUIDocument);
                    vct.ApplyCoordsToViews();
                }
                else
                {
                    string info = "Die aktive Ansicht muss vom Typ 2d sein!" + Environment.NewLine + Environment.NewLine + "Unterstützte Ansichtstypen:" + Environment.NewLine + "Floor plan, ceiling plan, structural plan, area plan.";
                    TaskDialog.Show("Warning", info);
                }
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
                View activeView = uiapp.ActiveUIDocument.ActiveView;
                if (activeView.ViewType == ViewType.CeilingPlan || activeView.ViewType == ViewType.EngineeringPlan || activeView.ViewType == ViewType.FloorPlan || activeView.ViewType == ViewType.AreaPlan)
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
                else
                {
                    string info = "Die aktive Ansicht muss vom Typ 2d sein!" + Environment.NewLine + Environment.NewLine + "Unterstützte Ansichtstypen:" + Environment.NewLine + "Floor plan, ceiling plan, structural plan, area plan.";
                    TaskDialog.Show("Warning", info);
                }

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
                View activeView = uiapp.ActiveUIDocument.ActiveView;
                if (activeView.ViewType == ViewType.ThreeD || activeView.ViewType == ViewType.Section || activeView.ViewType == ViewType.CeilingPlan || activeView.ViewType == ViewType.EngineeringPlan || activeView.ViewType == ViewType.FloorPlan || activeView.ViewType == ViewType.AreaPlan)
                {
                    ViewCoordsTool vct = new ViewCoordsTool(uiapp.ActiveUIDocument);
                    if (activeView.ViewType == ViewType.ThreeD)
                    {
                        vct.Save3dCoordinatesAs();
                    }
                    else
                    {
                        vct.SaveCurrentCoordinatesAs();
                    }
                }
                else
                {
                    string info = "Die aktive Ansicht muss vom Typ 2d sein!" + Environment.NewLine + Environment.NewLine + "Unterstützte Ansichtstypen:" + Environment.NewLine + "Floor plan, ceiling plan, structural plan, area plan, 3D, section";
                    TaskDialog.Show("Warning", info);
                }

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
                View activeView = uiapp.ActiveUIDocument.ActiveView;
                if (activeView.ViewType == ViewType.ThreeD || activeView.ViewType == ViewType.Section || activeView.ViewType == ViewType.CeilingPlan || activeView.ViewType == ViewType.EngineeringPlan || activeView.ViewType == ViewType.FloorPlan || activeView.ViewType == ViewType.AreaPlan)
                {
                    ViewCoordsTool vct = new ViewCoordsTool(uiapp.ActiveUIDocument);
                    if (activeView.ViewType == ViewType.ThreeD)
                    {
                        vct.Load3dCoordinatesFrom();
                    }
                    else
                    {
                        vct.LoadCoordinatesFrom();
                    }
                }
                else
                {
                    string info = "Die aktive Ansicht ist nicht unterstüzt!" + Environment.NewLine + Environment.NewLine + "Unterstützte Ansichtstypen:" + Environment.NewLine + "Floor plan, ceiling plan, structural plan, area plan, 3D, section";
                    TaskDialog.Show("Warning", info);
                }
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
                ExcelDataImport excelDataImport = new ExcelDataImport();
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
                OpeningWindowMainViewModel openingWindowMainViewModel = OpeningWindowMainViewModel.Initialize(uiapp.ActiveUIDocument);
                OpeningsMainWindow openingsMainWindow = new OpeningsMainWindow(openingWindowMainViewModel);
                openingsMainWindow.ShowDialog();
                GtbSchema gtbSchema = new GtbSchema();
                gtbSchema.SetGtbSchema();
                if (openingsMainWindow.OpeningSymbolTool != null)
                {
                    openingsMainWindow.OpeningSymbolTool.GtbSchema = gtbSchema;
                    openingsMainWindow.OpeningSymbolTool.ProcessSelectedViews();
                    //openingsMainWindow.OpeningSymbolTool.DisplayProblematic();
                }
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

    class ExternalEventTagAllOpenings : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Initiated tag all openings tool");
            try
            {
                Document doc = uiapp.ActiveUIDocument.Document;
                if (OpeningTagger.IsValidViewType(doc))
                {
                    OpeningTagger openingTagger = OpeningTagger.Initialize(doc);
                    GtbWindowResult gtbWindowResult =  openingTagger.DisplayWindow();
                    if(gtbWindowResult == GtbWindowResult.Apply)
                    {
                        openingTagger.TagThemAll();
                        openingTagger.ShowNewTagsInfo();
                    }
                }
                else
                {
                    TaskDialog.Show("Error", "Unterstützte Ansichten:\n- Grundrisse\n- Deckenpläne\n- Tragwerkspläne");
                }
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
            return "Tag all openings";
        }
    }

    class ExternalEventUpdateViewModel : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Loading current context to view model");
            try
            {
                App.Instance.DurchbruchMemoryViewModel.LoadContext(uiapp);
                App.Instance.DurchbruchMemoryViewModel.InitializeDurchbruche();
                App.Instance.DurchbruchMemoryViewModel.SignalEvent.Set();
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
            return "Current context loaded";
        }
    }

    class ExternalEventShowElement : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Loading current context to view model");
            try
            {
                App.Instance.DurchbruchMemoryViewModel.ShowElement();
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
            return "Current context loaded";
        }
    }

    class ExternalEventCutOpeningMemory : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Initiated roof symbol selector");
            try
            {
                CutOpeningMemory cutOpeningMemory = new CutOpeningMemory(uiapp.ActiveUIDocument);
                cutOpeningMemory.GetAllOpenings();
                cutOpeningMemory.SetOpeningMemories();
                CutOpeningMemoryWindow window = new CutOpeningMemoryWindow(cutOpeningMemory);
                window.ShowDialog();
                if (window.GtbWindowResult == GtbWindowResult.Apply)
                {
                    window.CutOpeningMemory.DisplayChanges();
                }
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
            return "Symbol roof selector";
        }
    }

    class ExternalEventMepExtract : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Initiated mep extractor");
            try
            {
                RevitDataExtractor revitDataExtractor = new RevitDataExtractor(uiapp.ActiveUIDocument.Document);
                revitDataExtractor.ExtractMepSystems();
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
            return "MEP systems extract";
        }
    }

    class ExternalEventShowOnView : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Opening selected view");
            try
            {
                App.Instance.DurchbruchMemoryViewModel.OpenView();
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
            return "Opened selected view";
        }
    }

    class ExternalEventSaveData : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Saving data to extensible storage");
            try
            {
                App.Instance.DurchbruchMemoryViewModel.SaveOpeningsToExStorage();
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
            return "data saved to exstorage";
        }
    }
}
