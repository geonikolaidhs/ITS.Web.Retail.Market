using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{

    public class ContactFormEmailTemplate : EmailTemplate
    {
        public ContactFormEmailTemplate() : base()
        {
            this.ViewName = "ContactFormTemplate";
        }

        public string FullName;
        public string Email;
        public string Subject;
        public string Message;
    }
}
