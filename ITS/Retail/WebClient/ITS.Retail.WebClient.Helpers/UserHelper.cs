using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Common;
using DevExpress.Data.Filtering;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Common.Helpers;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.WebClient.Helpers
{
	public class UserHelper
	{
        public static UserType GetUserType(User user)
        {
            if (user.Role.Type == eRoleType.Customer || user.Role.Type == eRoleType.Supplier)
            {
                return UserType.TRADER;
            }
            else if (user.Role.Type == eRoleType.CompanyUser)
            {
                return UserType.COMPANYUSER;
            }
            else if (user.Role.Type == eRoleType.CompanyAdministrator || user.Role.Type == eRoleType.SystemAdministrator)
            {
                return UserType.ADMIN;
            }
            return UserType.NONE;
        }


		public static IEnumerable<Store> GetStoresThatUserOwns(User user)
		{
            CompanyNew userSupplier = GetCompany(user);
            if (userSupplier == null)
            {
                return new List<Store>();
            }
            else
            {
                return BOApplicationHelper.GetUserEntities<Store>(user.Session, user).Where(g => g.Owner != null && g.Owner.Oid == userSupplier.Oid);
            }
		}


        public static IEnumerable<Store> GetStoresWhereUserBuysFrom(User user)
        {
            CompanyNew userSupplier = GetCompany(user);
            if (userSupplier == null)
            {
                return BOApplicationHelper.GetUserEntities<Store>(user.Session, user);
            }
            else
            {
                return BOApplicationHelper.GetUserEntities<Store>(user.Session, user).Where(g => g.Owner.Oid != userSupplier.Oid);
            }
        }

        /// <summary>
        /// Checks if the posted password satisfies certain conditions 
        /// </summary>
        /// <param name="pass">The posted password</param>
        /// <returns>True if password satisfies the conditions, false otherwise</returns>
        public static bool CheckPasswordStrength(string pass)
		{
            if (pass.Length < 4) //Password length 8 or more
            {
                return false;
            }

            //if (!(Regex.Match(pass, @"\d", RegexOptions.ECMAScript).Success)) // password should contain at least one number
            //{
            //    return false;
            //}

            //if (Regex.Match(pass, @"[a-z]", RegexOptions.ECMAScript).Success && Regex.Match(pass, @"[A-Z]", RegexOptions.ECMAScript).Success)
            //{
            //    return true;
            //}
                return true;
            }

		/// <summary>
        /// Calculates a List of CriteriaOperators like (propertyName = priceCatalogOid) where priceCatalogOid = { all user Suppliers PriceCatalogs }
		/// </summary>
		/// <param name="user"></param>
        /// <param name="propertyName"></param>
        /// <returns>A List of CriteriaOperators like (propertyName = priceCatalogOid) where priceCatalogOid = { all user Suppliers PriceCatalogs }</returns>
		[System.Obsolete("PriceCatalogs now have owners")]
		public static List<CriteriaOperator> GetUserSupplierPriceCatalogsFilter(User user, string propertyName = "Oid")
		{
			List<CriteriaOperator> priceCatalogsFilterList = new List<CriteriaOperator>();
			XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(user.Session, user);
			List<Guid> oidList = new List<Guid>();
			foreach (CompanyNew supl in userSuppliers)
			{
				Queue<PriceCatalog> pcQueue = new Queue<PriceCatalog>();
				XPCollection<PriceCatalog> pcCollection = new XPCollection<PriceCatalog>(user.Session, new BinaryOperator("Owner.Oid", supl.Oid));
				foreach (PriceCatalog pc in pcCollection)
				{
					pcQueue.Enqueue(pc);
				}

				while (pcQueue.Count != 0)
				{
					PriceCatalog currPC = pcQueue.Dequeue();
					if (!oidList.Contains(currPC.Oid))
					{
						oidList.Add(currPC.Oid);
					}
					if (currPC.ParentCatalog != null && !oidList.Contains(currPC.ParentCatalog.Oid))
					{
						pcQueue.Enqueue(currPC.ParentCatalog);
					}

				}
			}
			foreach (Guid catalogID in oidList)
			{
				priceCatalogsFilterList.Add(new BinaryOperator(propertyName, catalogID, BinaryOperatorType.Equal));
			}

			return priceCatalogsFilterList;
		}

		/// <summary>
		/// Υπολογίζει και επιστρέφει μία List από CriteriaOperators της μορφής (propertyName = priceCatalogOid) όπου priceCatalogOid = { όλοι οι τιμοκατάλογοι από τους οποίους ψωνίζει ο Customer}
		/// </summary>
        /// <param name="customer"></param>
        /// <param name="store"></param>
        /// <param name="propertyName"></param>
		/// <returns></returns>
		public static List<CriteriaOperator> GetUserCustomerPriceCatalogsFilter(Customer customer, Store store,string propertyName = "Oid")
		{
			List<CriteriaOperator> priceCatalogsFilterList = new List<CriteriaOperator>();

            if (customer == null || store == null)
            {
                return priceCatalogsFilterList;//TO CHECK
            }

			//List<Guid> oidList = new List<Guid>();
			//Queue<PriceCatalog> pcQueue = new Queue<PriceCatalog>();

   //         PriceCatalog custPc = customer.GetPriceCatalog(store);

   //         if(custPc==null)
   //         {
   //             return priceCatalogsFilterList;
   //         }

			//pcQueue.Enqueue(custPc);
			//while (pcQueue.Count != 0)
			//{
			//	PriceCatalog currPC = pcQueue.Dequeue();
   //             if (currPC != null)
   //             {
   //                 if (!oidList.Contains(currPC.Oid))
   //                 {
   //                     oidList.Add(currPC.Oid);
   //                 }
   //                 if (currPC.ParentCatalog != null && !oidList.Contains(currPC.ParentCatalog.Oid))
   //                 {
   //                     pcQueue.Enqueue(currPC.ParentCatalog);
   //                 }
   //             }
			//}

            foreach (Guid catalogID in PriceCatalogHelper.GetPriceCatalogsFromPolicy(new EffectivePriceCatalogPolicy(store, customer)))
			{
				priceCatalogsFilterList.Add(new BinaryOperator(propertyName, catalogID));
			}
			return priceCatalogsFilterList;
		}


		/// <summary>
		/// Υπολογίζει και επιστρέφει μία List από CriteriaOperators της μορφής (propertyName = priceCatalogOid) όπου priceCatalogOid = { όλοι οι τιμοκατάλογοι από τους οποίους ψωνίζουν οι Customers του user }
		/// </summary>
		/// <param name="user"></param>
        /// <param name="store"></param>
        /// <param name="propertyName"></param>
		/// <returns></returns>
		public static List<CriteriaOperator> GetUserCustomerPriceCatalogsFilter(User user, Store store,string propertyName = "Oid")
		{

			List<CriteriaOperator> priceCatalogsFilterList = new List<CriteriaOperator>();
            XPCollection<Customer> userCustomers = BOApplicationHelper.GetUserEntities<Customer>(user.Session, user);
			List<Guid> oidList = new List<Guid>();
			foreach (Customer cust in userCustomers)
			{
				//Queue<PriceCatalog> pcQueue = new Queue<PriceCatalog>();
    //            PriceCatalog custPc = cust.GetPriceCatalog(store);
				//pcQueue.Enqueue(custPc);
				//while (pcQueue.Count != 0)
				//{
				//	PriceCatalog currPC = pcQueue.Dequeue();
				//	if (!oidList.Contains(currPC.Oid))
				//	{
				//		oidList.Add(currPC.Oid);
				//	}
				//	if (currPC.ParentCatalog != null && !oidList.Contains(currPC.ParentCatalog.Oid))
				//	{
				//		pcQueue.Enqueue(currPC.ParentCatalog);
				//	}

				//}
                foreach (Guid catalogID in PriceCatalogHelper.GetPriceCatalogsFromPolicy(new EffectivePriceCatalogPolicy(store, cust)))
                {
                    priceCatalogsFilterList.Add(new BinaryOperator(propertyName, catalogID));
                }
            }


            return priceCatalogsFilterList;
		}

		/// <summary>
		/// Returns PriceCatalogs of current user
		/// </summary>
		/// <param name="user">Current user</param>
		/// <returns>A XPCollection of PriceCatalogs</returns>
		public static XPCollection<PriceCatalog> GetUserCustomerPriceCatalogs(User user, Store store)
		{
			List<CriteriaOperator> priceCatalogsFilterList = GetUserCustomerPriceCatalogsFilter(user,store);
			return new XPCollection<PriceCatalog>(user.Session, CriteriaOperator.Or(priceCatalogsFilterList));

		}

		/// <summary>
        /// Calculates a List of CriteriaOperators like (propertyName = supplierOid) where supplierOid = { all user Suppliers }
		/// </summary>
		/// <param name="user"></param>
        /// <param name="propertyName"></param>
        /// <returns>A List of CriteriaOperators like (propertyName = supplierOid) where supplierOid = { all user Suppliers }</returns>
        public static List<CriteriaOperator> GetUserSupplierStoresFilter(User user, string propertyName = "Supplier.Oid")
        {
            List<CriteriaOperator> storesFilterList = new List<CriteriaOperator>();
            XPCollection<CompanyNew> userSuppliers = BOApplicationHelper.GetUserEntities<CompanyNew>(user.Session, user);
            foreach (CompanyNew supl in userSuppliers)
            {
                storesFilterList.Add(new BinaryOperator(propertyName, supl.Oid, BinaryOperatorType.Equal));
            }

            return storesFilterList;
        }

        /// <summary>
        /// Checks if user is Customer
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Boolean</returns>
        public static bool IsCustomer(User user)
		{
            if(user==null)
            {
                return false;
            }
            if(user.Role.Type != eRoleType.Customer)
            {
                return false;
            }
            return true;
		}

        /// <summary>
        /// Checks if user is Supplier
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Boolean</returns>
        public static bool IsSupplier(User user)
        {
            if (user == null)
            {
                return false;
            }
            if (user.Role.Type != eRoleType.Supplier)
            {
                return false;
            }
            return true;
		}

        /// <summary>
        /// Checks if user is Supplier or Customer
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Boolean</returns>
        public static bool IsTrader(User user)
        {
            return IsCustomer(user) || IsSupplier(user);
        }

        /// <summary>
        /// Checks if user needs POS username and password
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Boolean</returns>
        public static bool UsesPOSCredentials(User user)
        {
            return IsCompanyUser(user) || IsCompanyAdmin(user);
        }

        /// <summary>
        /// Returns the Customer object which is connected to current user
        /// </summary>
        /// <param name="user">Current User</param>
        /// <param name="uow">Current Session</param>
        /// <returns>Customer</returns>
        public static Customer GetCustomer(User user,UnitOfWork uow )
        {
            Customer customer = BOApplicationHelper.GetUserEntities<Customer>(uow, user).FirstOrDefault();
            if (customer == null)
            {
                return null;
            }
            return uow.GetObjectByKey<Customer>(customer.Oid);
        }

		/// <summary>
		/// Checks if user is Company User
		/// </summary>
		/// <param name="user"></param>
		/// <returns>Boolean</returns>
		public static bool IsCompanyUser(User user)
		{
            if (user == null)
            {
                return false;
            }
            if (user.Role.Type != eRoleType.CompanyUser)
            {
                return false;
            }
            return true;
		}

        public static bool RenderOwnerCombobox(User user){
            return IsSystemAdmin(user);
        }

        /// <summary>
        /// Checks if user is System Administrator 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Boolean</returns>
        public static bool IsSystemAdmin(User user)
        {
            if (user == null)
            {
                return false;
            }
            if (user.Role.Type != eRoleType.SystemAdministrator)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if user is Company Administrator 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Boolean</returns>
        public static bool IsCompanyAdmin(User user)
        {
            if (user == null)
            {
                return false;
            }
            if (user.Role.Type != eRoleType.CompanyAdministrator)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  Returns Supplier assigned to specific User
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Supplier</returns>
        public static CompanyNew GetCompany(User user)
		{
            XPCollection<CompanyNew> sup = BOApplicationHelper.GetUserEntities<CompanyNew>(user.Session, user);
			return sup.FirstOrDefault();
		}


		/// <summary>
		/// Returns Customer assigned to specific User
		/// </summary>
		/// <param name="user"></param>
		/// <returns>Customer</returns>
		public static Customer GetCustomer(User user)
		{
            XPCollection<Customer> sup = BOApplicationHelper.GetUserEntities<Customer>(user.Session, user);
			return sup.FirstOrDefault();
		}
		public static string EncodePassword(string originalPassword)
		{
            //Declarations
            byte[] originalBytes;
            byte[] encodedBytes;
            byte[] doubleEncodedBytes;
			MD5 md5;

			//Instantiate MD5CryptoServiceProvider, get bytes for original password and compute hash (encoded password)
			using (md5 = new MD5CryptoServiceProvider())
			{
				originalBytes = Encoding.Default.GetBytes(originalPassword);
				encodedBytes = md5.ComputeHash(originalBytes);
			}

			using (SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider())
			{
				doubleEncodedBytes = sha1.ComputeHash(encodedBytes);
			}
			return BitConverter.ToString(doubleEncodedBytes);
		}

		public static List<CriteriaOperator> GetUserSupplierVisiblePriceCatalogsFilter(User user, string propertyName = "Oid")
		{
			List<CriteriaOperator> filters = new List<CriteriaOperator>();
            XPCollection<Store> userStores = BOApplicationHelper.GetUserEntities<Store>(user.Session, user);

			foreach (Store store in userStores)
			{
				foreach (StorePriceList storePriceList in store.StorePriceLists)
				{
					filters.Add(new BinaryOperator(propertyName, storePriceList.PriceList.Oid, BinaryOperatorType.Equal));
				}
			}

			return filters;
		}

		public static ePermition GetUserEntityPermition(User user, string EntityType)
		{
			ePermition permition = new ePermition();

			if (user.Role == null)
			{
				return permition;
			}
			EntityAccessPermision eat = user.Session.FindObject<EntityAccessPermision>(CriteriaOperator.And(new BinaryOperator("EntityType", EntityType),
										 new ContainsOperator("RoleEntityAccessPermisions", new BinaryOperator("Role.Oid", user.Role.Oid))));

			if (eat == null)
			{
				permition = (ePermition.Visible | ePermition.Insert | ePermition.Update | ePermition.Delete);
			}
			else
			{
				if (eat.CanDelete)
				{
					permition = permition | ePermition.Delete;
				}
				if (eat.CanInsert)
				{
					permition = permition | ePermition.Insert;
				}
				if (eat.CanUpdate)
				{
					permition = permition | ePermition.Update;
				}
				if (eat.Visible)
				{
					permition = permition | ePermition.Visible;
				}
			}

			return permition;
		}

		/// <summary>
		/// Υπολογίζει και επιστρέφει μία List από CriteriaOperators της μορφής (propertyName = priceCatalogOid) 
		/// όπου priceCatalogOid = { όλοι οι τιμοκατάλογοι των καταστημάτων που έχει πρόσβαση ο χρήστης, αναδρομικά πρός τα πάνω}
		/// </summary>
		/// <param name="user"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		public static List<CriteriaOperator> GetUserSupplierStorePriceCatalogsFilter(User user, string propertyName = "Oid")
		{
			List<CriteriaOperator> filters = new List<CriteriaOperator>();
            XPCollection<Store> userStores = BOApplicationHelper.GetUserEntities<Store>(user.Session, user);

			Dictionary<Guid, PriceCatalog> dict = new Dictionary<Guid, PriceCatalog>();

			foreach (Store store in userStores)
			{
				foreach (StorePriceList storePriceList in store.StorePriceLists)
				{
					if (storePriceList.PriceList != null && !dict.ContainsKey(storePriceList.PriceList.Oid))
					{
						dict[storePriceList.PriceList.Oid] = storePriceList.PriceList;
					}
				}
			}

			int i;
			Dictionary<Guid, PriceCatalog> finalDict = null;
			do
			{
				i = 0;
				finalDict = new Dictionary<Guid, PriceCatalog>(dict);
				foreach (KeyValuePair<Guid, PriceCatalog> pair in dict)
				{
					if (pair.Value.ParentCatalog != null && !finalDict.ContainsKey(pair.Value.ParentCatalog.Oid))
					{
						finalDict[pair.Value.ParentCatalog.Oid] = pair.Value.ParentCatalog;
						i++;
					}
				}
				if (i > 0)
				{
					dict = new Dictionary<Guid, PriceCatalog>(finalDict);
				}
			} while (i > 0);

			foreach (KeyValuePair<Guid, PriceCatalog> pair in finalDict)
			{
				filters.Add(new BinaryOperator(propertyName, pair.Key, BinaryOperatorType.Equal));
			}

			return filters;
		}

        /// <summary>
        /// Checks if user is System Admin or Company Admin
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static bool IsAdmin(User user)
        {
            return IsSystemAdmin(user) || IsCompanyAdmin(user);
        }


        public static T GenerateUserToken<T>(User user) where T : Token
        {   
            try
            {
                CriteriaOperator criteria = new BinaryOperator("User", user.Oid);
                XPCollection<T> existingTokens = new XPCollection<T>(user.Session, criteria);
                if (existingTokens.Count > 0)
                {
                    user.Session.Delete(existingTokens);
                }
                T token = (T)Activator.CreateInstance(typeof(T),new object[] {user.Session });
                token.User = user;
                return token;
            }
            catch(Exception exception)
            {
                string errorMessage = exception.GetFullMessage();
                return null;
            }
        }

        /// <summary>
        /// Generates a new random encoded password
        /// </summary>
        /// <returns>Returns a new random encoded password</returns>
        public static string GeneratePassword(out string passphrase)
        {
            int characters = 8;
            string newPassword = Guid.NewGuid().ToString();
            newPassword = newPassword.Substring(0, characters);
            passphrase = newPassword;
            newPassword = EncodePassword(newPassword);
            return newPassword;
        }

        /// <summary>
        /// Checks if user has access to current store in StoreController or Dual mode, or to any stores in Retail mode
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="applicationInstance">The application instance</param>
        /// <param name="storeOid">The cuurrent store in case of StoreController or Dual mode</param>
        /// <param name="message">The message to be returned in case of error</param>
        /// <returns></returns>
        public static bool UserCanLoginToCurrentStore(User user, eApplicationInstance applicationInstance, Guid storeOid, out string message)
        {
            message = "";
            if (IsSystemAdmin(user) || IsCompanyAdmin(user))
            {
                return true;
            }
            else if (applicationInstance == eApplicationInstance.RETAIL)
            {
                message = Resources.NoStoreAccess;
                return BOApplicationHelper.GetUserEntities<Store>(user.Session, user).Count() > 0;
            }
            else
            {
                message = Resources.YouDoNotHaveAccessToThisStore;
                return !IsCustomer(user) && BOApplicationHelper.GetEntityCollectionUsers(user.Session, new List<Guid>() {storeOid}).Contains(user);
            }   
        }


        //public static int UsersCountForLicense(UnitOfWork session)
        //{
        //    CriteriaOperator licenseUsersCriteria = CriteriaOperator.And(new BinaryOperator("IsActive",true),
        //                                                                 new BinaryOperator("IsB2CUser",false));
        //    int usersCount = (int)session.Evaluate<User>(CriteriaOperator.Parse("Count"), licenseUsersCriteria);
        //    return usersCount;
        //}
    }
}