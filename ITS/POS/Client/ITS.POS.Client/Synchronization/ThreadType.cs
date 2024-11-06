using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Synchronization
{
    public enum ThreadType
    {
        GET_UPDATES,
        POST_TRANSACTIONS,
        PUBLISH_STATUS,
        UPDATE_STATUS,
        AUTO_FOCUS,
        POST_VERSIONS
    }
}
