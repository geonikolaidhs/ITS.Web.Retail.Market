using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    [NonPersistent]
    public class Token : BaseObj
    {
        public Token()
        {

        }

        public Token(Session session)
            : base(session)
        {
            
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            this.TokenString = Guid.NewGuid().ToString();
            this._ValidFromTicks = DateTime.Now.Ticks;
        }

        private User _User;
        private string _TokenString;
        protected long _ValidFromTicks;
        protected long _ValidUntilTicks;
      

        public string TokenString
        {
            get
            {
                return _TokenString;
            }
            set
            {
                SetPropertyValue("TokenString", ref _TokenString, value);
            }
        }

        public long ValidFromTicks
        {
            get
            {
                return _ValidFromTicks;
            }
            set
            {
                SetPropertyValue("ValidFromTicks", ref _ValidFromTicks, value);
            }
        }

        public DateTime ValidFrom
        {
            get
            {
                return new DateTime(_ValidFromTicks);
            }
        }

        public long ValidUntilTicks
        {
            get
            {
                return _ValidUntilTicks;
            }
            set
            {
                SetPropertyValue("ValidUntilTicks", ref _ValidUntilTicks, value);
            }
        }


        public DateTime ValidUntil
        {
            get
            {
                return new DateTime(_ValidUntilTicks);
            }
        }

        public User User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }

        public bool IsValid
        {
            get
            {
                long nowTicks = DateTime.Now.Ticks;
                return this.ValidFromTicks <= nowTicks && nowTicks <= this.ValidUntilTicks;
            }
        }
    }
}
