using Autodesk.Revit.DB;
using GtbTools;
using Model;
using System.Collections.Generic;

namespace ViewModels
{
    public class NewDurchbruchViewModel
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

        private NewDurchbruchViewModel()
        {

        }

        public static NewDurchbruchViewModel Initialize(DurchbruchModel durchbruchModel)
        {
            NewDurchbruchViewModel result = new NewDurchbruchViewModel();
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
            ElementId =  DurchbruchModel.ElementId.IntegerValue.ToString();
        }

        private void SetShape()
        {
            if (DurchbruchModel.Shape == DurchbruchShape.Rectangular) Shape = "Rectangular";
            if (DurchbruchModel.Shape == DurchbruchShape.Round) Shape = "Round";
        }

        private void SetDimensions()
        {
            double depthMetric = DurchbruchModel.Depth.AsDouble() * 304.8;
            Depth = depthMetric.ToString("F1");
            if (DurchbruchModel.Shape == DurchbruchShape.Round)
            {
                Width = "---";
                Height = "---";
                double diameterMetric = DurchbruchModel.Diameter.AsDouble() * 304.8;
                Diameter = diameterMetric.ToString("F1");
            }
            if (DurchbruchModel.Shape == DurchbruchShape.Rectangular)
            {
                double widthMetric = DurchbruchModel.Width.AsDouble() * 304.8;
                double heightMetric = DurchbruchModel.Height.AsDouble() * 304.8;
                Width = widthMetric.ToString("F1");
                Height = heightMetric.ToString("F1");
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
