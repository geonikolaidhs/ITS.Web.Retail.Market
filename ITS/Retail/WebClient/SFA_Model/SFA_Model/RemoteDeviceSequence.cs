using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 1, Permissions = eUpdateDirection.NONE)]
    public class RemoteDeviceSequence: BasicObj
    {
        public int RemoteDeviceSequenceNumber { get; set; }
    }
}