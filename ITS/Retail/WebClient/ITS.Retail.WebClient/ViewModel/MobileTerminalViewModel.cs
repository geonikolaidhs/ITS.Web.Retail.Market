using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace ITS.Retail.WebClient.ViewModel
{
    public class MobileTerminalViewModel : BasePersistableViewModel, IPersistableViewModel
    {
        [Required]
        public string Name { get; set; }

        public int ID { get; set; }

        public bool IsActive { get; set; }

        [Required]
        public string IPAddress { get; set; }

        [Required]
        public string Remarks { get; set; }

        public Guid Store { get; set; }
        
        public override Type PersistedType
        {
            get { return typeof(MobileTerminal); }
        }
    }
}