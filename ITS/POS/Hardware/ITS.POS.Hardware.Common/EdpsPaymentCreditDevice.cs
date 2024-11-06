using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Exceptions;
using System.Runtime.InteropServices;

namespace ITS.POS.Hardware.Common
{
    public class EdpsPaymentCreditDevice : Device
    {
        private static byte UseLPR = 84;//ASCII 'T'
        tagPOS_STATIC_DATA PosStaticData = new tagPOS_STATIC_DATA();
        public EdpsPaymentCreditDevice(ConnectionType conType, string deviceName)
            : base()
        {
            this.ConType = conType;
            this.DeviceName = deviceName;
        }

        public bool PortInitialized { get; private set; }

        public override eDeviceCheckResult CheckDevice(out string message)
        {
            /*message = "Not Checked";
            return eDeviceCheckResult.INFO;*/
            if(this.PortInitialized)
            {
                message = "OK";
                return eDeviceCheckResult.SUCCESS;
            }
            PortInitialization();
            if (this.PortInitialized)
            {
                message = "OK";
                return eDeviceCheckResult.SUCCESS;
            }
            message = "Cannot Connect to EDPS Device";
            return eDeviceCheckResult.WARNING;
        }

        public override void AfterLoad(List<Device> devices)
        {
            base.AfterLoad(devices);
        }

        private void PortInitialization()
        {
            EdpsLink.EDPOS_CloseComm();
            COM_PORT port;
            BAUD_RATE baud;
            if (Enum<COM_PORT>.TryParse(this.Settings.COM.PortName, out port, true))
            {
                baud = (BAUD_RATE)this.Settings.COM.BaudRate;
                if (EdpsLink.EDPOS_InitComm(port, baud, 60, ref PosStaticData) == 0)
                {
                    this.PortInitialized = true;
                }
            }
        }

        public bool TestInternalCommunication()
        {
            if(!this.PortInitialized)
            {
                PortInitialization();                
            }
            return this.PortInitialized;            
        }

        public bool TestCommunication()
        {
            if (!this.PortInitialized)
            {
                PortInitialization();
            }
            return EdpsLink.EDPOS_DoTestComm() == 0;
        }

        public EdpsBatchCloseResult BatchClose()
        {
            if (TestInternalCommunication() == false)
            {
                throw new POSUserVisibleException(Resources.POSClientResources.EDPS_COMMUNICATION_ERROR);
            }

            tagBATCH_TOTALS totals;// = new tagBATCH_TOTALS();
            tagBATCH_ENTRY[] transactionlog = null;
            int size = 1000;
            byte[] rc = new byte[3], authID = new byte[7];
            IntPtr totalsPtr = Marshal.AllocHGlobal(72);
            IntPtr transactionsPtr = Marshal.AllocHGlobal(112000);
            IntPtr sizePtr = Marshal.AllocHGlobal(4);
            IntPtr rcPtr = Marshal.AllocHGlobal(3);
            IntPtr authPtr = Marshal.AllocHGlobal(7);
            try
            {
                if (0 == EdpsLink.EDPOS_DoBatchClose_arr(totalsPtr, transactionsPtr, sizePtr, rcPtr, authPtr, true))
                {
                    Marshal.Copy(authPtr, authID, 0, 7);
                    Marshal.Copy(rcPtr, rc, 0, 3);
                    size = Marshal.ReadInt32(sizePtr);
                    transactionlog = new tagBATCH_ENTRY[size];
                    for (int i = 0; i < size; i++)
                    {
                        transactionlog[i] = (tagBATCH_ENTRY)Marshal.PtrToStructure(IntPtr.Add(transactionsPtr, i * 112), typeof(tagBATCH_ENTRY));
                    }
                    totals = (tagBATCH_TOTALS)Marshal.PtrToStructure(totalsPtr, typeof(tagBATCH_TOTALS));
                    if (rc[0] == rc[1] && rc[0] == '0')
                    {
                        return new EdpsBatchCloseResult(totals, transactionlog, size);
                    }
                    string errorCode = "" + rc[0] + rc[1];
                    if (EdpsLink.ErrorCodes.ContainsKey(errorCode))
                    {
                        throw new POSUserVisibleException("Error:" + EdpsLink.ErrorCodes[errorCode]);
                    }
                    throw new POSUserVisibleException("Error:" + errorCode);
                }
                throw new POSUserVisibleException("Edps: Connection error");
            }
            finally
            {
                Marshal.FreeHGlobal(totalsPtr);
                Marshal.FreeHGlobal(transactionsPtr);
                Marshal.FreeHGlobal(sizePtr);
                Marshal.FreeHGlobal(rcPtr);
                Marshal.FreeHGlobal(authPtr);
            }
        }

