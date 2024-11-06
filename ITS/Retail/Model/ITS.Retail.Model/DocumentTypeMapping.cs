using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Mobile.AuxilliaryClasses;

namespace ITS.Retail.Model
{
    public class DocumentTypeMapping : BaseObj
    {
        public DocumentTypeMapping()
        {
            
        }
        public DocumentTypeMapping(Session session)
            : base(session)
        {
            
        }

        // Fields...
        private DocumentType _DocumentType;
        private eDocumentType _EDocumentType;

        public eDocumentType eDocumentType
        {
            get
            {
                return _EDocumentType;
            }
            set
            {
                SetPropertyValue("eDocumentType", ref _EDocumentType, value);
            }
        }


        [Indexed("GCRecord;eDocumentType", Unique = true)]
        public DocumentType DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                if (_DocumentType == value){
                    return;
                }

                
                DocumentType prevDocumentType = _DocumentType;
                _DocumentType = value;

                if (IsLoading)
                {
                    return;
                }

               
                if (prevDocumentType != null && prevDocumentType.DocumentTypeMapping == this){
                    prevDocumentType.DocumentTypeMapping = null;
                }

                
                if (_DocumentType != null){
                    _DocumentType.DocumentTypeMapping = this;
                }

                OnChanged("DocumentType");
            }
        }

    }
}
