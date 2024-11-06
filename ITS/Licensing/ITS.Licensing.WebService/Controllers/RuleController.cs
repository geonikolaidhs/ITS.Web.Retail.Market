using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ITS.Licensing.LicenseModel;
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
using DevExpress.Web.Mvc;
using ITS.Licensing.Web.Helpers;

namespace ITS.Licensing.Web.Controllers
{
    public class RuleController : BaseObjController
    {

        public class Operators
        {
            public int value;
            public string name;
            public Operators(int val, string name)
            {
                value = val;
                this.name = name;
            }
        }

        //private List<Operators> GetOperators()
        //{
        //    List<Operators> listToReturn = new List<Operators>();
        //    for (int i = 0; i < 15; i++)
        //    {
        //        try
        //        {
        //            BinaryOperatorType bt = (BinaryOperatorType)i;
        //            listToReturn.Add(new Operators(i, bt.ToString()));
        //        }
        //        catch
        //        {
        //        }
        //    }
        //    return listToReturn;
        //}

        public ActionResult Grid(string ApplicationInfoGuid)
        {
            Guid parsedApplicationInfoGuid = Guid.Empty;
            if (Guid.TryParse(ApplicationInfoGuid, out parsedApplicationInfoGuid))
            {
                ApplicationInfo ai = LicenseConnectionHelper.GetNewUnitOfWork().FindObject<ApplicationInfo>(new BinaryOperator("Oid", parsedApplicationInfoGuid));
                ViewData["ApplicationInfoGuid"] = ApplicationInfoGuid;
                ViewBag.SerialNumbersComboBox = new XPCollection<SerialNumber>(ai.Session, new BinaryOperator("Application.Oid", parsedApplicationInfoGuid));                
                //ViewData["Operators"] = GetOperators();
                return PartialView("Grid", ai.Rules);
            }
            
            //List<BinaryOperatorType> bin_operators = new List<BinaryOperatorType>();
            //foreach(BinaryOperatorType bin_operator in Enum.GetValues(typeof(BinaryOperatorType))){
            //    bin_operators.Add(bin_operator);
            //}

            //ViewData["BinaryOperators"] = bin_operators.ToArray<BinaryOperatorType>();
            
            return PartialView("Grid", null);
        }

        public ActionResult Add([ModelBinder(typeof(DevExpressEditorsBinder))] Rule ct)
        {
            ViewData["ApplicationInfoGuid"] = Request["ApplicationInfoGuid"];
            Rule rule = null;
            if (ModelState.IsValid)
            {
                try
                {
                    rule = new Rule(ct.Session);
                    rule.ApplicationInfo = rule.Session.FindObject<ApplicationInfo>(new BinaryOperator("Oid", Guid.Parse(Request["ApplicationInfoGuid"])));                    
                    rule.Field = ct.Field;
                    rule.Operator = Parse(Request["operator_combobox"]);//ct.Operator;
                    rule.Value = ct.Value;
                    rule.Entity = ct.Entity;
                    rule.Description = ct.Description;
                    rule.Save();

                    foreach (SerialNumber sn in rule.ApplicationInfo.SerialNumbers)
                    {
                        ValidationRule validation_rule = new ValidationRule(rule.Session);
                        validation_rule.Rule = rule;
                        sn.ValidationRules.Add(validation_rule);
                        sn.Save();
                    }
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                    ViewBag.CurrentItem = ct;
                }
            }
            else
            {
                Session["Error"] = "AnErrorOccurred";
                ViewBag.CurrentItem = ct;
            }
            return PartialView("Grid", rule.ApplicationInfo.Rules);

        }

        private BinaryOperatorType Parse(string operateror_str)
        {
            foreach(BinaryOperatorType bin_operator_type in Enum.GetValues(typeof(BinaryOperatorType))){
                if(bin_operator_type.ToString().Equals(operateror_str)){
                    return bin_operator_type;
                }
            }
            return BinaryOperatorType.Equal;
        }

        [HttpPost]
        public ActionResult Update([ModelBinder(typeof(DevExpressEditorsBinder))] Rule ct)
        {
            ViewData["ApplicationInfoGuid"] = Request["ApplicationInfoGuid"];
            Rule rule = null;
            if (ModelState.IsValid)
            {
                try
                {
                    rule = ct.Session.FindObject<Rule>(new BinaryOperator("Oid",ct.Oid));
                    rule.ApplicationInfo = rule.Session.FindObject<ApplicationInfo>(new BinaryOperator("Oid", Guid.Parse(Request["ApplicationInfoGuid"])));
                    rule.Field = ct.Field;
                    rule.Operator = Parse(Request["operator_combobox"]); //ct.Operator;
                    rule.Value = ct.Value;
                    rule.Entity = ct.Entity;
                    rule.Description = ct.Description;
                    rule.Save();
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = "AnErrorOccurred";
                ViewBag.CurrentItem = ct;
            }
            return PartialView("Grid", rule.ApplicationInfo.Rules);
        }
        public ActionResult Delete([ModelBinder(typeof(DevExpressEditorsBinder))] Rule ct)
        {
            ViewData["ApplicationInfoGuid"] = Request["ApplicationInfoGuid"];
            ApplicationInfo applicationInfo = ct.Session.FindObject<ApplicationInfo>(new BinaryOperator("Oid", Request["ApplicationInfoGuid"]));
            if (ModelState.IsValid)
            {
                try
                {
                    Rule rule = ct.Session.FindObject<Rule>(new BinaryOperator("Oid",ct.Oid));

                    XPCollection<ValidationRule> validation_rules = new XPCollection<ValidationRule>(rule.Session,new BinaryOperator("Rule.Oid",rule.Oid));
                    rule.Session.Delete(validation_rules);
                    rule.Delete();
                    
                }
                catch (Exception e)
                {
                    Session["Error"] = e.Message + Environment.NewLine + e.StackTrace;
                }
            }
            else
            {
                Session["Error"] = "AnErrorOccurred";
                ViewBag.CurrentItem = ct;
            }
            return PartialView("Grid", applicationInfo.Rules);
        }

    }
}
