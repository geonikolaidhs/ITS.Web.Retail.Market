using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.AuxillaryClasses;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.Helpers
{
    public class ActiveUsersValidator
    {
        private ConcurrentBag<ApplicationUser> ApplicationUsers;

        public ActiveUsersValidator()
        {
            ApplicationUsers = new ConcurrentBag<ApplicationUser>();
        }

        public int NumberOfActiveUsers
        {
            get
            {
                int applicationUsers = 0;
                if(this.ApplicationUsers != null)
                {
                    applicationUsers = this.ApplicationUsers.Count;
                }
                return applicationUsers + CountActivePOS();// + CountActiveMobileTerminals();
            }
        }

        public int NumberOfActivePeripheralUsers
        {
            get
            {
                return CountActiveMobileTerminals();
            }
        }

        public int NumberOfActiveTabletSmartPhoneUsers
        {
            get
            {
                return 0;//TODO Implement this after developing mobile app
            }
        }

        public void AddApplicationUser(string userID, string sessionID)
        {
            ApplicationUser alreadyLoggedInUser = this.ApplicationUsers.Where(user => user.UserID == userID).FirstOrDefault();
            if (alreadyLoggedInUser == null)
            {
                ApplicationUser newApplicationUser = new ApplicationUser();
                newApplicationUser.UserID = userID;
                newApplicationUser.SessionID = sessionID;
                this.ApplicationUsers.Add(newApplicationUser);
            }
        }

        public void RemoveApplicationUser(string sessionID)
        {
            ApplicationUser loggedOfUser = this.ApplicationUsers.Where(user => user.SessionID == sessionID).FirstOrDefault();
            if (loggedOfUser != null)
            {
                this.ApplicationUsers.TryTake(out loggedOfUser);
            }
        }

        public int CountActivePOS()
        {
            if(MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL)
            {
                return 0;
            }
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                CriteriaOperator criteria = new BinaryOperator("IsActive", true);
                return (int)uow.Evaluate(typeof(ITS.Retail.Model.POS), CriteriaOperator.Parse("Count"), criteria);
            }
        }

        public int CountActiveMobileTerminals()
        {
            if (MvcApplication.ApplicationInstance == eApplicationInstance.RETAIL)
            {
                return 0;
            }
            using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
            {
                CriteriaOperator criteria = new BinaryOperator("IsActive", true);
                return (int)uow.Evaluate(typeof(ITS.Retail.Model.MobileTerminal), CriteriaOperator.Parse("Count"), criteria);
            }
        }

        public int CountActiveNonWebUsers()
        {
            return CountActivePOS() + CountActiveMobileTerminals();
        }

        public bool FreeConnectionSlotExists(int MaxAllowedUsers)
        {
            return this.NumberOfActiveUsers < MaxAllowedUsers;
        }
    }
}