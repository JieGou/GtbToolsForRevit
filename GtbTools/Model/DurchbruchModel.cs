using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ExStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Model
{
    public class DurchbruchModel
    {
        public ElementId ElementId { get; set; }
        public DurchbruchShape Shape { get; set; }
        public Parameter Diameter { get; set; }
        public Parameter Width { get; set; }
        public Parameter Height { get; set; }
        public Parameter Depth { get; set; }
        public List<View> Views { get; set; }
        public Parameter OpeningMark { get; set; }
        public DurchbruchStatus DurchbruchStatus { get; set; }
        public FamilyInstance FamilyInstance { get; set; }
        public OpeningMemory OpeningMemory { get; set; }

        UIDocument _uiDoc;

        private DurchbruchModel()
        {

        }

        public static DurchbruchModel Initialize(FamilyInstance familyInstance, UIDocument uIDocument)
        {
            DurchbruchModel result = new DurchbruchModel();
            result.FamilyInstance = familyInstance;
            result._uiDoc = uIDocument;
            result.SetId();
            result.SetDimensions();
            result.SetShape();
            result.SetOpeningMark();
            result.SetViews();
            result.SetMemory();
            result.SetStatus();
            return result;
        }

        private void SetId()
        {
            ElementId = FamilyInstance.Id;
        }

        private void SetDimensions()
        {
            Depth = FamilyInstance.get_Parameter(new Guid("17a96ef5-1311-49f2-a0d1-4fe5f3f3854b"));
            Width = FamilyInstance.get_Parameter(new Guid("46982e85-76c3-43fb-828f-ddf7a643566f"));
            Height = FamilyInstance.get_Parameter(new Guid("8eb274b3-fc0c-43e0-a46b-236bf59f292d"));
            Diameter = FamilyInstance.get_Parameter(new Guid("9c805bcc-ebc9-4d4c-8d73-26970789417a"));
        }

        private void SetShape()
        {
            if(Width != null && Height != null)
            {
                Shape = DurchbruchShape.Rectangular;
            }
            if(Diameter != null)
            {
                Shape = DurchbruchShape.Round;
            }
        }

        private void SetOpeningMark()
        {
            OpeningMark = FamilyInstance.get_Parameter(new Guid("ed25fc8e-129f-4a2b-8d69-4de0c6615ec5"));
        }

        private void SetViews()
        {
            Views = new List<View>();
            List<View> allViews = new List<View>();
            FilteredElementCollector ficol = new FilteredElementCollector(_uiDoc.Document);
            ficol.OfClass(typeof(View));
            foreach (View view in ficol)
            {
                if (view.IsTemplate) continue;
                if (view.ViewType == ViewType.FloorPlan || view.ViewType == ViewType.CeilingPlan || view.ViewType == ViewType.EngineeringPlan || view.ViewType == ViewType.Section)
                {
                    allViews.Add(view);
                }
            }
            foreach (View view in allViews)
            {
                FilteredElementCollector ficol2 = new FilteredElementCollector(_uiDoc.Document, view.Id);
                List<FamilyInstance> instances = ficol2.OfClass(typeof(FamilyInstance)).Select(x => x as FamilyInstance).Where(y => y.Id.IntegerValue == FamilyInstance.Id.IntegerValue).ToList();
                if (instances.Count > 0) Views.Add(view);
            }
        }

        private void SetMemory()
        {
            OpeningMemory = OpeningMemory.Initialize(FamilyInstance);
        }

        private void SetStatus()
        {
            if(OpeningMemory.IsDimChanged && OpeningMemory.IsPosChanged)
            {
                DurchbruchStatus = DurchbruchStatus.MovedAndResized;
                return;
            }
            if(OpeningMemory.IsNew)
            {
                DurchbruchStatus = DurchbruchStatus.New;
                return;
            }
            if (OpeningMemory.IsPosChanged)
            {
                DurchbruchStatus = DurchbruchStatus.Moved;
                return;
            }
            if (OpeningMemory.IsDimChanged)
            {
                DurchbruchStatus = DurchbruchStatus.Resized;
                return;
            }

            DurchbruchStatus = DurchbruchStatus.Unchanged;
        }
    }
}
