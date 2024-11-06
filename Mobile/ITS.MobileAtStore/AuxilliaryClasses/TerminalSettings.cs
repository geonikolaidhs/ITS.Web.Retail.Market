using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace ITS.MobileAtStore.AuxilliaryClasses
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

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
