using GtbTools;
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
            DurchbruchMemoryViewModel = durchbruchMemoryViewModel;
            InitializeComponent();
            DataGridNew.DataContext = this.DurchbruchMemoryViewModel;
            DataGridMoved.DataContext = this.DurchbruchMemoryViewModel;
            DataGridResized.DataContext = this.DurchbruchMemoryViewModel;
            DataGridMovedAndResized.DataContext = this.DurchbruchMemoryViewModel;
        }

        private void Btn_Click_ClearAll(object sender, RoutedEventArgs e)
        {
            DataGridResized.UnselectAll();
            DataGridNew.UnselectAll();
            DataGridMoved.UnselectAll();
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
            DurchbruchViews durchbruchViews = new DurchbruchViews(sendingClass.Views);
            durchbruchViews.DurchbruchMemoryViewModel = DurchbruchMemoryViewModel;
            durchbruchViews.ShowDialog();
        }
        private void BtnClick_MovedDurchBruchViews(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MovedDurchbruchViewModel sendingClass = (MovedDurchbruchViewModel)button.DataContext;
            DurchbruchViews durchbruchViews = new DurchbruchViews(sendingClass.Views);
            durchbruchViews.DurchbruchMemoryViewModel = DurchbruchMemoryViewModel;
            durchbruchViews.ShowDialog();
        }
        private void BtnClick_ResizedDurchBruchViews(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ResizedDurchbruchViewModel sendingClass = (ResizedDurchbruchViewModel)button.DataContext;
            DurchbruchViews durchbruchViews = new DurchbruchViews(sendingClass.Views);
            durchbruchViews.DurchbruchMemoryViewModel = DurchbruchMemoryViewModel;
            durchbruchViews.ShowDialog();
        }

        private void DataGridResized_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResizedDurchbruchViewModel item = (ResizedDurchbruchViewModel)DataGridResized.SelectedItem;
            if (item == null) return;
            DurchbruchMemoryViewModel.CurrentSelection = item.DurchbruchModel.ElementId;
            DurchbruchMemoryViewModel.ShowElementEvent.Raise();
        }

        private void DataGridMoved_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MovedDurchbruchViewModel item = (MovedDurchbruchViewModel)DataGridMoved.SelectedItem;
            if (item == null) return;
            DurchbruchMemoryViewModel.CurrentSelection = item.DurchbruchModel.ElementId;
            DurchbruchMemoryViewModel.ShowElementEvent.Raise();
            //raise event to create a ball or a line
        }

        private void DataGridNew_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NewDurchbruchViewModel item = (NewDurchbruchViewModel)DataGridNew.SelectedItem;
            if (item == null) return;
            DurchbruchMemoryViewModel.CurrentSelection = item.DurchbruchModel.ElementId;
            DurchbruchMemoryViewModel.ShowElementEvent.Raise();
        }

    }
}
