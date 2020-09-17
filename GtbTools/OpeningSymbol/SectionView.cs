using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpeningSymbol
{
    public class SectionView
    {
        public View View { get; set; }
        public ViewDiscipline ViewDiscipline { get; set; }
        public List<RectangularOpening> RectangularOpenings { get; set; }
        public List<RoundOpening> RoundOpenings { get; set; }

        List<FamilyInstance> _rectFamilyInstances;
        List<FamilyInstance> _roundFamilyInstances;
        List<FamilyInstance> _cutElements;
        Document _doc;
        ViewDirection _viewDirection;

        public SectionView(Document doc, View view, ViewDiscipline viewDiscipline)
        {
            _doc = doc;
            View = view;
            ViewDiscipline = viewDiscipline;
        }

        //run separately
        public void CreateOpeningLists()
        {
            SearchFamilyInstances();
            SetViewDirection();
            RectangularOpenings = new List<RectangularOpening>();
            RoundOpenings = new List<RoundOpening>();
            if (ViewDiscipline == ViewDiscipline.ARC) CreateArcOpeningList();
            if (ViewDiscipline == ViewDiscipline.TGA) CreateTgaOpeningList();
        }

        private void CreateTgaOpeningList()
        {
            foreach (FamilyInstance recFamIns in _rectFamilyInstances)
            {
                bool isCutOnSection = _cutElements.Contains(recFamIns);
                RectangularOpening rectangularOpening = RectangularOpening.Initialize(recFamIns, _viewDirection, ViewDiscipline.TGA, isCutOnSection);
                RectangularOpenings.Add(rectangularOpening);
            }
            foreach (FamilyInstance roundFamIns in _roundFamilyInstances)
            {
                bool isCutOnSection = _cutElements.Contains(roundFamIns);
                RoundOpening roundOpening = RoundOpening.Initialize(roundFamIns, _viewDirection, ViewDiscipline.TGA, isCutOnSection);
                RoundOpenings.Add(roundOpening);
            }
        }

        private bool Equals(FamilyInstance searchedElement, List<FamilyInstance> searchList)
        {
            bool result = false;
            foreach (FamilyInstance fi in searchList)
            {
                if (fi.Id.IntegerValue == searchedElement.Id.IntegerValue) result = true;
            }
            return result;
        }

        private void CreateArcOpeningList()
        {
            foreach (FamilyInstance recFamIns in _rectFamilyInstances)
            {
                bool isCutOnSection = Equals(recFamIns, _cutElements);
                
                if (isCutOnSection) MessageBox.Show("Element is cut");
                RectangularOpening rectangularOpening = RectangularOpening.Initialize(recFamIns, _viewDirection, ViewDiscipline.ARC, isCutOnSection);
                RectangularOpenings.Add(rectangularOpening);
            }
            foreach (FamilyInstance roundFamIns in _roundFamilyInstances)
            {
                bool isCutOnSection = Equals(roundFamIns, _cutElements);

                if (isCutOnSection) MessageBox.Show("Element is cut");
                RoundOpening roundOpening = RoundOpening.Initialize(roundFamIns, _viewDirection, ViewDiscipline.ARC, isCutOnSection);
                RoundOpenings.Add(roundOpening);
            }
        }

        private void SearchFamilyInstances()
        {
            FilteredElementCollector ficol = new FilteredElementCollector(_doc, View.Id);
            List<FamilyInstance> genModelInstances = ficol.OfClass(typeof(FamilyInstance))
                                    .Select(x => x as FamilyInstance)
                                        .Where(y => y.Symbol.Family.FamilyCategory.Name == "Generic Models").ToList();
            _roundFamilyInstances = new List<FamilyInstance>();
            _rectFamilyInstances = new List<FamilyInstance>();

            foreach (FamilyInstance fi in genModelInstances)
            {
                if (fi.Symbol.Family.Name == "XXX_Rectangular Face Opening_MT") _rectFamilyInstances.Add(fi);
                if (fi.Symbol.Family.Name == "XXX_Round Face Opening_MT") _roundFamilyInstances.Add(fi);
            }
            int roundIns = _roundFamilyInstances.Count;
            int rectIns = _rectFamilyInstances.Count;
            //string test1 = String.Format("There are {0} round openings", roundIns);
            //string test2 = String.Format("There are {0} rectangular openings", rectIns);
            //MessageBox.Show(test1 + Environment.NewLine + test2);
        }
        private void SetViewDirection()
        {
            double rdX = Math.Abs(View.RightDirection.X);
            double rdY = Math.Abs(View.RightDirection.Y);
            double vdX = Math.Abs(View.ViewDirection.Y);
            double vdY = Math.Abs(View.ViewDirection.X);

            if (vdY == 1 && rdX == 1) _viewDirection = ViewDirection.SectionV;
            if (vdX == 1 && rdY == 1) _viewDirection = ViewDirection.SectionH;
        }

        //Run separately with doc
        public void FindCutElements(Document doc)
        {
            _cutElements = new List<FamilyInstance>();

            List<CurveLoop> _crop = View.GetCropRegionShapeManager().GetCropShape().ToList<CurveLoop>();
            CurveLoop cvLoop = _crop.First();
            List<XYZ> _cropPoints = new List<XYZ>();
            foreach (Curve cv in cvLoop)
            {
                if (cv is Line)
                {
                    _cropPoints.Add(cv.GetEndPoint(0));
                }
                else
                {
                    // redundant all curves in crop region are straight lines
                    List<XYZ> temp = cv.Tessellate().ToList<XYZ>();
                    temp.RemoveAt(temp.Count - 1);
                    _cropPoints.AddRange(temp);
                }
            }

            if (_cropPoints.Count > 3)
            {
                Outline _outline = new Outline(_cropPoints[0], _cropPoints[1]);

                for (int i = 2; i < _cropPoints.Count; i++)
                {
                    _outline.AddPoint(_cropPoints[i]);
                }

                BoundingBoxIntersectsFilter CutPlaneFilter = new BoundingBoxIntersectsFilter(_outline);
                // simplefy the result using OfClass or OfCategory
                _cutElements = new FilteredElementCollector(doc, View.Id)
                                                        .OfClass(typeof(FamilyInstance))
                                                            .WherePasses(CutPlaneFilter)
                                                                .Select(x => x as FamilyInstance)
                                                                    .Where(y => y.Symbol.Family.FamilyCategory.Name == "Generic Models").ToList();
                MessageBox.Show(_cutElements.Count.ToString());
            }
        }
    }
}
