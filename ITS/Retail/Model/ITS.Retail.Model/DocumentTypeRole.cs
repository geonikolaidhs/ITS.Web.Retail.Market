using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    public class DocumentTypeRole : BaseObj
    {
        public DocumentTypeRole()
        {
            
        }
        public DocumentTypeRole(Session session)
            : base(session)
        {
            
        }


        // Fields...
        private eDocumentTypeView _DocumentView;
        private Role _Role;
        private DocumentType _DocumentType;

        [Indexed("GCRecord;Role", Unique = true), Association("DocumentType-DocumentTypeRoles")]
        public DocumentType DocumentType
        {
            get
            {
                return _DocumentType;
            }
            set
            {
                SetPropertyValue("DocumentType", ref _DocumentType, value);
            }
        }

        [Association("Role-DocumentTypeRoles")]
        public Role Role
        {
            get
            {
                return _Role;
            }
            set
            {
                SetPropertyValue("Role", ref _Role, value);
            }
        }


        public eDocumentTypeView DocumentView
        {
            get
            {
                return _DocumentView;
            }
            set
            {
                SetPropertyValue("DocumentView", ref _DocumentView, value);
            }
        }
    }
}
