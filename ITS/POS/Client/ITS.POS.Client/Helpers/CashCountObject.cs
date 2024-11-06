using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Helpers
{
    [Serializable]
    public class CashCountCoinObj : INotifyPropertyChanged
    {
        public CashCountCoinObj()
        {

        }
        #region Property CountedCoins
        private decimal _CountedCoins;
        public decimal CountedCoins
        {
            get { return _CountedCoins; }
            set
            {
                if (_CountedCoins != value)
                {
                    _CountedCoins = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CountedCoins"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GridValue"));
                }
            }
        }
        #endregion
        #region Property CoinValue
        private decimal _CoinValue;
        public decimal CoinValue
        {
            get { return _CoinValue; }
            set
            {
                if (_CoinValue != value)
                {
                    _CoinValue = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CoinValue"));
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GridValue"));
                }
            }
        }
        #endregion
        #region Property Amount
        public decimal Amount
        {
            get { return CountedCoins * CoinValue; }
        }
        #endregion
        public override string ToString()
        {
            try
            {

                string valueString = CoinValue.ToString("0.00");
                while (valueString.Length < 5)
                {
                    valueString = " " + valueString;
                }
                string outputString = valueString + " * ";
                valueString = CountedCoins.ToString();
                while (valueString.Length < 5)
                {
                    valueString = " " + valueString;
                }
                outputString += valueString + " = ";
                valueString = (CountedCoins * CoinValue).ToString("0.00");
                while (valueString.Length < 8)
                {
                    valueString = " " + valueString;
                }
                outputString += valueString;
                return outputString;
            }
            catch
            {
                return base.ToString();
            }

        }
        public string GridValue { get { return this.ToString(); } }
        public event PropertyChangedEventHandler PropertyChanged;
    }
    [Serializable]
    public class CashCountAmountObj : INotifyPropertyChanged
    {
        public CashCountAmountObj()
        {

        }
        #region Property Amount
        private decimal _Amount;
        public decimal Amount
        {
            get { return _Amount; }
            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Amount"));
                }
            }
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class CashCountFinalObject
    {
        public string Description { get; set; }
        public decimal Amount { get; set; }
       
    }
}
