using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SFA_Model.NonPersistant
{
    [NonPersistent]
    public class BasicObj : XPCustomObject, IXPObject, IComparable//, IBasicObj
    {
        // this delegate is just, so you don't have to pass an object array. _(params)_
        public delegate object ConstructorDelegate(params object[] args);

        public static ConstructorDelegate CreateConstructor(Type type, params Type[] parameters)
        {
            // Get the constructor info for these parameters
            var constructorInfo = type.GetConstructor(parameters);

            // define a object[] parameter
            var paramExpr = Expression.Parameter(typeof(Object[]));

            // To feed the constructor with the right parameters, we need to generate an array 
            // of parameters that will be read from the initialize object array argument.
            var constructorParameters = parameters.Select((paramType, index) =>
                // convert the object[index] to the right constructor parameter type.
                Expression.Convert(
                    // read a value from the object[index]
                    Expression.ArrayAccess(
                        paramExpr,
                        Expression.Constant(index)),
                    paramType)).ToArray();

            // just call the constructor.
            var body = Expression.New(constructorInfo, constructorParameters);

            var constructor = Expression.Lambda<ConstructorDelegate>(body, paramExpr);
            return constructor.Compile();
        }

        public BasicObj()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public BasicObj(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        [Key]
        public Guid Oid { get; set; }
        public string CreatedByDevice { get; set; }
        public string UpdateByDevice { get; set; }

        public long CreatedOnTicks { get; set; }

        public bool IsActive { get; set; }

        public bool IsSynchronized { get; set; }

        public string ReferenceId { get; set; }

        public bool RowDeleted { get; set; }

        public Guid UpdatedBy { get; set; }

        public long UpdatedOnTicks { get; set; }


    }
}
