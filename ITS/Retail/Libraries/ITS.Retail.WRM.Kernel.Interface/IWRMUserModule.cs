using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.WRM.Kernel.Interface
{
    public interface IWRMUserModule : IKernelModule, IDisposable
    {
        UserType GetUserType(IUser user);
        IEnumerable<IStore> GetStoresThatUserOwns(IUser user);
        IEnumerable<IStore> GetStoresWhereUserBuysFrom(IUser user);

        IEnumerable<ICompany> GetUserCompanies(IUser user);



        bool CheckPasswordStrength(string password);

        bool IsSystemAdmin(IUser user);

        ICompany GetCompany(IUser user);

        ICustomer GetCustomer(IUser user);
        string EncodePassword(string originalPassword);

        ePermition GetUserEntityPermition(IUser user, string EntityType);
        bool IsAdmin(IUser user);

        string GeneratePassword(out string passphrase);

        bool UserCanLoginToCurrentStore(IUser user, eApplicationInstance applicationInstance, Guid storeOid, out string message);


    }
}
