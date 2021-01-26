using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalLinkControl
{
    public class ExternalLinkToolViewModel
    {
        public List<RevitLinkViewModel> RevitLinkViewModels { get; set; }
        public RevitLinkViewModel EditedLinkViewModel { get; set; }

        Document _document;
        
        public ExternalLinkToolViewModel(Document document)
        {
            _document = document;
        }

        public void CreateViewModels()
        {
            RevitLinkViewModels = new List<RevitLinkViewModel>();
            List<View> views = GetViews();
            foreach (RevitLinkType rvtLinkType in GetRevitLinkTypes())
            {
                RevitLinkViewModel model = new RevitLinkViewModel(rvtLinkType, views);
                model.CreateViewModels();
                RevitLinkViewModels.Add(model);
            }           
        }

        public void ApplyChangesToViewModel()
        {
            foreach (RevitViewModel model in EditedLinkViewModel.RevitViewModels)
            {
                if (model.IsVisible) model.TurnVisibilityOn(_document);
                if (!model.IsVisible) model.TurnVisibilityOff(_document);
            }
        }

        private List<RevitLinkType> GetRevitLinkTypes()
        {
            FilteredElementCollector ficol = new FilteredElementCollector(_document);
            return ficol.OfClass(typeof(RevitLinkType)).Select(e => e as RevitLinkType).ToList();
        }

        private List<View> GetViews()
        {
            FilteredElementCollector ficol = new FilteredElementCollector(_document);
            return ficol.OfClass(typeof(View)).Select(e => e as View).Where(e => IsPrimaryView(e) && e.ViewType == ViewType.FloorPlan || e.ViewType == ViewType.CeilingPlan).ToList();
        }

        private bool IsPrimaryView(View view)
        {
            bool result = false;
            ElementId test = view.GetPrimaryViewId();
            if (test == null || test == ElementId.InvalidElementId) result = true;
            return result;
        }
    }
}
