﻿using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface ICoupon: ICouponBase
    {
        string Code { get; set; }
        decimal Amount { get; set; }
        ICouponCategory CouponCategory { get; set; }
        int NumberOfTimesUsed { get; set; }
        ICouponMask CouponMask { get; set; }
    }
}