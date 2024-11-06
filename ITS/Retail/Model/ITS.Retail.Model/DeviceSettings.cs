using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Model.Attributes;

namespace ITS.Retail.Model
{
    //[Updater(Order = 80,
    //    Permissions = eUpdateDirection.STORECONTROLLER_TO_POS |eUpdateDirection.STORECONTROLLER_TO_MASTER)]
    public class DeviceSettings : BaseObj
    {
        public DeviceSettings()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public DeviceSettings(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
            CharacterSet = 1253;
            NewLine = "\\n";
        }


        // Fields...
        private int _NumberOfLines;
        private int _LineChars;
        private string _NewLine;
        private int _CharacterSet;
        private DeviceType _DeviceType;
        private bool _ConvertCharset;
        private int _ConvertCharsetFrom;
        private int _ConvertCharsetTo;
        private int _CommandChars = 30;//the smaller value for supported phiscal printers

        /// <summary>
        /// Gets or Sets the device's character set used.
        /// </summary>
        public int CharacterSet
        {
            get
            {
                return _CharacterSet;
            }
            set
            {
                SetPropertyValue("CharacterSet", ref _CharacterSet, value);
            }
        }

        /// <summary>
        /// Gets or Sets the value used to interpret the end of a call to write and read internal functions. Default is "\n".
        /// </summary>
        public string NewLine
        {
            get
            {
                return _NewLine;
            }
            set
            {
                SetPropertyValue("NewLine", ref _NewLine, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of characters per line. Used for Print and Display Devices.
        /// </summary>
        public int LineChars
        {
            get
            {
                return _LineChars;
            }
            set
            {
                SetPropertyValue("LineChars", ref _LineChars, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of lines. Used for Display Devices.
        /// </summary>
        public int NumberOfLines
        {
            get
            {
                return _NumberOfLines;
            }
            set
            {
                SetPropertyValue("NumberOfLines", ref _NumberOfLines, value);
            }
        }

        [DescriptionField]
        public DeviceType DeviceType
        {
            get
            {
                return _DeviceType;
            }
            set
            {
                SetPropertyValue("DeviceType", ref _DeviceType, value);
            }
        }

        public bool ConvertCharset
        {
            get
            {
                return _ConvertCharset;
            }
            set
            {
                SetPropertyValue("ConvertCharset", ref _ConvertCharset, value);
            }
        }

        public int ConvertCharsetFrom
        {
            get
            {
                return _ConvertCharsetFrom;
            }
            set
            {
                SetPropertyValue("ConvertCharsetFrom", ref _ConvertCharsetFrom, value);
            }
        }

        public int ConvertCharsetTo
        {
            get
            {
                return _ConvertCharsetTo;
            }
            set
            {
                SetPropertyValue("ConvertCharsetTo", ref _ConvertCharsetTo, value);
            }
        }
        public int CommandChars
        {
            get
            {
                return _CommandChars;
            }
            set
            {
                SetPropertyValue("CommandChars", ref _CommandChars, value);
            }
        }
    }
}
