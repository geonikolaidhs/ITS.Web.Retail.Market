using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
	public class KeyListenerParams : ObserverParams
	{
	    //	public POSKeyMapping KeyMapping { get; set; }
        public eActions ActionCode { get; set; }
        public eNotificationsTypes NotificationType { get; set; }
        public Keys RedirectTo { get; set; }
        public Keys KeyData { get; set; }
        public string ActionParameters { get; set; }

        public KeyListenerParams(eActions actionCode, eNotificationsTypes notificationType, Keys redirectTo, Keys keyData, string actionParameters)
		{
            this.ActionCode = actionCode;
            this.NotificationType = notificationType;
            this.RedirectTo = redirectTo;
            this.KeyData = keyData;
            this.ActionParameters = actionParameters;
		}


        public override Type GetObserverType()
        {
            return typeof(IObserverKeyListener);
        }
    }
}
