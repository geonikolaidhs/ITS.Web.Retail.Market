using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    public class VerifyEmailTemplate : EmailTemplate
    {
        string _URL;


        public string URL
        {
            get
            {
                return _URL;
            }
            set
            {
                SetPropertyValue("URL", ref _URL, value);
            }
        }
    }
}
