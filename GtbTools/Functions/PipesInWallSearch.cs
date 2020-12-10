using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GUI;
using PipesInWall;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Functions
{
    public class PipesInWallSearch
    {
        public ExternalEvent TheEvent { get; set; }
        public PipesInWallViewModel PipesInWallViewModel {get; set;}
        public PipesInWallAction Action { get; set; }

        UIDocument _uidoc;
        Document _doc;

        public PipesInWallSearch()
        {

        }

        public void Initialize(UIDocument uIDocument)
        {
            _uidoc = uIDocument;
            _doc = uIDocument.Document;
            PipesInWallViewModel = PipesInWallViewModel.Initialize(_doc);
        }

        public void DisplayWindow()
        {
            PipesInWallWindow window = new PipesInWallWindow(PipesInWallViewModel);
            window.ShowDialog();
        }

        public void SetEvent(ExternalEvent externalEvent)
        {
            TheEvent = externalEvent;
        }
    }
}
