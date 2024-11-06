using ITS.Retail.ResourcesLib;
using System;
using System.Linq;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.ComponentModel.DataAnnotations;

namespace ITS.Retail.Platform.Enumerations
{
    [Editor(typeof(EnumGridComboBox), typeof(UITypeEditor))]
    [EnumList(typeof(eActions))]
    public enum eActions
    {
        [Display(Name = "SHUTDOWN", ResourceType = typeof(Resources))]
        SHUTDOWN = -10,  //external

        [Display(Name = "NONEACTION", ResourceType = typeof(Resources))]
        NONE = 0,      //external

        [Display(Name = "CHECKPRICE", ResourceType = typeof(Resources))]
        CHECKPRICE = 1,  //external ελεγχος τιμης
                         //KEYMAPPINGS=2, //external obsolete
                         //CONFIG=3,  //external  obsolete

        [Display(Name = "FIND_ITEM", ResourceType = typeof(Resources))]
        FIND_ITEM = 4, //external ευρεση ειδους

        [Display(Name = "DELETE_ITEM", ResourceType = typeof(Resources))]
        DELETE_ITEM = 5, //external διαγραφη γραμμης

        [Display(Name = "MOVE_UP", ResourceType = typeof(Resources))]
        MOVE_UP = 6, //external πανω

        [Display(Name = "MOVE_DOWN", ResourceType = typeof(Resources))]
        MOVE_DOWN = 7,  //external κατω

        [Display(Name = "MULTIPLY_QTY", ResourceType = typeof(Resources))]
        MULTIPLY_QTY = 8,  //external πολλαπλασιαστης ποσοτητας

        [Display(Name = "DOCUMENT_TOTAL", ResourceType = typeof(Resources))]
        DOCUMENT_TOTAL = 9, //external συνολο
        //ADD_PAYMENT = 10, //MOVED TO INTERNAL: 100021
        [Display(Name = "DELETE_PAYMENT", ResourceType = typeof(Resources))]
        DELETE_PAYMENT = 11, //external //διαγραφη πληρωμης

        [Display(Name = "ISSUE_Z", ResourceType = typeof(Resources))]
        ISSUE_Z = 12, //external εκδοση ζ

        [Display(Name = "CANCEL_DOCUMENT", ResourceType = typeof(Resources))]
        CANCEL_DOCUMENT = 13, //external ακυρωση αποδειξης/παραστατικου

        [Display(Name = "START_FISCAL_DAY", ResourceType = typeof(Resources))]
        START_FISCAL_DAY = 14,  //external εναρξη ημερας

        [Display(Name = "START_SHIFT", ResourceType = typeof(Resources))]
        START_SHIFT = 15, //external εναρξη βαρδιας

        [Display(Name = "ISSUE_X", ResourceType = typeof(Resources))]
        ISSUE_X = 16,  //external εκδοση χ

        [Display(Name = "CANCEL_POS", ResourceType = typeof(Resources))]
        CANCEL = 17, //external ακυρωση

        [Display(Name = "BACKSPACE", ResourceType = typeof(Resources))]
        BACKSPACE = 18, //external 

