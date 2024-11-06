using System;
using System.Collections.Generic;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Resources;

namespace ITS.POS.Hardware.Common
{
    /// <summary>
    /// Represents a dataSign EAFDSS device.
    /// </summary>
    public class DataSignESD : Device
    {
        /// <summary>
        /// Only ConnectionType.Ethernet and Emulated is supported.
        /// </summary>
        /// <param name="conType"></param>
        public DataSignESD(ConnectionType conType, string deviceName)
        {
            ConType = conType;
            DeviceName = deviceName;
        }

        public enum DataSignResult
        {
            ERR_SUCCESS,
            ERR_BADARGUMENT,
            ERR_FILEIO_ERROR,
            ERR_COMMUNICATION_ERROR,
            ERR_UNRECOVERABLE_ERROR,
            ERR_UNEXPECTED_ERROR,
            ERR_INVALIDDEVICEID,
            ERR_INVALIDBASEDIR,
            ERR_DEVFILELOAD_ERR,
            ERR_NOTSELECTED,
            ERR_INVALIDINPUTFILE,
            INVALID_PROPERTY,
            UNSUPPORTED_CONNECTION_TYPE
        }

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            message = "NOT CHECKED";
            return eDeviceCheckResult.INFO;
        }

        /// <summary>
        /// Signs a document and returns the exit code.
        /// </summary>
        /// <param name="ABCDirectory">The ABC Directory</param>
        /// <param name="fileToSign">The path to the file to sign</param>
        /// <param name="signature">The electronic signature</param>
        /// <returns>Result:
        ///0       ERR_SUCCESS                     No errors, success
        ///1       ERR_BADARGUMENT                 Bad parameter specified
        ///2       ERR_FILEIO_ERROR                Error in file/filesystem operation
        ///3       ERR_COMMUNICATION_ERROR         Communication with device failed
        ///4       ERR_UNRECOVERABLE_ERROR         Hardware error: device needs service
        ///5       ERR_UNEXPECTED_ERROR            Unexpected error, operation aborted
        ///6       ERR_INVALIDDEVICEID             Serial number is not the one expected
        ///7       ERR_INVALIDBASEDIR              Base directory specified is invalid
        ///8       ERR_DEVFILELOAD_ERR             Failed loading device descriptor file
        ///9       ERR_NOTSELECTED                 Device specified is not selected
        ///10      ERR_INVALIDINPUTFILE            File contains invalid characters
        ///11      INVALID_PROPERTY                A Required connection propery was null or invalid.For Ethernet Connection the "IPAddress" property is required.
        ///12      UNSUPPORTED_CONNECTION_TYPE     Only ETHERNET connection type is supported
        /// </returns>
        public DataSignResult SignDocument(string ABCDirectory, string fileToSign, ref string signature)
        {
            switch (ConType)
            {
                case ConnectionType.ETHERNET:
                    rapidsign.rapidsign signHelper = new rapidsign.rapidsign();
                    if (String.IsNullOrEmpty(Settings.Ethernet.IPAddress))
                    {
                        return DataSignResult.INVALID_PROPERTY;
                    }
                    string deviceLocation = Settings.Ethernet.IPAddress;
                    //if (String.IsNullOrWhiteSpace(deviceLocation))
                    //{
                    //    return DataSignResult.UNSUPPORTED_CONNECTION_TYPE;
                    //}
                    return (DataSignResult)signHelper.FSL_SignDocument(ABCDirectory, deviceLocation, fileToSign, ref signature);
                case ConnectionType.EMULATED:
                    signature = "====================";
                    return DataSignResult.ERR_SUCCESS;
            }
            return DataSignResult.UNSUPPORTED_CONNECTION_TYPE;
        }

        /// <summary>
        /// Issues a 'Z' report.
        /// </summary>
        /// <param name="ABCDirectory">The ABC Directory</param>
        /// <returns>Result:
        ///0       ERR_SUCCESS                     No errors, success
        ///1       ERR_BADARGUMENT                 Bad parameter specified
        ///2       ERR_FILEIO_ERROR                Error in file/filesystem operation
        ///3       ERR_COMMUNICATION_ERROR         Communication with device failed
        ///4       ERR_UNRECOVERABLE_ERROR         Hardware error: device needs service
        ///5       ERR_UNEXPECTED_ERROR            Unexpected error, operation aborted
        ///6       ERR_INVALIDDEVICEID             Serial number is not the one expected
        ///7       ERR_INVALIDBASEDIR              Base directory specified is invalid
        ///8       ERR_DEVFILELOAD_ERR             Failed loading device descriptor file
        ///9       ERR_NOTSELECTED                 Device specified is not selected
        ///10      ERR_INVALIDINPUTFILE            File contains invalid characters
        ///11      INVALID_PROPERTY                A Required connection propery was null or invalid.For Ethernet Connection the "IPAddress" property is required.
        ///12      UNSUPPORTED_CONNECTION_TYPE     Only ETHERNET connection type is supported
        /// </returns>
        public DataSignResult IssueZreport(string ABCDirectory)
        {
            rapidsign.rapidsign signHelper = new rapidsign.rapidsign();
            switch (ConType)
            {
                case ConnectionType.ETHERNET:
                    if (String.IsNullOrEmpty(Settings.Ethernet.IPAddress))
                    {
                        return DataSignResult.INVALID_PROPERTY;
                    }
                    string deviceLocation = Settings.Ethernet.IPAddress;
                    return (DataSignResult)signHelper.FSL_IssueZreport(ABCDirectory, deviceLocation);
                case ConnectionType.EMULATED:
                    //do nothing
                    return DataSignResult.ERR_SUCCESS;
            }
            return DataSignResult.UNSUPPORTED_CONNECTION_TYPE;

        }

    }
}
