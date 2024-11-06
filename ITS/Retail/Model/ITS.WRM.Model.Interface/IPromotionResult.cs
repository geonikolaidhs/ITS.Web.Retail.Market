using ITS.Retail.Platform.Enumerations;
using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IPromotionResult: IBaseObj
    { 
        IPromotion Promotion { get; set; }
        ePromotionResultExecutionPlan ExecutionPlan { get; set; }
    }
}
