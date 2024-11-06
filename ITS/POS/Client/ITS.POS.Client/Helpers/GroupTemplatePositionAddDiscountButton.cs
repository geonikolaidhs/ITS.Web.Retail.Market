using ITS.POS.Client.UserControls;
using System.ComponentModel;

namespace ITS.POS.Client.Helpers
{
    [TypeConverterAttribute(typeof(ExpandableObjectConverter))]
    public class GroupTemplatePositionAddDiscountButton : GroupTemplatePosition<ucAddDiscountButton>
    {
        public string DiscountTypeCode { get; set; }

        /// <summary>
        /// Optional. The ValueOrPercentage to use when applying the discount
        /// </summary>
        public decimal ValueOrPercentage { get; set; }

        protected override void PrepareControl(ucAddDiscountButton control)
        {
            control.DiscountTypeCode = DiscountTypeCode;
            control.ValueOrPercentage = ValueOrPercentage;
        }
    }
}
