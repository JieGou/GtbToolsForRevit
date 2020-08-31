/*
 * Created by SharpDevelop.
 * User: m.trawczynski
 * Date: 24.08.2020
 * Time: 17:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace GtbMakros
{
	/// <summary>
	/// Description of RevitTools.
	/// </summary>
	public class RevitTools
	{
		public List<ModelView> ModelViewList {get; set;}
		Document Doc;
		UIDocument UIDoc;
		bool GewerkParameterExists = true;
		Dictionary<ElementId, int> oldViewScales = new Dictionary<ElementId, int>();
		
		public RevitTools(UIDocument uiDoc)
		{
			UIDoc = uiDoc;
			Doc = uiDoc.Document;
		}
		
		public List<View> GetFloorAndCeilingViews()
		{
			FilteredElementCollector ficol = new FilteredElementCollector(Doc);
			ficol.OfClass(typeof(View)).ToList();
			List<View> viewList = new List<View>();
			foreach (View v in ficol) 
			{
				if(v.GetPrimaryViewId() != ElementId.InvalidElementId) continue;
				if(v.IsTemplate) continue;
				if(v.ViewType == ViewType.FloorPlan || v.ViewType == ViewType.CeilingPlan)
				{
					viewList.Add(v);
				}
			}
			return viewList;
		}
		
		public List<ModelView> GetModelViewList(List<View> viewList)
		{
			List<ModelView> result = new List<ModelView>();
			foreach (View v in viewList)
			{
				ModelView mv = new ModelView();
				mv.Name = v.Name;
				mv.IsSelected = false;
				mv.View= v;
				mv.Gewerk = GetGewerkParameterValue(v);
				result.Add(mv);			
			}
			if(!GewerkParameterExists)
			{
				ModelViewList = result;
				return result;
			}
			List<ModelView> orderedResult = result.OrderBy(o => o.Gewerk).ToList();
			
			ModelViewList = orderedResult;
			return orderedResult;
		}
		
		//must be changed to search parameter by GUID
		public string GetGewerkParameterValue(View view)
		{
			string result = null;
			IList<Parameter> pList = view.GetParameters("Gewerk");
			string pName = "GEWERK";
			Parameter par = null;
			if(pList.Count == 0)
			{
				GewerkParameterExists = false;
				return result;
			}
			foreach (Parameter p in pList) 
			{
				if(p.Definition.Name.ToUpper() == pName) par = p;
				result = par.AsString();
			}
			return result;
		}
		
		public void AlignViewScale(View view, View originView)
		{
			if(view.Scale != originView.Scale)
			{
				
				if(oldViewScales.ContainsKey(view.Id))
				{
					return;
				}
				else
				{
					oldViewScales.Add(view.Id, view.Scale);
				}
				using (Transaction tx = new Transaction(Doc, "Changing scale for " + view.Name))
				{
					tx.Start();
					if(view.ViewTemplateId == ElementId.InvalidElementId)
					{
						view.Scale = originView.Scale;	
					}
					else
					{
						View viewTemplate = Doc.GetElement(view.ViewTemplateId) as View;
						viewTemplate.Scale = originView.Scale;
					}
					tx.Commit();
				}
			}
		}
		
		public void RestoreViewsScale()
		{
			if(oldViewScales.Count == 0) return;
			foreach (var pair in oldViewScales) 
			{
				View view = Doc.GetElement(pair.Key) as View;
				using (Transaction tx = new Transaction(Doc, "Restoring scale for " + view.Name))
				{
					tx.Start();										
					if(view.ViewTemplateId == ElementId.InvalidElementId)
					{
						view.Scale = pair.Value;	
					}
					else
					{
						View viewTemplate = Doc.GetElement(view.ViewTemplateId) as View;
						viewTemplate.Scale = pair.Value;
					}
					tx.Commit();
				}				
			}
		}
		
		public void OpenViews(List<View> views, View activeView)
		{
			//IList<XYZ> coords = GetActiveViewPQCoords(activeView);
			
//			foreach (View v in views) 
//			{
//				AlignViewScale(v, activeView);	
//			}
			
			
			foreach (View v in views) 
			{
				UIDoc.ActiveView = v;
				Transaction tx = new Transaction(Doc, "View activation");
				tx.Start();
				
				tx.Commit();
			}
			UIDoc.UpdateAllOpenViews();
		}
		
		public void CloseUnusedViews(List<View> views, View activeView)
		{
			IList<UIView> uiviews = UIDoc.GetOpenUIViews();
			List<int> list = new List<int>();
			list.Add(activeView.Id.IntegerValue);
			foreach (View v in views) 
			{
				list.Add(v.Id.IntegerValue);
			}
			foreach (UIView uiv in uiviews) 
			{
				
				bool delete = true;
				int a = uiv.ViewId.IntegerValue;
				foreach (int e in list) 
				{
					if(a == e) delete = false;
				}
				if(delete) uiv.Close();
			}
 		}
		
		public IList<XYZ> GetActiveViewPQCoords(View activeView)
		{
			IList<XYZ> result = new List<XYZ>();
			UIView uiview = null;
			IList<UIView> uiviews = UIDoc.GetOpenUIViews();
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
		
		public void ApplyCoordsToView(List<View> viewList, View activeView)
		{
			IList<XYZ> coords = GetActiveViewPQCoords(activeView);
			XYZ p = coords[0];
			XYZ q = coords[1];
			foreach (View view in viewList) 
			{
				UIView uiview = GetUIView(view);				
				uiview.ZoomAndCenterRectangle(p,q);
				Transaction tx = new Transaction(Doc, "View activation");
				tx.Start();
				
				tx.Commit();
				uiview.Zoom(2);
			}
		}
		
		private UIView GetUIView(View view)
		{
			UIView uiview = null;
			IList<UIView> uiviews = UIDoc.GetOpenUIViews();
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
		
		//Critical error
		private void SetViewScale(View view, int viewScale)
		{
			using (Transaction tx = new Transaction(Doc, "View scale change"))
			{
				tx.Start();
			    try 
			    {
			    	view.Scale = viewScale;
			    	
			    }
			    catch (Exception ex)
			    {
			    	TaskDialog.Show("Error", ex.ToString());
			    	throw;
			    }
			    tx.Commit();
			}
		}
		
		public static List<ModelView> FilterViewList(List<ModelView> modelViewList, string filter)
		{
			List<ModelView> result = new List<ModelView>();
			
			foreach (ModelView mv in modelViewList) 
			{
				if(mv.Name.ToString().ToUpper().Contains(filter.ToUpper())) result.Add(mv);
				
			}			
			return result;
		}
	}
}
