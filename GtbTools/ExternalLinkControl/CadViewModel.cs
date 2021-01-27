using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalLinkControl
{
    public class CadViewModel : INotifyPropertyChanged
    {
        public ElementId ViewId { get; set; }
        public View View { get; set; }
        public CADLinkType CadLinkType { get; set; }
        public bool CategoryExistsInView = true;
        private bool _isVisible;
        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }
        public string ViewType { get; set; }
        public bool IsTemplate { get; set; }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private CadViewModel()
        {

        }

        public static CadViewModel Initialize(View view, CADLinkType cadLinkType)
        {
            CadViewModel result = new CadViewModel();
            result.View = view;
            result.ViewId = view.Id;
            result.CadLinkType = cadLinkType;
            result.CheckVisibility();
            result.SetViewType();
            result.IsTemplate = result.View.IsTemplate;
            return result;
        }

        public void TurnVisibilityOn(Document document)
        {
            if (!View.IsTemplate && IsCadControlledByTemplate(document)) return;
            using (Transaction tx = new Transaction(document, CadLinkType.Name + " unhidden on " + View.Name))
            {
                tx.Start();
                View.SetCategoryHidden(CadLinkType.Category.Id, false);
                tx.Commit();
            }
        }

        public void TurnVisibilityOff(Document document)
        {
            if (!View.IsTemplate && IsCadControlledByTemplate(document)) return;
            using (Transaction tx = new Transaction(document, CadLinkType.Name + " hidden on " + View.Name))
            {
                tx.Start();
                View.SetCategoryHidden(CadLinkType.Category.Id, true);
                tx.Commit();
            }
        }

        private bool IsCadControlledByTemplate(Document doc)
        {
            bool result = false;
            ElementId viewTemplateId = View.ViewTemplateId;
            if (viewTemplateId != null && viewTemplateId != ElementId.InvalidElementId)
            {
                View viewTemplate = doc.GetElement(viewTemplateId) as View;
                List<int> nonControlledParameters = viewTemplate.GetNonControlledTemplateParameterIds().Select(e => e.IntegerValue).ToList();
                if (!nonControlledParameters.Contains((int)BuiltInParameter.VIS_GRAPHICS_IMPORT))
                {
                    result = true;
                }
            }
            return result;
        }

        private void CheckVisibility()
        {
            try
            {
                IsVisible = CadLinkType.Category.get_Visible(View);
            }
            catch
            {
                CategoryExistsInView = false;
                //TaskDialog.Show("Error!", View.Name + "- Can't identify visibility of: " + CadLinkType.Name);               
            }
        }

        private void SetViewType()
        {
            ViewType = Enum.GetName(typeof(ViewType), View.ViewType);
        }
    }
}
