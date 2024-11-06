using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Model.Settings;

namespace ITS.POS.Client.Exceptions
{
    public class InvalidActionParametersException : POSException
    {
        private object _invalidArguments;
        public object InvalidArguments
        {
            get
            {
                return _invalidArguments;
            }
        }

        //private Type _expectedArgumentsType;
        //public Type ExpectedArgumentsType
        //{
        //    get
        //    {
        //        return _expectedArgumentsType; 
        //    }
        //}

        private eActions _actionCode;
        public eActions ActionCode
        {
            get
            {
                return _actionCode;
            }
        }

        public InvalidActionParametersException(eActions actionCode,object invalidArguments)//,Type expectedArgumentsType = null)
            : base("Action '" + actionCode.ToString() + "' arguments of type '" + (invalidArguments == null ? "null" : invalidArguments.GetType().Name) + "' are invalid." )//+ (expectedArgumentsType == null ? "" : " Expected '") + expectedArgumentsType.Name + "'") 
        {
            this._actionCode = actionCode;
            //this._expectedArgumentsType = expectedArgumentsType;
            this._invalidArguments = invalidArguments;
        }
    }
}
