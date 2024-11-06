using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    public class POSActionLevel : BaseObj
    {
        public POSActionLevel()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public POSActionLevel(Session session)
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

        private POSActionLevelsSet _POSActionLevelsSet;
        [Association("POSActionLevelsSet-POSActionLevels")]
        public POSActionLevelsSet POSActionLevelsSet
        {
            get
            {
                return _POSActionLevelsSet;
            }
            set
            {
                SetPropertyValue("POSActionLevelsSet", ref _POSActionLevelsSet, value);
            }
        }

        private eActions _ActionCode;
        [Indexed("POSActionLevelsSet;GCRecord", Unique = true)]
        [DescriptionField]
        public eActions ActionCode
        {
            get
            {
                return _ActionCode;
            }
            set
            {
                SetPropertyValue("ActionCode", ref _ActionCode, value);
            }
        }

        private eKeyStatus _KeyLevel;
        public eKeyStatus KeyLevel
        {
            get
            {
                return _KeyLevel;
            }
            set
            {
                SetPropertyValue("KeyLevel", ref _KeyLevel, value);
            }
        }
    }
}
