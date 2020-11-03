using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttingElementTool
{
    public class CuttingElement
    {
        //cut openings cutting elements: ducts, pipes, cable trays, conduits
        //yet pipes only
        public ElementId ElementId { get; set; }
        public CuttingElementType Type { get; set; }
        public Line CenterLine { get; set; }

        //continue, slope
    }
}
