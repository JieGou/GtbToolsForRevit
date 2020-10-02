using Autodesk.Revit.DB;
using GtbTools;
using Model;
using System.Collections.Generic;

namespace ViewModels
{
    public class ResizedDurchbruchViewModel
    {
        public string ElementId { get; set; }
        public string Shape { get; set; }
        public string Diameter { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Depth { get; set; }
        public List<ModelView> Views { get; set; }
        public string OpeningMark { get; set; }
        public DurchbruchModel DurchbruchModel { get; set; }

        private ResizedDurchbruchViewModel()
        {

        }

        public static ResizedDurchbruchViewModel Initialize(DurchbruchModel durchbruchModel)
        {
            ResizedDurchbruchViewModel result = new ResizedDurchbruchViewModel();
            result.DurchbruchModel = durchbruchModel;
            result.SetElementId();
            result.SetShape();
            result.SetDimensions();
            result.SetMark();
            result.SetViews();
            return result;
        }

        private void SetElementId()
        {
            ElementId = DurchbruchModel.ElementId.IntegerValue.ToString();
        }

        private void SetShape()
        {
            if (DurchbruchModel.Shape == DurchbruchShape.Rectangular) Shape = "Rectangular";
            if (DurchbruchModel.Shape == DurchbruchShape.Round) Shape = "Round";
        }

        private void SetDimensions()
        {
            Depth = DurchbruchModel.Depth.AsValueString();
            if (DurchbruchModel.Shape == DurchbruchShape.Round)
            {
                Width = "---";
                Height = "---";
                Diameter = DurchbruchModel.Diameter.AsValueString();
            }
            if (DurchbruchModel.Shape == DurchbruchShape.Rectangular)
            {
                Width = DurchbruchModel.Width.AsValueString();
                Height = DurchbruchModel.Height.AsValueString();
                Diameter = "---";
            }
        }

        private void SetMark()
        {
            OpeningMark = DurchbruchModel.OpeningMark.AsString();
        }

        private void SetViews()
        {
            Views = new List<ModelView>();
            foreach (View view in DurchbruchModel.Views)
            {
                ModelView modelView = new ModelView
                {
                    Name = view.Name,
                    View = view,
                    IsSelected = true
                };
                Views.Add(modelView);
            }
        }
    }
}
