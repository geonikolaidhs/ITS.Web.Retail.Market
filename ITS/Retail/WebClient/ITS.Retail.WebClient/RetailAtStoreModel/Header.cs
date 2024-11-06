using ITS.Retail.Mobile.AuxilliaryClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ITS.Retail.WebClient.RetailAtStoreModel
{
    public enum DOC_STATUS : byte
    {
        OPEN = 0,
        CLOSED = 1,
        EXPORTED = 2
    }

    public class Header
    {

        public Guid Oid { get; set; }
        public DateTime UpdatedOn { get; set; }
        public DateTime CreatedOn { get; set; }


        public Header()
        {
            this.Details = new List<Line>();
        }
        /// <summary>
        /// Gets or sets the Terminal ID
        /// </summary>
        public int TerminalID
        {
            get
            {
                return this._terminalID;
            }
            set
            {
                this.SetPropertyValue("TerminalID", ref this._terminalID, value);
            }
        }



        /// <summary>
        /// Gets or sets the document type
        /// </summary>
        public eDocumentType DocType
        {
            get
            {
                return this._docType;
            }
            set
            {
                this.SetPropertyValue("DocType", ref this._docType, value);
            }
        }

        /// <summary>
        /// Gets or sets the document status
        /// </summary>
        public DOC_STATUS DocStatus
        {
            get
            {
                return this._docStatus;
            }
            set
            {
                this.SetPropertyValue("DocStatus", ref this._docStatus, value);
            }
        }

        /// <summary>
        /// Gets or sets the date set by the user for this document.
        /// </summary>
        public DateTime DocDate
        {
            get
            {
                return this._docDate;
            }
            set
            {
                this.SetPropertyValue("DocDate", ref this._docDate, value);
            }
        }

        /// <summary>
        /// Gets or sets if should append or not
        /// </summary>
        public bool Append
        {
            get
            {
                return this._append;
            }
            set
            {
                this.SetPropertyValue("Append", ref this._append, value);
            }
        }

        /// <summary>
        /// Gets or sets the document number
        /// </summary>
        public int DocNumber
        {
            get
            {
                return this._docNumber;
            }
            set
            {
                this.SetPropertyValue("DocNumber", ref this._docNumber, value);
            }
        }

        /// <summary>
        /// Gets or sets the code of the document
        /// </summary>
        public string Code
        {
            get
            {
                return this._code;
            }
            set
            {
                this.SetPropertyValue("Code", ref this._code, value);
            }
        }

        /// <summary>
        /// Gets or sets the customer code
        /// </summary>
        public string CustomerCode
        {
            get
            {
                return this._customerCode;
            }
            set
            {
                this.SetPropertyValue("CustomerCode", ref this._customerCode, value);
            }
        }

        /// <summary>
        /// Gets or sets the customer name
        /// </summary>
        public string CustomerName
        {
            get
            {
                return this._CustomerName;
            }
            set
            {
                this.SetPropertyValue("CustomerName", ref this._CustomerName, value);
            }
        }

        /// <summary>
        /// Gets or sets the customer tax number
        /// </summary>
        public string CustomerAFM
        {
            get
            {
                return this._CustomerAFM;
            }
            set
            {
                this.SetPropertyValue("CustomerAFM", ref this._CustomerAFM, value);
            }
        }

        /// <summary>
        /// Gets or sets if this document is offline or not
        /// </summary>
        public bool ForcedOffline
        {
            get
            {
                return this._ForcedOffline;
            }
            set
            {
                this.SetPropertyValue("ForcedOffline", ref this._ForcedOffline, value);
            }
        }

        /// <summary>
        /// Gets the document lines
        /// </summary>
        public List<Line> Details { get; set; }


        /// <summary>
        /// Gets or sets the outputPath Name  that will be used to export this Header. This corresponds to the one entered in the Settings.xml
        /// file OutputPath tag attribute Name
        /// </summary>
        public int OutputId
        {
            get
            {
                return this._outputId;
            }
            set
            {
                this.SetPropertyValue("OutputId", ref this._outputId, value);
            }
        }


        /// <summary>
       

        private DOC_STATUS _docStatus = DOC_STATUS.OPEN;
        private eDocumentType _docType;
        private int _terminalID = 0;
        private DateTime _docDate = DateTime.Now;
        private bool _append = false;
        private int _docNumber;
        private string _code;
        private string _customerCode;
        private string _CustomerName;
        private string _CustomerAFM;
        private bool _ForcedOffline;
        private int _outputId = -1;

        private void SetPropertyValue<T>(string p1, ref T p2, T value)
        {
            p2 = value;
        }
    }
}