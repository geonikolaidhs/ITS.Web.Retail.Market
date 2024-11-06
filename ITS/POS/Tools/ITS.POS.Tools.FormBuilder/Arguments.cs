using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace ITS.POS.Tools.FormBuilder
{

    /// <summary>
    /// Arguments class
    /// </summary>
    public class Arguments
    {
        // Variables
        private StringDictionary Parameters;

        public Arguments(string[] Args)
        {
            Parameters = new StringDictionary();
            foreach (string arg in Args)
            {
                // Apply validDelims to current arg to see how many significant characters were present
                // We're limiting to 3 to forcefully ignore any characters in the parameter VALUE
                // that would normally be used as a delimiter
                string[] parts = arg.Split('=');
                //string currentParam = null;

                switch (parts.Length)
                {
                    //case flag parameter:  text.exe -myparam
                    case 1:
                        Parameters.Add(parts[0].TrimStart('-'), "true");
                        break;
                    //case parameter with value:  text.exe -mypath="c:\test"
                    case 2:
                        Parameters.Add(parts[0].TrimStart('-'), parts[1]);
                        break;
                }
            }
        }

        public string this[string Param]
        {
            get
            {
                return (Parameters[Param]);
            }
        }
    }
}
