using ITS.Retail.PrintServer.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace ITS.Retail.PrintServer.Common
{
    public class PrintServerSettings : INotifyPropertyChanged
    {
        private int _Port;

        public event PropertyChangedEventHandler PropertyChanged;

        public PrintServerSettings()
        {
            this.PrinterPOSAssociations = new BindingList<PrinterPOSAssociation>();
            this.Printers = new BindingList<PrinterInfo>();
        }
        ///// <summary>
        ///// Gets or sets the DefaultPrinter name
        ///// </summary>
        ////public string DefaultPrinter { get; set; }

        /// <summary>
        /// Gets the DefaultPrinter name
        /// </summary>
        //public string DefaultPrinter { get; set; }
        public string DefaultPrinter
        {
            get
            {
                return this.Printers.First(printer => printer.IsDefault).Name;
            }
        }
        /// <summary>
        /// Gets or sets the associatons between POS'es and Printers
        /// </summary>
        public BindingList<PrinterPOSAssociation> PrinterPOSAssociations
        {
            get
            {
                return _PrinterPOSAssociations;
            }
            set
            {
                SetPropertyValue("PrinterPOSAssociations", ref _PrinterPOSAssociations, value);
            }
        }

        /// <summary>
        /// Gets or sets the URL of StoreController
        /// </summary>
        public string StoreControllerURL
        {
            get
            {
                return _StoreControllerURL;
            }
            set
            {
                SetPropertyValue("StoreControllerURL", ref _StoreControllerURL, value);
            }
        }

        /// <summary>
        /// Gets or sets the Print Server's Listening port
        /// </summary>
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                SetPropertyValue("Port", ref _Port, value);
            }
        }        


        /// <summary>
        /// Helping Function to assign property values and triggering the required events
        /// </summary>
        /// <typeparam name="T">The type of the property</typeparam>
        /// <param name="propertyName">The name of the Property to assign</param>
        /// <param name="localField">The local field that stores the property's value</param>
        /// <param name="newValue">The property's new value</param>
        protected void SetPropertyValue<T>(string propertyName, ref T localField, T newValue)
        {
            localField = newValue;
            Notify(propertyName);
        }

        private void Notify(string propertyName)
        {            
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); 
            }
        }

        //Fields
        private BindingList<PrinterInfo> _Printers;
        private BindingList<PrinterPOSAssociation> _PrinterPOSAssociations;
        private string _StoreControllerURL;

        public BindingList<PrinterInfo> Printers
        {
            get
            {
                return _Printers;
            }
            set
            {
                SetPropertyValue("Printers", ref _Printers, value);
            }
        }
    }
}
