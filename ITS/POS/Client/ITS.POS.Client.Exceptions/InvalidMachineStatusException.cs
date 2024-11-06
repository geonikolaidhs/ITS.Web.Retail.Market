using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.Exceptions
{
    public class InvalidMachineStatusException : POSException
    {
        public eMachineStatus ExpectedMachineStatuses { get; set; }
        public eMachineStatus CurrentMachineStatus { get; set; }

        public InvalidMachineStatusException(eMachineStatus currentStatus, eMachineStatus expectedStatuses) : base("Invalid Machine Status '"+currentStatus+"'. Expected '"+expectedStatuses+"'")
        {
            ExpectedMachineStatuses = expectedStatuses;
            CurrentMachineStatus = currentStatus;
        }
    }
}
