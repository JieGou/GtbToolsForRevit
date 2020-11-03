using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CuttingElementTool
{
    public class CutSearchOpeningModel
    {
        public FamilyInstance FamilyInstance { get; set; }
        public XYZ CenterPoint { get; set; }
        public ElementId CuttingElementId { get; set; }

        public CutSearchOpeningModel(FamilyInstance familyInstance)
        {
            FamilyInstance = familyInstance;
        }

        public void SetCenterPoint()
        {
            XYZ ho = FamilyInstance.HandOrientation;
            LocationPoint locationPoint = FamilyInstance.Location as LocationPoint;
            XYZ originPoint = locationPoint.Point;
            double x = originPoint.X - ho.Y * originPoint.X;
            double y = originPoint.Y + ho.X * originPoint.Y;
            double z = originPoint.Z;
            CenterPoint = new XYZ(x, y, z);
        }
    }
}
