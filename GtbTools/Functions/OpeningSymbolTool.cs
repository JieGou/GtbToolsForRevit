using Autodesk.Revit.DB;
using GtbTools;
using OpeningSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ViewModels;

namespace Functions
{
    public class OpeningSymbolTool
    {
        public OpeningWindowMainViewModel OpeningWindowMainViewModel { get; set; }
        public List<SectionView> SectionViews { get; set; }
        public List<PlanView> PlanViews { get; set; }

        private OpeningSymbolTool()
        {

        }

        public static OpeningSymbolTool Initialize(OpeningWindowMainViewModel openingWindowMainViewModel)
        {
            OpeningSymbolTool result = new OpeningSymbolTool();
            result.OpeningWindowMainViewModel = openingWindowMainViewModel;
            result.CreateSectionViews();



            return result;
        }

        private void CreateSectionViews()
        {
            SectionViews = new List<SectionView>();
            OpeningSymbol.ViewDiscipline viewDiscipline = OpeningWindowMainViewModel.ViewDiscipline;
            foreach (ModelView mv in OpeningWindowMainViewModel.SectionViews)
            {
                if(mv.IsSelected)
                {
                    SectionView sectionView = new SectionView(OpeningWindowMainViewModel.Document, mv.View, viewDiscipline);
                    sectionView.FindCutElements(OpeningWindowMainViewModel.Document);
                    sectionView.CreateOpeningLists();
                    SectionViews.Add(sectionView);
                }
            }
        }
        private void CreatePlanViews()
        {

        }

        public void ProcessSectionViews()
        {
            MessageBox.Show("Processing sections");



            foreach (SectionView sectionView in SectionViews)
            {
                using (Transaction tx = new Transaction(OpeningWindowMainViewModel.Document, "Changing symbols on: " + sectionView.View.Name))
                {
                    tx.Start();
                    foreach (RoundOpening ro in sectionView.RoundOpenings)
                    {
                        ro.SwitchSymbol();
                    }
                    foreach (RectangularOpening ro in sectionView.RectangularOpenings)
                    {
                        ro.SwitchSymbol();
                    }
                    tx.Commit();
                }
            }

        }
    }
}
