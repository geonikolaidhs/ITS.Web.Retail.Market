using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ITS.Retail.Model;

namespace ITS.Retail.WebClient
{
    public class CustomerRevenue
    {
        public Customer Customer { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthOfYear
        {
            get
            {
                return Month.ToString("00") + "/" + Year;
            }
        }
        public decimal Revenue { get; set; }

    }
}