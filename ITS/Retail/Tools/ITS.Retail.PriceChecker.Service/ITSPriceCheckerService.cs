using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace ITS.Retail.PriceChecker.Service
{
    public partial class ITSPriceCheckerService : ServiceBase
    {
        public ITSPriceCheckerService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            SocketServer.StartListening();
        }

        protected override void OnStop()
        {
            
        }
    }
}
