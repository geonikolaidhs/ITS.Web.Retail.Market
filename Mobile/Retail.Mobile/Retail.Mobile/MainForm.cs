using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Common;
using DevExpress.Xpo;
using System.IO;
using System.Xml;
using DevExpress.Data.Filtering;
using Retail.Mobile;
using Retail.Mobile_Model;
using ITS.Common.Keyboards.Compact;
using System.Threading;
using System.Reflection;
using OpenNETCF.Net.NetworkInformation;
using OpenNETCF.Net;
using System.Xml.Serialization;

namespace Retail.Mobile
{    
    public partial class MainForm : Form
    {
        public MainForm()
        {
            
            InitializeComponent();
            System.Net.ServicePointManager.CertificatePolicy = new TrustAllCertificatePolicy();
            lblBuildVersion.Text = this.GetBuildLabel();
            lblVersion.Text = this.GetVersion();
        }


        Guid Userid = Guid.Empty;
        NewXmlParser InputXml;

        #region metavlites
        Thread _connCheckerThread;

        TabPage LoginPage;
        TabPage SettingsPage;
        TabPage DetailOrdesPage;
        TabPage MasterOrdesPage;
        private Document _document;


        int timeout = 30000;
        int currentRecord = -1;

        LinesForm linesForm;


        public Document getDocument()
        {
            return _document;
        }
        #endregion

        public RetailService.RetailService GetService(int time)
        {
            RetailService.RetailService myservice = new RetailService.RetailService();
            myservice.Timeout = time;
            myservice.Url = AppSettings.IP;
            myservice.UserAgent = "Retail Mobile Client";
            return myservice;
        }

        private delegate void SetOnOffStatus(Boolean status);
        private void SetStatus(Boolean status)
        {
            lbl_status.ForeColor = Color.White;


            if (status)
            {
                lbl_status.Text = " "+Resources.Resources.Online;
                Text = "Retail --- "+Resources.Resources.Online;
                lbl_status.BackColor = Color.Green;
                if (_document != null)
                {
                    this.updateNonupdateRecordsOnline();
                }
                else
                    return;

                btnExport.Enabled = true;
            }
            else
            {
                lbl_status.Text = " "+Resources.Resources.Offline;
                lbl_status.BackColor = Color.Red;
                Text = "Retail --- "+Resources.Resources.Offline;
                btnExport.Enabled = false;
            }
        }

        private void CheckConnectionStatus()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            string thisVersion= string.Format("v{0}.{1}.{2}.{3}", new object[] { version.Major, version.Minor, version.Build, version.Revision });
            while (true)
            {
                try
                {

                    RetailService.RetailService ws = null;
                    //This will throw an exception if it cannot connect
                    try
                    {
                        ws = GetService(10000);
                        string remoteVersion = ws.GetVersion();
                        //if (remoteVersion != thisVersion)
                        //{
                        //    UpdaterProcess proc = new UpdaterProcess(BeginUpdate);
                        //    string[][] files = ws.GetMobileFilelist();
                        //    this.Invoke(proc, new object[] {files});
                        //}
                        AppSettings.OperationMode = AppSettings.OPERATION_MODE.ONLINE;
                        SetOnOffStatus SetStat = new SetOnOffStatus(SetStatus);
                        this.Invoke(SetStat, new object[] { true });
                    }
                    catch (Exception ex)
                    {
                       
                        AppSettings.OperationMode = AppSettings.OPERATION_MODE.BATCH;
                        SetOnOffStatus SetStat = new SetOnOffStatus(SetStatus);
                        this.Invoke(SetStat, new object[] { false });
                    }
                    if (ws != null)
                    {
                        ws.Dispose();
                    }
                }
                catch
                {
                }
                Thread.Sleep(10000);
            }
        }

        private delegate void UpdaterProcess(String[][] files);
        public void BeginUpdate(String[][] files)
        {
            System.Windows.Forms.MessageBox.Show("Υπάρχει νεότερη αναβάθμιση του λογισμικού.", "Retail@Mobile");

            //Create the updater directory
            Directory.CreateDirectory("\\Application\\its\\Updater");
            DirectoryInfo di_to = new DirectoryInfo("\\Application\\its\\Updater");
            //Determine the required files to be copied 
            string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            string path = appPath + "\\updater";
            //Get Directory info
            DirectoryInfo di = new DirectoryInfo(path);
            // Create an array representing the files in the current directory.
            FileInfo[] fi = di.GetFiles();
            // Print out the names of the files in the current directory.
            foreach (FileInfo fiTemp in fi)
            {
                if (File.Exists("\\Application\\its\\Updater\\" + fiTemp.Name))
                    File.Delete("\\Application\\its\\Updater\\" + fiTemp.Name);
                File.Copy(fiTemp.FullName, "\\Application\\its\\Updater\\" + fiTemp.Name);
            }
            ///Create Application Updater Object
            Updater.ApplicationSettingsForUpdater appUpdater = new Updater.ApplicationSettingsForUpdater();
            appUpdater.applicationName = "ITS RetailMobile";
            appUpdater.executableAfterInstall = Assembly.GetExecutingAssembly().GetName().CodeBase;
            appUpdater.filesTobeBackedUp = new String[2];
            appUpdater.filesTobeBackedUp[0] = AppSettings.databasePath + "\\Pda-Retail.db";
            appUpdater.filesTobeBackedUp[1] = appPath + "\\config.xml";
            appUpdater.installationProcedure = new String[1];
            appUpdater.installationProcedure[0] = "RetailMobileSetup.cab";
            appUpdater.mobileFileListToDownload = files;

            //TODO save to xml file
            XmlSerializer ser = new XmlSerializer(typeof(Updater.ApplicationSettingsForUpdater));
            string filename = "\\Application\\its\\Updater\\updater.xml";
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, appUpdater);
            writer.Close();

            System.Diagnostics.Process.Start("\\Application\\its\\Updater\\updater.exe", "\"" + System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + "\"");
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            
            //Application.Exit();

