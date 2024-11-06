using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Helpers
{
    public class DynamicFunction
    {
        public String Name { get; set; }
        public String Code { get; set; }
        public Guid DocumentTypeOid { get; set; }
        public String ReturnType { get; set; }
        public String Parameters { get; set; }

        public DynamicFunction(String name, String code, Guid documentOid, String returnType, String parameters)
        {
            this.Name = name;
            this.Code = code;
            this.DocumentTypeOid = documentOid;
            this.ReturnType = returnType;
            this.Parameters = parameters;
        }
    }
}
