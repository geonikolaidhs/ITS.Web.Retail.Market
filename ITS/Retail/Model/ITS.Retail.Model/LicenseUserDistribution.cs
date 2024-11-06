using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Xml.Serialization;

namespace ITS.Retail.Model
{
    [Updater(Order = 218, Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class LicenseUserDistribution : BaseObj
    {
        private Guid _Server;
        private LicenseServerInstance _LicenseServerInstance;
        private string _Description;
        private byte[] _Info;
        private ServerLicenseInfo _ServerLicenseInfo;

        private byte[] Key = { 185, 45, 138, 86, 180, 73, 50, 31, 73, 26, 53, 32, 2, 143, 64, 108, 214, 198, 77, 246, 219, 201, 128, 120 };
        private byte[] IV = { 57, 193, 117, 41, 127, 122, 176, 58 };

        public LicenseUserDistribution()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public LicenseUserDistribution(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            _Info = null;
            _ServerLicenseInfo = null;
        }

        public int MaxConnectedUsers
        {
            get
            {
                return this.ServerLicenseInfo.MaxConnectedUsers;
            }
        }

        public int MaxPeripheralsUsers
        {
            get
            {
                return this.ServerLicenseInfo.MaxPeripheralsUsers;
            }
        }

        public int MaxTabletSmartPhoneUsers
        {
            get
            {
                return this.ServerLicenseInfo.MaxTabletSmartPhoneUsers;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return this.ServerLicenseInfo.StartDate;
            }
        }
        public DateTime EndDate
        {
            get
            {
                return this.ServerLicenseInfo.EndDate;
            }
        }

        public int DaysToAlertBeforeExpiration
        {
            get
            {
                return this.ServerLicenseInfo.DaysToAlertBeforeExpiration;
            }
        }

        public int GreyZoneDays
        {
            get
            {
                return this.ServerLicenseInfo.GreyZoneDays;
            }
        }


        public Guid Server
        {
            get
            {
                return _Server;
            }
            set
            {
                SetPropertyValue("Server", ref _Server, value);
            }
        }

        public string Description
        {
            get
            {
                return _Description;
            }
            set
            {
                SetPropertyValue("Description", ref _Description, value);
            }
        }
        
        public byte[] Info
        {
            get
            {
                return _Info;
            }
            set
            {
                _ServerLicenseInfo = null;
                SetPropertyValue("Info", ref _Info, value);                
            }
        }

        public ServerLicenseInfo ServerLicenseInfo
        {
            get
            {
                if (this.Info == null || this.Info.Length <= 0 )
                {
                    _ServerLicenseInfo = null;
                }
                else if (_ServerLicenseInfo == null)
                {
                    using (TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider())
                    {
                        using (MemoryStream fileStream = new MemoryStream(this.Info))
                        {
                            using (CryptoStream cryptoStream = new CryptoStream(fileStream, cryptoServiceProvider.CreateDecryptor(Key, IV), CryptoStreamMode.Read))
                            {
                                XmlSerializer serializer = new XmlSerializer(typeof(ServerLicenseInfo));
                                _ServerLicenseInfo = serializer.Deserialize(cryptoStream) as ServerLicenseInfo;
                            }
                        }
                    }
                }
                return _ServerLicenseInfo;
            }
        }

        public LicenseServerInstance LicenseServerInstance
        {
            get
            {
                return _LicenseServerInstance;
            }
            set
            {
                SetPropertyValue("LicenseServerInstance", ref _LicenseServerInstance, value);
            }
        }

        public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew supplier, Store store, string deviceID)
        {
            CriteriaOperator crop = null;
            switch (direction)
            {
                case eUpdateDirection.MASTER_TO_STORECONTROLLER:
                    if (supplier == null || store == null)
                    {
                        throw new Exception("LicenseUserDistribution.GetUpdaterCriteria(); Error: Supplier or Store is null");
                    }
                    crop = CriteriaOperator.And(new BinaryOperator("LicenseServerInstance", LicenseServerInstance.STORE_CONTROLLER) ,
                                                new BinaryOperator("Server", store.StoreControllerSettings.Oid));
                    break;
            }
            return crop;
        }

        public void SetInfo(ServerLicenseInfo serverLicenseInfo)
        {
            using (TripleDESCryptoServiceProvider cryptoServiceProvider = new TripleDESCryptoServiceProvider())
            {
                using (MemoryStream fileStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(fileStream, cryptoServiceProvider.CreateEncryptor(Key, IV), CryptoStreamMode.Write))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ServerLicenseInfo));
                        serializer.Serialize(cryptoStream, serverLicenseInfo);
                        cryptoStream.Flush();
                        fileStream.Flush();
                        cryptoStream.Close();
                        fileStream.Close();
                        this.Info = fileStream.ToArray();
                    }
                }
            }
        }
    }
}
