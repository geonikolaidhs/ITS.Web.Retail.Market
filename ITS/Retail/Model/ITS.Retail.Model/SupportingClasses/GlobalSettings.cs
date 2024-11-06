using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    public static class GlobalSettings
    {
        private static Session _Session;
        public static Session Session
        {
            get { return _Session; }
            set
            {
                _Session = value;
            }
        }
        

        private static User _User;
        public static User User
        {
            get { return _User; }
            set
            {
                _User = value;
            }
        }

        private static eDivision _Division;
        public static eDivision Division
        {
            get { return _Division; }
            set
            {
                _Division = value;
            }
        }

        private static Store _Store;
        public static Store Store
        {
            get { return _Store; }
            set
            {
                _Store = value;
            }
        }



        //public static GlobalSettings(Session session, User user, eDivision division, Store store)
        //{
        //    Session = session;
        //    user.GetData(session, user);
        //    Division = division;
        //    Store.GetData(session,store);
        //}

        
    }
}
