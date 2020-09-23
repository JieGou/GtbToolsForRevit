using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using GtbTools;
using GUI;
using OpeningSymbol;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ViewModels;
using ExStorage;

namespace Functions
{
    public class OpeningSymbolTool
    {
        public OpeningWindowMainViewModel OpeningWindowMainViewModel { get; set; }
        public List<SectionView> SectionViews { get; set; }
        public List<PlanView> PlanViews { get; set; }
        public OperationStatus OperationStatus { get; set; }
        public GtbSchema GtbSchema { get; set; }

        private OpeningSymbolTool()
        {

        }

        public static OpeningSymbolTool Initialize(OpeningWindowMainViewModel openingWindowMainViewModel)
        {
            OpeningSymbolTool result = new OpeningSymbolTool();
            result.OpeningWindowMainViewModel = openingWindowMainViewModel;
            result.CreateSectionViews();
            result.CreatePlanViews();



            return result;
        }

        private void CreateSectionViews()
        {
            SectionViews = new List<SectionView>();
            OpeningSymbol.ViewDiscipline viewDiscipline = OpeningWindowMainViewModel.ViewDiscipline;
            foreach (ModelView mv in OpeningWindowMainViewModel.SectionViews)
            {
                if(mv.IsSelected)
                {
                    SectionView sectionView = new SectionView(OpeningWindowMainViewModel.Document, mv.View, viewDiscipline);
                    sectionView.FindCutElements(OpeningWindowMainViewModel.Document);
                    sectionView.CreateOpeningLists();
                    SectionViews.Add(sectionView);
                }
            }
        }
        private void CreatePlanViews()
        {
            PlanViews = new List<PlanView>();
            OpeningSymbol.ViewDiscipline viewDiscipline = OpeningWindowMainViewModel.ViewDiscipline;
            foreach (ModelView mv in OpeningWindowMainViewModel.PlanViews)
            {
                if(mv.IsSelected)
                {
                    PlanView planView = new PlanView(OpeningWindowMainViewModel.Document, mv.View, viewDiscipline);
                    planView.CreateOpeningList();
                    PlanViews.Add(planView);
                }
            }
        }

        public void ProcessSelectedViews()
        {
            Dispatcher dispatcher;
            OperationStatus = OperationStatus.Initialize("Symbol changing tool initialized...");

            Thread windowThread = new Thread(delegate ()
            {
                ProcessWindow processWindow = new ProcessWindow(OperationStatus);
                processWindow.Show();
                OperationStatus.SignalEvent.Set();
                Dispatcher.Run();
            });
            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.Start();

            OperationStatus.SignalEvent.WaitOne();
            OperationStatus.SignalEvent.Reset();

            OperationStatus.AddLineToTextMessage("Processing section views:");

            using(TransactionGroup transactionGroup = new TransactionGroup(OpeningWindowMainViewModel.Document))
            {
                transactionGroup.Start("GTB Symbol Tool");

                foreach (SectionView sectionView in SectionViews)
                {
                    string info = "Changing symbols on: " + sectionView.View.Name;
                    OperationStatus.AddLineToTextMessage(info);
                    OperationStatus.AddLineToTextMessage(String.Format("Found {0} round openings", sectionView.RoundOpenings.Count));
                    OperationStatus.AddLineToTextMessage(String.Format("Found {0} rectangular openings", sectionView.RectangularOpenings.Count));
                    using (Transaction tx = new Transaction(OpeningWindowMainViewModel.Document))
                    {
                        tx.Start(info);
                        foreach (RoundOpening ro in sectionView.RoundOpenings)
                        {
                            ro.SwitchSymbol(GtbSchema);
                        }
                        foreach (RectangularOpening ro in sectionView.RectangularOpenings)
                        {
                            ro.SwitchSymbol(GtbSchema);
                        }
                        tx.Commit();
                    }
                    if (OperationStatus.UserAborted)
                    {
                        OperationStatus.AddLineToTextMessage("Process aborted by user. Rolling back changes...");
                        OperationStatus.ShowCountDown(3);
                        dispatcher = Dispatcher.FromThread(windowThread);
                        dispatcher.Invoke(OperationStatus.CloseOperationWindow);
                        transactionGroup.RollBack();
                        return;
                    }
                }

                OperationStatus.AddLineToTextMessage("Processing plan views:");

                foreach (PlanView planView in PlanViews)
                {
                    string info = "Changing symbols on: " + planView.View.Name;
                    OperationStatus.AddLineToTextMessage(info);
                    OperationStatus.AddLineToTextMessage(String.Format("Found {0} round openings", planView.RoundOpenings.Count));
                    OperationStatus.AddLineToTextMessage(String.Format("Found {0} rectangular openings", planView.RectangularOpenings.Count));
                    using (Transaction tx = new Transaction(OpeningWindowMainViewModel.Document))
                    {
                        tx.Start(info);
                        foreach (RoundOpening ro in planView.RoundOpenings)
                        {
                            ro.SwitchSymbol(GtbSchema);
                        }
                        foreach (RectangularOpening ro in planView.RectangularOpenings)
                        {
                            ro.SwitchSymbol(GtbSchema);
                        }
                        tx.Commit();
                    }
                    if (OperationStatus.UserAborted)
                    {
                        OperationStatus.AddLineToTextMessage("Process aborted by user. Rolling back changes...");
                        OperationStatus.ShowCountDown(3);
                        dispatcher = Dispatcher.FromThread(windowThread);
                        dispatcher.Invoke(OperationStatus.CloseOperationWindow);
                        transactionGroup.RollBack();
                        return;
                    }
                }

                transactionGroup.Assimilate();
            }
            OperationStatus.DisableAbortButton();
            OperationStatus.AddLineToTextMessage("Application ended successfully!");
            OperationStatus.AddLineToTextMessage("Symbols have been changed.");
            OperationStatus.CloseButtonEnabled = true;
            //OperationStatus.ShowCountDown(5);
            //dispatcher = Dispatcher.FromThread(windowThread);
            //dispatcher.Invoke(OperationStatus.CloseOperationWindow);
        }
    }
}
