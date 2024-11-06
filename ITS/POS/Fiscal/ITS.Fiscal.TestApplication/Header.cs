using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using ITS.Retail.Platform.Enumerations;


namespace ITS.Fiscal.TestApplication
{

    public class Header : INotifyPropertyChanged
    {


        internal class Detail
        {
            public eMinistryVatCategoryCode Vat { get; set; }
            public double Gross { get; set; }
            public double Net { get; set; }
            public double VatAmount { get; set; }


            public Detail(eMinistryVatCategoryCode vat, double gross)
            {
                Vat = vat;
                Gross = Math.Round(gross,2);
                Net =Math.Round(Gross / (1+Header.VatMapping[vat]),2);
                VatAmount = Math.Round(Gross - Net, 2);
            }
            public String ReceiptLine
            {
                get
                {
                    String v = Vat.GetDescription();
                    return String.Format("ΕΙΔΟΣ {0}    {1} {0}", v, String.Format("{0,27}", Gross.ToString("c2")));
                }
            }

        }
        private bool _Closed;
        public bool Closed {
            get
            {
                return _Closed;
            }
            set
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Closed"));
                _Closed = value;
            }
        }

        public double GrossA { get; set; }
        public double GrossB { get; set; }
        public double GrossC { get; set; }
        public double GrossD { get; set; }
        public double GrossE { get; set; }

        public double Gross
        {
            get
            {
                return GrossA + GrossB + GrossC + GrossD + GrossE;
            }
        }

        public double NetA { get; set; }
        public double NetB { get; set; }
        public double NetC { get; set; }
        public double NetD { get; set; }
        public double NetE { get; set; }

        public double VATA { get; set; }
        public double VATB { get; set; }
        public double VATC { get; set; }
        public double VATD { get; set; }
        public DateTime Date { get; protected set; }
        public String OwnerTaxCode { get;  set; }
        public String CustomerTaxCode { get;  set; }

        String _Signature;
        public String Signature
        {
            get
            {
                return _Signature;
            }
            set
            {
                _Signature = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Receipt"));
            }
        }
        static NumberFormatInfo nfi = new NumberFormatInfo() { CurrencyDecimalSeparator = ".", NumberDecimalSeparator = "." }; 
        public String OfficialString
        {
            get
            {
                
                string seperator = "/";
                return String.Format(nfi,"[<]{0}" + seperator + "{1}" + seperator + "{2}" + seperator + "{3:yyyyMMddHHmm}" + seperator + "{4}" + seperator + "{5}" + seperator +
                "{6}" + seperator + "{7:0.00}" + seperator + "{8:0.00}" + seperator + "{9:0.00}" + seperator + "{10:0.00}" + seperator + "{11:0.00}" + seperator + "{12:0.00}" + seperator + "{13:0.00}"
                + seperator + "{14:0.00}" + seperator + "{15:0.00}" + seperator + "{16:0.00}" + seperator + "{17}[>]",
                String.Concat(OwnerTaxCode.Take(12)),
                String.Concat(CustomerTaxCode.Take(12)),
                String.Concat(CustomerTaxCode.Take(19)) ,
                Date,
                "173",
                "ΣΕΙΡΑ 1",
                1, NetA, NetB,
                NetC, NetD, NetE,
                VATA,
                VATB,
                VATC,
                VATD,
                Gross, 0   //Euro is 0
                );
            }

        }

        public String Receipt
        {
            get
            {
                String toReturn = "<HEADER>" + Environment.NewLine;
                if (Details.Count > 0)
                {
                    toReturn += Details.Select(g => g.ReceiptLine).Aggregate((f, s) => f + Environment.NewLine + s);
                }
                if (Closed)
                {
                    toReturn += Environment.NewLine + String.Format("ΣΥΝΟΛΟ       {0:c2}    ", Gross);
                    toReturn += Environment.NewLine + "        <FOOTER>       ";
                    toReturn += Environment.NewLine + Signature;
                }
                return toReturn;
            }
        }

        private List<Detail> Details;

        public static Dictionary<eMinistryVatCategoryCode, double> VatMapping = new Dictionary<eMinistryVatCategoryCode, double>()
        {
            {eMinistryVatCategoryCode.A, 0.065},
            {eMinistryVatCategoryCode.B, 0.130},
            {eMinistryVatCategoryCode.C, 0.230},
            {eMinistryVatCategoryCode.D, 0.360},
            {eMinistryVatCategoryCode.E, 0.000}
        };

        public Header()
        {
            Details = new List<Detail>();
            Date = DateTime.Now;
        }

        public void AddDetail(eMinistryVatCategoryCode vat, double gross)
        {
            Details.Add(new Detail(vat,gross));
            GrossA = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.A).Sum(g=>g.Gross);
            GrossB = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.B).Sum(g=>g.Gross);
            GrossC = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.C).Sum(g=>g.Gross);
            GrossD = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.D).Sum(g=>g.Gross);
            GrossE = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.E).Sum(g=>g.Gross);

            NetA = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.A).Sum(g=>g.Net);
            NetB = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.B).Sum(g=>g.Net);
            NetC = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.C).Sum(g=>g.Net);
            NetD = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.D).Sum(g=>g.Net);
            NetE = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.E).Sum(g=>g.Net);

            VATA = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.A).Sum(g=>g.VatAmount);
            VATB = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.B).Sum(g=>g.VatAmount);
            VATC = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.C).Sum(g=>g.VatAmount);
            VATD = Details.Where(g=>g.Vat== eMinistryVatCategoryCode.D).Sum(g=>g.VatAmount);

            PropertyChanged(this, new PropertyChangedEventArgs("Receipt"));
            PropertyChanged(this, new PropertyChangedEventArgs("Gross"));
         
        }


        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
    
        

}
