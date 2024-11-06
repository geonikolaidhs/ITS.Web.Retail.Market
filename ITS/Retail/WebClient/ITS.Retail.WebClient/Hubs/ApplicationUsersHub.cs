using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.WebClient.Helpers;
using System.Web.SessionState;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Common;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.WebClient.Hubs
{
    [Authorize]
    public class ApplicationUsersHub : Hub
    {
        public void Send(string name, string message)
        {
            string userString = MvcApplication.ActiveUsersValidator.NumberOfActiveUsers == 1 ? ResourcesLib.Resources.User : ResourcesLib.Resources.Users;
            string applicationMode = ITS.Retail.Platform.Enumerations.Enum<eApplicationInstance>.ToLocalizedString(MvcApplication.ApplicationInstance);
            string clientMessage = String.Format("{0} {1}. {2}: {3}.",
                                                 MvcApplication.ActiveUsersValidator.NumberOfActiveUsers,
                                                 userString,
                                                 ResourcesLib.Resources.ApplicationInstance,
                                                 applicationMode);
            Clients.All.assertUsers(clientMessage);
        }

        public override Task OnConnected()
        {
            MvcApplication.ActiveUsersValidator.AddApplicationUser(Context.User.Identity.Name, Context.ConnectionId);
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            MvcApplication.ActiveUsersValidator.RemoveApplicationUser(Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }
    }
}
