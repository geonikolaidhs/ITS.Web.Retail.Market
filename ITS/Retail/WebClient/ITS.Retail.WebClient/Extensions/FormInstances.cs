using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.Web.Mvc.UI;
using DevExpress.Xpo;
using System.Web.Mvc.Html;
using ITS.Retail.ResourcesLib;
using System.Reflection;
using ITS.Retail.WebClient.AuxillaryClasses;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model;
using ITS.Retail.WebClient.Controllers;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.WebClient.Extensions
{
    public static class FormInstances
    {
        //Creates Settings for a FormLayout TextBox
        public static Action<MVCxFormLayoutItem> TextBoxItem(bool enabled = true, bool readOnly = false,
            string width = "medium", bool password = false, string Value = null, bool IsSearchable = false, bool IsRequired = false,
            string onValidate = null, string onValueChanged = null, int widthPercentage = 0, string caption = null, string name = null,
            LayoutItemCaptionLocation captionLocation = LayoutItemCaptionLocation.Top)
        {
            return (itemSettings) =>
            {
                itemSettings.NestedExtensionType = FormLayoutNestedExtensionItemType.TextBox;
                itemSettings.HelpTextStyle.BackgroundImage.ImageUrl = "~/Content/img/info.png";
                itemSettings.HelpTextSettings.Position = HelpTextPosition.Top;
                itemSettings.CaptionSettings.Location = captionLocation;
                if (caption != null)
                {
                    itemSettings.Caption = caption;
                }
                if (name != null)
                {
                    itemSettings.Name = name;
                }

                if (IsSearchable == true)
                {
                    itemSettings.CaptionCellStyle.CssClass = "isSearchable";
                }
                var textBoxSettings = (TextBoxSettings)itemSettings.NestedExtensionSettings;
                textBoxSettings.ReadOnly = readOnly;
                textBoxSettings.ControlStyle.CssClass = width;
                if (enabled == false)
                {
                    textBoxSettings.ReadOnly = !enabled;
                    textBoxSettings.ControlStyle.CssClass = width + " readOnlyInputElement";
                }
                textBoxSettings.Properties.ValidationSettings.RequiredField.IsRequired = IsRequired;
                textBoxSettings.ShowModelErrors = true;
                textBoxSettings.Properties.Password = password;
                textBoxSettings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                textBoxSettings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;

                if (widthPercentage != 0)
                {
                    textBoxSettings.Width = System.Web.UI.WebControls.Unit.Percentage(widthPercentage);
                }

                if (Value != null)
                {
                    textBoxSettings.Text = Value;
                }
                if (onValidate != null)
                {
                    textBoxSettings.Properties.ClientSideEvents.Validation = onValidate;
                }
                if (onValueChanged != null)
                {
                    textBoxSettings.Properties.ClientSideEvents.ValueChanged = onValueChanged;
                }
            };
        }
        ////Creates Settings for a FormLayout GDPR TextBox
        //public static MvcHtmlString GDPRTextBoxItem(HtmlHelper Html, string Caption, object DatabindingValue, bool GDPREnabled, string name, bool enabled = true, bool readOnly = false,
        //    string width = "medium", bool password = false, string Value = null, bool IsSearchable = false, bool IsRequired = false,
        //    string onValidate = null, string onValueChanged = null, int widthPercentage = 0,
        //    LayoutItemCaptionLocation captionLocation = LayoutItemCaptionLocation.Top)
        //{
        //    bool HasGDPRAttribute = false;
        //    if ((DatabindingValue.GetType().IsPrimitive || DatabindingValue.GetType() != typeof(System.String)))
        //    {
        //        HasGDPRAttribute = (DatabindingValue.GetType().GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0) ? true : false;
        //    }
        //    else if (DatabindingValue != null)
        //    {
        //        HasGDPRAttribute = (DatabindingValue.GetType().GetCustomAttributes(typeof(GDPRAttribute), false).Count() > 0) ? true : false;
        //    }
        //    if (GDPREnabled || (!GDPREnabled && HasGDPRAttribute))
        //    {
        //        var label = Html.DevExpress().Label(edtSettings =>
        //                         {
        //                             edtSettings.ControlStyle.CssClass = "label";
        //                             edtSettings.Text = Resources.Name + ":" + (HasGDPRAttribute ? "*" : "");
        //                             edtSettings.AssociatedControlName = name;

        //                         });
        //        var textbox = Html.DevExpress().TextBox(settings =>
        //         {
        //             settings.Name = name;
        //             settings.ControlStyle.CssClass = "editor";
        //             settings.Enabled = enabled;
        //             settings.ReadOnly = readOnly;
        //             settings.ControlStyle.CssClass = width;
        //             if (enabled == false)
        //             {
        //                 settings.ReadOnly = !enabled;
        //                 settings.ControlStyle.CssClass = width + " readOnlyInputElement";
        //             }
        //             settings.Properties.ValidationSettings.RequiredField.IsRequired = IsRequired;
        //             settings.ShowModelErrors = true;
        //             settings.Properties.Password = password;
        //             settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
        //             settings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
        //             if (widthPercentage != 0) settings.Width = System.Web.UI.WebControls.Unit.Percentage(widthPercentage);
        //             if (Value != null) settings.Text = Value;
        //             if (onValidate != null) settings.Properties.ClientSideEvents.Validation = onValidate;
        //             if (onValueChanged != null) settings.Properties.ClientSideEvents.ValueChanged = onValueChanged;
        //         }).Bind(DatabindingValue);
        //        return MvcHtmlString.Create(label.ToString() + textbox.ToString());
        //    }
        //    else
        //    {
        //        return Html.DevExpress().Label(edtSettings =>
        //        {
        //            edtSettings.ControlStyle.CssClass = "label";
        //            edtSettings.Text = "";

        //        }).GetHtml(); ;
        //    }
        //}

        //Creates Settings for a FormLayout UploadControlItem
        public static Action<MVCxFormLayoutItem> UploadControlItem(bool enabled, string[] allowedFileExtensions, string fileUploadComplete = "", string width = "medium", LayoutItemCaptionLocation captionLocation = LayoutItemCaptionLocation.Top, string caption = null)
        {
            return (itemSettings) =>
            {
                itemSettings.NestedExtensionType = FormLayoutNestedExtensionItemType.UploadControl;
                itemSettings.HelpTextStyle.BackgroundImage.ImageUrl = "~/Content/img/info.png";
                itemSettings.HelpTextSettings.Position = HelpTextPosition.Top;
                itemSettings.CaptionSettings.Location = captionLocation;
                if (caption != null)
                {
                    itemSettings.Caption = caption;
                }
                var settings = (UploadControlSettings)itemSettings.NestedExtensionSettings;
                settings.ControlStyle.CssClass = width;
                if (enabled == false)
                {
                    settings.ControlStyle.CssClass = width + " readOnlyInputElement";
                }

                settings.UploadMode = UploadControlUploadMode.Standard;
                settings.ValidationSettings.AllowedFileExtensions = allowedFileExtensions;
                settings.ValidationSettings.ShowErrors = true;
                settings.CallbackRouteValues = new { Controller = "Workflow", Action = "UploadControl" };
                settings.ClientSideEvents.FileUploadComplete = fileUploadComplete;
                //TO DO
                //settings.ValidationSettings.Assign(ITS.Workflow.Website.Controllers.WorkflowController.ValidationSettings);
            };
        }

        //Creates Settings for a FormLayout Memo
        public static Action<MVCxFormLayoutItem> MemoItem(bool enabled, bool readOnly = false,
            string width = "medium", int columns = 40, int rows = 6, string Value = null, string name = null, bool IsSearchable = false, string onValidate = null, string caption = null, LayoutItemCaptionLocation captionLocation = LayoutItemCaptionLocation.Top)
        {
            return (itemSettings) =>
            {
                itemSettings.NestedExtensionType = FormLayoutNestedExtensionItemType.Memo;
                itemSettings.HelpTextStyle.BackgroundImage.ImageUrl = "~/Content/img/info.png";
                itemSettings.HelpTextSettings.Position = HelpTextPosition.Right;

                itemSettings.CaptionSettings.Location = captionLocation;

                if (caption != null)
                {
                    itemSettings.Caption = caption;
                }

                if (name != null)
                {
                    itemSettings.Name = name;
                }

                if (IsSearchable == true)
                {
                    itemSettings.CaptionCellStyle.CssClass = "isSearchable";
                }
                var settings = (MemoSettings)itemSettings.NestedExtensionSettings;
                settings.ReadOnly = readOnly;
                settings.ControlStyle.CssClass = width;
                if (enabled == false)
                {
                    settings.ReadOnly = !enabled;
                    settings.ControlStyle.CssClass = width + " readOnlyInputElement";
                }

                settings.Properties.Columns = columns;
                settings.Properties.Rows = rows;
                settings.ShowModelErrors = true;
                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                settings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;

                if (Value != null)
                {
                    settings.Text = Value;
                }
                if (onValidate != null)
                {
                    settings.Properties.ClientSideEvents.Validation = onValidate;
                }
            };
        }

        //Creates Settings for a FormLayout Captca
        public static Action<MVCxFormLayoutItem> CaptcaItem()
        {
            return (itemSettings) =>
            {
                itemSettings.ShowCaption = DefaultBoolean.False;
                itemSettings.NestedExtensionType = FormLayoutNestedExtensionItemType.Captcha;
                itemSettings.HelpTextStyle.BackgroundImage.ImageUrl = "~/Content/img/info.png";
                itemSettings.HelpTextSettings.Position = HelpTextPosition.Right;
                var captchaSettings = (CaptchaSettings)itemSettings.NestedExtensionSettings;
                captchaSettings.TextBox.Position = DevExpress.Web.Captcha.ControlPosition.Bottom;
                captchaSettings.TextBox.ShowLabel = true;
                captchaSettings.TextBox.LabelText = Resources.PleaseFillCodeShown;
                captchaSettings.CodeLength = 6;
                captchaSettings.CharacterSet = "abcdefhjklmnpqrstuvxyz23456789";
                captchaSettings.RefreshButton.Visible = true;
                captchaSettings.ChallengeImage.FontFamily = "Tahoma";
                captchaSettings.ValidationSettings.EnableValidation = true;
                captchaSettings.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                captchaSettings.ValidationSettings.ErrorText = Resources.CaptchaIncorrectCode;
            };
        }

        //Creates Settings for a FormLayout LabelItem
        public static Action<MVCxFormLayoutItem> LabelItem(string text)
        {
            return (itemSettings) =>
            {
                itemSettings.NestedExtensionType = FormLayoutNestedExtensionItemType.Label;
                itemSettings.ShowCaption = DefaultBoolean.False;
                var label = itemSettings.NestedExtensionSettings as LabelSettings;
                label.Text = text;
            };
        }

        //Creates Settings for a FormLayout Date
        public static Action<MVCxFormLayoutItem> DateItem(bool enabled, string width = "medium", bool IsSearchable = false, bool required = false,
            bool shortFormat = false, bool timeVisible = true, string caption = null, string name = null, string onValueChanged = null, LayoutItemCaptionLocation captionLocation = LayoutItemCaptionLocation.Top)
        {
            return (itemSettings) =>
            {
                itemSettings.NestedExtensionType = FormLayoutNestedExtensionItemType.DateEdit;

                itemSettings.HelpTextStyle.BackgroundImage.ImageUrl = "~/Content/img/info.png";

                itemSettings.CaptionSettings.Location = captionLocation;

                //itemSettings.HelpTextSettings.Position = HelpTextPosition.Right;
                if (IsSearchable == true)
                {
                    itemSettings.CaptionCellStyle.CssClass = "isSearchable";
                }

                if (caption != null)
                {
                    itemSettings.Caption = caption;
                }
                if (name != null)
                {
                    itemSettings.Name = name;
                }

                var DateEditSettings = (DateEditSettings)itemSettings.NestedExtensionSettings;
                DateEditSettings.ControlStyle.CssClass = width;
                if (enabled == false)
                {
                    DateEditSettings.ReadOnly = !enabled;
                    DateEditSettings.Properties.DropDownButton.Enabled = false;
                    DateEditSettings.ControlStyle.CssClass = width + " readOnlyInputElement";
                }

                DateEditSettings.Properties.ClientSideEvents.ValueChanged = onValueChanged;


                DateEditSettings.Properties.EnableClientSideAPI = true;
                DateEditSettings.ShowModelErrors = true;
                DateEditSettings.Properties.EditFormat = EditFormat.Custom;

                if (shortFormat == true)
                {
                    DateEditSettings.Properties.EditFormatString = "dd/MM/yyyy";
                }
                else
                {
                    DateEditSettings.Properties.EditFormatString = "dd/MM/yyyy hh:mm tt";
                }

                if (timeVisible == true)
                {
                    DateEditSettings.Properties.TimeSectionProperties.Visible = true;
                }
                else
                {
                    DateEditSettings.Properties.TimeSectionProperties.Visible = false;
                }

            };
        }

        //Creates Settings for a FormLayout CheckBox
        public static Action<MVCxFormLayoutItem> CheckBoxItem(bool enabled, string cssClass = "", string caption = "", string onInit = "", string onChange = "", int colSpan = 1, int width = 0, FormLayoutHorizontalAlign horizontalAlign = FormLayoutHorizontalAlign.NotSet, FieldRequiredMarkMode required = FieldRequiredMarkMode.Auto)
        {
            return (itemSettings) =>
            {
                itemSettings.NestedExtensionType = FormLayoutNestedExtensionItemType.CheckBox;
                itemSettings.HelpTextStyle.BackgroundImage.ImageUrl = "~/Content/img/info.png";
                itemSettings.HelpTextSettings.Position = HelpTextPosition.Right;
                itemSettings.CaptionSettings.Location = LayoutItemCaptionLocation.Right;
                itemSettings.CaptionSettings.HorizontalAlign = FormLayoutHorizontalAlign.Left;

                itemSettings.RequiredMarkDisplayMode = required;

                itemSettings.CssClass = cssClass;
                if (caption != "")
                {
                    itemSettings.Caption = caption;
                }
                itemSettings.ColSpan = colSpan;

                if (width != 0)
                {
                    itemSettings.Width = width;
                }

                if (horizontalAlign != FormLayoutHorizontalAlign.NotSet)
                {
                    itemSettings.HorizontalAlign = FormLayoutHorizontalAlign.Center;
                }

                CheckBoxSettings settings = (CheckBoxSettings)itemSettings.NestedExtensionSettings;
                settings.Properties.ValueType = typeof(bool);
                settings.Properties.AllowGrayed = false;
                settings.Properties.ValueChecked = true;
                settings.Properties.ValueGrayed = false;
                settings.Properties.ValueUnchecked = false;
                settings.Properties.EnableClientSideAPI = true;
                settings.Properties.ClientSideEvents.CheckedChanged = onChange;
                settings.Properties.ClientSideEvents.Init = onInit;

                //settings.ClientEnabled = enabled;
                if (enabled == false)
                {
                    settings.ReadOnly = !enabled;
                    settings.ControlStyle.CssClass = "readOnlyInputElement";
                }
            };
        }

        //Creates Settings for a FormLayout TokenBoxItem
        public static Action<MVCxFormLayoutItem> TokenBoxItem<T>(bool enabled, IEnumerable<T> dataSource,
            string width = "medium", string textField = "Description", string valueField = "Oid",
            string onTokenChange = "", string onInit = "", bool IsSearchable = false)
        {
            return (itemSettings) =>
            {
                itemSettings.NestedExtensionType = FormLayoutNestedExtensionItemType.TokenBox;
                itemSettings.HelpTextStyle.BackgroundImage.ImageUrl = "~/Content/img/info.png";
                itemSettings.HelpTextSettings.Position = HelpTextPosition.Right;
                if (IsSearchable == true)
                {
                    itemSettings.CaptionCellStyle.CssClass = "isSearchable";
                }
                TokenBoxSettings settings = (TokenBoxSettings)itemSettings.NestedExtensionSettings;
                settings.Properties.AllowCustomTokens = false;
                settings.Properties.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
                settings.Properties.ShowDropDownOnFocus = ShowDropDownOnFocusMode.Always;
                settings.Properties.ClientSideEvents.TokensChanged = onTokenChange;
                settings.Properties.ClientSideEvents.Init = onInit;
                settings.Properties.TextField = textField;
                settings.Properties.ValueField = valueField;
                settings.ControlStyle.CssClass = width;
                settings.ShowModelErrors = true;
                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                settings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
                if (enabled == false)
                {
                    settings.ReadOnly = !enabled;
                    settings.Properties.DropDownButton.Enabled = false;
                    settings.ControlStyle.CssClass = width + " readOnlyInputElement";
                }
                settings.Properties.DataSource = dataSource;
            };
        }

        //Creates Settings for a FormLayout ComboBox
        public static Action<MVCxFormLayoutItem> ComboBoxItem<T>(HtmlHelper Html, object Model,
            bool enabled, IEnumerable<T> dataSource, string valueField = "Oid",
            string textField = "Description", Type valueType = null, string width = "medium",
            bool IsSearchable = false, string OnValueChangedJs = "", string OnInitJs = "", string OnGotFocus = "", string name = "",
            bool validation = true, LayoutItemCaptionLocation captionPosition = LayoutItemCaptionLocation.Top, string caption = "", int colSpan = 1, bool isRequired = false, bool validationPositionRight = false)
        {

            return (itemSettings) =>
            {
                object bindingList = dataSource;
                if (valueField != null && dataSource != null)
                {
                    PropertyInfo propInfo = typeof(T).GetProperty(valueField);
                    if (!enabled)
                    {
                        object val = null;
                        if (Model != null)
                        {
                            if (Model.GetType().IsEnum)
                            {
                                val = ((Enum)Model).ToLocalizedString();
                            }
                            else
                            {
                                val = Model.GetType() == valueType ? Model : propInfo.GetValue(Model, null);
                            }
                            bindingList = dataSource.Where(x => propInfo.GetValue(x, null).Equals(val));
                        }
                    }
                }
                else
                {
                    bindingList = dataSource;
                }
                if (String.IsNullOrWhiteSpace(name))
                {
                    name = itemSettings.NestedExtensionSettings.Name;
                }
                object bind = null;
                if (Model != null && (Model.Equals(Guid.Empty) == false || typeof(Enum).IsAssignableFrom(Model.GetType())))
                {
                    bind = Model;
                }



                if (caption != "")
                {
                    itemSettings.Caption = caption;
                }
                itemSettings.HelpTextStyle.BackgroundImage.ImageUrl = "~/Content/img/info.png";
                itemSettings.HelpTextSettings.Position = HelpTextPosition.Right;
                itemSettings.CaptionSettings.Location = captionPosition;
                itemSettings.ColSpan = colSpan;

                if (IsSearchable == true)
                {
                    itemSettings.CaptionCellStyle.CssClass = "isSearchable";
                }
                itemSettings.SetNestedContent(() =>
                {
                    Html.DevExpress().ComboBox(settings =>
                    {
                        settings.Name = name;
                        settings.Properties.DropDownStyle = DropDownStyle.DropDownList;
                        settings.Properties.IncrementalFilteringMode = IncrementalFilteringMode.None;
                        if (valueField != null)
                        {
                            settings.Properties.ValueField = valueField;
                        }
                        if (textField != null)
                        {
                            settings.Properties.TextField = textField;
                        }
                        settings.Properties.ValueType = valueType;
                        settings.Properties.EnableClientSideAPI = true;
                        settings.Properties.ClientSideEvents.ValueChanged = OnValueChangedJs;
                        settings.Properties.ClientSideEvents.Init = OnInitJs;
                        settings.Properties.ClientSideEvents.GotFocus = OnGotFocus;


                        settings.ControlStyle.CssClass = width;
                        if (enabled == false)
                        {
                            settings.ReadOnly = !enabled;
                            settings.Properties.DropDownButton.Enabled = false;
                            settings.ControlStyle.CssClass = width + " readOnlyInputElement";
                        }
                        if (settings.Properties.Items.Count >= 10)
                            settings.Properties.DropDownRows = 10;
                        if (validation)
                        {
                            settings.ShowModelErrors = true;

                            if (validationPositionRight)
                            {
                                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithTooltip;
                                settings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Right;
                            }
                            else
                            {
                                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                                settings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
                            }

                            settings.Properties.ValidationSettings.RequiredField.IsRequired = isRequired;
                        }
                    }).BindList(bindingList).Bind(bind).Render();
                });
            };
        }


        //Creates Settings for a FormLayout RadioButtonList
        public static Action<MVCxFormLayoutItem> RadioButtonListItem(bool enabled, int items = 2, string OnValueChangedJs = "", string OnInitJs = "", int selectedIndex = 0)
        {
            return (itemSettings) =>
            {
                itemSettings.NestedExtensionType = FormLayoutNestedExtensionItemType.RadioButtonList;
                itemSettings.HelpTextStyle.BackgroundImage.ImageUrl = "~/Content/img/info.png";
                itemSettings.HelpTextSettings.Position = HelpTextPosition.Right;
                RadioButtonListSettings settings = (RadioButtonListSettings)itemSettings.NestedExtensionSettings;
                for (int i = 0; i < items; i++)
                {
                    settings.Properties.Items.Add();
                }
                if (enabled == false)
                {
                    settings.ReadOnly = !enabled;
                    settings.ControlStyle.CssClass = "readOnlyInputElement";
                }

                settings.Properties.ClientSideEvents.ValueChanged = OnValueChangedJs;
                settings.Properties.ClientSideEvents.Init = OnInitJs;

                settings.Properties.RepeatDirection = System.Web.UI.WebControls.RepeatDirection.Horizontal;
                settings.SelectedIndex = selectedIndex;
            };
        }

        //Creates Settings for a FormLayout SpinEditItem
        public static Action<MVCxFormLayoutItem> SpinEditItem(bool enabled, string width = "medium",
            SpinEditNumberType type = SpinEditNumberType.Float, bool IsSearchable = false, string NumberChanged = "", string caption = "",
            LayoutItemCaptionLocation captionLocation = LayoutItemCaptionLocation.Top, FormLayoutNestedExtensionItemType extensionType = FormLayoutNestedExtensionItemType.SpinEdit,
            int colSpan = 1, int widthPixels = 0, decimal minValue = 0, decimal maxValue = 999999999, bool allowMouseWheel = true,
            bool showIncrementButtons = true, string displayFormat = null, string name = null, ErrorDisplayMode errorDisplayMode = ErrorDisplayMode.ImageWithText, ErrorTextPosition errorTextPosition = ErrorTextPosition.Bottom)
        {

            return (itemSettings) =>
            {

                itemSettings.HelpTextStyle.BackgroundImage.ImageUrl = "~/Content/img/info.png";
                itemSettings.HelpTextSettings.Position = HelpTextPosition.Right;

                if (name != null)
                {
                    itemSettings.Name = name;
                }
                if (caption != "")
                {
                    itemSettings.Caption = caption;
                }
                itemSettings.CaptionSettings.Location = captionLocation;
                itemSettings.NestedExtensionType = extensionType;
                itemSettings.ColSpan = colSpan;


                if (IsSearchable == true)
                {
                    itemSettings.CaptionCellStyle.CssClass = "isSearchable";
                }
                SpinEditSettings settings = (SpinEditSettings)itemSettings.NestedExtensionSettings;
                settings.ControlStyle.CssClass = width;
                if (enabled == false)
                {

                    settings.ReadOnly = !enabled;
                    settings.Properties.SpinButtons.Enabled = false;
                    settings.ControlStyle.CssClass = width + " readOnlyInputElement";
                    settings.ClientEnabled = enabled;
                }

                settings.Properties.MinValue = minValue;
                settings.Properties.MaxValue = maxValue;
                settings.Properties.NumberType = type;
                settings.Properties.ClientSideEvents.NumberChanged = NumberChanged;
                settings.ShowModelErrors = true;
                settings.Properties.ValidationSettings.ErrorDisplayMode = errorDisplayMode;
                settings.Properties.ValidationSettings.ErrorTextPosition = errorTextPosition;
                settings.Properties.AllowMouseWheel = allowMouseWheel;
                settings.Properties.SpinButtons.ShowIncrementButtons = showIncrementButtons;
                if (displayFormat != null)
                {
                    settings.Properties.DisplayFormatString = displayFormat;
                }
                if (settings.Name == "DefaultDocumentDiscount")
                {
                    settings.Properties.DisplayFormatString = "p";
                    settings.Properties.DisplayFormatInEditMode = false;
                    settings.Properties.NumberFormat = SpinEditNumberFormat.Number;
                    settings.Properties.NumberType = SpinEditNumberType.Float;
                    settings.Properties.MinValue = 0;
                    settings.Properties.MaxValue = 100;
                    settings.Properties.Increment = 1M;
                    settings.Properties.DecimalPlaces = 2;
                }

                if (widthPixels != 0)
                {
                    settings.Width = widthPixels;
                }
            };
        }

        //Creates Settings for a FormLayout DropDownEdit
        public static Action<MVCxFormLayoutItem> ListEditItem(HtmlHelper html, ViewContext viewContext, bool enabled, object datasource, object model,
            int height = 200, string valueField = "Oid", string Name = "",
            string textField = "Description", Type valueType = null, string width = "medium", string onSelectedIndexChanged = "",
            int widthPixels = 200, bool performValidation = false, FormLayoutVerticalAlign layoutVerticalAlign = FormLayoutVerticalAlign.NotSet, string cssClass = "",
            FieldRequiredMarkMode required = FieldRequiredMarkMode.Auto)
        {
            return (itemSettings) =>
            {
                if (layoutVerticalAlign != FormLayoutVerticalAlign.NotSet)
                {
                    itemSettings.VerticalAlign = layoutVerticalAlign;
                }

                if (!String.IsNullOrWhiteSpace(cssClass))
                {
                    itemSettings.CssClass = cssClass;
                }

                itemSettings.RequiredMarkDisplayMode = required;

                string name = string.IsNullOrEmpty(Name) == false ? Name : itemSettings.NestedExtensionSettings.Name;
                if (valueType == null)
                {
                    valueType = typeof(Guid);
                }
                itemSettings.SetNestedContent(() =>
                {
                    viewContext.Writer.Write(html.Hidden(name + "_initial", model));
                    viewContext.Writer.Write(html.Hidden(name + "_selected", model));

                    html.DevExpress().ListBox(ddesettings =>
                    {
                        ddesettings.Name = name;
                        ddesettings.Properties.SelectionMode = ListEditSelectionMode.CheckColumn;



                        ddesettings.Properties.ValueField = valueField;
                        ddesettings.Properties.TextField = textField;
                        ddesettings.Properties.ValueType = valueType;
                        ddesettings.ControlStyle.CssClass = width;
                        if (enabled == false)
                        {
                            ddesettings.ReadOnly = !enabled;
                            ddesettings.ControlStyle.CssClass = width + " readOnlyInputElement";
                        }
                        ddesettings.Properties.ClientSideEvents.SelectedIndexChanged =
                        @"function (s,e) {
                            var selectedValues = " + name + @".GetSelectedValues()
                            $('#" + name + @"_selected').val(selectedValues);
                            " + (String.IsNullOrWhiteSpace(onSelectedIndexChanged) ? "" : onSelectedIndexChanged + "(s,e);") + @"
                        }";

                        ddesettings.Properties.ClientSideEvents.Init =
                        @"function (s, e) {    
                          //debugger;                      
                          var initialOrgs = $('#" + name + @"_initial').val();
                          var values = initialOrgs.split(',');
                          " + name + @".SelectValues(values);
                        }";

                        ddesettings.Height = height;

                        ddesettings.Width = widthPixels;
                        if (performValidation)
                        {
                            ddesettings.ShowModelErrors = true;
                            ddesettings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                            ddesettings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
                        }
                        ddesettings.Properties.EnableClientSideAPI = true;

                        ddesettings.ControlStyle.VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top;

                    }).BindList(datasource).Render();

                    itemSettings.CaptionSettings.Location = LayoutItemCaptionLocation.Top;
                });

            };
        }

        //Creates Settings for a content with ComboBox and SpinEdit 
        public static Action<MVCxFormLayoutItem> ComboBoxPlusSpinEditContent(HtmlHelper html, ViewContext viewContext, object Model, string comboBindingProperty, string spineditBindingProperty, object bindingList, string name, bool enabled,
            Type type, string valueField, string textField, string width = "small", string NumberChanged = "")
        {
            return (itemSettings) =>
            {
                itemSettings.CssClass = "GroupDiscountsPercentage";
                itemSettings.SetNestedContent(() =>
                {
                    viewContext.Writer.Write("<table><tr><td>");
                    html.DevExpress().SpinEdit(s =>
                    {
                        s.Name = name + "_Value";
                        s.Properties.MinValue = 0;
                        s.Properties.MaxValue = 99999999999;
                        s.ControlStyle.CssClass = width;

                        if (enabled == false)
                        {
                            s.ReadOnly = !enabled;
                            s.Properties.SpinButtons.Enabled = false;
                            s.ControlStyle.CssClass = width + " readOnlyInputElement";
                        }
                    }).Bind(DataBinder.Eval(Model, spineditBindingProperty)).Render();

                    viewContext.Writer.Write("</td><td>");

                    html.DevExpress().ComboBox(settings =>
                    {
                        settings.Name = name + "_Type";
                        settings.Properties.DropDownStyle = DropDownStyle.DropDownList;
                        settings.Properties.IncrementalFilteringMode = IncrementalFilteringMode.None;
                        settings.ControlStyle.CssClass = width;
                        if (enabled == false)
                        {
                            settings.ReadOnly = !enabled;
                            settings.Properties.DropDownButton.Enabled = false;
                            settings.ControlStyle.CssClass = width + " readOnlyInputElement";
                        }
                        settings.ShowModelErrors = true;
                        settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                        settings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
                        settings.Properties.ClientSideEvents.SelectedIndexChanged = NumberChanged;
                        settings.Properties.ValueType = type;
                        settings.Properties.ValueField = valueField;
                        settings.Properties.TextField = textField;
                    }).BindList(bindingList).Bind(DataBinder.Eval(Model, comboBindingProperty)).Render();

                    viewContext.Writer.Write("</td></tr></table>");
                });
            };
        }

        public static Action<MVCxFormLayoutItem> ButtonItem(bool enabled, string OnClickJs = "", bool useSubmitBehaviour = false, string text = "")
        {
            return (itemSettings) =>
            {
                itemSettings.NestedExtensionType = FormLayoutNestedExtensionItemType.Button;
                itemSettings.ShowCaption = DefaultBoolean.False;
                ButtonSettings settings = (ButtonSettings)itemSettings.NestedExtensionSettings;
                settings.ClientEnabled = enabled;
                settings.UseSubmitBehavior = useSubmitBehaviour;
                settings.EnableClientSideAPI = true;
                settings.Text = text;
                if (String.IsNullOrWhiteSpace(OnClickJs) == false)
                {
                    settings.ClientSideEvents.Click = OnClickJs;
                }
            };
        }


        public static Action<MVCxFormLayoutItem> TimeItem(bool enabled, bool showModelErrors = false)
        {
            return (itemSets) =>
            {
                itemSets.NestedExtensionType = FormLayoutNestedExtensionItemType.TimeEdit;
                TimeEditSettings timeSets = itemSets.NestedExtensionSettings as TimeEditSettings;
                timeSets.Properties.EditFormatString = "HH:mm:ss";
                timeSets.ShowModelErrors = showModelErrors;
            };

        }

        public static ComboBoxExtension DropDownListSearchableComboBoxBody(HtmlHelper Html, bool enabled,
            ItemsRequestedByFilterConditionMethod itemsRequestedByFilterConditionMethod,
            ItemRequestedByValueMethod itemRequestedByValueMethod, List<DropDownSearchColumn> dropDownSearchColumns,
            string name,
            string callbackRouteValuesController = "", string callbackRouteValuesAction = "", string callbackRouteValuesArea = "",
            string clientSideEventsInit = "", string clientSideEventsKeyPress = "", string clientSideEventsSelectedIndexChanged = "",
            string clientSideEventsGotFocus = "", string clientSideEventsLostFocus = "", string clientSideEventsValueChanged = "", string clientSidesEventsButtonClick = "",
            string valueField = "Oid", string textField = "Description", Type valueType = null, string width = "medium",
            int callbackPageSize = 15, int filterMinLength = 0, int dropDownRows = 10, int incrementalFilteringDelay = 1000, string textFormatString = "{0}",
            object model = null, Type modelType = null, string deleteImg = "", string searchImg = "", string caption = "", EditorCaptionPosition captionPosition = EditorCaptionPosition.Top)
        {
            return Html.DevExpress().ComboBox(settings =>
            {
                settings.Name = name;
                settings.Properties.DropDownStyle = DropDownStyle.DropDownList;
                if (string.IsNullOrEmpty(callbackRouteValuesArea))
                {
                    settings.CallbackRouteValues = new { Controller = callbackRouteValuesController, Action = callbackRouteValuesAction, Name = name };
                }
                else
                {
                    settings.CallbackRouteValues = new { Area = callbackRouteValuesArea, Controller = callbackRouteValuesController, Action = callbackRouteValuesAction, Name = name };
                }


                settings.Properties.CallbackPageSize = callbackPageSize;
                settings.Properties.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
                if (valueField != null)
                {
                    settings.Properties.ValueField = valueField;
                }
                if (textField != null)
                {
                    settings.Properties.TextField = textField;
                }
                settings.Properties.ValueType = valueType;
                settings.Properties.FilterMinLength = filterMinLength;
                settings.Properties.EnableClientSideAPI = true;
                settings.Properties.TextFormatString = textFormatString;
                settings.ControlStyle.CssClass = width;
                if (enabled == false)
                {
                    settings.ReadOnly = !enabled;
                    settings.Properties.DropDownButton.Enabled = false;
                    settings.ControlStyle.CssClass = width + " readOnlyInputElement";
                }
                if (settings.Properties.Items.Count >= dropDownRows)
                {
                    settings.Properties.DropDownRows = dropDownRows;
                }
                settings.ShowModelErrors = true;
                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                settings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;
                settings.Properties.Caption = caption;
                settings.Properties.CaptionSettings.Position = captionPosition;
                settings.Properties.ClientSideEvents.Init = clientSideEventsInit;
                settings.Properties.ClientSideEvents.KeyPress = clientSideEventsKeyPress;
                settings.Properties.ClientSideEvents.SelectedIndexChanged = clientSideEventsSelectedIndexChanged;
                settings.Properties.ClientSideEvents.GotFocus = clientSideEventsGotFocus;
                settings.Properties.ClientSideEvents.LostFocus = clientSideEventsLostFocus;
                settings.Properties.ClientSideEvents.ValueChanged = clientSideEventsValueChanged;

                settings.Properties.IncrementalFilteringDelay = incrementalFilteringDelay;


                if (!String.IsNullOrEmpty(deleteImg) || !String.IsNullOrEmpty(searchImg))
                {
                    settings.Properties.ClientSideEvents.ButtonClick = clientSidesEventsButtonClick;
                }
                if (!String.IsNullOrEmpty(deleteImg))
                {
                    settings.Properties.NullText = Resources.ComboBoxNote;
                    var button = new EditButton(" ");
                    button.Image.Url = deleteImg;
                    button.Image.AlternateText = Resources.Clear;
                    settings.Properties.Buttons.Add(button);
                }
                if (!String.IsNullOrEmpty(searchImg))
                {
                    settings.Properties.NullText = Resources.ComboBoxNote;
                    var button = new EditButton(" ");
                    button.Image.Url = searchImg;
                    button.Image.AlternateText = Resources.Search;
                    settings.Properties.Buttons.Add(button);
                }




                if (dropDownSearchColumns != null && dropDownSearchColumns.Count > 0)
                {
                    foreach (DropDownSearchColumn dropDownSearchColumn in dropDownSearchColumns)
                    {
                        settings.Properties.Columns.Add(dropDownSearchColumn.Field, dropDownSearchColumn.Display, dropDownSearchColumn.Size);
                    }
                }
            }).BindList(itemsRequestedByFilterConditionMethod, itemRequestedByValueMethod)
             .BindList(
                        modelType == null || model == null || (model.GetType() != typeof(Guid) && model.GetType() != typeof(BasicObj)) ? null
                                    : (
                                        model.GetType() == typeof(Guid) ? BaseController.GetObjectByValue(modelType, model)
                                        : BaseController.GetObjectByValue(model.GetType(), DataBinder.Eval(model, "Oid"))
                                      )
                       ).Bind(model) as ComboBoxExtension;
        }



        public static ComboBoxExtension DropDownListNotSearchableComboBoxBody(HtmlHelper Html, bool enabled,
          string callbackRouteValuesController, string callbackRouteValuesAction, string callbackRouteValuesArea,
          ItemsRequestedByFilterConditionMethod itemsRequestedByFilterConditionMethod,
          ItemRequestedByValueMethod itemRequestedByValueMethod, List<DropDownSearchColumn> dropDownSearchColumns,
          string name,
          string clientSideEventsInit = "", string clientSideEventsKeyPress = "", string clientSideEventsSelectedIndexChanged = "",
          string clientSideEventsGotFocus = "", string clientSideEventsLostFocus = "", string clientSideEventsValueChanged = "",
          string valueField = "Oid", string textField = "Description", Type valueType = null, Type modelType = null, string width = "medium",
          int callbackPageSize = 15, int filterMinLength = 0, int dropDownRows = 10, int incrementalFilteringDelay = 1000, string textFormatString = "{0}",
          object model = null, string beginCallback = "")
        {
            return Html.DevExpress().ComboBox(settings =>
            {
                settings.Name = name;
                settings.Properties.DropDownStyle = DropDownStyle.DropDown;
                if (string.IsNullOrEmpty(callbackRouteValuesArea))
                {
                    settings.CallbackRouteValues = new { Controller = callbackRouteValuesController, Action = callbackRouteValuesAction, Name = name };
                }
                else
                {
                    settings.CallbackRouteValues = new { Area = callbackRouteValuesArea, Controller = callbackRouteValuesController, Action = callbackRouteValuesAction, Name = name };
                }


                settings.Properties.CallbackPageSize = callbackPageSize;
                settings.Properties.IncrementalFilteringMode = IncrementalFilteringMode.Contains;
                if (valueField != null)
                {
                    settings.Properties.ValueField = valueField;
                }
                if (textField != null)
                {
                    settings.Properties.TextField = textField;
                }
                settings.Properties.ValueType = valueType;
                settings.Properties.FilterMinLength = filterMinLength;
                settings.Properties.EnableClientSideAPI = true;
                settings.Properties.TextFormatString = textFormatString;
                settings.ControlStyle.CssClass = width;
                if (enabled == false)
                {
                    settings.ReadOnly = !enabled;
                    settings.Properties.DropDownButton.Enabled = false;
                    settings.ControlStyle.CssClass = width + " readOnlyInputElement";
                }
                if (settings.Properties.Items.Count >= dropDownRows)
                {
                    settings.Properties.DropDownRows = dropDownRows;
                }
                settings.ShowModelErrors = true;
                settings.Properties.ValidationSettings.ErrorDisplayMode = ErrorDisplayMode.ImageWithText;
                settings.Properties.ValidationSettings.ErrorTextPosition = ErrorTextPosition.Bottom;

                settings.Properties.ClientSideEvents.Init = clientSideEventsInit;
                settings.Properties.ClientSideEvents.KeyPress = clientSideEventsKeyPress;
                settings.Properties.ClientSideEvents.SelectedIndexChanged = clientSideEventsSelectedIndexChanged;
                settings.Properties.ClientSideEvents.GotFocus = clientSideEventsGotFocus;
                settings.Properties.ClientSideEvents.LostFocus = clientSideEventsLostFocus;
                settings.Properties.ClientSideEvents.ValueChanged = clientSideEventsValueChanged;

                settings.Properties.IncrementalFilteringDelay = incrementalFilteringDelay;

                if (!String.IsNullOrEmpty(beginCallback))
                {
                    settings.Properties.ClientSideEvents.BeginCallback = beginCallback;
                }

                if (dropDownSearchColumns != null && dropDownSearchColumns.Count > 0)
                {
                    foreach (DropDownSearchColumn dropDownSearchColumn in dropDownSearchColumns)
                    {
                        settings.Properties.Columns.Add(dropDownSearchColumn.Field, dropDownSearchColumn.Display, dropDownSearchColumn.Size);
                    }
                }
            }).BindList(itemsRequestedByFilterConditionMethod, itemRequestedByValueMethod).BindList(
                        modelType == null || model == null || (model.GetType() != typeof(Guid) && model.GetType() != typeof(BasicObj)) ? null
                                    : (
                                        model.GetType() == typeof(Guid) ? BaseController.GetObjectByValue(modelType, model)
                                        : BaseController.GetObjectByValue(model.GetType(), DataBinder.Eval(model, "Oid"))
                                      )).Bind(model) as ComboBoxExtension;
        }

    }
}