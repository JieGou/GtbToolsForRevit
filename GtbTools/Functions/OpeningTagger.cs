using Autodesk.Revit.DB;
using GUI;
using OpeningSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public class OpeningTagger
    {
        public Document Document { get; set; }
        public List<FamilySymbol> GenericModelTags { get; set; }

        List<FamilyInstance> wallInstances;
        List<FamilyInstance> bodenInstances;
        List<FamilyInstance> deckenInstances;

        ElementId wallTagId;
        ElementId floorTagId;
        ElementId ceilingTagId;

        private OpeningTagger()
        {

        }

        public static OpeningTagger Initialize(Document document)
        {
            OpeningTagger result = new OpeningTagger();
            result.Document = document;
            result.SetGenericModelTags();
            result.SetOpenings();
            return result;
        }

        public  void TagThemAll()
        {
            if (wallTagId == null) return;
            using (Transaction tx = new Transaction(Document, "Multi tagging"))
            {
                tx.Start();
                foreach (FamilyInstance fi in wallInstances)
                {
                    ElementId openingId = fi.Id;
                    Element element = fi as Element;
                    LocationPoint lp = fi.Location as LocationPoint;
                    XYZ xyz = lp.Point;
                    Reference reference = new Reference(element);
                    IndependentTag newTag = IndependentTag.Create(Document, wallTagId, Document.ActiveView.Id, reference, true, TagOrientation.Horizontal, xyz);
                }
                foreach (FamilyInstance fi in deckenInstances)
                {
                    ElementId openingId = fi.Id;
                    Element element = fi as Element;
                    LocationPoint lp = fi.Location as LocationPoint;
                    XYZ xyz = lp.Point;
                    Reference reference = new Reference(element);
                    IndependentTag newTag = IndependentTag.Create(Document, ceilingTagId, Document.ActiveView.Id, reference, true, TagOrientation.Horizontal, xyz);
                }
                foreach (FamilyInstance fi in bodenInstances)
                {
                    ElementId openingId = fi.Id;
                    Element element = fi as Element;
                    LocationPoint lp = fi.Location as LocationPoint;
                    XYZ xyz = lp.Point;
                    Reference reference = new Reference(element);
                    IndependentTag newTag = IndependentTag.Create(Document, floorTagId, Document.ActiveView.Id, reference, true, TagOrientation.Horizontal, xyz);
                }
                tx.Commit();
            }
        }

        public void DisplayWindow()
        {
            QuickTagWindow quickTagWindow = new QuickTagWindow(GenericModelTags);
            quickTagWindow.SetLists();
            quickTagWindow.ShowDialog();
            wallTagId = quickTagWindow.WandSymbol;
            floorTagId = quickTagWindow.BodenSymbol;
            ceilingTagId = quickTagWindow.DeckenSymbol;
        }

        private void SetGenericModelTags()
        {
            GenericModelTags = new List<FamilySymbol>();
            FilteredElementCollector ficol = new FilteredElementCollector(Document);
            GenericModelTags = ficol.OfClass(typeof(FamilySymbol))
                                    .Select(x => x as FamilySymbol)
                                        .Where(y => y.Family.FamilyCategory.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModelTags).ToList();
        }

        private void SetOpenings()
        {
            PlanView planView = new PlanView(Document, Document.ActiveView, OpeningSymbol.ViewDiscipline.TGA);
            planView.CreateOpeningList();
            List<RoundOpening> roundOpenings = planView.RoundOpenings;
            List<RectangularOpening> rectangularOpenings = planView.RectangularOpenings;
            wallInstances = new List<FamilyInstance>();
            bodenInstances = new List<FamilyInstance>();
            deckenInstances = new List<FamilyInstance>();
            foreach (RoundOpening ro in roundOpenings)
            {
                if (ro.OpeningHost == OpeningHost.Wall) wallInstances.Add(ro.FamilyInstance);
                if (ro.OpeningHost == OpeningHost.FloorOrCeiling)
                {
                    if(ro.PlanViewLocation == PlanViewLocation.AboveCutPlane)
                    {
                        deckenInstances.Add(ro.FamilyInstance);
                    }
                    else
                    {
                        bodenInstances.Add(ro.FamilyInstance);
                    }

                }
            }
            foreach (RectangularOpening ro in rectangularOpenings)
            {
                if (ro.OpeningHost == OpeningHost.Wall) wallInstances.Add(ro.FamilyInstance);
                if (ro.OpeningHost == OpeningHost.FloorOrCeiling)
                {
                    if (ro.PlanViewLocation == PlanViewLocation.AboveCutPlane)
                    {
                        deckenInstances.Add(ro.FamilyInstance);
                    }
                    else
                    {
                        bodenInstances.Add(ro.FamilyInstance);
                    }

                }
            }
        }
    }
}
