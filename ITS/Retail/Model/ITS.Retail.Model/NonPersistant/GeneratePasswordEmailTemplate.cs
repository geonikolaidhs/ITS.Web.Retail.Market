using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    public class GeneratePasswordEmailTemplate : EmailTemplate
    {
        public GeneratePasswordEmailTemplate()
            : base()
        {
            this.ViewName = "EmailTemplateGeneratePassword";
        }

        public string Password;
    }
}
