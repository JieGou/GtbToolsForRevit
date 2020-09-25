using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace OpeningSymbol
{
    public class PlanView
    {
        public View View { get; set; }
        public ViewDiscipline ViewDiscipline {get; set;}
        public ViewDirection ViewDirection { get; set; }
        public List<RectangularOpening> RectangularOpenings { get; set; }
        public List<RoundOpening> RoundOpenings { get; set; }

        double _absoluteCutPlane;

        List<FamilyInstance> _rectFamilyInstances;
        List<FamilyInstance> _roundFamilyInstances;
        Document _doc;

        public PlanView( Document doc, View view, ViewDiscipline viewDiscipline)
        {
            _doc = doc;
            View = view;
            ViewDiscipline = viewDiscipline;
        }

        public void CreateOpeningList()
        {
            SearchFamilyInstances();
            SetViewDirection();
            SetCutPlane();
            RectangularOpenings = new List<RectangularOpening>();
            RoundOpenings = new List<RoundOpening>();
            if (ViewDiscipline == ViewDiscipline.TWP) CreateArcOpeningList();
            if (ViewDiscipline == ViewDiscipline.TGA) CreateTgaOpeningList();
        }

        private  void CreateTgaOpeningList()
        {
            foreach (FamilyInstance recFamIns in _rectFamilyInstances)
            {
                RectangularOpening rectangularOpening = RectangularOpening.Initialize(recFamIns, ViewDirection, ViewDiscipline.TGA, _absoluteCutPlane);
                RectangularOpenings.Add(rectangularOpening);
            }
            foreach (FamilyInstance roundFamIns in _roundFamilyInstances)
            {
                RoundOpening roundOpening = RoundOpening.Initialize(roundFamIns, ViewDirection, ViewDiscipline.TGA, _absoluteCutPlane);
                RoundOpenings.Add(roundOpening);
            }
        }

        private void CreateArcOpeningList()
        {
            foreach (FamilyInstance recFamIns in _rectFamilyInstances)
            {
                RectangularOpening rectangularOpening = RectangularOpening.Initialize(recFamIns, ViewDirection, ViewDiscipline.TWP, _absoluteCutPlane);
                RectangularOpenings.Add(rectangularOpening);
            }
            foreach (FamilyInstance roundFamIns in _roundFamilyInstances)
            {
                RoundOpening roundOpening = RoundOpening.Initialize(roundFamIns, ViewDirection, ViewDiscipline.TWP, _absoluteCutPlane);
                RoundOpenings.Add(roundOpening);
            }
        }

        private void SearchFamilyInstances()
        {
            FilteredElementCollector ficol = new FilteredElementCollector(_doc, View.Id);
            List<FamilyInstance> genModelInstances = ficol.OfClass(typeof(FamilyInstance))
                                    .Select(x => x as FamilyInstance)
                                        .Where(y => y.Symbol.Family.FamilyCategory.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModel).ToList();
            _roundFamilyInstances = new List<FamilyInstance>();
            _rectFamilyInstances = new List<FamilyInstance>();

            foreach (FamilyInstance fi in genModelInstances)
            {
                if (fi.Symbol.Family.Name == "XXX_Rectangular Face Opening_MT") _rectFamilyInstances.Add(fi);
                if (fi.Symbol.Family.Name == "XXX_Round Face Opening_MT") _roundFamilyInstances.Add(fi);
            }
            int roundIns = _roundFamilyInstances.Count;
            int rectIns = _rectFamilyInstances.Count;
        }

        private void SetViewDirection()
        {
            ElementId id = View.GetTypeId();
            ViewFamilyType viewFamilyType = _doc.GetElement(id) as ViewFamilyType;
            if(viewFamilyType.PlanViewDirection == PlanViewDirection.Down)
            {
                ViewDirection = ViewDirection.PlanDown;
            }
            if (viewFamilyType.PlanViewDirection == PlanViewDirection.Up)
            {
                ViewDirection = ViewDirection.PlanUp;
            }
        }
        private void SetCutPlane()
        {
            _absoluteCutPlane = -1;
            ViewPlan viewPlan = View as ViewPlan;
            PlanViewRange planViewRange = viewPlan.GetViewRange();

            ElementId cutPlaneId = planViewRange.GetLevelId(PlanViewPlane.CutPlane);
            double cutPlaneOffset = planViewRange.GetOffset(PlanViewPlane.CutPlane)*304.8;
            if (cutPlaneId.IntegerValue > 0)
            {
                Level level = _doc.GetElement(cutPlaneId) as Level;
                double elevation = level.Elevation * 304.8;
                _absoluteCutPlane = elevation + cutPlaneOffset;
            }
        }
    }
}
