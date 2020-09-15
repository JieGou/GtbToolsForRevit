using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace OpeningSymbol
{
    public class PlanView
    {
        public View View { get; set; }
        public ViewDiscipline ViewDiscipline {get; set;}
        public ViewDirection ViewDirection { get; set; }
        public List<RectangularOpening> RectangularOpenings { get; set; }
        public List<RoundOpening> RoundOpenings { get; set; }

        public PlanView()
        {

        }

        public void CreateOpeningList()
        {
            if (ViewDiscipline == ViewDiscipline.ARC) CreateArcOpeningList();
            if (ViewDiscipline == ViewDiscipline.TGA) CreateTgaOpeningList();
        }

        private  void CreateTgaOpeningList()
        {

        }
        private void CreateArcOpeningList()
        {

        }
    }
}
