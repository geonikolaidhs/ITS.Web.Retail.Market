using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using System.Resources;
using System.Reflection;



namespace ITS.Retail.Model
{
    public class DisplayOrderAttribute :Attribute

    {
       
        private int order;

        public int Order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
            }
        }
        

    }
}
