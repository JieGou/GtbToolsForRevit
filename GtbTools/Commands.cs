using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using CuttingElementTool;
using DurchbruchRotationFix;
using ExStorage;
using FamilyTools;
using Functions;
using GtbTools.GUI;
using GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using ViewModels;

namespace GtbTools
{
    /// <summary>
    /// Shows and hides dock panel. Is asigned to revit user interface show/hide button
    /// </summary>
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
                App.Instance.Toggle(commandData.Application);
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

    /// <summary>
    /// Initiates family tools window. Is asigned to family edit button
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class FamilyEditButton : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("FamilyEdit Tool");
            try
            {
                DefinitionFile definitionFile = commandData.Application.Application.OpenSharedParameterFile();
                ConnectorParameters connectorParameters = new ConnectorParameters(commandData.Application.ActiveUIDocument.Document, definitionFile);                
                CheckboxLabelReplace checkboxLabelReplace = new CheckboxLabelReplace(commandData.Application.ActiveUIDocument.Document);
                FamilyToolsWindow window = new FamilyToolsWindow(checkboxLabelReplace, connectorParameters);
                window.ShowDialog();
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

    /// <summary>
    /// Changes dock panel button state and text when user closes dock panel with "x". Aligns button state and text with dockpanel visibility on startup.
    /// </summary>
    class ExternalEventShowHideDock : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Using event to change show hide button state and text.");
            try
            {
                App.Instance.SwitchDockPanelButton(uiapp);
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
            return "Show hide dock button visibility correction by event.";
        }
    }

