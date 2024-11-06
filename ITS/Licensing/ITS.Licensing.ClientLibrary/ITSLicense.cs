using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Reflection;
using System.Security.Cryptography;
using System.Runtime.Serialization;
using Microsoft.Win32;

namespace ITS.Licensing.ClientLibrary
{
    public class ITSLicense
    {
        public enum LicenseStatusType
        {
            UNDEFINED,
            LICENSE_VALID,
            LICENSE_VERSION_INVALID,
            LICENSE_VALID_UPDATES_EXPIRED,
            LICENSE_INVALID,
            LICENSE_CHANGED,
            LICENSE_MAXIMUM_REACHED
        }

        public enum UserAccessType { None,Full,Web,Mobile,Smartphone };
        public static int[] RollAccess(UserAccessType userAccessType)
        {
            int[] roll = new int[] { 0, 1, 2, 3 };
            switch (userAccessType)
            {
                case UserAccessType.None:
                    roll = new int[] { 0, 0, 0, 0 };
                    break;
                case UserAccessType.Full:
                    roll = new int[] { 0, 1, 2, 3 };
                    break;
                case UserAccessType.Web:
                    roll = new int[] { 1, 2, 3, 0 };
                    break;
                case UserAccessType.Mobile:
                    roll = new int[] { 2, 3, 0, 1 };
                    break;
                case UserAccessType.Smartphone:
                    roll = new int[] { 3, 0, 1, 2 };
                    break;
            }
            return roll;
        }

        protected const char paddingCharacter = 'F';
        protected Guid applicationID;

        public Guid ApplicationID
        {
            get { return applicationID; }
            protected set { applicationID = value; }
        }
        protected Guid applicationVersionID;

        public Guid ApplicationVersionID
        {
            get { return applicationVersionID; }
            protected set { applicationVersionID = value; }
        }
        protected String applicationName;

        public String ApplicationName
        {
            get { return applicationName; }
            protected set { applicationName = value; }
        }
        protected String applicationVersion;

        public String ApplicationVersion
        {
            get { return applicationVersion; }
            protected set { applicationVersion = value; }
        }
        protected String serialNumber;

        public String SerialNumber
        {
            get { return serialNumber; }
            protected set { serialNumber = value; }
        }


        protected String machineID;

        public String MachineID
        {
            get { return machineID; }
            protected set { machineID = value; }
        }


        protected String activationKey;

        public String ActivationKey
        {
            get { return activationKey; }
            set { activationKey = value; }
        }

        protected DateTime updateLicenseStartDate;

        public DateTime UpdateLicenseStartDate
        {
            get
            {
                return updateLicenseStartDate;
            }
            protected set
            {
                if (updateLicenseStartDate == value)
                    return;
                updateLicenseStartDate = value;
            }
        }

        protected DateTime updateLicenseEndDate;
        public DateTime UpdateLicenseEndDate
        {
            get
            {
                return updateLicenseEndDate;
            }
            protected set
            {
                if (updateLicenseEndDate == value)
                    return;
                updateLicenseEndDate = value;
            }
        }

        protected LicenseStatusType licenseStatus;
        public LicenseStatusType LicenseStatus
        {
            get
            {
                return licenseStatus;
            }
            protected set
            {
                if (licenseStatus == value)
                    return;
                licenseStatus = value;
            }
        }


        protected DateTime applicationDateTime;
        public DateTime ApplicationDateTime
        {
            get
            {
                return applicationDateTime;
            }
            protected set
            {
                if (applicationDateTime == value)
                    return;
                applicationDateTime = value;
            }
        }

        protected String storedApplicationActivationKey;
        public String StoredApplicationActivationKey
        {
            get
            {
                return storedApplicationActivationKey;
            }
            protected set
            {
                if (storedApplicationActivationKey == value)
                    return;
                storedApplicationActivationKey = value;
            }
        }


        protected byte[] GetMachineApplicationUniqueKey()
        {
            int length = ApplicationID.ToString().Length > MachineID.Length ? ApplicationID.ToString().Length : MachineID.Length;
            if (length < SerialNumber.Length) length = SerialNumber.Length;
            String MyApplicationID = ApplicationID.ToString().PadLeft(length, paddingCharacter);
            String MyMachineID = MachineID.PadRight(length, paddingCharacter);
            String MyApplicationKey = SerialNumber.PadLeft(length, paddingCharacter);

            String machineAppCode = "";
            for (int i = 0, j = 1; i < length; i += j)
            {
                machineAppCode += MyApplicationID.Substring(i, j);
                machineAppCode += MyMachineID.Substring(i, j);
                machineAppCode += MyApplicationKey.Substring(i, j);
            }
            using (SHA1CryptoServiceProvider sha = new SHA1CryptoServiceProvider())
            {
                byte[] buffer = Encoding.UTF8.GetBytes(machineAppCode);
                byte[] encoded = sha.ComputeHash(buffer);
                return encoded;
            }

        }

