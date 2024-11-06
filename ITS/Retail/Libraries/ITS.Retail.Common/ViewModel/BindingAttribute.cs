using System;


namespace ITS.Retail.Common.ViewModel
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindingAttribute : Attribute
    {
        public BindingAttribute(String requestKey)
        {
            BindingRequestKey = requestKey;
            ComplexObject = false;
            IsCheckBox = false;
            Separator = "_";
            DoNotBind = false;
        }

        public String BindingRequestKey { get; set; }
        public bool ComplexObject { get; set; }
        public String Separator { get; set; }
        public bool IsCheckBox { get; set; }
        public object DefaultValue { get; set; }
        public bool DoNotBind { get; set; }

        public Type EnumerableType { get; set; }
    }
}