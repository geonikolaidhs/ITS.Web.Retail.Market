using System;
using System.Collections.Generic;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

namespace ITS.MobileAtStore.ObjectModel
{
    //[Persistent("ITS_DATALOGGER_HEADER")]
    //[Persistent("ORDER_MOB_HDR")]
    public class Header : BaseXPObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Header"/> class. 
        /// </summary>
        public Header(): base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Header"/> class. 
        /// </summary>
        /// <param name="session">The Session</param>
        public Header(Session session) : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>
        /// This Callback is triggered when the object is created
        /// </summary>
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            // Place here your initialization code.
            this._lines = new List<Line>();            
        }

        /// <summary>
        /// Returns the current line appropriately formatted
        /// </summary>
        /// <param name="format">The format</param>
        /// <returns>The formatted object</returns>
        public string ToString(String format)
        {
            string code = this.Code ?? string.Empty;
            string customerCode = this.CustomerCode ?? string.Empty;
            string customerAfm = this.CustomerAFM ?? string.Empty;
            string customerName = this.CustomerName ?? string.Empty;
            return string.Format(
                new PaddedStringFormatInfo(),
                format,
                code,
                customerCode,
                customerAfm,
                customerName,
                this.DocDate,
                this.DocNumber,
                this.DocStatus,
                this.DocType,
                this.TerminalID,
                this.LineCount);
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
        public DOC_TYPES DocType
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
        /// Gets or sets the document lines
        /// </summary>
        public List<Line> Lines
        {
            get
            {
                return this._lines ?? this.Details;
            }
            set
            {
                this._lines = value;
            }
        }

        /// <summary>
        /// Gets the document lines
        /// </summary>
        public List<Line> Details
        {
            get
            {
                List<Line> details = new List<Line>();//null;
                using (XPCollection<Line> results = new XPCollection<Line>(this.Session, new BinaryOperator("Header", this.Oid)))
                {
                    if(results!=null)
                    {
                        foreach(Line line in results)
                        {
                            details.Add(line);
                        }
                    }
                }
                return details;
            }
        }

        
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
        /// Deletes the header and its lines from the database
        /// </summary>
        /// <param name="headerOid"></param>
        /// <param name="dl"></param>
        public static void CascadeDelete(Guid headerOid, UnitOfWork uow)
        {
            Header header = uow.GetObjectByKey<Header>(headerOid);
            if (header != null)
            {
                XPCollection<Line> col = new XPCollection<Line>(uow, new BinaryOperator("Header", header.Oid) );
                col.DeleteObjectOnRemove = true;
                uow.Delete(col);
                header.Delete();
            }
            uow.CommitChanges();
        }

        /// <summary>
        /// Deletes the header and its lines from the database
        /// </summary>
        /// <param name="headerOid"></param>
        /// <param name="dl"></param>
        public static void DeleteInvLnes(UnitOfWork uow)
        {
            XPCollection<InvLine> col = new XPCollection<InvLine>(uow, new BinaryOperator("Export",3));
            col.DeleteObjectOnRemove = true;
            uow.Delete(col);

            uow.CommitChanges();
        }

        /// <summary>
        /// Counts the Lines in this Header
        /// </summary>
        /// <returns></returns>
        public int CountLines()
        {
            return (int)Session.Evaluate(typeof(Line), CriteriaOperator.Parse("Count()"),
                CriteriaOperator.And(new BinaryOperator("Header", Oid), 
                CriteriaOperator.Or(new NullOperator("LinkedLine"), new BinaryOperator("LinkedLine",Guid.Empty))));
        }

        /// <summary>
        /// Gets or sets the line count. 
        /// </summary>
        [PersistentAlias("_LineCount")]
        public int LineCount
        {
            get
            {
                _LineCount = Lines == null ? 0 : Lines.Count;
                return _LineCount;
            }
        }

        [Persistent("LineCount")]
        private int _LineCount;

        private DOC_STATUS _docStatus = DOC_STATUS.OPEN;
        private DOC_TYPES _docType;
        private int _terminalID = 0;
        private DateTime _docDate = DateTime.Now;
        private bool _append = false; 
        private int _docNumber; 
        private string _code; 
        private string _customerCode; 
        private string _CustomerName;
        private string _CustomerAFM;
        private bool _ForcedOffline;
        private List<Line> _lines;
        private int _outputId = -1;
    }
}
