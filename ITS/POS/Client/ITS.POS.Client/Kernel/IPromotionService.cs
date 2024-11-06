using ITS.POS.Hardware.Common;
using ITS.POS.Model.Master;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    public interface IPromotionService : IPlatformPromotionService, IKernelModule
    {
        void ExecutePromotionResults(DocumentHeader header, ePromotionResultExecutionPlan executionPlan, Device printer, IFormManager formManager, Logger logFile);
    }
}
