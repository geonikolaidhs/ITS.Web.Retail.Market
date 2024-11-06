using DevExpress.Xpo;
using ITS.Retail.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.WebClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.Common;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    public class CheckoutController : BaseController
    {
        public ActionResult Index()
        {
            if (!IsUserLoggedIn)
            {
                Danger = Resources.PleaseLogin;
                return new RedirectResult("~/B2C/User/Login");
            }
            ViewBag.Title = Resources.Buy;
            ViewBag.MetaDescription = Resources.Buy;
            return View();
        }

        public ActionResult Save()
        {
            if (!IsUserLoggedIn)
            {
                Danger = Resources.PleaseLogin;
                return new RedirectResult("~/B2C/User/Login");
            }
            try
            {

                int paymentMethodSelection = 0;
                PaymentMethod paymentMethod = null;
                B2CPaymentMethodType b2CPaymentMethodType = B2CPaymentMethodType.CashOnDelivery;
                if (int.TryParse(Request["radiodListPaymentMethod"], out paymentMethodSelection))
                {
                    switch (paymentMethodSelection)
                    {
                        case 0:
                            paymentMethod = ShoppingCart.Session.GetObjectByKey<PaymentMethod>(CurrentCompany.OwnerApplicationSettings.CashOnDelivery.Oid);
                            b2CPaymentMethodType = B2CPaymentMethodType.CashOnDelivery;
                            break;
                        case 1:
                            paymentMethod = ShoppingCart.Session.GetObjectByKey<PaymentMethod>(CurrentCompany.OwnerApplicationSettings.BankDeposit.Oid);
                            b2CPaymentMethodType = B2CPaymentMethodType.BankDeposit;
                            break;
                        case 2:
                            paymentMethod = ShoppingCart.Session.GetObjectByKey<PaymentMethod>(CurrentCompany.OwnerApplicationSettings.PayPal.Oid);
                            b2CPaymentMethodType = B2CPaymentMethodType.PayPal;
                            break;
                        default :
                            paymentMethod = null;
                            b2CPaymentMethodType = B2CPaymentMethodType.CashOnDelivery;
                            break;
                    }
                }
                if (paymentMethod != null)
                {
                    ShoppingCart.DocumentPayments.Add(new DocumentPayment(ShoppingCart.Session){
                                                                            PaymentMethod = paymentMethod ,
                                                                            Amount = ShoppingCart.GrossTotal
                                                                        });
                }
                ShoppingCart.Remarks = Request["OrderComment"];
                if (ShoppingCart.Remarks == null)
                {
                    ShoppingCart.Remarks = string.Empty;
                }
                
                Guid addresGuid = Guid.Empty;
                if( Guid.TryParse(Request["selectedAddress"],out addresGuid) == false)
                {
                    throw new Exception(Resources.InvalidAddress);
                }
                Address address = ShoppingCart.Session.GetObjectByKey<Address>(addresGuid);
                if(address==null)
                {
                    throw new Exception(Resources.InvalidAddress);
                }
                ShoppingCart.DeliveryAddress = address.Description;
                ShoppingCart.BillingAddress = address;
                
                ShoppingCart.Save();
                ShoppingCart.Session.CommitTransaction();
                Guid ShoppingCartOid = ShoppingCart.Oid;
                ShoppingCart = null;
                Success = ResourcesLib.Resources.SavedSuccesfully;


                if (b2CPaymentMethodType != B2CPaymentMethodType.PayPal)
                {
                    if (CurrentCompany.OwnerApplicationSettings != null && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress)
                   && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpHost)
                   && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPassword)
                   && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpUsername)
                   )
                    {
                        OrderEmailTemplate orderEmailTemplate = new OrderEmailTemplate();
                        
                        orderEmailTemplate.DocumentHeader = XpoHelper.GetNewUnitOfWork().GetObjectByKey<DocumentHeader>(ShoppingCartOid);
                        orderEmailTemplate.WelcomeMessage = Resources.NewOrder + " " + Resources.WithNumber + " " + orderEmailTemplate.DocumentHeader.DocumentNumber;

                        string emailBody = RenderViewToString(orderEmailTemplate);
                        MailHelper.SendMailMessage(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress,
                            new List<string>() { CurrentUser.Email },
                            orderEmailTemplate.WelcomeMessage, emailBody, CurrentCompany.OwnerApplicationSettings.SmtpHost,
                            username: CurrentCompany.OwnerApplicationSettings.SmtpUsername,
                            password: CurrentCompany.OwnerApplicationSettings.SmtpPassword,
                            port: (string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPort) ? "25" : CurrentCompany.OwnerApplicationSettings.SmtpPort),
                            enableSSL: CurrentCompany.OwnerApplicationSettings.SmtpUseSSL
                            );
                    }
                }

                switch(b2CPaymentMethodType)
                {
                    case B2CPaymentMethodType.CashOnDelivery:
                        break;
                    case B2CPaymentMethodType.BankDeposit:
                        break;
                    case B2CPaymentMethodType.PayPal:
                    default: 
                        break;
                }

            }
            catch (Exception exception)
            {
                Session["Danger"] = exception.GetFullMessage();
            }

            return Request.UrlReferrer == null ? new RedirectResult("~/B2C") : new RedirectResult(Request.UrlReferrer.ToString());
        }

        public ActionResult PayPal(Guid? dc)
        {
            DocumentHeader document = XpoSession.GetObjectByKey<DocumentHeader>(dc);
            if(dc == null || document==null)
            {
                Danger = Resources.AnErrorOccurred;
                return new RedirectResult("~/B2C");
            }

            return View(document);

        }

        public ActionResult TablePartial()
        {   
            return PartialView("TablePartial");
        }
    }
}
