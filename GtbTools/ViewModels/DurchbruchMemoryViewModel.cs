using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ViewModels
{
    public class DurchbruchMemoryViewModel : INotifyPropertyChanged
    {
        public UIApplication UIApplication { get; set; }
        public UIDocument UIDocument {get; set;}
        public Document Document { get; set; }
        public DurchbruchModel CurrentModel { get; set; }
        public ElementId CurrentSelection { get; set; }
        public MovedAndResizedDbViewModel CurrentItem { get; set; }
        public View DesiredView { get; set; }
        public ExternalEvent ChangeDiameterEvent { get; set; }
        public ExternalEvent ChangeOffsetEvent { get; set; }
        public ExternalEvent LoadContextEvent { get; set; }
        public ExternalEvent ShowElementEvent { get; set; }
        public ExternalEvent OpenViewEvent { get; set; }
        public ExternalEvent SaveDataToExStorageEvent { get; set; }
        public List<NewDurchbruchViewModel> _newDurchbruche;
        public List<NewDurchbruchViewModel> NewDurchbruche
        {
            get => _newDurchbruche;
            set
            {
                if (_newDurchbruche != value)
                {
                    _newDurchbruche = value;
                    OnPropertyChanged(nameof(NewDurchbruche));
                }
            }
        }
        public List<MovedAndResizedDbViewModel> _movedDurchbruche;
        public List<MovedAndResizedDbViewModel> MovedDurchbruche
        {
            get => _movedDurchbruche;
            set
            {
                if (_movedDurchbruche != value)
                {
                    _movedDurchbruche = value;
                    OnPropertyChanged(nameof(MovedDurchbruche));
                }
            }
        }
        public List<ResizedDurchbruchViewModel> _resizedDurchbruche;
        public List<ResizedDurchbruchViewModel> ResizedDurchbruche
        {
            get => _resizedDurchbruche;
            set
            {
                if (_resizedDurchbruche != value)
                {
                    _resizedDurchbruche = value;
                    OnPropertyChanged(nameof(ResizedDurchbruche));
                }
            }
        }
        public List<MovedAndResizedDbViewModel> _movedAndResizedDurchbruche;
        public List<MovedAndResizedDbViewModel> MovedAndResizedDurchbruche
        {
            get => _movedAndResizedDurchbruche;
            set
            {
                if (_movedAndResizedDurchbruche != value)
                {
                    _movedAndResizedDurchbruche = value;
                    OnPropertyChanged(nameof(MovedAndResizedDurchbruche));
                }
            }
        }
        public bool SaveAllToStorage { get; set; } = false;
        public DurchbruchMemoryAction DurchbruchMemoryAction { get; set; }

        public ManualResetEvent SignalEvent = new ManualResetEvent(false);

        List<FamilyInstance> _familyInstancesAll;
        List<DurchbruchModel> _modelDurchbrucheAll;
        public List<int> OldPositionMarkers { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DurchbruchMemoryViewModel()
        {
            OldPositionMarkers = new List<int>();
        }

        public void InitializeDurchbruche()
        {
            SetAllDurchbruche();
            SetAllModelDurchbruche();
            SetChangedDurchbruche();
        }

        public void UpdateDurchbruche()
        {
            SetAllDurchbruche();
            SetAllModelDurchbruche();
            SetChangedDurchbruche();
        }

        public void LoadContext(UIApplication uIApplication)
        {
            UIApplication = uIApplication;
            UIDocument = UIApplication.ActiveUIDocument;
            Document = UIApplication.ActiveUIDocument.Document;
        }

        public void SetExternalEvents(ExternalEvent loadContextEvent, ExternalEvent showElementEvent
                                        , ExternalEvent openViewEvent, ExternalEvent saveDataToExStorageEvent
                                            , ExternalEvent changeDiameterEvent, ExternalEvent changeOffsetEvent)
        {
            LoadContextEvent = loadContextEvent;
            ShowElementEvent = showElementEvent;
            OpenViewEvent = openViewEvent;
            SaveDataToExStorageEvent = saveDataToExStorageEvent;
            ChangeDiameterEvent = changeDiameterEvent;
            ChangeOffsetEvent = changeOffsetEvent;
        }

        public void ShowElement()
        {
            UIDocument.Selection.SetElementIds(new List<ElementId>() { CurrentSelection });
            //UIDocument.ShowElements(CurrentSelection);
        }

        public void OpenView()
        {
            UIDocument.ActiveView = DesiredView;
            UIDocument.Selection.SetElementIds(new List<ElementId>() { CurrentSelection });
            UIDocument.ShowElements(CurrentSelection);
        }

        public void SaveOpeningsToExStorage()
        {
            UpdateDurchbruche();
            using (Transaction tx = new Transaction(Document, "GTB-Tool ExStorage Write"))
            {
                tx.Start();
                SaveNewOpenings();
                if (SaveAllToStorage)
                {
                    SaveMovedOpenings();
                    SaveResizedOpenings();
                    SaveMovedAndResizedOpenings();
                }
                tx.Commit();
            }
            UpdateDurchbruche();
        }

        private FamilySymbol FindPositionMarker()
        {
            FilteredElementCollector ficol = new FilteredElementCollector(Document);
            List<FamilySymbol> genModelInstances = ficol.OfClass(typeof(FamilySymbol))
                        .Select(x => x as FamilySymbol)
                            .Where(y => y.Category.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModel).ToList();
            FamilySymbol fs = genModelInstances.Where(e => e.Name == "LocationMarker").FirstOrDefault();
            return fs;
        }

        public void CreateOldPositionMarker()
        {
            FamilySymbol fs = FindPositionMarker();

            if (fs == null)
            {
                TaskDialog.Show("Info", "Can't find position marker family type!");
                return;
            }

            string oldPosition = CurrentItem.DurchbruchModel.OpeningMemory.OldPosition;
            string[] coords = oldPosition.Split(';');
            double x1 = Convert.ToDouble(coords[0], System.Globalization.CultureInfo.InvariantCulture);
            double y1 = Convert.ToDouble(coords[1], System.Globalization.CultureInfo.InvariantCulture);
            double z1 = Convert.ToDouble(coords[2], System.Globalization.CultureInfo.InvariantCulture);

            XYZ xyz1 = new XYZ(x1, y1, z1);

            string newPosition = CurrentItem.DurchbruchModel.OpeningMemory.NewPosition;
            string[] coordsNew = newPosition.Split(';');
            double x2 = Convert.ToDouble(coordsNew[0], System.Globalization.CultureInfo.InvariantCulture);
            double y2 = Convert.ToDouble(coordsNew[1], System.Globalization.CultureInfo.InvariantCulture);
            double z2 = Convert.ToDouble(coordsNew[2], System.Globalization.CultureInfo.InvariantCulture);

            XYZ xyz2 = new XYZ(x2, y2, z2);

            Transaction tx = new Transaction(Document, "Inserted position marker");
            tx.Start();
            FamilyInstance instance = Document.Create.NewFamilyInstance(xyz1, fs, StructuralType.NonStructural);

            //line creation postponed because of too many issues
            //try
            //{
            //    Line line = Line.CreateBound(xyz1, xyz2);
            //    Plane plane = Plane.CreateByThreePoints(xyz1, xyz2, new XYZ(0, 0, 0));
            //    SketchPlane sketchPlane = SketchPlane.Create(Document, plane);
            //    ModelCurve curve = Document.Create.NewModelCurve(line, sketchPlane);
            //    CurrentItem.OldPositionModelCurve = curve.Id.IntegerValue;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show("Can't create line marker" + Environment.NewLine + ex.ToString());
            //}
            tx.Commit();
            CurrentItem.OldPositionMarker = instance.Id.IntegerValue;
            OldPositionMarkers.Add(instance.Id.IntegerValue);
        }

        public void DeleteOldPositionMarker()
        {
            if (CurrentItem == null || CurrentItem.OldPositionMarker == 0) return;
            if (Document.GetElement(new ElementId(CurrentItem.OldPositionMarker)) == null) return;
            OldPositionMarkers.Remove(CurrentItem.OldPositionMarker);
            Transaction tx = new Transaction(Document, "Deleted position marker");
            tx.Start();
            Document.Delete(new ElementId(CurrentItem.OldPositionMarker));
            tx.Commit();
        }

        public void DeleteAllPositionMarkers()
        {
            Transaction tx = new Transaction(Document, "Deleted position markers");
            tx.Start();
            foreach (int idInteger in OldPositionMarkers)
            {
                ElementId id = new ElementId(idInteger);
                if (Document.GetElement(id) == null) continue;
                Document.Delete(id);
            }
            tx.Commit();
            OldPositionMarkers.Clear();
        }

        //method postponed
        public void DeleteOldPositionCurve()
        {
            if (CurrentItem == null || CurrentItem.OldPositionModelCurve == 0) return;
            if (Document.GetElement(new ElementId(CurrentItem.OldPositionModelCurve)) == null) return;
            Transaction tx = new Transaction(Document, "Deleted position curve");
            tx.Start();
            Document.Delete(new ElementId(CurrentItem.OldPositionModelCurve));
            tx.Commit();
        }

        private void SaveNewOpenings()
        {
            foreach (NewDurchbruchViewModel item in NewDurchbruche)
            {
                item.DurchbruchModel.OpeningMemory.SaveDateTostorage();
                item.DurchbruchModel.OpeningMemory.SaveDimensionsTostorage();
                item.DurchbruchModel.OpeningMemory.SavePositionTostorage();
            }
        }

        private void SaveResizedOpenings()
        {
            foreach (ResizedDurchbruchViewModel item in ResizedDurchbruche)
            {
                item.DurchbruchModel.OpeningMemory.SaveDateTostorage();
                item.DurchbruchModel.OpeningMemory.SaveDimensionsTostorage();
                item.DurchbruchModel.OpeningMemory.SavePositionTostorage();
            }
        }

        private void SaveMovedOpenings()
        {
            foreach (MovedAndResizedDbViewModel item in MovedDurchbruche)
            {
                item.DurchbruchModel.OpeningMemory.SaveDateTostorage();
                item.DurchbruchModel.OpeningMemory.SaveDimensionsTostorage();
                item.DurchbruchModel.OpeningMemory.SavePositionTostorage();
            }
        }
        private void SaveMovedAndResizedOpenings()
        {
            foreach (MovedAndResizedDbViewModel item in MovedAndResizedDurchbruche)
            {
                item.DurchbruchModel.OpeningMemory.SaveDateTostorage();
                item.DurchbruchModel.OpeningMemory.SaveDimensionsTostorage();
                item.DurchbruchModel.OpeningMemory.SavePositionTostorage();
            }
        }

        private void SetAllDurchbruche()
        {
            FilteredElementCollector ficol = new FilteredElementCollector(Document);
            List<FamilyInstance> genModelInstances = ficol.OfClass(typeof(FamilyInstance))
                                    .Select(x => x as FamilyInstance)
                                        .Where(y => y.Symbol.Family.FamilyCategory.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModel).ToList();
            _familyInstancesAll = new List<FamilyInstance>();
            foreach (FamilyInstance fi in genModelInstances)
            {
                Parameter gtbParameter = fi.get_Parameter(new Guid("4a581041-cc9c-4be4-8ab3-156d7b8e17a6"));
                if (gtbParameter != null && gtbParameter.AsString() != "GTB_Tools_location_marker") _familyInstancesAll.Add(fi);
            }
        }

        private void SetAllModelDurchbruche()
        {
            _modelDurchbrucheAll = new List<DurchbruchModel>();

            foreach (FamilyInstance fi in _familyInstancesAll)
            {
                DurchbruchModel durchbruchModel = DurchbruchModel.Initialize(fi, UIDocument);
                _modelDurchbrucheAll.Add(durchbruchModel);
            }
        }

        private void SetChangedDurchbruche()
        {
            NewDurchbruche = new List<NewDurchbruchViewModel>();
            ResizedDurchbruche = new List<ResizedDurchbruchViewModel>();
            MovedDurchbruche = new List<MovedAndResizedDbViewModel>();
            MovedAndResizedDurchbruche = new List<MovedAndResizedDbViewModel>();

            foreach (DurchbruchModel dbm in _modelDurchbrucheAll)
            {
                if(dbm.DurchbruchStatus == DurchbruchStatus.New)
                {
                    NewDurchbruchViewModel newDurchbruchViewModel = NewDurchbruchViewModel.Initialize(dbm);
                    NewDurchbruche.Add(newDurchbruchViewModel);
                }
                if (dbm.DurchbruchStatus == DurchbruchStatus.Resized)
                {
                    ResizedDurchbruchViewModel resizedDurchbruchViewModel = ResizedDurchbruchViewModel.Initialize(dbm);
                    ResizedDurchbruche.Add(resizedDurchbruchViewModel);
                }
                if (dbm.DurchbruchStatus == DurchbruchStatus.Moved)
                {
                    MovedAndResizedDbViewModel movedDurchbruchViewModel = MovedAndResizedDbViewModel.Initialize(dbm);
                    MovedDurchbruche.Add(movedDurchbruchViewModel);
                }
                if (dbm.DurchbruchStatus == DurchbruchStatus.MovedAndResized)
                {
                    MovedAndResizedDbViewModel movedAndResizedDbViewModel = MovedAndResizedDbViewModel.Initialize(dbm);
                    MovedAndResizedDurchbruche.Add(movedAndResizedDbViewModel);
                }
            }
        }
    }
}
