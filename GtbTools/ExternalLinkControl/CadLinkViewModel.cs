using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalLinkControl
{
    public class CadLinkViewModel
    {
        public List<CadViewModel> CadViewModels { get; set; }
        public CADLinkType CadLinkType { get; set; }

        List<View> _views;
        
        public CadLinkViewModel(CADLinkType cadLinkType, List<View> views)
        {
            CadLinkType = cadLinkType;
            _views = views;
        }

        public void CreateViewModels()
        {
            CadViewModels = new List<CadViewModel>();
            foreach (View v in _views)
            {
                CadViewModel model = CadViewModel.Initialize(v, CadLinkType);
                if (model.CategoryExistsInView)
                {
                    CadViewModels.Add(model);
                }
            }
        }
    }
}
