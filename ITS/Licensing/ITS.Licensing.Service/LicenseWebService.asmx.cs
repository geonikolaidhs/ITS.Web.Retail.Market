using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Data.Filtering;
using ITS.Licensing.Library;
using ITS.Licensing.ClientLibrary;
using ITS.Licensing.LicenseModel;


namespace ITS.Licensing.Service
{
    /// <summary>
    /// Summary description for Service1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    public class LicenceWebService : System.Web.Services.WebService
    {
        public enum ValidationStatus
        {
            LICENSE_VALID,
            LICENSE_VERSION_INVALID,
            LICENSE_VALID_UPDATES_EXPIRED,
            LICENSE_INVALID,
            LICENSE_CHANGED,
            LICENSE_MAXIMUM_REACHED
        }

        protected ValidationStatus GetExtendedLicenseObject(Guid ApplicationID, String serialNumber, String MachineID, DateTime InstalledDateTime, bool saveToDatabase, out LicenseExtended extendedLicense)
        {
            SerialNumber sn = XpoDefault.Session.FindObject<SerialNumber>(new BinaryOperator("Number", serialNumber));
            extendedLicense = null;
            ITS.Licensing.LicenseModel.License curretLicense = null;
            if (sn != null)
            {
                if (ApplicationID != sn.Application.ApplicationOid)
                {
                    return ValidationStatus.LICENSE_INVALID;
                }

                foreach (ITS.Licensing.LicenseModel.License ls in sn.Licences)
                {
                    if (ls.MachineID == MachineID)
                    {
                        curretLicense = ls;
                        break;
                    }
                }

                if (curretLicense == null)
                {
                    if (sn.Licences.Count < sn.NumberOfLicenses)
                    {
                        curretLicense = new ITS.Licensing.LicenseModel.License(sn.Session);
                        curretLicense.SerialNumber = sn;
                        curretLicense.MachineID = MachineID;
                    }
                    else
                    {
                        return ValidationStatus.LICENSE_MAXIMUM_REACHED;
                    }
                }
                else
                {
                    if (!saveToDatabase && curretLicense.InstalledVersionDateTime != InstalledDateTime)
                        return ValidationStatus.LICENSE_CHANGED;
                }
                extendedLicense = new LicenseExtended(curretLicense);
                if (saveToDatabase)
                {
                    curretLicense.ActivationKey = extendedLicense.ActivationKey;
                    curretLicense.Save();
                    curretLicense.Session.CommitTransaction();
                }

                if (curretLicense.SerialNumber.StartDate <= DateTime.Now && curretLicense.SerialNumber.FinalDate >= DateTime.Now)
                    return ValidationStatus.LICENSE_VALID;
                return ValidationStatus.LICENSE_VALID_UPDATES_EXPIRED;
            }
            return ValidationStatus.LICENSE_INVALID;

        }


        [WebMethod]
        public bool CheckOnlineStatus()
        {
            return true;
        }

        [WebMethod]
        public ValidationStatus CheckValidLicence(Guid ApplicationID, String serialNumber, String MachineID, String activationKey, DateTime ApplicationBuild, DateTime beginDate, DateTime endDate)
        {
            LicenseExtended extLicense;
            ValidationStatus retvalue = this.GetExtendedLicenseObject(ApplicationID, serialNumber, MachineID, ApplicationBuild, false, out extLicense);
            if (retvalue == ValidationStatus.LICENSE_VALID && (extLicense.License.SerialNumber.StartDate.Ticks - beginDate.Ticks > 1000 || extLicense.License.SerialNumber.FinalDate.Ticks - endDate.Ticks > 1000))
                return ValidationStatus.LICENSE_CHANGED;
            return retvalue;
        }

        [WebMethod]
        public ValidationStatus ActivateLicense(Guid ApplicationID, String serialNumber, String MachineID, DateTime ApplicationBuild, out DateTime beginDate, out DateTime endDate, out String ActivationKey)
        {
            LicenseExtended extLicense;
            ValidationStatus retvalue = this.GetExtendedLicenseObject(ApplicationID, serialNumber, MachineID,ApplicationBuild, true, out extLicense);

            if (retvalue==ValidationStatus.LICENSE_VALID || retvalue == ValidationStatus.LICENSE_VALID_UPDATES_EXPIRED)
            {
                beginDate = extLicense.License.SerialNumber.StartDate;
                endDate = extLicense.License.SerialNumber.FinalDate;
                ActivationKey = extLicense.ActivationKey;

            }
            else
            {
                ActivationKey = "";
                beginDate = endDate = new DateTime(0);
            }
            return retvalue;
        }


        public LicenceWebService()
            : base()
        {
            String conn = DevExpress.Xpo.DB.MSSqlConnectionProvider.GetConnectionString("localhost", "sa", "123456", "Licenses");
            XpoDefault.DataLayer = XpoDefault.GetDataLayer(conn, AutoCreateOption.DatabaseAndSchema);
            XpoDefault.Session = new Session(XpoDefault.DataLayer);

        }
    }
}
