using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    public class OrderEmailTemplate : EmailTemplate
    {
        public OrderEmailTemplate()
            : base()
        {
            this.ViewName = "EmailTemplateOrder";
        }

        public DocumentHeader DocumentHeader;
    }
}
