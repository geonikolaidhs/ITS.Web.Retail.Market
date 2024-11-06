using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Common.AuxilliaryClasses
{
    public class UserParameter
    {

        public UserParameter() { }

        public string ParameterName { get; set; }

        public string LabelText { get; set; }

        public object SelectedValue { get; set; }

        public object DefaultValue { get; set; }

        public bool Required { get; set; } = true;

        public eUserParameterType Type { get; set; }

        public Dictionary<object, object> Datasource { get; set; }

        public void CreateUserParameter(string labelText = "", object defaultValue = null, eUserParameterType type = eUserParameterType.Text, Dictionary<object, object> datasource = null)
        {
            this.LabelText = labelText;
            this.DefaultValue = defaultValue;
            this.Type = type;

            if (this.SelectedValue == null)
            {
                this.SelectedValue = defaultValue;
            }

            if (datasource == null)
            {
                this.Datasource = new Dictionary<object, object>();
            }
            else
            {
                this.Datasource = datasource;
            }



        }


        public UserParameter SelfCopy()
        {
            UserParameter copy = new UserParameter();
            copy.Datasource = this.Datasource;
            copy.DefaultValue = this.DefaultValue;
            copy.LabelText = LabelText;
            copy.Type = this.Type;
            return copy;
        }
    }
}
