using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface ICouponMask: ICouponBase
    {
        string Prefix { get; set; }
        string Mask { get; set; }
        string PropertyName { get; set; }
        ICouponCategory CouponCategory { get; set; }
    }
}
