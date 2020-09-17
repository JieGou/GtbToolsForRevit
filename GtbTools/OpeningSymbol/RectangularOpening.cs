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
        public OpeningHost OpeningHost { get; set; }

        ViewDirection _viewDirection;
        ViewDiscipline _viewDiscipline;
        bool _isCutByView;
        XYZ _xyz;
        double _x;
        double _y;
        double _z;

        private RectangularOpening()
        {

        }

        public static RectangularOpening Initialize(FamilyInstance familyInstance, ViewDirection viewDirection, ViewDiscipline viewDiscipline, bool isCutByView)
        {
            RectangularOpening result = new RectangularOpening();
            result.FamilyInstance = familyInstance;
            result._viewDirection = viewDirection;
            result._viewDiscipline = viewDiscipline;
            result._isCutByView = isCutByView;
            result.SetInstanceXYZ();
            result.FindElementHost();
            result.SetSymbolVisibility();
            return result;
        }

        private void SetInstanceXYZ()
        {
            _xyz = FamilyInstance.HandOrientation;
            _x = _xyz.X;
            _y = _xyz.Y;
            _z = _xyz.Z;
        }

        private void FindElementHost()
        {
            Element host = FamilyInstance.Host;
            if (host == null)
            {
                OpeningHost = OpeningHost.NotAssociated;
                return;
            }
            switch(host.Category.Name)
            {
                case "Floors":
                    OpeningHost = OpeningHost.FloorOrCeiling;
                    break;
                case "Walls":
                    OpeningHost = OpeningHost.Wall;
                    break;
                case "Ceilings":
                    OpeningHost = OpeningHost.FloorOrCeiling;
                    break;
                default:
                    OpeningHost = OpeningHost.Unknown;
                    break;
            }
        }

        private void SetSymbolVisibility()
        {
            if(_viewDirection == ViewDirection.SectionH)
            {
                if(OpeningHost == OpeningHost.FloorOrCeiling)
                {
                    if (Math.Abs(_x) == 1) SymbolVisibility = SymbolVisibility.FrontBackSymbol;
                    if (Math.Abs(_y) == 1) SymbolVisibility = SymbolVisibility.RightLeftSymbol;
                }
                if(OpeningHost == OpeningHost.Wall)
                {
                    if (Math.Abs(_x) == 1) SymbolVisibility = SymbolVisibility.TopSymbol;
                    if (Math.Abs(_y) == 1) SymbolVisibility = SymbolVisibility.RightLeftSymbol;
                    if (Math.Abs(_z) == 1) SymbolVisibility = SymbolVisibility.FrontBackSymbol;
                }
            }
            if (_viewDirection == ViewDirection.SectionV)
            {
                if (OpeningHost == OpeningHost.FloorOrCeiling)
                {
                    if (Math.Abs(_x) == 1) SymbolVisibility = SymbolVisibility.RightLeftSymbol;
                    if (Math.Abs(_y) == 1) SymbolVisibility = SymbolVisibility.FrontBackSymbol;
                }
                if (OpeningHost == OpeningHost.Wall)
                {
                    if (Math.Abs(_x) == 1) SymbolVisibility = SymbolVisibility.RightLeftSymbol;
                    if (Math.Abs(_y) == 1) SymbolVisibility = SymbolVisibility.TopSymbol;
                    if (Math.Abs(_z) == 1) SymbolVisibility = SymbolVisibility.FrontBackSymbol;
                }
            }
            if (_viewDirection == ViewDirection.PlanDown)
            {
                if (OpeningHost == OpeningHost.FloorOrCeiling) SymbolVisibility = SymbolVisibility.TopSymbol;
                if (OpeningHost == OpeningHost.Wall) SymbolVisibility = SymbolVisibility.FrontBackSymbol;
            }
            if (_viewDirection == ViewDirection.PlanUp)
            {
                if (OpeningHost == OpeningHost.FloorOrCeiling) SymbolVisibility = SymbolVisibility.TopSymbol;
                if (OpeningHost == OpeningHost.Wall) SymbolVisibility = SymbolVisibility.FrontBackSymbol;
            }
        }
        /// <summary>
        /// Requires revit transaction to run properly
        /// </summary>
        public void SwitchSymbol()
        {
            Parameter parARC = FamilyInstance.LookupParameter("ARC");
            Parameter parOben = FamilyInstance.LookupParameter("Ansicht nach Oben (Plan Views)");
            Parameter parTop = FamilyInstance.LookupParameter("Durchbruch durchgeschnitten (ARC, Top Symbol)");
            Parameter parFB = FamilyInstance.LookupParameter("Durchbruch durchgeschnitten (ARC, FB Symbol)");
            Parameter parLR = FamilyInstance.LookupParameter("Durchbruch durchgeschnitten (ARC, LR Symbol)");

            //Settings for ARC
            if (_viewDiscipline == ViewDiscipline.ARC)
            {               
                parARC.Set(1);

                if(OpeningHost == OpeningHost.Wall && _isCutByView)
                {
                    if(SymbolVisibility == SymbolVisibility.FrontBackSymbol)
                    {
                        parFB.Set(1);
                    }
                    if(SymbolVisibility == SymbolVisibility.RightLeftSymbol)
                    {
                        parLR.Set(1);
                    }
                    if(SymbolVisibility == SymbolVisibility.TopSymbol)
                    {
                        parTop.Set(1);
                    }
                }
                if(OpeningHost == OpeningHost.Wall && !_isCutByView)
                {
                    if (SymbolVisibility == SymbolVisibility.FrontBackSymbol)
                    {
                        parFB.Set(0);
                    }
                    if (SymbolVisibility == SymbolVisibility.RightLeftSymbol)
                    {
                        parLR.Set(0);
                    }
                    if (SymbolVisibility == SymbolVisibility.TopSymbol)
                    {
                        parTop.Set(0);
                    }
                }
                if(OpeningHost == OpeningHost.FloorOrCeiling && _isCutByView)
                {
                    if (SymbolVisibility == SymbolVisibility.FrontBackSymbol)
                    {
                        parFB.Set(1);
                    }
                    if (SymbolVisibility == SymbolVisibility.RightLeftSymbol)
                    {
                        parLR.Set(1);
                    }
                }
                if (OpeningHost == OpeningHost.FloorOrCeiling && !_isCutByView)
                {
                    if (SymbolVisibility == SymbolVisibility.FrontBackSymbol)
                    {
                        parFB.Set(0);
                    }
                    if (SymbolVisibility == SymbolVisibility.RightLeftSymbol)
                    {
                        parLR.Set(0);
                    }
                }
            }

            //Settings for TGA
            if(_viewDiscipline == ViewDiscipline.TGA)
            {
                parARC.Set(0);

                if(OpeningHost == OpeningHost.Wall)
                {                    
                    parOben.Set(0);
                }
                if(OpeningHost == OpeningHost.FloorOrCeiling)
                {
                    if (_viewDirection == ViewDirection.PlanDown) parOben.Set(0);
                    if (_viewDirection == ViewDirection.PlanUp) parOben.Set(1);
                }
            }
        }
    }
}
