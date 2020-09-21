﻿using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using System.Windows.Navigation;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.Drawing;
using System.Configuration;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Text;
using GtbTools.Excel;
using ViewModels;

namespace GtbTools.Forms
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class GtbDockPage : Page, Autodesk.Revit.UI.IDockablePaneProvider
    {
        OpeningWindowMainViewModel OpeningWindowMainViewModel { get; set; }

        #region Data
        ExternalEvent _exEventCopyCoords;
        ExternalEvent _exEventOpenViews;
        ExternalEvent _exEventSaveCoords;
        ExternalEvent _exEventLoadCoords;
        ExternalEvent _exEventExcel;
        ExternalEvent _exEventSymbols;
        ExternalEvent _wallSymbols;
        ExternalEvent _floorSymbols;
        ExternalEvent _roofSymbols;
        ExternalEvent _mepExtract;
        private Guid m_targetGuid;
        private DockPosition m_position = DockPosition.Bottom;
        private int m_left = 1;
        private int m_right = 1;
        private int m_top = 1;
        private int m_bottom = 1;
        #endregion
        public GtbDockPage(string plugInVersion, ExternalEvent exEventCopyCoords, ExternalEvent exEventOpenViews, ExternalEvent exEventSaveCoords, ExternalEvent exEventLoadCoords, ExternalEvent exEventExcel, ExternalEvent exEventSymbols, ExternalEvent wallSymbols, ExternalEvent floorSymbols, ExternalEvent roofSymbols, ExternalEvent mepExtract)
        {
            _exEventCopyCoords = exEventCopyCoords;
            _exEventOpenViews = exEventOpenViews;
            _exEventLoadCoords = exEventLoadCoords;
            _exEventSaveCoords = exEventSaveCoords;
            _exEventExcel = exEventExcel;
            _exEventSymbols = exEventSymbols;
            _wallSymbols = wallSymbols;
            _floorSymbols = floorSymbols;
            _roofSymbols = roofSymbols;
            _mepExtract = mepExtract;
            InitializeComponent();
            LblVersion.Content += plugInVersion;
        }
        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this as FrameworkElement;
            data.InitialState = new Autodesk.Revit.UI.DockablePaneState();
            data.InitialState.DockPosition = DockPosition.Floating;
            //DockablePaneId targetPane;
            //if (m_targetGuid == Guid.Empty)
            //    targetPane = null;
            //else targetPane = new DockablePaneId(m_targetGuid);
            //if (m_position == DockPosition.Tabbed)
            //data.InitialState.TabBehind = Autodesk.Revit.UI.DockablePanes.BuiltInDockablePanes.ProjectBrowser;
            //if (m_position == DockPosition.Floating)
            //{
            data.InitialState.SetFloatingRectangle(new Autodesk.Revit.DB.Rectangle(100, 100, 360, 540));
            //data.InitialState.DockPosition = DockPosition.Tabbed;
            //}
            //Log.Message("***Intial docking parameters***");
            //Log.Message(APIUtility.GetDockStateSummary(data.InitialState));
        }
        public void SetInitialDockingParameters(int left, int right, int top, int bottom, DockPosition position, Guid targetGuid)
        {
            m_position = position;
            m_left = left;
            m_right = right;
            m_top = top;
            m_bottom = bottom;
            m_targetGuid = targetGuid;
        }

        private void DockableDialogs_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_OpenViews(object sender, RoutedEventArgs e)
        {
            _exEventOpenViews.Raise();
        }

        private void Button_Click_CopyCoords(object sender, RoutedEventArgs e)
        {
            _exEventCopyCoords.Raise();
        }

        private void Button_Click_SaveCoords(object sender, RoutedEventArgs e)
        {
            _exEventSaveCoords.Raise();
        }

        private void Button_Click_LoadCoords(object sender, RoutedEventArgs e)
        {
            _exEventLoadCoords.Raise();
        }

        private void Button_Click_ExcelDataImport(object sender, RoutedEventArgs e)
        {
            _exEventExcel.Raise();
        }

        private void SymbolMainWindow_Click(object sender, RoutedEventArgs e)
        {
            _exEventSymbols.Raise();
        }

        private void SelectAllFloor_Click(object sender, RoutedEventArgs e)
        {
            _floorSymbols.Raise();
        }

        private void SelectAllWall_Click(object sender, RoutedEventArgs e)
        {
            _wallSymbols.Raise();
        }

        private void SelectAllRoof_Click(object sender, RoutedEventArgs e)
        {
            _roofSymbols.Raise();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _mepExtract.Raise();
        }
    }
}
