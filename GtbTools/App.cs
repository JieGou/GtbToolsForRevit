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
        public const string AssemblyMinorVersion = "0";
        public const string AssemblyBuildVersion = "0";
        public const string AssemblyRevisionVersion = "0";
        #endregion 

        public static string ExecutingAssemblyPath { get { return Assembly.GetExecutingAssembly().Location; } }
        RibbonItem _button;

        internal static App _app = null;

        public static App Instance
        {
            get { return _app; }
        }

        public Result OnStartup(UIControlledApplication application)
        {
            _app = this;
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
            DockablePaneProviderData data = new DockablePaneProviderData();
            GtbDockPage GtbDockableWindow = new GtbDockPage();
            data.FrameworkElement = GtbDockableWindow as System.Windows.FrameworkElement;
            data.InitialState = new DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Floating;
            data.InitialState.TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser;
            DockablePaneId dpid = new DockablePaneId(new Guid("{B77218E1-927B-4E18-BF23-016E6EECA726}"));
            app.RegisterDockablePane(dpid, "GTB-Berlin", GtbDockableWindow as IDockablePaneProvider);
        }

        private void ShowDockableWindow(ExternalCommandData commandData)
        {
            DockablePaneId dpid = new DockablePaneId(new Guid("{B77218E1-927B-4E18-BF23-016E6EECA726}"));
            DockablePane dp = commandData.Application.GetDockablePane(dpid);
            dp.Show();

            //commandData.Application.DockableFrameVisibilityChanged += new EventHandler<DockableFrameVisibilityChangedEventArgs>(Application_DockableFrameVisibilityChanged);

            //void Application_DockableFrameVisibilityChanged(object sender, DockableFrameVisibilityChangedEventArgs e)
            //{
            //    if (dp.IsShown())
            //    {
            //        _button.ItemText = "Ausblenden";
            //        PushButton pb = _button as PushButton;
            //        pb.LargeImage = GetEmbeddedImage("Resources.GtbActive.png");
            //        ShowDockableWindow(commandData);
            //    }
            //    else
            //    {
            //        _button.ItemText = "Anzeigen";
            //        PushButton pb = _button as PushButton;
            //        pb.LargeImage = GetEmbeddedImage("Resources.GtbInactive.png");
            //        HideDockableWindow(commandData);
            //    }
            //}
        }

        private void HideDockableWindow(ExternalCommandData commandData)
        {
            DockablePaneId dpid = new DockablePaneId(new Guid("{B77218E1-927B-4E18-BF23-016E6EECA726}"));
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
