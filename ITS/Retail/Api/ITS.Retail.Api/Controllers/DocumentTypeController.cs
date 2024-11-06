using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Api.Providers;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Validations;
using ITS.Retail.WRM.Kernel.Interface;
using Microsoft.AspNet.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace ITS.Retail.Api.Controllers
{
    //[RoutePrefix("api/DocumentTypeSFA")]
    public class DocumentTypeController : BaseODataController<DocumentType>
    {
        public DocumentTypeController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule)
        {

        }



        /// <summary>
        /// Επιστρέφει τα παραστατικά και τις σειρές τους που έχουν flag οτι είναι για φορητό.
        /// </summary>
        /// <param name="storeOid"></param> 
        /// <returns></returns>
        private List<DocumentType> GetValidDocTypes(Guid storeOid)
        {
            List<DocumentType> documentTypes = new List<DocumentType>();
            try
            {
                using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                {

                    Store store = uow.GetObjectByKey<Store>(storeOid);
                    CriteriaOperator seriesCrit = CriteriaOperator.And(new BinaryOperator("Store", store, BinaryOperatorType.Equal), new BinaryOperator("IsCancelingSeries", false, BinaryOperatorType.Equal),
                                                                      new BinaryOperator("eModule", eModule.SFA, BinaryOperatorType.Equal));

                    XPCollection<DocumentSeries> docSeries = new XPCollection<DocumentSeries>(uow, seriesCrit);
                    List<StoreDocumentSeriesType> storeDocSeries = new XPCollection<StoreDocumentSeriesType>(uow, CriteriaOperator.And(new InOperator("DocumentSeries", docSeries))).ToList();
                    documentTypes = storeDocSeries.Select(x => x.DocumentType).ToList();
                    List<DocumentType> loadingDocs = new XPCollection<DocumentType>(uow, CriteriaOperator.And(new BinaryOperator("ItemStockAffectionOptions", ItemStockAffectionOptions.INITIALISES, BinaryOperatorType.Equal))).ToList();
                    documentTypes.AddRange(loadingDocs);
                    return documentTypes;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return documentTypes;
        }


        /// <summary>
        /// Επιστρέφει τον Retail User με βάση το Oid του
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="Oid"></param>
        /// <returns></returns>
        private ITS.Retail.Model.User GetUser(UnitOfWork uow, Guid Oid)
        {
            return uow.GetObjectByKey<ITS.Retail.Model.User>(Oid);
        }
        /// <summary>
        /// Επιστρέφει τον Retail User με βάση το Oid του και ελέγχει αν έχει δικαιώματα στο συγκεκριμένο κατάστημα
        /// </summary>
        /// <param name="uow"></param>
        /// <param name="Oid"></param>
        /// <param name="store"></param>
        /// <returns></returns>
        private ITS.Retail.Model.User GetUserForStore(UnitOfWork uow, Guid Oid, Store store)
        {
            ITS.Retail.Model.User user = uow.GetObjectByKey<ITS.Retail.Model.User>(Oid);
            if (user != null)
            {
                CriteriaOperator crit = CriteriaOperator.And(new BinaryOperator("User", user), new BinaryOperator("EntityType", typeof(Store).FullName), (new BinaryOperator("EntityOid", store.Oid)));
                UserTypeAccess access = uow.FindObject<UserTypeAccess>(crit);
                if (access != null)
                {
                    return user;
                }
            }
            return null;
        }


    }
}
