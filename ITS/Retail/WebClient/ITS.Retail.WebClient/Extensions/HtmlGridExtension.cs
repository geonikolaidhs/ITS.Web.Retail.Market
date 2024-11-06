using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using ITS.Retail.Model;
using ITS.Retail.ResourcesLib;
using System.Collections;
using System.Web.UI.WebControls;
using DevExpress.Xpo;
using ITS.Retail.Model.Attributes;
using ITS.Retail.Platform.Enumerations;
using System.Web.UI;

namespace ITS.Retail.WebClient.Extensions
{
    /// <summary>
    /// Απαρίθμηση για την μορφή των βασικών λειτουργιών του Grid
    /// </summary>
    public enum GridViewCommandColumnType
    {
        /// <summary>
        /// Δεν εμφανίζεται δυνατότητα Edit,New,Delete
        /// </summary>
        NONE,
        /// <summary>
        /// Χρησιμοποιείται η εξωτερική toolbar και εμφανίζονται checkboxes σε κάθε εγγραφή
        /// </summary>
        CHECKBOX,
        /// <summary>
        /// Χρησιμοποιούνται εικονίδια για τις ενέργειες Edit,New,Delete
        /// </summary>
        ICONS
    }

    public static class PopUpControlExtension
    {
        public static PopupControlSettings GetDefaultSettings(string name, string ShownEventFunction = "", short Width = 0, short Height = 0)
        {            
            PopupControlSettings popUpSettings = new PopupControlSettings();
            popUpSettings.Name = name;
            
            string shownEvent = "function(s,e) {" + popUpSettings.Name + ".SetHeight(window.innerHeight -20);" + popUpSettings.Name + ".SetWidth(window.innerWidth -20);  }";
            if(!String.IsNullOrEmpty(ShownEventFunction)){
                shownEvent = "function(s,e) { " + ShownEventFunction + "(s,e); " + popUpSettings.Name + ".SetHeight(window.innerHeight -20);" + popUpSettings.Name + ".SetWidth(window.innerWidth -20);  }";
            }

            popUpSettings.CloseAction = CloseAction.CloseButton;
            popUpSettings.Modal = true;

            if (Width!=0)
            {
                popUpSettings.Width = Width;      
            }
            else
            {
                popUpSettings.ClientSideEvents.Shown = shownEvent;
            }

            if (Height != 0)
            {
                popUpSettings.Height = Height;  
            }

            popUpSettings.ShowCloseButton = true;
            popUpSettings.ShowHeader = true;


            if (Width == 0 && Height == 0)
            {
                popUpSettings.PopupHorizontalOffset = 10;
                popUpSettings.PopupVerticalOffset = 10;
            }
            else
            {
                popUpSettings.PopupHorizontalAlign = PopupHorizontalAlign.WindowCenter;
                popUpSettings.PopupVerticalAlign = PopupVerticalAlign.WindowCenter;
            }

            popUpSettings.PopupAnimationType = AnimationType.Fade;
            popUpSettings.AllowDragging = true;
            popUpSettings.DragElement = DragElement.Header;
            popUpSettings.AllowResize = true;
            popUpSettings.ResizingMode = ResizingMode.Live;
            popUpSettings.AppearAfter = 200;
            popUpSettings.DisappearAfter = 200;
            popUpSettings.ScrollBars = ScrollBars.Auto;
            popUpSettings.ShowPageScrollbarWhenModal = true;
            popUpSettings.AutoUpdatePosition = true;
            return popUpSettings;
        }
    }


