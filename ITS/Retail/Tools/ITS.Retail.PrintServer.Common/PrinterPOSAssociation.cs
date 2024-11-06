using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ITS.Retail.PrintServer.Common
{
    public class PrinterPOSAssociation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Printer
        {
            get
            {
                return _Printer;
            }
            set
            {
                SetPropertyValue("Printer", ref _Printer, value);
            }
        }
        public int POSID
        {
            get
            {
                return _POSID;
            }
            set
            {
                SetPropertyValue("POSID", ref _POSID, value);
            }
        }


        protected void SetPropertyValue<T>(string propertyName, ref T localField, T newValue)
        {
            localField = newValue;
            Notify(propertyName);
        }

        private void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }



        // Fields...
        private string _Printer;
        private int _POSID;
    }
}
