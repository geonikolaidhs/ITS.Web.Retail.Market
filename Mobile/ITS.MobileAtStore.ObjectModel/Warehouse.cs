using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.ObjectModel
{
    public class Warehouse
    {
        private string _compCode;
        public string CompCode
        {
                get
                {
                    return _compCode;
                }
                set
                {
                    _compCode = value;
                }
        }

        public string Description { get; set; }
        public string DisplayText
        {
            get
            {
                return CompCode + " - " + Description;
            }
        }
        public string ErrorMessage;
        public override string ToString()
        {
            return Description;
        }
    }
}
