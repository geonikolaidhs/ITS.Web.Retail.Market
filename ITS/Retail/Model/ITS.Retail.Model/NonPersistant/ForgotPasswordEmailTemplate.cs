using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    public class ForgotPasswordEmailTemplate : EmailTemplate
    {
        public ForgotPasswordEmailTemplate()
            : base()
        {
            this.ViewName = "EmailTemplateForgotPassword";
        }

        public string URL;
        public string Password;
    }
}
