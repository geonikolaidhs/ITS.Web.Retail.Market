using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using SFA_Model.NonPersistant;
using ITS.WRM.Model.Interface;
using ITS.WRM.Model.Interface.Model.SupportingClasses;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA_Model
{
    [CreateOrUpdaterOrder(Order = 265, Permissions = eUpdateDirection.MASTER_TO_SFA)]
    public class MeasurementUnit : LookUp2Fields, ITS.WRM.Model.Interface.Model.SupportingClasses.IRequiredOwner, IMeasurementUnit
    {
        public MeasurementUnit()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public MeasurementUnit(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public MeasurementUnit(string code, string description)
            : base()
        {

        }
        public MeasurementUnit(Session session, string code, string description)
            : base(session, code, description)
        {

        }
        public eMeasurementUnitType MeasurementUnitType { get; set; }
        
        public bool SupportDecimal { get; set; }

    }
}