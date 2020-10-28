using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GtbTools.Forms;
using Autodesk.Revit.UI.Events;
using ViewModels;

namespace GtbTools
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class App : IExternalApplication
    {
        #region Configuration and assembly version
#if DEBUG2018 || RELEASE2018 
        public const string AssemblyYear = "2018";
#elif DEBUG2019 || RELEASE2019
        public const string AssemblyYear = "2019";
#elif DEBUG2020 || RELEASE2020
        public const string AssemblyYear = "2020";
#elif DEBUG2021 || RELEASE2021
        public const string AssemblyYear = "2021";
#endif
        public const string AssemblyMinorVersion = "2";
        public const string AssemblyBuildVersion = "4";
        public const string AssemblyRevisionVersion = "3";
        #endregion

        public static string PlugInVersion
        {
            get
            {
                return string.Format("{0}.{1}.{2}.{3}",
                                AssemblyYear,
                                AssemblyMinorVersion,
                                AssemblyBuildVersion,
                                AssemblyRevisionVersion);
            }
        }

        public static string ExecutingAssemblyPath { get { return Assembly.GetExecutingAssembly().Location; } }
        public ErrorLog ErrorLog { get; set; }
        public DurchbruchMemoryViewModel DurchbruchMemoryViewModel { get; set; }
        public Functions.DurchbruchRotationFix DurchbruchRotationFix { get; set; }
        public Functions.RevitOpenedViews RevitOpenedViews { get; set; }
        public Functions.CopyParameterFromHost CopyParameterFromHost { get; set; }
        RibbonItem _button;
        ExternalEvent _toggleEvent;

        internal static App _app = null;

        public static App Instance
        {
            get { return _app; }
        }

        public Result OnStartup(UIControlledApplication application)
        {
            _app = this;
            ErrorLog = new ErrorLog();
            DurchbruchMemoryViewModel = new DurchbruchMemoryViewModel();
            DurchbruchRotationFix = new Functions.DurchbruchRotationFix();
            RevitOpenedViews = new Functions.RevitOpenedViews();
            CopyParameterFromHost = new Functions.CopyParameterFromHost();
            string path = Assembly.GetExecutingAssembly().Location;
            RibbonPanel gtbPanel = application.CreateRibbonPanel("GTB - Berlin");

            PushButtonData pushButtonGtbPanelControl = new PushButtonData( "GTB", "Anzeigen", path, "GtbTools.ShowHideDock");
            pushButtonGtbPanelControl.LargeImage = GetEmbeddedImage("Resources.GtbInactive.png");
            _button = gtbPanel.AddItem(pushButtonGtbPanelControl);

            PushButtonData pushButtonFamilyEdit = new PushButtonData("Family Edit", "Family Edit", path, "GtbTools.FamilyEditButton");
            pushButtonFamilyEdit.LargeImage = GetEmbeddedImage("Resources.FamilyEdit.png");
            gtbPanel.AddItem(pushButtonFamilyEdit);

            RegisterDockableWindow(application);
            IExternalEventHandler handler_event = new ExternalEventShowHideDock();
            _toggleEvent = ExternalEvent.Create(handler_event);
            application.DockableFrameVisibilityChanged += OnDockableFrameVisibilityChanged;
            application.ViewActivated += OnDockableFrameVisibilityChanged;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            ErrorLog.DeleteLog();
            return Result.Succeeded;
        }

        public void Toggle(UIApplication application)
        {
            if(_button.ItemText == "Anzeigen")
            {
                //_button.ItemText = "Ausblenden";
                //PushButton pb = _button as PushButton;
                //pb.LargeImage = GetEmbeddedImage("Resources.GtbActive.png");
                ShowDockableWindow(application);
            }
            else
            {
                //_button.ItemText = "Anzeigen";
                //PushButton pb = _button as PushButton;
                //pb.LargeImage = GetEmbeddedImage("Resources.GtbInactive.png");
                HideDockableWindow(application);
            }
        }

        public void SwitchDockPanelButton(UIApplication application)
        {
            DockablePaneId dpid = new DockablePaneId(new Guid("{9F702FC8-EC07-4A80-846F-04AFA5AC8820}"));
            DockablePane dp = null;
            try
            {
                dp = application.GetDockablePane(dpid);
            }
            catch (Exception)
            {
                ErrorLog.WriteToLog("User cancelled document loading. Panel was not created yet.");
                ErrorLog.RemoveLog = false;
                return;
            }
            
            if(dp.IsShown() && _button.ItemText == "Anzeigen")
            {
                _button.ItemText = "Ausblenden";
                PushButton pb = _button as PushButton;
                pb.LargeImage = GetEmbeddedImage("Resources.GtbActive.png");
            }
            if (!dp.IsShown() && _button.ItemText == "Ausblenden")
            {
                _button.ItemText = "Anzeigen";
                PushButton pb = _button as PushButton;
                pb.LargeImage = GetEmbeddedImage("Resources.GtbInactive.png");
            }
        }

        public void FixDockPanelButton(UIControlledApplication application)
        {
            DockablePaneId dpid = new DockablePaneId(new Guid("{9F702FC8-EC07-4A80-846F-04AFA5AC8820}"));
            DockablePane dp = application.GetDockablePane(dpid);
            if (dp.IsShown() && _button.ItemText == "Anzeigen")
            {
                _button.ItemText = "Ausblenden";
                PushButton pb = _button as PushButton;
                pb.LargeImage = GetEmbeddedImage("Resources.GtbActive.png");
            }
            if (!dp.IsShown() && _button.ItemText == "Ausblenden")
            {
                _button.ItemText = "Anzeigen";
                PushButton pb = _button as PushButton;
                pb.LargeImage = GetEmbeddedImage("Resources.GtbInactive.png");
            }
        }

        private void OnDockableFrameVisibilityChanged(object sender, EventArgs e)
        {
            _toggleEvent.Raise();
        }

        private void RegisterDockableWindow(UIControlledApplication app)
        {
            IExternalEventHandler handler_event = new ExternalEventApplyCoordsToViews();
            ExternalEvent exEvent = ExternalEvent.Create(handler_event);

            IExternalEventHandler handler_event2 = new ExternalEventOpenViews();
            ExternalEvent exEvent2 = ExternalEvent.Create(handler_event2);

            IExternalEventHandler handler_event3 = new ExternalEventSaveCoords();
            ExternalEvent exEvent3 = ExternalEvent.Create(handler_event3);

            IExternalEventHandler handler_event4 = new ExternalEventOpenCoords();
            ExternalEvent exEvent4 = ExternalEvent.Create(handler_event4);

            IExternalEventHandler handler_event5 = new ExternalEventExcelDataImporter();
            ExternalEvent exEvent5 = ExternalEvent.Create(handler_event5);

            IExternalEventHandler handler_event6 = new ExternalEventSymbolHandler();
            ExternalEvent exEvent6 = ExternalEvent.Create(handler_event6);

            IExternalEventHandler handler_event7 = new ExternalEventTagAllOpenings();
            ExternalEvent exEvent7 = ExternalEvent.Create(handler_event7);

            IExternalEventHandler handler_event8 = new ExternalEventUpdateViewModel();
            ExternalEvent exEvent8 = ExternalEvent.Create(handler_event8);

            IExternalEventHandler showElementIExEventHandler = new ExternalEventShowElement();
            ExternalEvent showElementEventHandler = ExternalEvent.Create(showElementIExEventHandler);

            IExternalEventHandler openViewExEventHandler = new ExternalEventShowOnView();
            ExternalEvent openViewEventHandler = ExternalEvent.Create(openViewExEventHandler);

            IExternalEventHandler saveDataExEventHandler = new ExternalEventSaveData();
            ExternalEvent saveDataEventHandler = ExternalEvent.Create(saveDataExEventHandler);

            IExternalEventHandler changeValue1ExEventHandler = new ExternalEventChangeDurchbruchDiameter();
            ExternalEvent changeValueEvent1 = ExternalEvent.Create(changeValue1ExEventHandler);

            IExternalEventHandler changeValue2ExEventHandler = new ExternalEventChangeDurchbruchOffset();
            ExternalEvent changeValueEvent2 = ExternalEvent.Create(changeValue2ExEventHandler);

            DurchbruchMemoryViewModel.SetExternalEvents(exEvent8, showElementEventHandler, openViewEventHandler, saveDataEventHandler, changeValueEvent1, changeValueEvent2);

            IExternalEventHandler handler_event9 = new ExternalEventCutOpeningMemory();
            ExternalEvent exEvent9 = ExternalEvent.Create(handler_event9);

            IExternalEventHandler handler_event10 = new ExternalEventMepExtract();
            ExternalEvent exEvent10 = ExternalEvent.Create(handler_event10);

            IExternalEventHandler fixRotationEventHandler = new ExternalEventFixRotationIssue();
            ExternalEvent fixRotationExEvent = ExternalEvent.Create(fixRotationEventHandler);

            IExternalEventHandler handler_event11 = new ExternalEventCopyElevations();
            ExternalEvent exEvent11 = ExternalEvent.Create(handler_event11);

            DurchbruchRotationFix.SetExternalEvents(fixRotationExEvent);

            IExternalEventHandler handler12 = new ExternalEventRevitOpenedViews();
            ExternalEvent exEvent12 = ExternalEvent.Create(handler12);
            RevitOpenedViews.SetEvent(exEvent12);

            IExternalEventHandler eventHandlerCopyParameter = new ExternalEventCopyParameter();
            ExternalEvent exEventCopyParameter = ExternalEvent.Create(eventHandlerCopyParameter);
            CopyParameterFromHost.SetEvents(exEventCopyParameter);

            DockablePaneProviderData data = new DockablePaneProviderData();

            GtbDockPage GtbDockableWindow = new GtbDockPage(PlugInVersion, exEvent, exEvent2, exEvent3, exEvent4, exEvent5, exEvent6, exEvent7, 
                                                                DurchbruchMemoryViewModel, exEvent9, exEvent10, DurchbruchRotationFix, exEvent11, RevitOpenedViews, CopyParameterFromHost);
            data.FrameworkElement = GtbDockableWindow as System.Windows.FrameworkElement;
            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Floating;
            data.InitialState.SetFloatingRectangle(new Autodesk.Revit.DB.Rectangle(100, 100, 360, 540));
            data.InitialState.TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser;

            DockablePaneId dpid = new DockablePaneId(new Guid("{9F702FC8-EC07-4A80-846F-04AFA5AC8820}"));
            
            app.RegisterDockablePane(dpid, "GTB-Berlin", GtbDockableWindow as IDockablePaneProvider);
        }

        private void ShowDockableWindow(UIApplication application)
        {
            DockablePaneId dpid = new DockablePaneId(new Guid("{9F702FC8-EC07-4A80-846F-04AFA5AC8820}"));
            DockablePane dp = application.GetDockablePane(dpid);
            
            dp.Show();
        }

        private void HideDockableWindow(UIApplication application)
        {
            DockablePaneId dpid = new DockablePaneId(new Guid("{9F702FC8-EC07-4A80-846F-04AFA5AC8820}"));
            DockablePane dp = application.GetDockablePane(dpid);
            dp.Hide();
        }

        private BitmapSource GetEmbeddedImage(string name)
        {
            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Stream stream = assembly.GetManifestResourceStream(name);
                return BitmapFrame.Create(stream);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
