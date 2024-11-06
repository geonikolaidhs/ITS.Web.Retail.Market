using System;
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.Retail.Model
{

    public class RelativeDocumentDetail : BaseObj
    {

        public RelativeDocumentDetail()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public RelativeDocumentDetail(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here. -- 
        }

        // Fields...
        private DocumentDetail _DerivedDocumentDetail;
        private decimal _Qty;
        private DocumentDetail _InitialDocumentDetail;
        private RelativeDocument _RelativeDocument;

        [Association("RelativeDocument-RelativeDocumentDetails")]
        public RelativeDocument RelativeDocument
        {
            get
            {
                return _RelativeDocument;
            }
            set
            {
                SetPropertyValue("RelativeDocument", ref _RelativeDocument, value);
            }
        }

         [Association("DocumentDetail-RelativeDocumentDetail-InitialDocumentDetail")] 
        public DocumentDetail InitialDocumentDetail
        {
            get
            {
                return _InitialDocumentDetail;
            }
            set
            {
                SetPropertyValue("InitialDocumentDetail", ref _InitialDocumentDetail, value);
            }
        }

        [Association("DocumentDetail-RelativeDocumentDetail-DerivedDocumentDetail")]       
        public DocumentDetail DerivedDocumentDetail
        {
            get
            {
                return _DerivedDocumentDetail;
            }
            set
            {
                SetPropertyValue("DerivedDocumentDetail", ref _DerivedDocumentDetail, value);
            }
        }

        public decimal Qty
        {
            get
            {
                return _Qty;
            }
            set
            {
                SetPropertyValue("Qty", ref _Qty, value);
            }
        }
    }
}
