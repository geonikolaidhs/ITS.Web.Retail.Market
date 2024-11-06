using ITS.Retail.Model;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Linq;
using System.Security.Claims;


using System.Threading.Tasks;
using ITS.Retail.Api.Authentication;
using System.Net;

using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Api.Providers;
using System.Web.Http.ModelBinding;
using System.Web.Http;
using System.Collections.Generic;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using ITS.Retail.Common;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.Retail.Api.Models;
using Newtonsoft.Json;
using ITS.Retail.Api.ViewModel;

namespace ITS.Retail.Api.Controllers
{

    [RoutePrefix("api/Custom")]
    [EnableQuery(MaxExpansionDepth = 4)]
    [CustomAuthorize]
    public class CustomController : ApiController
    {
        //UpdaterServiceHelper UpdaterServiceHelper = null;
        //SyncCriteriaHelper SyncCriteriaHelper = null;
        public CustomController(IWRMUserDbModule wrmDbModule) : base()
        {
            this.wrmUserDbModule = wrmDbModule;
        }
        /// <summary>
        /// Access to Database
        /// </summary>
        protected IWRMUserDbModule wrmUserDbModule
        {
            get;
            private set;
        }

        /// <summary>
        /// The Currently Loggedin User
        /// </summary>
        protected IUser CurrentUser { get; set; }



        internal void AfterAuthorization()
        {
            Claim claim = (this.User as System.Security.Claims.ClaimsPrincipal).Claims.FirstOrDefault(x => x.Type == "AspNet.Identity.SecurityStamp");
            CurrentUser = wrmUserDbModule.Query<User>().FirstOrDefault(x => x.AuthToken == claim.Value && x.UserName == this.User.Identity.Name);
        }


        [AcceptVerbs("GET")]
        [Route("GetStockInitialiseDocuments/{sfaOid}/{fromDate}/{toDate}/{storeOid}/{version}")]
        public async Task<IHttpActionResult> GetStockInitialiseDocuments(string sfaOid, long fromDate, long toDate, string storeOid, long version = 0)
        {
            string json = string.Empty;
            List<StockDocumentHeader> stockHeaders = new List<StockDocumentHeader>();
            try
            {
                Guid Oid;
                Guid storeId;
                if (Guid.TryParse(sfaOid, out Oid) && Guid.TryParse(storeOid, out storeId))
                {
                    List<DocumentHeader> headers = new List<DocumentHeader>();
                    CriteriaOperator crit = CriteriaOperator.And(
                        new BinaryOperator("Tablet.Oid", Oid, BinaryOperatorType.Equal),
                        new BinaryOperator("Status.TakeSequence", true, BinaryOperatorType.Equal),
                        new BinaryOperator("IsCanceled", false, BinaryOperatorType.Equal),
                        new BinaryOperator("Store.Oid", storeOid, BinaryOperatorType.Equal),
                        new BinaryOperator("IsCancelingAnotherDocument", false, BinaryOperatorType.Equal),
                        new BinaryOperator("UpdatedOnTicks", fromDate, BinaryOperatorType.GreaterOrEqual),
                        new BinaryOperator("UpdatedOnTicks", toDate, BinaryOperatorType.LessOrEqual),
                        new BinaryOperator("UpdatedOnTicks", version, BinaryOperatorType.Greater),
                        new BinaryOperator("DocumentType.ItemStockAffectionOptions", ItemStockAffectionOptions.INITIALISES));
                    using (UnitOfWork uow = wrmUserDbModule.GetUnitOfWork())
                    {
                        await Task.Run(() =>
                        {
                            headers = new XPCollection<DocumentHeader>(uow, crit).ToList();
                        });
                        headers.ForEach(x => stockHeaders.Add(new StockDocumentHeader(x)));
                    }
                }
            }
            catch (Exception ex)
            {
                return Ok<string>("error " + ex.Message);
            }
            return Ok<List<StockDocumentHeader>>(stockHeaders);
        }

        [AcceptVerbs("GET")]
        [Route("GetStockInitialiseDocuments/{type}/{series}/{store}/{fromDate}/{toDate}/{version}")]
        public async Task<IHttpActionResult> GetStockInitialiseDocuments(string type, string series, string store, long fromDate, long toDate, long version = 0)
        {
            List<StockDocumentHeader> stockHeaders = new List<StockDocumentHeader>();
            try
            {
                Guid serieOid;
                Guid.TryParse(series, out serieOid);
                Guid typeOid;
                Guid.TryParse(type, out typeOid);
                Guid storeOid;
                Guid.TryParse(store, out storeOid);
                List<DocumentHeader> headers = new List<DocumentHeader>();
                CriteriaOperator crit = CriteriaOperator.And(
                    new BinaryOperator("DocumentType.Oid", typeOid, BinaryOperatorType.Equal),
                    new BinaryOperator("DocumentSeries.Oid", serieOid, BinaryOperatorType.Equal),
                    new BinaryOperator("Status.TakeSequence", true, BinaryOperatorType.Equal),
                    new BinaryOperator("IsCanceled", false, BinaryOperatorType.Equal),
                    new BinaryOperator("IsCancelingAnotherDocument", false, BinaryOperatorType.Equal),
                    new BinaryOperator("UpdatedOnTicks", fromDate, BinaryOperatorType.GreaterOrEqual),
                    new BinaryOperator("UpdatedOnTicks", toDate, BinaryOperatorType.LessOrEqual),
                    new BinaryOperator("UpdatedOnTicks", version, BinaryOperatorType.Greater),
                    new BinaryOperator("DocumentType.ItemStockAffectionOptions", ItemStockAffectionOptions.INITIALISES, BinaryOperatorType.Equal));
                using (UnitOfWork uow = wrmUserDbModule.GetUnitOfWork())
                {
                    await Task.Run(() =>
                    {
                        headers = new XPCollection<DocumentHeader>(uow, crit).ToList();
                    });
                    headers.ForEach(x => stockHeaders.Add(new StockDocumentHeader(x)));
                }
            }
            catch (Exception ex)
            {
                return Ok<string>("error " + ex.Message);
            }
            return Ok<List<StockDocumentHeader>>(stockHeaders);
        }



        [AcceptVerbs("GET")]
        [Route("ConfirmReceivedDocument/{Oid}")]
        public async Task<IHttpActionResult> ConfirmReceivedDocument(string Oid)
        {
            Guid documentOid;
            Guid.TryParse(Oid, out documentOid);
            ReceivedDocument receivedDocument = new ReceivedDocument();
            try
            {
                DocumentHeader header = null;
                using (UnitOfWork uow = wrmUserDbModule.GetUnitOfWork())
                {
                    await Task.Run(() =>
                    {
                        CriteriaOperator crit = CriteriaOperator.And(new BinaryOperator("Oid", documentOid, BinaryOperatorType.Equal));
                        header = uow.FindObject<DocumentHeader>(crit);
                    });
                }
                receivedDocument.Oid = header?.Oid ?? Guid.Empty;
                receivedDocument.Number = header?.DocumentNumber ?? -1;
            }
            catch (Exception ex)
            {
                return Ok<string>("error " + ex.Message);
            }
            return Ok<ReceivedDocument>(receivedDocument);
        }

    }
}
