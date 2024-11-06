using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class CustomActionCode : BaseObj
    {
        private string _Code;
        private eActions _Action;
        public CustomActionCode()
			: base()
		{
			// This constructor is used when an object is loaded from a persistent storage.
			// Do not place any code here.
		}

        public CustomActionCode(Session session)
			: base(session)
		{
			// This constructor is used when an object is loaded from a persistent storage.
			// Do not place any code here.
		}


        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        public eActions Action
        {
            get
            {
                return _Action;
            }
            set
            {
                SetPropertyValue("Action", ref _Action, value);
            }
        }
    }
}
