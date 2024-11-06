using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.ViewModel.Totalisers
{
    public class CustomDataViewCategoryViewModel
    {
        public string label { get; set; }
        //public Guid Oid { get; set; }
        public string value { get; set; }
        public bool isDefault { get; set; }
    }
}