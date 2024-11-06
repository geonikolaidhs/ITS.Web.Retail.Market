using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    public class VariableActionType : BaseObj
    {
        private Variable _Variable;
        private ActionType _ActionType;
        private VariableMethods _VariableAction;
        //private VariableReplaceMethod _VariableReplaceMethod;
        //private bool _UpdateFieldOnRecalculate;
        private string _VariableName;

        public VariableActionType() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public VariableActionType(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        [Indexed("ActionType;GCRecord",Unique = true)]
        [Association("Variable-VariableActionTypes")]
        public Variable Variable
        {
            get
            {
                return _Variable;
            }
            set
            {
                SetPropertyValue("Variable", ref _Variable, value);
            }
        }

        [DisplayOrder(Order = 1)]
        public string VariableName
        {
            get
            {
                return _VariableName;
            }
            set
            {
                SetPropertyValue("VariableName", ref _VariableName, value);
            }
        }

        [Association("ActionType-VariableActionTypes")]
        public ActionType ActionType
        {
            get
            {
                return _ActionType;
            }
            set
            {
                SetPropertyValue("ActionType", ref _ActionType, value);
            }
        }

        public VariableMethods VariableAction
        {
            get
            {
                return _VariableAction;
            }
            set
            {
                SetPropertyValue("VariableAction", ref _VariableAction, value);
            }
        }

        //[DisplayOrder(Order = 2)]
        //public VariableReplaceMethod VariableReplaceMethod
        //{
        //    get
        //    {
        //        return _VariableReplaceMethod;
        //    }
        //    set
        //    {
        //        SetPropertyValue("VariableReplaceMethod", ref _VariableReplaceMethod, value);
        //    }
        //}

        //public bool UpdateFieldOnRecalculate
        //{
        //    get
        //    {
        //        return _UpdateFieldOnRecalculate;
        //    }
        //    set
        //    {
        //        SetPropertyValue("UpdateFieldOnRecalculate", ref _UpdateFieldOnRecalculate, value);
        //    }
        //}
    }
}
