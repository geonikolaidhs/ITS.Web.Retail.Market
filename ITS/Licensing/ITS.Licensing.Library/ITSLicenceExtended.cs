using System;
using System.Collections.Generic;
using System.Text;
using ITS.Licensing.ClientLibrary;
using ITS.Licensing.LicenseModel;


namespace ITS.Licensing.Library
{
    public class LicenseExtended :  ITSLicense
    {
        private License currentLicense;
        public License License
        {
            get
            {
                return currentLicense;
            }            
        }
        public LicenseExtended()
            : base()
        {
            
        }

        public LicenseExtended(License license): base()
        {
            SetLicenseDetails(license);
        }

        byte[] MachineApplicationUniqueKey;
        public void SetLicenseDetails(License license)
        {
            this.ApplicationID = license.SerialNumber.Application.ApplicationOid;
            this.ApplicationDateTime = license.InstalledVersionDateTime;
            this.SerialNumber = license.SerialNumber.Number;
            this.MachineID = license.MachineID;
            currentLicense = license;
            MachineApplicationUniqueKey = GetMachineApplicationUniqueKey();
            license.MachineApplicationUniqueKey = BitConverter.ToString(MachineApplicationUniqueKey).Replace("-", "");
            GenerateActivationKey();
        }



        public void GenerateActivationKey()
        {
            //int dwKeySize = 384;
            byte[] sdate = (BitConverter.GetBytes(currentLicense.SerialNumber.StartDate.ToFileTimeUtc()));
            byte[] edate = (BitConverter.GetBytes(currentLicense.SerialNumber.FinalDate.ToFileTimeUtc()));
            int i;
  
            byte[] crc = ClientLicense.CalculateCRC(MachineApplicationUniqueKey);

            byte[] finalbytes = new byte[20];

            for (i = 0; i < 8; i++)
            {
                finalbytes[i] = (byte)(MachineApplicationUniqueKey[i] ^ sdate[i]);
            }

            for (; i < 16; i++)
            {
                finalbytes[i] = (byte)(MachineApplicationUniqueKey[i] ^ edate[i - 8]);
            }


            for (; i < 20; i++)
            {
                finalbytes[i] = (byte)(MachineApplicationUniqueKey[i] ^ crc[i - 16]);
            }


            String returnCode = System.BitConverter.ToString(finalbytes).Replace("-", "");
            this.ActivationKey = returnCode;

        }
    }
}
