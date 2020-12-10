using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipesInWall
{
    public class PipeViewModel
    {
        public string ElementId { get; set; }
        public string RefLevel { get; set; }
        public string SystemType { get; set; }
        public string Status { get; set; } = "Unknown";
        public PipeModel PipeModel { get; set; } 

        public PipeViewModel(PipeModel pipeModel)
        {
            PipeModel = pipeModel;
        }

        public void SetViewModel()
        {
            ElementId = PipeModel.ElementId.IntegerValue.ToString();
            RefLevel = PipeModel.RefLevel.Name;
            SystemType = PipeModel.SystemType.AsValueString();
            if (PipeModel.PipeStatus == PipeStatus.OnePoint) Status = "OnePointIn";
            if (PipeModel.PipeStatus == PipeStatus.TwoPointsIn) Status = "TwoPointsIn";
        }
    }
}
