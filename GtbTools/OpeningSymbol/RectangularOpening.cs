using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace OpeningSymbol
{
    public class RectangularOpening
    {
        public FamilyInstance FamilyInstance { get; set; }
        public SymbolVisibility SymbolVisibility {get; set;}
        public bool IsCutByView { get; set; }
    }
}
