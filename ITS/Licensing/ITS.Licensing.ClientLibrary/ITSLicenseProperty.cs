using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.Licensing.ClientLibrary
{
    public class ITSLicensePropertyAttribute: System.Attribute
    {
        public long BuildDateTime;
        public ITSLicensePropertyAttribute()
        {
            BuildDateTime = DateTime.Now.Ticks;
        }
    }
}
