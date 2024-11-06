using DevExpress.Data.Filtering;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Xpo;
using ITS.Retail.Common;
using ITS.Retail.Common.ViewModel;
using ITS.Retail.Model;
using ITS.Retail.Platform;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Enumerations.Attributes;
using ITS.Retail.ResourcesLib;
using ITS.Retail.WebClient.Helpers;
using ITS.Retail.WebClient.Helpers.AuxilliaryClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace ITS.Retail.WebClient.Controllers
{
    public class VariableController : BaseObjController<Variable>
    {
        [Security(ReturnsPartial = false), Display(ShowSettings = true)]
        public ActionResult Index()
        {
            this.ToolbarOptions.DeleteButton.OnClick = "DeleteSelectedRows";
            this.ToolbarOptions.EditButton.OnClick = "EditSelectedRowsFromGrid";
            this.ToolbarOptions.NewButton.OnClick = "AddNewFromGrid";
            this.ToolbarOptions.ViewButton.Visible = true;
            this.ToolbarOptions.ViewButton.OnClick = "ShowGenericView";
            this.ToolbarOptions.CustomButton.Visible = true;
            this.ToolbarOptions.CustomButton.Name = "RecalculateActionTypes";
            this.ToolbarOptions.CustomButton.OnClick = "Variable.RecalculateActionTypes";
            this.ToolbarOptions.CustomButton.CCSClass = "fase";
            this.ToolbarOptions.CustomButton.Title = Resources.Recalculate;
            this.CustomJSProperties.AddJSProperty("gridName", "grdVariables");

            this.ToolbarOptions.ExportToButton.Visible = false;
            this.ToolbarOptions.ExportButton.Visible = true;
            this.ToolbarOptions.ExportButton.OnClick = "Variable.ExportButtonOnClick";
            this.ToolbarOptions.ImportButton.Visible = true;
            this.ToolbarOptions.ImportButton.OnClick = "Variable.ImportButtonOnClick";

            return View("Index", GetList<Variable>(XpoSession));
        }

        protected override GenericViewRuleset GenerateGenericViewRuleset()
        {
            GenericViewRuleset ruleset = base.GenerateGenericViewRuleset();
            ruleset.PropertiesToIgnore.AddRange(new List<string>() { "IsUsedByOtherVariables", "HasCircularReference", "IsActive"});
            return ruleset;
        }

        public FileContentResult ExportVariables(string VariablesOids)
        {
            IEnumerable<Guid> VariablesGuids = null;
            try
            {
                VariablesGuids = VariablesOids.Split(',').Select(x => Guid.Parse(x));
                XPCollection<Variable> variables = GetList<Variable>(XpoSession, new InOperator("Oid", VariablesGuids));
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sr = new StreamWriter(ms))
                    {
                        List<string> objectsToSerialize = new List<string>();
                        foreach (Variable variable in variables)
                        {
                            objectsToSerialize.Add(variable.ToJson(PlatformConstants.JSON_SERIALIZER_SETTINGS));
                        }
                        sr.Write(JsonConvert.SerializeObject(objectsToSerialize, PlatformConstants.JSON_SERIALIZER_SETTINGS));
                        sr.Flush();
                        return File(ms.ToArray(), "text/plain", "import_Variables.txt");
                    }
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return null;
            }

        }

        public override ActionResult Dialog(List<string> arguments)
        {
            this.DialogOptions.AdjustSizeOnInit = true;
            this.DialogOptions.HeaderText = Resources.Import;
            this.DialogOptions.BodyPartialView = "ImportSettingsUpload";
            this.DialogOptions.OKButton.OnClick = @"function (s,e) { UploadControl.Upload();}";
            this.DialogOptions.CancelButton.OnClick = "function (s,e) { Dialog.Hide();}";
            return PartialView();
        }

        public ActionResult ImportSettingsUploadForm()
        {
            return PartialView("ImportSettingsUpload");
        }

        [Security(ReturnsPartial = false)]
        public ActionResult UploadControl()
        {
            UploadControlExtension.GetUploadedFiles("UploadControl", UploadControlValidationSettings, VariablesUpload_FileUploadComplete);
            return null;
        }

        private void VariablesUpload_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {
                try
                {
                    using (UnitOfWork uow = XpoHelper.GetNewUnitOfWork())
                    {
                        using (StreamReader reader = new StreamReader(e.UploadedFile.FileContent))
                        {
                            String jsonData = reader.ReadToEnd();
                            string error;
                            JArray jVariables = JsonConvert.DeserializeObject(jsonData) as JArray;
                            foreach (JToken jVariable in jVariables)
                            {
                                JObject jObjVariable = JsonConvert.DeserializeObject(jVariable.ToString()) as JObject;
                                if (jObjVariable is JObject)
                                {
                                    Variable variable = new Variable(uow);
                                    variable.FromJson(jObjVariable, PlatformConstants.JSON_SERIALIZER_SETTINGS, false, false, out error);
                                    variable.Owner = uow.GetObjectByKey<CompanyNew>(EffectiveOwner.Oid);
                                    variable.Save();
                                }
                            }
                            XpoHelper.CommitChanges(uow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Session["Error"] = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                }
            }
        }

        public static readonly UploadControlValidationSettings UploadControlValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".txt" },
            MaxFileSize = 200971520
        };
    

    protected override void FillLookupComboBoxes()
        {
            if (Request["DXCallbackArgument"].Contains("STARTEDIT") ||
                Request["DXCallbackArgument"].Contains("ADDNEWROW") || 
                Request["DXCallbackArgument"].Contains("UPDATEEDIT"))
            {
                Variable currentVariable = Session["Variable"] as Variable;
                IEnumerable<Variable> variables = GetList<Variable>(currentVariable.Session, new BinaryOperator("Oid", currentVariable.Oid, BinaryOperatorType.NotEqual));
                Dictionary<string, string> varInfo = new Dictionary<string, string>();
                foreach (Variable var in variables.Where(var => var.Category == currentVariable.Category))
                {
                    varInfo.Add(var.FieldName, var.Description);
                }
                ViewBag.Variables = varInfo;

                IEnumerable<string> varFieldsNames = typeof(VariableValues).GetProperties().Where(prop => prop.Name.Contains("Field")).Select(prop => prop.Name).Except(variables.Select(var => var.FieldName));
                Dictionary<string, string> varFields = new Dictionary<string, string>();
                foreach (string field in varFieldsNames)
                {
                    varFields.Add(field, field);
                }
                ViewBag.VariableFields = varFields;
            }
        }

        public override ActionResult Grid()
        {
            if (Request["DXCallbackArgument"] != null)
            {
                if (Request["DXCallbackArgument"].Contains("STARTEDIT"))
                {
                    Session["Variable"] = XpoHelper.GetNewUnitOfWork().GetObjectByKey<Variable>(RetailHelper.GetOidToEditFromDxCallbackArgument(Request["DXCallbackArgument"]));
                }
                else if (Request["DXCallbackArgument"].Contains("ADDNEWROW"))
                {
                    Session["Variable"] = new Variable(XpoHelper.GetNewUnitOfWork());
                    ViewBag.IsNewVariable = true;
                }
            }
            return base.Grid();
        }

        public override ActionResult InsertPartial(Variable ct)
        {
            string errorMessage;
            CheckVariableModelState(ct, out errorMessage);
            if (!String.IsNullOrEmpty(errorMessage))
            {
                Session["Error"] = errorMessage;
                ModelState.AddModelError("Expression", errorMessage);
                ViewBag.IsNewVariable = true;
                FillLookupComboBoxes();
            }
            return base.InsertPartial(ct);
        }

        public override ActionResult UpdatePartial(Variable ct)
        {
            string errorMessage;
            CheckVariableModelState(ct, out errorMessage);
            if (!String.IsNullOrEmpty(errorMessage))
            {
                Session["Error"] = errorMessage;
                ModelState.AddModelError("Expression", errorMessage);
                FillLookupComboBoxes();
            }
            return base.UpdatePartial(ct);
        }

        private Type GetInnerValueType(Type basicObjectType, string propertyName)
        {
            if (propertyName.Contains(Variable.PROPERTY_SEPERATOR))
            {
                string innerBasicObjectPropertyName = propertyName.Split(Variable.PROPERTY_SEPERATOR.ToCharArray()).FirstOrDefault();
                string innerPropertyName = propertyName.Substring(innerBasicObjectPropertyName.Length + Variable.PROPERTY_SEPERATOR.Length);
                Type innerBasicObjectType = basicObjectType.GetProperty(innerBasicObjectPropertyName).PropertyType;
                return GetInnerValueType(innerBasicObjectType, innerPropertyName);
            }
            return basicObjectType.GetProperty(propertyName).PropertyType;
        }

        private Type GetBasicObjectPropertyValueType(Type basicObjectType, string propertyName)
        {
            if (propertyName.Contains(Variable.PROPERTY_SEPERATOR))
            {
                return GetInnerValueType(basicObjectType, propertyName);
            }
            return basicObjectType.GetProperty(propertyName).PropertyType;
        }

        protected void CheckVariableModelState(Variable variable, out string message)
        {
            message = null;
            if (String.IsNullOrEmpty(variable.Expression))
            {
                message = Resources.PLEASE_FILL_ALL_REQUIRED_FIELDS;
            }
            else
            {
                if (variable.HasCircularReference)
                {
                    message = string.Format(Resources.VariableHasCircularReference, variable.Description, variable.GetVariableDependency(null).Info);
                }
                else
                {
                    Type basicObjectType = typeof(DocumentHeader);
                    if (categoryTypeMapping.ContainsKey(variable.Category))
                    {
                        basicObjectType = categoryTypeMapping[variable.Category];
                    }
                    
                    if (variable.Source == VariableSource.FIELD )
                    {
                        Type propertyType = GetBasicObjectPropertyValueType(basicObjectType, variable.Expression.TrimStart(Variable.FIELD_VARIABLE_START_CHARACTER.ToCharArray()).TrimEnd(Variable.FIELD_VARIABLE_END_CHARACTER.ToCharArray()));
                        Type assignToType = typeof(VariableValues).GetProperty(variable.FieldName).PropertyType;
                        if (assignToType.IsAssignableFrom(propertyType) == false)
                        {
                            message = Resources.VariableFieldTypeError;
                        }
                    }
                }
            }
        }

        public override ActionResult DeletePartial(Variable ct)
        {
            if (ct.IsUsedByOtherVariables)
            {
                Session["Error"] = Resources.VariableIsUsedByOtherVariables;
                return null;
            }

            return base.DeletePartial(ct);
        }

        static readonly Dictionary<ActionEntityCategory, Type> categoryTypeMapping = new Dictionary<ActionEntityCategory, Type>()
        {
            {ActionEntityCategory.DocumentItem, typeof(Item)},
            {ActionEntityCategory.DocumentItemStock, typeof(ItemStock)},
            {ActionEntityCategory.DocumentCustomer, typeof(Customer)},
            {ActionEntityCategory.DocumentDetails, typeof(DocumentDetail)},
            {ActionEntityCategory.DocumentSupplier, typeof(SupplierNew)},
            {ActionEntityCategory.DocumentPaymentMethod, typeof(PaymentMethod)},
            {ActionEntityCategory.Document, typeof(DocumentHeader)},

        };

        private List<string> GetBasicObjectProperties(VariableSource variableSource, Type type, string prefix = "", int level = 0)
        {
            if(level >2)
            {
                return new List<string>();
            }
            IEnumerable<PropertyInfo> basicObjProperties = type.GetProperties().Where(prop => typeof(BasicObj).IsAssignableFrom(prop.PropertyType));
            List<string> properties = GetBasicProperties(variableSource, type, prefix);
            string innerPrefix = string.IsNullOrWhiteSpace(prefix)? "": prefix + ".";
            basicObjProperties.ToList().ForEach(prop =>
                {
                    properties.AddRange(GetBasicObjectProperties(variableSource, prop.PropertyType, innerPrefix + prop.Name, level+1));
                });
            
            return properties;
        }

        private List<string> GetProperties(ActionEntityCategory? category, VariableSource? variableSource)
        {
            if (category.HasValue && variableSource.HasValue)
            {
                ActionTypeInfoAttribute actionTypeInfo = EnumerationHelper.GetAttribute<ActionTypeInfoAttribute>(category.Value);
                Type basicObjectType = typeof(BasicObj);
                Type masterType = basicObjectType.Assembly.GetType(basicObjectType.FullName.Replace(basicObjectType.Name, actionTypeInfo.MasterEntity));
                Type type = typeof(DocumentHeader);

                if (categoryTypeMapping.ContainsKey(category.Value))
                {
                    type = categoryTypeMapping[category.Value];
                }
                return GetBasicObjectProperties(variableSource.Value, type);
            }
            return new List<string>();
        }

        [HttpPost]
        public ActionResult EntityFieldsComboBox(ActionEntityCategory? category, VariableSource? variableSource)
        {       
            return PartialView("EntityFieldsComboBox", GetProperties(category, variableSource));
        }

        [HttpPost]
        public ActionResult TargetFieldComboBox(ActionEntityCategory? category, VariableSource? variableSource)
        {
            ViewBag.TargetFields = GetProperties(category, variableSource);
            return PartialView("TargetFieldComboBox", (Variable)Session["Variable"]);
        }

        private static List<string> GetBasicProperties(VariableSource variableSource, Type type, string prefix = "")
        {
            IEnumerable<PropertyInfo> propValues = null;
            switch (variableSource)
            {
                case VariableSource.FIELD:
                    propValues = type.GetProperties().Where(prop => 
                        !typeof(BasicObj).IsAssignableFrom(prop.PropertyType) &&
                        !typeof(IEnumerable<BasicObj>).IsAssignableFrom(prop.PropertyType));
                    break;
                case VariableSource.FORMULA:
                    propValues = type.GetProperties().Where(prop => prop.PropertyType != typeof(bool) &&
                                       (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(decimal))
                                       );
                    break;
            }
            List<string> properties;
            if (string.IsNullOrWhiteSpace(prefix))
            {
                properties = propValues.Select(value => value.Name).ToList();
            }
            else
            {
                properties = propValues.Select(value => prefix + "." + value.Name).ToList();
            }
            return properties;
        }

        public ActionResult ActionTypeRecalculateForm()
        {
            CriteriaOperator criteria = null;
            List<Guid> variableOids = new List<Guid>();
            if (Request["Variables"] != null)
            {
                List<string> oids = Request["Variables"].ToString().Split(',').ToList();
                foreach (string oid in oids)
                {
                    Guid guid = Guid.Empty;
                    if (Guid.TryParse(oid, out guid))
                    {
                        variableOids.Add(guid);
                    }
                }
                criteria = new ContainsOperator("VariableActionTypes", new InOperator("Variable", variableOids));
            }

            UnitOfWork uow = XpoHelper.GetNewUnitOfWork();

            XPCollection<ActionType> actionTypes = GetList<ActionType>(uow, criteria);

            ViewBag.ActionTypes = GetList<ActionType>(uow);
            return PartialView("../Shared/_ActionTypeRecalculateForm", new ActionRecalculateCriteria() { ActionTypeOids = actionTypes.Select(actionType => actionType.Oid).ToList() });
        }
    }
}
