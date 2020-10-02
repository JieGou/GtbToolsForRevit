using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExStorage
{
    public class OpeningMemory
    {
        public string NewPosition { get; set; }
        public string NewDimensions { get; set; }
        public string NewDateSaved { get; set; }
        public string OldPosition { get; set; }
        public string OldDimensions { get; set; }
        public string OldDateSaved { get; set; }
        public GtbSchema GtbSchema { get; set; }
        public bool IsDimChanged { get; set; }
        public bool IsPosChanged { get; set; }
        public bool IsNew { get; set; } = false;

        Schema _schema;
        public FamilyInstance _familyInstance;

        private OpeningMemory()
        {

        }

        public static OpeningMemory Initialize(FamilyInstance familyInstance)
        {
            OpeningMemory result = new OpeningMemory();
            result._familyInstance = familyInstance;
            result.ReadCurrentSettings();
            result.ReadExternalStorage();
            result.CompareData();
            return result;
        }

        private void CompareData()
        {
            if(OldDateSaved == "-1")
            {
                IsNew = true;
                return;
            }
            if(OldPosition == NewPosition)
            {
                IsPosChanged = false;
            }
            else
            {
                IsPosChanged = true;
            }
            if (OldDimensions == NewDimensions)
            {
                IsDimChanged = false;
            }
            else
            {
                IsDimChanged = true;
            }
        }

        public void SavePositionTostorage()
        {
            GtbSchema.SetEntityField(_familyInstance, "positionXYZ", NewPosition);
        }

        public void SaveDimensionsTostorage()
        {
            GtbSchema.SetEntityField(_familyInstance, "dimensions", NewDimensions);
        }

        public void SaveDateTostorage()
        {
            GtbSchema.SetEntityField(_familyInstance, "dateSaved", NewDateSaved);
        }

        private void ReadCurrentSettings()
        {
            LocationPoint locationPoint = _familyInstance.Location as LocationPoint;
            NewPosition = locationPoint.Point.X.ToString() + ";" + locationPoint.Point.Y.ToString() + ";" + locationPoint.Point.Z.ToString();
            Parameter depth = _familyInstance.get_Parameter(new Guid("17a96ef5-1311-49f2-a0d1-4fe5f3f3854b"));
            Parameter diameter = _familyInstance.get_Parameter(new Guid("9c805bcc-ebc9-4d4c-8d73-26970789417a"));
            Parameter width = _familyInstance.get_Parameter(new Guid("46982e85-76c3-43fb-828f-ddf7a643566f"));
            Parameter height = _familyInstance.get_Parameter(new Guid("8eb274b3-fc0c-43e0-a46b-236bf59f292d"));
            if(depth != null && width != null && height != null)
            {
                NewDimensions = width.AsDouble().ToString() + "x" + height.AsDouble().ToString() + "x" + depth.AsDouble().ToString();
            }
            if (depth != null && diameter != null)
            {
                NewDimensions = diameter.AsDouble().ToString() + "x" + depth.AsDouble().ToString();
            }
            NewDateSaved = DateTime.Now.ToString("dd-MM-yyyy");
        }

        private void ReadExternalStorage()
        {
            GetSchema();
            Entity retrievedEntity = _familyInstance.GetEntity(_schema);
            for (int i = 0; i < 6; i++)
            {
                try
                {
                    if (i == 0) OldPosition = retrievedEntity.Get<string>(_schema.GetField("positionXYZ"));
                    if (i == 1) OldDimensions = retrievedEntity.Get<string>(_schema.GetField("dimensions"));
                    if (i == 2) OldDateSaved = retrievedEntity.Get<string>(_schema.GetField("dateSaved"));
                }
                catch
                {
                    if (i == 0) OldPosition = "-1";
                    if (i == 1) OldDimensions = "-1";
                    if (i == 2) OldDateSaved = "-1";
                }
            }
        }

        private void GetSchema()
        {
            GtbSchema = new GtbSchema();
            GtbSchema.SetGtbSchema();
            _schema = GtbSchema.Schema;
        }
    }
}
