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
#endif
        public const string AssemblyMinorVersion = "2";
        public const string AssemblyBuildVersion = "2";
        public const string AssemblyRevisionVersion = "1";
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
        RibbonItem _button;

        internal static App _app = null;

        public static App Instance
        {
            get { return _app; }
        }

        public Result OnStartup(UIControlledApplication application)
        {
            _app = this;
            ErrorLog = new ErrorLog();
            string path = Assembly.GetExecutingAssembly().Location;
            RibbonPanel gtbPanel = application.CreateRibbonPanel("GTB - Berlin");
            PushButtonData pushButtonGtbPanelControl = new PushButtonData( "GTB", "Anzeigen", path, "GtbTools.ShowHideDock");
            pushButtonGtbPanelControl.LargeImage = GetEmbeddedImage("Resources.GtbInactive.png");
            _button = gtbPanel.AddItem(pushButtonGtbPanelControl);
            RegisterDockableWindow(application);

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            ErrorLog.DeleteLog();
            return Result.Succeeded;
        }

        public void Toggle(ExternalCommandData commandData)
        {
            if(_button.ItemText == "Anzeigen")
            {
                _button.ItemText = "Ausblenden";
                PushButton pb = _button as PushButton;
                pb.LargeImage = GetEmbeddedImage("Resources.GtbActive.png");
                ShowDockableWindow(commandData);
            }
            else
            {
                _button.ItemText = "Anzeigen";
                PushButton pb = _button as PushButton;
                pb.LargeImage = GetEmbeddedImage("Resources.GtbInactive.png");
                HideDockableWindow(commandData);
            }
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

            IExternalEventHandler handler_event7 = new ExternalEventSelectWallSymbols();
            ExternalEvent exEvent7 = ExternalEvent.Create(handler_event7);

            IExternalEventHandler handler_event8 = new ExternalEventSelectFloorSymbols();
            ExternalEvent exEvent8 = ExternalEvent.Create(handler_event8);



            DockablePaneProviderData data = new DockablePaneProviderData();

            GtbDockPage GtbDockableWindow = new GtbDockPage(PlugInVersion, exEvent, exEvent2, exEvent3, exEvent4, exEvent5, exEvent6, exEvent7, exEvent8);
            data.FrameworkElement = GtbDockableWindow as System.Windows.FrameworkElement;
            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Floating;
            data.InitialState.SetFloatingRectangle(new Autodesk.Revit.DB.Rectangle(100, 100, 360, 540));
            data.InitialState.TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser;

            DockablePaneId dpid = new DockablePaneId(new Guid("{9F702FC8-EC07-4A80-846F-04AFA5AC8820}"));
            
            app.RegisterDockablePane(dpid, "GTB-Berlin", GtbDockableWindow as IDockablePaneProvider);
        }

        private void ShowDockableWindow(ExternalCommandData commandData)
        {
            DockablePaneId dpid = new DockablePaneId(new Guid("{9F702FC8-EC07-4A80-846F-04AFA5AC8820}"));
            DockablePane dp = commandData.Application.GetDockablePane(dpid);
            
            dp.Show();
        }

        private void HideDockableWindow(ExternalCommandData commandData)
        {
            DockablePaneId dpid = new DockablePaneId(new Guid("{9F702FC8-EC07-4A80-846F-04AFA5AC8820}"));
            DockablePane dp = commandData.Application.GetDockablePane(dpid);
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
                TaskDialog.Show("bla", ex.ToString());
                return null;
            }
        }
    }
}
