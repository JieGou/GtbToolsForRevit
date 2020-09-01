/*
 * Erstellt mit SharpDevelop.
 * Benutzer: m.trawczynski
 * Datum: 31.08.2020
 * Zeit: 14:19
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using System.IO;

namespace GtbTools
{
	/// <summary>
	/// To be used in actual dll
	/// </summary>
	public class ViewCoordsTool
	{
		UIDocument _uiDoc;
		string _coordinatesString;
		
		public ViewCoordsTool(UIDocument uiDoc)
		{
			_uiDoc = uiDoc;
		}
		
		public void ApplyCoordsToViews()
		{
			IList<XYZ> coords = GetActiveViewPQCoords();
			XYZ p = coords[0];
			XYZ q = coords[1];
			
			IList<UIView> viewList = _uiDoc.GetOpenUIViews();
			
			foreach (UIView uiview in viewList) 
			{	
				if(IsFloorOrCeilingView(uiview))
				{
					uiview.ZoomAndCenterRectangle(p,q);
				}
			}
		}

		public void	SaveCurrentCoordinates()
		{
			View view1 = _uiDoc.Document.ActiveView;
			IList<UIView> uiviews = _uiDoc.GetOpenUIViews();
			UIView uiview1 = null;
			foreach( UIView uv in uiviews )
  			{
			    if( uv.ViewId.Equals( view1.Id ) )
			    {
			      uiview1 = uv;
			      break;
			    }
			}		
						
			Rectangle rect = uiview1.GetWindowRectangle();
			IList<XYZ> corners = uiview1.GetZoomCorners();
			XYZ p = corners[0];
			XYZ q = corners[1];
			
			string content = p.X + ":"+ p.Y + ":"+ p.Z + ":"+ q.X + ":"+ q.Y + ":" +q.Z;
			
		    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		    string savePath = Path.Combine(path, "CoordsPQ.txt");
		    _coordinatesString = content;
			
		    File.WriteAllText(savePath, content);
		}
		
		public void SaveCurrentCoordinatesAs()
		{
			View view1 = _uiDoc.Document.ActiveView;
			IList<UIView> uiviews = _uiDoc.GetOpenUIViews();
			UIView uiview1 = null;
			foreach( UIView uv in uiviews )
  			{
			    if( uv.ViewId.Equals( view1.Id ) )
			    {
			      uiview1 = uv;
			      break;
			    }
			}		
						
			Rectangle rect = uiview1.GetWindowRectangle();
			IList<XYZ> corners = uiview1.GetZoomCorners();
			XYZ p = corners[0];
			XYZ q = corners[1];
			
			string content = p.X + ":"+ p.Y + ":"+ p.Z + ":"+ q.X + ":"+ q.Y + ":" +q.Z;
			
		    string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		    string savePath = Path.Combine(path, "CoordsPQ.txt");
		    _coordinatesString = content;
			
			Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
			
			string date = DateTime.Now.ToString("dd.MM.yy HH-mm-ss");
			string name = "Koord_" + date;
			
			dlg.FileName = name; // Default file name
			dlg.DefaultExt = ".txt"; // Default file extension
			dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
			dlg.InitialDirectory = @"H:\Revit\Makros\Gemeinsam genutzte Dateien\Koordinaten";
			Nullable<bool> result =  dlg.ShowDialog();
			
			if (result == true)
			{
			    string filename = dlg.FileName;
			    File.WriteAllText(filename, content);
			}
			
		}
		
		public void LoadCoordinatesFrom()
		{
			Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
			dlg.Filter = "Text documents (.txt)|*.txt"; // Filter files by extension
			dlg.InitialDirectory = @"H:\Revit\Makros\Gemeinsam genutzte Dateien\Koordinaten";
			Nullable<bool> result = dlg.ShowDialog();
			
			if (result == true)
			{
			    string savePath = dlg.FileName;
			   	string content = File.ReadAllText(savePath);
			    string[] coordArray = content.Split(':');
			    XYZ p = new XYZ(Convert.ToDouble(coordArray[0]), Convert.ToDouble(coordArray[1]), Convert.ToDouble(coordArray[2]));
			    XYZ q = new XYZ(Convert.ToDouble(coordArray[3]), Convert.ToDouble(coordArray[4]), Convert.ToDouble(coordArray[5]));

				View view2 = _uiDoc.Document.ActiveView;
				UIView uiview2 = null;
				IList<UIView> uiviews = _uiDoc.GetOpenUIViews();

				foreach( UIView uv in uiviews )
	  			{
				    if( uv.ViewId.Equals( view2.Id ) )
				    {
				      uiview2 = uv;
				      break;
				    }
				}
				uiview2.ZoomAndCenterRectangle(p, q);	
			}
		
		}
		
		public void LoadCoordinates()
		{
			string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
		    string savePath = Path.Combine(path, "CoordsPQ.txt");
		    if(File.Exists(savePath))
	        {
		       	string content = File.ReadAllText(savePath);
			    string[] coordArray = content.Split(':');
			    XYZ p = new XYZ(Convert.ToDouble(coordArray[0]), Convert.ToDouble(coordArray[1]), Convert.ToDouble(coordArray[2]));
			    XYZ q = new XYZ(Convert.ToDouble(coordArray[3]), Convert.ToDouble(coordArray[4]), Convert.ToDouble(coordArray[5]));

				View view2 = _uiDoc.Document.ActiveView;
				UIView uiview2 = null;
				IList<UIView> uiviews = _uiDoc.GetOpenUIViews();

				foreach( UIView uv in uiviews )
	  			{
				    if( uv.ViewId.Equals( view2.Id ) )
				    {
				      uiview2 = uv;
				      break;
				    }
				}
				uiview2.ZoomAndCenterRectangle(p, q);
		    }
		}
		
		private IList<XYZ> GetActiveViewPQCoords()
		{
			View activeView = _uiDoc.Document.ActiveView;
			IList<XYZ> result = new List<XYZ>();
			UIView uiview = null;
			IList<UIView> uiviews = _uiDoc.GetOpenUIViews();
			foreach( UIView uv in uiviews )
  			{
			    if( uv.ViewId.Equals( activeView.Id ) )
			    {
			      uiview = uv;
			      break;
			    }
			}					
			Rectangle rect = uiview.GetWindowRectangle();
			result = uiview.GetZoomCorners();
			return result;
		}
		
		private bool IsFloorOrCeilingView(UIView uiView)
		{
			bool result = false;
			View v = _uiDoc.Document.GetElement(uiView.ViewId) as View;
            if(v.ViewType == ViewType.FloorPlan || v.ViewType == ViewType.CeilingPlan || v.ViewType == ViewType.EngineeringPlan || v.ViewType == ViewType.AreaPlan) result = true;
			return result;
		}
		
		private void ActivateView(UIView uiView)
		{
			View view = _uiDoc.Document.GetElement(uiView.ViewId) as View;
			_uiDoc.ActiveView = view;
		}
		
		private UIView GetUIView(View view)
		{
			UIView uiview = null;
			IList<UIView> uiviews = _uiDoc.GetOpenUIViews();
			foreach( UIView uv in uiviews )
  			{
			    if( uv.ViewId.Equals( view.Id ) )
			    {
			      uiview = uv;
			      break;
			    }
			}
			return uiview;
		}
	}
}
