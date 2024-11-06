using ITS.WRM.Model.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    public class PersistentObjectModel : IPersistentObjectModel
    {
        public DateTime CreatedOn { get; set; }
        
        public Guid Oid { get; set; }

        public DateTime UpdatedOn { get; set; }

    }
}