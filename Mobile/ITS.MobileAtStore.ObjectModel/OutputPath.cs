using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.ObjectModel
{
    /// <summary>
    /// Serves as a container for a list of paths
    /// </summary>
    public class OutputPath
    {
        private string _outputPathName;
        /// <summary>
        /// The name of this output path that will be displayed to the user
        /// </summary>
        public string OutputPathName
        {
            get
            {
                return _outputPathName;
            }
            set
            {
                _outputPathName = value;
            }
        }

        private string _folderPath;
        /// <summary>
        /// The folder path that we will be used as an output path
        /// </summary>
        public string FolderPath
        {
            get
            {
                return _folderPath;
            }
            set
            {
                _folderPath = value;
            }
        }

        private bool _default;
        /// <summary>
        /// Indicates whether or not this is the default output path
        /// </summary>
        public bool Default
        {
            get
            {
                return _default;
            }
            set
            {
                _default = value;
            }
        }

        private string _DocTypes;
        /// <summary>
        /// Comma delimited DocType numbers
        /// </summary>
        public string DocTypes
        {
            get
            {
                return _DocTypes;
            }
            set
            {
                _DocTypes = value;
            }
        }
        public override string  ToString()
        {
            return OutputPathName;
        }
    }
}
