using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Retail.ResourcesLib;
using ITS.Retail.Model;
using ITS.Retail.Model.NonPersistant;
using ITS.Retail.WebClient.Helpers;

namespace ITS.Retail.WebClient.Areas.B2C.Controllers
{
    public class ContactController : BaseController
    {
        //
        // GET: /B2C/Contact/

        public ActionResult Index()
        {
            ViewBag.Title = Resources.ContactUs;
            ViewBag.MetaDescription = Resources.ContactUs;
            return View();
        }

        [HttpPost]
        public ActionResult AjaxContactForm(ITS.Retail.Model.AjaxContactFormValidationData validationData) {
            if (!Request.IsAjaxRequest()) { 
                ModelState.Clear();
                return View("Index", validationData);
            }
            
            if (ModelState.IsValid) {
                if (CurrentCompany.OwnerApplicationSettings != null && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress)
                && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpHost)
                && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPassword)
                && !string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpUsername)
                )
                {
                    ContactFormEmailTemplate contactFormEmailTemplate = new ContactFormEmailTemplate() { 
                        WelcomeMessage = Resources.NewMessageFromContactForm, 
                        FullName = validationData.FullName, 
                        Email = validationData.Email, 
                        Subject = validationData.Subject, 
                        Message = validationData.Message };

                    string emailBody = RenderViewToString(contactFormEmailTemplate);
                    MailHelper.SendMailMessage(CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress,
                        new List<string>() { CurrentCompany.OwnerApplicationSettings.SmtpEmailAddress },
                        contactFormEmailTemplate.WelcomeMessage, emailBody, CurrentCompany.OwnerApplicationSettings.SmtpHost,
                        username: CurrentCompany.OwnerApplicationSettings.SmtpUsername,
                        password: CurrentCompany.OwnerApplicationSettings.SmtpPassword,
                        port: (string.IsNullOrEmpty(CurrentCompany.OwnerApplicationSettings.SmtpPort) ? "25" : CurrentCompany.OwnerApplicationSettings.SmtpPort),
                        enableSSL: CurrentCompany.OwnerApplicationSettings.SmtpUseSSL
                        );
                    Session["Success"] = Resources.FormSuccessfullySubmitted;
                    ModelState.Clear();
                    return PartialView("AjaxContactFormPartial", validationData);
                }
                else
                {
                    Session["Danger"] = Resources.ErrorSubmitingForm;
                    return PartialView("AjaxContactFormPartial", validationData);
                }
            }
            else
                return PartialView("AjaxContactFormPartial", validationData);
        }
    

    }
}
