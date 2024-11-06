using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;

namespace ITS.Retail.Model
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    //[Updater(Order = 10025,
    // Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_POS)]
    [EntityDisplayName("PromotionApplicationRuleDetailCustomDataViewCondition", typeof(ResourcesLib.Resources))]
    public class PromotionApplicationRuleDetailCustomDataViewCondition : PromotionApplicationRuleDetail, IPromotionApplicationRuleDetailCustomDataViewCondition
    {
    }
}