        public EdpsDeviceResult CancelPayment(int receiptNumber, decimal amount, decimal tip, int installments, int installmentsMonthDelay, string cashierID)
        {
            try
            {
                if (!this.PortInitialized)
                {
                    PortInitialization();
                }
                if (!this.PortInitialized)
                {
                    throw new POSUserVisibleException("Cannot communicate with credit card device");
                }
                tagTXN_REQUEST request = new tagTXN_REQUEST();
                request.iReceiptNo = receiptNumber;
                request.eTxnType = (byte)TXN_TYPE.TXN_VOID;
                request.iECRRefNo = 1001; //unused

                request.llAmount = (ulong)(100 * amount); // amount is 1,00 EUR
                request.llTipAmount = (ulong)(100 * tip);
                request.iInstallments = (installments == 0 ? 1 : installments); // no installements
                request.iPostDating = installmentsMonthDelay;
                request.szCashier = (cashierID == null ? "   " : cashierID.PadRight(3).Substring(0, 3));
                request.szAuthID = " ";
                tagTXN_RESPONSE response = new tagTXN_RESPONSE();
                if (EdpsLink.EDPOS_DoTransaction(ref request, ref response, UseLPR) == 0)
                {
                    return new EdpsDeviceResult(response);
                }
                return null;
            }
            catch (Exception ex)
            {
                LogInfoException("Error during Edps Payment Communication", ex);
                throw;
            }
        }

        public EdpsDeviceResult ExecuteRefund( decimal amount, decimal tip, int installments, int installmentsMonthDelay, string cashierID)
        {
            try
            {
                if (TestInternalCommunication() == false)
                {
                    throw new POSUserVisibleException(Resources.POSClientResources.EDPS_COMMUNICATION_ERROR);
                }
                tagTXN_REQUEST request = new tagTXN_REQUEST();
                request.iReceiptNo = 0;
                request.eTxnType = (byte)TXN_TYPE.TXN_REFUND;
                request.iECRRefNo = 1001; //unused


                request.llAmount = (ulong)(100 * amount); // amount is 1,00 EUR
                request.llTipAmount = (ulong)(100 * tip);
                request.iInstallments = (installments == 0 ? 1 : installments); // no installements
                request.iPostDating = installmentsMonthDelay;
                request.szCashier = (cashierID == null ? "   " : cashierID.PadRight(3).Substring(0, 3));
                request.szAuthID = " ";
                tagTXN_RESPONSE response = new tagTXN_RESPONSE();
                if (EdpsLink.EDPOS_DoTransaction(ref request, ref response, UseLPR) == 0)
                {
                    return new EdpsDeviceResult(response);
                }
                return null;
            }
            catch (Exception ex)
            {
                LogInfoException("Error during Edps Payment Communication", ex);
                throw;
            }
        }

        public EdpsDeviceResult ExecutePayment(decimal amount, decimal tip, int installments, int installmentsMonthDelay,
            string cashierID)
        {
            try
            {
                if (TestInternalCommunication() == false)
                {
                    throw new POSUserVisibleException(Resources.POSClientResources.EDPS_COMMUNICATION_ERROR);
                }
                tagTXN_REQUEST request = new tagTXN_REQUEST();
                request.iReceiptNo = 0;
                request.eTxnType = (byte)TXN_TYPE.TXN_SALE;
                request.iECRRefNo = 1001; //unused

                request.llAmount = (ulong)(100 * amount); // amount is 1,00 EUR
                request.llTipAmount = (ulong)(100 * tip);
                request.iInstallments = (installments == 0 ? 1 : installments); // no installements
                request.iPostDating = installmentsMonthDelay;
                request.szCashier = (cashierID == null ? "   " : cashierID.PadRight(3).Substring(0, 3));
                request.szAuthID = " ";
                tagTXN_RESPONSE response = new tagTXN_RESPONSE();
                if (EdpsLink.EDPOS_DoTransaction(ref request, ref response, UseLPR) == 0)
                {
                    return new EdpsDeviceResult(response);
                }
                return null;
            }
            catch (Exception ex)
            {
                LogInfoException("Error during Edps Payment Communication", ex);
                throw;
            }
        }

        public EdpsDeviceResult ExecuteOfflinePayment(decimal amount, decimal tip, int installments, int installmentsMonthDelay, string cashierID, string authID)
        {
            try
            {
                if (!this.PortInitialized)
                {
                    PortInitialization();
                }
                if (!this.PortInitialized)
                {
                    throw new POSUserVisibleException("Cannot communicate with credit card device");
                }
                tagTXN_REQUEST request = new tagTXN_REQUEST();
                request.iReceiptNo = 0;
                request.eTxnType = (byte)TXN_TYPE.TXN_OFFLINE;
                request.iECRRefNo = 1001; //unused

                request.llAmount = (ulong)(100 * amount); // amount is 1,00 EUR
                request.llTipAmount = (ulong)(100 * tip);
                request.iInstallments = (installments == 0 ? 1 : installments); // no installements
                request.iPostDating = installmentsMonthDelay;
                request.szCashier = (cashierID == null ? "   " : cashierID.PadRight(3).Substring(0, 3));
                request.szAuthID = authID.PadRight(7).Substring(0,7);
                tagTXN_RESPONSE response = new tagTXN_RESPONSE();
                if (EdpsLink.EDPOS_DoTransaction(ref request, ref response, UseLPR) == 0)
                {
                    return new EdpsDeviceResult(response);
                }
                return null;
            }
            catch (Exception ex)
            {
                LogInfoException("Error during Edps Payment Communication", ex);
                throw;
            }
        }
    }
}
