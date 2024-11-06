using DevExpress.Data.Filtering;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Exceptions;
using ITS.POS.Client.Kernel;
using ITS.POS.Model.Settings;
using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions
{
    public class ActionAddCoupon : Action
    {
        public override eMachineStatus ValidMachineStatuses
        {
            get {
                return eMachineStatus.OPENDOCUMENT | eMachineStatus.OPENDOCUMENT_PAYMENT;
            }
        }

        public override eActions ActionCode
        {
            get 
            {
                return eActions.ADD_COUPON;
            }
        }

        protected override void ExecuteCore(ActionParams parameters, bool dontCheckPermissions)
        {
            IConfigurationManager configurationManager = Kernel.GetModule<IConfigurationManager>();
            IAppContext appcontext = Kernel.GetModule<IAppContext>();
            IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
            ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();

            appcontext.MainForm.ResetInputText();

            ActionAddCouponParams actionAddCouponParameters = parameters as ActionAddCouponParams;
            ITS.Retail.Platform.Common.ViewModel.CouponViewModel couponViewModel = null;
            try
            {
                //Check if is a usable coupon from web service and throw new POS Exception if needed
                using (WebService.POSUpdateService webService = new WebService.POSUpdateService())
                {
                    webService.Url = configurationManager.StoreControllerWebServiceURL;
                    WebService.CouponViewModel webServiceCouponViewModel = webService.GetExistingUsableCoupon(configurationManager.TerminalID.ToString(),actionAddCouponParameters.CouponCode, configurationManager.CurrentStoreOid);
                    if (webServiceCouponViewModel == null)
                    {
                        throw new POSUserVisibleException(ITS.POS.Resources.POSClientResources.COUPON_HAS_NOT_BEEN_FOUND);
                    }
                    couponViewModel = new Retail.Platform.Common.ViewModel.CouponViewModel()
                    {
                        Oid = webServiceCouponViewModel.Oid,
                        Amount = webServiceCouponViewModel.Amount,
                        Code = webServiceCouponViewModel.Code,
                        Owner = webServiceCouponViewModel.Owner,
                        Description = webServiceCouponViewModel.Description,
                        IsActiveUntilDate = webServiceCouponViewModel.IsActiveUntilDate,
                        IsActiveFromDate = webServiceCouponViewModel.IsActiveFromDate,
                        NumberOfTimesUsed = webServiceCouponViewModel.NumberOfTimesUsed,
                        IsUnique = webServiceCouponViewModel.IsUnique,
                        CouponAmountIsAppliedAs = (CouponAmountIsAppliedAs)Enum.Parse(typeof(CouponAmountIsAppliedAs), webServiceCouponViewModel.CouponAmountIsAppliedAs.ToString()),
                        CouponAmountType = (CouponAmountType)Enum.Parse(typeof(CouponAmountType), webServiceCouponViewModel.CouponAmountType.ToString()),
                        CouponAppliesOn = (CouponAppliesOn)Enum.Parse(typeof(CouponAppliesOn), webServiceCouponViewModel.CouponAppliesOn.ToString()),
                        DiscountType = webServiceCouponViewModel.DiscountType,
                        PaymentMethod = webServiceCouponViewModel.PaymentMethod,
                        CouponCategory = webServiceCouponViewModel.CouponCategory,
                        CouponCategoryDescription = webServiceCouponViewModel.CouponCategoryDescription,
                        CouponMaskOid = webServiceCouponViewModel.CouponMaskOid,
                        DecodedString = webServiceCouponViewModel.DecodedString
                    };

                    //Check if Coupon is already used
                    if(couponViewModel.IsUnique)
                    {
                        CriteriaOperator transactionCouponHasBeenUsedCriteria = CriteriaOperator.And(
                                                                                                        new BinaryOperator("IsCanceled", false),
                                                                                                        new BinaryOperator("CouponCode", couponViewModel.Code)
                                                                                                     );
                        int numberOfMatchingCoupons = Convert.ToInt32(sessionManager.GetSession<Model.Transactions.TransactionCoupon>()
                                                                    .Evaluate<Model.Transactions.TransactionCoupon>(
                                                                                                                    CriteriaOperator.Parse("Count()"),
                                                                                                                    transactionCouponHasBeenUsedCriteria
                                                                                                                    )
                                       );
                        if (numberOfMatchingCoupons >= 1)
                        {
                            throw new POSUserVisibleException(string.Format(Resources.POSClientResources.COUPON_HAS_ALREADY_BEEN_USED_TIMES, numberOfMatchingCoupons));
                        }
                    }

                    //Check if Coupon can be applied
                    switch (couponViewModel.CanBeUsed)
                    {
                        case CouponCanBeUsedMessage.UNDEFINED:
                            throw new POSUserVisibleException(Resources.POSClientResources.COUPON_HAS_NOT_BEEN_FOUND);
                        case CouponCanBeUsedMessage.NOT_ACTIVE_YET:
                            throw new POSUserVisibleException(string.Format(Resources.POSClientResources.COUPON_IS_ACTIVE_FROM_UNTIL, new DateTime(couponViewModel.IsActiveFrom).ToString(), new DateTime(couponViewModel.IsActiveUntil).ToString()));
                        case CouponCanBeUsedMessage.EXPIRED:
                            throw new POSUserVisibleException(string.Format(Resources.POSClientResources.COUPON_IS_ACTIVE_FROM_UNTIL, new DateTime(couponViewModel.IsActiveFrom).ToString(), new DateTime(couponViewModel.IsActiveUntil).ToString()));
                        case CouponCanBeUsedMessage.ALREADY_USED:
                            throw new POSUserVisibleException(string.Format(Resources.POSClientResources.COUPON_HAS_ALREADY_BEEN_USED_TIMES, couponViewModel.NumberOfTimesUsed));
                        case CouponCanBeUsedMessage.USABLE:
                            eMachineStatus machineStatus = appcontext.GetMachineStatus();
                            switch (machineStatus)
                            {
                                case eMachineStatus.OPENDOCUMENT:
                                    if (couponViewModel.CouponAppliesOn == CouponAppliesOn.ITEM
                                        && couponViewModel.CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.DISCOUNT
                                        && appcontext.CurrentDocumentLine != null
                                      )
                                    {
                                        decimal discount = couponViewModel.Amount;
                                        POS.Model.Settings.DiscountType discountType = sessionManager.GetObjectByKey<POS.Model.Settings.DiscountType>(couponViewModel.DiscountType);
                                        actionManager.GetAction(eActions.ADD_LINE_DISCOUNT).Execute(new ActionAddLineDiscountParams(discount, discountType, true,couponViewModel), true);
                                        return;
                                    }
                                    throw new POSUserVisibleException(Resources.POSClientResources.INVALID_ACTION);
                                case eMachineStatus.OPENDOCUMENT_PAYMENT:
                                    if ((couponViewModel.CouponAppliesOn == CouponAppliesOn.GROSS_TOTAL
                                            && couponViewModel.CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.DISCOUNT
                                           )
                                        || couponViewModel.CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.PAYMENT_METHOD
                                        || couponViewModel.CouponAmountIsAppliedAs == CouponAmountIsAppliedAs.POINTS
                                        )
                                    {
                                        switch (couponViewModel.CouponAmountIsAppliedAs)
                                        {
                                            case CouponAmountIsAppliedAs.DISCOUNT:
                                                POS.Model.Settings.DiscountType discountType = sessionManager.GetObjectByKey<POS.Model.Settings.DiscountType>(couponViewModel.DiscountType);
                                                actionManager.GetAction(eActions.ADD_DOCUMENT_DISCOUNT).Execute(new ActionAddDocumentDiscountParams(couponViewModel.Amount, discountType,couponViewModel), true);
                                                return;
                                            case CouponAmountIsAppliedAs.PAYMENT_METHOD:
                                                if (couponViewModel.Amount > appcontext.CurrentDocument.GrossTotal)
                                                {
                                                    throw new POSUserVisibleException(Resources.POSClientResources.AMOUNT_EXCEED_LIMIT);
                                                }
                                                POS.Model.Settings.PaymentMethod paymentMethod = sessionManager.GetObjectByKey<POS.Model.Settings.PaymentMethod>(couponViewModel.PaymentMethod);
                                                actionManager.GetAction(eActions.ADD_PAYMENT).Execute(new ActionAddPaymentParams(paymentMethod, couponViewModel.Amount,couponViewModel));
                                                return;
                                            case CouponAmountIsAppliedAs.POINTS:
                                                throw new NotImplementedException();
                                        }
                                    }
                                    throw new POSUserVisibleException(Resources.POSClientResources.INVALID_ACTION);
                                default:
                                    throw new POSUserVisibleException(Resources.POSClientResources.INVALID_ACTION);
                            }
                        default:
                            throw new POSUserVisibleException(ITS.POS.Resources.POSClientResources.COUPON_HAS_NOT_BEEN_FOUND);
                    }

                }
            }
            catch (POSUserVisibleException posException)
            {
                //Kernel.LogFile.DebugException(ITS.POS.Resources.POSClientResources.ERROR, posException);
                Kernel.LogFile.Debug(posException, Resources.POSClientResources.ERROR);
                throw;
            }
            catch (POSException posException)
            {
                //Kernel.LogFile.DebugException(ITS.POS.Resources.POSClientResources.ERROR, posException);
                Kernel.LogFile.Debug(posException, Resources.POSClientResources.ERROR);
                throw new POSUserVisibleException(posException.Message, posException);
            }
            catch (Exception exception)
            {
                Kernel.LogFile.Debug(exception, Resources.POSClientResources.CANNOT_CONNECT_TO_THE_SERVER);
                //Kernel.LogFile.DebugException(ITS.POS.Resources.POSClientResources.CANNOT_CONNECT_TO_THE_SERVER, exception);
                throw new POSUserVisibleException(ITS.POS.Resources.POSClientResources.CANNOT_CONNECT_TO_THE_SERVER, exception);
            }
        }
        
        public ActionAddCoupon(IPosKernel kernel)
            : base(kernel)
        {

        }

        public override bool RequiresParameters
        {
            get
            {
                return true;
            }
        }
    }
}
