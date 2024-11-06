using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;

namespace ITS.Retail.Common.Helpers
{
    public class BOUserHelper
    {
        private User _User;
        private Session _Session;


        public BOUserHelper()
        {
        }

        public BOUserHelper(Session session, User user)
        {
            _Session = session;
            _User = new User(session);
            _User.GetData(user);
            _User.Oid = user.Oid;
        }

        // Αν γυρίσει null επιτρέπεται να έχει access σ' όλα
        public EntityAccessPermision GetEntityPermission(User user, Type entityType)
        {
            string type=entityType.ToString();
            if (user == null || user.Role == null || user.Role.Oid == Guid.Empty ) return null;
            CriteriaOperator crop = CriteriaOperator.And(new BinaryOperator("Role.Oid", user.Role.Oid, BinaryOperatorType.Equal), new BinaryOperator("EntityType", type, BinaryOperatorType.Equal));
            RoleEntityAccessPermision foundIt = user.Session.FindObject<RoleEntityAccessPermision>(crop);
            return foundIt.EnityAccessPermision;
        }

        // Επιστρέφει τον πρώτο Trader που ανήκει ο User
        public Trader GetTrader(User user)
        {
            Store store = BOApplicationHelper.GetUserEntities<Store>(user.Session, user).FirstOrDefault<Store>();
            return store == null ? null : store.Address.Trader;
        }
    }
}
