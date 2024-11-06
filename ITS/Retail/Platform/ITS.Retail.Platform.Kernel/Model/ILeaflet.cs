using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel.Model
{
    public  interface ILeaflet : ILookup2Fields
    {
         DateTime EndTime { get;  }
         DateTime StartTime  { get;  }
         DateTime EndDate { get;  }
         DateTime StartDate { get;  }
         string ImageDescription { get;  }
         string ImageInfo { get;  }
    }
}
