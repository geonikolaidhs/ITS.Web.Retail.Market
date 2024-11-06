using System;

using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Net;

namespace ITS.MobileAtStore
{
    public class TerminalSettings : INotifyPropertyChanged
    {
        private int _ID;
        public int ID
        {
            get
            {
                return _ID;
            }
            set
            {
                _ID = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ID"));
                }
            }
        }

        private string _IP;
        public string IP
        {
            get
            {
                if (string.IsNullOrEmpty(_IP))
                {
                    string strHostName = Dns.GetHostName();
                    IPHostEntry hostInfo = Dns.GetHostByName(strHostName);
                    _IP = hostInfo.AddressList[0].ToString();
                }
                return _IP;
            }
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
