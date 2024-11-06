using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Enumerations
{
    [Flags]
    public  enum eFiscalMethod
    {
        UNKNOWN = 1,
        /// <summary>
        /// Φορολογικός Εκτυπωτής
        /// </summary>
        ADHME = 2,
        /// <summary>
        /// Φορολογικός Μηχανισμός
        /// </summary>
        EAFDSS = 4
    }
}
