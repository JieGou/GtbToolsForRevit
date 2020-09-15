using Autodesk.Revit.DB;
using GtbTools;
using OpeningSymbol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                    SectionView sectionView = new SectionView(OpeningWindowMainViewModel.Document);
                    sectionView.View = mv.View;
                    sectionView.ViewDiscipline = viewDiscipline;
                }
            }
        }
        private void CreatePlanViews()
        {

        }
    }
}
