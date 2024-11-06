using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.ObjectModel
{
    public class ProductQuantity
    {
        private string _code;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        private decimal _quantity = 0;
        public decimal Quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
            }
        }

        private string _extraCode;
        public string ExtraCode
        {
            get
            {
                return _extraCode;
            }
            set
            {
                _extraCode = value;
            }
        }
    }
}
