using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    public class ActionTypeDocStatus : BaseObj
    {
        private ActionTypeEntity _ActionTypeEntity;
        private string _DocStatusCode;
        private DocumentStatus _DocumentStatus;

        public ActionTypeDocStatus()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ActionTypeDocStatus(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        [Association("DocumentStatus-ActionTypeDocStatuses")]
        public DocumentStatus DocumentStatus
        {
            get
            {
                return _DocumentStatus;
            }
            set
            {
                SetPropertyValue("DocumentStatus", ref _DocumentStatus, value);
            }
        }

        public string DocStatusCode
        {
            get
            {
                return _DocStatusCode;
            }
            set
            {
                SetPropertyValue("DocStatusCode", ref _DocStatusCode, value);
            }
        }

        [Association("ActionTypeEntity-ActionTypeDocStatuses")]
        public ActionTypeEntity ActionTypeEntity
        {
            get
            {
                return _ActionTypeEntity;
            }
            set
            {
                SetPropertyValue("ActionType", ref _ActionTypeEntity, value);
            }
        }
    }
}
