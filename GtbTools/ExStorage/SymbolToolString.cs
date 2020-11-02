using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ExStorage
{
    class SymbolToolString
    {
        public int Discipline { get; set; }
        public int TopSymbol { get; set; }
        public int LRSymbol { get; set; }
        public int FBSymbol { get; set; }
        public int ABSymbol { get; set; }
        public int ManSymbol { get; set; }

        private SymbolToolString()
        {

        }

        public static SymbolToolString ReadJsonString(string jsonString)
        {
            SymbolToolString result = JsonConvert.DeserializeObject<SymbolToolString>(jsonString);
            return result;
        }

        public static string CreateJsonString(OpeningExStorage openingExStorage)
        {
            SymbolToolString sts = new SymbolToolString();
            sts.Discipline = openingExStorage.Discipline;
            sts.TopSymbol = openingExStorage.TopSymbol;
            sts.LRSymbol = openingExStorage.LRSymbol;
            sts.FBSymbol = openingExStorage.FBSymbol;
            sts.ABSymbol = openingExStorage.ABSymbol;
            sts.ManSymbol = openingExStorage.ManSymbol;
            string result = JsonConvert.SerializeObject(sts);
            return result;
        }
    }
}
