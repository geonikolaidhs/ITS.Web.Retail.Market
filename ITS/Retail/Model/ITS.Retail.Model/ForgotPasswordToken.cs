using DevExpress.Xpo;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model
{
    public class ForgotPasswordToken : Token
    {
        public ForgotPasswordToken()
        {

        }

        public ForgotPasswordToken(Session session)
            : base(session)
        {            
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            this.ValidUntilTicks = this.ValidFrom.AddHours(2).Ticks;
        }
    }
}
