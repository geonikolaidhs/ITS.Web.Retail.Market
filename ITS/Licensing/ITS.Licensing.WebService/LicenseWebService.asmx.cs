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
using System.Xml.Linq;
using System.Text;
using SevenZip.Compression.LZMA;
using System.Collections.Generic;
using ITS.Licensing.Web.Helpers;


namespace ITS.Licensing.Web
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
            using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
			{
				SerialNumber sn = uow.FindObject<SerialNumber>(new BinaryOperator("Number", serialNumber));
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

					//InstalledDateTime < sn.StartDate ||
					if ( InstalledDateTime > sn.FinalDate)
					{
						return ValidationStatus.LICENSE_VERSION_INVALID;
					}

					if (curretLicense == null)
					{
						if (sn.Licences.Count < sn.NumberOfLicenses)
						{

							curretLicense = new ITS.Licensing.LicenseModel.License(uow);
							curretLicense.SerialNumber = sn;
							curretLicense.MachineID = MachineID;
							curretLicense.InstalledVersionDateTime = InstalledDateTime;
						}
						else
						{
							return ValidationStatus.LICENSE_MAXIMUM_REACHED;
						}
					}
					else
					{
						if (!saveToDatabase && Math.Abs(curretLicense.InstalledVersionDateTime.Ticks - InstalledDateTime.Ticks) > 100000)
							return ValidationStatus.LICENSE_CHANGED;
					}
					extendedLicense = new LicenseExtended(curretLicense);
					if (saveToDatabase)
					{
						curretLicense.InstalledVersionDateTime = InstalledDateTime;
						curretLicense.ActivationKey = extendedLicense.ActivationKey;
						curretLicense.Save();
						uow.CommitChanges();
					}

					//curretLicense.SerialNumber.StartDate <= DateTime.Now &&
					if ( curretLicense.SerialNumber.FinalDate >= DateTime.Now)
						return ValidationStatus.LICENSE_VALID;
					return ValidationStatus.LICENSE_VALID_UPDATES_EXPIRED;
				}
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
			//TODO use activation key
			LicenseExtended extLicense;
			ValidationStatus retvalue = this.GetExtendedLicenseObject(ApplicationID, serialNumber, MachineID, ApplicationBuild, false, out extLicense);
			return retvalue;
		}

		[WebMethod]
		public int NumberOfUsersLicence(Guid ApplicationID, String serialNumber, String MachineID, String activationKey, DateTime ApplicationBuild, DateTime beginDate, DateTime endDate)
		{
			/* LicenseExtended extLicense;
			 ValidationStatus retvalue = this.GetExtendedLicenseObject(ApplicationID, serialNumber, MachineID, ApplicationBuild, false, out extLicense);
			 if (retvalue == ValidationStatus.LICENSE_VALID && (extLicense.License.SerialNumber.StartDate.Ticks - beginDate.Ticks > 1000 || extLicense.License.SerialNumber.FinalDate.Ticks - endDate.Ticks > 1000))
				 return ValidationStatus.LICENSE_CHANGED;
			 return retvalue;*/
			return 0;
		}

		[WebMethod]
		public ValidationStatus ActivateLicense(Guid ApplicationID, String serialNumber, String MachineID, DateTime ApplicationBuild, out DateTime beginDate, out DateTime endDate, out String ActivationKey)
		{
			LicenseExtended extLicense;
			ValidationStatus retvalue = this.GetExtendedLicenseObject(ApplicationID, serialNumber, MachineID, ApplicationBuild, true, out extLicense);

			if (retvalue == ValidationStatus.LICENSE_VALID || retvalue == ValidationStatus.LICENSE_VALID_UPDATES_EXPIRED)
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
            ConfigurationHelper.ReadConfiguration(Server.MapPath(MvcApplication.CONFIG_FILE));
        }

		protected string EncryptString(string input,string hash)
		{
			String output = "";

			int hash_length = (hash.Length) / 2, i;
			byte[] hash_bytes = new byte[hash_length];
			for (i = 0; i < hash_length; i++)
			{
				hash_bytes[i] = Convert.ToByte(hash.Substring(2 * i, 2), 16);
			}
			byte[] input_bytes = Encoding.UTF8.GetBytes(input);
			int input_length = input_bytes.Length;
			byte[] step1_bytes = new byte[input_length];
			for (i = 0; i < input_length; i++)
			{
				step1_bytes[i] = (byte)(input_bytes[i] ^ hash_bytes[i%hash_length]);
			}
			byte[] step2_bytes = SevenZipHelper.Compress(step1_bytes);
			byte[] step3_bytes = new byte[step2_bytes.Length];
			for (i = 0; i < step2_bytes.Length; i++)
			{
				step3_bytes[i] = (byte)(step2_bytes[i] ^ hash_bytes[i % hash_length]);
			}
			output = BitConverter.ToString(step3_bytes).Replace("-", "");
			return output;
		}

		protected string DecryptString(string input, string hash)
		{
			String output = "";

			int hash_length = (hash.Length) / 2, i;
			byte[] hash_bytes = new byte[hash_length];
			for (i = 0; i < hash_length; i++)
			{
				hash_bytes[i] = Convert.ToByte(hash.Substring(2 * i, 2), 16);
			}
			int step3_length = (input.Length) / 2;
			byte[] step3_bytes = new byte[step3_length];
			for (i = 0; i < step3_length; i++)
			{
				step3_bytes[i] = Convert.ToByte(input.Substring(2 * i, 2), 16);
			}
			byte[] step2_bytes = new byte[step3_length];
			for (i = 0; i < step2_bytes.Length; i++)
			{
				step2_bytes[i] = (byte)(step3_bytes[i] ^ hash_bytes[i % hash_length]);
			}
			byte[] step1_bytes = SevenZipHelper.Decompress(step2_bytes);

			byte[] input_bytes = new byte[step1_bytes.Length];
			for (i = 0; i < step1_bytes.Length; i++)
			{
				input_bytes[i] = (byte)(step1_bytes[i] ^ hash_bytes[i % hash_length]);
			}
			return Encoding.UTF8.GetString(input_bytes);
		}

		[WebMethod]
		public string GetSettingsXml(Guid appID, string serialnumber, string MachineID, string activationkey, DateTime applBuild, DateTime beginDate, DateTime endDate, out bool success)
		{            
			try
			{
				ValidationStatus currentStatus = CheckValidLicence(appID, serialnumber, MachineID, activationkey, applBuild, beginDate, endDate);
				if (currentStatus == ValidationStatus.LICENSE_VALID || currentStatus == ValidationStatus.LICENSE_VALID_UPDATES_EXPIRED)
				{
                    using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
					{
						SerialNumber sn = uow.FindObject<SerialNumber>(new BinaryOperator("Number", serialnumber));

						XElement xmlDoc = new XElement("settings");

						foreach(ValidationRule rule in sn.ValidationRules){
							XElement validationTag = 
								new XElement("validation",
								new XElement("class", rule.Rule.Entity),
								new XElement("condition",
									new XElement("field", rule.Rule.Field),
									new XElement("operator", rule.Rule.Operator),
									new XElement("value", rule.Rule.Value)
								),
								new XElement("limit", rule.limit)
							);
							xmlDoc.Add(validationTag);
						}

						XElement user_settings = new XElement("users");
						foreach (UserRule rule in sn.UserRules)
						{
							XElement user_settings_tag =
								new XElement("userrule",
									new XElement("usertype", rule.UserType),
									new XElement("limit", rule.Limit)
								);
							user_settings.Add(user_settings_tag);
						}
						xmlDoc.Add(user_settings);

						string encrypt = EncryptString(xmlDoc.ToString(), activationkey);
						success = true;
						return encrypt;
					}
				}
				else
				{
					success = false;
					return null;
				}
			}
			catch (Exception ex)
			{
				success = false;
				return null;
			}
		}

		[WebMethod]
		public bool PostApplicationData(byte[] ZipedXml)
		{
			try
			{
                using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
				{
					XDocument xml = XDocument.Parse(Encoding.UTF8.GetString(SevenZipHelper.Decompress(ZipedXml)));
					SerialNumber sn = uow.FindObject<SerialNumber>(new BinaryOperator("Number", xml.Element("applicationusers").Element("sn").Value.Trim()));
					IEnumerable<XElement> applicationUserNodes = xml.Element("applicationusers").Elements("user");
					foreach(XElement node in applicationUserNodes){
						ApplicationUser applicationUser = uow.FindObject<ApplicationUser>(new BinaryOperator("Key",node.Element("key").Value.Trim()));
						if (applicationUser == null)
						{
							applicationUser = new ApplicationUser(uow);
						}
						applicationUser.Name = node.Element("name").Value.Trim();
						applicationUser.Key = node.Element("key").Value.Trim();
						applicationUser.UserApplicationOid = Guid.Parse(node.Element("guid").Value.Trim());
						applicationUser.ApplicationUsersSerialNumber = sn;
						applicationUser.Type = node.Element("type").Value.Trim();
						applicationUser.Version = long.Parse( node.Element("version").Value.Trim() );
						applicationUser.Save();
					}
					sn.Version = long.Parse(xml.Element("applicationusers").Element("version").Value.Trim()); ;
					sn.Save();
					uow.CommitTransaction();
				}
			}
			catch (Exception ex)
			{
				return false;
			}
			return true;
		}

		[WebMethod]
		public long GetVersion(string serialNumber)//,string entity = "User")
		{
			try
			{
                using (UnitOfWork uow = LicenseConnectionHelper.GetNewUnitOfWork())
				{
					SerialNumber sn = uow.FindObject<SerialNumber>(new BinaryOperator("Number", serialNumber));

					if(sn == null){
						throw new Exception();
					}
					else
					{

						return sn.Version;
					}
				}
			}
			catch (Exception ex)
			{
				return DateTime.MinValue.Ticks;
			}
		}
	}
}
