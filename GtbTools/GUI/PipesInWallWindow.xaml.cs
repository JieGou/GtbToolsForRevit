using Autodesk.Revit.DB;
using OwnerSearch;
using PipesInWall;
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

namespace GUI
{
    /// <summary>
    /// Interaction logic for PipesInWallWindow.xaml
    /// </summary>
    public partial class PipesInWallWindow : Window
    {
        public PipesInWallViewModel PipesInWallViewModel { get; set; }

        public PipesInWallWindow(PipesInWallViewModel pipesInWallViewModel)
        {
            PipesInWallViewModel = pipesInWallViewModel;
            this.DataContext = this;
            SetOwner();
            InitializeComponent();
        }

        private void SetOwner()
        {
            WindowHandleSearch search = WindowHandleSearch.MainWindowHandle;
            search.SetAsOwner(this);
        }

        private void Btn_Click_ArcAnalyze(object sender, RoutedEventArgs e)
        {
            //logic
            PipesInWallViewModel.GetSelectedWallTypes();
            PipesInWallViewModel.GetAllWallInstances();
            BtnTgaAnalyze.IsEnabled = true;
        }

        private void ComboBoxLinks_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PipesInWallViewModel.SelectedLink = ComboBoxLinks.SelectedItem as RevitLinkInstance;
            PipesInWallViewModel.SetWallFamilies();
            WallFamiliesBox.ItemsSource = PipesInWallViewModel.WallFamilies;
            BtnArcAnalyze.IsEnabled = true;
            BtnTgaAnalyze.IsEnabled = false;
            BtnApply.IsEnabled = false;
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            //logic

        }

        private void BtnTgaAnalyze_Click(object sender, RoutedEventArgs e)
        {
            //logic
            PipesInWallViewModel.AnalyzePipes();
            DataGridControlList.ItemsSource = PipesInWallViewModel.PipeViewModels;
            BtnApply.IsEnabled = true;
        }
    }
}
