using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.AuxillaryClasses
{
    public class DropDownSearchColumn
    {
        public string Field { get; private set; }
        public string Display { get; private set; }
        public int Size { get; private set; }

        public DropDownSearchColumn(string field, string display, int size)
        {
            this.Field = field;
            this.Display = display;
            this.Size = size;
        }
    }
}