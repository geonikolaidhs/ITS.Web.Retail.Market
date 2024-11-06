using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.Model.NonPersistant
{
    /// <summary>
    /// A class that contains all necessary info
    /// to execute the relevant Totalizer
    /// </summary>
    public class ActionTypeTriggerInfo
    {
        /// <summary>
        /// The id (Guid) of the entity that triggered the Totalizer
        /// </summary>
        public Guid EntityOid { get; set; }

        /// <summary>
        /// The Type (FullName) of the entity that triggered the Totalizer
        /// </summary>
        public Type EntityType { get; set; }

        public DatabaseAction DataBaseAction { get; set; }

        /// <summary>
        /// A list of all member values (of the entity that triggered the Totalizer) that changed
        /// and they respective values
        /// </summary>
        public List<MemberValuesDifference> MemberValuesDifferences { get; set; }

        /// <summary>
        /// A list of all Oids od ActionTypeEntities that should be executed
        /// </summary>
        public List<Guid> ActionTypeEntities { get; set; }

        public ActionTypeTriggerInfo(Guid entityOid, Type entityType, DatabaseAction dataBaseAction, List<MemberValuesDifference> memberValuesDifferences ,List<Guid> actionTypeEntities)
        {
            EntityOid = entityOid;
            EntityType = entityType;
            DataBaseAction = dataBaseAction;
            MemberValuesDifferences = memberValuesDifferences;
            ActionTypeEntities = actionTypeEntities;
        }

        //public bool ActionIsRecalculated { get; set; }

        public bool ShouldTriggerActionType(BasicObj obj, ActionTypeEntity actionTypeEntity)
        {
            //if( this.ActionTypeEntities.Count <= 0 )
            //{
            //    return false;
            //}

            MethodInfo methodInfo = this.EntityType.GetMethod("ShouldExecuteActionTypes");
            if (methodInfo == null)
            {
                return false;
            }

            object result = methodInfo.Invoke(null, new object[] { this.MemberValuesDifferences, obj, actionTypeEntity });
            return (bool)result;
        }
    }
}
