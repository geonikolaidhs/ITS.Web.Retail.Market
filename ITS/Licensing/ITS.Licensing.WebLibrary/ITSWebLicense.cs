using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Licensing.ClientLibrary;
using System.Reflection;
using Microsoft.Win32;
using System.IO;
using System.Xml.Linq;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Specialized;
using System.Xml;
using SevenZip.Compression.LZMA;

namespace ITS.Licensing.WebLibrary
{
    public class ITSWebLicense: ITSLicense 
    {
        private class Validation
        {
            String className;
            public String ClassName
            {
                get
                {
                    return className;
                }
                set
                {
                    if (className == value)
                        return;
                    className = value;
                }
            }


            String ruleField;
            public String RuleField
            {
                get
                {
                    return ruleField;
                }
                set
                {
                    if (ruleField == value)
                        return;
                    ruleField = value;
                }
            }

            String ruleOperator;
            public String RuleOperator
            {
                get
                {
                    return ruleOperator;
                }
                set
                {
                    if (ruleOperator == value)
                        return;
                    ruleOperator = value;
                }
            }

            String ruleValue;
            public String RuleValue
            {
                get
                {
                    return ruleValue;
                }
                set
                {
                    if (ruleValue == value)
                        return;
                    ruleValue = value;
                }
            }


            int count;
            public int Count
            {
                get
                {
                    return count;
                }
                set
                {
                    if (count == value)
                        return;
                    count = value;
                }
            }

            public String Condition()
            {
                string condition = null;
                if(this.RuleField!=null && this.RuleField!="" &&
                    this.RuleOperator!=null && this.RuleOperator!="" &&
                    this.RuleValue!=null && this.RuleValue!=""
                    ){
                    condition = this.RuleField + this.RuleOperator + this.RuleValue;
                }
                return condition;
            }

            public CriteriaOperator Criteria
            {
                get 
                {
                    //CriteriaOperator crop = new BinaryOperator(RuleField,RuleValue,BinaryOperatorType.BitwiseAnd);
                    return CriteriaOperator.Parse(this.Condition()); 
                }
            }

            public bool Status(UnitOfWork uow, Assembly asm)
            {
                if (this.Count < 0)//<0 =>απεριόριστους  χρήστες στον πίνακα των χρηστών
                {
                    return true;
                }
                IEnumerable<Type> list = asm.GetTypes().AsEnumerable<Type>().Where(g => g.Name == className);
                Type type = list.First();
                CriteriaOperator crop = CriteriaOperator.Parse(this.Condition());
                XPCollection collection = new XPCollection(uow, type, crop);
                if(collection == null){
                    return true;
                }
                return collection.Count <= this.Count;
            }
        }

        private class UserValidation
        {
            private ITSWebLicense itsWebLicense;

            internal UserValidation(ITSWebLicense WebLicense)
            {
                itsWebLicense = WebLicense;
            }
            
            //String className;
            public String ClassName
            {
                get
                {
                    return "User";
                }
            }

            UserAccessType userAccess;
            public UserAccessType UserAccess
            {
                get
                {
                    return userAccess;
                }
                set
                {
                    if (userAccess == value)
                        return;
                    userAccess = value;
                }
            }

            int count;
            public int Count
            {
                get
                {
                    return count;
                }
                set
                {
                    if (count == value)
                        return;
                    count = value;
                }
            }

            public CriteriaOperator Criteria
            {
                get
                {
                    CriteriaOperator crop = new NotOperator(new NullOperator("Key"));
                    return crop;//CriteriaOperator.Parse(this.Condition());
                }
            }

            public bool Status(UnitOfWork uow, Assembly asm)
            {
                if(this.Count<0)//<0 =>απεριόριστους  χρήστες τύπου this.userAccess
                {
                    return true;
                }

                IEnumerable<Type> list = asm.GetTypes().AsEnumerable<Type>().Where(g => g.Name == ClassName);//TODO...
                Type type = list.First();
                //CriteriaOperator crop = CriteriaOperator.Parse(this.Condition());
                XPCollection collection = new XPCollection(uow, type, Criteria);
                if (collection == null)
                {
                    return true;
                }

                int counter = 0;
                foreach (dynamic dyno in collection)
                {
                    if (itsWebLicense.GetUserType(dyno.Key) == UserAccess)
                    {
                        counter++;
                    }
                }
                collection.Dispose();
                return counter <= Count;
            }
        }


        List<Validation> validationRules;
        List<UserValidation> userValidationRules;

