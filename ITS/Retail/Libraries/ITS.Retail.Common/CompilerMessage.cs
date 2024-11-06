using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Common
{
    public class CompilerMessage :MarshalByRefObject
    {
        public String File;
        public String Line;
        public int Column;
        public String IsWarning;
        public String ErrorNumber;
        public String ErrorText;
    }
}
