using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Api.Providers;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Validations;
using ITS.Retail.WRM.Kernel.Interface;
using Microsoft.AspNet.OData;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace ITS.Retail.Api.Controllers
{

    public class DocumentHeaderController : BaseODataController<DocumentHeader>
    {
        public DocumentHeaderController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }

        public override async Task<IHttpActionResult> Post([ModelBinder(BinderType = typeof(WrmModelBinder))]DocumentHeader requestModel)
        {

            using (UnitOfWork uow = wrmUserDbModule.GetUnitOfWork())
            {
                try
                {

                    if (wrmUserDbModule.Access<DocumentHeader>(null, this.CurrentUser).HasFlag(ePermition.Insert) == false)
                    {
                        return Unauthorized();
                    }

                    DocumentHeader dbObject = null;
                    await Task.Run(() =>
                    {
                        dbObject = uow.GetObjectByKey<DocumentHeader>(requestModel.Oid);
                    });

                    if (dbObject != null)
                    {
                        return BadRequest("Duplicate Primary Key");
                    }
                    string headerToJson = requestModel.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS, true);
                    JObject jsonObj = JObject.Parse(headerToJson);
                    string error = string.Empty;
                    dbObject = new DocumentHeader(uow);
                    dbObject.FromJson(jsonObj, PlatformConstants.JSON_SERIALIZER_SETTINGS, true, false, out error);

                    if (error != string.Empty)
                    {
                        return BadRequest(error);
                    }

                    dbObject.Oid = requestModel.Oid;
                    DocumentStatus status = uow.GetObjectByKey<DocumentStatus>(dbObject.Status.Oid);
                    if (status == null)
                    {
                        throw new Exception("There is a problem with your data concerning Store's Document Status! ");
                    }

                    DocumentSeries series = uow.GetObjectByKey<DocumentSeries>(dbObject.DocumentSeries.Oid);
                    if (series == null)
                    {
                        throw new Exception("There is a problem with your data concerning Store's Document Series!");
                    }
                    DocumentType type = uow.GetObjectByKey<DocumentType>(dbObject.DocumentType.Oid);
                    if (type == null)
                    {
                        throw new Exception("There is a problem with your data concerning Store's Document Type!");
                    }

                    CriteriaOperator crop = CriteriaOperator.And(
                                                            new BinaryOperator("DocumentSeries", series, BinaryOperatorType.Equal),
                                                            new BinaryOperator("DocumentType", type, BinaryOperatorType.Equal));
                    XPCollection<StoreDocumentSeriesType> stst = new XPCollection<StoreDocumentSeriesType>(uow, crop);
                    DocumentSequence seq = null;
                    if (stst == null || stst.Count < 1)
                    {
                        throw new Exception("There is a problem with your data concerning Store's Document Type, Document Series and Document Sequence!");
                    }
                    if (status.TakeSequence)
                    {
                        List<DocumentSequence> allSequences = new XPCollection<DocumentSequence>(uow).ToList();
                        foreach (StoreDocumentSeriesType sd in stst)
                        {
                            if (sd.DocumentSeries.Store.Oid == dbObject.Store.Oid && sd.DocumentSeries.HasAutomaticNumbering && series.Oid == sd.DocumentSeries.Oid)
                            {
                                seq = allSequences.Where(x => x.DocumentSeries.Oid == sd.DocumentSeries.Oid).FirstOrDefault();
                                if (seq == null)
                                {
                                    seq = uow.GetObjectByKey<DocumentSequence>(sd.DocumentSeries.DocumentSequence?.Oid ?? Guid.Empty);
                                }
                                if (seq != null)
                                    break;
                            }
                        }
                        bool isNewSequence = false;
                        if (seq == null && series.HasAutomaticNumbering)
                        {
                            isNewSequence = true;
                            seq = new DocumentSequence(uow);
                            seq.DocumentSeries = series;
                            seq.DocumentNumber = 1;
                            seq.Description = series.Description;
                            series.DocumentSequence = seq;
                        }
                        if (seq == null)
                        {
                            throw new Exception("There is a problem with your data concerning Store's Document Sequence!");
                        }
                        if (!isNewSequence && series.HasAutomaticNumbering)
                        {
                            seq.DocumentNumber++;
                        }
                        if (series.HasAutomaticNumbering)
                        {
                            dbObject.DocumentNumber = seq.DocumentNumber;
                        }
                    }
                    dbObject.FiscalDate = DateTime.Now;
                    dbObject.FinalizedDate = DateTime.Now;
                    dbObject.IsSynchronized = true;
                    dbObject.SkipOnSavingProcess = true;
                    uow.CommitChanges();
                    return Ok(200);
                }
                catch (Exception ex)
                {
                    WebApiConfig.ApiLogger.Log(ex, " ,DocumentHeaderController, Post", KernelLogLevel.Error);
                    uow.RollbackTransaction();
                    return BadRequest(ex.Message);
                }
            }

        }


    }
}
