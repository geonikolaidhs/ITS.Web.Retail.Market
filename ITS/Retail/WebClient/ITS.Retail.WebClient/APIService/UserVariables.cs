using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.APIService
{
    public class UserVariables
    {
        public Guid Oid { get; set; }
        public Guid Company { get; set; }
        public List<Guid> AllowedCompanies { get; set; }

    }
}