using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using SQLiteNetExtensions;
//using SQLiteNetExtensions.Attributes;
//using SQLiteNetExtensions.Extensions;

namespace ITS.WRM.Model.Interface.Model
{
    public interface IReasonCategory: ILookUp2Fields
    {
         
        
        List<IReason> Reasons
        {
            get;
            set;
        }
    }
}