        private string DecodeString(string input,string hash)
        {
            //String output = "";

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

        private void	ReadXml()
        {   
            String encodedFile, decodedFile;
            String whereInRegistry = "SOFTWARE\\I.T.S. S.A.\\" + ApplicationName;
            RegistryKey toLook = Registry.CurrentUser;
            using (RegistryKey rkey = toLook.OpenSubKey(whereInRegistry))
            {
                encodedFile = rkey.GetValue("Vl", "").ToString();
                decodedFile = DecodeString(encodedFile, rkey.GetValue("ActivationKey", "").ToString());

                XDocument xml = XDocument.Parse(decodedFile);
                IEnumerable<XElement> validationNodes = xml.Element("settings").Elements("validation");

                IEnumerable<Validation> val = from node in validationNodes
                                              select new Validation
                                              {
                                                  ClassName = node.Element("class").Value.Trim(),
                                                  RuleField = node.Element("condition").Element("field").Value.Trim(),
                                                  RuleOperator = node.Element("condition").Element("operator").Value.Trim(),
                                                  RuleValue = node.Element("condition").Element("value").Value.Trim(),
                                                  Count = Int32.Parse(node.Element("limit").Value.Trim())
                                              };
                validationRules = val.ToList();

                IEnumerable<XElement> userValidationNodes = xml.Element("settings").Element("users").Elements("userrule");
                IEnumerable<UserValidation> user_val = from node in userValidationNodes
                                              select new UserValidation(this)
                                              {
                                                  UserAccess = ITS.Licensing.ClientLibrary.ITSLicense.ConvertToUserAccessType(node.Element("usertype").Value.Trim() ),
                                                  Count = Int32.Parse(node.Element("limit").Value.Trim())
                                              };
                userValidationRules = user_val.ToList();
            }
        }

        int numberOfTransactions;
        public int NumberOfTransactions
        {
            get
            {
                return numberOfTransactions;
            }
            set
            {
                if (numberOfTransactions == value)
                    return;
                numberOfTransactions = value;
            }
        }
        
        


        public ITSWebLicense(DateTime application, Type t)
        {
            applicationDateTime = application;
            GenerateMachineID();

            ApplicationID = System.Runtime.InteropServices.Marshal.GetTypeLibGuidForAssembly(Assembly.GetAssembly(t));
            ApplicationName = Assembly.GetAssembly(t).GetName().Name;
            DetermineLicenced(false);            
            //ReadXml("C:\\Works\\ITS.License\\TestMVCApp\\Settings.xml");
            //ReadXml();//"D:\\Visual Studio Projects\\ITSLicencing\\TestMVCApp\\Settings.xml");
        }

        protected override void DetermineLicenced(bool localmachine=true)
        {
            //base.DetermineLicenced();
            String whereInRegistry = "SOFTWARE\\I.T.S. S.A.\\" + ApplicationName;
            RegistryKey toLook = (localmachine) ? Registry.LocalMachine : Registry.CurrentUser;
            using (RegistryKey rkey = toLook.OpenSubKey(whereInRegistry))
            {
                if (rkey == null)
                {
                    licenseStatus = LicenseStatusType.UNDEFINED;
                    return;
                }
                ActivationKey = StoredApplicationActivationKey = rkey.GetValue("ActivationKey", "").ToString();
                SerialNumber = rkey.GetValue("SerialNumber", "").ToString();
                if (SerialNumber != "" && StoredApplicationActivationKey != "")
                {
                    //DateTime from, to;
                    bool chk = GetLicenseDates(StoredApplicationActivationKey);
                    if (chk)
                    {
                        licenseStatus = (DateTime.Now <= updateLicenseEndDate && DateTime.Now >= updateLicenseStartDate) ? LicenseStatusType.LICENSE_VALID : LicenseStatusType.LICENSE_VALID_UPDATES_EXPIRED;


                        DateTime assemblyTime = ApplicationDateTime;
                        if (!(assemblyTime <= updateLicenseEndDate && assemblyTime >= updateLicenseStartDate))
                        {
                            licenseStatus = LicenseStatusType.LICENSE_VERSION_INVALID;
                            return;
                        }
                        else
                        {
                            ReadXml();
                        }


                    }
                    else
                    {
                        licenseStatus = LicenseStatusType.LICENSE_INVALID;
                        return;
                    }
                }
                else
                {
                    licenseStatus = LicenseStatusType.UNDEFINED;
                    return;
                }
            }
            return;
        }



        public bool IsValid(Type type, UnitOfWork Session, object obj)
        {
            //throw new NotImplementedException();
            IEnumerable<Validation> effective = validationRules.Where(g => g.ClassName == type.Name);
            if (effective.Count() == 0)
                return true;
            foreach (Validation rule in effective)
            {
                CriteriaOperator crop = CriteriaOperator.Parse(rule.Condition());
                XPCollection ct = new XPCollection(Session, type, false);
                ct.Add(obj);
                ct.Filter = crop;
                if (ct.Count == 0)
                    continue;
                
                XPCollection f = new XPCollection(Session, type, crop);
                if (f.Count >= rule.Count)
                {
                    f.Dispose();
                    return false;
                }
                f.Dispose();
            }
            return true;

        }

        bool ValidSerialNumber, ValidActivation, ValidRules,ValidUserRules, ValidOnline;

        public bool CheckLicense(UnitOfWork uow, Assembly asm)
        {
 
#region Check Activation //TODO
            DetermineLicenced(false);
            ValidActivation = this.LicenseStatus == LicenseStatusType.LICENSE_VALID || this.LicenseStatus == LicenseStatusType.LICENSE_VALID_UPDATES_EXPIRED;
#endregion
#region    Check Serial //TODO
            ValidSerialNumber = ValidateSerialNumber(this.SerialNumber);
#endregion
#region     Rules
            ValidRules = true;
            if (validationRules == null)
            {
                ValidRules = false;
            }
            else
            {
                foreach (Validation rule in validationRules)
                {
                    if (!rule.Status(uow, asm))
                    {
                        ValidRules = false;
                        break;
                    }
                }
            }
#endregion

#region      User Rules
            ValidUserRules = true;
            if (userValidationRules == null)
            {
                ValidUserRules = false;
            }
            else
            {
                foreach (UserValidation userRule in userValidationRules)
                {
                    if (!userRule.Status(uow, asm))
                    {
                        ValidUserRules = false;
                        break;
                    }
                }
            }
#endregion

#region     Update lic server
#endregion

#region            Check Online //TODO
            ValidOnline = true;
#endregion
            return ValidSerialNumber && ValidActivation && ValidRules && ValidUserRules && ValidOnline;
        }

        public bool HandleRegistration(NameValueCollection tmp, IDictionary<string, object> ViewData)
        {
            if (tmp["step"] == null)
            {
                String sn = tmp["sn1"] + tmp["sn2"] + tmp["sn3"] + tmp["sn4"];
                if (!SetSerialNumber(sn, false))
                {
                    ViewData["Message"] = "Invalid serial number";                    
                }                
            }
            else if (tmp["step"] == "activate")
            {

                if (SetActivationKey())
                {
                    return true;
                }

            }
            return false;
        }

        private bool SetActivationKey()
        {
            try{
                LicenseWebService.LicenceWebService webservice = new LicenseWebService.LicenceWebService();
                webservice.Url = Configuration.webServiceUrl;

                webservice.Timeout = 10000;
                webservice.CheckOnlineStatus();

                String ActKey;
                DateTime startDate, endDate;                
                LicenseWebService.ValidationStatus status = webservice.ActivateLicense(ApplicationID,SerialNumber, MachineID, ApplicationDateTime, out startDate, out endDate,out ActKey );
                this.ActivationKey = ActKey;
                switch (status)
                {
                    case LicenseWebService.ValidationStatus.LICENSE_INVALID:
                        //("Η συγκεκριμένη άδεια δεν υπάρχει. Αν έχετε νόμιμο αντίγραφο, ελέγξτε αν έχετε εισάγει σωστά το κλειδί της εφαρμογής. Για βοήθεια, παρακαλώ επικοινωνήστε με την ITS Α.Ε.");
                        break;
                    case LicenseWebService.ValidationStatus.LICENSE_MAXIMUM_REACHED:
                        //("Ο συγκεκριμένος σειριακός αριθμός έχει ενεργοποιηθεί για όλες τις συσκευές που ήταν δυνατό. Παρακαλώ επικοινωνήστε με την ITS Α.Ε.");
                        break;
                    case LicenseWebService.ValidationStatus.LICENSE_VERSION_INVALID:
                        //("H άδεια για το συγκεκριμένο λογισμικό δεν καλύπτει την συγκεκριμένη έκδοση. Παρακαλώ επικοινωνήστε με την ITS Α.Ε.");                        
                        break;
                    case LicenseWebService.ValidationStatus.LICENSE_VALID_UPDATES_EXPIRED:
                        /*
                        ("Το συγκεκριμένο αντίγραφο καλύπτεται από την άδεια που έχετε αγοράσει." +
                            Environment.NewLine +  "H άδεια αναβαθμίσεων για το συγκεκριμένο λογισμικό έχει λήξει. Αν επιθυμείτε ανανέωση της συγκεκριμένης υπηρεσίας, παρακαλώ επικοινωνήστε με την ITS Α.Ε.");
                         */
                        goto case LicenseWebService.ValidationStatus.LICENSE_VALID;
                    case LicenseWebService.ValidationStatus.LICENSE_VALID:
                        String whereInRegistry = "SOFTWARE\\I.T.S. S.A.\\" + ApplicationName;
                        RegistryKey regKey = Registry.CurrentUser.OpenSubKey(whereInRegistry, true);
                        regKey.SetValue("ActivationKey", ActivationKey);
                        regKey.Close();
                        bool success;
                        /*
                        string xmldata = webservice.GetSettingsXml(this.ApplicationID, this.SerialNumber, this.MachineID, this.ActivationKey, this.ApplicationDateTime, this.UpdateLicenseStartDate, this.UpdateLicenseEndDate, out success);
                        regKey = Registry.CurrentUser.OpenSubKey(whereInRegistry, true);
                        regKey.SetValue("Vl", xmldata);
                        regKey.Close();
                        ReadXml();
                        */
                        success = SaveSettingsXml();

                        return success;
                        //("H ενεργοποίηση του προϊόντος ολοκληρώθηκε με επιτυχία.");
                        //break;
                    default:
                        throw new Exception();
                }

                return true;
            }
            catch (Exception ){
                return false;
            }
        }

        public bool SaveSettingsXml(){
            LicenseWebService.LicenceWebService webservice = new LicenseWebService.LicenceWebService();
            webservice.Url = Configuration.webServiceUrl;

            webservice.Timeout = 10000;
            webservice.CheckOnlineStatus();


            String whereInRegistry = "SOFTWARE\\I.T.S. S.A.\\" + ApplicationName;
            RegistryKey regKey = Registry.CurrentUser.OpenSubKey(whereInRegistry, true);
            bool success;

            string xmldata = webservice.GetSettingsXml(this.ApplicationID, this.SerialNumber, this.MachineID, this.ActivationKey, this.ApplicationDateTime, this.UpdateLicenseStartDate, this.UpdateLicenseEndDate, out success);
            if (!success || xmldata == null || xmldata == "")
            {
                return false;
            }
            regKey = Registry.CurrentUser.OpenSubKey(whereInRegistry, true);
            regKey.SetValue("Vl", xmldata);
            regKey.Close();
            ReadXml();
            return success;
        }

        private void WriteSerialNumberForm(TextWriter stream)
        {
            stream.WriteLine(@"<form method='post'>
                Insert your serial number:
                <input type='text' name='sn1' id ='sn1' size='5' maxlength='5' />-<input type='text' name='sn2' id ='sn2' size='5' maxlength='5' />-<input type='text' name='sn3' id ='sn3' size='5' maxlength='5' />-<input type='text' name='sn4' id ='sn4' size='5' maxlength='5'  />
                <input type='submit' text='Save Serial Number and Perform Activation' id = 'submit' name='submit' />
            </form>");
        }

        private void WriteLicenseInvalid(TextWriter stream)
        {
            stream.WriteLine("<div id='licenseNotice'>Your license is invalid. If you think this message is erroneous, please contact ITS to resolve this issue.</div>");
        }


        public void GenerateRegistrationOutput(TextWriter stream, IDictionary<string, object> ViewData)
        {
            stream.WriteLine("<h2>License page</h2>");
            stream.WriteLine("<p style='font-size:small'>This page is accessible only from the localhost</p>");
            if (ViewData.Count > 0)
            {
                foreach (KeyValuePair<string, object> element in ViewData)
                {
                    stream.WriteLine("<div id='" + element.Key + "' >" + element.Key + ": " + element.Value.ToString() + "</div>");
                }
            }
            switch (this.LicenseStatus)
            {
                case LicenseStatusType.LICENSE_VALID:
                    WriteValid(stream);
                    break;
                case LicenseStatusType.LICENSE_VALID_UPDATES_EXPIRED:
                    WriteExpirationNotice(stream);
                    WriteValid(stream);
                    break;
                case LicenseStatusType.UNDEFINED:
                    if (!this.ValidateSerialNumber(SerialNumber))
                    {
                        WriteSerialNumberForm(stream);
                    }
                    else
                    {
                        WriteActivationForm(stream);
                    }
                    break;
                case LicenseStatusType.LICENSE_CHANGED:
                    WriteLicenseAlteration(stream);
                    WriteActivationForm(stream);
                    break;
                default:
                    WriteLicenseInvalid(stream);
                    break;
            }
            
        }

        private void WriteLicenseAlteration(TextWriter stream)
        {
            stream.WriteLine("<div id='licenseNotice'>Your license has been altered. A recativation is required.</div>");
        }

        private void WriteActivationForm(TextWriter stream)
        {
            stream.WriteLine("<div id='licenseNotice'>Before continuing please be sure that your server can connect to ITS Licensing server<form method='post'><input type='hidden' name='step' value='activate'><input type='submit' text='PerformActivation'></input></form></div>");
        }

        private void WriteExpirationNotice(TextWriter stream)
        {
            stream.WriteLine("<div id='licenseNotice'>Your license for updates for this software will expire soon. Please consider updating</div>");
        }

        private void WriteValid(TextWriter stream)
        {
            stream.WriteLine("<div id='licenseNotice'>Your license has been validated. If this message insists, please perform an iisreset</div>");
        }


    }
}
