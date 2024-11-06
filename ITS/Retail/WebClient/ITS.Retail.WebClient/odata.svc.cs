//------------------------------------------------------------------------------
// <copyright file="WebDataService.svc.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using DevExpress.Xpo;
using ITS.Retail.Common;
using System;
using System.Collections.Generic;
using System.Data.Services;
using System.Data.Services.Common;
using System.Linq;
using System.ServiceModel.Web;
using System.Web;
using System.Data.Services.Providers;
using System.Linq.Expressions;
using System.ServiceModel;
using System.Web.Security;
using ITS.Retail.Model;
using DevExpress.Data.Filtering;

namespace ITS.Retail.WebClient
{
#if DEBUG
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]

    public class OData : XpoDataServiceV3
    {
        private Guid currentUserOid;
        private Guid currentCompanyOid;

        static XpoContext context = new XpoContext("ODataExample", "ITS.Retail.Model", XpoHelper.DataLayer);
        public OData() :
             base(context,context)
        {
            
        }
        
        // This method is called only once to initialize service-wide policies.
        public static void InitializeService(DataServiceConfiguration config)
        {
            config.SetEntitySetAccessRule("*", EntitySetRights.AllRead);
            config.UseVerboseErrors = true;
            config.SetServiceOperationAccessRule("*", ServiceOperationRights.AllRead);
            config.SetServiceActionAccessRule("*", ServiceActionRights.Invoke);
            config.DataServiceBehavior.MaxProtocolVersion = DataServiceProtocolVersion.V3;
            config.MaxResultsPerCollection = 100;
            config.DataServiceBehavior.AcceptProjectionRequests = true;
            config.DataServiceBehavior.AcceptCountRequests = true;
            config.AnnotationsBuilder = CreateAnnotationsBuilder(() =>
            {
                return context;
            });
        }

        protected override void OnStartProcessingRequest(ProcessRequestArgs args)
        {
            
            base.OnStartProcessingRequest(args);
        }

        public override object Authenticate(ProcessRequestArgs args)
        {            
            string authCookie = args.OperationContext.RequestHeaders["userCookie"];
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie);    
            if(ticket == null || string.IsNullOrWhiteSpace(ticket.Name))
            {
                return null;
            }
            using (UnitOfWork uow = new UnitOfWork(Context.ObjectLayer))
            {
                User currentUser = uow.FindObject<User>(new BinaryOperator("UserName", ticket.Name));
                this.currentUserOid = currentUser.Oid;
                if(currentUser != null)
                {
                    PrepareUser(currentUser);
                    return "ok";
                }
                return null;
            }
        }

        private void PrepareUser(User currentUser)
        {
            
            //throw new NotImplementedException();
            if(currentUser.Role.Type == Platform.Enumerations.eRoleType.SystemAdministrator)
            {
                currentCompanyOid = Guid.Empty;
            }
        }

        [QueryInterceptor("Item")]
        public Expression<Func<Item, bool>> OnItemQuery()
        {
            return item => item.Owner.Oid == currentCompanyOid;
        }

    }
#endif
}
