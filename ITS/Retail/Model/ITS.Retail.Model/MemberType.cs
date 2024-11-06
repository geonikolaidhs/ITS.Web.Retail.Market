//-----------------------------------------------------------------------
// <copyright file="MemberType.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{

    public class MemberType : Lookup2Fields
    {
        public MemberType()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public MemberType(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public MemberType(string code, string description)
            : base(code, description)
        {
            
        }
        public MemberType(Session session, string code, string description)
            : base(session, code, description)
        {
            
        }
         

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        private MemberType _Traders;
        //[Association("MemberType-MemberTypes")]
        public MemberType Traders
        {
            get
            {
                return _Traders;
            }
            set
            {
                SetPropertyValue("Traders", ref _Traders, value);
            }
        }
    }

}