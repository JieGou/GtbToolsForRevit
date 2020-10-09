using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public class GetSetElevation
    {
        Document doc;
        List<FamilyInstance> _openings;

        public GetSetElevation(Document document)
        {
            doc = document;
        }

        public void GetOpenings()
        {
            FilteredElementCollector ficol = new FilteredElementCollector(doc);
            List<FamilyInstance> genModelInstances = ficol.OfClass(typeof(FamilyInstance))
                                    .Select(x => x as FamilyInstance)
                                        .Where(y => y.Symbol.Family.FamilyCategory.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModel).ToList();
            _openings = new List<FamilyInstance>();
            foreach (var item in genModelInstances)
            {
                Parameter p = item.get_Parameter(new Guid("6674e38a-1c26-498a-bcb0-89856c998d0b"));
                if (p != null) _openings.Add(item);
            }
        }

        public void SetElevations()
        {
            using(Transaction tx = new Transaction(doc, "Copying Elevation to OpElevation"))
            {
                tx.Start();
                foreach (var item in _openings)
                {
                    Parameter defaultElevation = item.get_Parameter(BuiltInParameter.INSTANCE_ELEVATION_PARAM);
                    string defaultValue = defaultElevation.AsValueString();
                    Parameter openingElevation = item.get_Parameter(new Guid("6674e38a-1c26-498a-bcb0-89856c998d0b"));
                    string openingEl = openingElevation.AsValueString();
                    double value = defaultElevation.AsDouble();
                    if (defaultValue != openingEl)
                    {
                        openingElevation.Set(value);
                    }
                }
                tx.Commit();
            }
        }
    }
}
