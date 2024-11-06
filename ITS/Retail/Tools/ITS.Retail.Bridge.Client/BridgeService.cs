using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace ITS.Retail.Bridge.Service
{
    public partial class BridgeService : ServiceBase
    {
        BridgeClient bridge;

        public BridgeService()
        {
            InitializeComponent();
            
        }

        protected override void OnStart(string[] args)
        {
            bridge = new BridgeClient(Retail.Bridge.Service.BridgeClient.Mode.FILEWATCHER,display:true);
        }

        protected override void OnStop()
        {
            
        }
    }
}
