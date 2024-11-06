using ITS.Retail.ResourcesLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace ITS.Retail.Platform.Enumerations
{
    [Flags]
    public enum ePosCommand
    {
        [Display(Name = "NONE_COMMAND", ResourceType = typeof(Resources))]
        NONE = 0,
        [Display(Name = "SEND_CHANGES", ResourceType = typeof(Resources))]
        SEND_CHANGES = 1,
        [Display(Name = "RESTART_POS", ResourceType = typeof(Resources))]
        RESTART_POS = 2,
        [Display(Name = "ISSUE_X", ResourceType = typeof(Resources))]
        ISSUE_X = 4,
        [Display(Name = "ISSUE_Z", ResourceType = typeof(Resources))]
        ISSUE_Z = 8,
        [Display(Name = "RELOAD_ENTITIES", ResourceType = typeof(Resources))]
        RELOAD_ENTITIES = 16,
        RETRY_IMMEDIATE = 32,
        EXECUTE_POS_SQL = 35,
        EXECUTE_POS_CMD = 67,
        POS_UPDATE = 69,
        POS_APPLICATION_RESTART = 70
    }
}