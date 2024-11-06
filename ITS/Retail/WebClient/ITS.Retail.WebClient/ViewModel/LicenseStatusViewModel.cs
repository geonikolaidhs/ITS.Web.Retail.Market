using ITS.Licensing.Enumerations;
using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel
{
    public class LicenseStatusViewModel
    {
        public LicenseStatus LicenseStatus { get; set; }

        public LicenseType LicenseType { get; set; }

        public int CurrentUsers { get; set; }

        public int MaxUsers { get; set; }
        public int MaxPeripheralUsers { get; set; }
        public int MaxTabletSmartPhoneUsers { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string ErrorMessage { get; set; }

        public int GreyZoneDays { get; set; }
        public int DaysToAlertBeforeExpiration { get; set; }

        public bool InGreyZone
        {
            get
            {
                DateTime today = DateTime.Today;
                DateTime greyZoneDateTime = EndDate.AddDays(GreyZoneDays);
                return EndDate.Date < today && today <= greyZoneDateTime.Date;
            }
        }

        public string GreyZoneMessage
        {
            get
            {
                string messsage = String.Empty;
                if (InGreyZone)
                {
                    messsage = String.Format(Resources.YourLicenseWillExpireInDays, EndDate.ToShortDateString(), EndDate.AddDays(GreyZoneDays).Subtract(DateTime.Now).Days);
                }
                return messsage;
            }
        }

        public bool IsActive
        {
            get
            {
                return StartDate.Date <= DateTime.Today && DateTime.Today <= EndDate.Date;
            }
        }

        public LicenseStatus LicenseStatusComputed
        {

            get
            {
                if(InGreyZone || IsActive)
                {
                    return LicenseStatus.Valid;
                }
                return LicenseStatus.Invalid;
            }
        }
    }
}