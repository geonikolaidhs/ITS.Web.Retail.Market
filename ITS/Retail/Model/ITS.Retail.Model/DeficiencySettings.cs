using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    public class DeficiencySettings : Lookup2Fields, IRequiredOwner
    {
        public DeficiencySettings()
        {
            
        }
        public DeficiencySettings(Session session)
            : base(session)
        {
            
        }
        public DeficiencySettings(string code, string description)
            : base(code, description)
        {
            
        }
        public DeficiencySettings(Session session, string code, string description)
            : base(session, code, description)
        {
            
        }

        // Fields...


        // Fields...
        private DocumentType _DeficiencyDocumentType;
        //[Association("DeficiencySettings-DocumentType")]
        public DocumentType DeficiencyDocumentType
        {
            get
            {
                return _DeficiencyDocumentType;
            }
            set
            {
                if (_DeficiencyDocumentType == value)
                {
                    return;
                }

                DocumentType previousValue = _DeficiencyDocumentType;
                _DeficiencyDocumentType = value;

                if (IsLoading)
                {
                    return;
                }

                if (previousValue != null && previousValue.DeficiencySettings == this)
                {
                    previousValue.DeficiencySettings = null;
                }

                if (_DeficiencyDocumentType != null)
                {
                    _DeficiencyDocumentType.DeficiencySettings = this;
                }
                OnChanged("DeficiencyDocumentType");
            }
        }

        //public IEnumerable<DocumentType> DocumentTypes
        //{
        //    get
        //    {
        //        return DeficiencySettingsDetails==null ? null : DeficiencySettingsDetails.Select(deficiencyDetail=>deficiencyDetail.DocumentType);
        //    }
        //}

        [Aggregated, Association("DeficiencySettings-DeficiencySettingsDetails")]
        public XPCollection<DeficiencySettingsDetail> DeficiencySettingsDetails
        {
            get
            {
                return GetCollection<DeficiencySettingsDetail>("DeficiencySettingsDetails");
            }
        }

    }
}
