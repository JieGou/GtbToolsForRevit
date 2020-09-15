using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpeningSymbol
{
    public class SectionView
    {
        public View View { get; set; }
        public ViewDiscipline ViewDiscipline { get; set; }
        public List<RectangularOpening> RectangularOpenings { get; set; }
        public List<RoundOpening> RoundOpenings { get; set; }

        Document _doc;

        public SectionView(Document doc)
        {
            _doc = doc;
        }

        public void CreateOpeningList()
        {
            if (ViewDiscipline == ViewDiscipline.ARC) CreateArcOpeningList();
            if (ViewDiscipline == ViewDiscipline.TGA) CreateTgaOpeningList();
        }

        private void CreateTgaOpeningList()
        {

        }
        private void CreateArcOpeningList()
        {

        }
    }
}
