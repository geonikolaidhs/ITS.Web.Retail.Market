using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 130, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class User : BaseObj
    {
        public User()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public User(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            this.Email = string.Empty;
            this.IsB2CUser = false;
        }
        public bool IsB2CUser { get; set; }
        public DateTime LastLoginDate { get; set; }
        public string Comment { get; set; }
        public DateTime FailedPasswordAttemptWindowStart { get; set; }
        public int FailedPasswordAttemptCount { get; set; }
        public DateTime LastLockedOutDate { get; set; }
        public DateTime LastActivityDate { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public string PasswordAnswer { get; set; }
        public string PasswordQuestion { get; set; }
        public bool IsApproved { get; set; }
        public string Email { get; set; }
        public string Key { get; set; }
        public bool IsCentralStore { get; set; }
        public bool TermsAccepted { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string TaxCode { get; set; }
        //Role Role { get; set; }
        public string POSUserName { get; set; }
        public string POSPassword { get; set; }
        public ePOSUserLevel POSUserLevel { get; set; }
        public string AuthToken { get; set; }
    }
}