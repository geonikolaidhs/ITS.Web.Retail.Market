using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    [AllowAnonymous]
    public class BaseApiController : Controller
    {

        private static List<DocumentType> _AllDocumentTypes = null;

        public static List<DocumentType> AllDocumentTypes
        {
            get
            {
                if (_AllDocumentTypes == null)
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        _AllDocumentTypes = new XPCollection<DocumentType>(uow, CriteriaOperator.And(new OperandProperty("GCRecord").IsNull())).ToList();
                    }
                }
                return _AllDocumentTypes;
            }
            set
            {
                _AllDocumentTypes = value;
            }
        }


    }
}