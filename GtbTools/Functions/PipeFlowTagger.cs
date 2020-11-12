using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using GUI;
using PipeFlowTool;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Functions
{
    public class PipeFlowTagger : INotifyPropertyChanged
    {
        #region Properties and fields
        public UIDocument UIDocument { get; set; }
        public Document Document { get; set; }
        public List<Pipe> ReferencePipes { get; set; }
        public List<VerticalPipingLine> PipingLines { get; set; }
        public ElementId SelectedItem { get; set; }
        public PipeFlowToolAction Action { get; set; }
        public ExternalEvent StartEvent { get; set; }
        public List<XYZ> UsedCoordinates { get; set; }
        private List<LineViewModel> _lineViewModels;
        public List<LineViewModel> LineViewModels
        {
            get => _lineViewModels;
            set
            {
                if (_lineViewModels != value)
                {
                    _lineViewModels = value;
                    OnPropertyChanged(nameof(LineViewModels));
                }
            }
        }

        public List<FamilySymbol> PipeFittingTags { get; set; }
        public List<FamilySymbol> SelectedTags { get; set; }
        public DefaultDirections DefaultDirections { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Dictionary<string, List<XYZ>> _systemUsedCoordinates;
        List<IndependentTag> _allViewTags;
        List<IndependentTag> _pipeFlowTags;
        List<ElementId> _taggedElementIds;
        double _topLevel;
        double _bottomLevel;
        View _activeView;
        ViewType _viewType;
        bool _isTopUndefined = false;
        bool _isBottomUndefined = false;


        public ManualResetEvent SignalEvent = new ManualResetEvent(false);
        #endregion

        public PipeFlowTagger()
        {
            DefaultDirections = new DefaultDirections();
        }

        public void Initialize(UIDocument uIDocument)
        {
            UIDocument = uIDocument;
            Document = uIDocument.Document;
            SetActiveView();
            SetViewLevels();
            SetAnnotationSymbols();
            SignalEvent.Set();
        }

        public void SetEvent(ExternalEvent startEvent)
        {
            StartEvent = startEvent;
        }

        public void AnalyzeView()
        {
            UsedCoordinates = new List<XYZ>();
            _systemUsedCoordinates = new Dictionary<string, List<XYZ>>();
            CheckFlags();
            FindReferencePipes();
            CreatePipingLines();
            SetAllViewTags();
        }

        public void DisplayWindow()
        {
            Thread windowThread = new Thread(delegate ()
            {
                SignalEvent.WaitOne();
                SignalEvent.Reset();
                PipeFlowTagWindow window = new PipeFlowTagWindow(this);
                window.Show();
                Dispatcher.Run();
            });
            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.Start();
        }

        public void TagAllLines()
        {
            int count = 0;
            using (Transaction tx = new Transaction(Document, "Adding PIF Tags"))
            {
                tx.Start();
                foreach (VerticalPipingLine line in PipingLines)
                {
                    if(line.TagTheLine(SelectedTags, Document, DefaultDirections)) count++;
                }
                tx.Commit();
            }
            TaskDialog.Show("Info", String.Format("Added {0} new tags!", count));
        }

        public void SelectElement()
        {
            UIDocument.Selection.SetElementIds(new List<ElementId>() { SelectedItem });
        }

        public void SetTaggedElementIds()
        {
            SetPipeFlowTags();
            _taggedElementIds = new List<ElementId>();
            foreach (IndependentTag item in _pipeFlowTags)
            {
                ElementId id = item.TaggedLocalElementId;
                if(id.IntegerValue > 0) _taggedElementIds.Add(id);
            }
        }

        private void SetAnnotationSymbols()
        {
            PipeFittingTags = new List<FamilySymbol>();
            FilteredElementCollector ficol = new FilteredElementCollector(Document);
            PipeFittingTags = ficol.OfClass(typeof(FamilySymbol)).Select(x => x as FamilySymbol)
                                     .Where(y => y.Category.Id.IntegerValue == (int)BuiltInCategory.OST_PipeFittingTags).ToList();
        }

        public void SetViewModelList()
        {
            LineViewModels = new List<LineViewModel>();
            foreach (VerticalPipingLine line in PipingLines)
            {
                LineViewModel model = LineViewModel.Initialize(line);
                LineViewModels.Add(model);
            }
        }

        private void CreatePipingLines()
        {
            PipingLines = new List<VerticalPipingLine>();
            foreach (Pipe pipe in ReferencePipes)
            {
                string systemTypeName = SetSystemTypeName(pipe);
                VerticalPipingLine line = VerticalPipingLine.Initialize(pipe, systemTypeName, _topLevel, _bottomLevel);
                if (line.GoesAbove || line.GoesBelow) PipingLines.Add(line);
            }
        }

        private void CheckFlags()
        {
            if (_viewType != ViewType.FloorPlan && _viewType != ViewType.CeilingPlan) TaskDialog.Show("Warning", "View must be of type: Grundriss oder Decken Plan");
            if (_isTopUndefined) TaskDialog.Show("Warning", "Top level is undefined");
            if (_isBottomUndefined) TaskDialog.Show("Warning", "Bottom level is undefined");
        }

        private  void FindReferencePipes()
        {
            FilteredElementCollector ficol = new FilteredElementCollector(Document, _activeView.Id);
            ReferencePipes = ficol.OfClass(typeof(Pipe)).Select(x => x as Pipe).Where(p => IsPipeVertical(p) && !IsLocationUsedBySystem(p)).ToList();
        }

        private void SetActiveView()
        {
            _activeView = Document.ActiveView;
            _viewType = Document.ActiveView.ViewType;
        }

        private void SetViewLevels()
        {
            if(_viewType == ViewType.FloorPlan || _viewType == ViewType.CeilingPlan)
            {
                ViewPlan viewPlan = _activeView as ViewPlan;
                PlanViewRange range = viewPlan.GetViewRange();
                ElementId bottomLvlId = range.GetLevelId(PlanViewPlane.BottomClipPlane);
                ElementId topLvlId = range.GetLevelId(PlanViewPlane.TopClipPlane);

                if (bottomLvlId.IntegerValue > 0)
                {
                    //double offset = range.GetOffset(PlanViewPlane.BottomClipPlane);
                    Element bottomLevelAsElement = Document.GetElement(bottomLvlId);
                    Level bottomLevel = bottomLevelAsElement as Level;
                    _bottomLevel = bottomLevel.Elevation;
                }
                else
                {
                    _isBottomUndefined = true;
                }


                if (topLvlId.IntegerValue > 0)
                {
                    //double offset = range.GetOffset(PlanViewPlane.TopClipPlane);
                    Element topLevelAsElement = Document.GetElement(topLvlId);
                    Level topLevel = topLevelAsElement as Level;
                    _topLevel = topLevel.Elevation;
                }
                else
                {
                    _isTopUndefined = true;
                }
            }
        }

        private bool IsPipeVertical(Pipe pipe)
        {
            bool result = false;
            LocationCurve lc = pipe.Location as LocationCurve;
            Line l = lc.Curve as Line;
            XYZ direction = l.Direction;
            double test = Math.Sin(Math.PI / 180 * 89);
            if (Math.Abs(direction.Z) >= test) result = true;
            return result;
        }

        //postponed because location has to be distinguished by system as well
        //private bool IsLocationUsed(Pipe pipe)
        //{
        //    bool result = false;
        //    LocationCurve lc = pipe.Location as LocationCurve;
        //    Line l = lc.Curve as Line;
        //    XYZ origin = l.Origin;
        //    double tolerance = 0.5 * pipe.Diameter;
        //    List<XYZ> test = UsedCoordinates.Where(e => Math.Abs(e.X - origin.X) < tolerance && Math.Abs(e.Y - origin.Y) < tolerance).ToList();
        //    if(test.Count > 0)
        //    {
        //        result = true;
        //    }
        //    else
        //    {
        //        UsedCoordinates.Add(origin);
        //    }
        //    return result;
        //}

        /// <summary>
        /// Checks if the coordinates are used by pipe of the same systemtype name
        /// </summary>
        private bool IsLocationUsedBySystem(Pipe pipe)
        {

            bool result = false;
            LocationCurve lc = pipe.Location as LocationCurve;
            Line l = lc.Curve as Line;
            XYZ origin = l.Origin;
            double tolerance = 0.5 * pipe.Diameter;
            string systemTypeName = SetSystemTypeName(pipe);
            var testList = _systemUsedCoordinates.Where(e => e.Key == systemTypeName).Select(e => e.Value).ToList();
            if(testList.Count == 1)
            {
                List<XYZ> systemCoordinates = _systemUsedCoordinates[systemTypeName];
                List<XYZ> test = systemCoordinates.Where(e => Math.Abs(e.X - origin.X) < tolerance && Math.Abs(e.Y - origin.Y) < tolerance).ToList();
                if (test.Count > 0)
                {
                    result = true;
                }
                else
                {
                    systemCoordinates.Add(origin);
                }
            }
            else
            {
                List<XYZ> newList = new List<XYZ>() { origin };
                _systemUsedCoordinates.Add(systemTypeName, newList);
            }
            return result;
        }

        private string SetSystemTypeName(Pipe pipe)
        {
            ElementId pipingSystemId = pipe.MEPSystem.GetTypeId();
            PipingSystemType pipingSystemType = Document.GetElement(pipingSystemId) as PipingSystemType;
            return pipingSystemType.Name;
        }

        private void SetAllViewTags()
        {
            FilteredElementCollector ficol = new FilteredElementCollector(Document, _activeView.Id);
            _allViewTags = ficol.OfClass(typeof(IndependentTag)).Select(x => x as IndependentTag).ToList();
        }

        private void SetPipeFlowTags()
        {
            _pipeFlowTags = _allViewTags.Where(e => IsOfSelectedFamilySymbol(e)).ToList();
        }

        private bool IsOfSelectedFamilySymbol(IndependentTag tag)
        {
            bool result = false;
            foreach (FamilySymbol fs in SelectedTags)
            {
                ElementId fsId = fs.Id;
                if(tag.GetTypeId().IntegerValue == fsId.IntegerValue)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public void CheckExistingTags()
        {
            foreach (VerticalPipingLine vpl in PipingLines)
            {
                List<Element> elements = vpl.LineElements.Where(e => _taggedElementIds.Contains(e.Id)).ToList();
                if (elements.Count > 0) vpl.IsTagged = true;
            }
        }
    }
}
