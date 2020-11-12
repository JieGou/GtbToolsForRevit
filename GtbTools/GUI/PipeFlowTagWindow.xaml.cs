using Autodesk.Revit.DB;
using Functions;
using OwnerSearch;
using PipeFlowTool;
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
    /// Interaction logic for PipeFlowTagWindow.xaml
    /// </summary>
    public partial class PipeFlowTagWindow : Window
    {
        public PipeFlowTagger PipeFlowTagger { get; set; }

        public PipeFlowTagWindow(PipeFlowTagger pipeFlowTagger)
        {
            SetOwner();
            PipeFlowTagger = pipeFlowTagger;
            InitializeComponent();
            SetComBoxes();
        }

        private void SetOwner()
        {
            WindowHandleSearch search = WindowHandleSearch.MainWindowHandle;
            search.SetAsOwner(this);
        }

        private void SetComBoxes()
        {
            ComBoxViaUp.ItemsSource = PipeFlowTagger.PipeFittingTags;
            ComBoxViaUp.DisplayMemberPath = "Name";
            SetSelectedValue(ComBoxViaUp, "durchgehend", "auf");

            ComBoxViaDown.ItemsSource = PipeFlowTagger.PipeFittingTags;
            ComBoxViaDown.DisplayMemberPath = "Name";
            SetSelectedValue(ComBoxViaDown, "durchgehend", "ab");

            ComBoxNachOben.ItemsSource = PipeFlowTagger.PipeFittingTags;
            ComBoxNachOben.DisplayMemberPath = "Name";
            SetSelectedValue(ComBoxNachOben, "nach", "oben");

            ComBoxNachUnten.ItemsSource = PipeFlowTagger.PipeFittingTags;
            ComBoxNachUnten.DisplayMemberPath = "Name";
            SetSelectedValue(ComBoxNachUnten, "nach", "unten");

            ComBoxVonOben.ItemsSource = PipeFlowTagger.PipeFittingTags;
            ComBoxVonOben.DisplayMemberPath = "Name";
            SetSelectedValue(ComBoxVonOben, "von", "oben");

            ComBoxVonUnten.ItemsSource = PipeFlowTagger.PipeFittingTags;
            ComBoxVonUnten.DisplayMemberPath = "Name";
            SetSelectedValue(ComBoxVonUnten, "von", "unten");
        }

        private void SetSelectedValue(ComboBox cbox, string filter1, string filter2)
        {
            FamilySymbol familySymbol = PipeFlowTagger.PipeFittingTags.Where(e => e.Name.ToUpper().Contains(filter1.ToUpper()) && e.Name.ToUpper().Contains(filter2.ToUpper())).FirstOrDefault();
            cbox.SelectedItem = familySymbol;
        }

        private void Btn_Click_Analyze(object sender, RoutedEventArgs e)
        {
            PipeFlowTagger.Action = PipeFlowToolAction.Analyze;
            PipeFlowTagger.StartEvent.Raise();
            PipeFlowTagger.SignalEvent.WaitOne();
            PipeFlowTagger.SignalEvent.Reset();
            PipeFlowTagger.SelectedTags = new List<FamilySymbol>();
            try
            {
                PipeFlowTagger.SelectedTags.Add(ComBoxViaUp.SelectedItem as FamilySymbol);
                PipeFlowTagger.SelectedTags.Add(ComBoxViaDown.SelectedItem as FamilySymbol);
                PipeFlowTagger.SelectedTags.Add(ComBoxNachOben.SelectedItem as FamilySymbol);
                PipeFlowTagger.SelectedTags.Add(ComBoxNachUnten.SelectedItem as FamilySymbol);
                PipeFlowTagger.SelectedTags.Add(ComBoxVonOben.SelectedItem as FamilySymbol);
                PipeFlowTagger.SelectedTags.Add(ComBoxVonUnten.SelectedItem as FamilySymbol);
                Btn_TagThem.IsEnabled = true;
                //sort the list of tags
                PipeFlowTagger.SetTaggedElementIds();
                PipeFlowTagger.CheckExistingTags();
                PipeFlowTagger.SetViewModelList();
                MyDataGrid.DataContext = PipeFlowTagger;
                MyDataGrid.Items.Refresh();
            }
            catch
            {
                MessageBox.Show("Select all types!");
            }
        }

        private void Btn_Click_TagThemAll(object sender, RoutedEventArgs e)
        {
            PipeFlowTagger.Action = PipeFlowToolAction.Tag;
            PipeFlowTagger.StartEvent.Raise();
            PipeFlowTagger.SignalEvent.WaitOne();
            PipeFlowTagger.SignalEvent.Reset();
            Btn_TagThem.IsEnabled = false; ;
        }

        private void Btn_Click_DefaultDirections(object sender, RoutedEventArgs e)
        {
            CustomDirectionsWindow window = new CustomDirectionsWindow(PipeFlowTagger.DefaultDirections, this);
            window.ShowDialog();
        }

        private void MyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LineViewModel item = MyDataGrid.SelectedItem as LineViewModel;
            if (item == null) return;
            PipeFlowTagger.SelectedItem = item.ReferencePipeId;
            PipeFlowTagger.Action = PipeFlowToolAction.Show;
            PipeFlowTagger.StartEvent.Raise();
        }
    }
}
