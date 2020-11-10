using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.UI;
using GUI;
using PipeFlowTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Functions
{
    public class PipeFlowTagger
    {
        #region Properties
        public UIDocument UIDocument { get; set; }
        public Document Document { get; set; }
        public View ActiveView { get; set; }
        public ViewType ViewType { get; set; }
        public double TopLevel { get; set; }
        public double BottomLevel { get; set; }
        public FamilySymbol SelectedTagSymbol { get; set; }
        public List<Pipe> ReferencePipes { get; set; }
        public List<VerticalPipingLine> PipingLines { get; set; }
        public PipeFlowToolAction Action { get; set; }
        public ExternalEvent StartEvent { get; set; }
        public bool IsTopUndefined { get; set; } = false;
        public bool IsBottomUndefined { get; set; } = false;
        public List<XYZ> UsedCoordinates { get; set; }
        public List<LineViewModel> LineViewModels { get; set; }
        public List<FamilySymbol> PipeFittingTags { get; set; }
        public List<FamilySymbol> SelectedTags { get; set; }
        public DefaultDirections DefaultDirections { get; set; }

        public ManualResetEvent SignalEvent = new ManualResetEvent(false);
        #endregion

        public PipeFlowTagger()
        {

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
            CheckFlags();
            FindReferencePipes();
            CreatePipingLines();
            SetViewModelList();
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
            using (Transaction tx = new Transaction(Document, "Adding PIF Tags"))
            {
                tx.Start();
                foreach (VerticalPipingLine line in PipingLines)
                {
                    line.TagTheLine(SelectedTags, Document, DefaultDirections);
                }
                tx.Commit();
            }
        }

        private void SetAnnotationSymbols()
        {
            PipeFittingTags = new List<FamilySymbol>();
            FilteredElementCollector ficol = new FilteredElementCollector(Document);
            PipeFittingTags = ficol.OfClass(typeof(FamilySymbol)).Select(x => x as FamilySymbol)
                                     .Where(y => y.Category.Id.IntegerValue == (int)BuiltInCategory.OST_PipeFittingTags).ToList();
        }

        private void SetViewModelList()
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
                VerticalPipingLine line = VerticalPipingLine.Initialize(pipe, systemTypeName, TopLevel, BottomLevel);
                if (line.GoesAbove || line.GoesBelow) PipingLines.Add(line);
            }
        }

        private void CheckFlags()
        {
            if (IsTopUndefined) TaskDialog.Show("Warning", "Top level is undefined");
            if (IsBottomUndefined) TaskDialog.Show("Warning", "Bottom level is undefined");
        }

        private  void FindReferencePipes()
        {
            FilteredElementCollector ficol = new FilteredElementCollector(Document, ActiveView.Id);
            ReferencePipes = ficol.OfClass(typeof(Pipe)).Select(x => x as Pipe).Where(p => IsPipeVertical(p) && !IsLocationUsed(p)).ToList();
        }

        private void SetActiveView()
        {
            ActiveView = Document.ActiveView;
            ViewType = Document.ActiveView.ViewType;
        }

        private void SetViewLevels()
        {
            if(ViewType == ViewType.FloorPlan || ViewType == ViewType.CeilingPlan)
            {
                ViewPlan viewPlan = ActiveView as ViewPlan;
                PlanViewRange range = viewPlan.GetViewRange();
                ElementId bottomLvlId = range.GetLevelId(PlanViewPlane.BottomClipPlane);
                ElementId topLvlId = range.GetLevelId(PlanViewPlane.TopClipPlane);

                if (bottomLvlId.IntegerValue > 0)
                {
                    //double offset = range.GetOffset(PlanViewPlane.BottomClipPlane);
                    Element bottomLevelAsElement = Document.GetElement(bottomLvlId);
                    Level bottomLevel = bottomLevelAsElement as Level;
                    BottomLevel = bottomLevel.Elevation;
                }
                else
                {
                    IsBottomUndefined = true;
                }


                if (topLvlId.IntegerValue > 0)
                {
                    //double offset = range.GetOffset(PlanViewPlane.TopClipPlane);
                    Element topLevelAsElement = Document.GetElement(topLvlId);
                    Level topLevel = topLevelAsElement as Level;
                    TopLevel = topLevel.Elevation;
                }
                else
                {
                    IsTopUndefined = true;
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

        private bool IsLocationUsed(Pipe pipe)
        {
            bool result = false;
            LocationCurve lc = pipe.Location as LocationCurve;
            Line l = lc.Curve as Line;
            XYZ origin = l.Origin;
            double tolerance = 0.5 * pipe.Diameter;
            List<XYZ> test = UsedCoordinates.Where(e => Math.Abs(e.X - origin.X) < tolerance && Math.Abs(e.Y - origin.Y) < tolerance).ToList();
            if(test.Count > 0)
            {
                result = true;
            }
            else
            {
                UsedCoordinates.Add(origin);
            }
            return result;
        }
        private string SetSystemTypeName(Pipe pipe)
        {
            ElementId pipingSystemId = pipe.MEPSystem.GetTypeId();
            PipingSystemType pipingSystemType = Document.GetElement(pipingSystemId) as PipingSystemType;
            return pipingSystemType.Name;
        }
    }
}
