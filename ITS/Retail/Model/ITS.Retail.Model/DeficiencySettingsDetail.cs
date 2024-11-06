using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    public class DeficiencySettingsDetail : BaseObj
    {


        private DocumentType _DocumentType;
        private DeficiencySettings _DeficiencySettings;
        public DeficiencySettingsDetail()
        {
            
        }
        public DeficiencySettingsDetail(Session session)
            : base(session)
        {
            
        }



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
                                                       
        [Association("DeficiencySettings-DeficiencySettingsDetails"), Indexed(Unique = false)]
        public DeficiencySettings DeficiencySettings
        {
            get
            {
                return _DeficiencySettings;
            }
            set
            {
                SetPropertyValue("DeficiencySettings", ref _DeficiencySettings, value);
            }
        }

    }
}
