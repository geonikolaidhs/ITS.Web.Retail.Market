using System;

namespace ITS.Retail.WebClient.Controllers
{
    public class SecurityAttribute : Attribute
    {
        public bool ReturnsPartial;
        public bool OverrideSecurity;
        public bool DontLogAction;
        public bool POSSpecific;
        public SecurityAttribute()
        {
            ReturnsPartial = true;
            POSSpecific = false;
        }
    }
}