using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IPromotionApplicationRuleGroup: IBaseObj
    {
        IPromotion Promotion { get; set; }
        //IGroupOperatorType Operator { get; set; }
        Guid? ParentPromotionApplicationRuleGroupOid { get; set; }
    }
}
