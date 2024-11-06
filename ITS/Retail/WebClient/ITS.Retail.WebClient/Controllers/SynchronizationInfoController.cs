using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.WebClient.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ITS.Retail.Common.ViewModel;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;
using System.Threading;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.Helpers;
using System;

namespace ITS.Retail.WebClient.Controllers
{
    public class SynchronizationInfoController : BaseController
    {
        public ActionResult Index()
        {
            if (ApplicationHelper.IsStoreControllerInstance())
            {
                Updater.Resume("PostSyncInfoThread");
            }
            IEnumerable<SynchronizationInfoViewModel>  model = CreateModel();
            ViewData["IsAdministrator"] = UserHelper.IsSystemAdmin(CurrentUser);
            return View(model);

        }

        public ActionResult PivotGrid()
        {
            IEnumerable<SynchronizationInfoViewModel> model = CreateModel();
            return PartialView(model);
        }


        protected IEnumerable<SynchronizationInfoViewModel> CreateModel()
        {
            eIdentifier deviceType = MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL ? eIdentifier.STORECONTROLLER : eIdentifier.POS ;
            XPCollection<SynchronizationInfo> syncInfo = GetList<SynchronizationInfo>(XpoSession, new BinaryOperator("DeviceType", deviceType));
            List<SynchronizationInfoViewModel> model = syncInfo.Select(item => new SynchronizationInfoViewModel().LoadPersistent(item)).Cast<SynchronizationInfoViewModel>().ToList();

            if (MvcApplication.ApplicationInstance == eApplicationInstance.STORE_CONTROLER)
            {
                XPCollection<SynchronizationInfo> currentSCInfo = GetList<SynchronizationInfo>(XpoSession, new BinaryOperator("DeviceType", eIdentifier.STORECONTROLLER));
                model.AddRange(currentSCInfo.Select(item => new SynchronizationInfoViewModel().LoadPersistent(item)).Cast<SynchronizationInfoViewModel>().ToList());
            }

            return model;
        }

        public void PauseThreads(string ThreadName)
        {
            Updater.Pause(ThreadName);
        }

        public ActionResult UpdaterThreadsGrid()
        {
            List<string> isChecked = new List<string>();
            isChecked.Add(Updater.GetUpdatesThread.ThreadState == ThreadState.Running 
                                                                  || (Updater.pauseEvent != null
                                                                     && Updater.pauseEvent.PauseEventStartUpdateThread !=null
                                                                     && Updater.pauseEvent.PauseEventStartUpdateThread.WaitOne(0)
                                                                     )
                                                                  ? "checked"
                                                                  : "");

            isChecked.Add(Updater.PostSyncInfoThread.ThreadState == ThreadState.Running
                                                                    || (Updater.pauseEvent !=null
                                                                        && Updater.pauseEvent.PauseEventPostSyncInfoThread != null
                                                                        && Updater.pauseEvent.PauseEventPostSyncInfoThread.WaitOne(0)
                                                                       )
                                                                    ? "checked"
                                                                    : "");
            isChecked.Add(Updater.PostRecordsThread.ThreadState == ThreadState.Running
                                                                   || (Updater.pauseEvent != null
                                                                       && Updater.pauseEvent.PauseEventPostRecordsThread != null
                                                                       && Updater.pauseEvent.PauseEventPostRecordsThread.WaitOne(0)
                                                                      )
                                                                   ? "checked"
                                                                   : "");
            ViewData["IsChecked"] = isChecked;
            List<bool> isExceptionSet = new List<bool>();
            isExceptionSet.Add(Updater.pauseEvent.ExceptionThrownStartUpdateThread);
            isExceptionSet.Add(Updater.pauseEvent.ExceptionThrownPostSyncInfoThread);
            isExceptionSet.Add(Updater.pauseEvent.ExceptionThrownPostRecordsThread);
            ViewData["ExceptionThrown"] = isExceptionSet;
            return PartialView(CreateModelUpdaterThreads());
        }

        [HttpPost]
        public ActionResult ControlThreads(string Row, string Checked)
        {
            List<UpdaterThreadsViewModel> model = CreateModelUpdaterThreads();
            int row = Int32.Parse(Row);
            bool start = bool.Parse(Checked);
            if(start == true)
            {
                Updater.Resume(model.ElementAt(row).ThreadName);
            }
            else
            {
                PauseThreads(model.ElementAt(row).ThreadName);
            }
            
            return PartialView(model);
        }

        private List<UpdaterThreadsViewModel> CreateModelUpdaterThreads()
        {
            List<UpdaterThreadsViewModel> model = new List<UpdaterThreadsViewModel>();

            bool startUpdateThreadCheck = Updater.pauseEvent != null && Updater.pauseEvent.PauseEventStartUpdateThread != null && Updater.pauseEvent.PauseEventStartUpdateThread.WaitOne(0);
            model.Add(new UpdaterThreadsViewModel("StartUpdate", Updater.GetUpdatesThread.ThreadState.ToString() == "Running" || startUpdateThreadCheck ? "Running" : "Waiting/On Sleep Mode"));

            bool postSyncInfoThreadCheck = Updater.pauseEvent != null && Updater.pauseEvent.PauseEventPostSyncInfoThread != null && Updater.pauseEvent.PauseEventPostSyncInfoThread.WaitOne(0);
            model.Add(new UpdaterThreadsViewModel("PostSyncInfoThread", Updater.PostSyncInfoThread.ThreadState.ToString() == "Running" || postSyncInfoThreadCheck ? "Running" : "Waiting/On Sleep Mode"));

            bool postRecordsThreadCheck = Updater.pauseEvent != null && Updater.pauseEvent.PauseEventPostRecordsThread != null && Updater.pauseEvent.PauseEventPostRecordsThread.WaitOne(0);
            model.Add(new UpdaterThreadsViewModel("PostRecordsThread",Updater.PostRecordsThread.ThreadState.ToString() == "Running" || postRecordsThreadCheck ? "Running" : "Waiting/On Sleep Mode"));

            return model;
        }
    }
}
