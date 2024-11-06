using System;
using System.Collections.Generic;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Web.Security;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Common;
using WebMatrix.WebData;
using ITS.Retail.WebClient.Helpers;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;

namespace ITS.Retail.WebClient.Providers
{
    public class XpoMembershipProvider : ExtendedMembershipProvider
    {

        private readonly static object lockObject = new object();
        private enum FailureType
        {
            Password = 1,
            PasswordAnswer = 2
        }

        //
        // Used when determining encryption key values.
        //

        private MachineKeySection machineKey;

        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        public override void Initialize(String name, System.Collections.Specialized.NameValueCollection config)
        {
            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
            {
                name = "XpoMembershipProvider";
            }

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "XPO Membership provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            pApplicationName = GetConfigValue(config["applicationName"],
                                    System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
            pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"));
            pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"));
            pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"));
            pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""));
            pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"));
            pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));

            pPasswordFormat = MembershipPasswordFormat.Hashed;
        }

        //
        // A helper function to retrieve config values from the configuration file.
        //

        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
            {
                return defaultValue;
            }

            return configValue;
        }

        //
        // System.Web.Security.MembershipProvider properties.
        //

        private string pApplicationName;
        private bool pEnablePasswordReset;
        private bool pEnablePasswordRetrieval;
        private bool pRequiresQuestionAndAnswer;
        private bool pRequiresUniqueEmail;
        private int pMaxInvalidPasswordAttempts;
        private int pPasswordAttemptWindow;
        private MembershipPasswordFormat pPasswordFormat;

        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }

        public override bool EnablePasswordReset
        {
            get { return pEnablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return pEnablePasswordRetrieval; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return pRequiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return pRequiresUniqueEmail; }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return pMaxInvalidPasswordAttempts; }
        }

        public override int PasswordAttemptWindow
        {
            get { return pPasswordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return pPasswordFormat; }
        }

        private int pMinRequiredNonAlphanumericCharacters;

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return pMinRequiredNonAlphanumericCharacters; }
        }

        private int pMinRequiredPasswordLength;

        public override int MinRequiredPasswordLength
        {
            get { return pMinRequiredPasswordLength; }
        }

        private string pPasswordStrengthRegularExpression;

        public override string PasswordStrengthRegularExpression
        {
            get { return pPasswordStrengthRegularExpression; }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, newPassword, false);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                if (args.FailureInformation != null)
                {
                    throw args.FailureInformation;
                }
                else
                {
                    throw new Exception("Change password canceled due to new password validation failure.");
                }
            }

            using (Session session = XpoHelper.GetNewSession())
            {
                User user = session.FindObject<User>(
                    new BinaryOperator("UserName", username, BinaryOperatorType.Equal));
                if (user != null)
                {
                    user.Password = UserHelper.EncodePassword(newPassword);
                }
                else
                {
                    return false;
                }
                user.UpdatedBy = user;
                user.Save();
            }

            return true;
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            ValidatePasswordEventArgs args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresQuestionAndAnswer && String.IsNullOrEmpty(passwordAnswer))
            {
                status = MembershipCreateStatus.InvalidAnswer;
                return null;
            }

            if (RequiresUniqueEmail)
            {
                if (!IsEmail(email))
                {
                    status = MembershipCreateStatus.InvalidEmail;
                    return null;
                }
                if (!String.IsNullOrEmpty(GetUserNameByEmail(email)))
                {
                    status = MembershipCreateStatus.DuplicateEmail;
                    return null;
                }
            }

            MembershipUser mUser = GetUser(username, false);

            if (mUser != null)
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            using (UnitOfWork session = XpoHelper.GetNewUnitOfWork())
            {
                User user = new User(session)
                {

                    UserName = username,
                    Password = UserHelper.EncodePassword(password),
                    Email = email,
                    PasswordQuestion = passwordQuestion,
                    PasswordAnswer = UserHelper.EncodePassword(passwordAnswer),
                    IsApproved = isApproved,
                    FailedPasswordAnswerAttemptCount = 0,
                    FailedPasswordAnswerAttemptWindowStart = DateTime.MinValue,
                    IsLockedOut = false,
                    LastActivityDate = DateTime.Now,
                    LastLockedOutDate = DateTime.MinValue,
                    FailedPasswordAttemptCount = 0,
                    FailedPasswordAttemptWindowStart = DateTime.MinValue
                };

                user.Save();
                session.CommitChanges();
                status = MembershipCreateStatus.Success;
            }

            return GetUser(username, false);
        }

        /* Used by Web Site Administration Tool */

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            using (Session session = XpoHelper.GetNewSession())
            {
                User user = session.FindObject<User>(
                    new BinaryOperator("UserName", username, BinaryOperatorType.Equal));

                if (user == null)
                    return false;

                user.Delete();
                user.Save();
            }
            return true;
        }

        /* Used by Web Site Administration Tool */

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection mclUsers = new MembershipUserCollection();

            using (Session session = XpoHelper.GetNewSession())
            {
                XPCollection<User> xpcUsers = new XPCollection<User>(session,
                        new BinaryOperator("Email", String.Format("%{0}%", emailToMatch), BinaryOperatorType.Like),
                        new SortProperty("UserName", DevExpress.Xpo.DB.SortingDirection.Ascending));

                totalRecords = Convert.ToInt32(session.Evaluate<User>(CriteriaOperator.Parse("Count()"),
                        new BinaryOperator("Email", String.Format("%{0}%", emailToMatch), BinaryOperatorType.Like)));

                xpcUsers.SkipReturnedObjects = pageIndex * pageSize;
                xpcUsers.TopReturnedObjects = pageSize;

                foreach (User xpoUser in xpcUsers)
                {
                    MembershipUser mUser = GetUserFromXpoUser(xpoUser);
                    mclUsers.Add(mUser);
                }
            }

            return mclUsers;
        }

        /* Used by Web Site Administration Tool */

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection mclUsers = new MembershipUserCollection();

            using (Session session = XpoHelper.GetNewSession())
            {
                XPCollection<User> xpcUsers = new XPCollection<User>(session,
                        new BinaryOperator("UserName", String.Format("%{0}%", usernameToMatch), BinaryOperatorType.Like),
                        new SortProperty("UserName", DevExpress.Xpo.DB.SortingDirection.Ascending));

                totalRecords = Convert.ToInt32(session.Evaluate<User>(CriteriaOperator.Parse("Count()"),
                        new BinaryOperator("UserName", String.Format("%{0}%", usernameToMatch), BinaryOperatorType.Like)));

                xpcUsers.SkipReturnedObjects = pageIndex * pageSize;
                xpcUsers.TopReturnedObjects = pageSize;

                foreach (User xpoUser in xpcUsers)
                {
                    MembershipUser mUser = GetUserFromXpoUser(xpoUser);
                    mclUsers.Add(mUser);
                }
            }

            return mclUsers;
        }

        /* Used by Web Site Administration Tool */

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            MembershipUserCollection mclUsers = new MembershipUserCollection();

            using (Session session = XpoHelper.GetNewSession())
            {
                XPCollection<User> xpcUsers = new XPCollection<User>(session, null,
                        new SortProperty("UserName", DevExpress.Xpo.DB.SortingDirection.Ascending));

                totalRecords = Convert.ToInt32(session.Evaluate<User>(CriteriaOperator.Parse("Count()"), null));

                xpcUsers.SkipReturnedObjects = pageIndex * pageSize;
                xpcUsers.TopReturnedObjects = pageSize;

                foreach (User xpoUser in xpcUsers)
                {
                    MembershipUser mUser = GetUserFromXpoUser(xpoUser);
                    mclUsers.Add(mUser);
                }
            }

            return mclUsers;
        }

        public override int GetNumberOfUsersOnline()
        {
            using (Session session = XpoHelper.GetNewSession())
            {
                XPCollection<User> xpcUsers = new XPCollection<User>(session,
                     new BinaryOperator("IsOnline", true, BinaryOperatorType.Equal));

                return xpcUsers.Count;
            }
        }

        public override string GetPassword(string username, string answer)
        {
            if (!EnablePasswordRetrieval)
            {
                throw new ProviderException("Password Retrieval Not Enabled.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Cannot retrieve Hashed passwords.");
            }

            string password;
            string passwordAnswer;

            using (Session session = XpoHelper.GetNewSession())
            {
                User user = session.FindObject<User>(
                    new BinaryOperator("UserName", username, BinaryOperatorType.Equal));

                if (user == null)
                {
                    throw new MembershipPasswordException("The specified user is not found.");
                }
                if (user.IsLockedOut)
                {
                    throw new MembershipPasswordException("The specified user is locked out.");
                }

                password = user.Password;
                passwordAnswer = user.PasswordAnswer;
            }

            if (RequiresQuestionAndAnswer && !LoginHelper.CheckPassword(answer, passwordAnswer))
            {
                UpdateFailureCount(username, FailureType.PasswordAnswer);

                throw new MembershipPasswordException("Incorrect password answer.");
            }

            if (PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                password = DecodePassword(password);
            }

            return password;
        }
        static ConcurrentDictionary<string, MembershipUser> membershipUserCache = new ConcurrentDictionary<string, MembershipUser>();
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (username == "superadmin")
            {
                return new MembershipUser(
                                this.Name,
                                "superadmin",
                                Guid.Parse("8F8556BD-4437-45E7-B539-5B90454484A6"),
                                "",
                                "",
                                "",
                                true,
                                false,
                                DateTime.Now,
                                DateTime.Now,
                                DateTime.Now,
                                DateTime.Now,
                                DateTime.MinValue
                                );
            }

            MembershipUser mUser;
            if(membershipUserCache.ContainsKey(username))
            {
                mUser = membershipUserCache[username];
                if(userIsOnline &&  DateTime.Now.Ticks - mUser.LastActivityDate.Ticks <= LoginHelper.UserActivityLogInterval)
                {
                    return mUser;
                }
            }

            using (UnitOfWork session = XpoHelper.GetNewUnitOfWork())
            {
                User user = session.FindObject<User>(
                            new BinaryOperator("UserName", username, BinaryOperatorType.Equal));

                if (user == null)
                    return null;

                mUser = GetUserFromXpoUser(user);

                try
                {
                    if (userIsOnline && DateTime.Now.Ticks - user.LastActivityDate.Ticks > LoginHelper.UserActivityLogInterval)
                    {
                        user.LastActivityDate = DateTime.Now;
                        user.Save();
                        session.CommitChanges();
                    }
                }
                catch(Exception ex)
                {
                    //Optimistic lock error
                    string errorMessage = ex.GetFullMessage();
                }
            }
            membershipUserCache[username]= mUser;
            return mUser;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            MembershipUser mUser;

            using (Session session = XpoHelper.GetNewSession())
            {
                User user = session.FindObject<User>(
                            new BinaryOperator("Oid", providerUserKey, BinaryOperatorType.Equal));

                if (user == null)
                    return null;

                mUser = GetUserFromXpoUser(user);

                if (userIsOnline)
                    user.LastActivityDate = DateTime.Now;

                user.Save();
            }

            return mUser;
        }

        public override string GetUserNameByEmail(string email)
        {
            using (Session session = XpoHelper.GetNewSession())
            {
                User user = session.FindObject<User>(
                            new BinaryOperator("Email", email, BinaryOperatorType.Equal));

                if (user == null)
                    return String.Empty;

                return user.UserName;
            }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotImplementedException("ResetPassword");
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException("UnlockUser");
        }

        /* Used by Web Site Administration Tool */

        public override void UpdateUser(MembershipUser mUser)
        {
            using (Session session = XpoHelper.GetNewSession())
            {
                User xpoUser = session.FindObject<User>(
                    new BinaryOperator("UserName", mUser.UserName, BinaryOperatorType.Equal));

                if (xpoUser == null)
                {
                    throw new ProviderException("The specified user is not found.");
                }

                xpoUser.Email = mUser.Email;
                xpoUser.Comment = mUser.Comment;
                xpoUser.IsApproved = mUser.IsApproved;
                xpoUser.LastLoginDate = mUser.LastLoginDate;
                xpoUser.LastActivityDate = mUser.LastActivityDate;

                xpoUser.Save();
            }
        }

        public override bool ValidateUser(string username, string password)
        {
            return LoginHelper.ValidateUser(username, password);
        }

        private void UpdateFailureCount(string username, FailureType failureType)
        {
            DateTime windowStart;
            DateTime windowEnd;
            int failureCount;

            using (Session session = XpoHelper.GetNewSession())
            {
                User user = session.FindObject<User>(new BinaryOperator("UserName", username));

                switch (failureType)
                {
                    case FailureType.Password:
                        failureCount = user.FailedPasswordAttemptCount;
                        windowStart = user.FailedPasswordAttemptWindowStart;
                        windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                        user.FailedPasswordAttemptWindowStart = DateTime.Now;

                        if (DateTime.Now > windowEnd)
                        {
                            user.FailedPasswordAttemptCount = 1;
                        }
                        else
                        {
                            user.FailedPasswordAttemptCount++;
                        }

                        if (user.FailedPasswordAttemptCount >= MaxInvalidPasswordAttempts)
                        {
                            if (!user.IsLockedOut)
                            {
                                user.LastLockedOutDate = DateTime.Now;
                                user.IsLockedOut = true;
                            }
                        }
                        break;
                    case FailureType.PasswordAnswer:
                        failureCount = user.FailedPasswordAnswerAttemptCount;
                        windowStart = user.FailedPasswordAnswerAttemptWindowStart;
                        windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

                        user.FailedPasswordAnswerAttemptWindowStart = DateTime.Now;

                        if (DateTime.Now > windowEnd)
                        {
                            user.FailedPasswordAnswerAttemptCount = 1;
                        }
                        else
                        {
                            user.FailedPasswordAnswerAttemptCount++;
                        }

                        if (user.FailedPasswordAnswerAttemptCount >= MaxInvalidPasswordAttempts)
                        {
                            if (!user.IsLockedOut)
                            {
                                user.LastLockedOutDate = DateTime.Now;
                                user.IsLockedOut = true;
                            }
                        }
                        break;
                }
                user.Save();
            }
        }

        private string DecodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            if (String.IsNullOrEmpty(password))
            {
                return password;
            }

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;

                case MembershipPasswordFormat.Encrypted:
                    password = Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;

                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot decode a hashed password.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        private static bool IsEmail(string inputEmail)
        {
            string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                  @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                  @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(inputEmail))
                return (true);
            else
                return (false);
        }

        private static byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            }
            return returnBytes;
        }

        private MembershipUser GetUserFromXpoUser(User xUser)
        {
            MembershipUser mUser = new MembershipUser(
                this.Name,
                xUser.UserName,
                xUser.Oid,
                xUser.Email,
                xUser.PasswordQuestion,
                xUser.Comment,
                xUser.IsApproved,
                xUser.IsLockedOut,
                xUser.CreatedOn,
                xUser.LastLoginDate,
                xUser.LastActivityDate,
                xUser.UpdatedOn,
                xUser.LastLockedOutDate
                );
            return mUser;
        }

        public override bool ConfirmAccount(string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override bool ConfirmAccount(string userName, string accountConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {
            throw new NotImplementedException();
        }

        public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values)
        {
            MembershipCreateStatus status;
            MembershipUser membershipUser = this.CreateUser(userName, password, "", "", "", true, null, out status);
            if (membershipUser == null)
            {
                return null;
            }
            return DateTime.Now.Ticks.ToString();
        }

        public override bool DeleteAccount(string userName)
        {
            throw new NotImplementedException();
        }

        public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow)
        {
            throw new NotImplementedException();
        }

        public override ICollection<OAuthAccountData> GetAccountsForUser(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreateDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastPasswordFailureDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetPasswordChangedDate(string userName)
        {
            throw new NotImplementedException();
        }

        public override int GetPasswordFailuresSinceLastSuccess(string userName)
        {
            throw new NotImplementedException();
        }

        public override int GetUserIdFromPasswordResetToken(string token)
        {
            throw new NotImplementedException();
        }

        public override bool IsConfirmed(string userName)
        {
            throw new NotImplementedException();
        }

        public override bool ResetPasswordWithToken(string token, string newPassword)
        {
            throw new NotImplementedException();
        }
    }
}