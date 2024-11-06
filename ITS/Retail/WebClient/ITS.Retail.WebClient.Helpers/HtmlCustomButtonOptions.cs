using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public class HtmlCustomButtonOptions : HtmlButtonOptions
    {
        public string CCSClass { get; set; }
        public string Title { get; set; }
        public Boolean EncodeHtml { get; set; }
        public Boolean UseSubmitBehavior { get; set; }
    }
}
