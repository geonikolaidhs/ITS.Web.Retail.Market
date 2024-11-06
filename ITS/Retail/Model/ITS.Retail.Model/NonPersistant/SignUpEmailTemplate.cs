using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    public class SignUpEmailTemplate : EmailTemplate
    {

        public SignUpEmailTemplate()
            : base()
        {
            this.ViewName = "EmailTemplateSignUp";
        }

        public string URL;
    }
}