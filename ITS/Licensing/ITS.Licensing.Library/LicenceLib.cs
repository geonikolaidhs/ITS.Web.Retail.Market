using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Management;
using Microsoft.Win32;
using System.Collections;
using ITS.Licensing.ClientLibrary;

namespace ITS.Licensing.Library
{
    public static class LicenseLib
    {

        public static String GenerateHash(byte[] machineApplicationUniqueKey, DateTime startDate, DateTime endDate)
        {
            //int dwKeySize = 384;
            byte[] sdate = (BitConverter.GetBytes(startDate.ToFileTimeUtc()));
            byte[] edate = (BitConverter.GetBytes(endDate.ToFileTimeUtc()));
            int i;
            /*/byte[] machineApplicationUniqueKey// = new byte[20];
            {
                int length = MachineApplicationUniqueKey.Length;
                for (i = 0; i < length; i++)
                    machineApplicationUniqueKey[i] = Convert.ToByte(MachineApplicationUniqueKey.Substring(2 * i, 2), 16);
            }*/
            //GenerateMachineApplicationUniqueKey(ApplicationID, ApplicationKey, DeviceID, paddingCharacter);
            byte[] crc = ClientLicense.CalculateCRC(machineApplicationUniqueKey);

            byte[] finalbytes = new byte[20];
            
            for (i = 0; i < 8; i++)
            {
                finalbytes[i] = (byte)(machineApplicationUniqueKey[i] ^ sdate[i]);
            }

            for (; i < 16; i++)
            {
                finalbytes[i] = (byte)(machineApplicationUniqueKey[i] ^ edate[i - 8]);
            }


            for (; i < 20; i++)
            {
                finalbytes[i] = (byte)(machineApplicationUniqueKey[i] ^ crc[i - 16]);
            }


            String returnCode = System.BitConverter.ToString(finalbytes).Replace("-", "");
            return returnCode;
        }

        /*
        public static bool ActivateProduct(Guid ApplicationID, String ApplicationKey, out String message)
        {
            return false;
        }   
        */

        private static String GetStringOfNumber(int n)
        {
            String toReturn ="";
            while (n>0)
            {
                int r = n % 36;
                toReturn = GetCharOfNumber(r)+toReturn;
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

        public static String GenerateKey(Guid ApplicationID)
        {
            int n1, n2, n3, n4, n5, application;
            Random rnd = new Random();
            n1 = rnd.Next(1679616, int.MaxValue - 221) % 1679616;
            n1 += -(n1 % 221) +27;

            n2 = rnd.Next(1679616, int.MaxValue - 221) % 1679616;
            n2 += -(n2 % 27) + 21;

            n3 = rnd.Next(1679616, int.MaxValue - 221) % 1679616;
            n3 += -(n3 % 57) + 32;

            application = 0;
            foreach (byte b in ApplicationID.ToByteArray())
                application += b;

            n4 = rnd.Next(1679616, int.MaxValue - application) % 1679616;
            n4 -= n4 % application;

            n5 = (n1 + n2 + n3 + n4) % 1679616;

            return GetStringOfNumber(n1) + GetStringOfNumber(n2) + GetStringOfNumber(n3) + GetStringOfNumber(n4) + GetStringOfNumber(n5);
        }
    }
}
