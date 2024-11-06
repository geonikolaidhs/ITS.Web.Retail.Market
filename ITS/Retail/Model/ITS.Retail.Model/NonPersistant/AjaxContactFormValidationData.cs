using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.ResourcesLib;

namespace ITS.Retail.Model
{
    [NonPersistent]
    public class AjaxContactFormValidationData
    {
        [Required(
            ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "FullnameIsRequired")]
        public string FullName { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "EmailIsRequired")]
        [RegularExpression(".+\\@.+\\..+",
            ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "EmailIsInValid")]
        public string Email { get; set; }

        [Required(
            ErrorMessageResourceType = typeof(Resources),
            ErrorMessageResourceName = "SubjectIsRequired")]
        public string Subject { get; set; }

        public string Message { get; set; }
    }  
}


