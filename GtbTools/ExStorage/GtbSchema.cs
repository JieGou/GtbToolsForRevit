using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExStorage
{
    public class GtbSchema
    {
        public Schema Schema { get; private set; }

        public GtbSchema()
        {

        }

        public void SetGtbSchema()
        {
            Schema = Schema.Lookup(new System.Guid("B7DAE0FA-056A-483C-8F8E-3CA1469528EB"));
            if(Schema == null)
            {
                Schema = CreateGtbSchema();
            }
            
        }

        public bool SetEntityField(FamilyInstance familyInstance, string fieldName, int value)
        {
            bool result = false;
            Entity entity = familyInstance.GetEntity(Schema);
            Field field = Schema.GetField(fieldName);
            try
            {
                entity.Get<int>(field);
                entity.Set(field, value);
                familyInstance.SetEntity(entity);
                result = true;
            }
            catch
            {
                SetSchemaEntity(familyInstance, field, value);
            }
            return result;
        }

        private void SetSchemaEntity(FamilyInstance familyInstance, Field field, int value)
        {
            Entity entity = new Entity(Schema);
            entity.Set<int>(field, value);
            familyInstance.SetEntity(entity);
        }

        private Schema CreateGtbSchema()
        {
            SchemaBuilder schemaBuilder = new SchemaBuilder(new System.Guid("B7DAE0FA-056A-483C-8F8E-3CA1469528EB"));
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);
            schemaBuilder.SetWriteAccessLevel(AccessLevel.Public);
            schemaBuilder.SetVendorId("GTBE");
            schemaBuilder.SetSchemaName("gtb_schema");
            FieldBuilder discipline = schemaBuilder.AddSimpleField("discipline", typeof(int));
            FieldBuilder topSymbol = schemaBuilder.AddSimpleField("topSymbol", typeof(int));
            FieldBuilder lrSymbol = schemaBuilder.AddSimpleField("lrSymbol", typeof(int));
            FieldBuilder fbSymbol = schemaBuilder.AddSimpleField("fbSymbol", typeof(int));
            FieldBuilder abSymbol = schemaBuilder.AddSimpleField("abSymbol", typeof(int));
            FieldBuilder manSymbol = schemaBuilder.AddSimpleField("manSymbol", typeof(int));
            discipline.SetDocumentation("Opening discipline automatic setting");
            topSymbol.SetDocumentation("Top symbol automatic setting");
            lrSymbol.SetDocumentation("Left right symbol automatic setting");
            fbSymbol.SetDocumentation("Front back symbol automatic setting");
            abSymbol.SetDocumentation("Above below symbol automatic setting");
            manSymbol.SetDocumentation("Manual symbol automatic setting");
            return schemaBuilder.Finish();
        }
    }
}
