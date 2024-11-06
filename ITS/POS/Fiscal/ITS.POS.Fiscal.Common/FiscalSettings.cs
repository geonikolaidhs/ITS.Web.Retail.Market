using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Fiscal.Common
{
    public class FiscalServiceSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// Server Listening port
        /// </summary>
        private int _port;
        public int Port
        {
            get
            {
                return _port;
            }
            set
            {
                _port = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Port"));
            }
        }

        /// <summary>
        /// Algobox Connection Type
        /// </summary>
        private ConnectionType _ConnectionType;
        public ConnectionType ConnectionType
        {
            get
            {
                return _ConnectionType;
            }
            set
            {
                _ConnectionType = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ConnectionType"));
            }
        }

        /// <summary>
        /// Algobox Device Name
        /// </summary>
        private String _DeviceName;
        public string DeviceName
        {
            get
            {
                return _DeviceName;
            }
            set
            {
                _DeviceName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DeviceName"));
            }
        }

        /// <summary>
        /// COM Connection Property: Port (eg COM1, COM2 etc)
        /// </summary>
        private String _COM_PortName;
        public string COM_PortName
        {
            get
            {
                return _COM_PortName;
            }
            set
            {
                _COM_PortName = value;
                OnPropertyChanged(new PropertyChangedEventArgs("COM_PortName"));
            }
        }


        /// <summary>
        /// Property: AesKey
        /// </summary>
        private string _aesKey;
        public string AesKey
        {
            get
            {
                return _aesKey;
            }
            set
            {
                _aesKey = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AesKey"));
            }
        }




        /// <summary>
        /// Property: EafdssSN
        /// </summary>
        private string _eafdssSN;
        public string EafdssSN
        {
            get
            {
                return _eafdssSN;
            }
            set
            {
                _eafdssSN = value;
                OnPropertyChanged(new PropertyChangedEventArgs("EafdssSN"));
            }
        }


        /// <summary>
        /// Property: GgpsUrl
        /// </summary>
        private string _ggpsUrl;
        public string GgpsUrl
        {
            get
            {
                return _ggpsUrl;
            }
            set
            {
                _ggpsUrl = value;
                OnPropertyChanged(new PropertyChangedEventArgs("GgpsUrl"));
            }
        }

        /// <summary>
        /// Property: SendFiles
        /// </summary>
        private bool _sendFiles;
        public bool SendFiles
        {
            get
            {
                return _sendFiles;
            }
            set
            {
                _sendFiles = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SendFiles"));
            }
        }

        /// <summary>
        /// Property: SMTP Server
        /// </summary>
        private string _smtpServer;
        public string SmtpServer
        {
            get
            {
                return _smtpServer;
            }
            set
            {
                _smtpServer = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SmtpServer"));
            }
        }


        /// <summary>
        /// Property: SMTP Server
        /// </summary>
        private string _smtpUser;
        public string SmtpUser
        {
            get
            {
                return _smtpUser;
            }
            set
            {
                _smtpUser = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SmtpUser"));
            }
        }


        /// <summary>
        /// Property: SMTP Server
        /// </summary>
        private string _smtpPort;
        public string SmtpPort
        {
            get
            {
                return _smtpPort;
            }
            set
            {
                _smtpPort = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SmtpPort"));
            }
        }

        /// <summary>
        /// Property: SmtpPass
        /// </summary>
        private string _smtpPass;
        public string SmtpPass
        {
            get
            {
                return _smtpPass;
            }
            set
            {
                _smtpPass = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SmtpPass"));
            }
        }


        /// <summary>
        /// Property: MailList
        /// </summary>
        private string _mailList;
        public string MailList
        {
            get
            {
                return _mailList;
            }
            set
            {
                _mailList = value;
                OnPropertyChanged(new PropertyChangedEventArgs("MailList"));
            }
        }



        /// <summary>
        /// Property: SmtpPass
        /// </summary>
        private bool _sendMailOnUploadFail;
        public bool SendMailOnUploadFail
        {
            get
            {
                return _sendMailOnUploadFail;
            }
            set
            {
                _sendMailOnUploadFail = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SendMailOnUploadFail"));
            }
        }

        /// <summary>
        /// Property: SmtpPass
        /// </summary>
        private bool _sendMailOnUploadSuccess;
        public bool SendMailOnUploadSuccess
        {
            get
            {
                return _sendMailOnUploadSuccess;
            }
            set
            {
                _sendMailOnUploadSuccess = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SendMailOnUploadSuccess"));
            }
        }



        /// <summary>
        /// Ethernet Connection Property: IPAddress
        /// </summary>
        private string _ip;
        public string Ethernet_IPAddress
        {
            get
            {
                return _ip;
            }
            set
            {
                _ip = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Ethernet_IPAddress"));
            }
        }

        private string _msg;
        public string DeviceNotFoundMessage
        {
            get
            {
                return _msg;
            }
            set
            {
                _msg = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DeviceNotFoundMessage"));
            }
        }

        private bool _fiscalOnError;
        public bool FiscalOnError
        {
            get
            {
                return _fiscalOnError;
            }
            set
            {
                _fiscalOnError = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FiscalOnError"));
            }
        }

        private string _sn;
        public string SerialNumber
        {
            get
            {
                return _sn;
            }
            set
            {
                _sn = value;
                OnPropertyChanged(new PropertyChangedEventArgs("SerialNumber"));
            }
        }

        private string _rn;
        public string RegistrationNumber
        {
            get
            {
                return _rn;
            }
            set
            {
                _rn = value;
                OnPropertyChanged(new PropertyChangedEventArgs("RegistrationNumber"));
            }
        }


        private string _AbcFolder;
        public string AbcFolder
        {
            get
            {
                return _AbcFolder;
            }
            set
            {
                _AbcFolder = value;
                OnPropertyChanged(new PropertyChangedEventArgs("AbcFolder"));
            }
        }

        private eFiscalDevice _fd;
        public eFiscalDevice FiscalDevice
        {
            get
            {
                return _fd;
            }
            set
            {
                _fd = value;
                OnPropertyChanged(new PropertyChangedEventArgs("FiscalDevice"));
            }
        }


        public FiscalServiceSettings()
        {
            ConnectionType = Retail.Platform.Enumerations.ConnectionType.COM;
            Ethernet_IPAddress = "";
            COM_PortName = "";
            DeviceName = "";
            SerialNumber = "-";
            DeviceNotFoundMessage = "ΧΩΡΙΣ ΣΗΜΑΝΣΗ ΛΟΓΩ ΒΛΑΒΗΣ ΤΗΣ Ε.Α.Φ.Δ.Σ.Σ. ΜΕ ΑΡ.ΜΗΤΡΩΟΥ {0}";
        }
        [Browsable(false)]
        public bool IsCOM
        {
            get
            {
                return ConnectionType == Retail.Platform.Enumerations.ConnectionType.COM;
            }
            set
            {
                if (value)
                {
                    ConnectionType = Retail.Platform.Enumerations.ConnectionType.COM;
                }
            }
        }



        [Browsable(false)]
        public bool IsEthernet
        {
            get
            {
                return ConnectionType == Retail.Platform.Enumerations.ConnectionType.ETHERNET;
            }
            set
            {
                if (value)
                {
                    ConnectionType = Retail.Platform.Enumerations.ConnectionType.ETHERNET;
                }
            }
        }

        private bool _KeepLog;
        public bool KeepLog
        {
            get
            {
                return _KeepLog;
            }
            set
            {
                _KeepLog = value;
                OnPropertyChanged(new PropertyChangedEventArgs("KeepLog"));
            }
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            // get handler (usually a local event variable or just the event)
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion
    }
}
