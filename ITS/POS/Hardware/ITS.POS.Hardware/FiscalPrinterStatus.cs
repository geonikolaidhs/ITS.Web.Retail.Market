using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware
{
    /// <summary>
    /// Base class for describing the fiscal printer's status flags. Contains common flags for all fiscal printers
    /// </summary>
    public abstract class FiscalPrinterStatus
    {
        /// <summary>
        /// Retruns true if there is no paper left in the printer.
        /// </summary>
        public virtual bool PrinterPaperEnd { get; set; }

        /// <summary>
        /// Returns true if a fiscal day is open.
        /// </summary>
        public virtual bool DayOpen { get; set; }

        /// <summary>
        /// Returns true if a report is being printed currently.
        /// </summary>
        public virtual bool ReportOpen { get; set; }

        /// <summary>
        /// Returns true if the fiscal memory is full.
        /// </summary>
        public virtual bool FiscalFileFull { get; set; }

        /// <summary>
        /// Returns true if there is an open transaction and is in payments mode.
        /// </summary>
        public virtual bool TransactionInPayment { get; set; }

        /// <summary>
        /// Returns true if there is an open transaction.
        /// </summary>
        public virtual bool TransactionOpen { get; set; }
        
        /// <summary>
        /// Returns the serial number of the fiscal printer.
        /// </summary>
        public virtual string SerialNumber { get; set; }
    }
}
