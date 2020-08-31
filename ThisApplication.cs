/*
 * Created by SharpDevelop.
 * User: m.trawczynski
 * Date: 24.08.2020
 * Time: 14:25
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GtbMakros
{	
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    [Autodesk.Revit.DB.Macros.AddInId("8BEF5C20-C7E0-4B2D-AFB3-AA2D0B63AF5B")]
	public partial class ThisApplication
	{
		
		private void Module_Startup(object sender, EventArgs e)
		{

		}

		private void Module_Shutdown(object sender, EventArgs e)
		{

		}

		#region Revit Macros generated code
		private void InternalStartup()
		{
			this.Startup += new System.EventHandler(Module_Startup);
			this.Shutdown += new System.EventHandler(Module_Shutdown);
		}
		#endregion
		
		public ErrorLog ErrorLog {get; set;}
		public Document Doc {get; private set;}
		public UIDocument UIDoc {get; set;}
		public GeneralTools GeneralTools {get; set;}
		public RevitTools RevitTools {get; set;}
		
		//Cant use Module Startup because it is initialized on build only
		//Initialization must be performed in every method for now
		private void Initialize(string commandName)
		{
			ErrorLog = new ErrorLog();
			ErrorLog.WriteToLog(commandName);
			Doc = this.ActiveUIDocument.Document;
			UIDoc = this.ActiveUIDocument;
			GeneralTools = new GeneralTools();
			RevitTools = new RevitTools(UIDoc);
		}
					
		public void AnsichtKoordinatenSpeichern()
		{
			this.Initialize("AnsichtKoordinatenSpeichern command started");
			ViewCoordsTool vct = new ViewCoordsTool(ActiveUIDocument);
			vct.SaveCurrentCoordinates();
		    ErrorLog.WriteToLog("Command ended successfully.");
		}
		
		public void AnsichtKoordinatenLaden()
		{
			Initialize("AnsichtKoordinatenLaden command started");
			ViewCoordsTool vct = new ViewCoordsTool(ActiveUIDocument);
			vct.LoadCoordinates();
		    ErrorLog.WriteToLog("Command ended successfully.");
		}
		
		public void SaveCoordsAs()
		{
			ViewCoordsTool vct = new ViewCoordsTool(ActiveUIDocument);
			vct.SaveCurrentCoordinatesAs();
		}
		
		public void LoadCoordsFrom()
		{
			ViewCoordsTool vct = new ViewCoordsTool(ActiveUIDocument);
			vct.LoadCoordinatesFrom();
		}
		
		public void OffeneAnsichten()
		{
//			Initialize("OffeneAnsichten command started");
//			List<View> viewList = RevitTools.GetFloorAndCeilingViews();
//			List<ModelView> modelViewList = RevitTools.GetModelViewList(viewList);
//			ZoomWindow zm = new ZoomWindow(RevitTools);
//			zm.ShowDialog();
//			
//			if(zm.StringResult == "APPLY")
//			{
//				modelViewList = zm.ModelViewList;
//				List<View> selectedViewList = new List<View>();				
//				View originActiveView = UIDoc.ActiveView;
//				ErrorLog.WriteToLog("Active view: " + originActiveView.Name);
//				ErrorLog.WriteToLog("User selected views:");
//				foreach (ModelView mv in modelViewList) 
//				{
//					if(mv.IsSelected)
//					{
//						selectedViewList.Add(mv.View);
//						ErrorLog.WriteToLog(mv.View.Name + ", View type: " + mv.View.ViewType.ToString());
//					}
//				}
//				ErrorLog.WriteToLog("Getting active view rectangle coordinates...");
//				IList<XYZ> list = RevitTools.GetActiveViewPQCoords(originActiveView);
//				ErrorLog.WriteToLog("Opening views...");
//				RevitTools.OpenViews(selectedViewList, originActiveView);				
//				
//				ErrorLog.WriteToLog("Setting active view...");
//				UIDoc.ActiveView = originActiveView;
//				ErrorLog.WriteToLog("Closing unused views...");
//				RevitTools.CloseUnusedViews(selectedViewList, Doc.ActiveView);
//				//UIDoc.UpdateAllOpenViews();
//				//RevitCommandId id2 = RevitCommandId.LookupPostableCommandId(PostableCommand.TileViews);
//				//UIDoc.Application.PostCommand(id2);
//				ErrorLog.WriteToLog("Applying coords to views...");
//				RevitTools.ApplyCoordsToView(selectedViewList, originActiveView);
//				UIDoc.UpdateAllOpenViews();
//				//RevitTools.RestoreViewsScale();
//				RevitTools.ApplyCoordsToView(selectedViewList, originActiveView);
//				
//				ErrorLog.WriteToLog("Command ended successfully.");
//			}			
//			//RevitCommandId id = RevitCommandId.LookupPostableCommandId(PostableCommand.CascadeWindows);
//			//UIDoc.Application.PostCommand(id);
		}
		public void SetScale()
		{
			Initialize("SetScale");
			View view = Doc.ActiveView;
			View viewTemplate = Doc.GetElement(view.ViewTemplateId) as View;
			
			using (Transaction tx = new Transaction(Doc, "View scale temp change"))
			{
				tx.Start();
				viewTemplate.Scale = 100;
				tx.Commit();
			}
			
		}
		public void TabTest()
		{
			View view = Doc.ActiveView;
			
		}
		
		public void CleanOpenViews()
		{
			OpenViewsTool ovt = new OpenViewsTool(this.ActiveUIDocument);
			ovt.CreateModelViewList();
			ZoomWindow zm = new ZoomWindow(ovt);
			zm.ShowDialog();
			if(ovt.WindowResult == WindowResult.UserApply) ovt.OpenViews();
			CleanApplyCoordsToOpenViews();
		}
		
		public void CleanApplyCoordsToOpenViews()
		{
			ViewCoordsTool vct = new ViewCoordsTool(ActiveUIDocument);
			vct.ApplyCoordsToViews();
		}
	}
}