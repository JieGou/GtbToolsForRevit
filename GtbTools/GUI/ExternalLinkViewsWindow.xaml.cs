using ExternalLinkControl;
using Functions;
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
    /// Interaction logic for ExternalLinkViewsWindow.xaml
    /// </summary>
    public partial class ExternalLinkViewsWindow : Window
    {
        public RevitLinkViewModel RevitLinkViewModel { get; set; }
        public bool ApplyChanges = false;
        ExternalLinkTool _externalLinkTool;

        public ExternalLinkViewsWindow(Window owner, RevitLinkViewModel revitLinkViewModel, ExternalLinkTool externalLinkTool)
        {
            _externalLinkTool = externalLinkTool;
            Owner = owner;
            RevitLinkViewModel = revitLinkViewModel;
            this.DataContext = RevitLinkViewModel;
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            foreach (RevitViewModel model in DataGridViews.SelectedItems)
            {
                if(!model.IsVisible) model.IsVisible = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (RevitViewModel model in DataGridViews.SelectedItems)
            {
                if (model.IsVisible) model.IsVisible = false;
            }
        }

        private void BtnClick_ClearAll(object sender, RoutedEventArgs e)
        {
            foreach (RevitViewModel model in DataGridViews.Items)
            {
                if (model.IsVisible) model.IsVisible = false;
            }
        }

        private void BtnClick_SelectAll(object sender, RoutedEventArgs e)
        {
            foreach (RevitViewModel model in DataGridViews.Items)
            {
                if (!model.IsVisible) model.IsVisible = true;
            }
        }

        private void BtnClick_Cancel(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnClick_Apply(object sender, RoutedEventArgs e)
        {
            _externalLinkTool.ExternalLinkToolViewModel.EditedLinkViewModel = RevitLinkViewModel;
            _externalLinkTool.Action = ExternalLinkToolAction.Modify;
            _externalLinkTool.TheEvent.Raise();
        }
    }
}
