using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware
{
    public enum PrinterTransactionControl
    {
        /// <summary>
        /// Starts a transaction.
        /// </summary>
        Transaction = 11,
        
        /// <summary>
        /// Ends a transaction by printing the buffered data.
        /// </summary>
        Normal = 12,
    }
}
