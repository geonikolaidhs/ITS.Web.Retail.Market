
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ITS.POS.Hardware.Common
{
    public static class CardlinkLink
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        [DllImport("EcrDll64.dll", EntryPoint = "fullInit10", CharSet = CharSet.Ansi)]
        public static extern void fullInit10_64(ref StringBuilder host, int port, bool flag, ref StringBuilder logDir);

        [DllImport("EcrDll64.dll", EntryPoint = "destroy", CharSet = CharSet.Ansi)]
        public static extern void destroy_64();

        /*print data function declaration for CSharp sample*/
        [DllImport("EcrDll64.dll", EntryPoint = "printData10", CharSet = CharSet.Ansi)]
        public static extern bool printData10_64(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode, ref StringBuilder msgOpt,
            ref StringBuilder printDataReq, ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp,
            ref StringBuilder msgOptResp, ref StringBuilder respCodeResp, ref StringBuilder printResultResp, ref StringBuilder eftTidResp);

        [DllImport("EcrDll86.dll", EntryPoint = "printData10", CharSet = CharSet.Ansi)]
        public static extern bool printData10_86(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode, ref StringBuilder msgOpt,
            ref StringBuilder printDataReq, ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp,
            ref StringBuilder msgOptResp, ref StringBuilder respCodeResp, ref StringBuilder printResultResp, ref StringBuilder eftTidResp);

        [DllImport("EcrDll64.dll", EntryPoint = "init10", CharSet = CharSet.Ansi)]
        public static extern void init10_64(ref StringBuilder host, int port, bool flag);
        [DllImport("EcrDll86.dll", EntryPoint = "init10", CharSet = CharSet.Ansi)]
        public static extern void init10_86(ref StringBuilder host, int port, bool flag);


        [DllImport("EcrDll86.dll", EntryPoint = "fullInit10", CharSet = CharSet.Ansi)]
        public static extern void fullInit10_86(ref StringBuilder host, int port, bool flag, ref StringBuilder logDir);

        [DllImport("EcrDll86.dll", EntryPoint = "destroy", CharSet = CharSet.Ansi)]
        public static extern void destroy_86();

        [DllImport("EcrDll64.dll", EntryPoint = "loyaltySale10", CharSet = CharSet.Ansi)]
        public static extern bool loyaltySale10_64(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode,
           ref StringBuilder uniqueTxnId, ref StringBuilder amount, int amountLen, ref StringBuilder msgOpt, ref StringBuilder roomNumber,
           ref StringBuilder checkinDate, ref StringBuilder checkoutDate, ref StringBuilder agreementNum, ref StringBuilder agreementDate,
           ref StringBuilder cashierNum, ref StringBuilder tillNum, ref StringBuilder invoiceNum, ref StringBuilder invoiceNumCum,
           ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp, ref StringBuilder respCodeResp,
           ref StringBuilder respMessageResp, ref StringBuilder cardTypeResp, ref StringBuilder accNumResp, ref StringBuilder refNumResp,
           ref StringBuilder authCodeResp, ref StringBuilder batchNumResp, ref StringBuilder amountResp, ref StringBuilder msgOptResp,
           ref StringBuilder tipAmountResp, ref StringBuilder foreignAmountResp, ref StringBuilder foreignCurrencyCodeResp, ref StringBuilder exchangeRateInclMarkupResp,
           ref StringBuilder dccMarkupPercentageResp, ref StringBuilder dccExchangeDateOfRatetResp, ref StringBuilder bankId,
           ref StringBuilder aquirerName, ref StringBuilder numOfInstallmentsResp, ref StringBuilder numOfPostdateInstallments,
           ref StringBuilder earned, ref StringBuilder balance, ref StringBuilder discount, ref StringBuilder redemption,
           ref StringBuilder bonusPoolBlock, ref StringBuilder eftTidResp, ref StringBuilder mid, ref StringBuilder sn, ref StringBuilder merchantName,
           ref StringBuilder address, ref StringBuilder city, ref StringBuilder phone, ref StringBuilder applicationName, ref StringBuilder cardExpiryDate, ref StringBuilder posApplicationVersion,
           ref StringBuilder paymentSpecs, ref StringBuilder AID, ref StringBuilder TC, ref StringBuilder transactionType);

        [DllImport("EcrDll86.dll", EntryPoint = "loyaltySale10", CharSet = CharSet.Ansi)]
        public static extern bool loyaltySale10_86(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode,
            ref StringBuilder uniqueTxnId, ref StringBuilder amount, int amountLen, ref StringBuilder msgOpt, ref StringBuilder roomNumber,
            ref StringBuilder checkinDate, ref StringBuilder checkoutDate, ref StringBuilder agreementNum, ref StringBuilder agreementDate,
            ref StringBuilder cashierNum, ref StringBuilder tillNum, ref StringBuilder invoiceNum, ref StringBuilder invoiceNumCum,
            ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp, ref StringBuilder respCodeResp,
            ref StringBuilder respMessageResp, ref StringBuilder cardTypeResp, ref StringBuilder accNumResp, ref StringBuilder refNumResp,
            ref StringBuilder authCodeResp, ref StringBuilder batchNumResp, ref StringBuilder amountResp, ref StringBuilder msgOptResp,
            ref StringBuilder tipAmountResp, ref StringBuilder foreignAmountResp, ref StringBuilder foreignCurrencyCodeResp, ref StringBuilder exchangeRateInclMarkupResp,
            ref StringBuilder dccMarkupPercentageResp, ref StringBuilder dccExchangeDateOfRatetResp, ref StringBuilder bankId,
            ref StringBuilder aquirerName, ref StringBuilder numOfInstallmentsResp, ref StringBuilder numOfPostdateInstallments,
            ref StringBuilder earned, ref StringBuilder balance, ref StringBuilder discount, ref StringBuilder redemption,
            ref StringBuilder bonusPoolBlock, ref StringBuilder eftTidResp, ref StringBuilder mid, ref StringBuilder sn, ref StringBuilder merchantName,
            ref StringBuilder address, ref StringBuilder city, ref StringBuilder phone, ref StringBuilder applicationName, ref StringBuilder cardExpiryDate, ref StringBuilder posApplicationVersion,
            ref StringBuilder paymentSpecs, ref StringBuilder AID, ref StringBuilder TC, ref StringBuilder transactionType);

        [DllImport("EcrDll86.dll", EntryPoint = "loyaltyRefund10", CharSet = CharSet.Ansi)]
        public static extern bool loyaltyRefund10_86(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode, ref StringBuilder uniqueTxnId,
            ref StringBuilder amount, int amountLen, ref StringBuilder msgOpt, ref StringBuilder roomNumber, ref StringBuilder checkinDate,
            ref StringBuilder checkoutDate, ref StringBuilder agreementNum, ref StringBuilder agreementDate, ref StringBuilder cashierNum,
            ref StringBuilder tillNum, ref StringBuilder invoiceNum, ref StringBuilder invoiceNumCum, ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp,
            ref StringBuilder msgCodeResp, ref StringBuilder respCodeResp, ref StringBuilder respMessageResp, ref StringBuilder cardTypeResp,
            ref StringBuilder accNumResp, ref StringBuilder refNumResp, ref StringBuilder authCodeResp, ref StringBuilder batchNumResp,
            ref StringBuilder amountResp, ref StringBuilder msgOptResp, ref StringBuilder foreignAmountResp, ref StringBuilder foreignCurrencyCodeResp,
            ref StringBuilder exchangeRateInclMarkupResp, ref StringBuilder bankId, ref StringBuilder aquirerName, ref StringBuilder numOfInstallmentsResp,
            ref StringBuilder numOfPostdateInstallments, ref StringBuilder earned, ref StringBuilder balance, ref StringBuilder discount,
            ref StringBuilder redemption, ref StringBuilder bonusPoolBlock, ref StringBuilder eftTidResp, ref StringBuilder mid, ref StringBuilder sn, ref StringBuilder merchantName,
            ref StringBuilder address, ref StringBuilder city, ref StringBuilder phone, ref StringBuilder applicationName, ref StringBuilder cardExpiryDate, ref StringBuilder posApplicationVersion,
            ref StringBuilder paymentSpecs, ref StringBuilder AID, ref StringBuilder TC, ref StringBuilder transactionType);

        [DllImport("EcrDll64.dll", EntryPoint = "loyaltyRefund10", CharSet = CharSet.Ansi)]
        public static extern bool loyaltyRefund10_64(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode, ref StringBuilder uniqueTxnId,
         ref StringBuilder amount, int amountLen, ref StringBuilder msgOpt, ref StringBuilder roomNumber, ref StringBuilder checkinDate,
         ref StringBuilder checkoutDate, ref StringBuilder agreementNum, ref StringBuilder agreementDate, ref StringBuilder cashierNum,
         ref StringBuilder tillNum, ref StringBuilder invoiceNum, ref StringBuilder invoiceNumCum, ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp,
         ref StringBuilder msgCodeResp, ref StringBuilder respCodeResp, ref StringBuilder respMessageResp, ref StringBuilder cardTypeResp,
         ref StringBuilder accNumResp, ref StringBuilder refNumResp, ref StringBuilder authCodeResp, ref StringBuilder batchNumResp,
         ref StringBuilder amountResp, ref StringBuilder msgOptResp, ref StringBuilder foreignAmountResp, ref StringBuilder foreignCurrencyCodeResp,
         ref StringBuilder exchangeRateInclMarkupResp, ref StringBuilder bankId, ref StringBuilder aquirerName, ref StringBuilder numOfInstallmentsResp,
         ref StringBuilder numOfPostdateInstallments, ref StringBuilder earned, ref StringBuilder balance, ref StringBuilder discount,
         ref StringBuilder redemption, ref StringBuilder bonusPoolBlock, ref StringBuilder eftTidResp, ref StringBuilder mid, ref StringBuilder sn, ref StringBuilder merchantName,
         ref StringBuilder address, ref StringBuilder city, ref StringBuilder phone, ref StringBuilder applicationName, ref StringBuilder cardExpiryDate, ref StringBuilder posApplicationVersion,
         ref StringBuilder paymentSpecs, ref StringBuilder AID, ref StringBuilder TC, ref StringBuilder transactionType);

        [DllImport("EcrDll64.dll", EntryPoint = "fullReconciliation10", CharSet = CharSet.Ansi)]
        public static extern bool fullReconciliation10_64(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode,
     ref StringBuilder uniqueTxnId, ref StringBuilder batchNum, ref StringBuilder reconciliationMode,
     ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp,
     ref StringBuilder respCodeResp, ref StringBuilder respMessageResp, ref StringBuilder batchNumResp,
     ref StringBuilder reconciliationModeResp, ref StringBuilder batchNetAmountResp, ref StringBuilder batchOrigAmountResp,
     ref StringBuilder batchTotalCounterResp, ref StringBuilder batchTotalsResp, ref StringBuilder eftTidResp);

        [DllImport("EcrDll86.dll", EntryPoint = "fullReconciliation10", CharSet = CharSet.Ansi)]
        public static extern bool fullReconciliation10_86(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode,
           ref StringBuilder uniqueTxnId, ref StringBuilder batchNum, ref StringBuilder reconciliationMode,
           ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp,
           ref StringBuilder respCodeResp, ref StringBuilder respMessageResp, ref StringBuilder batchNumResp,
           ref StringBuilder reconciliationModeResp, ref StringBuilder batchNetAmountResp, ref StringBuilder batchOrigAmountResp,
           ref StringBuilder batchTotalCounterResp, ref StringBuilder batchTotalsResp, ref StringBuilder eftTidResp);

        [DllImport("EcrDll64.dll", EntryPoint = "reconciliation10", CharSet = CharSet.Ansi)]
        public static extern bool reconciliation10_64(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode, ref StringBuilder uniqueTxnId,
          ref StringBuilder batchNum, ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp,
          ref StringBuilder respCodeResp, ref StringBuilder respMessageResp, ref StringBuilder batchNumResp, ref StringBuilder batchNetAmountResp,
          ref StringBuilder batchOrigAmountResp, ref StringBuilder batchTotalCounterResp, ref StringBuilder eftTidResp);

        [DllImport("EcrDll86.dll", EntryPoint = "reconciliation10", CharSet = CharSet.Ansi)]
        public static extern bool reconciliation10_86(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode, ref StringBuilder uniqueTxnId,
          ref StringBuilder batchNum, ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp,
          ref StringBuilder respCodeResp, ref StringBuilder respMessageResp, ref StringBuilder batchNumResp, ref StringBuilder batchNetAmountResp,
          ref StringBuilder batchOrigAmountResp, ref StringBuilder batchTotalCounterResp, ref StringBuilder eftTidResp);

        /*get transaction  function declaration for CSharp sample*/
        [DllImport("EcrDll64.dll", EntryPoint = "getTransactionDetails10", CharSet = CharSet.Ansi)]
        public static extern bool getTransactionDetails10(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode,
            ref StringBuilder uniqueTxnId, ref StringBuilder refNum, ref StringBuilder token,
            ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp, ref StringBuilder respCodeResp,
            ref StringBuilder respMessageResp, ref StringBuilder cardTypeResp, ref StringBuilder accNumResp, ref StringBuilder refNumResp,
            ref StringBuilder authCodeResp, ref StringBuilder batchNumResp, ref StringBuilder amountResp, ref StringBuilder roomNumber, ref StringBuilder checkinDate, ref StringBuilder checkoutDate, ref StringBuilder agreementNum, ref StringBuilder agreementDate,
            ref StringBuilder cashierNum, ref StringBuilder tillNum, ref StringBuilder invoiceNum, ref StringBuilder msgOptResp,
            ref StringBuilder tipAmountResp, ref StringBuilder ecrRefNumResp, ref StringBuilder eftRefNumResp, ref StringBuilder foreignAmountResp, ref StringBuilder foreignCurrencyCodeResp, ref StringBuilder exchangeRateInclMarkupResp,
            ref StringBuilder dccMarkupPercentageResp, ref StringBuilder dccExchangeDateOfRatetResp, ref StringBuilder bankId,
            ref StringBuilder aquirerName, ref StringBuilder numOfInstallmentsResp, ref StringBuilder numOfPostdateInstallments,
            ref StringBuilder earned, ref StringBuilder balance, ref StringBuilder discount, ref StringBuilder redemption,
            ref StringBuilder bonusPoolBlock, ref StringBuilder eftTidResp, ref StringBuilder mid, ref StringBuilder sn, ref StringBuilder merchantName,
            ref StringBuilder address, ref StringBuilder city, ref StringBuilder phone, ref StringBuilder applicationName, ref StringBuilder cardExpiryDate, ref StringBuilder posApplicationVersion,
            ref StringBuilder paymentSpecs, ref StringBuilder AID, ref StringBuilder TC, ref StringBuilder transactionType, ref StringBuilder tokenResp);




        /// <summary>
        /// Request Fields
        /// </summary>
        private static StringBuilder sessionId, msgType, msgCode, uniqueTxnId, amount, msgOpt, roomNumber, checkinDate, checkoutDate, agreementNum, refNum, authCode,
            agreementDate, ecrRefNum, billerTid, batchNum, printDataReq, reconciliationMode, cashierNum, tillNum, invoiceNum, invoiceNumCum, pan, expirationDate, cvv2, token;


        /// <summary>
        /// Response Fields
        /// </summary>
        private static StringBuilder sessionIdResp, msgTypeResp, msgCodeResp, respCodeResp, respMessageResp, cardTypeResp, accNumResp, refNumResp,
            authCodeResp, batchNumResp, amountResp, msgOptResp, tipAmountResp, foreignAmountResp, foreignCurrencyCodeResp,
            exchangeRateInclMarkupResp, dccMarkupPercentageResp, dccExchangeDateOfRatetResp, eftTidResp, merchantNameResp, addressResp,
            postalCodeResp, townResp, vatRegNoResp, mccResp, ecrRefNumResp, eftRefNumResp, numOfInstallmentsResp, printResultResp,
            firstInstallmentDateResp, discountRateForInstallmentsResp, batchNetAmountResp, batchOrigAmountResp, batchTotalCounterResp,
            reconciliationModeResp, batchTotalsResp, bankId, aquirerName, numOfPostdateInstallments, earned, balance,
            discount, redemption, bonusPoolBlock, midResp, snResp, addtionalMerchantNameResp, additionalAddressResp, cityResp, phoneResp, applicationNameResp,
            cardExpiryDateResp, posTerminalVersionResp, paymentSpecsResp, AID, TC, transactionType, tokenResp;

        private static int amountLen;

        private static bool environment64 = Environment.Is64BitProcess;

        public static CardlinkDeviceResult ExecutePayment(decimal am, int documentNumber, string POSUsername, int POSID, int installments, int installmentsMonthDelay, EthernetDeviceSettings settings)
        {
            return _ExecutePayment(am, documentNumber, POSUsername, POSID, installments, installmentsMonthDelay, settings);
        }

        public static CardlinkDeviceResult ExecuteRefund(decimal am, int documentNumber, string POSUsername, int POSID, int installments, int installmentsMonthDelay, EthernetDeviceSettings settings)
        {

            return _ExecuteRefund(am, documentNumber, POSUsername, POSID, installments, installmentsMonthDelay, settings);
        }

        public static CardlinkBatchCloseResult BatchClose(EthernetDeviceSettings settings, string lastBatch)
        {
            return _BatchClose(settings, lastBatch);
        }

        public static bool TestComunnication(EthernetDeviceSettings settings)
        {
            return _PrintData("267EN%%E *T E S T* %%ATerminal Id: " + tillNum, settings);
        }

        private static bool Initialised = false;

        private static bool _PrintData(string data, EthernetDeviceSettings settings)
        {
            sessionId = new StringBuilder("000000");
            msgType = new StringBuilder("600");
            msgCode = new StringBuilder("00");
            msgOpt = new StringBuilder("3000");
            printDataReq = new StringBuilder(data);
            bool result;
            try
            {
                ClearFields(settings);
                result = environment64 ? printData10_64(ref sessionId, ref msgType, ref msgCode, ref msgOpt, ref printDataReq, ref sessionIdResp,
                                         ref msgTypeResp, ref msgCodeResp, ref msgOptResp, ref respCodeResp, ref printResultResp, ref eftTidResp)
                                       : result = printData10_86(ref sessionId, ref msgType, ref msgCode, ref msgOpt, ref printDataReq, ref sessionIdResp,
                                         ref msgTypeResp, ref msgCodeResp, ref msgOptResp, ref respCodeResp, ref printResultResp, ref eftTidResp);
            }
            catch (Exception ex)
            {
                Destroy();
                return false;
            }
            return result;
        }
        private static CardlinkDeviceResult _ExecutePayment(decimal am, int documentNumber, string POSUsername, int POSID, int installments, int installmentsMonthDelay, EthernetDeviceSettings settings)
        {
            CultureInfo originalCultureInfo = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            CardlinkDeviceResult cardlinkResult = null;
            ClearFields(settings);
            PrepareFields(am, documentNumber, POSUsername, POSID, "200", "00", "0500");
            bool result = false;
            try
            {

                result = environment64 ? loyaltySale10_64(ref sessionId, ref msgType, ref msgCode,
                          ref uniqueTxnId, ref amount, amountLen, ref msgOpt, ref roomNumber,
                          ref checkinDate, ref checkoutDate, ref agreementNum, ref agreementDate,
                          ref cashierNum, ref tillNum, ref invoiceNum, ref invoiceNumCum,
                          ref sessionIdResp, ref msgTypeResp, ref msgCodeResp, ref respCodeResp,
                          ref respMessageResp, ref cardTypeResp, ref accNumResp, ref refNumResp,
                          ref authCodeResp, ref batchNumResp, ref amountResp, ref msgOptResp,
                          ref tipAmountResp, ref foreignAmountResp, ref foreignCurrencyCodeResp, ref exchangeRateInclMarkupResp,
                          ref dccMarkupPercentageResp, ref dccExchangeDateOfRatetResp, ref bankId,
                          ref aquirerName, ref numOfInstallmentsResp, ref numOfPostdateInstallments,
                          ref earned, ref balance, ref discount, ref redemption,
                          ref bonusPoolBlock, ref eftTidResp, ref midResp, ref snResp, ref addtionalMerchantNameResp,
                          ref additionalAddressResp, ref cityResp, ref phoneResp, ref applicationNameResp, ref cardExpiryDateResp, ref posTerminalVersionResp,
                          ref paymentSpecsResp, ref AID, ref TC, ref transactionType)
                          :
                          loyaltySale10_86(ref sessionId, ref msgType, ref msgCode,
                          ref uniqueTxnId, ref amount, amountLen, ref msgOpt, ref roomNumber,
                          ref checkinDate, ref checkoutDate, ref agreementNum, ref agreementDate,
                          ref cashierNum, ref tillNum, ref invoiceNum, ref invoiceNumCum,
                          ref sessionIdResp, ref msgTypeResp, ref msgCodeResp, ref respCodeResp,
                          ref respMessageResp, ref cardTypeResp, ref accNumResp, ref refNumResp,
                          ref authCodeResp, ref batchNumResp, ref amountResp, ref msgOptResp,
                          ref tipAmountResp, ref foreignAmountResp, ref foreignCurrencyCodeResp, ref exchangeRateInclMarkupResp,
                          ref dccMarkupPercentageResp, ref dccExchangeDateOfRatetResp, ref bankId,
                          ref aquirerName, ref numOfInstallmentsResp, ref numOfPostdateInstallments,
                          ref earned, ref balance, ref discount, ref redemption,
                          ref bonusPoolBlock, ref eftTidResp, ref midResp, ref snResp, ref addtionalMerchantNameResp,
                          ref additionalAddressResp, ref cityResp, ref phoneResp, ref applicationNameResp, ref cardExpiryDateResp, ref posTerminalVersionResp,
                          ref paymentSpecsResp, ref AID, ref TC, ref transactionType);

                cardlinkResult = result ? new CardlinkDeviceResult(sessionId, msgType, msgCode, uniqueTxnId, amount, amountLen, msgOpt,
                                roomNumber, checkinDate, checkoutDate, agreementNum, agreementDate, cashierNum, tillNum,
                                invoiceNum, invoiceNumCum, sessionIdResp, msgTypeResp, msgCodeResp, respCodeResp, respMessageResp, cardTypeResp,
                                accNumResp, refNumResp, authCodeResp, batchNumResp, amountResp, msgOptResp, tipAmountResp,
                                foreignAmountResp, foreignCurrencyCodeResp, exchangeRateInclMarkupResp,
                               dccMarkupPercentageResp, dccExchangeDateOfRatetResp, bankId,
                               aquirerName, numOfInstallmentsResp, numOfPostdateInstallments,
                               earned, balance, discount, redemption,
                               bonusPoolBlock, eftTidResp, midResp, snResp, addtionalMerchantNameResp,
                               additionalAddressResp, cityResp, phoneResp, applicationNameResp, cardExpiryDateResp, posTerminalVersionResp,
                               paymentSpecsResp, AID, TC, transactionType) : null;
            }
            catch (Exception ex)
            {
                Destroy();
                throw new Exception(ex.Message);
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCultureInfo;
            }
            return cardlinkResult;
        }


        private static CardlinkDeviceResult _ExecuteRefund(decimal am, int documentNumber, string POSUsername, int POSID, int installments, int installmentsMonthDelay, EthernetDeviceSettings settings)

        {
            CultureInfo originalCultureInfo = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            CardlinkDeviceResult result = null;
            ClearFields(settings);
            PrepareFields(am, documentNumber, POSUsername, POSID, "200", "20", "500");
            bool deviceResult = false;
            try
            {
                deviceResult = environment64 ?
               loyaltyRefund10_64(ref sessionId, ref msgType, ref msgCode, ref uniqueTxnId, ref amount, amountLen, ref msgOpt,
               ref roomNumber, ref checkinDate, ref checkoutDate, ref agreementNum, ref agreementDate, ref cashierNum, ref tillNum,
               ref invoiceNum, ref invoiceNumCum, ref sessionIdResp, ref msgTypeResp, ref msgCodeResp, ref respCodeResp,
               ref respMessageResp, ref cardTypeResp, ref accNumResp, ref refNumResp, ref authCodeResp, ref batchNumResp, ref amountResp,
               ref msgOptResp, ref foreignAmountResp, ref foreignCurrencyCodeResp, ref exchangeRateInclMarkupResp, ref bankId, ref aquirerName,
               ref numOfInstallmentsResp, ref numOfPostdateInstallments, ref earned, ref balance, ref discount, ref redemption,
               ref bonusPoolBlock, ref eftTidResp, ref midResp, ref snResp, ref addtionalMerchantNameResp,
               ref additionalAddressResp, ref cityResp, ref phoneResp, ref applicationNameResp, ref cardExpiryDateResp, ref posTerminalVersionResp,
               ref paymentSpecsResp, ref AID, ref TC, ref transactionType)
               : deviceResult =
               loyaltyRefund10_86(ref sessionId, ref msgType, ref msgCode, ref uniqueTxnId, ref amount, amountLen, ref msgOpt,
               ref roomNumber, ref checkinDate, ref checkoutDate, ref agreementNum, ref agreementDate, ref cashierNum, ref tillNum,
               ref invoiceNum, ref invoiceNumCum, ref sessionIdResp, ref msgTypeResp, ref msgCodeResp, ref respCodeResp,
               ref respMessageResp, ref cardTypeResp, ref accNumResp, ref refNumResp, ref authCodeResp, ref batchNumResp, ref amountResp,
               ref msgOptResp, ref foreignAmountResp, ref foreignCurrencyCodeResp, ref exchangeRateInclMarkupResp, ref bankId, ref aquirerName,
               ref numOfInstallmentsResp, ref numOfPostdateInstallments, ref earned, ref balance, ref discount, ref redemption,
               ref bonusPoolBlock, ref eftTidResp, ref midResp, ref snResp, ref addtionalMerchantNameResp,
               ref additionalAddressResp, ref cityResp, ref phoneResp, ref applicationNameResp, ref cardExpiryDateResp, ref posTerminalVersionResp,
               ref paymentSpecsResp, ref AID, ref TC, ref transactionType);

                result = deviceResult ? new CardlinkDeviceResult(sessionId, msgType, msgCode, uniqueTxnId, amount, amountLen, msgOpt,
                                  roomNumber, checkinDate, checkoutDate, agreementNum, agreementDate, cashierNum, tillNum,
                                  invoiceNum, invoiceNumCum, sessionIdResp, msgTypeResp, msgCodeResp, respCodeResp, respMessageResp, cardTypeResp,
                                  accNumResp, refNumResp, authCodeResp, batchNumResp, amountResp, msgOptResp, tipAmountResp,
                                  foreignAmountResp, foreignCurrencyCodeResp, exchangeRateInclMarkupResp,
                                  dccMarkupPercentageResp, dccExchangeDateOfRatetResp, bankId,
                                  aquirerName, numOfInstallmentsResp, numOfPostdateInstallments,
                                  earned, balance, discount, redemption,
                                  bonusPoolBlock, eftTidResp, midResp, snResp, addtionalMerchantNameResp,
                                  additionalAddressResp, cityResp, phoneResp, applicationNameResp, cardExpiryDateResp, posTerminalVersionResp,
                                  paymentSpecsResp, AID, TC, transactionType) : null;
            }
            catch (Exception ex)
            {
                Destroy();
                throw;
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = originalCultureInfo;
            }
            return result;
        }



        public static CardlinkBatchCloseResult _BatchClose(EthernetDeviceSettings settings, string lastBatchNo)
        {
            msgType = new StringBuilder("500");
            msgCode = new StringBuilder("20");
            reconciliationMode = new StringBuilder("1");
            batchNum = new StringBuilder(lastBatchNo);
            bool deviceResult = false;
            CardlinkBatchCloseResult result = new CardlinkBatchCloseResult();
            ClearFields(settings);
            try
            {
                deviceResult = environment64 ? fullReconciliation10_64(ref sessionId, ref msgType, ref msgCode, ref uniqueTxnId, ref batchNum,
                ref reconciliationMode, ref sessionIdResp, ref msgTypeResp, ref msgCodeResp, ref respCodeResp, ref respMessageResp,
                ref batchNumResp, ref reconciliationModeResp, ref batchNetAmountResp, ref batchOrigAmountResp, ref batchTotalCounterResp,
                ref batchTotalsResp, ref eftTidResp)
               :
               fullReconciliation10_86(ref sessionId, ref msgType, ref msgCode, ref uniqueTxnId, ref batchNum,
                ref reconciliationMode, ref sessionIdResp, ref msgTypeResp, ref msgCodeResp, ref respCodeResp, ref respMessageResp,
                ref batchNumResp, ref reconciliationModeResp, ref batchNetAmountResp, ref batchOrigAmountResp, ref batchTotalCounterResp,
                ref batchTotalsResp, ref eftTidResp);

                result = deviceResult ? new CardlinkBatchCloseResult(sessionIdResp, msgType, msgCode, msgCodeResp, respMessageResp, eftTidResp
                                  , batchNumResp, batchNetAmountResp, batchOrigAmountResp, batchTotalCounterResp, batchTotalsResp,
                                  batchNumResp, batchNetAmountResp, batchTotalsResp, batchTotalCounterResp, batchOrigAmountResp) : null;
            }
            catch (Exception ex)
            {
                Destroy();
                throw;
            }

            return result;
        }

        private static void ClearFields(EthernetDeviceSettings settings)
        {
            if (!Initialised)
            {
                PortInitialization(settings);
            }
            sessionIdResp = new StringBuilder(new string(' ', 6));
            msgTypeResp = new StringBuilder(new string(' ', 3));
            msgCodeResp = new StringBuilder(new string(' ', 2));
            respCodeResp = new StringBuilder(new string(' ', 2));
            respMessageResp = new StringBuilder(new string(' ', 60));
            cardTypeResp = new StringBuilder(new string(' ', 16));
            accNumResp = new StringBuilder(new string(' ', 23));
            refNumResp = new StringBuilder(new string(' ', 6));
            authCodeResp = new StringBuilder(new string(' ', 6));
            batchNumResp = new StringBuilder(new string(' ', 6));
            amountResp = new StringBuilder(new string(' ', 13));
            msgOptResp = new StringBuilder(new string(' ', 4));
            tipAmountResp = new StringBuilder(new string(' ', 10));
            foreignAmountResp = new StringBuilder(new string(' ', 13));
            foreignCurrencyCodeResp = new StringBuilder(new string(' ', 3));
            exchangeRateInclMarkupResp = new StringBuilder(new string(' ', 9));
            dccMarkupPercentageResp = new StringBuilder(new string(' ', 8));
            dccExchangeDateOfRatetResp = new StringBuilder(new string(' ', 4));
            merchantNameResp = new StringBuilder(new string(' ', 24));
            addressResp = new StringBuilder(new string(' ', 30));
            postalCodeResp = new StringBuilder(new string(' ', 10));
            townResp = new StringBuilder(new string(' ', 24));
            vatRegNoResp = new StringBuilder(new string(' ', 9));
            mccResp = new StringBuilder(new string(' ', 4));
            ecrRefNumResp = new StringBuilder(new string(' ', 6));
            eftRefNumResp = new StringBuilder(new string(' ', 12));
            batchNetAmountResp = new StringBuilder(new string(' ', 13));
            batchOrigAmountResp = new StringBuilder(new string(' ', 13));
            batchTotalCounterResp = new StringBuilder(new string(' ', 4));
            batchTotalsResp = new StringBuilder(new string(' ', 1004));
            reconciliationModeResp = new StringBuilder(new string(' ', 1));
            printResultResp = new StringBuilder(new string(' ', 2));
            numOfInstallmentsResp = new StringBuilder(new string(' ', 2));
            numOfPostdateInstallments = new StringBuilder(new string(' ', 4));
            eftTidResp = new StringBuilder(new string(' ', 12));
            bankId = new StringBuilder(new string(' ', 3));
            aquirerName = new StringBuilder(new string(' ', 10));
            earned = new StringBuilder(new string(' ', 6));
            balance = new StringBuilder(new string(' ', 13));
            discount = new StringBuilder(new string(' ', 13));
            redemption = new StringBuilder(new string(' ', 13));
            bonusPoolBlock = new StringBuilder(new string(' ', 1004));
            midResp = new StringBuilder(new string(' ', 12));
            snResp = new StringBuilder(new string(' ', 12));
            addtionalMerchantNameResp = new StringBuilder(new string(' ', 40));
            additionalAddressResp = new StringBuilder(new string(' ', 40));
            cityResp = new StringBuilder(new string(' ', 40));
            phoneResp = new StringBuilder(new string(' ', 25));
            applicationNameResp = new StringBuilder(new string(' ', 16));
            cardExpiryDateResp = new StringBuilder(new string(' ', 4));
            posTerminalVersionResp = new StringBuilder(new string(' ', 9));
            paymentSpecsResp = new StringBuilder(new string(' ', 3));
            AID = new StringBuilder(new string(' ', 20));
            TC = new StringBuilder(new string(' ', 16));
            transactionType = new StringBuilder(new string(' ', 20));
            tokenResp = new StringBuilder(new string(' ', 60));
            Guid id = Guid.NewGuid();
            uniqueTxnId = new StringBuilder(id.ToString());
            invoiceNum = new StringBuilder("0".PadLeft(6, '0'));
            invoiceNumCum = new StringBuilder("0".PadLeft(6, '0'));
        }

        private static void PrepareFields(decimal am, int documentNumber, string POSUsername, int POSID, string messageType, string messageCode, string msgOptions)
        {
            invoiceNumCum = new StringBuilder(new string('0', 6));
            amountLen = 0;
            sessionId = new StringBuilder("000000");
            msgCode = new StringBuilder(messageCode);
            msgType = new StringBuilder(messageType);
            amount = new StringBuilder(EnsureDecimalDigits(am));
            amountLen = amount.Length;
            msgOpt = new StringBuilder(msgOptions);
            cashierNum = new StringBuilder(POSUsername.PadLeft(4, '0'));
            tillNum = new StringBuilder(POSID.ToString().PadLeft(4, '0'));
            invoiceNum = new StringBuilder(documentNumber.ToString().PadLeft(6, '0'));
            invoiceNumCum = new StringBuilder("0".PadLeft(6, '0'));

        }

        private static string EnsureDecimalDigits(decimal amount)
        {
            decimal outAmount;
            decimal.TryParse(amount.ToString().Replace(',', '.'), NumberStyles.Any, new CultureInfo("en-US"), out outAmount);
            string strAmount = outAmount.ToString();
            string[] strArray = outAmount.ToString().Split('.');
            if (strArray != null && strArray.Length > 1)
            {
                return strArray[1].Length == 1 ? strAmount + "0" : strAmount;
            }
            return strAmount;
        }

        public static void Destroy()
        {
            try
            {
                if (environment64)
                {
                    destroy_64();
                }
                else
                {
                    destroy_86();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                Initialised = false;
            }
        }
        public static void PortInitialization(EthernetDeviceSettings settings)
        {
            try
            {
                if (environment64)
                {
                    PortInitialization_64(settings);
                }
                else
                {
                    PortInitialization_86(settings);
                }
            }
            catch (Exception ex) { }
        }


        private static void PortInitialization_86(EthernetDeviceSettings settings)
        {
            StringBuilder port = new StringBuilder(settings.Port);
            StringBuilder logDir = new StringBuilder("c:\\EcrDllLogs\\");
            StringBuilder host = new StringBuilder(settings.IPAddress);
            bool flagLog = false;

            try
            {
                fullInit10_86(ref host, settings.Port, flagLog, ref logDir);
                Initialised = true;

            }
            catch (Exception ex)
            { }
        }

        private static void PortInitialization_64(EthernetDeviceSettings settings)
        {
            StringBuilder port = new StringBuilder(settings.Port);
            StringBuilder logDir = new StringBuilder("c:\\EcrDllLogs\\");
            StringBuilder host = new StringBuilder(settings.IPAddress);
            bool flagLog = false;

            try
            {
                fullInit10_64(ref host, settings.Port, flagLog, ref logDir);
                Initialised = true;
            }
            catch (Exception ex)
            { }
        }


    }
}
