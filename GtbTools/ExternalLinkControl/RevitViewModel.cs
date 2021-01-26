using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExternalLinkControl
{
    public class RevitViewModel : INotifyPropertyChanged
    {
        public ElementId ViewId { get; set; }
        public View View { get; set; }
        public RevitLinkType RevitLinkType { get; set; }
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

        private RevitViewModel()
        {

        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public static RevitViewModel Initialize(View view, RevitLinkType revitLinkType)
        {
            RevitViewModel result = new RevitViewModel();
            result.View = view;
            result.ViewId = view.Id;
            result.RevitLinkType = revitLinkType;
            result.CheckVisibility();
            result.SetViewType();
            result.IsTemplate = result.View.IsTemplate;
            return result;
        }

        public void TurnVisibilityOn(Document document)
        {
            using(Transaction tx = new Transaction(document, RevitLinkType.Name + " unhidden on " + View.Name))
            {
                tx.Start();
                View.UnhideElements(new List<ElementId>() { RevitLinkType.Id });
                tx.Commit();
            }
        }

        public void TurnVisibilityOff(Document document)
        {
            using (Transaction tx = new Transaction(document, RevitLinkType.Name + " hidden on " + View.Name))
            {
                tx.Start();
                View.HideElements(new List<ElementId>() { RevitLinkType.Id });
                tx.Commit();
            }
        }

        private void CheckVisibility()
        {
            IsVisible = !RevitLinkType.IsHidden(View);
        }

        private void SetViewType()
        {
            ViewType = Enum.GetName(typeof(ViewType), View.ViewType);
        }
    }
}
