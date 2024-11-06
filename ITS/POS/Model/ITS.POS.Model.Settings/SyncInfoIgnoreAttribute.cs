using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Model.Settings
{
    /// <summary>
    /// A Model class marked with this attribute will be ignored from the post Synchronization Info thread
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SyncInfoIgnoreAttribute : Attribute
    {
    }
}
