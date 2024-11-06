using System;
using System.Collections.Generic;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Exceptions;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Base abstract class for all devices.
    /// </summary>
    public abstract class Device
    {

        private String deviceName;
        private int priority;
        private int failureCount;

        public virtual bool ShouldRunOnMainThread
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the device's Priority
        /// </summary>
        public int Priority
        {
            get
            {
                return priority;
            }
            set
            {
                priority = value;
            }
        }

        /// <summary>
        /// Gets or sets the device's failure Count
        /// </summary>
        public int FailureCount
        {
            get
            {
                return failureCount;
            }
            set
            {
                failureCount = value;
            }
        }
        /// <summary>
        /// Gets or sets the device's name.
        /// </summary>
        public String DeviceName
        {
            get
            {
                return deviceName;
            }
            protected set
            {
                deviceName = value;
            }
        }


        private ConnectionType connectionType;
        /// <summary>
        /// Gets the device's Connection Type
        /// </summary>
        public ConnectionType ConType
        {
            get
            {
                return connectionType;
            }
            protected set
            {
                connectionType = value;
            }
        }

        
        /// <summary>
        /// Gets or Sets the device's settings
        /// </summary>
        public Settings Settings { get; set; }

        private NLog.Logger Logger { get; set; }

        /// <summary>
        /// Provides a logger to the device for internal logging.
        /// </summary>
        /// <param name="logger"></param>
        public void SetLogger(NLog.Logger logger)
        {
            this.Logger = logger;
        }

        /// <summary>
        /// Gets the device's logger.
        /// </summary>
        /// <returns></returns>
        protected NLog.Logger GetLogger()
        {
            return this.Logger;
        }

        //protected static string ConvertGreekStringToGreeklish(string text)
        //{
        //    string retval="";
        //    string mappingFrom = "αβγδεζηθικλμνξοπρστυφχψωάέήίόύώϊϋΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΆΈΉΊΌΎΏΪΫ";
        //    string mappingTo = "ABέΈEZHώIKΎMNΏOήPϊTYϋXYΆAEHIOYΆIYABέΈEZHώIKΎMNΏOήPϊTYϋXYΆAEHIOYΆIY";
        //    foreach (char c in text)            
        //    {
        //        int t = mappingFrom.IndexOf(c);
        //        if (t >= 0)
        //            retval += mappingTo.Substring(t, 1);
        //        else
        //            retval += c;
        //    }
        //    return ConvertStringTo737(retval);
        //}

        protected static string ConvertStringTo737(string text)
        {
            string retval = "";

            string mappingFrom = "αβγδεζηθικλμνξοπρστυφχψωάέήίόύώϊϋΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩΆΈΉΊΌΎΏΪΫ";
            string mappingTo = "™› ΅Ά£¤¥¦§¨©«¬­®―ΰαβγεζηιδθ€‚ƒ„…†‡‰‹‘’“”•–—κλμνξοπτυ";
            foreach (char c in text)
            {
                int t = mappingFrom.IndexOf(c);
                if (t >= 0)
                    retval += mappingTo.Substring(t, 1);
                else
                    retval += c;
            }
            return retval;
        }

        public Device()
        {
            deviceName = "";
            connectionType = ConnectionType.NONE;
            Settings = new Settings();
            FailureCount = 0;
        }

        /// <summary>
        /// Override in a device implementation, to do stuff after all devices are loaded.
        /// </summary>
        public virtual void AfterLoad(List<Device> devices)
        {
            //Override to do stuff after all devices are loaded
        }

        /// <summary>
        /// Checks the result code of an opos driver function, and throws an exception if needed.
        /// </summary>
        /// <param name="resultCode"></param>
        protected virtual void handleOposResult(int resultCode,int extendedResultCode = 0)
        {
            OPOSErrorCode errorCode = OPOSErrorCode.Success;
            try
            {
                errorCode = (OPOSErrorCode)resultCode;
            }
            catch
            {
                throw new Exception("Unknown Error code: "+ resultCode);
            }

            if (errorCode != OPOSErrorCode.Success)
            {
                throw new OPOSDriverException(errorCode, extendedResultCode);
            }

        }

        protected void LogTrace(string message)
        {
            if(this.Logger !=null)
            {
                this.Logger.Trace(message);
            }
        }

        protected void LogTraceException(string message, Exception ex)
        {
            if (this.Logger != null)
            {
                this.Logger.Trace(ex, message);
                //this.Logger.TraceException(message,ex);
            }
        }

        protected void LogDebug(string message)
        {
            if (this.Logger != null)
            {
                this.Logger.Debug(message);
            }
        }

        //protected void LogDebugException(string message, Exception ex)
        //{
        //    if (this.Logger != null)
        //    {
        //        this.Logger.Debug(ex, message);
        //        //this.Logger.DebugException(message, ex);
        //    }
        //}

        protected void LogInfo(string message)
        {
            if (this.Logger != null)
            {
                this.Logger.Info(message);
            }
        }

        protected void LogInfoException(string message, Exception ex)
        {
            if (this.Logger != null)
            {
                this.Logger.Info(ex, message);
                //this.Logger.InfoException(message, ex);
            }
        }

        protected void LogWarn(string message)
        {
            if (this.Logger != null)
            {
                this.Logger.Warn(message);
            }
        }

        protected void LogWarnException(string message, Exception ex)
        {
            if (this.Logger != null)
            {
                this.Logger.Warn(ex, message);
                //this.Logger.WarnException(message, ex);
            }
        }

        protected void LogError(string message)
        {
            if (this.Logger != null)
            {
                this.Logger.Error(message);
            }
        }

        protected void LogErrorException(string message, Exception ex)
        {
            if (this.Logger != null)
            {
                this.Logger.Error(ex, message);
                //this.Logger.ErrorException(message, ex);
            }
        }

        /// <summary>
        /// Override in a device implementation to provide device checking at application startup.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract eDeviceCheckResult CheckDevice(out string message);

        Encoding fromEncoding = null;
        Encoding toEncoding = null;

        private Encoding FromEncoding
        {
            get
            {
                if(fromEncoding == null)
                {
                    fromEncoding = Encoding.GetEncoding(this.Settings.ConvertCharsetFrom);
                }
                return fromEncoding;
            }
        }


        private Encoding ToEncoding
        {
            get
            {
                if (toEncoding == null)
                {
                    toEncoding = Encoding.GetEncoding(this.Settings.ConvertCharsetTo);
                }
                return toEncoding;                
            }
        }


        protected string ConvertString(string input)
        {
            if(this.Settings.ConvertCharset == false)
            {
                return ConvertStringTo737(input);//return input;
            }
            try
            {                
                return ToEncoding.GetString(FromEncoding.GetBytes(input));
            }
            catch (Exception ex)
            {
                this.LogInfoException("Conversion Failure skipping conversion ", ex);
                return input;
            }
        }
        
    }
}
