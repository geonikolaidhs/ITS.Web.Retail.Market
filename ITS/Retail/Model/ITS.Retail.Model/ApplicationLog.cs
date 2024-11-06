//-----------------------------------------------------------------------
// <copyright file="ApplicationLog.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{

    public class ApplicationLog : BaseObj
    {
        public ApplicationLog()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ApplicationLog(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        private string _IPAddress;
        public string IPAddress
        {
            get
            {
                return _IPAddress;
            }
            set
            {
                SetPropertyValue("IPAddress", ref _IPAddress, value);
            }
        }
        private string _DeviceName;
        public string DeviceName
        {
            get
            {
                return _DeviceName;
            }
            set
            {
                SetPropertyValue("DeviceName", ref _DeviceName, value);
            }
        }

        private string _UserAgent;
        [Size(SizeAttribute.Unlimited)]
        public string UserAgent
        {
            get
            {
                return _UserAgent;
            }
            set
            {
                SetPropertyValue("UserAgent", ref _UserAgent, value);
            }
        }

        private string _Controller;
        public string Controller
        {
            get
            {
                return _Controller;
            }
            set
            {
                SetPropertyValue("Controller", ref _Controller, value);
            }
        }

        private string _Action;
        public string Action
        {
            get
            {
                return _Action;
            }
            set
            {
                SetPropertyValue("Action", ref _Action, value);
            }
        }

        private string _Result;
        [Size(SizeAttribute.Unlimited)]
        [DescriptionField]
        public string Result
        {
            get
            {
                return _Result;
            }
            set
            {
                SetPropertyValue("Result", ref _Result, value);
            }
        }

        private string _Error;
        [Size(SizeAttribute.Unlimited)]
        public string Error
        {
            get
            {
                return _Error;
            }
            set
            {
                SetPropertyValue("Error", ref _Error, value);
            }
        }

    }

}