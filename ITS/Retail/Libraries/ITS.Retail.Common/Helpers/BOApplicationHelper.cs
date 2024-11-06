using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;
using System.IO;
using System.Threading;

namespace ITS.Retail.Common.Helpers
{
    public static class BOApplicationHelper
    {
        /// <summary>
        /// Returns all objects of type T assigned to userr  
        /// </summary>
        /// <typeparam name="T">Type of objects</typeparam> 
        /// <param name="session">Current Session</param>
        /// <param name="user">Current user</param>
        /// <returns>A XPCollection of type T objects</returns>
        public static XPCollection<T> GetUserEntities<T>(Session session, User user = null)
        {
            IQueryable<UserTypeAccess> uta = new XPQuery<UserTypeAccess>(session);
            if (user != null)
            {
                uta = uta.Where(ut => ut.User.Oid == user.Oid);
            }
            uta = uta.Where(ut => ut.EntityType == typeof(T).ToString());
            
            List<Guid> guids = uta.Select(ut => ut.EntityOid).ToList();

            return new XPCollection<T>(session, new InOperator("Oid", guids));


        }


        /// <summary>
        /// Returns a collection of users that are connected to any of given entities 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entityCollectionOid"></param>
        /// <returns></returns>
        public static XPCollection<User> GetEntityCollectionUsers(Session session, IEnumerable<Guid> entityCollectionOid)
        {
            CriteriaOperator crop;
            entityCollectionOid.DefaultIfEmpty(Guid.Empty);
            if(entityCollectionOid.Count() == 1)
            {
                crop = new BinaryOperator("EntityOid", entityCollectionOid.First(), BinaryOperatorType.Equal);
            }
            else
            {
                crop = new InOperator("EntityOid", entityCollectionOid);
            }
            XPCollection<UserTypeAccess> uta = new XPCollection<UserTypeAccess>(session, crop, new SortProperty("Oid", DevExpress.Xpo.DB.SortingDirection.Ascending));
            var userIDs = from obj in uta
                          select obj.User.Oid;
            return new XPCollection<User>(session, new InOperator("Oid", userIDs.ToList()), new SortProperty("UserName", DevExpress.Xpo.DB.SortingDirection.Ascending));

        }
    }
}