        protected bool GetLicenseDates(String hash)
        {
            try
            {
                int length = (hash.Length) / 2, i;
                byte[] finalbytes = new byte[length];
                for (i = 0; i < length; i++)
                    finalbytes[i] = Convert.ToByte(hash.Substring(2 * i, 2), 16);
                byte[] mauq = GetMachineApplicationUniqueKey();
                byte[] sdate = new byte[8];
                byte[] edate = new byte[8];
                byte[] crc = new byte[4];

                for (i = 0; i < 8; i++)
                {
                    sdate[i] = (byte)(mauq[i] ^ finalbytes[i]);
                }

                for (; i < 16; i++)
                {
                    edate[i - 8] = (byte)(mauq[i] ^ finalbytes[i]);
                }


                for (; i < 20; i++)
                {
                    crc[i - 16] = (byte)(mauq[i] ^ finalbytes[i]);
                }

                byte[] crc2 = ClientLicense.CalculateCRC(mauq);
                bool valid = true;
                for (i = 0; i < 4; i++)
                    if (crc2[i] != crc[i])
                    {
                        valid = false;
                        break;
                    }
                if (valid)
                {
                    UpdateLicenseStartDate = DateTime.FromFileTime(BitConverter.ToInt64(sdate, 0));
                    UpdateLicenseEndDate = DateTime.FromFileTime(BitConverter.ToInt64(edate, 0));
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        protected void GenerateMachineID()
        {
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
            mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {

                id += mo["ProcessorID"].ToString();
            }
            id += "-";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mos.Get();
            foreach (ManagementObject mo in moc)
            {
                id += (string)mo["SerialNumber"];
            }
            MachineID = id;
        }

        protected virtual void DetermineLicenced(bool localmachine = true)
        {
            String whereInRegistry = "SOFTWARE\\I.T.S. S.A.\\" + ApplicationName;
            RegistryKey toLook = localmachine ? Registry.LocalMachine : Registry.CurrentUser;
            using (RegistryKey rkey = toLook.OpenSubKey(whereInRegistry))
            {
                if (rkey == null)
                {
                    licenseStatus = LicenseStatusType.UNDEFINED;
                    return;
                }
                ActivationKey = StoredApplicationActivationKey = rkey.GetValue("ActivationKey", "").ToString();//TOCHECK
                SerialNumber = rkey.GetValue("SerialNumber", "").ToString();
                if (SerialNumber != "" && StoredApplicationActivationKey != "")
                {
                   // DateTime from, to;
                    bool chk = GetLicenseDates(StoredApplicationActivationKey);
                    if (chk)
                    {
                        licenseStatus = (DateTime.Now <= updateLicenseEndDate ) ? LicenseStatusType.LICENSE_VALID : LicenseStatusType.LICENSE_VALID_UPDATES_EXPIRED;
                        //&& DateTime.Now >= updateLicenseStartDate

                       
                        DateTime assemblyTime = ApplicationDateTime;
                        if (!(assemblyTime <= updateLicenseEndDate && assemblyTime >= updateLicenseStartDate))
                        {
                            licenseStatus = LicenseStatusType.LICENSE_VERSION_INVALID;
                            return;
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
            
            //Check with web
            LicenseWebService.LicenceWebService webservice = new LicenseWebService.LicenceWebService();      
            webservice.Url = Configuration.webServiceUrl;
            
            try
            {
                LicenseWebService.ValidationStatus validation = webservice.CheckValidLicence(ApplicationID, SerialNumber, MachineID, StoredApplicationActivationKey, ApplicationDateTime, UpdateLicenseStartDate, UpdateLicenseStartDate);
                    //webservice.CheckValidLicence(ApplicationID.ToString(), SerialNumber, MachineID, StoredApplicationActivationKey);

                switch (validation)
                {
                    case LicenseWebService.ValidationStatus.LICENSE_CHANGED:
                        licenseStatus = LicenseStatusType.LICENSE_CHANGED;
                        return;
                    case LicenseWebService.ValidationStatus.LICENSE_INVALID:
                        licenseStatus = LicenseStatusType.LICENSE_INVALID;
                        return;
                    case LicenseWebService.ValidationStatus.LICENSE_MAXIMUM_REACHED:
                        licenseStatus = LicenseStatusType.LICENSE_MAXIMUM_REACHED;
                        return;
                    case LicenseWebService.ValidationStatus.LICENSE_VALID:
                        licenseStatus = LicenseStatusType.LICENSE_VALID;
                        return;
                    case LicenseWebService.ValidationStatus.LICENSE_VALID_UPDATES_EXPIRED:
                        licenseStatus = LicenseStatusType.LICENSE_VALID_UPDATES_EXPIRED;
                        return;
                    case LicenseWebService.ValidationStatus.LICENSE_VERSION_INVALID:
                        licenseStatus = LicenseStatusType.LICENSE_VERSION_INVALID;
                        return;
                }

            }
            catch (Exception )
            {

            }
            
            return;
        }

        public ITSLicense(DateTime application)
        {
            applicationDateTime = application;
            GenerateMachineID();
            ApplicationID = System.Runtime.InteropServices.Marshal.GetTypeLibGuidForAssembly(Assembly.GetEntryAssembly());
            ApplicationName = Assembly.GetEntryAssembly().GetName().Name;
            DetermineLicenced();
            if (!ValidateSerialNumber(SerialNumber))
            {
                SerialNumberForm frm = new SerialNumberForm();
                frm.itslic = this;
                frm.ShowDialog();
            }
        }

        protected ITSLicense()
        {
        }

        #region Duplicate Code!!!
        private static String GetStringOfNumber(int n)
        {
            String toReturn = "";
            while (n > 0)
            {
                int r = n % 36;
                toReturn = GetCharOfNumber(r) + toReturn;
                n -= r;
                n /= 36;
            }
            return toReturn.PadLeft(4, '0');
        }        
        private static char GetCharOfNumber(int i)
        {
            if (i >= 36)
                throw new Exception("non expected call");
            char c = '0';
            if (i >= 10)
            {
                c = 'A';
                c -= (char)10;
            }
            c += (char)i;
            return c;
        }
        #endregion

        public static String GenerateUserKey(String SerialNumberID, ITS.Licensing.ClientLibrary.ITSLicense.UserAccessType userAccessType)
        {
            const int int_limit = 1679616;
            int n1, n2, n3, n4, n5;
            int[] application = new int[] { 0, 0, 0, 0 };
            for (int i = 0; i < 4; i++)
            {
                foreach (byte b in Encoding.UTF8.GetBytes(SerialNumberID.Substring(i * 5, 5)))
                    application[i] += b;
            }
            Random rnd = new Random();

            int[] roll = ITS.Licensing.ClientLibrary.ITSLicense.RollAccess(userAccessType);

            n1 = rnd.Next(int_limit, int.MaxValue) % int_limit;
            n1 -= n1 % application[roll[0]];

            n2 = rnd.Next(int_limit, int.MaxValue) % int_limit;
            n2 -= n2 % application[roll[1]];

            n3 = rnd.Next(int_limit, int.MaxValue) % int_limit;
            n3 -= n3 % application[roll[2]];

            n4 = rnd.Next(int_limit, int.MaxValue) % int_limit;
            n4 -= n4 % application[roll[3]];

            n5 = (n1 + n2 + n3 + n4) % int_limit;

            return GetStringOfNumber(n1) + GetStringOfNumber(n2) + GetStringOfNumber(n3) + GetStringOfNumber(n4) + GetStringOfNumber(n5);
        }

        protected bool ValidateSerialNumber(String newSerialNumber)
        {
            if (newSerialNumber==null ||newSerialNumber.Length != 20)
                return false;
            String part1 = newSerialNumber.Substring(0, 4).ToUpper();
            String part2 = newSerialNumber.Substring(4, 4).ToUpper();
            String part3 = newSerialNumber.Substring(8, 4).ToUpper();
            String part4 = newSerialNumber.Substring(12, 4).ToUpper();
            String part5 = newSerialNumber.Substring(16, 4).ToUpper();
            int n1 = GetNumberOfString(part1);
            int n2 = GetNumberOfString(part2);
            int n3 = GetNumberOfString(part3);
            int n4 = GetNumberOfString(part4);
            int n5 = GetNumberOfString(part5);

            if (n1 % 221 != 27)
                return false;
            if (n2 % 27 != 21)
                return false;
            if (n3 % 57 != 32)
                return false;
            int application = 0;
            foreach (byte b in ApplicationID.ToByteArray())
                application += b;
            if (n4 % application != 0)
                return false;
            if ((n1 + n2 + n3 + n4) % 1679616 != n5)
                return false;
            return true; ;
        }

        public static UserAccessType ConvertToUserAccessType(String accessType)
        {
            switch(accessType.ToUpper()){
                case "NONE":
                    return UserAccessType.None;
                case "FULL":
                    return UserAccessType.Full;
                case "WEB":
                    return UserAccessType.Web;
                case "MOBILE":
                    return UserAccessType.Mobile;
                case "SMARTPHONE":
                    return UserAccessType.Smartphone;
                default :
                    return UserAccessType.None;
            }
        }

        public UserAccessType GetUserType(String userKey)//,ITSLicense itslicense)
        {
            if(userKey==null){
                return UserAccessType.None;
            }
            foreach (UserAccessType access in Enum.GetValues(typeof(UserAccessType)))
            {
                if(ValidateUserKey(userKey,access)){
                    return access;
                }
            }
            return UserAccessType.None;
        }

        public bool UserHasAccess(String userKey, UserAccessType userAccess)
        {
            UserAccessType access = GetUserType(userKey);
            if(userAccess.Equals(UserAccessType.None) || access.Equals(UserAccessType.None) )
            {
                return false;
            }
            if( access.Equals(UserAccessType.Full) || access.Equals(userAccess)) 
            {
                return true;
            }
            return false;
        }

        protected bool ValidateUserKey(String newUserKey, UserAccessType userAccessType)
        {
            if(newUserKey == null || newUserKey == ""
            || SerialNumber == null || SerialNumber == ""
            )
            {
                return false;
            }

            int[] roll = RollAccess(userAccessType);

            if (newUserKey == null || newUserKey.Length != 20)
                return false;
            String part1 = newUserKey.Substring(0, 4).ToUpper();
            String part2 = newUserKey.Substring(4, 4).ToUpper();
            String part3 = newUserKey.Substring(8, 4).ToUpper();
            String part4 = newUserKey.Substring(12, 4).ToUpper();
            String part5 = newUserKey.Substring(16, 4).ToUpper();
            int n1 = GetNumberOfString(part1);
            int n2 = GetNumberOfString(part2);
            int n3 = GetNumberOfString(part3);
            int n4 = GetNumberOfString(part4);
            int n5 = GetNumberOfString(part5);
            int[] application = new int[]{0,0,0,0};
            for (int i = 0; i < 4; i++)
            {
                foreach (byte b in Encoding.UTF8.GetBytes(SerialNumber.Substring(i*5,5)))
                    application[i] += b;
            }

            if (n1 % application[roll[0]] != 0)
                return false;
            if (n2 % application[roll[1]] != 0)
                return false;
            if (n3 % application[roll[2]] != 0)
                return false;
            if (n4 % application[roll[3]] != 0)
                return false;
            if ((n1 + n2 + n3 + n4) % 1679616 != n5)
                return false;
            return true; ;
        }

        protected int GetNumberOfString(String s)
        {
            int toReturn = 0;
            foreach (char c in s)
            {
                toReturn *= 36;
                toReturn += GetNumberOfChar(c);
            }
            return toReturn;
        }

        protected int GetNumberOfChar(char c)
        {
            int toReturn;
            toReturn = c - '0';
            if (toReturn > 9)
                toReturn = 10 + c - 'A';
            return toReturn;
        }

        public bool SetSerialNumber(String newSerialNumber, bool localmachine = true)
        {
            try
            {
                if (!ValidateSerialNumber(newSerialNumber.ToUpper()))
                {
                    return false;
                }
                SerialNumber = newSerialNumber;
                String whereInRegistry = "SOFTWARE\\I.T.S. S.A.\\" + ApplicationName;
                if (localmachine)
                {
                    using (RegistryKey rkey = Registry.LocalMachine.CreateSubKey(whereInRegistry))
                    {
                        rkey.SetValue("SerialNumber", newSerialNumber);
                    }
                }
                else
                {
                    using (RegistryKey rkey = Registry.CurrentUser.CreateSubKey(whereInRegistry))
                    {
                        rkey.SetValue("SerialNumber", newSerialNumber);
                    }
                }
                DetermineLicenced(localmachine);
                return true;
            }
            catch (Exception )
            {
                return false;
            }
        }

        public string GetLicenseServerURL()
        {
            return Configuration.webServiceUrl;
        }
    }
}
