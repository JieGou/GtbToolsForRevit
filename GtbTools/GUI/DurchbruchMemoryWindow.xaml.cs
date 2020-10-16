using GtbTools;
using OwnerSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ViewModels;

namespace GUI
{
    /// <summary>
    /// Interaction logic for DurchbruchMemoryWindow.xaml
    /// </summary>
    public partial class DurchbruchMemoryWindow : Window
    {
        public DurchbruchMemoryViewModel DurchbruchMemoryViewModel { get; set; }

        public DurchbruchMemoryWindow(DurchbruchMemoryViewModel durchbruchMemoryViewModel)
        {
            SetOwner();
            DurchbruchMemoryViewModel = durchbruchMemoryViewModel;
            InitializeComponent();
            DataGridNew.DataContext = this.DurchbruchMemoryViewModel;
            DataGridMoved.DataContext = this.DurchbruchMemoryViewModel;
            DataGridResized.DataContext = this.DurchbruchMemoryViewModel;
            DataGridMovedAndResized.DataContext = this.DurchbruchMemoryViewModel;
        }

        private void SetOwner()
        {
            WindowHandleSearch revitHandleSearch = WindowHandleSearch.MainWindowHandle;
            revitHandleSearch.SetAsOwner(this);
        }

        private void Btn_Click_ClearAll(object sender, RoutedEventArgs e)
        {
            DataGridResized.UnselectAll();
            DataGridNew.UnselectAll();
            DataGridMoved.UnselectAll();
            DataGridMovedAndResized.UnselectAll();
        }

        private void Btn_Click_SaveNew(object sender, RoutedEventArgs e)
        {
            DurchbruchMemoryViewModel.SaveDataToExStorageEvent.Raise();
        }

        private void Btn_Click_SaveAll(object sender, RoutedEventArgs e)
        {
            DurchbruchMemoryViewModel.SaveAllToStorage = true;
            DurchbruchMemoryViewModel.SaveDataToExStorageEvent.Raise();
        }

        private void BtnClick_NewDurchBruchViews(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            NewDurchbruchViewModel sendingClass = (NewDurchbruchViewModel)button.DataContext;
            DurchbruchViews durchbruchViews = new DurchbruchViews(sendingClass.Views, this);
            durchbruchViews.DurchbruchMemoryViewModel = DurchbruchMemoryViewModel;
            durchbruchViews.ShowDialog();
        }
        private void BtnClick_MovedDurchBruchViews(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MovedAndResizedDbViewModel sendingClass = (MovedAndResizedDbViewModel)button.DataContext;
            DurchbruchViews durchbruchViews = new DurchbruchViews(sendingClass.Views, this);
            durchbruchViews.DurchbruchMemoryViewModel = DurchbruchMemoryViewModel;
            durchbruchViews.ShowDialog();
        }
        private void BtnClick_ResizedDurchBruchViews(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ResizedDurchbruchViewModel sendingClass = (ResizedDurchbruchViewModel)button.DataContext;
            DurchbruchViews durchbruchViews = new DurchbruchViews(sendingClass.Views, this);
            durchbruchViews.DurchbruchMemoryViewModel = DurchbruchMemoryViewModel;
            durchbruchViews.ShowDialog();
        }

        private void BtnClick_MovedAndResized(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MovedAndResizedDbViewModel sendingClass = (MovedAndResizedDbViewModel)button.DataContext;
            DurchbruchViews durchbruchViews = new DurchbruchViews(sendingClass.Views, this);
            durchbruchViews.DurchbruchMemoryViewModel = DurchbruchMemoryViewModel;
            durchbruchViews.ShowDialog();
        }

        private void DataGridResized_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResizedDurchbruchViewModel item = (ResizedDurchbruchViewModel)DataGridResized.SelectedItem;
            if (item == null) return;
            DurchbruchMemoryViewModel.CurrentSelection = item.DurchbruchModel.ElementId;
            DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.ShowElement;
            DurchbruchMemoryViewModel.SignalEvent.Set();
            DurchbruchMemoryViewModel.ShowElementEvent.Raise();
        }

        private void DataGridMoved_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MovedAndResizedDbViewModel item = (MovedAndResizedDbViewModel)DataGridMoved.SelectedItem;
            if (item == null) return;
            DurchbruchMemoryViewModel.CurrentSelection = item.DurchbruchModel.ElementId;
            DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.ShowElement;
            DurchbruchMemoryViewModel.SignalEvent.Set();
            DurchbruchMemoryViewModel.ShowElementEvent.Raise();
            //raise event to create a ball or a line
        }

        private void DataGridNew_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NewDurchbruchViewModel item = (NewDurchbruchViewModel)DataGridNew.SelectedItem;
            if (item == null) return;
            DurchbruchMemoryViewModel.CurrentSelection = item.DurchbruchModel.ElementId;
            DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.ShowElement;
            DurchbruchMemoryViewModel.SignalEvent.Set();
            DurchbruchMemoryViewModel.ShowElementEvent.Raise();
        }

        private void DataGridMovedAndResized_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MovedAndResizedDbViewModel item = (MovedAndResizedDbViewModel)DataGridMovedAndResized.SelectedItem;
            if (item == null) return;
            DurchbruchMemoryViewModel.CurrentSelection = item.DurchbruchModel.ElementId;
            DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.ShowElement;
            DurchbruchMemoryViewModel.SignalEvent.Set();
            DurchbruchMemoryViewModel.ShowElementEvent.Raise();
        }

        private void ScrViewerMoved_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 7);
            e.Handled = true;
        }

        private void ScrViewerNew_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 7);
            e.Handled = true;
        }

        private void ScrViewerResized_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 7);
            e.Handled = true;
        }

        private void ScrViewerMovedAndResized_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta / 7);
            e.Handled = true;
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            DataGridCell item = (DataGridCell)sender;
            MovedAndResizedDbViewModel sendingClass = (MovedAndResizedDbViewModel)item.DataContext;
            DurchbruchMemoryViewModel.CurrentItem = sendingClass;
            DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.ShowPosition;
            DurchbruchMemoryViewModel.SignalEvent.Set();
            DurchbruchMemoryViewModel.ShowElementEvent.Raise();
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            DataGridCell item = (DataGridCell)sender;
            MovedAndResizedDbViewModel sendingClass = (MovedAndResizedDbViewModel)item.DataContext;
            DurchbruchMemoryViewModel.CurrentItem = sendingClass;
            DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.DeletePosition;
            DurchbruchMemoryViewModel.SignalEvent.Set();
            DurchbruchMemoryViewModel.ShowElementEvent.Raise();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DurchbruchMemoryViewModel.OldPositionMarkers.Count > 0)
            {
                DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.DeleteRemainingMarkers;
                DurchbruchMemoryViewModel.SignalEvent.Set();
                DurchbruchMemoryViewModel.ShowElementEvent.Raise();
            }
        }
    }
}
