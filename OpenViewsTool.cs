/*
 * Created by SharpDevelop.
 * User: m.trawczynski
 * Date: 08/31/2020
 * Time: 11:36
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Windows.Threading;

namespace GtbMakros
{
	/// <summary>
	/// This class will be used to link with GTB dock panel. Implement InotifyProperty change on VS
	/// </summary>
	public class OpenViewsTool
	{
		public List<ModelView> ModelViewList {get; set;}
		public WindowResult WindowResult {get; set;}
		
		UIDocument _uiDoc;
		Document _doc;
		View _activeView;
		
		public OpenViewsTool(UIDocument uiDoc)
		{
			_uiDoc = uiDoc;
			_doc = uiDoc.Document;
			_activeView = _doc.ActiveView;
			//CreateModelViewList();
		}
		
		public void OpenViews()
		{
			ViewCoordsTool vct = new ViewCoordsTool(_uiDoc);
			
			if(WindowResult != WindowResult.UserApply) return;
			foreach (ModelView mv in ModelViewList) 
			{
				if(mv.IsSelected) 
				{
					_uiDoc.ActiveView = mv.View;
					DoEvents();
				}
				_uiDoc.ActiveView = _activeView;
			}
		}
		
		public void CreateModelViewList()
		{
			List<View> viewList = GetFloorAndCeilingViews();
			ModelViewList = new List<ModelView>();
			foreach (View v in viewList)
			{
				ModelView mv = new ModelView();
				mv.Name = v.Name;
				mv.IsSelected = false;
				mv.View= v;
				ModelViewList.Add(mv);		
			}
		}
		
		private List<View> GetFloorAndCeilingViews()
		{
			FilteredElementCollector ficol = new FilteredElementCollector(_doc);
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
		
		public void DoEvents()
		{
    		DispatcherFrame frame = new DispatcherFrame();
    		Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
        	new DispatcherOperationCallback(ExitFrame), frame);
    		Dispatcher.PushFrame(frame);
		}

		public object ExitFrame(object f)
		{
    		((DispatcherFrame)f).Continue = false;
   
    		return null;
		}
	}
	public enum WindowResult
	{
		UserApply,
		UserClosed
	}
}