    /// <summary>
    /// Initiates view coords tool and copies coordinates from active view to all opened plan views.
    /// </summary>
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
            return "Copy active view coordinates to opened views.";
        }
    }

    /// <summary>
    /// Opens selected views with user defined settings
    /// </summary>
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
                View activeView = uiapp.ActiveUIDocument.ActiveView;
                if (activeView.ViewType == ViewType.CeilingPlan || activeView.ViewType == ViewType.EngineeringPlan || activeView.ViewType == ViewType.FloorPlan || activeView.ViewType == ViewType.AreaPlan)
                {
                    ViewCoordsTool viewCoordsTool = new ViewCoordsTool(uiapp.ActiveUIDocument);
                    if (openViewsTool.WindowResult == WindowResult.UserApply) viewCoordsTool.ApplyCoordsToViews();
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
            return "Open multiple views";
        }
    }

    /// <summary>
    /// Saves currect view coordinates to file.
    /// </summary>
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
                        View3D view3D = activeView as View3D;
                        if(view3D.IsPerspective)
                        {
                            TaskDialog.Show("Warning!", "Perspective ansichtmodus ist nicht unterstützt!" + Environment.NewLine + "Bitte verwenden Sie den orthogonalen Modus.");
                        }
                        else
                        {
                            vct.Save3dCoordinatesAs();
                        }
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

    /// <summary>
    /// Loads saved coordinates from file
    /// </summary>
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
                        View3D view3D = activeView as View3D;
                        if (view3D.IsPerspective)
                        {
                            TaskDialog.Show("Warning!", "Perspective ansichtmodus ist nicht unterstützt!" + Environment.NewLine + "Bitte verwenden Sie den orthogonalen Modus.");
                        }
                        else
                        {
                            vct.Load3dCoordinatesFrom();
                        }
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

    /// <summary>
    /// Excel data importer. Not active solution
    /// </summary>
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

    /// <summary>
    /// Align symbol visibility.
    /// </summary>
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
                //GtbSchema gtbSchema = new GtbSchema();
                //gtbSchema.SetGtbSchema();
                if (openingsMainWindow.OpeningSymbolTool != null)
                {
                    //openingsMainWindow.OpeningSymbolTool.GtbSchema = gtbSchema;
                    openingsMainWindow.OpeningSymbolTool.ProcessSelectedViewsLight();
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

    /// <summary>
    /// Tag all openings on view
    /// </summary>
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

    /// <summary>
    /// Opening memory loading data context and initializing openings
    /// </summary>
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

    /// <summary>
    /// Opening memory events
    /// </summary>
    class ExternalEventShowElement : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Opening memory events");
            try
            {
                DurchbruchMemoryViewModel model = App.Instance.DurchbruchMemoryViewModel;
                model.SignalEvent.WaitOne();
                model.SignalEvent.Reset();
                if (model.DurchbruchMemoryAction == DurchbruchMemoryAction.ShowElements)
                {
                    model.ShowElements();
                }
                if (model.DurchbruchMemoryAction == DurchbruchMemoryAction.ShowElement)
                {
                    model.ShowElement();
                }
                if (model.DurchbruchMemoryAction == DurchbruchMemoryAction.ShowPosition)
                {
                    model.CreateOldPositionMarker();
                }
                if (model.DurchbruchMemoryAction == DurchbruchMemoryAction.DeletePosition)
                {
                    model.DeleteOldPositionMarker();
                }
                if (model.DurchbruchMemoryAction == DurchbruchMemoryAction.DeleteRemainingMarkers)
                {
                    MessageBoxResult result = MessageBox.Show("Möchten Sie verbleibende Positionsmarkierungen löschen?", "Warning!", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        model.DeleteAllPositionMarkers();
                    }
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
            return "Opening memory events";
        }
    }

    /// <summary>
    /// Old version of cut opening memory
    /// </summary>
    class ExternalEventCutOpeningMemory : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Old cut opening memories");
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
            return "Old cut openings memory";
        }
    }

    /// <summary>
    /// Extract mep system data
    /// </summary>
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

    /// <summary>
    /// Open view and select element opening memory
    /// </summary>
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

    /// <summary>
    /// Saving opening memory elements to external storage
    /// </summary>
    class ExternalEventSaveData : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Saving data to extensible storage");
            try
            {
                App.Instance.DurchbruchMemoryViewModel.SaveOpeningsToExStorage();
                if(App.Instance.DurchbruchMemoryViewModel.MemorySaveOption == MemorySaveOption.All)
                {
                    TaskDialog.Show("Info", "All openings have been saved!");
                }
                if (App.Instance.DurchbruchMemoryViewModel.MemorySaveOption == MemorySaveOption.New)
                {
                    TaskDialog.Show("Info", "New openings have been saved!");
                }
                if (App.Instance.DurchbruchMemoryViewModel.MemorySaveOption == MemorySaveOption.Selected)
                {
                    TaskDialog.Show("Info", "Selected openings have been saved!");
                }
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
            return "data saved to exstorage";
        }
    }

    /// <summary>
    /// Rotation issue fix tool
    /// </summary>
    class ExternalEventFixRotationIssue : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Fixing rotation issue...");
            try
            {

                Functions.DurchbruchRotationFix durchbruchRotationFix = Functions.DurchbruchRotationFix.Initialize(uiapp.ActiveUIDocument);
                durchbruchRotationFix.DisplayWindow();
                if(durchbruchRotationFix.DurchbruchRotationFixWindow.WindowDecision == WindowDecision.Apply)
                {
                    durchbruchRotationFix.FixRotation(durchbruchRotationFix.RotationFixViewModel.RotatedElementsToFix);
                }
                if (durchbruchRotationFix.DurchbruchRotationFixWindow.WindowDecision == WindowDecision.Show)
                {
                    App.Instance.DurchbruchRotationFix.SelectOpenings(App.Instance.DurchbruchRotationFix.RotationFixViewModel.Selection);
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
            return "Fixed rotation issue";
        }
    }

    /// <summary>
    /// Opening elevation copy tool
    /// </summary>
    class ExternalEventCopyElevations : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Preaparing to copy elevation to opening elevation parameter...");
            try
            {
                GetSetElevation getSetElevation = new GetSetElevation(uiapp.ActiveUIDocument.Document);
                getSetElevation.GetOpenings();
                CopyElevationWindow window = new CopyElevationWindow(getSetElevation);
                window.ShowDialog();
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
            return "Copied elevations.";
        }
    }

    /// <summary>
    /// Save currently opened views. Loads saved views.
    /// </summary>
    class ExternalEventRevitOpenedViews : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Revit opened views tool initialized");
            try
            {
                RevitOpenedViews model = App.Instance.RevitOpenedViews;
                model.LoadContext(uiapp);
                if(model.IsLoading == true)
                {
                    model.LoadSavedViews();
                }
                if (model.IsSaving == true)
                {
                    model.SaveOpenedViews();
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
            return "Revit opened views";
        }
    }

    /// <summary>
    /// Openings diameter change event
    /// </summary>
    class ExternalEventChangeDurchbruchDiameter : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Changing durchbruch value");
            try
            {
                DurchbruchMemoryViewModel model = App.Instance.DurchbruchMemoryViewModel;
                //there have to be separate events because change selection event is conflicted with edit ending event
                model.CurrentModel.SetNewDiameter(uiapp.ActiveUIDocument.Document);
                //model.CurrentModel.SetNewOffset(uiapp.ActiveUIDocument.Document);               
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
            return "Change durchbruch value";
        }
    }

    /// <summary>
    /// Openings offset change event
    /// </summary>
    class ExternalEventChangeDurchbruchOffset : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Changing durchbruch value");
            try
            {
                DurchbruchMemoryViewModel model = App.Instance.DurchbruchMemoryViewModel;
                //there have to be separate events because change selection event is conflicted with edit ending event
                model.CurrentModel.SetNewOffset(uiapp.ActiveUIDocument.Document);
                //model.CurrentModel.SetNewOffset(uiapp.ActiveUIDocument.Document);               
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
            return "Change durchbruch value";
        }
    }

    /// <summary>
    /// Parameter copy paste between host and opening generic model family instance
    /// </summary>
    class ExternalEventCopyParameter : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Copy parameter from host or self");
            try
            {
                CopyParameterFromHost copyParameterFromHost = App.Instance.CopyParameterFromHost;
                if (copyParameterFromHost.IsInitialized)
                {
                    if (copyParameterFromHost._hostClicked)
                    {
                        copyParameterFromHost.CopyParametersHost();
                    }
                    if (copyParameterFromHost._selfClicked)
                    {
                        copyParameterFromHost.CopyParametersSelf();
                    }
                }
                else
                {
                    copyParameterFromHost.Initialize(uiapp.ActiveUIDocument.Document);
                    copyParameterFromHost.DisplayWindow();
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
            return "Copy parameter form host";
        }
    }

    /// <summary>
    /// Fix opening diameter with pipe slope
    /// </summary>
    class ExternalEventFixDiameter : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Fix pipe slope addition to diameter");
            try
            {
                CuttingElementSearch cuttingElementSearch = App.Instance.CuttingElementSearch;
                if(cuttingElementSearch.ToolAction == CutElementToolAction.Initialize)
                {
                    cuttingElementSearch.InitializeDocument(uiapp.ActiveUIDocument.Document);
                    cuttingElementSearch.SetCuttingElements();
                    cuttingElementSearch.SetOpeningModels();
                    cuttingElementSearch.SetLinks();
                    cuttingElementSearch.DisplayWindow();
                }
                if (cuttingElementSearch.ToolAction == CutElementToolAction.SearchLinks)
                {
                    cuttingElementSearch.SetLinkedDoc();
                    cuttingElementSearch.SetLinkedCuttingElements();
                    cuttingElementSearch.SetOpeningModels();
                    cuttingElementSearch.SignalEvent.Set();
                }
                if (cuttingElementSearch.ToolAction == CutElementToolAction.SelectItems)
                {
                    List<ElementId> elementIds = new List<ElementId>();
                    elementIds = cuttingElementSearch.Selection.Select(e => e.FamilyInstance.Id as ElementId).ToList();
                    uiapp.ActiveUIDocument.Selection.SetElementIds(new List<ElementId>());
                    uiapp.ActiveUIDocument.Selection.SetElementIds(elementIds);
                }
                if (cuttingElementSearch.ToolAction == CutElementToolAction.FixSelected)
                {
                    cuttingElementSearch.FixSelection();
                    cuttingElementSearch.SignalEvent.Set();
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
            return "Fix pipe slope addition to diameter";
        }
    }

    /// <summary>
    /// Annotate vertical stacks to show flow direction of the medium through active plan view
    /// </summary>
    class ExternalEventAnnotateStacks : IExternalEventHandler
    {
        public void Execute(UIApplication uiapp)
        {
            ErrorLog errorLog = App.Instance.ErrorLog;
            errorLog.WriteToLog("Annotate vertical stacks");
            try
            {
                PipeFlowTagger pipeFlowTagger = App.Instance.PipeFlowTagger;
                if(pipeFlowTagger.Action == PipeFlowTool.PipeFlowToolAction.Initialize)
                {
                    pipeFlowTagger.Initialize(uiapp.ActiveUIDocument);
                    pipeFlowTagger.DisplayWindow();
                }
                if (pipeFlowTagger.Action == PipeFlowTool.PipeFlowToolAction.Analyze)
                {
                    pipeFlowTagger.AnalyzeView();
                    pipeFlowTagger.SignalEvent.Set();
                }
                if (pipeFlowTagger.Action == PipeFlowTool.PipeFlowToolAction.Tag)
                {
                    pipeFlowTagger.TagAllLines();
                    pipeFlowTagger.SignalEvent.Set();
                }
                if (pipeFlowTagger.Action == PipeFlowTool.PipeFlowToolAction.Show)
                {
                    uiapp.ActiveUIDocument.Selection.SetElementIds(new List<ElementId>());
                    pipeFlowTagger.SelectElement();
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
            return "VerticalStackAnnotation";
        }
    }
}
