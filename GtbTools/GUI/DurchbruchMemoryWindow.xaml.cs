using Autodesk.Revit.UI;
using GtbTools;
using OwnerSearch;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Threading;
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
            SetLabels();
        }

        private void SetLabels()
        {
            string newLbl = String.Format("{0} new Durchbruche", DurchbruchMemoryViewModel.NewDurchbruche.Count);
            string movedLbl = String.Format("{0} moved Durchbruche", DurchbruchMemoryViewModel.MovedDurchbruche.Count);
            string resLbl = String.Format("{0} resized Durchbruche", DurchbruchMemoryViewModel.ResizedDurchbruche.Count);
            string movresLbl = String.Format("{0} moved and resized", DurchbruchMemoryViewModel.MovedAndResizedDurchbruche.Count);
            lblNew.Content = newLbl;
            lblMoved.Content = movedLbl;
            lblResized.Content = resLbl;
            lblMovRes.Content = movresLbl;
            if (DurchbruchMemoryViewModel.NewDurchbruche.Count > 0) lblNew.FontWeight = FontWeights.Bold;
            if (DurchbruchMemoryViewModel.MovedDurchbruche.Count > 0) lblMoved.FontWeight = FontWeights.Bold;
            if (DurchbruchMemoryViewModel.ResizedDurchbruche.Count > 0) lblResized.FontWeight = FontWeights.Bold;
            if (DurchbruchMemoryViewModel.MovedAndResizedDurchbruche.Count > 0) lblMovRes.FontWeight = FontWeights.Bold;
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
            DurchbruchMemoryViewModel.SaveAllToStorage = false;
            DurchbruchMemoryViewModel.SaveDataToExStorageEvent.Raise();
            DurchbruchMemoryViewModel.SignalEvent.WaitOne();
            DurchbruchMemoryViewModel.SignalEvent.Reset();
            SetLabels();
        }

        private void Btn_Click_SaveAll(object sender, RoutedEventArgs e)
        {
            DurchbruchMemoryViewModel.SaveAllToStorage = true;
            DurchbruchMemoryViewModel.SaveDataToExStorageEvent.Raise();
            DurchbruchMemoryViewModel.SignalEvent.WaitOne();
            DurchbruchMemoryViewModel.SignalEvent.Reset();
            SetLabels();
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

        private void DataGridResized_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //MessageBox.Show("Ended editing");
            DataGrid item = (DataGrid)sender;
            ResizedDurchbruchViewModel sendingClass = (ResizedDurchbruchViewModel)item.SelectedItem;
            DurchbruchMemoryViewModel.CurrentModel = sendingClass.DurchbruchModel;
            string id = sendingClass.ElementId;
            string shape = sendingClass.Shape;
            string pipeDiameter = sendingClass.PipeDiameter;
            string offset = sendingClass.Offset;

            //MessageBox.Show(id + ", " + shape + ", " + pipeDiameter + ", " + offset);
            //regex
            //value changed

            double metricOffset = sendingClass.DurchbruchModel.CutOffset.AsDouble() * 304.8;
            string offsetString = metricOffset.ToString("F1", CultureInfo.InvariantCulture);

            if (shape == "Rectangular")
            {
                //MessageBox.Show("Editing is not allowed for rectangular openings!", "Info");
                sendingClass.Offset = offsetString;
                sendingClass.PipeDiameter = "---";
                return;
            }

            double metricPipeDiameter = sendingClass.DurchbruchModel.PipeDiameter.AsDouble() * 304.8;
            string pipeDiameterString = metricPipeDiameter.ToString("F1", CultureInfo.InvariantCulture);

            if (offset != offsetString)
            {
                //MessageBox.Show("Changed offset");
                double number;
                bool result = double.TryParse(offset, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
                if (!result)
                {
                    MessageBox.Show("Der eingefügte Wert muss eine Zahl sein!");
                    sendingClass.Offset = offsetString;
                    return;
                }
                if(number < 0)
                {
                    MessageBox.Show("Wert kann nicht kleiner als null sein!");
                    sendingClass.Offset = offsetString;
                }
                else
                {
                    string newOffset = number.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Offset = newOffset;
                    //calculate new total D in gridtable
                    double diameterNumber = Convert.ToDouble(pipeDiameter, CultureInfo.InvariantCulture);
                    double totalDiameter = diameterNumber + 2 * number;
                    string totalDiameterString = totalDiameter.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Diameter = totalDiameterString;
                    //raise event here to change offset
                    DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.SetNewOffset;
                    DurchbruchMemoryViewModel.CurrentModel.NewOffset = number;
                    DurchbruchMemoryViewModel.ChangeOffsetEvent.Raise();
                }

            }
            if (pipeDiameter != pipeDiameterString)
            {
                //MessageBox.Show("Changed pipe diameter");
                double number;
                bool result = double.TryParse(pipeDiameter, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
                if (!result)
                {
                    MessageBox.Show("Der eingefügte Wert muss eine Zahl sein!");
                    sendingClass.PipeDiameter = pipeDiameterString; ;
                    return;
                }
                if (number > 0)
                {
                    string newPipeDiameter = number.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.PipeDiameter = newPipeDiameter;
                    //calculate new total D in gridtable
                    double offsetNumber = Convert.ToDouble(offset, CultureInfo.InvariantCulture);
                    double totalDiameter = number + 2 * offsetNumber;
                    string totalDiameterString = totalDiameter.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Diameter = totalDiameterString;
                    //raise event here to change offset
                    DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.SetNewDiameter;
                    DurchbruchMemoryViewModel.CurrentModel.NewDiameter = number;
                    DurchbruchMemoryViewModel.ChangeDiameterEvent.Raise();
                }
                else
                {
                    MessageBox.Show("Wert muss größer als null sein!");
                    sendingClass.PipeDiameter = pipeDiameterString;
                }
            }
        }

        private void DataGridNew_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //MessageBox.Show("Ended editing");
            DataGrid item = (DataGrid)sender;
            NewDurchbruchViewModel sendingClass = (NewDurchbruchViewModel)item.SelectedItem;
            DurchbruchMemoryViewModel.CurrentModel = sendingClass.DurchbruchModel;
            string id = sendingClass.ElementId;
            string shape = sendingClass.Shape;
            string pipeDiameter = sendingClass.PipeDiameter;
            string offset = sendingClass.Offset;

            //MessageBox.Show(id + ", " + shape + ", " + pipeDiameter + ", " + offset);
            //regex
            //value changed

            double metricOffset = sendingClass.DurchbruchModel.CutOffset.AsDouble() * 304.8;
            string offsetString = metricOffset.ToString("F1", CultureInfo.InvariantCulture);

            if (shape == "Rectangular")
            {
                //MessageBox.Show("Editing is not allowed for rectangular openings!", "Info");
                sendingClass.Offset = offsetString;
                sendingClass.PipeDiameter = "---";
                return;
            }

            double metricPipeDiameter = sendingClass.DurchbruchModel.PipeDiameter.AsDouble() * 304.8;
            string pipeDiameterString = metricPipeDiameter.ToString("F1", CultureInfo.InvariantCulture);

            if (offset != offsetString)
            {
                //MessageBox.Show("Changed offset");
                double number;
                bool result = double.TryParse(offset, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
                if (!result)
                {
                    MessageBox.Show("Der eingefügte Wert muss eine Zahl sein!");
                    sendingClass.Offset = offsetString;
                    return;
                }
                if (number < 0)
                {
                    MessageBox.Show("Wert kann nicht kleiner als null sein!");
                    sendingClass.Offset = offsetString;
                }
                else
                {
                    string newOffset = number.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Offset = newOffset;
                    //calculate new total D in gridtable
                    double diameterNumber = Convert.ToDouble(pipeDiameter, CultureInfo.InvariantCulture);
                    double totalDiameter = diameterNumber + 2 * number;
                    string totalDiameterString = totalDiameter.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Diameter = totalDiameterString;
                    //raise event here to change offset
                    DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.SetNewOffset;
                    DurchbruchMemoryViewModel.CurrentModel.NewOffset = number;
                    DurchbruchMemoryViewModel.ChangeOffsetEvent.Raise();
                }

            }
            if (pipeDiameter != pipeDiameterString)
            {
                //MessageBox.Show("Changed pipe diameter");
                double number;
                bool result = double.TryParse(pipeDiameter, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
                if (!result)
                {
                    MessageBox.Show("Der eingefügte Wert muss eine Zahl sein!");
                    sendingClass.PipeDiameter = pipeDiameterString; ;
                    return;
                }
                if (number > 0)
                {
                    string newPipeDiameter = number.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.PipeDiameter = newPipeDiameter;
                    //calculate new total D in gridtable
                    double offsetNumber = Convert.ToDouble(offset, CultureInfo.InvariantCulture);
                    double totalDiameter = number + 2 * offsetNumber;
                    string totalDiameterString = totalDiameter.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Diameter = totalDiameterString;
                    //raise event here to change offset
                    DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.SetNewDiameter;
                    DurchbruchMemoryViewModel.CurrentModel.NewDiameter = number;
                    DurchbruchMemoryViewModel.ChangeDiameterEvent.Raise();
                }
                else
                {
                    MessageBox.Show("Wert muss größer als null sein!");
                    sendingClass.PipeDiameter = pipeDiameterString;
                }
            }
        }

        private void DataGridMoved_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //MessageBox.Show("Ended editing");
            DataGrid item = (DataGrid)sender;
            MovedAndResizedDbViewModel sendingClass = (MovedAndResizedDbViewModel)item.SelectedItem;
            DurchbruchMemoryViewModel.CurrentModel = sendingClass.DurchbruchModel;
            string id = sendingClass.ElementId;
            string shape = sendingClass.Shape;
            string pipeDiameter = sendingClass.PipeDiameter;
            string offset = sendingClass.Offset;

            //MessageBox.Show(id + ", " + shape + ", " + pipeDiameter + ", " + offset);
            //regex
            //value changed

            double metricOffset = sendingClass.DurchbruchModel.CutOffset.AsDouble() * 304.8;
            string offsetString = metricOffset.ToString("F1", CultureInfo.InvariantCulture);

            if (shape == "Rectangular")
            {
                //MessageBox.Show("Editing is not allowed for rectangular openings!", "Info");
                sendingClass.Offset = offsetString;
                sendingClass.PipeDiameter = "---";
                return;
            }

            double metricPipeDiameter = sendingClass.DurchbruchModel.PipeDiameter.AsDouble() * 304.8;
            string pipeDiameterString = metricPipeDiameter.ToString("F1", CultureInfo.InvariantCulture);

            if (offset != offsetString)
            {
                //MessageBox.Show("Changed offset");
                double number;
                bool result = double.TryParse(offset, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
                if (!result)
                {
                    MessageBox.Show("Der eingefügte Wert muss eine Zahl sein!");
                    sendingClass.Offset = offsetString;
                    return;
                }
                if (number < 0)
                {
                    MessageBox.Show("Wert kann nicht kleiner als null sein!");
                    sendingClass.Offset = offsetString;
                }
                else
                {
                    string newOffset = number.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Offset = newOffset;
                    //calculate new total D in gridtable
                    double diameterNumber = Convert.ToDouble(pipeDiameter, CultureInfo.InvariantCulture);
                    double totalDiameter = diameterNumber + 2 * number;
                    string totalDiameterString = totalDiameter.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Diameter = totalDiameterString;
                    //raise event here to change offset
                    DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.SetNewOffset;
                    DurchbruchMemoryViewModel.CurrentModel.NewOffset = number;
                    DurchbruchMemoryViewModel.ChangeOffsetEvent.Raise();
                }

            }
            if (pipeDiameter != pipeDiameterString)
            {
                //MessageBox.Show("Changed pipe diameter");
                double number;
                bool result = double.TryParse(pipeDiameter, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
                if (!result)
                {
                    MessageBox.Show("Der eingefügte Wert muss eine Zahl sein!");
                    sendingClass.PipeDiameter = pipeDiameterString; ;
                    return;
                }
                if (number > 0)
                {
                    string newPipeDiameter = number.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.PipeDiameter = newPipeDiameter;
                    //calculate new total D in gridtable
                    double offsetNumber = Convert.ToDouble(offset, CultureInfo.InvariantCulture);
                    double totalDiameter = number + 2 * offsetNumber;
                    string totalDiameterString = totalDiameter.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Diameter = totalDiameterString;
                    //raise event here to change offset
                    DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.SetNewDiameter;
                    DurchbruchMemoryViewModel.CurrentModel.NewDiameter = number;
                    DurchbruchMemoryViewModel.ChangeDiameterEvent.Raise();
                }
                else
                {
                    MessageBox.Show("Wert muss größer als null sein!");
                    sendingClass.PipeDiameter = pipeDiameterString;
                }
            }
        }

        private void DataGridMovedAndResized_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            //MessageBox.Show("Ended editing");
            DataGrid item = (DataGrid)sender;
            MovedAndResizedDbViewModel sendingClass = (MovedAndResizedDbViewModel)item.SelectedItem;
            DurchbruchMemoryViewModel.CurrentModel = sendingClass.DurchbruchModel;
            string id = sendingClass.ElementId;
            string shape = sendingClass.Shape;
            string pipeDiameter = sendingClass.PipeDiameter;
            string offset = sendingClass.Offset;

            //MessageBox.Show(id + ", " + shape + ", " + pipeDiameter + ", " + offset);
            //regex
            //value changed

            double metricOffset = sendingClass.DurchbruchModel.CutOffset.AsDouble() * 304.8;
            string offsetString = metricOffset.ToString("F1", CultureInfo.InvariantCulture);

            if (shape == "Rectangular")
            {
                //MessageBox.Show("Editing is not allowed for rectangular openings!", "Info");
                sendingClass.Offset = offsetString;
                sendingClass.PipeDiameter = "---";
                return;
            }

            double metricPipeDiameter = sendingClass.DurchbruchModel.PipeDiameter.AsDouble() * 304.8;
            string pipeDiameterString = metricPipeDiameter.ToString("F1", CultureInfo.InvariantCulture);

            if (offset != offsetString)
            {
                //MessageBox.Show("Changed offset");
                double number;
                bool result = double.TryParse(offset, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
                if (!result)
                {
                    MessageBox.Show("Der eingefügte Wert muss eine Zahl sein!");
                    sendingClass.Offset = offsetString;
                    return;
                }
                if (number < 0)
                {
                    MessageBox.Show("Wert kann nicht kleiner als null sein!");
                    sendingClass.Offset = offsetString;
                }
                else
                {
                    string newOffset = number.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Offset = newOffset;
                    //calculate new total D in gridtable
                    double diameterNumber = Convert.ToDouble(pipeDiameter, CultureInfo.InvariantCulture);
                    double totalDiameter = diameterNumber + 2 * number;
                    string totalDiameterString = totalDiameter.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Diameter = totalDiameterString;
                    //raise event here to change offset
                    DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.SetNewOffset;
                    DurchbruchMemoryViewModel.CurrentModel.NewOffset = number;
                    DurchbruchMemoryViewModel.ChangeOffsetEvent.Raise();
                }

            }
            if (pipeDiameter != pipeDiameterString)
            {
                //MessageBox.Show("Changed pipe diameter");
                double number;
                bool result = double.TryParse(pipeDiameter, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
                if (!result)
                {
                    MessageBox.Show("Der eingefügte Wert muss eine Zahl sein!");
                    sendingClass.PipeDiameter = pipeDiameterString; ;
                    return;
                }
                if (number > 0)
                {
                    string newPipeDiameter = number.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.PipeDiameter = newPipeDiameter;
                    //calculate new total D in gridtable
                    double offsetNumber = Convert.ToDouble(offset, CultureInfo.InvariantCulture);
                    double totalDiameter = number + 2 * offsetNumber;
                    string totalDiameterString = totalDiameter.ToString("F1", CultureInfo.InvariantCulture);
                    sendingClass.Diameter = totalDiameterString;
                    //raise event here to change offset
                    DurchbruchMemoryViewModel.DurchbruchMemoryAction = DurchbruchMemoryAction.SetNewDiameter;
                    DurchbruchMemoryViewModel.CurrentModel.NewDiameter = number;
                    DurchbruchMemoryViewModel.ChangeDiameterEvent.Raise();
                }
                else
                {
                    MessageBox.Show("Wert muss größer als null sein!");
                    sendingClass.PipeDiameter = pipeDiameterString;
                }
            }
        }
    }
}
