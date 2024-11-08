﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.17929.
// 
#pragma warning disable 1591

namespace ITS.Licensing.WebLibrary.LicenseWebService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="LicenceWebServiceSoap", Namespace="http://tempuri.org/")]
    public partial class LicenceWebService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback CheckOnlineStatusOperationCompleted;
        
        private System.Threading.SendOrPostCallback CheckValidLicenceOperationCompleted;
        
        private System.Threading.SendOrPostCallback NumberOfUsersLicenceOperationCompleted;
        
        private System.Threading.SendOrPostCallback ActivateLicenseOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetSettingsXmlOperationCompleted;
        
        private System.Threading.SendOrPostCallback PostApplicationDataOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetVersionOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public LicenceWebService() {
            this.Url = global::ITS.Licensing.WebLibrary.Properties.Settings.Default.ITS_Licensing_WebLibrary_LicenseWebService_LicenceWebService;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event CheckOnlineStatusCompletedEventHandler CheckOnlineStatusCompleted;
        
        /// <remarks/>
        public event CheckValidLicenceCompletedEventHandler CheckValidLicenceCompleted;
        
        /// <remarks/>
        public event NumberOfUsersLicenceCompletedEventHandler NumberOfUsersLicenceCompleted;
        
        /// <remarks/>
        public event ActivateLicenseCompletedEventHandler ActivateLicenseCompleted;
        
        /// <remarks/>
        public event GetSettingsXmlCompletedEventHandler GetSettingsXmlCompleted;
        
        /// <remarks/>
        public event PostApplicationDataCompletedEventHandler PostApplicationDataCompleted;
        
        /// <remarks/>
        public event GetVersionCompletedEventHandler GetVersionCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CheckOnlineStatus", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool CheckOnlineStatus() {
            object[] results = this.Invoke("CheckOnlineStatus", new object[0]);
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void CheckOnlineStatusAsync() {
            this.CheckOnlineStatusAsync(null);
        }
        
        /// <remarks/>
        public void CheckOnlineStatusAsync(object userState) {
            if ((this.CheckOnlineStatusOperationCompleted == null)) {
                this.CheckOnlineStatusOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCheckOnlineStatusOperationCompleted);
            }
            this.InvokeAsync("CheckOnlineStatus", new object[0], this.CheckOnlineStatusOperationCompleted, userState);
        }
        
        private void OnCheckOnlineStatusOperationCompleted(object arg) {
            if ((this.CheckOnlineStatusCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CheckOnlineStatusCompleted(this, new CheckOnlineStatusCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/CheckValidLicence", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ValidationStatus CheckValidLicence(System.Guid ApplicationID, string serialNumber, string MachineID, string activationKey, System.DateTime ApplicationBuild, System.DateTime beginDate, System.DateTime endDate) {
            object[] results = this.Invoke("CheckValidLicence", new object[] {
                        ApplicationID,
                        serialNumber,
                        MachineID,
                        activationKey,
                        ApplicationBuild,
                        beginDate,
                        endDate});
            return ((ValidationStatus)(results[0]));
        }
        
        /// <remarks/>
        public void CheckValidLicenceAsync(System.Guid ApplicationID, string serialNumber, string MachineID, string activationKey, System.DateTime ApplicationBuild, System.DateTime beginDate, System.DateTime endDate) {
            this.CheckValidLicenceAsync(ApplicationID, serialNumber, MachineID, activationKey, ApplicationBuild, beginDate, endDate, null);
        }
        
        /// <remarks/>
        public void CheckValidLicenceAsync(System.Guid ApplicationID, string serialNumber, string MachineID, string activationKey, System.DateTime ApplicationBuild, System.DateTime beginDate, System.DateTime endDate, object userState) {
            if ((this.CheckValidLicenceOperationCompleted == null)) {
                this.CheckValidLicenceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnCheckValidLicenceOperationCompleted);
            }
            this.InvokeAsync("CheckValidLicence", new object[] {
                        ApplicationID,
                        serialNumber,
                        MachineID,
                        activationKey,
                        ApplicationBuild,
                        beginDate,
                        endDate}, this.CheckValidLicenceOperationCompleted, userState);
        }
        
        private void OnCheckValidLicenceOperationCompleted(object arg) {
            if ((this.CheckValidLicenceCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.CheckValidLicenceCompleted(this, new CheckValidLicenceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/NumberOfUsersLicence", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public int NumberOfUsersLicence(System.Guid ApplicationID, string serialNumber, string MachineID, string activationKey, System.DateTime ApplicationBuild, System.DateTime beginDate, System.DateTime endDate) {
            object[] results = this.Invoke("NumberOfUsersLicence", new object[] {
                        ApplicationID,
                        serialNumber,
                        MachineID,
                        activationKey,
                        ApplicationBuild,
                        beginDate,
                        endDate});
            return ((int)(results[0]));
        }
        
        /// <remarks/>
        public void NumberOfUsersLicenceAsync(System.Guid ApplicationID, string serialNumber, string MachineID, string activationKey, System.DateTime ApplicationBuild, System.DateTime beginDate, System.DateTime endDate) {
            this.NumberOfUsersLicenceAsync(ApplicationID, serialNumber, MachineID, activationKey, ApplicationBuild, beginDate, endDate, null);
        }
        
        /// <remarks/>
        public void NumberOfUsersLicenceAsync(System.Guid ApplicationID, string serialNumber, string MachineID, string activationKey, System.DateTime ApplicationBuild, System.DateTime beginDate, System.DateTime endDate, object userState) {
            if ((this.NumberOfUsersLicenceOperationCompleted == null)) {
                this.NumberOfUsersLicenceOperationCompleted = new System.Threading.SendOrPostCallback(this.OnNumberOfUsersLicenceOperationCompleted);
            }
            this.InvokeAsync("NumberOfUsersLicence", new object[] {
                        ApplicationID,
                        serialNumber,
                        MachineID,
                        activationKey,
                        ApplicationBuild,
                        beginDate,
                        endDate}, this.NumberOfUsersLicenceOperationCompleted, userState);
        }
        
        private void OnNumberOfUsersLicenceOperationCompleted(object arg) {
            if ((this.NumberOfUsersLicenceCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.NumberOfUsersLicenceCompleted(this, new NumberOfUsersLicenceCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/ActivateLicense", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public ValidationStatus ActivateLicense(System.Guid ApplicationID, string serialNumber, string MachineID, System.DateTime ApplicationBuild, out System.DateTime beginDate, out System.DateTime endDate, out string ActivationKey) {
            object[] results = this.Invoke("ActivateLicense", new object[] {
                        ApplicationID,
                        serialNumber,
                        MachineID,
                        ApplicationBuild});
            beginDate = ((System.DateTime)(results[1]));
            endDate = ((System.DateTime)(results[2]));
            ActivationKey = ((string)(results[3]));
            return ((ValidationStatus)(results[0]));
        }
        
        /// <remarks/>
        public void ActivateLicenseAsync(System.Guid ApplicationID, string serialNumber, string MachineID, System.DateTime ApplicationBuild) {
            this.ActivateLicenseAsync(ApplicationID, serialNumber, MachineID, ApplicationBuild, null);
        }
        
        /// <remarks/>
        public void ActivateLicenseAsync(System.Guid ApplicationID, string serialNumber, string MachineID, System.DateTime ApplicationBuild, object userState) {
            if ((this.ActivateLicenseOperationCompleted == null)) {
                this.ActivateLicenseOperationCompleted = new System.Threading.SendOrPostCallback(this.OnActivateLicenseOperationCompleted);
            }
            this.InvokeAsync("ActivateLicense", new object[] {
                        ApplicationID,
                        serialNumber,
                        MachineID,
                        ApplicationBuild}, this.ActivateLicenseOperationCompleted, userState);
        }
        
        private void OnActivateLicenseOperationCompleted(object arg) {
            if ((this.ActivateLicenseCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.ActivateLicenseCompleted(this, new ActivateLicenseCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetSettingsXml", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetSettingsXml(System.Guid appID, string serialnumber, string MachineID, string activationkey, System.DateTime applBuild, System.DateTime beginDate, System.DateTime endDate, out bool success) {
            object[] results = this.Invoke("GetSettingsXml", new object[] {
                        appID,
                        serialnumber,
                        MachineID,
                        activationkey,
                        applBuild,
                        beginDate,
                        endDate});
            success = ((bool)(results[1]));
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetSettingsXmlAsync(System.Guid appID, string serialnumber, string MachineID, string activationkey, System.DateTime applBuild, System.DateTime beginDate, System.DateTime endDate) {
            this.GetSettingsXmlAsync(appID, serialnumber, MachineID, activationkey, applBuild, beginDate, endDate, null);
        }
        
        /// <remarks/>
        public void GetSettingsXmlAsync(System.Guid appID, string serialnumber, string MachineID, string activationkey, System.DateTime applBuild, System.DateTime beginDate, System.DateTime endDate, object userState) {
            if ((this.GetSettingsXmlOperationCompleted == null)) {
                this.GetSettingsXmlOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetSettingsXmlOperationCompleted);
            }
            this.InvokeAsync("GetSettingsXml", new object[] {
                        appID,
                        serialnumber,
                        MachineID,
                        activationkey,
                        applBuild,
                        beginDate,
                        endDate}, this.GetSettingsXmlOperationCompleted, userState);
        }
        
        private void OnGetSettingsXmlOperationCompleted(object arg) {
            if ((this.GetSettingsXmlCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetSettingsXmlCompleted(this, new GetSettingsXmlCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/PostApplicationData", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public bool PostApplicationData([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] ZipedXml) {
            object[] results = this.Invoke("PostApplicationData", new object[] {
                        ZipedXml});
            return ((bool)(results[0]));
        }
        
        /// <remarks/>
        public void PostApplicationDataAsync(byte[] ZipedXml) {
            this.PostApplicationDataAsync(ZipedXml, null);
        }
        
        /// <remarks/>
        public void PostApplicationDataAsync(byte[] ZipedXml, object userState) {
            if ((this.PostApplicationDataOperationCompleted == null)) {
                this.PostApplicationDataOperationCompleted = new System.Threading.SendOrPostCallback(this.OnPostApplicationDataOperationCompleted);
            }
            this.InvokeAsync("PostApplicationData", new object[] {
                        ZipedXml}, this.PostApplicationDataOperationCompleted, userState);
        }
        
        private void OnPostApplicationDataOperationCompleted(object arg) {
            if ((this.PostApplicationDataCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.PostApplicationDataCompleted(this, new PostApplicationDataCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetVersion", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public long GetVersion(string serialNumber) {
            object[] results = this.Invoke("GetVersion", new object[] {
                        serialNumber});
            return ((long)(results[0]));
        }
        
        /// <remarks/>
        public void GetVersionAsync(string serialNumber) {
            this.GetVersionAsync(serialNumber, null);
        }
        
        /// <remarks/>
        public void GetVersionAsync(string serialNumber, object userState) {
            if ((this.GetVersionOperationCompleted == null)) {
                this.GetVersionOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetVersionOperationCompleted);
            }
            this.InvokeAsync("GetVersion", new object[] {
                        serialNumber}, this.GetVersionOperationCompleted, userState);
        }
        
        private void OnGetVersionOperationCompleted(object arg) {
            if ((this.GetVersionCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetVersionCompleted(this, new GetVersionCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.17929")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://tempuri.org/")]
    public enum ValidationStatus {
        
        /// <remarks/>
        LICENSE_VALID,
        
        /// <remarks/>
        LICENSE_VERSION_INVALID,
        
        /// <remarks/>
        LICENSE_VALID_UPDATES_EXPIRED,
        
        /// <remarks/>
        LICENSE_INVALID,
        
        /// <remarks/>
        LICENSE_CHANGED,
        
        /// <remarks/>
        LICENSE_MAXIMUM_REACHED,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void CheckOnlineStatusCompletedEventHandler(object sender, CheckOnlineStatusCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CheckOnlineStatusCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CheckOnlineStatusCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void CheckValidLicenceCompletedEventHandler(object sender, CheckValidLicenceCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CheckValidLicenceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal CheckValidLicenceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ValidationStatus Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ValidationStatus)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void NumberOfUsersLicenceCompletedEventHandler(object sender, NumberOfUsersLicenceCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class NumberOfUsersLicenceCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal NumberOfUsersLicenceCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public int Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((int)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void ActivateLicenseCompletedEventHandler(object sender, ActivateLicenseCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class ActivateLicenseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal ActivateLicenseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public ValidationStatus Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((ValidationStatus)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public System.DateTime beginDate {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.DateTime)(this.results[1]));
            }
        }
        
        /// <remarks/>
        public System.DateTime endDate {
            get {
                this.RaiseExceptionIfNecessary();
                return ((System.DateTime)(this.results[2]));
            }
        }
        
        /// <remarks/>
        public string ActivationKey {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[3]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetSettingsXmlCompletedEventHandler(object sender, GetSettingsXmlCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetSettingsXmlCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetSettingsXmlCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
        
        /// <remarks/>
        public bool success {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[1]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void PostApplicationDataCompletedEventHandler(object sender, PostApplicationDataCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PostApplicationDataCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal PostApplicationDataCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public bool Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetVersionCompletedEventHandler(object sender, GetVersionCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetVersionCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetVersionCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public long Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((long)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591