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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(DurchbruchMemoryViewModel.NewDurchbruche.Count.ToString());
        }

        private void BtnClick_NewDurchBruchViews(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            NewDurchbruchViewModel sendingClass = (NewDurchbruchViewModel)button.DataContext;
            string info = "";
            foreach (ModelView mv in sendingClass.Views)
            {
                info += mv.Name + Environment.NewLine;
            }
            MessageBox.Show(info);
        }
        private void BtnClick_MovedDurchBruchViews(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            MovedDurchbruchViewModel sendingClass = (MovedDurchbruchViewModel)button.DataContext;
            string info = "";
            foreach (ModelView mv in sendingClass.Views)
            {
                info += mv.Name + Environment.NewLine;
            }
            MessageBox.Show(info);
        }
        private void BtnClick_ResizedDurchBruchViews(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ResizedDurchbruchViewModel sendingClass = (ResizedDurchbruchViewModel)button.DataContext;
            string info = "";
            foreach (ModelView mv in sendingClass.Views)
            {
                info += mv.Name + Environment.NewLine;
            }
            MessageBox.Show(info);
        }

        private void DataGridResized_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ResizedDurchbruchViewModel item = (ResizedDurchbruchViewModel)DataGridResized.SelectedItem;
            DurchbruchMemoryViewModel.CurrentSelection = item.DurchbruchModel.ElementId;
            DurchbruchMemoryViewModel.ShowElementEvent.Raise();
        }
    }
}
