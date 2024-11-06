using DevExpress.Xpo;
using ITS.POS.Fiscal.Common;
using ITS.POS.FiscalService.RequestResponseLogging.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Fiscal.Service.RequestResponceLogging
{
    public class RequestResponseLogger
    {
        private bool KeepLog { get; set; }

        public RequestResponseLogger(bool keepLog)
        {
            this.KeepLog = keepLog;
        }

        public void Log(int id, string sender, string receiver, string request, string response)
        {
            if (this.KeepLog == false)
            {
                return;
            }

            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                try
                {
                    FiscalServiceLogEntry logEntry = new FiscalServiceLogEntry(uow)
                    {
                        Id = id,
                        Receiver = receiver,
                        Sender = sender,
                        TimeStamp = DateTime.Now,
                        Request = request,
                        Response = response
                    };
                    //logEntry.Save();
                    uow.Save(logEntry);
                    uow.CommitChanges();
                }
                catch (Exception exception)
                {
                    throw;
                }
            }
        }
    }
}