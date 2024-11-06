using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.ObjectModel
{
    public class Customer
    {
        private string _code;
        public string Code
        {
            get { return _code; }
            set
            {
                _code = value;
            }
        }

        private string _aFM;
        public string AFM
        {
            get { return _aFM; }
            set
            {
                _aFM = value;
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
            }
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
            }
        }
        
    }
}