            //TODO clear temp folder content

        }

        private void HideTabPages(TabControl TabCtrl)
        {
            while (TabCtrl.TabPages.Count != 0)
                TabCtrl.TabPages.RemoveAt(0);
        }

        private void HideTabPages(TabControl TabCtrl, int index)
        {
            TabCtrl.TabPages.RemoveAt(index);
        }

        private void ShowTabPages(TabControl TabCtrl, TabPage Tabpg)
        {
            TabCtrl.TabPages.Add(Tabpg);
        }

        private void ShowTabPages(TabControl TabCtrl, TabPage Tabpg, int selectedpage)
        {
            TabCtrl.TabPages.Add(Tabpg);
            TabCtrl.SelectedIndex = selectedpage;
        }

        private void txtcustomer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                //string data = ServiceProvider.GetDataFromServiceWithGetMethod(@"http://localhost:40386/Customers/");
                string data = ServiceProvider.GetDataFromServiceWithPostMethod("Customers", "", "");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string data = ServiceProvider.GetDataFromServiceWithPostMethod("Customers", "", "");
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void bntlogin_Click(object sender, EventArgs e)
        {
            try
            {
                if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.ONLINE)
                {
                    using (RetailService.RetailService a = GetService(timeout))
                    {
                        Userid = a.Login(txt_user.Text, txt_pass.Text,1);

                        if (Userid != Guid.Empty)
                        {
                            using (UnitOfWork uow = new UnitOfWork(XpoDefault.Session.DataLayer))
                            {
                                Settings set = uow.FindObject<Settings>(CriteriaOperator.Parse("Oid>0", ""));
                                if (set == null)
                                    set = new Settings(uow);

                                set.UserID = Userid.ToString();
                                set.User = txt_user.Text;
                                set.Pass = txt_pass.Text;
                                uow.CommitChanges();
                            }

                            HideTabPages(MaintabControl);
                            ShowTabPages(MaintabControl, SettingstabPage);
                        }
                        else
                        {
                            MessageBox.Show(Resources.Resources.UserNotFound, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                            txt_user.Text = "";
                            txt_pass.Text = "";
                            txt_user.Focus();

                        }
                    }
                }
                else
                {
                    using (UnitOfWork uow = new UnitOfWork(XpoDefault.Session.DataLayer))
                    {
                        Settings set = uow.FindObject<Settings>(CriteriaOperator.Parse("Oid>0", ""));
                        if (set == null)
                        {
                            MessageBox.Show(Resources.Resources.ApplicationCoundNotConnectToServer, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);
                        }
                        else
                        {
                            Userid = new Guid(set.UserID);
                            HideTabPages(MaintabControl);
                            ShowTabPages(MaintabControl, SettingstabPage);
                            SettingstabPage.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }
        }

        private void txt_search_item_KeyPress(object sender, KeyPressEventArgs e)
        {


            try
            {
                if (e.KeyChar == 13 && currentRecord == -1)
                {
                    if (txt_search_item.Text.Trim() == "")
                    {
                        return;
                    }

                    if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.BATCH)
                    {
                        decimal qty = 0;

                        //DocLine line = XpoDefault.Session.FindObject<DocLine>(CriteriaOperator.Parse(string.Format("[DocHead] = '{0}' AND ItemCode = '{1}'", _document.Header.Oid, txt_search_item.Text.Trim())));
                        CriteriaOperator cr = CriteriaOperator.And(
                                new BinaryOperator("DocHead.Oid", _document.Header.Oid),
                                CriteriaOperator.Or(
                                    new BinaryOperator("ItemCode", txt_search_item.Text.Trim().PadLeft(7, '0')),
                                    new BinaryOperator("Barcode", txt_search_item.Text.Trim().PadLeft(14, '0')),
                                    new BinaryOperator("ItemCode", txt_search_item.Text.Trim()),
                                    new BinaryOperator("Barcode", txt_search_item.Text.Trim())
                                )
                                ,(new NullOperator("_RefDocLine"))
                                );
                        
                        DocLine line = XpoDefault.Session.FindObject<DocLine>(cr);
                            
                        if (line != null && line.ItemCode.PadLeft(7, '0') != txt_search_item.Text.Trim().PadLeft(7, '0') && line.Barcode.PadLeft(14, '0') != txt_search_item.Text.Trim().PadLeft(14, '0'))
                            line = null;
                        qty = (line != null ? (decimal)line.Qty : 0);

                        if (!KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true, 7, 3, NumKeypad.OPERATOR.FORBID_OPERATORS, qty, Resources.Resources.Quantity))
                            return;
                        if (qty == 0)
                        {
                            if (line == null)
                            {
                                MessageBox.Show(Resources.Resources.CannotInsertWithZeroValue, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                                return;
                            }
                            else
                            {
                                if (MessageBox.Show(Resources.Resources.WouldYouLikeToDeleteRecord, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                {
                                    _document.Header.DocLines.Remove(line);
                                    _document.Header.UpdateDocumentHeader();
                                    UpdateDocumentHeaderDisplay();
                                }
                                else
                                    return;
                            }
                        }
                        else
                        {
                            txt_qty.Text = qty.ToString();
                            if (line == null)
                            {

                                line = new DocLine(XpoDefault.Session);
                                line.Qty = Convert.ToDouble(txt_qty.Text);
                                line.DocHead = _document.Header;
                                line.ItemCode = txt_search_item.Text.Trim();

                                _document.Header.DocLines.Add(line);
                            }
                            else//addorreplace
                            {
                                Retail.Mobile.AddOrReplace.AddOrReplaceResult addOrReplaceResult = AddOrReplace.Execute();

                                if (addOrReplaceResult == Retail.Mobile.AddOrReplace.AddOrReplaceResult.ADD)
                                {
                                    line.Qty = line.Qty + (double)qty;
                                    if (line.Qty >= 10000)
                                    {
                                        MessageBox.Show(Resources.Resources.MaxOrderQtyError, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                                        line.Qty = 9999.999;
                                    }
                                }
                                else if (addOrReplaceResult == AddOrReplace.AddOrReplaceResult.REPLACE)
                                {
                                    line.Qty = (double)qty;
                                }
                                else
                                {
                                    updateCurrentRecord();
                                    return;
                                }
                            }
                        }
                        line.EditOffline = true;
                        txt_qty.Text = qty.ToString();

                        //DocLine line = new DocLine(XpoDefault.Session);                      
                        //line.Qty = Convert.ToDouble(txt_qty.Text);

                        _document.Header.Save();
                        updateCurrentRecord();
                    }
                    else//online
                    {
                        using (RetailService.RetailService Myservice = GetService(timeout))
                        {
                            Boolean isbarcode = false, inputBarcode = true;
                            string itemxml = Myservice.GetItemWithBarcode(txt_search_item.Text.Trim(), "Name,Code,Oid,PackingQty", Userid.ToString());
                            if (itemxml == "-1")
                            {
                                inputBarcode = false;
                                itemxml = Myservice.GetItem("Code='" + txt_search_item.Text.Trim() + "'", "Name,Code,Oid,PackingQty,DefaultBarcode", Userid.ToString());
                                isbarcode = true;
                            }
                            if (itemxml == "-1")
                            {
                                itemxml = Myservice.GetItem("Name='" + txt_search_item.Text.Trim() + "'", "Name,Code,Oid,PackingQty,DefaultBarcode", Userid.ToString());
                            }

                            if (itemxml == "-1")
                            {
                                MessageBox.Show(Resources.Resources.ItemNotFound, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                                txt_search_item.SelectAll();
                                txt_search_item.Focus();
                            }
                            else
                            {
                                InputXml = new NewXmlParser(itemxml);
                                if (InputXml.GetError().Length > 1)
                                {
                                    MessageBox.Show(Resources.Resources.ConnectionError, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                                    txt_search_item.SelectAll();
                                    txt_search_item.Focus();
                                    return;
                                }
                                decimal qty = 0;

                                
                                String barcodeToSend = (inputBarcode) ? txt_search_item.Text.Trim() : InputXml.GetProperty2("Item", "DefaultBarcode");
                                CriteriaOperator cr = CriteriaOperator.And(
                                        new BinaryOperator("DocHead.Oid", _document.Header.Oid),
                                        new BinaryOperator("Barcode", barcodeToSend.Trim().PadLeft(14, '0')),
                                        (new NullOperator("_RefDocLine"))
                                    );
                                DocLine line=XpoDefault.Session.FindObject<DocLine>(cr);

                                txt_item_details.Text = InputXml.GetProperty2("Item", "Name").Trim() + "\r\n" + InputXml.GetProperty2("Item", "Code").Trim();


                                double packingQty = Double.Parse(InputXml.GetProperty2("Item", "PackingQty").Trim());

                                qty = (line != null ? (decimal)line.Qty : 0);

                                if (txt_qty.Text.Length == 0)
                                {
                                    if (!KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true, 7, 3, NumKeypad.OPERATOR.FORBID_OPERATORS, qty, InputXml.GetProperty2("Item", "Code").Trim() + " - " + InputXml.GetProperty2("Item", "Name").Trim() + "(Συσκ: " + packingQty + ")"))
                                        return;

                                }
                                else
                                {
                                    qty = Decimal.Parse(txt_qty.Text);
                                }

                                if (line != null)
                                {
                                    Retail.Mobile.AddOrReplace.AddOrReplaceResult addOrReplaceResult = AddOrReplace.Execute();

                                    if (addOrReplaceResult == AddOrReplace.AddOrReplaceResult.CANCEL)
                                    {
                                        updateCurrentRecord();
                                        return;
                                    }
                                    if (addOrReplaceResult == Retail.Mobile.AddOrReplace.AddOrReplaceResult.ADD)
                                    {

                                        qty += (decimal)line.Qty;
                                        if (qty >= 10000)
                                        {
                                            MessageBox.Show(Resources.Resources.MaxOrderQtyError, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                                            qty = (decimal)9999.999;
                                        }                                      
                                    }
                                }
                                txt_qty.Text = qty.ToString();

                                if (qty == 0)
                                {
                                    if (line == null)
                                    {
                                        MessageBox.Show(Resources.Resources.CannotInsertWithZeroValue, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);

                                    }
                                    else
                                    {
                                        if (MessageBox.Show(Resources.Resources.WouldYouLikeToDeleteRecord, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                                        {
                                            foreach (DocLine dc in line.RefLines)
                                            {
                                                _document.Header.DocLines.Remove(dc);
                                            }
                                            line.Session.Delete(line.RefLines);
                                             
                                            _document.Header.DocLines.Remove(line);
                                            _document.Header.UpdateDocumentHeader();
                                            UpdateDocumentHeaderDisplay();
                                        }
                                    }
                                    _document.Header.Save();
                                    currentRecord = -1;
                                    updateCurrentRecord();
                                    return;

                                }


                                using (RetailService.RetailService getLine = GetService(timeout))
                                {
                                    
                                    Settings set = XpoDefault.Session.FindObject<Settings>(CriteriaOperator.Parse("Oid>0", ""));

                                    RetailService.TransferedDocumentDetail[] lineDetails = getLine.GetDocumentDetail(set.UserID, barcodeToSend, (double)qty);
                                    //NewXmlParser doclinexml = new NewXmlParser(lineDetails);
                                    if (lineDetails.Length == 1 && lineDetails[0].Barcode == null)
                                    {
                                        MessageBox.Show(Resources.Resources.Error+":\r\n"+lineDetails[0].ItemName, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                                    }
                                    if (line == null)
                                        line = new DocLine(XpoDefault.Session);
                                    else
                                    {
                                        foreach (DocLine dc in line.RefLines)
                                        {
                                            _document.Header.DocLines.Remove(dc);
                                        }
                                        line.Session.Delete(line.RefLines);                                        
                                    }
                                    line.PackingQty = packingQty;
                                    try
                                    {
                                        int i = 0;
                                        foreach (RetailService.TransferedDocumentDetail tline in lineDetails)
                                        {
                                            DocLine currentLine;
                                            currentLine = (i == 0) ? line : new DocLine(XpoDefault.Session);
                                            currentLine.Barcode = tline.Barcode;
                                            currentLine.FinalUnitPrice = tline.FinalUnitPrice;
                                            currentLine.FirstDiscount = tline.FirstDiscount;
                                            currentLine.GrossTotal = tline.GrossTotal;
                                            currentLine.ItemCode = tline.ItemCode;
                                            currentLine.ItemName = tline.ItemName;
                                            currentLine.ItemOid = tline.ItemOid.ToString();
                                            currentLine.ItemPrice = tline.UnitPrice;
                                            currentLine.NetTotal = tline.NetTotal;
                                            currentLine.NetTotalAfterDiscount = tline.NetTotalAfterDiscount;
                                            currentLine.Qty = tline.Qty;
                                            currentLine.SecondDiscount = tline.SecondDiscount;
                                            currentLine.TotalDiscount = tline.TotalDiscount;
                                            currentLine.TotalVatAmount = tline.TotalVatAmount;
                                            currentLine.UnitPriceAfterDiscount = tline.UnitPriceAfterDiscount;
                                            currentLine.VatAmount = tline.VatAmount;
                                            currentLine.VatFactor = tline.VatFactor;
                                            currentLine.EditOffline = false;
                                            if (i > 0)
                                            {
                                                line.RefLines.Add(currentLine);
                                                currentLine.RefDocLine = line;
                                            }
                                            _document.Header.DocLines.Add(currentLine);
                                            i++;
                                        }                                        
                                        txt_price.Text = line.ItemPrice.ToString("c", Common.CultureInfo);

                                    }
                                    catch (Exception exept)
                                    {
                                        int rr = 0;
                                    }


                                   
                                    _document.Header.UpdateDocumentHeader();
                                    UpdateDocumentHeaderDisplay();
                                }



                                txt_qty.Text = line.Qty.ToString();
                                _document.Header.Save();
                                txtRecords.Text = _document.Header.DocLines.Count.ToString();

                                updateCurrentRecord();

                            }
                        }
                    }
                }
                else if (e.KeyChar == 13)
                {
                    CheckForUpdatesInLine();
                }
            }
            catch (Exception ex)
            {
                string message = ex.Message;
                if(ex.InnerException!=null && String.IsNullOrEmpty(ex.InnerException.Message)==false)
                {
                    message += "\r\n";
                    message += ex.InnerException.Message;
                }
                MessageBox.Show(message);
            }

        }

        private void CheckForUpdatesInLine()
        {
            ////////
            if (currentRecord == -1)
                return;


            if (txt_qty.Text.Length == 0)
            {
                txt_qty.Text = "0";
            }
            Decimal dec;
            try
            {
                dec = Decimal.Parse(txt_qty.Text);
                DocLine line = _document.Header.DocLines[currentRecord];
                if (dec == 0)
                {
                    if (MessageBox.Show(Resources.Resources.WouldYouLikeToDeleteRecord, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        foreach (DocLine ln in line.RefLines)
                        {
                            _document.Header.DocLines.Remove(ln);
                        }

                        _document.Header.DocLines.Remove(line);
                        _document.Header.Save();
                        _document.Header.UpdateDocumentHeader();
                        UpdateDocumentHeaderDisplay();
                        currentRecord = -1;
                    }
                    updateCurrentRecord();
                    return;
                }
                else if (dec > AppSettings.limit || dec < -AppSettings.limit)
                {
                    MessageBox.Show(Resources.Resources.InvalidQty, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                    updateCurrentRecord();
                    return;
                }
                else
                {
                    if ((decimal)line.Qty == dec)
                    {
                        return;
                    }
                    if (MessageBox.Show(Resources.Resources.WouldYouLikeToChangeQty, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    {
                        currentRecord = -1;
                        updateCurrentRecord();

                        return;
                    }
                    if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.BATCH)
                    {
                        line.Qty = (double)dec;
                        line.EditOffline = true;
                        foreach (DocLine ln in line.RefLines)
                        {
                            ln.Qty  = (double)dec;
                        }
                    }
                    else
                    {
                        using (RetailService.RetailService getLine = GetService(timeout))
                        {
                            String barcodeToSend = _document.Header.DocLines[currentRecord].Barcode;
                            Settings set = XpoDefault.Session.FindObject<Settings>(CriteriaOperator.Parse("Oid>0", ""));

                            RetailService.TransferedDocumentDetail[] lineDetails = getLine.GetDocumentDetail(set.UserID, barcodeToSend, (double)dec);
                            if (lineDetails.Length == 1 && lineDetails[0].Barcode == null)
                            {
                                MessageBox.Show(Resources.Resources.Error+":\r\n" + lineDetails[0].ItemName, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                            }
                            //NewXmlParser doclinexml = new NewXmlParser(lineDetails);
                            try
                            {
                                int i = 0;
                                foreach (RetailService.TransferedDocumentDetail tline in lineDetails)
                                {
                                    DocLine currentLine=null;
                                    if (i == 0)
                                    {
                                        currentLine = line;
                                    }
                                    else
                                    {
                                        foreach (DocLine dc in line.RefLines)
                                        {
                                            if (dc.ItemCode == tline.ItemCode)
                                                currentLine = dc;
                                        }
                                    }
                                    if (currentLine == null)
                                        currentLine = new DocLine(XpoDefault.Session);
                                    currentLine.Barcode = tline.Barcode;
                                    currentLine.FinalUnitPrice = tline.FinalUnitPrice;
                                    currentLine.FirstDiscount = tline.FirstDiscount;
                                    currentLine.GrossTotal = tline.GrossTotal;
                                    currentLine.ItemCode = tline.ItemCode;
                                    currentLine.ItemName = tline.ItemName;
                                    currentLine.ItemOid = tline.ItemOid.ToString();
                                    currentLine.ItemPrice = tline.UnitPrice;
                                    currentLine.NetTotal = tline.NetTotal;
                                    currentLine.NetTotalAfterDiscount = tline.NetTotalAfterDiscount;
                                    currentLine.Qty = tline.Qty;
                                    currentLine.SecondDiscount = tline.SecondDiscount;
                                    currentLine.TotalDiscount = tline.TotalDiscount;
                                    currentLine.TotalVatAmount = tline.TotalVatAmount;
                                    currentLine.UnitPriceAfterDiscount = tline.UnitPriceAfterDiscount;
                                    currentLine.VatAmount = tline.VatAmount;
                                    currentLine.VatFactor = tline.VatFactor;
                                    currentLine.EditOffline = false;
                                    if (i > 0)
                                    {
                                        line.RefLines.Add(currentLine);
                                        currentLine.RefDocLine = line;
                                    }
                                    _document.Header.DocLines.Add(currentLine);
                                    i++;
                                }  
                            }
                            catch (Exception exept)
                            {
                                int rr = 0;
                            }

                        }
                    }
                    _document.Header.Save();
                    _document.Header.UpdateDocumentHeader();
                    currentRecord = -1;
                    UpdateDocumentHeaderDisplay();
                    updateCurrentRecord();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Resources.Resources.InvalidQty+"."+Resources.Resources.YourChangesWillNotBeSaved, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
                return;
            }
            ////////
        }

        private void txt_qty_KeyPress(object sender, KeyPressEventArgs e)
        {
            txt_search_item_KeyPress(sender, e);
        }

        public void TranslateMainForm()
        {
            tabPage2.Text = Resources.Resources.Login;
            label9.Text = Resources.Resources.InsertYourData;
            label10.Text = Resources.Resources.Username;
            label2.Text = Resources.Resources.Password;
            btnClose.Text = Resources.Resources.Exit;
            btnConnect.Text = Resources.Resources.Login;

            tabPage3.Text = Resources.Resources.CheckOut;


            label14.Text = Resources.Resources.Status;
            label7.Text = Resources.Resources.ValueBeforeDiscount;
            label13.Text = Resources.Resources.Discount;
            label12.Text = Resources.Resources.VAT;
            label8.Text = Resources.Resources.Sum;
            button23.Text = Resources.Resources.Delete;
            btnExport.Text = Resources.Resources.Send;
            btn_showmainmenu.Text = Resources.Resources.Exit;
            label6.Text = Resources.Resources.Type;
            label11.Text = Resources.Resources.Series;


            OrderstabPage.Text = Resources.Resources.DocumentLines;
            lblRecords.Text = Resources.Resources.Search;
            lblProduct.Text = Resources.Resources.ItemCode;
            label3.Text = Resources.Resources.Quantity;
            label4.Text = Resources.Resources.Value;
            lbl_status.Text = Resources.Resources.Offline;
            btnReturn.Text = Resources.Resources.Details;
            btnNew.Text = Resources.Resources.NewItem;
            button22.Text = Resources.Resources.Exit;

            SettingstabPage.Text = Resources.Resources.DocumentLines;
            btn_orders.Text = "[1]" + Resources.Resources.Order;
            btn_settings.Text = "[2]" + Resources.Resources.Settings;
            button21.Text = "[0]" + Resources.Resources.Exit;

            //tabPage1
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                TranslateMainForm();                
                

                _connCheckerThread = new Thread(new ThreadStart(this.CheckConnectionStatus));
                _connCheckerThread.Start();

                //Orise ta tabpages gia na mporeis na ta kaleis 
                LoginPage = MaintabControl.TabPages[0];
                MasterOrdesPage = MaintabControl.TabPages[1];
                DetailOrdesPage = MaintabControl.TabPages[2];
                SettingsPage = MaintabControl.TabPages[3];


                using (UnitOfWork uow = new UnitOfWork(XpoDefault.Session.DataLayer))
                {
                    Settings set = uow.FindObject<Settings>(CriteriaOperator.Parse("Oid>0", ""));
                    if (set == null)
                    {
                        HideTabPages(MaintabControl);
                        ShowTabPages(MaintabControl, LoginPage, 0);
                        //ShowTabPages(MaintabControl, SettingstabPage,0);
                        txt_user.Focus();
                    }
                    else
                    {
                        Userid = new Guid(set.UserID);
                        HideTabPages(MaintabControl);
                        ShowTabPages(MaintabControl, SettingstabPage);

                    }
                }




            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }
        }


        private void menuItem2_Click(object sender, EventArgs e)
        {
            using (SettingsForm frm = new SettingsForm(this))
            {
                frm.ShowDialog();
            }
            updateCurrentRecord();
        }

        private void menuItem3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_orders_Click(object sender, EventArgs e)
        {
            LoadingForm ldForm = new LoadingForm();
            ldForm.Left = (Screen.PrimaryScreen.WorkingArea.Width - ldForm.Width) / 2;
            ldForm.Top = (Screen.PrimaryScreen.WorkingArea.Height - ldForm.Height) / 2;
            ldForm.Height = 43;

            ldForm.Show();
            Application.DoEvents();
            InitOrders();

            ldForm.Hide();
            ldForm.Dispose();

            HideTabPages(MaintabControl);

            ShowTabPages(MaintabControl, DetailOrdesPage);

            ShowTabPages(MaintabControl, MasterOrdesPage, 0);

            _document = new Document();
            _document.Load();//DOC_TYPES.ORDER);
            txtRecords.Text = _document.Header.DocLines.Count.ToString();

            _document.Header.UpdateDocumentHeader();
            UpdateDocumentHeaderDisplay();
            updateCurrentRecord();
        }


        private void InitOrders()
        {
            try
            {
                if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.ONLINE)
                {
                    using (RetailService.RetailService a = GetService(timeout))
                    {
                        using (UnitOfWork uow = new UnitOfWork(XpoDefault.Session.DataLayer))
                        {
                            string xmldata = a.GetSeries(Userid.ToString(), 0);

                            InputXml = new NewXmlParser(xmldata);
                            if (InputXml.GetError() != "")
                            {
                                MessageBox.Show(InputXml.GetError());
                                return;
                            }


                            string customerid = InputXml.GetProperty2("customer", "customerid");
                            string customername = InputXml.GetProperty2("customer", "customername");

                            string storeid = InputXml.GetProperty2("store", "storeid");
                            string storename = InputXml.GetProperty2("store", "storename");

                            string companyid = InputXml.GetProperty2("company", "companyid");
                            string companyname = InputXml.GetProperty2("company", "companyname");

                            Settings set = uow.FindObject<Settings>(CriteriaOperator.Parse("Oid>0", ""));
                            if (set == null)
                                set = new Settings(uow);

                            set.CompanyID = companyid;
                            set.CompanyName = companyname;
                            set.StoreID = storeid;
                            set.StoreName = storename;
                            set.CustomerID = customerid;
                            set.CustomerName = customername;


                            uow.CommitChanges();

                            XmlNodeList nodelist = InputXml.GetNodeList("types");
                            for (int i = 0; i < nodelist.Count; i++)
                            {
                                Types_Order to = uow.FindObject<Types_Order>(CriteriaOperator.Parse("Id='" + InputXml.GetProperty(nodelist[i], "oid") + "'", ""));
                                if (to == null)
                                    to = new Types_Order(uow);

                                to.Id = new Guid(InputXml.GetProperty(nodelist[i], "oid"));
                                to.Descr = InputXml.GetProperty(nodelist[i], "name");
                                to.Save();
                            }
                            uow.CommitChanges();

                            nodelist = InputXml.GetNodeList("series");
                            for (int i = 0; i < nodelist.Count; i++)
                            {
                                Series_order so = uow.FindObject<Series_order>(CriteriaOperator.Parse("Id='" + InputXml.GetProperty(nodelist[i], "oid") + "'" + " and Type_Order.Id='" + InputXml.GetProperty(nodelist[i], "type") + "'", ""));
                                if (so == null)
                                    so = new Series_order(uow);

                                so.Type_Order = uow.FindObject<Types_Order>(CriteriaOperator.Parse("Id='" + InputXml.GetProperty(nodelist[i], "type") + "'", ""));
                                so.Id = new Guid(InputXml.GetProperty(nodelist[i], "oid"));
                                so.Descr = InputXml.GetProperty(nodelist[i], "name");
                                so.Save();
                            }

                            nodelist = InputXml.GetNodeList("status");
                            for (int i = 0; i < nodelist.Count; i++)
                            {
                                OrderStatus so = uow.FindObject<OrderStatus>(CriteriaOperator.Parse("Id='" + InputXml.GetProperty(nodelist[i], "oid") + "'", ""));
                                if (so == null)
                                    so = new OrderStatus(uow);
                                so.Id = new Guid(InputXml.GetProperty(nodelist[i], "oid"));
                                so.Descr = InputXml.GetProperty(nodelist[i], "description");
                                so.IsDefault = Boolean.Parse(InputXml.GetProperty(nodelist[i], "IsDefault"));
                                so.Save();
                            }

                            uow.CommitChanges();
                        }
                    }
                }
                else//offline
                {

                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            XPCollection<OrderStatus> orders = new XPCollection<OrderStatus>(XpoDefault.Session, null, new SortProperty("IsDefault", DevExpress.Xpo.DB.SortingDirection.Descending));
            cbKind.DataSource = orders;
            cbKind.ValueMember = "Id";
            cbKind.DisplayMember = "Descr";
        }

        private void btn_settings_Click(object sender, EventArgs e)
        {
            using (SettingsForm frm = new SettingsForm(this))
            {
                frm.ShowDialog();
            }
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            try
            {
                if (_connCheckerThread != null)
                {

                    _connCheckerThread.Abort();
                    _connCheckerThread = null;
                }
                XpoDefault.Session.Dispose();
                XpoDefault.DataLayer.Dispose();
                System.Diagnostics.Process.GetCurrentProcess().Kill();
               // this.Close();
            }
            catch (Exception ex)
            {

            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                txt_pass.Focus();
                txt_pass.SelectAll();
            }
        }

        private void txt_pass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                bntlogin_Click(null, null);
            }
        }

        private void cbtypes_SelectedValueChanged(object sender, EventArgs e)
        {
            try
            {
                string id = "";
                if (cbtypes.SelectedValue is Types_Order)
                {
                    id = (cbtypes.SelectedValue as Types_Order).Id.ToString();
                }
                else
                    id = cbtypes.SelectedValue.ToString();

                XPCollection<Series_order> series = new XPCollection<Series_order>(XpoDefault.Session, CriteriaOperator.Parse("Type_Order.Id='" + id + "'", ""));
                cbseries.DataSource = series;
                cbseries.ValueMember = "Id";
                cbseries.DisplayMember = "Descr";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.BATCH)
            {
                MessageBox.Show(Resources.Resources.ConnectToServerPrompt, AppSettings.Title, MessageBoxButtons.OK, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1);
            }
            else
            {
                if (MessageBox.Show(Resources.Resources.WouldYouLikeToSendYourOrder, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    ExportDocument();//0);//,false);
                    _document.Header.UpdateDocumentHeader();
                    this.txtTotalAmount.Text = _document.Header.NetTotal.ToString("c", Common.CultureInfo);
                    this.txtDiscount.Text = _document.Header.TotalDiscountAmount.ToString("c", Common.CultureInfo);
                    this.txtVAT.Text = _document.Header.TotalVatAmount.ToString("c", Common.CultureInfo);
                    this.txtTotalSum.Text = _document.Header.GrossTotal.ToString("c", Common.CultureInfo);
                }
            }
        }

        private void ExportDocument()//DOC_TYPES type)
        {

            try
            {
                DocHead head = XpoDefault.Session.FindObject<DocHead>(null);//CriteriaOperator.Parse("DocType=" + (int)type, ""));

                if (head == null || head.DocLines.Count == 0)
                {
                    //if (isTemporary)
                    //    return;
                    MessageBox.Show(Resources.Resources.PleaseInsertItems);
                    return;
                }


                if (head != null)
                {
                    string statusid = "";
                    if (cbKind.SelectedValue is Types_Order)
                    {
                        statusid = (cbKind.SelectedValue as Types_Order).Id.ToString();
                    }
                    else
                        statusid = cbKind.SelectedValue.ToString();



                    string cbKindtxt = "";
                    if (cbKind.SelectedValue is Types_Order)
                    {
                        cbKindtxt = (cbKind.SelectedValue as Series_order).Id.ToString();
                    }
                    else
                        cbKindtxt = cbKind.SelectedValue.ToString();

                    Settings set = XpoDefault.Session.FindObject<Settings>(CriteriaOperator.Parse("Oid>0", ""));


                    NewXmlCreator xml2send = new NewXmlCreator("", "");
                    List<string> data = new List<string>();
                    //data.Add("text=" + seriesid + "|id=Series");
                    //data.Add("text=" + typeid + "|id=Type");

                    data.Add("text=" + cbKindtxt + "|id=Status");
                    //data.Add("text=" + (int)type + "|id=Division");
                    data.Add("text=" + set.CompanyID + "|id=companyid");
                    data.Add("text=" + set.StoreID + "|id=storeid");
                    data.Add("text=" + set.CustomerID + "|id=customerid");
                    data.Add("text=" + head.HeadOid + "|id=RemoteDeviceDocumentHeaderGuid");
                    data.Add("text=" + Userid.ToString() + "|id=User");

                    xml2send.CreateNodes("Header", "item", data.ToArray());


                    for (int i = 0; i < head.DocLines.Count; i++)
                    {
                        if (head.DocLines[i].RefDocLine == null)
                        {
                            int q = (int)Math.Round(head.DocLines[i].Qty * 1000);
                            data.Clear();
                            data.Add("item|" + ((head.DocLines[i].Barcode == null || head.DocLines[i].Barcode == "") ? head.DocLines[i].ItemCode : head.DocLines[i].Barcode));
                            data.Add("qty|" + q);
                            xml2send.CreateNodes("lines", data.ToArray());
                        }
                    }

                    xml2send.Xmlclose();

                    using (RetailService.RetailService service = GetService(timeout))
                    {

                        RetailService.TInvalidItem[] invalidItems = service.ValidateOrder(xml2send.MyXml);
                        if (invalidItems == null || invalidItems.Length == 0)
                        {
                            string answer = service.PostDocument(xml2send.MyXml);
                            if (answer != "1")
                            {
                                InputXml = new NewXmlParser(answer);
                                if (InputXml.GetError() != "")
                                {
                                    MessageBox.Show(InputXml.GetError());
                                }
                            }
                            else
                            {
                                MessageBox.Show(Resources.Resources.OrderSuccesfullySend);

                                XpoDefault.Session.BeginTransaction();

                                XPCollection<DocLine> col = new XPCollection<DocLine>(XpoDefault.Session);
                                col.DeleteObjectOnRemove = true;
                                //col.Criteria = CriteriaOperator.Parse("[DocHead] = ?", _document.Header.Oid.ToString());
                                XpoDefault.Session.Delete(col);
                                XPCollection<DocHead> heads = new XPCollection<DocHead>(XpoDefault.Session);
                                XpoDefault.Session.Delete(heads);
                                XpoDefault.Session.CommitTransaction();

                                txt_search_item.Text = "";
                                txt_qty.Text = "";
                                txt_price.Text = "";
                                txt_item_details.Text = "";
                                txt_search_item.Focus();
                                txtRecords.Text = "0";
                            }
                        }
                        else
                        {
                            MessageBox.Show(Resources.Resources.Found + " " + invalidItems.Length + " " + Resources.Resources.SupplierItemsNotFound);
                            foreach (Retail.Mobile.RetailService.TInvalidItem invItem in invalidItems)
                            {
                                DocLine dc = XpoDefault.Session.FindObject<DocLine>(new BinaryOperator("ItemOid", invItem.ItemOid.ToString()));
                                if (dc != null)
                                {
                                    XpoDefault.Session.BeginTransaction();
                                    dc.DocHead = null;                                    
                                    dc.Delete();
                                    XpoDefault.Session.CommitTransaction();
                                }
                            }
                            ExportDocument();//type);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //if(isTemporary)
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLines_Click(object sender, EventArgs e)
        {

            using (LinesForm frm = new LinesForm(this._document, this))
            {
                linesForm = frm;
                frm.ShowDialog();
                linesForm = null;
            }

            updateCurrentRecord();
        }

        private void btn_showmainmenu_Click(object sender, EventArgs e)
        {
            HideTabPages(MaintabControl);
            ShowTabPages(MaintabControl, SettingstabPage);
            SettingstabPage.Focus();
        }



        private void MaintabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MaintabControl.SelectedIndex >= 0 && MaintabControl.TabPages[MaintabControl.SelectedIndex].Name == "OrderstabPage")
                txt_search_item.Focus();
            if (MaintabControl.SelectedIndex >= 0 && MaintabControl.TabPages[MaintabControl.SelectedIndex].Name == "SettingstabPage")
                this.Focus();
        }





        private void button22_Click(object sender, EventArgs e)
        {
            MaintabControl.SelectedIndex = 1;
        }


        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (MaintabControl.SelectedIndex >= 0 && MaintabControl.TabPages[MaintabControl.SelectedIndex].Name == "SettingstabPage")
            {
                switch (e.KeyChar)
                {
                    case '1':
                        btn_orders_Click(null, null);
                        break;
                    case '2':
                        btn_settings_Click(null, null);
                        break;
                    case '0':
                        btn_exit_Click(null, null);
                        break;
                    default:

                        e.Handled = true;
                        break;
                }
            }
        }

        private void txt_search_item_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 40)
            {
                this.SelectNextControl(sender as Control, true, true, true, true);
            }

        }

        private void txt_qty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 38)
            {
                this.SelectNextControl(sender as Control, false, true, true, true);
            }
        }
        public void UpdateDocumentHeaderDisplay()
        {
            if (AppSettings.OperationMode == AppSettings.OPERATION_MODE.BATCH)
            {
                this.txtTotalAmount.Text = "------";
                this.txtDiscount.Text = "------";
                this.txtVAT.Text = "------";
                this.txtTotalSum.Text = "------";
            }
            else
            {
                this.txtTotalAmount.Text = _document.Header.NetTotal.ToString("c", Common.CultureInfo);
                this.txtDiscount.Text = _document.Header.TotalDiscountAmount.ToString("c", Common.CultureInfo);
                this.txtVAT.Text = _document.Header.TotalVatAmount.ToString("c", Common.CultureInfo);
                this.txtTotalSum.Text = _document.Header.GrossTotal.ToString("c", Common.CultureInfo);
            }
        }


        private void updateNonupdateRecordsOnline()
        {
            if (AppSettings.OperationMode != AppSettings.OPERATION_MODE.ONLINE)
                return;

            List<DocLine> notFound = new List<DocLine>();
            int counter = 0;
            foreach (DocLine line in _document.Header.DocLines)
            {
                try
                {
                    if (line.EditOffline == true || line.Barcode == null || line.Barcode == "")
                    {
                        counter++;
                        using (RetailService.RetailService Myservice = GetService(timeout))
                        {
                            double packingQty = 0;
                            if (line.Barcode == null || line.Barcode == "")
                            {
                                line.Barcode = line.ItemCode;
                                string itemxml = Myservice.GetItemWithBarcode(line.ItemCode, "Name,Code,Oid,PackingQty", Userid.ToString());
                                if (itemxml == "-1")
                                {
                                    line.Barcode = null;
                                    itemxml = Myservice.GetItem("Code='" + line.ItemCode + "'", "Name,Code,Oid,PackingQty,DefaultBarcode", Userid.ToString());
                                }
                                if (itemxml == "-1")
                                {
                                    itemxml = Myservice.GetItem("Name='" + line.ItemCode + "'", "Name,Code,Oid,PackingQty,DefaultBarcode", Userid.ToString());
                                }

                                if (itemxml == "-1")
                                {
                                    notFound.Add(line);
                                    continue;
                                }
                                InputXml = new NewXmlParser(itemxml);
                                packingQty = Double.Parse(InputXml.GetProperty2("Item", "PackingQty").Trim());
                                if (line.Barcode == null)
                                {

                                    line.Barcode = InputXml.GetProperty2("Item", "DefaultBarcode");
                                }
                            }
                            else
                                packingQty = line.PackingQty;

                            String barcodeToSend = line.Barcode;
                            Settings set = XpoDefault.Session.FindObject<Settings>(CriteriaOperator.Parse("Oid>0", ""));

                            RetailService.TransferedDocumentDetail[] lineDetails = Myservice.GetDocumentDetail(set.UserID, barcodeToSend, line.Qty);
                            if (lineDetails.Length == 1 && lineDetails[0].Barcode == null)
                            {
                                notFound.Add(line);
                                continue;
                            }
                            foreach (DocLine dc in line.RefLines)
                            {
                                _document.Header.DocLines.Remove(dc);
                            }
                            line.Session.Delete(line.RefLines);

                            try
                            {
                                int i = 0;
                                foreach (RetailService.TransferedDocumentDetail tline in lineDetails)
                                {
                                    DocLine currentLine;
                                    currentLine = (i == 0) ? line : new DocLine(XpoDefault.Session);
                                    currentLine.Barcode = tline.Barcode;
                                    currentLine.FinalUnitPrice = tline.FinalUnitPrice;
                                    currentLine.FirstDiscount = tline.FirstDiscount;
                                    currentLine.GrossTotal = tline.GrossTotal;
                                    currentLine.ItemCode = tline.ItemCode;
                                    currentLine.ItemName = tline.ItemName;
                                    currentLine.ItemOid = tline.ItemOid.ToString();
                                    currentLine.ItemPrice = tline.UnitPrice;
                                    currentLine.NetTotal = tline.NetTotal;
                                    currentLine.NetTotalAfterDiscount = tline.NetTotalAfterDiscount;
                                    currentLine.Qty = tline.Qty;
                                    currentLine.SecondDiscount = tline.SecondDiscount;
                                    currentLine.TotalDiscount = tline.TotalDiscount;
                                    currentLine.TotalVatAmount = tline.TotalVatAmount;
                                    currentLine.UnitPriceAfterDiscount = tline.UnitPriceAfterDiscount;
                                    currentLine.VatAmount = tline.VatAmount;
                                    currentLine.VatFactor = tline.VatFactor;
                                    currentLine.PackingQty = packingQty;
                                    currentLine.EditOffline = false;
                                    if (i > 0)
                                    {
                                        line.RefLines.Add(currentLine);
                                        currentLine.RefDocLine = line;
                                    }
                                    _document.Header.DocLines.Add(currentLine);
                                    i++;
                                }
                            }
                            catch (Exception exept)
                            {
                                
                                int rr = 0;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(line.Barcode);
                    int r=0;
                }
            }
            if (notFound.Count > 0)
            {
                _document.Header.Session.BeginTransaction();
                MessageBox.Show(Resources.Resources.Found+" " + notFound.Count + " "+Resources.Resources.SupplierItemsNotFound);
                foreach (DocLine line in notFound)
                {
                    _document.Header.DocLines.Remove(line);
                }
                _document.Header.Save();
                _document.Header.Session.CommitTransaction();

                if (linesForm != null)
                    linesForm.UpdateData();
                updateCurrentRecord();
            }
            _document.Header.UpdateDocumentHeader();
            UpdateDocumentHeaderDisplay();
        }

        private void updateCurrentRecord()
        {
            if (currentRecord >= _document.Header.DocLines.Count)
                currentRecord = _document.Header.DocLines.Count - 1;
            if (currentRecord >= 0)
            {
                txtRecords.Text = (currentRecord + 1).ToString() + "/" + _document.Header.DocLines.Count.ToString();
                txt_price.Text = _document.Header.DocLines[currentRecord].ItemPrice.ToString("0.00");
                txt_qty.Text = _document.Header.DocLines[currentRecord].Qty.ToString();

                txt_search_item.Text = (_document.Header.DocLines[currentRecord].Barcode == null) ? _document.Header.DocLines[currentRecord].ItemCode : _document.Header.DocLines[currentRecord].Barcode.ToString();
                txt_item_details.Text = _document.Header.DocLines[currentRecord].ItemName;

                txtRecords.Enabled = false;
                txt_price.Enabled = false;
                txt_qty.Text = _document.Header.DocLines[currentRecord].Qty.ToString();
                txt_search_item.Enabled = false;
                txt_item_details.Enabled = false;
                txt_qty.Enabled = (_document.Header.DocLines[currentRecord].RefDocLine == null);
                    
            }
            else
            {
                txtRecords.Text = "*/" + _document.Header.DocLines.Count.ToString();
                txt_price.Text = "";
                txt_qty.Text = "";
                txt_search_item.Text = "";
                txt_item_details.Text = "";

                txt_qty.Enabled = true;
                txtRecords.Enabled = false;
                txt_price.Enabled = false;
                txt_qty.Text = "";
                txt_search_item.Enabled = true;
                txt_item_details.Enabled = false;
                txt_search_item.Focus();
            }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            currentRecord = -1;
            updateCurrentRecord();
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            if (currentRecord == 0)
                return;

            CheckForUpdatesInLine();
            if (currentRecord > 0)
                currentRecord--;
            else if (currentRecord == -1)
                currentRecord = _document.Header.DocLines.Count - 1;
            updateCurrentRecord();
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
            if (currentRecord + 1 >= _document.Header.DocLines.Count)
            {
                return;
            }
            CheckForUpdatesInLine();
            if (currentRecord == -1)
            {
                currentRecord = 0;
            }
            else if (currentRecord + 1 < _document.Header.DocLines.Count)
                currentRecord++;
            updateCurrentRecord();

        }

        private void txt_search_item_GotFocus(object sender, EventArgs e)
        {
            txt_search_item.SelectAll();
        }

        private void txt_qty_GotFocus(object sender, EventArgs e)
        {
            if (txt_search_item.Enabled)
                txt_search_item.Focus();
            else
                btnForward.Focus();
            //txt_qty.SelectAll();
            try
            {
                decimal qty = decimal.Parse(txt_qty.Text);
                KeyboardGateway.OpenNumeric(ref qty, null, false, false, 0, 9999.999m, true, 7, 3, NumKeypad.OPERATOR.FORBID_OPERATORS, qty, Resources.Resources.Quantity);
                txt_qty.Text = qty.ToString();
                CheckForUpdatesInLine();
            }
            catch (Exception ex)
            {
            }

        }

        private void txt_qty_TextChanged(object sender, EventArgs e)
        {
            if (txt_qty.Text.Length > 9)
            {
                txt_qty.Text = txt_qty.Text.Substring(0, 9);
                txt_qty.SelectionStart = 9;
                txt_qty.SelectionLength = 0;
            }
        }

        private void txt_price_GotFocus(object sender, EventArgs e)
        {

        }

        private void button23_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Resources.Resources.WouldYouLikeToDeleteOrder, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                if (MessageBox.Show(Resources.Resources.WouldYouLikeToDeleteOrder+" "+Resources.Resources.ActionCannotBeReverted, AppSettings.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    _document.Header.Session.BeginTransaction();
                    _document.Header.Session.Delete(_document.Header.DocLines);
                    _document.Header.Save();
                    _document.Header.Session.CommitTransaction();
                    _document.Header.UpdateDocumentHeader();
                    UpdateDocumentHeaderDisplay();
                }
            }
            else
                return;
        }

        private void button24_Click(object sender, EventArgs e)
        {
            char c = '\r';
            KeyPressEventArgs ev = new KeyPressEventArgs(c);
            txt_search_item_KeyPress(null, ev);


        }

        private void txt_user_GotFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = true;
        }

        private void txt_user_LostFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
        }

        private void txt_pass_GotFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = true;
        }

        private void txt_pass_LostFocus(object sender, EventArgs e)
        {
            inputPanel1.Enabled = false;
        }


        /// <summary>
        /// Returns the assembly description which includes the build label of this installation.
        /// </summary>
        /// <returns></returns>
        public string GetBuildLabel()
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(false);
            string buildLabel = string.Empty;
            if (attributes != null)
                foreach (object o in attributes)
                {
                    if (o.GetType() == typeof(AssemblyDescriptionAttribute))
                    {
                        AssemblyDescriptionAttribute ada = o as AssemblyDescriptionAttribute;
                        buildLabel = ada.Description;
                    }
                }

            return buildLabel;
        }

        /// <summary>
        /// Returns the version of the executing assembly
        /// </summary>
        /// <returns></returns>
        public string GetVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return string.Format("v{0}.{1}.{2}.{3}", new object[] { version.Major, version.Minor, version.Build, version.Revision });
        }

    }


}
