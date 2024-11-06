using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Collections.Generic;
using Newtonsoft.Json;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
   
    public class RelativeDocument : BaseObj
    {

        public RelativeDocument()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public RelativeDocument(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        // Fields...
        private DocumentHeader _DerivedDocument;
        private DocumentHeader _InitialDocument;

        [Association("RelativeDocuments-DerivedDocumentHeader")]
        public DocumentHeader DerivedDocument
        {
            get
            {
                return _DerivedDocument;
            }
            set
            {
                SetPropertyValue("DerivedDocument", ref _DerivedDocument, value);
            }
        }

        [Association("RelativeDocuments-InitialDocumentHeader")]
        public DocumentHeader InitialDocument
        {
            get
            {
                return _InitialDocument;
            }
            set
            {
                SetPropertyValue("InitialDocument", ref _InitialDocument, value);
            }
        }

        [Association("RelativeDocument-RelativeDocumentDetails"),Aggregated]
        public XPCollection<RelativeDocumentDetail> RelativeDocumentDetails
        {
            get
            {
                return GetCollection<RelativeDocumentDetail>("RelativeDocumentDetails");
            }
        }

        public override Dictionary<string, object> GetDict(JsonSerializerSettings settings, bool includeType, bool includeDetails, eUpdateDirection direction = eUpdateDirection.MASTER_TO_STORECONTROLLER | eUpdateDirection.POS_TO_STORECONTROLLER | eUpdateDirection.STORECONTROLLER_TO_MASTER | eUpdateDirection.STORECONTROLLER_TO_POS)
        {
            Dictionary<string, object> dictionary = base.GetDict(settings, includeType, includeDetails);

            if (includeDetails)
            {
                dictionary.Add("RelativeDocumentDetails", RelativeDocumentDetails.Select(g => g.GetDict(settings, includeType, includeDetails)).ToList());
            }
            return dictionary;
        }
    }
}