    /// <summary>
    /// Επεκτάσεις για το Grid της DevExpress
    /// </summary>
    public static class HtmlGridExtension
    {
        /// <summary>
        /// Creates settings for a GridView Extension with defined parameters
        /// </summary>
        /// <param name="objectType">The object type bound to grid</param>
        /// <param name="controllerType">The controller type</param>
        /// <param name="columnType">The type of command column</param>
        /// <param name="generateColumns">Defines if columns bound to defined properties will be generated</param>
        /// <param name="showAddIcon">If command column type ICOS is selected, defines if add button will be visible</param>
        /// <param name="showEditIcon">If command column type ICOS is selected, defines if edit button will be visible</param>
        /// <param name="showDeleteIcon">If command column type ICOS is selected, defines if delete button will be visible</param>
        /// <returns>A GridViewSettings object</returns>
        public static GridViewSettings GetDefaultSettings(ViewContext viewContext,Type objectType, Type controllerType, string gridName,
            GridViewCommandColumnType columnType = GridViewCommandColumnType.CHECKBOX,            
            bool generateColumns = true, bool showAddIcon = true, bool showEditIcon = true, 
            bool showDeleteIcon = true, List<string> excludeFields = null,List<string> includedFields = null)
        {
            string baseName = controllerType.Name.Replace("Controller", "");
            string baseType = objectType.Name;
            GridViewSettings settings = new GridViewSettings();
            settings.Name = gridName;  
            settings.SettingsCookies.StoreColumnsVisiblePosition = true;
            settings.SettingsCookies.StoreColumnsWidth = true;
            settings.SettingsCookies.StorePaging = true;
            settings.SettingsPager.PageSizeItemSettings.Visible = true;
            settings.SettingsPager.PageSizeItemSettings.Items = new string[] { "10", "20", "50" };
            settings.SettingsBehavior.EnableCustomizationWindow = true;
            settings.SettingsBehavior.ColumnResizeMode = ColumnResizeMode.NextColumn;
            settings.SettingsBehavior.EnableRowHotTrack = true;
            settings.Width = Unit.Percentage(100);
            settings.Settings.ShowFilterRow = true;
            settings.Settings.ShowFilterRowMenu = true;
            settings.Settings.ShowFilterBar = GridViewStatusBarMode.Auto;
            settings.Settings.ShowHeaderFilterButton = true;
            settings.Settings.ShowFooter = true;
            settings.Settings.ShowGroupPanel = true;
            settings.Settings.ShowTitlePanel = true;
            settings.KeyFieldName = "Oid";
            settings.CallbackRouteValues = new { Controller = baseName, Action = "Grid" };
            settings.SettingsEditing.Mode = GridViewEditingMode.PopupEditForm;
            settings.SettingsPopup.EditForm.Modal = true;
            settings.SettingsBehavior.ConfirmDelete = true;
            settings.KeyboardSupport = false;
            settings.SettingsPager.PageSize = 30;            
            settings.CommandColumn.HeaderStyle.Wrap = DefaultBoolean.True;
            settings.CommandColumn.MinWidth = 30;
            settings.CommandColumn.AllowDragDrop = DefaultBoolean.False;
            settings.ClientSideEvents.SelectionChanged = "SelectionChanged";
            settings.ClientSideEvents.Init = "CustomizationWindow";
            
            settings.Settings.GridLines = GridLines.Vertical;
            settings.Styles.Header.HorizontalAlign = HorizontalAlign.Center;
            settings.Styles.Cell.HorizontalAlign = HorizontalAlign.Left;
            settings.SettingsCookies.Enabled = false;
            settings.SettingsCookies.StoreFiltering = false;
            settings.SettingsPager.Position = PagerPosition.TopAndBottom;

            settings.HtmlRowPrepared = (sender,  e) =>
            {
                if (e.RowType == GridViewRowType.Data)
                    e.Row.Attributes.Remove("oncontextmenu");
            };

            switch(columnType)
            {
                case GridViewCommandColumnType.CHECKBOX:
                    settings.CommandColumn.Visible = true;
                    settings.CommandColumn.ShowNewButton = false;                    
                    settings.CommandColumn.ShowUpdateButton = false;
                    settings.CommandColumn.ShowDeleteButton = false;
                    settings.CommandColumn.ShowEditButton = false;
                    settings.CommandColumn.Width = Unit.Pixel(30);
                    settings.CommandColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center;
                    settings.CommandColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    settings.SettingsBehavior.AllowSelectByRowClick = false;
                    settings.CommandColumn.ShowSelectCheckbox = true;
                    settings.CommandColumn.SelectAllCheckboxMode = GridViewSelectAllCheckBoxMode.AllPages;
                break;
                case GridViewCommandColumnType.ICONS:
                    var Url = new UrlHelper(HttpContext.Current.Request.RequestContext);
                    MVCxGridViewColumn column = new MVCxGridViewColumn();
                    column.Name = "CustomCommandColumn";
                    column.Caption = " ";
                    column.Settings.AllowDragDrop = DefaultBoolean.False;
                    column.Settings.AllowSort = DefaultBoolean.False;
                    column.Width = 70;
                    column.EditFormSettings.Visible = DefaultBoolean.False;
                    column.VisibleIndex = 0;
                    settings.CommandColumn.Visible = true;
                    settings.CommandColumn.ShowDeleteButton = true;
                    settings.CommandColumn.ShowEditButton = true;
                    settings.CommandColumn.ShowNewButtonInHeader = true;
                    settings.CommandColumn.ButtonType = GridCommandButtonRenderMode.Image;
                    settings.SettingsCommandButton.DeleteButton.Image.Url = Url.Content("~/Content/img/del.png");
                    settings.SettingsCommandButton.NewButton.Image.Url = Url.Content("~/Content/img/plus.png");
                    settings.SettingsCommandButton.UpdateButton.Image.Url = Url.Content("~/Content/img/save.png");
                    settings.SettingsCommandButton.CancelButton.Image.Url = Url.Content("~/Content/img/undo.png");
                    settings.SettingsCommandButton.EditButton.Image.Url = Url.Content("~/Content/img/edit.png");
                    settings.SettingsCommandButton.NewButton.Image.ToolTip = Resources.New;
                    settings.SettingsCommandButton.DeleteButton.Image.ToolTip = Resources.Delete;
                    settings.SettingsCommandButton.EditButton.Image.ToolTip = Resources.Edit;
                    settings.CommandColumn.CellStyle.CssClass = "GridImages";
                    settings.CommandColumn.MinWidth = 67;
                    settings.CommandColumn.Width = Unit.Percentage(5);
                    settings.CommandColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left;
                    settings.CommandColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
                    
                    break;
                default:
                    settings.CommandColumn.Visible = false;
                    break;

            }

            settings.CommandColumn.CellStyle.CssClass = "firstCollumn";
            settings.CommandColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center;
            settings.CommandColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center;
            settings.SettingsPopup.EditForm.VerticalAlign = PopupVerticalAlign.WindowCenter;
            settings.SettingsPopup.EditForm.HorizontalAlign = PopupHorizontalAlign.Center;
            settings.CommandColumn.FixedStyle = GridViewColumnFixedStyle.None;
            if (generateColumns)
            {
                IEnumerable<PropertyInfo> properties = objectType.GetProperties().Where(g => g.DeclaringType.IsSubclassOf(typeof(BasicObj)) || g.DeclaringType == objectType);
                if (excludeFields != null)
                {
                    properties = properties.Where(g => (excludeFields.Contains(g.Name) == false));
                }
                else if (includedFields != null)
                {
                    properties = properties.Where(g => (includedFields.Contains(g.Name)));
                }
                                                                                  
                foreach (PropertyInfo propInfo in properties)
                {
                    IEnumerable<GridAttribute> att = propInfo.GetCustomAttributes(typeof(GridAttribute), true).Cast<GridAttribute>();
                    if (( typeof(IEnumerable).IsAssignableFrom(propInfo.PropertyType) && !typeof(string).IsAssignableFrom(propInfo.PropertyType)) || (att.Count() != 0 && att.FirstOrDefault().HideFromGrid == true))
                    {
                        continue;
                    }
                    string caption = propInfo.Name;
                    try
                    {
                        caption = Resources.ResourceManager.GetString(propInfo.Name, Resources.Culture);
                    }
                    catch (Exception ex)
                    {
                        string errorMessage = ex.GetFullMessage();
                        caption = propInfo.Name;
                    }
                    if (caption == null)
                    {
                        caption = propInfo.Name;
                    }
                    MVCxGridViewColumn col = new MVCxGridViewColumn();
                    col.FieldName = propInfo.Name;
                    col.Caption = caption;
                    col.Settings.AutoFilterCondition = AutoFilterCondition.Contains;
                    col.HeaderStyle.Wrap = DefaultBoolean.True;
                    if (propInfo.PropertyType == typeof(bool) || propInfo.PropertyType == typeof(bool?))
                    {
                        col.ColumnType = MVCxGridViewColumnType.CheckBox;
                        (col.PropertiesEdit as CheckBoxProperties).ValueChecked = true;
                        (col.PropertiesEdit as CheckBoxProperties).ValueGrayed = false;
                        (col.PropertiesEdit as CheckBoxProperties).ValueUnchecked = false;
                        (col.PropertiesEdit as CheckBoxProperties).ValueType = propInfo.PropertyType;
                        (col.PropertiesEdit as CheckBoxProperties).AllowGrayed = false;
                        (col.PropertiesEdit as CheckBoxProperties).AllowGrayedByClick = false;
                    }
                    else if (propInfo.PropertyType == typeof(DateTime) || propInfo.PropertyType == typeof(DateTime?))
                    {
                        col.ColumnType = MVCxGridViewColumnType.DateEdit;
                        col.PropertiesEdit.DisplayFormatString = "dd-MMM-yyyy HH:mm:ss";
                    }
                    else if ((propInfo.PropertyType.IsSubclassOf(typeof(BasicObj))))
                    {
                        col.ColumnType = MVCxGridViewColumnType.ComboBox;
                        XPCollection datasource = new XPCollection(Common.XpoHelper.GetNewUnitOfWork(), propInfo.PropertyType);
                        var comboBoxProperties = col.PropertiesEdit as ComboBoxProperties;

                        comboBoxProperties.DataSource = datasource;
                        if ( propInfo.PropertyType.GetProperty("Description") != null )
                        {
                            comboBoxProperties.TextField = "Description";
                        }
                        else if (propInfo.PropertyType.GetProperty("Name") != null)
                        {
                            comboBoxProperties.TextField = "Name";
                        }
                        else
                        {
                            throw new Exception(String.Format("{0} does not have a property named Description or Name", propInfo.Name));
                        }

                        comboBoxProperties.ValueField = "Oid";
                        col.HeaderStyle.Wrap = DefaultBoolean.True;
                        comboBoxProperties.ValueType = typeof(Guid);

                        PropertyInfo desc = propInfo.PropertyType.GetProperties().
                           FirstOrDefault(x => x.GetCustomAttributes(typeof(DescriptionFieldAttribute), false).Count() > 0);

                        if (desc == null)
                        {
                            desc = propInfo.PropertyType.GetProperties().
                                FirstOrDefault(x => x.GetCustomAttributes(typeof(DescriptionFieldAttribute), true).Count() > 0);
                        }

                        if (desc != null)
                        {
                            col.FieldName = propInfo.Name + "." + desc.Name;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else if ((propInfo.PropertyType.IsSubclassOf(typeof(XPBaseObject))))
                    {
                        continue;
                    }
                    else if (propInfo.PropertyType.IsEnum)
                    {
                        string pName = propInfo.Name;
                        col.SetDataItemTemplateContent(c =>
                        {
                            var vobj = DataBinder.Eval(c.DataItem, pName);
                            if (vobj is Enum)
                            {
                                Enum obj = (Enum)vobj;
                                string text = obj.ToLocalizedString();
                                viewContext.Writer.Write(text);
                            }
                            else if (vobj == null)
                            {
                                viewContext.Writer.Write("");
                            }
                        });
                    }
                    settings.Columns.Add(col);
                }
            }

            settings.Settings.HorizontalScrollBarMode = ScrollBarMode.Hidden;
            return settings;
        }

        public static bool IsAllSelectedGridView(ASPxGridView gridView)
        {
            if (gridView.VisibleRowCount == 0)
            {
                return false;
            }
            
            for (var i = 0; i < gridView.VisibleRowCount; i++)
            {
                if (!gridView.Selection.IsRowSelected(i))
                    return false;
            }
            return true;
        }

    }
}