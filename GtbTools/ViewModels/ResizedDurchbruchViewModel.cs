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
        public string Offset { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Depth { get; set; }
        public List<ModelView> Views { get; set; }
        public string OpeningMark { get; set; }
        public DurchbruchModel DurchbruchModel { get; set; }

        public string OldDiameter { get; set; }
        public string OldWidth { get; set; }
        public string OldHeight { get; set; }
        public string OldDepth { get; set; }
        public string DateSaved { get; set; }

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
            result.GetOldDimensions();
            result.SetDateSaved();
            return result;
        }

        private void SetDateSaved()
        {
            DateSaved = DurchbruchModel.OpeningMemory.OldDateSaved;
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

        private void GetOldDimensions()
        {
            if(DurchbruchModel.Shape == DurchbruchShape.Rectangular)
            {
                string[] dims = DurchbruchModel.OpeningMemory.OldDimensions.Split('x');
                OldWidth = dims[0];
                OldHeight = dims[1];
                OldDepth = dims[2];
                OldDiameter = "---";
            }
            if (DurchbruchModel.Shape == DurchbruchShape.Round)
            {
                string[] dims = DurchbruchModel.OpeningMemory.OldDimensions.Split('x');
                OldDiameter = dims[0];
                OldDepth = dims[1];
                OldWidth = "---";
                OldHeight = "---";
            }
        }
}
}
