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
        public ExternalEvent LoadContextEvent { get; set; }
        public ExternalEvent ShowElementEvent { get; set; }
        public List<NewDurchbruchViewModel> NewDurchbruche { get; set; }
        public List<MovedDurchbruchViewModel> MovedDurchbruche { get; set; }
        public List<ResizedDurchbruchViewModel> ResizedDurchbruche { get; set; }

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

        public void SetExternalEvents(ExternalEvent loadContextEvent, ExternalEvent showElementEvent)
        {
            LoadContextEvent = loadContextEvent;
            ShowElementEvent = showElementEvent;
        }

        public void ShowElement()
        {
            UIDocument.ShowElements(CurrentSelection);
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
