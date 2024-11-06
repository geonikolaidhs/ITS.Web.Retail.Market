using DevExpress.Data.Filtering;
using DevExpress.Xpo;

namespace ITS.POS.Model.Settings
{
    public class ReasonCategory : Lookup2Fields
    {
        public ReasonCategory()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ReasonCategory(Session session)
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

        public XPCollection<Reason> Reasons
        {
            get
            {
                return new XPCollection<Reason>(this.Session, new BinaryOperator("Category", Oid));
            }
        }
    }
}
