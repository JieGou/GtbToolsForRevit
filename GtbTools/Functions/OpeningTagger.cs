using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GUI;
using OpeningSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        List<ElementId> taggedWallIds;
        List<ElementId> taggedFloorIds;
        List<ElementId> taggedCeilingIds;

        int newTagsCount = 0;

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
            GetAllTaggedIds();
            if (wallTagId == null || floorTagId == null || ceilingTagId == null)
            {
                TaskDialog.Show("Info", "Bitte wählen Sie alle Typen aus");
                return;
            }

            using (Transaction tx = new Transaction(Document, "Multi tagging"))
            {
                tx.Start();
                foreach (FamilyInstance fi in wallInstances)
                {
                    ElementId openingId = fi.Id;
                    if (taggedWallIds.Contains(openingId)) continue;
                    Element element = fi as Element;
                    LocationPoint lp = fi.Location as LocationPoint;
                    XYZ xyz = lp.Point;
                    Reference reference = new Reference(element);
#if DEBUG2018 || RELEASE2018
                    IndependentTag newTag = IndependentTag.Create(Document, Document.ActiveView.Id, reference, true, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, xyz);
                    newTag.ChangeTypeId(wallTagId);
#else
                    IndependentTag newTag = IndependentTag.Create(Document, wallTagId, Document.ActiveView.Id, reference, true, TagOrientation.Horizontal, xyz);
#endif
                    newTagsCount++;
                }
                foreach (FamilyInstance fi in deckenInstances)
                {
                    ElementId openingId = fi.Id;
                    if (taggedCeilingIds.Contains(openingId)) continue;
                    Element element = fi as Element;
                    LocationPoint lp = fi.Location as LocationPoint;
                    XYZ xyz = lp.Point;
                    Reference reference = new Reference(element);
#if DEBUG2018 || RELEASE2018
                    IndependentTag newTag = IndependentTag.Create(Document, Document.ActiveView.Id, reference, true, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, xyz);
                    newTag.ChangeTypeId(ceilingTagId);
#else
                    IndependentTag newTag = IndependentTag.Create(Document, ceilingTagId, Document.ActiveView.Id, reference, true, TagOrientation.Horizontal, xyz);
#endif
                    XYZ thPos = newTag.TagHeadPosition;
                    XYZ newPosition = new XYZ(thPos.X, thPos.Y + 0.1, thPos.Z);
                    newTag.TagHeadPosition = newPosition;
                    newTagsCount++;
                }
                foreach (FamilyInstance fi in bodenInstances)
                {
                    ElementId openingId = fi.Id;
                    if (taggedFloorIds.Contains(openingId)) continue;
                    Element element = fi as Element;
                    LocationPoint lp = fi.Location as LocationPoint;
                    XYZ xyz = lp.Point;
                    Reference reference = new Reference(element);
#if DEBUG2018 || RELEASE2018
                    IndependentTag newTag = IndependentTag.Create(Document, Document.ActiveView.Id, reference, true, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, xyz);
                    newTag.ChangeTypeId(floorTagId);
#else
                    IndependentTag newTag = IndependentTag.Create(Document, floorTagId, Document.ActiveView.Id, reference, true, TagOrientation.Horizontal, xyz);
#endif
                    newTagsCount++;
                }
                tx.Commit();
            }
        }

        public GtbWindowResult DisplayWindow()
        {
            QuickTagWindow quickTagWindow = new QuickTagWindow(GenericModelTags);
            quickTagWindow.SetLists();
            quickTagWindow.ShowDialog();
            wallTagId = quickTagWindow.WandSymbol;
            floorTagId = quickTagWindow.BodenSymbol;
            ceilingTagId = quickTagWindow.DeckenSymbol;
            return quickTagWindow.WindowResult;
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

        private void GetAllTaggedIds()
        {
            List<IndependentTag> wallTags = new List<IndependentTag>();
            List<IndependentTag> floorTags = new List<IndependentTag>();
            List<IndependentTag> ceilingTags = new List<IndependentTag>();
            FilteredElementCollector ficol1 = new FilteredElementCollector(Document, Document.ActiveView.Id);
            FilteredElementCollector ficol2 = new FilteredElementCollector(Document, Document.ActiveView.Id);
            FilteredElementCollector ficol3 = new FilteredElementCollector(Document, Document.ActiveView.Id);
            wallTags = ficol1.OfClass(typeof(IndependentTag)).Select(x => x as IndependentTag).ToList();
            floorTags = ficol2.OfClass(typeof(IndependentTag)).Select(x => x as IndependentTag).ToList();
            ceilingTags = ficol3.OfClass(typeof(IndependentTag)).Select(x => x as IndependentTag).ToList();
            taggedWallIds = wallTags.Select(x => x.TaggedLocalElementId).ToList();
            taggedFloorIds = floorTags.Select(x => x.TaggedLocalElementId).ToList();
            taggedCeilingIds = ceilingTags.Select(x => x.TaggedLocalElementId).ToList();
        }
        public static bool IsValidViewType(Document doc)
        {
            bool result = false;
            View v = doc.ActiveView;
            if (v.ViewType == ViewType.FloorPlan || v.ViewType == ViewType.CeilingPlan || v.ViewType == ViewType.EngineeringPlan)
            {
                result = true;
            }
            return result;
        }
        
        public void ShowNewTagsInfo()
        {
            string info1 = String.Format("Es wurde {0} neues Beschriftung hinzugefügt.", newTagsCount);
            string info2 = String.Format("Es wurden {0} neue Beschriftungen hinzugefügt.", newTagsCount);
            if(newTagsCount == 1)
            {
                TaskDialog.Show("Info", info1);
            }
            else
            {
                TaskDialog.Show("Info", info2);
            }
        }
    }
}
