namespace ITS.Retail.Platform.Kernel.Model
{
    public interface IPromotionApplicationRuleDetailCustomDataViewParameter : IPromotionApplicationRuleDetail
    {
        string Description { get; }
        string Name { get; }
        string Type { get; }
        object Value { get; }
    }
}
