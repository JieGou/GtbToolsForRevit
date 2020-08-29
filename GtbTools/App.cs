using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

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
        PushButtonData _pushButtonData;

        /// <summary>
        /// Singleton external application class instance.
        /// </summary>
        internal static App _app = null;

        /// <summary>
        /// Provide access to singleton class instance.
        /// </summary>
        public static App Instance
        {
            get { return _app; }
        }

        public Result OnStartup(UIControlledApplication application)
        {
            _app = this;
            string path = Assembly.GetExecutingAssembly().Location;
            RibbonPanel gtbPanel = application.CreateRibbonPanel("GTB - Berlin");

            PushButtonData pushButtonGtbPanelControl = new PushButtonData( "GTB", "Anzeigen", path, "GtbTools.ImportCsvTables");
            pushButtonGtbPanelControl.LargeImage = new BitmapImage(new Uri(@"C:\Users\Work\source\repos\GtbTools\GtbTools\Resources\GtbInactive.png"));

            _button = gtbPanel.AddItem(pushButtonGtbPanelControl);
            _pushButtonData = pushButtonGtbPanelControl;

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public void Toggle()
        {
            if(_button.ItemText == "Anzeigen")
            {
                _button.ItemText = "Ausblenden";
                PushButton pb = _button as PushButton;
                pb.LargeImage = new BitmapImage(new Uri(@"C:\Users\Work\source\repos\GtbTools\GtbTools\Resources\GtbActive.png"));

                //_pushButtonData.LargeImage = new BitmapImage(new Uri(@"C:\Users\Work\source\repos\GtbTools\GtbTools\Resources\GtbActive.png"));

            }
            else
            {
                _button.ItemText = "Anzeigen";
                PushButton pb = _button as PushButton;
                pb.LargeImage = new BitmapImage(new Uri(@"C:\Users\Work\source\repos\GtbTools\GtbTools\Resources\GtbInactive.png"));
                //_pushButtonData.LargeImage = new BitmapImage(new Uri(@"C:\Users\Work\source\repos\GtbTools\GtbTools\Resources\GtbInactive.png"));
            }
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
