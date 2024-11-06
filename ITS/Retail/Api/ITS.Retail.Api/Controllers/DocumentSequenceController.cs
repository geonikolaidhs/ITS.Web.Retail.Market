using DevExpress.Xpo;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.WRM.Kernel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ITS.Retail.Api.Controllers
{
    public class DocumentSequenceController : BaseODataController<DocumentSequence>
    {
        public DocumentSequenceController(IWRMUserDbModule wrmDbModule) : base(wrmDbModule) { }
        /* /// <summary>
         /// Returns DocumentSequence from specific document series
         /// </summary>
         /// <param name="DocumentSeries"></param>
         /// <returns></returns>
         public IQueryable<DocumentSequenceViewModel> GetDocumentSeries(Guid DocumentSeries)
         {
             var Ws = db.Query<DocumentSequence>().Where(x => x.DocumentSeries.Oid == DocumentSeries)
                                         .Select(s => new DocumentSequenceViewModel(s));
             return Ws;
         }

         /// <summary>
         /// Returns all document sequence from specific owner
         /// </summary>
         /// <param name="Owner"></param>
         /// <returns></returns>
         public IQueryable<DocumentSequenceViewModel> GetOwnerDocumentSequence(Guid Owner)
         {
             var Ws = db.Query<DocumentSequence>().Where(x => x.Owner.Oid == Owner)
                                         .Select(s => new DocumentSequenceViewModel(s));
             return Ws;
         }

         /// <summary>
         /// Retrun all sequence from specific owner and specific document series 
         /// All keys are guid 
         /// </summary>
         /// <param name="Owner"></param>
         /// <param name="DocumentSeries"></param>
         /// <returns></returns>
         public IQueryable<DocumentSequenceViewModel> GetOwnerDocumentSequence(Guid Owner, Guid DocumentSeries)
         {
             var Ws = db.Query<DocumentSequence>().Where(x => x.Owner.Oid == Owner && x.DocumentSeries.Oid == DocumentSeries)
                                         .Select(s => new DocumentSequenceViewModel(s));
             return Ws;
         }

     */
    }
}