        [Display(Name = "ADD_LINE_DISCOUNT", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("DiscountTypeCode", typeof(string))]
        ADD_LINE_DISCOUNT = 19, //external εκπωση γραμμης

        [Display(Name = "ADD_DOCUMENT_DISCOUNT", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("DiscountTypeCode", typeof(string))]
        ADD_DOCUMENT_DISCOUNT = 20, //external εκπτωση συνολου

        [Display(Name = "FIND_SCAN_CODE", ResourceType = typeof(Resources))]
        FIND_SCAN_CODE = 21,  //external 
                              //GET_ITEM_PRICE = 22, //MOVED TO INTERNAL : 100022

        [Display(Name = "USE_PROFORMA", ResourceType = typeof(Resources))]
        USE_PROFORMA = 30, //external προτιμολογιο

        [Display(Name = "PREVIEW_X_REPORT", ResourceType = typeof(Resources))]
        PREVIEW_X_REPORT = 31, //external προεπισκοπηση αναφορας χ

        [Display(Name = "OPEN_DRAWER", ResourceType = typeof(Resources))]
        OPEN_DRAWER = 32, //external ανοιγμα συρταριου

        [Display(Name = "DISPLAY_WITHDRAWAL", ResourceType = typeof(Resources))]
        DISPLAY_WITHDRAWAL = 33, //external αναληψη

        [Display(Name = "DISPLAY_DEPOSIT", ResourceType = typeof(Resources))]
        DISPLAY_DEPOSIT = 34, //external καταθεση

        [Display(Name = "RETURN_ITEM", ResourceType = typeof(Resources))]
        RETURN_ITEM = 35, //external επιστροφη

        [Display(Name = "RESTART", ResourceType = typeof(Resources))]
        RESTART = 36,  //external επανεκκινηση υπολογιστη

        [Display(Name = "CHANGE_ITEM_PRICE", ResourceType = typeof(Resources))]
        CHANGE_ITEM_PRICE = 37, //external αλλαγη τιμης

        [Display(Name = "APPLICATION_EXIT", ResourceType = typeof(Resources))]
        APPLICATION_EXIT = 38, //external τερματισμος εφαρμογης

        [Display(Name = "ADD_PAYMENT", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("PaymentMethodCode", typeof(string))]
        ADD_PAYMENT = 39,  //external πληρωμη

        [Display(Name = "ADD_ITEM", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("ItemCode", typeof(string))]
        ADD_ITEM = 40,//external ειδος

        [Display(Name = "ADD_PAYMENT_FROM_FORM", ResourceType = typeof(Resources))]
        ADD_PAYMENT_FROM_FORM = 41,//external πληρωμη με επιλογη

        [Display(Name = "ADD_TOTAL_PAYMENT", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("PaymentMethodCode", typeof(string))]
        ADD_TOTAL_PAYMENT = 42,//external συνολικη πληρωμη

        [Display(Name = "ADD_TOTAL_PAYMENT_FROM_FORM", ResourceType = typeof(Resources))]
        ADD_TOTAL_PAYMENT_FROM_FORM = 43,//external συνολικη πληρωμη με επιλογη

        [Display(Name = "DISPLAY_RETURN_ITEM_FORM", ResourceType = typeof(Resources))]
        DISPLAY_RETURN_ITEM_FORM = 44,//external επιστροφη με επιλογη

        [Display(Name = "STRESS_TEST", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("NumberOfReceipts", typeof(int))]
        [ActionKeybindParameter("ItemsPerReceipt", typeof(int))]
        [ActionKeybindParameter("RandomCustomer", typeof(bool))]
        [ActionKeybindParameter("RandomPayment", typeof(bool))]
        [ActionKeybindParameter("RandomCancelLines", typeof(bool))]
        [ActionKeybindParameter("RandomCancelDocument", typeof(bool))]
        [ActionKeybindParameter("RandomProforma", typeof(bool))]
        STRESS_TEST = 45,//external 

        [Display(Name = "FISCAL_PRINTER_REPRINT_Z_REPORTS", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("UseDateFilter", typeof(bool))]
        [ActionKeybindParameter("Mode", typeof(eReprintZReportsMode))]
        FISCAL_PRINTER_REPRINT_Z_REPORTS = 46,//external Φορ. Εκτ επανεκτυπωση Ζ

        [Display(Name = "REPRINT_RECEIPTS", ResourceType = typeof(Resources))]
        REPRINT_RECEIPTS = 47, //external επανεκτυπωση αποδειξεων

        [Display(Name = "FISCAL_PRINTER_PRINT_FISCAL_MEMORY_BLOCKS", ResourceType = typeof(Resources))]
        FISCAL_PRINTER_PRINT_FISCAL_MEMORY_BLOCKS = 48, //external 

        [Display(Name = "SERVICE_FORCED_CANCEL_DOCUMENT", ResourceType = typeof(Resources))]
        SERVICE_FORCED_CANCEL_DOCUMENT = 49, //external 

        [Display(Name = "SERVICE_FORCED_START_DAY_FISCAL_PRINTER", ResourceType = typeof(Resources))]
        SERVICE_FORCED_START_DAY_FISCAL_PRINTER = 50, //external 

        [Display(Name = "ADD_ITEM_WEIGHTED", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("ItemCode", typeof(string))]
        ADD_ITEM_WEIGHTED = 51, //external ζυγιζομενο ειδος με ληψη βαρους

        [Display(Name = "ADD_LINE_DISCOUNT_FROM_FORM", ResourceType = typeof(Resources))]
        ADD_LINE_DISCOUNT_FROM_FORM = 52, //external προσθηκη εκπτωσης γραμμης με επιλογη

        [Display(Name = "ADD_DOCUMENT_DISCOUNT_FROM_FORM", ResourceType = typeof(Resources))]
        ADD_DOCUMENT_DISCOUNT_FROM_FORM = 53, //external προσθηκη εκπτωσης συνολου με επιλογη

        [Display(Name = "ADD_CUSTOMER", ResourceType = typeof(Resources))]
        ADD_CUSTOMER = 54, //external πελατης

        [Display(Name = "CANCEL_DISCOUNT", ResourceType = typeof(Resources))]
        CANCEL_DISCOUNT = 55, //external ακυρωση εκπτωσης

        [Display(Name = "SET_FISCAL_ON_ERROR", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("SetFiscalOnError", typeof(bool))]
        SET_FISCAL_ON_ERROR = 56, //θεση φορ μηχ σε βλαβη

        [Display(Name = "SET_DOCUMENT_ON_HOLD", ResourceType = typeof(Resources))]
        SET_DOCUMENT_ON_HOLD = 57, // σε αναμονη

        [Display(Name = "GET_DOCUMENT_FROM_HOLD", ResourceType = typeof(Resources))]
        GET_DOCUMENT_FROM_HOLD = 58, // απο αναμονη

        [Display(Name = "SET_OR_GET_DOCUMENT_ON_HOLD", ResourceType = typeof(Resources))]
        SET_OR_GET_DOCUMENT_ON_HOLD = 59, // σε 'η  απο αναμονη

        [Display(Name = "GENERIC_CANCEL", ResourceType = typeof(Resources))]
        GENERIC_CANCEL = 60,

        [Display(Name = "ACTION_PAUSE", ResourceType = typeof(Resources))]
        PAUSE = 61,

        [Display(Name = "SERVICE_FORCED_START_DAY_FISCAL_PRINTER", ResourceType = typeof(Resources))]
        SERVICE_START_LEGAL_RECEIPT_FISCAL_PRINTER = 62, //external 

        [Display(Name = "SERVICE_FORCED_START_DAY_FISCAL_PRINTER", ResourceType = typeof(Resources))]
        SERVICE_RESTORE_FISCAL_PRINTER = 63,

        [Display(Name = "USE_DEFAULT_DOCUMENT_TYPE", ResourceType = typeof(Resources))]
        USE_DEFAULT_DOCUMENT_TYPE = 64,

        [Display(Name = "DISPLAY_VAT_FACTORS", ResourceType = typeof(Resources))]
        DISPLAY_VAT_FACTORS = 65,

        [Display(Name = "SET_STANDALONE_FISCAL_ON_ERROR", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("SetStandaloneFiscalOnError", typeof(bool))]
        SET_STANDALONE_FISCAL_ON_ERROR = 66,

        [Display(Name = "FORCE_DELETE_PAYMENT", ResourceType = typeof(Resources))]
        FORCE_DELETE_PAYMENT = 67,

        [Display(Name = "EDPS_CHECK_COMMUNICATION", ResourceType = typeof(Resources))]
        EDPS_CHECK_COMMUNICATION = 68,

        [Display(Name = "ADD_COUPON", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("CouponCode", typeof(string))]
        ADD_COUPON = 69,

        [Display(Name = "ISSUE_Z_EAFDSS", ResourceType = typeof(Resources))]
        ISSUE_Z_EAFDSS = 70,

        [Display(Name = "DATABASE_MAINTENANCE", ResourceType = typeof(Resources))]
        DATABASE_MAINTENANCE = 71,

        [Display(Name = "DATABASE_MAINTENANCE_LIGHT", ResourceType = typeof(Resources))]
        DATABASE_MAINTENANCE_LIGHT = 72,


        [Display(Name = "USE_DOCUMENT_TYPE", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("DocumentTypeCode", typeof(string))]
        USE_DOCUMENT_TYPE = 73,

        [Display(Name = "USE_SPECIAL_PROFORMA", ResourceType = typeof(Resources))]
        USE_SPECIAL_PROFORMA = 74, //external ειδικό προτιμολόγιο

        PRINTER_FEED = 75,

        [Display(Name = "FIND_ITEM_BY_DESCRIPTION", ResourceType = typeof(Resources))]
        FIND_ITEM_BY_DESCRIPTION = 76,

        [Display(Name = "RESET_EAFDSS_DEVICES_ORDER", ResourceType = typeof(Resources))]
        RESET_EAFDSS_DEVICES_ORDER = 80,

        [Display(Name = "USE_OPOS_REPORT", ResourceType = typeof(Resources))]
        [ActionKeybindParameter("PosReportCode", typeof(string))]
        USE_OPOS_REPORT = 81,

        [Display(Name = "CARDLINK_BATCH_CLOSE", ResourceType = typeof(Resources))]
        CARDLINK_BATCH_CLOSE = 82,

        [Display(Name = "CARDLINK_CHECK_COMMUNICATION", ResourceType = typeof(Resources))]
        CARDLINK_CHECK_COMMUNICATION = 83,

        [Display(Name = "CALL_OTHER_ACTION", ResourceType = typeof(Resources))]
        CALL_OTHER_ACTION = 1000, // special externals κληση αλλης εντολης

        [Display(Name = "CASH_COUNT_MOVE_UP", ResourceType = typeof(Resources))]
        CASH_COUNT_MOVE_UP = 2000, // κληση που αφορά μόνο ταμειακό φύλλο

        [Display(Name = "CASH_COUNT_MOVE_DOWN", ResourceType = typeof(Resources))]
        CASH_COUNT_MOVE_DOWN = 2001, // κληση που αφορά μόνο ταμειακό φύλλο

        [Display(Name = "CASH_COUNT_NEXT", ResourceType = typeof(Resources))]
        CASH_COUNT_NEXT = 2002, // κληση που αφορά μόνο ταμειακό φύλλο

        [Display(Name = "CASH_COUNT_PREVIOUS", ResourceType = typeof(Resources))]
        CASH_COUNT_PREVIOUS = 2003, // κληση που αφορά μόνο ταμειακό φύλλο

        [Display(Name = "CASH_COUNT_REMOVE_LINE", ResourceType = typeof(Resources))]
        CASH_COUNT_REMOVE_LINE = 2004, // κληση που αφορά μόνο ταμειακό φύλλο

        [Display(Name = "CASH_COUNT_MULTIPLY", ResourceType = typeof(Resources))]
        CASH_COUNT_MULTIPLY = 2005, // κληση που αφορά μόνο ταμειακό φύλλο

        [Display(Name = "CASH_COUNT_ENTER", ResourceType = typeof(Resources))]
        CASH_COUNT_ENTER = 2006, // κληση που αφορά μόνο ταμειακό φύλλο

        [Display(Name = "CASH_COUNT_SUBMIT", ResourceType = typeof(Resources))]
        CASH_COUNT_SUBMIT = 2007, // κληση που αφορά μόνο ταμειακό φύλλο

        [Display(Name = "CASH_COUNT_CANCEL", ResourceType = typeof(Resources))]
        CASH_COUNT_CANCEL = 2008, // κληση που αφορά μόνο ταμειακό φύλλο

        //Internal Actions > 100000
        KEYBOARD = 100001,  //internal
        //ADD_ITEM = 100002, //internal obsolete
        OPEN_SCANNERS = 100003,  //internal
        START_NEW_DOCUMENT = 100004,  //internal
        CLOSE_DOCUMENT = 100005, //internal
        PUBLISH_DOCUMENT_INFO = 100006,  //internal
        PRINT_RECEIPT = 100007,  //internal
        CUSTOMER_POLE_DISPLAY_MESSAGE = 100008, //internal
        PUBLISH_DOCUMENT_LINE_INFO = 100009,  //internal
        ADD_CUSTOMER_INTERNAL = 100010, //internal
        LOAD_EXISTING_DOCUMENT = 100011,  //internal
        PUBLISH_DOCUMENT_PAYMENT_INFO = 100012,  //internal
        SHOW_ERROR = 100013,  //internal
        PUBLISH_LINE_QUANTITY_INFO = 100014,  //internal
        UPDATE_COMMUNICATION_STATUS = 100015,  //internal
        PUBLISH_KEY_PRESS = 100016,  //internal
        POST_STATUS = 100017,  //internal
        DISPLAY_TOUCH_PAD = 100018,  //internal
        HIDE_TOUCH_PAD = 100019,  //internal
        ADD_ITEM_INTERNAL = 100020,  //internal
        //DEPRECATED_ADD_PAYMENT = 100021,  //MOVED TO EXTERNAL : 39
        //GET_ITEM_PRICE = 100022, //internal
        PUBLISH_MACHINE_STATUS = 100023, //internal
        PUBLISH_DOCUMENT_QUANTITY = 100024, //internal
        FISCAL_PRINTER_PRINT_RECEIPT = 100025,  //internal
        SLIP_PRINT = 100026, // internal
        LOAD_EXISTING_DOCUMENTS_ON_HOLD = 100029,//internal
        CASHIER_POLE_DISPLAY_MESSAGE = 100030, //internal
        EDPS_BATCH_CLOSE = 100130, //internal
        CHECK_STATUS_WITH_FISCAL_PRINTER = 100131,
        CANCEL_NOT_INCLUDED_ITEMS = 100135, //Creates the form where the not included items in new price catalog are listed
        SHOW_BLINKING_ERROR = 100137,//internal
        PRINT_DOCUMENT_TO_WINDOWS_PRINTER = 100181,//internal
        PRINT_REPORT_TO_OPOS_PRINTER = 100182//internal
    }



    public abstract class GridComboBox : UITypeEditor
    {
        private const string StrAddNew = "<Add New...>";


        private IList _dataList;
        private readonly ListBox _listBox;
        private Boolean _escKeyPressed;
        private ListAttribute _listAttribute;
        private IWindowsFormsEditorService _editorService;


        /// <summary>
        /// Constructor
        /// </summary>
        public GridComboBox()
        {
            _listBox = new ListBox();

            // Properties
            _listBox.BorderStyle = BorderStyle.None;

            // Events
            _listBox.Click += myListBox_Click;
            _listBox.PreviewKeyDown += myListBox_PreviewKeyDown;
        }


        /// <summary>
        /// Get/Set for ListBox
        /// </summary>
        protected ListBox ListBox
        {
            get { return (_listBox); }
        }

        /// <summary>
        /// Get/Set for DataList
        /// </summary>
        protected IList DataList
        {
            get { return (_dataList); }
            set { _dataList = value; }
        }

        /// <summary>
        /// Get/Set for ListAttribute
        /// </summary>
        protected ListAttribute ListAttribute
        {
            get { return (_listAttribute); }
            set { _listAttribute = value; }
        }


        /// <summary>
        /// Close DropDown window to finish editing
        /// </summary>
        public void CloseDropDownWindow()
        {
            if (_editorService != null)
                _editorService.CloseDropDown();
        }


        /// <summary>
        /// Populate the ListBox with data items
        /// </summary>
        /// <param name="context"></param>
        /// <param name="currentValue"></param>
        private void PopulateListBox(ITypeDescriptorContext context, Object currentValue)
        {
            // Clear List
            _listBox.Items.Clear();

            // Retrieve the reference to the items to be displayed in the list
            if (_dataList == null)
                RetrieveDataList(context);

            if (_dataList != null)
            {
                if ((_listAttribute is IAddNew) && (((IAddNew)_listAttribute).AddNew))
                    _listBox.Items.Add(StrAddNew);

                // Add Items to the ListBox
                foreach (object obj in _dataList)
                {
                    _listBox.Items.Add(obj);
                }

                // Select current item 
                if (currentValue != null)
                    _listBox.SelectedItem = currentValue;
            }

            // Set the height based on the Items in the ListBox
            _listBox.Height = _listBox.PreferredHeight;
        }


        /// <summary>
        /// Get the object selected in the ComboBox
        /// </summary>
        /// <returns>Selected Object</returns>
        protected abstract object GetDataObjectSelected(ITypeDescriptorContext context);

        /// <summary>
        /// Find the list of data items to populate the ListBox
        /// </summary>
        /// <param name="context"></param>
        protected abstract void RetrieveDataList(ITypeDescriptorContext context);


        /// <summary>
        /// Preview Key Pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myListBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                _escKeyPressed = true;
        }

        /// <summary>
        /// ListBox Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void myListBox_Click(object sender, EventArgs e)
        {
            //when user clicks on an item, the edit process is done.
            this.CloseDropDownWindow();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if ((context != null) && (provider != null))
            {
                //Uses the IWindowsFormsEditorService to display a 
                // drop-down UI in the Properties window:
                _editorService = provider.GetService(
                                    typeof(IWindowsFormsEditorService))
                                 as IWindowsFormsEditorService;

                if (_editorService != null)
                {
                    // Add Values to the ListBox
                    PopulateListBox(context, value);

                    // Set to false before showing the control
                    _escKeyPressed = false;

                    // Attach the ListBox to the DropDown Control
                    _editorService.DropDownControl(_listBox);

                    // User pressed the ESC key --> Return the Old Value
                    if (!_escKeyPressed)
                    {
                        // Get the Selected Object
                        object obj = GetDataObjectSelected(context);

                        // If an Object is Selected --> Return it
                        if (obj != null)
                            return (obj);
                    }
                }
            }

            return (value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return (UITypeEditorEditStyle.DropDown);
        }

    }


    public class EnumGridComboBox : GridComboBox
    {

        /// <summary>
        /// Get the object selected in the ComboBox
        /// </summary>
        /// <returns>Selected Object</returns>
        protected override object GetDataObjectSelected(ITypeDescriptorContext context)
        {
            return (base.ListBox.SelectedItem);
        }

        /// <summary>
        /// Find the list of data items to populate the ComboBox
        /// </summary>
        /// <param name="context"></param>
        protected override void RetrieveDataList(ITypeDescriptorContext context)
        {
            // Find the Attribute that has the path to the Enumerations list
            foreach (Attribute attribute in context.PropertyDescriptor.Attributes)
            {
                if (attribute is EnumListAttribute)
                {
                    base.ListAttribute = attribute as EnumListAttribute;
                    break;
                }
            }

            // If we found the Attribute, find the Data List
            if (base.ListAttribute != null)
            {
                // Save the DataList
                Type enumType = ((EnumListAttribute)base.ListAttribute).EnumType;
                switch (enumType.Name)
                {

                    case "eActions":
                        ////Get all the actions that are external and do not require keybound parameters.
                        base.DataList = Enum.GetValues(enumType).Cast<eActions>().Where(ac => (int)ac < 10000 && ac.GetActionKeybindParameters().Count == 0).OrderBy(v => v.ToString()).ToList();
                        break;
                    default:
                        base.DataList = Enum.GetValues(enumType);
                        break;
                }



            }
        }
    }

    public class EnumListAttribute : ListAttribute
    {

        private readonly Type _enumType;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumType">List of items for display in the GridComboBox</param>
        public EnumListAttribute(Type enumType)
        {
            if (enumType.BaseType == typeof(Enum))
                this._enumType = enumType;
            else
                throw new ArgumentException("Argument must be of type Enum");
        }

        /// <summary>
        /// Get/Set for EnumType
        /// </summary>
        public Type EnumType
        {
            get { return (_enumType); }
        }

    }

    public abstract class ListAttribute : Attribute
    {
    }

    public interface IAddNew
    {
        bool AddNew { get; }
    }
}
