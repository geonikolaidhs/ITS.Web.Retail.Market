using DevExpress.Xpo;
using ITS.POS.FiscalService.RequestResponseLogging.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Fiscal.GUI
{
    public partial class RequestResponseLogForm : Form
    {
        private List<LogEntry> logEntries;

        public RequestResponseLogForm()
        {
            InitializeComponent();
        }

        private void RequestResponseLogForm_Load(object sender, EventArgs e)
        {
            dataGridViewLog.AutoSize = true;
            XpoHelper.SetTransactionFile("./Service/FiscalServer.sqlite");
            ReadLogEntries();
            dataGridViewLog.DataSource = this.logEntries;
        }

        private void ReadLogEntries()
        {
            if (logEntries != null && logEntries.Count > 0)
            {
                logEntries.Clear();
                logEntries = null;
            }
            logEntries = new List<LogEntry>();
            using ( UnitOfWork uow = XpoHelper.GetNewUnitOfWork()  )
            {
                XPCollection<FiscalServiceLogEntry> fiscalServiceLogEntries = new XPCollection<FiscalServiceLogEntry>(PersistentCriteriaEvaluationBehavior.InTransaction,uow,null);
                foreach (FiscalServiceLogEntry fiscalServiceLogEntry in fiscalServiceLogEntries)
                {
                    logEntries.Add(new LogEntry()
                    {
                        Id = fiscalServiceLogEntry.Id,
                        Receiver = fiscalServiceLogEntry.Receiver,
                        Request = fiscalServiceLogEntry.Request,
                        Response = fiscalServiceLogEntry.Response,
                        Sender = fiscalServiceLogEntry.Sender,
                        TimeStamp = fiscalServiceLogEntry.TimeStamp
                    });
                }
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            dataGridViewLog.DataSource = null;
            ReadLogEntries();
            dataGridViewLog.DataSource = this.logEntries;
        }
    }
}
