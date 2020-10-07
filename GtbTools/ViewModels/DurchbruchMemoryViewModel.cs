using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ViewModels
{
    public class DurchbruchMemoryViewModel
    {
        public UIApplication UIApplication { get; set; }
        public UIDocument UIDocument {get; set;}
        public Document Document { get; set; }
        public ElementId CurrentSelection { get; set; }
        public View DesiredView { get; set; }
        public ExternalEvent LoadContextEvent { get; set; }
        public ExternalEvent ShowElementEvent { get; set; }
        public ExternalEvent OpenViewEvent { get; set; }
        public ExternalEvent SaveDataToExStorageEvent { get; set; }
        public List<NewDurchbruchViewModel> NewDurchbruche { get; set; }
        public List<MovedDurchbruchViewModel> MovedDurchbruche { get; set; }
        public List<ResizedDurchbruchViewModel> ResizedDurchbruche { get; set; }
        public bool SaveAllToStorage { get; set; } = false;

        public ManualResetEvent SignalEvent = new ManualResetEvent(false);

        List<FamilyInstance> _familyInstancesAll;
        List<DurchbruchModel> _modelDurchbrucheAll;

        public DurchbruchMemoryViewModel()
        {
            
        }

        public void InitializeDurchbruche()
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

        public void SetExternalEvents(ExternalEvent loadContextEvent, ExternalEvent showElementEvent, ExternalEvent openViewEvent, ExternalEvent saveDataToExStorageEvent)
        {
            LoadContextEvent = loadContextEvent;
            ShowElementEvent = showElementEvent;
            OpenViewEvent = openViewEvent;
            SaveDataToExStorageEvent = saveDataToExStorageEvent;
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
            using (Transaction tx = new Transaction(Document, "GTB-Tool ExStorage Write"))
            {
                tx.Start();
                SaveNewOpenings();
                if (SaveAllToStorage)
                {
                    SaveMovedOpenings();
                    SaveResizedOpenings();
                }
                tx.Commit();
            }
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
            foreach (MovedDurchbruchViewModel item in MovedDurchbruche)
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
                if (gtbParameter == null) continue;
                _familyInstancesAll.Add(fi);
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
            MovedDurchbruche = new List<MovedDurchbruchViewModel>();

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
                    MovedDurchbruchViewModel movedDurchbruchViewModel = MovedDurchbruchViewModel.Initialize(dbm);
                    MovedDurchbruche.Add(movedDurchbruchViewModel);
                }
            }
        }
    }
}
