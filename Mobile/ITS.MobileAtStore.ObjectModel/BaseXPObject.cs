using System;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using System.Reflection;

namespace ITS.MobileAtStore.ObjectModel
{
    /// <summary>
    /// The baseXPObject extends the XPBaseObject and provides a base for our XPO objects to inherit from.
    /// </summary>
    [NonPersistent]
    public class BaseXPObject : XPBaseObject
    {
        /// <summary>
        ///
        /// </summary>
        public BaseXPObject()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="session"></param>
        public BaseXPObject(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>
        ///
        /// </summary>
        
        private Guid _Oid;
        [Key(true)]
        public Guid Oid
        {
            get
            {
                return _Oid;
            }
            set
            {
                SetPropertyValue("Oid", ref _Oid, value);
            }
        }

        private DateTime _createdOn = DateTime.Now;
        /// <summary>
        /// The CreatedOn property gets or sets when this object was created.
        /// </summary>
        public DateTime CreatedOn
        {
            get
            {
                return _createdOn;
            }
            set
            {
                SetPropertyValue("CreatedOn", ref _createdOn, value);
            }
        }


        private DateTime _updatedOn = DateTime.Now;
        /// <summary>
        /// The UpdatedOn property gets or sets when this object was recently updated
        /// </summary>
        public DateTime UpdatedOn
        {
            get
            {
                return _updatedOn;
            }
            set
            {
                SetPropertyValue("UpdatedOn", ref _updatedOn, value);
            }
        }

        protected override void OnSaving()
        {
            UpdatedOn = DateTime.Now;
            base.OnSaving();
        }


        public virtual void GetData(BaseXPObject other)
        {
            foreach (PropertyInfo pInfo in other.GetType().GetProperties())
            {
                if (pInfo.CanWrite == false)
                {
                    continue;
                }
                if (typeof(BaseXPObject).IsAssignableFrom(pInfo.PropertyType))
                {
                    //TODO
                }
                else
                {
                    pInfo.SetValue(this, pInfo.GetValue(other, null), null);
                }
            }
        }
    }
}
