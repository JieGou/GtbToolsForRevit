using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Plumbing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipesInWall
{
    public class PipeModel
    {
        public ElementId ElementId { get; set; }
        public Level RefLevel { get; set; }
        public Parameter SystemType { get; set; }
        public PipeStatus PipeStatus { get; set; }

        Pipe _pipe;

        public PipeModel(Pipe pipe)
        {
            _pipe = pipe;
        }

        public void SetProperties()
        {
            ElementId = _pipe.Id;
            RefLevel = _pipe.ReferenceLevel;
            SystemType = _pipe.get_Parameter(BuiltInParameter.RBS_PIPING_SYSTEM_TYPE_PARAM);

        }
    }
}
