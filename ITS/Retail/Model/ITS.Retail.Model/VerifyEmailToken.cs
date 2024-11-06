using DevExpress.Xpo;
using ITS.Retail.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class VerifyEmailToken : Token
    {
        public VerifyEmailToken()
        {

        }

        public VerifyEmailToken(Session session)
            : base(session)
        {
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            this.ValidUntilTicks = this.ValidFrom.AddDays(7).Ticks;
        }
    }
}
