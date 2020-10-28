using FamilyTools;
using OwnerSearch;
using System.Windows;

namespace GUI
{
    /// <summary>
    /// Interaction logic for FamilyToolsWindow.xaml
    /// </summary>
    public partial class FamilyToolsWindow : Window
    {
        CheckboxLabelReplace _checkboxLabelReplace;
        public FamilyToolsWindow(CheckboxLabelReplace checkboxLabelReplace)
        {
            _checkboxLabelReplace = checkboxLabelReplace;
            SetOwner();
            InitializeComponent();
        }

        private void SetOwner()
        {
            WindowHandleSearch search = WindowHandleSearch.MainWindowHandle;
            search.SetAsOwner(this);
        }

        private void Btn_Click_AddParameter(object sender, RoutedEventArgs e)
        {
            _checkboxLabelReplace.AddParameter();
        }

        private void Btn_Click_FindTypesByName(object sender, RoutedEventArgs e)
        {
            _checkboxLabelReplace.AlignTypesWithSymbol();
        }

        private void Btn_Click_DeleteSetLabels(object sender, RoutedEventArgs e)
        {
            _checkboxLabelReplace.DeleteAndSetLabel();
        }

        private void Btn_Click_3in1(object sender, RoutedEventArgs e)
        {
            _checkboxLabelReplace.AddParameter();
            _checkboxLabelReplace.AlignTypesByVisibility();
            _checkboxLabelReplace.DeleteAndSetLabel();
        }

        private void Btn_Click_FindTypesByVisibility(object sender, RoutedEventArgs e)
        {
            _checkboxLabelReplace.AlignTypesByVisibility();
        }
    }
}
