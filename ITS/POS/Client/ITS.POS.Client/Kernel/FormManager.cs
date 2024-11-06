using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using ITS.POS.Client.Forms;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.UserControls;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.POS.Client.Exceptions;
using ITS.POS.Resources;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Controls;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using ITS.POS.Hardware.Common;
using ITS.POS.Hardware;
using System.ComponentModel;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Provides functions for manipulating forms.
    /// </summary>
    public class FormManager : IFormManager
    {
        private IPosKernel Kernel { get; set; }

        private ISessionManager SessionManager { get; set; }
        private IConfigurationManager ConfigurationManager { get; set; }
        private IAppContext AppContext { get; set; }

        public FormManager(IPosKernel kernel, ISessionManager sessionManager, IConfigurationManager configurationManager, IAppContext appContext)
        {
            this.Kernel = kernel;
            this.SessionManager = sessionManager;
            this.ConfigurationManager = configurationManager;
            this.AppContext = appContext;
        }

        /// <summary>
        /// Handles the displaying of a new form and manages the form's observer subscriptions.
        /// </summary>
        /// <typeparam name="TFormToShow"></typeparam>
        /// <param name="currentForm"></param>
        /// <param name="closeCurrentForm"></param>
        /// <param name="displayAsDialog"></param>
        /// <param name="disposeAfterShowDialog"></param>
        /// <returns></returns>
        public DialogResult ShowForm<TFormToShow>(Form currentForm, bool closeCurrentForm, bool displayAsDialog = false, bool disposeAfterShowDialog = false) where TFormToShow : Form
        {
            DialogResult result = DialogResult.OK;

            TFormToShow form;
            if (AppContext.MainForm != null && typeof(TFormToShow).IsAssignableFrom(typeof(frmMainBase)))
            {
                form = AppContext.MainForm as TFormToShow;
            }
            else if (AppContext.SplashForm != null && AppContext.SplashForm.IsDisposed == false && typeof(TFormToShow) == typeof(frmSplashScreen))
            {
                form = AppContext.SplashForm as TFormToShow;
            }
            else if (typeof(IPOSForm).IsAssignableFrom(typeof(TFormToShow)))
            {
                form = Activator.CreateInstance(typeof(TFormToShow), Kernel) as TFormToShow;
                if (typeof(TFormToShow) == typeof(frmSplashScreen))
                {
                    AppContext.SplashForm = form as frmSplashScreen;
                }
            }
            else
            {
                throw new POSException("Invalid Form type");
            }


            if (currentForm is IObserverContainer && !currentForm.GetType().IsSubclassOf(typeof(frmMainBase)))
            {
                (currentForm as IObserverContainer).DropSubscriptions();
            }

            if (form is IObserverContainer)
            {
                (form as IObserverContainer).IntitializeSubscriptions();
            }
            AppContext.MainForm.Invoke((MethodInvoker)delegate
            {
                if (displayAsDialog)
                {

                    result = form.ShowDialog();
                    if (disposeAfterShowDialog)
                    {
                        form.Dispose();
                    }
                }
                else
                {
                    form.Show();
                    Application.DoEvents();
                    form.Focus();
                    form.BringToFront();
                    Program.SetActiveWindow(form);
                }

                if (closeCurrentForm)
                {
                    if (currentForm.GetType().IsSubclassOf(typeof(frmMainBase)))
                    {
                        currentForm.Hide();
                    }
                    else
                    {
                        currentForm.Close();
                        if (typeof(TFormToShow) == typeof(frmSplashScreen))
                        {
                            AppContext.SplashForm = null;
                        }
                    }
                }
            });
            return result;
        }

        /// <summary>
        /// Gets all child controls of a control.
        /// </summary>
        /// <param name="control"></param>
        /// <param name="ofType">Optional parameter to get only the child controls of the given type</param>
        /// <returns></returns>
        public IEnumerable<Control> GetAllChildControls(Control control, Type ofType = null)
        {
            var controls = control.Controls.Cast<Control>();


            if (ofType == null)
            {
                return controls.SelectMany(ctrl => GetAllChildControls(ctrl, ofType))
                      .Concat(controls);
            }
            else
            {
                return controls.SelectMany(ctrl => GetAllChildControls(ctrl, ofType))
                                      .Concat(controls)
                                      .Where(c => ofType.IsAssignableFrom(c.GetType()));
            }
        }

        private int GetTop(int inputCounter, int gridViewCounter)
        {
            return (inputCounter * 25) + gridViewCounter * ucCustomEnumerationGrid.CUSTOM_ENUMERATION_HEIGHT;
        }

        /// <summary>
        /// Creates a popup Form with the provided inputVariables.
        /// </summary>
        /// <param name="customFields"></param>
        /// <returns></returns>
        public Form CreateCustomFieldsInputForm(IEnumerable<CustomField> customFields)
        {
            int formWidth = 800;
            frmCustomFieldsInput form = new frmCustomFieldsInput(Kernel);
            form.AutoValidate = AutoValidate.EnableAllowFocusChange;
            form.Width = formWidth;

            List<DevExpress.XtraEditors.BaseControl> controlList = new List<DevExpress.XtraEditors.BaseControl>();
            int selectedControlIndex = 0;
            SimpleButton okBtn = new SimpleButton();
            SimpleButton cancelBtn = new SimpleButton();

            Random randomizer = new Random();
            int inputCounter = 0;
            int gridViewCounter = 0;

            foreach (CustomField field in customFields)
            {
                if (!String.IsNullOrEmpty(field.Label))
                {
                    inputCounter++;

                    Label label = new Label();
                    label.Name = "Label" + randomizer.Next(1000);
                    label.Text = field.Label;
                    label.Left = -150;
                    label.Width = 280;
                    label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                    label.AutoEllipsis = true;
                    label.Top = GetTop(inputCounter, gridViewCounter) + 30;//(inputCounter * 25);

                    form.Controls.Add(label);
                    Type fieldType = typeof(CustomFieldStorage).GetProperty(field.FieldName).PropertyType;
                    DevExpress.XtraEditors.BaseControl inputControl;
                    ucCustomEnumerationGrid customEnumerationGrid = null;
                    if (fieldType == typeof(DateTime))
                    {
                        inputControl = new DateEdit();
                    }
                    else if (fieldType == typeof(Guid)) //Custom enum
                    {
                        inputControl = null;
                        gridViewCounter++;
                        customEnumerationGrid = new ucCustomEnumerationGrid();
                    }
                    else
                    {
                        inputControl = new ucTouchFriendlyInput();
                        (inputControl as ucTouchFriendlyInput).AutoHideTouchPad = false;
                    }

                    if (inputControl != null)
                    {
                        inputControl.Tag = label;
                        inputControl.KeyDown += delegate (object sender, KeyEventArgs e)
                        {
                            if (e.KeyCode == Keys.Enter && selectedControlIndex != (controlList.Count - 1))
                            {
                                selectedControlIndex++;
                                if (controlList.ElementAtOrDefault(selectedControlIndex) != null)
                                {
                                    if (controlList.ElementAtOrDefault(selectedControlIndex) == okBtn)
                                    {
                                        okBtn.PerformClick();
                                    }
                                    else
                                    {
                                        controlList.ElementAtOrDefault(selectedControlIndex).Focus();
                                    }
                                }
                            }
                        };

                        inputControl.GotFocus += delegate (object sender, EventArgs e)
                        {
                            Label lblOfInput = (sender as DevExpress.XtraEditors.BaseControl).Tag as Label;
                            selectedControlIndex = controlList.IndexOf(sender as DevExpress.XtraEditors.BaseControl);
                        };
                    }

                    if (fieldType == typeof(double))
                    {
                        (inputControl as ucTouchFriendlyInput).Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                        (inputControl as ucTouchFriendlyInput).Properties.Mask.UseMaskAsDisplayFormat = true;
                        (inputControl as ucTouchFriendlyInput).Properties.Mask.EditMask = "###########0.0#########";
                    }
                    else if (fieldType == typeof(int))
                    {
                        (inputControl as ucTouchFriendlyInput).Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
                        (inputControl as ucTouchFriendlyInput).Properties.Mask.UseMaskAsDisplayFormat = true;
                        (inputControl as ucTouchFriendlyInput).Properties.Mask.EditMask = "d";
                    }
                    else if (fieldType == typeof(Guid)) //Custom enum
                    {
                        Guid customEnumOid = field.CustomEnumeration;
                        XPCollection<CustomEnumerationValue> customEnumerationValues = new XPCollection<CustomEnumerationValue>(SessionManager.GetSession<CustomEnumerationValue>(),
                            new BinaryOperator("CustomEnumerationDefinition", customEnumOid),
                            new SortProperty("Ordering", DevExpress.Xpo.DB.SortingDirection.Ascending));

                        BindingSource bindingSource = new BindingSource();
                        bindingSource.DataSource = customEnumerationValues;
                        customEnumerationGrid.DataSource = bindingSource;
                        CustomEnumerationValue firstEnumerationValue = customEnumerationValues.FirstOrDefault();
                        customEnumerationGrid.SelectedValue = firstEnumerationValue == null ? firstEnumerationValue.Oid : Guid.Empty;
                        customEnumerationGrid.AutoSize = false;
                        customEnumerationGrid.Width = 200 ;
                        customEnumerationGrid.Height = 150;
                    }
                    else if (fieldType == typeof(DateTime))
                    {
                        if (ConfigurationManager.UsesTouchScreen)
                        {
                            (inputControl as DateEdit).Properties.CalendarView = DevExpress.XtraEditors.Repository.CalendarView.TouchUI;
                        }
                        else
                        {
                            (inputControl as DateEdit).Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
                            (inputControl as DateEdit).Properties.Mask.UseMaskAsDisplayFormat = true;
                            (inputControl as DateEdit).Properties.Mask.EditMask = "([012]?[1-9]|[123]0|31)/(0?[1-9]|1[012])/([123][0-9])?[0-9][0-9]";
                        }
                        (inputControl as DateEdit).DateTime = DateTime.Now;

                    }

                    if (inputControl != null)
                    {
                        inputControl.Name = field.Oid.ToString();
                        inputControl.Width = 300;
                        inputControl.Left = label.Right;// formWidth - inputControl.Width - label.Width;
                        inputControl.Top = GetTop(inputCounter, gridViewCounter)+25; //(inputCounter * 25);
                        
                        form.Controls.Add(inputControl);
                        controlList.Add(inputControl);
                    }
                    if (customEnumerationGrid != null)
                    {
                        customEnumerationGrid.Name = field.Oid.ToString();
                        //customEnumerationGrid.Width = 300;
                        customEnumerationGrid.Left = label.Right;// formWidth - customEnumerationGrid.Width - label.Width;
                        int gridCounter = gridViewCounter - 1;
                        if (gridCounter < 0)
                        {
                            gridCounter = 0;
                        }
                        customEnumerationGrid.Top = GetTop(inputCounter, gridCounter); //(inputCounter * 25);
                        form.Controls.Add(customEnumerationGrid);

                        #region Add NavigationButtons
                        int arrowButtonsSize = 40;
                        SimpleButton upButton = new SimpleButton();
                        upButton.Left = customEnumerationGrid.Right;
                        upButton.Top = customEnumerationGrid.Top;
                        upButton.Width = upButton.Height = arrowButtonsSize;
                        upButton.Image = global::ITS.POS.Client.Properties.Resources.arrow_up;
                        upButton.Click += delegate (object sender, EventArgs e)
                        {
                            customEnumerationGrid.MoveToNextRow(GridNavigationDirection.UP);
                        };
                        form.Controls.Add(upButton);

                        SimpleButton downButton = new SimpleButton();
                        downButton.Left = upButton.Left;
                        downButton.Top = customEnumerationGrid.Top + upButton.Height;
                        downButton.Width = downButton.Height = arrowButtonsSize;
                        downButton.Image = global::ITS.POS.Client.Properties.Resources.arrow_down;
                        downButton.Click += delegate (object sender, EventArgs e)
                        {
                            customEnumerationGrid.MoveToNextRow(GridNavigationDirection.DOWN);
                        };
                        form.Controls.Add(downButton);
                        #endregion

                    }
                }
            }



            okBtn.Width = 120;
            okBtn.Name = "OkButton";
            okBtn.Text = "OK";
            okBtn.Top = GetTop(inputCounter, gridViewCounter) - 35; //(inputCounter * 25) + 40;
            okBtn.Height = 70;
            okBtn.Left = formWidth - 240;
            okBtn.Image = ITS.POS.Client.Properties.Resources.availibity_ok_32;
            okBtn.Click += delegate (object sender, EventArgs e)
            {
                if (controlList.Where(x => String.IsNullOrWhiteSpace(x.Text)).Count() > 0)
                {
                    this.ShowMessageBox(POSClientResources.PLEASE_FILL_ALL_REQUIRED_FIELDS);
                    controlList.Where(x => String.IsNullOrWhiteSpace(x.Text)).FirstOrDefault().Focus();
                }
                else
                {
                    okBtn.FindForm().DialogResult = DialogResult.OK;
                    okBtn.FindForm().Hide();
                }
            };

            cancelBtn.Width = 120;
            cancelBtn.Name = "cancelButton";
            cancelBtn.Text = Resources.POSClientResources.CANCEL;
            cancelBtn.Image = ITS.POS.Client.Properties.Resources.Ribbon_Exit_32x32;
            cancelBtn.Top = GetTop(inputCounter, gridViewCounter) + 40; //(inputCounter * 25) + 40;
            cancelBtn.Height = 70;
            cancelBtn.Left = formWidth - 240;
            cancelBtn.Click += delegate (object sender, EventArgs e)
            {
                cancelBtn.FindForm().DialogResult = DialogResult.Cancel;
                cancelBtn.FindForm().Hide();
            };


            form.Controls.Add(cancelBtn);
            form.CancelButton = cancelBtn;
            form.Controls.Add(okBtn);
            controlList.Add(okBtn);

            if (inputCounter == 0)
            {
                return null;
            }
            else
            {
                return form;
            }
        }

        /// <summary>
        /// Creates and returns a message box for customing.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public frmMessageBox CreateMessageBox(string message)
        {
            frmMessageBox msgbx = new frmMessageBox(Kernel);
            msgbx.Message = message;
            return msgbx;
        }

        /// <summary>
        /// Displays a messagebox with only a Cancel button and returns the dialog result.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public DialogResult ShowCancelOnlyMessageBox(string message)
        {
            using (frmMessageBox frm = CreateMessageBox(message))
            {
                frm.btnCancel.Visible = true;
                frm.btnRetry.Visible = false;
                frm.btnOK.Visible = false;
                frm.AcceptButton = null;
                frm.CancelButton = frm.btnCancel;
                return frm.ShowDialog();
            }
        }


        /// <summary>
        /// Displays a messagebox with only a Retry and Ok button when the document failed to print, returns the dialog result.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public DialogResult ShowFailToPrintMessageBox(string message)
        {
            using (frmMessageBox frm = CreateMessageBox(message))
            {
                var OkImage = frm.btnOK.Image;
                var RetryImage = frm.btnRetry.Image;
                frm.Height = 250;
                frm.btnRetry.Visible = true;
                frm.btnRetry.Text = POSClientResources.CONTINUE;
                frm.btnRetry.Width = 130;
                frm.btnRetry.Location = new Point(300, 0);
                frm.btnOK.Visible = true;
                frm.btnOK.Text = POSClientResources.REPRINT_RECEIPT;
                frm.btnOK.Left = 1;
                frm.btnOK.Width = 130;
                frm.btnOK.Image = RetryImage;
                frm.btnRetry.Image = OkImage;
                frm.AcceptButton = frm.btnOK;
                frm.CancelButton = frm.btnRetry;
                frm.btnCancel.Visible = false;
                frm.SetTextAreaHeight(150);
                return frm.ShowDialog();
            }
        }

        /// <summary>
        /// Displays a messagebox with only a Cancel button and returns the dialog result.
        /// </summary>
        /// <param name="message">The message to be displayed</param>
        /// <returns></returns>
        public void ShowCancelOnlyMessageBoxWithSound(string message, List<Device> comScanners)
        {
            try
            {
                using (frmMessageBox frm = CreateMessageBox(message))
                {
                    frm.btnCancel.Visible = true;
                    frm.btnRetry.Visible = false;
                    frm.btnOK.Visible = false;
                    frm.AcceptButton = null;
                    frm.CancelButton = frm.btnCancel;

                    comScanners.ForEach(scanner =>
                    {
                        (scanner as Scanner).Pause();
                    });


                    Thread soundThread = new Thread(() =>
                    {
                        do
                        {
                            Helpers.UtilsHelper.HardwareBeep(2000, 600);
                            Helpers.UtilsHelper.HardwareBeep(1000, 300);
                            Helpers.UtilsHelper.HardwareBeep(2000, 600);
                        } while (true);
                    });

                    soundThread.Start();
                    soundThread.Join(100);
                    frm.ShowDialog();
                    soundThread.Join(100);
                    if (frm.DialogResult == DialogResult.Cancel)
                    {
                        soundThread.Join(2200);
                        soundThread.Abort();
                    }
                }
            }
            catch (ThreadAbortException ex)
            {

            }
            catch (Exception ex)
            {

            }
            finally
            {
                comScanners.ForEach(scanner =>
                {
                    (scanner as Scanner).Resume();
                });
            }
        }

        /// <summary>
        /// Shows a messagebox with the button combination provided.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogResult ShowMessageBox(string message, MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            using (frmMessageBox frm = CreateMessageBox(message))
            {
                switch (buttons)
                {
                    case MessageBoxButtons.OK:
                        frm.btnCancel.Visible = false;
                        frm.btnRetry.Visible = false;
                        frm.AcceptButton = frm.btnOK;
                        frm.CancelButton = frm.btnOK;
                        break;
                    case MessageBoxButtons.OKCancel:
                        frm.btnRetry.Visible = false;
                        frm.AcceptButton = frm.btnOK;
                        frm.CancelButton = frm.btnCancel;
                        break;
                    case MessageBoxButtons.RetryCancel:
                        frm.btnOK.Visible = false;
                        frm.AcceptButton = frm.btnRetry;
                        frm.CancelButton = frm.btnCancel;
                        break;
                    case MessageBoxButtons.YesNo:
                        frm.btnOK.Visible = false;
                        frm.btnRetry.Visible = false;
                        frm.btnCancel.Visible = false;
                        frm.btnYes.Visible = true;
                        frm.btnNo.Visible = true;
                        frm.AcceptButton = frm.btnYes;
                        frm.CancelButton = frm.btnNo;
                        break;
                    case MessageBoxButtons.YesNoCancel:
                        frm.btnOK.Visible = false;
                        frm.btnRetry.Visible = false;
                        frm.btnYes.Visible = true;
                        frm.btnNo.Visible = true;
                        frm.AcceptButton = frm.btnYes;
                        frm.CancelButton = frm.btnCancel;
                        break;
                    case MessageBoxButtons.AbortRetryIgnore:
                        throw new POSException("Unsupported buttons combination");
                }
                return frm.ShowDialog();
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.Winapi)]
        internal static extern IntPtr GetFocus();

        /// <summary>
        /// Gets the currently focused control of the application.
        /// </summary>
        /// <returns></returns>
        public Control GetFocusedControl()
        {
            Control focusedControl = null;
            // To get hold of the focused control:
            IntPtr focusedHandle = GetFocus();
            if (focusedHandle != IntPtr.Zero)
            {
                // Note that if the focused Control is not a .Net control, then this will return null.
                focusedControl = Control.FromHandle(focusedHandle);
            }
            return focusedControl;
        }

        /// <summary>
        /// Applies, if appropriate, the application's background image to the given form.
        /// </summary>
        /// <param name="form"></param>
        /// <param name="enablelowendmode"></param>
        public void HandleBackGroundImage(Form form, bool enablelowendmode)
        {

            if (enablelowendmode)
            {
                form.BackgroundImage = null;
            }
            else
            {
                form.BackgroundImage = ITS.POS.Client.Properties.Resources.BackgroundImage;
            }
        }


    }
}
