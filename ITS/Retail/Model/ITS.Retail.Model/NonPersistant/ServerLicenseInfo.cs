using DevExpress.Xpo;
using System;

namespace ITS.Retail.Model.NonPersistant
{
    [NonPersistent]
    public class ServerLicenseInfo
    {
        public int MaxConnectedUsers { get; set; }

        public int MaxPeripheralsUsers { get; set; }

        public int MaxTabletSmartPhoneUsers { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int DaysToAlertBeforeExpiration { get; set; }

        public int GreyZoneDays { get; set; }
    }
}
