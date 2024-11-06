using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    public interface ICardlinkLink
    {
        void fullinit(ref StringBuilder host, int port, bool flag, ref StringBuilder logDir);
        bool sale(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode,
            ref StringBuilder uniqueTxnId, ref StringBuilder amount, int amountLen, ref StringBuilder msgOpt, ref StringBuilder cashierNum,
            ref StringBuilder tillNum, ref StringBuilder invoiceNum, ref StringBuilder roomNumber,
            ref StringBuilder checkoutDate, ref StringBuilder agreementNum, ref StringBuilder agreementDate, ref StringBuilder sessionIdResp,
            ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp, ref StringBuilder respCodeResp, ref StringBuilder respMessageResp,
            ref StringBuilder cardTypeResp, ref StringBuilder accNumResp, ref StringBuilder refNumResp, ref StringBuilder authCodeResp,
            ref StringBuilder batchNumResp, ref StringBuilder amountResp, ref StringBuilder msgOptResp, ref StringBuilder tipAmountResp,
            ref StringBuilder foreignAmountResp, ref StringBuilder foreignCurrencyCodeResp, ref StringBuilder exchangeRateInclMarkupResp,
            ref StringBuilder dccMarkupPercentageResp, ref StringBuilder dccExchangeDateOfRatetResp, ref StringBuilder eftTidResp);
        bool refund(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode, ref StringBuilder uniqueTxnId,
           ref StringBuilder amount, int amountLen, ref StringBuilder msgOpt, ref StringBuilder roomNumber, ref StringBuilder checkoutDate,
           ref StringBuilder agreementNum, ref StringBuilder agreementDate, ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp,
           ref StringBuilder msgCodeResp, ref StringBuilder respCodeResp, ref StringBuilder respMessageResp, ref StringBuilder cardTypeResp,
           ref StringBuilder accNumResp, ref StringBuilder refNumResp, ref StringBuilder authCodeResp, ref StringBuilder batchNumResp,
           ref StringBuilder amountResp, ref StringBuilder msgOptResp, ref StringBuilder foreignAmountResp, ref StringBuilder foreignCurrencyCodeResp,
           ref StringBuilder exchangeRateInclMarkupResp, ref StringBuilder eftTidResp);
        bool fullReconciliation(ref StringBuilder sessionId, ref StringBuilder msgType, ref StringBuilder msgCode,
           ref StringBuilder uniqueTxnId, ref StringBuilder batchNum, ref StringBuilder reconciliationMode,
           ref StringBuilder sessionIdResp, ref StringBuilder msgTypeResp, ref StringBuilder msgCodeResp,
           ref StringBuilder respCodeResp, ref StringBuilder respMessageResp, ref StringBuilder batchNumResp,
           ref StringBuilder reconciliationModeResp, ref StringBuilder batchNetAmountResp, ref StringBuilder batchOrigAmountResp,
           ref StringBuilder batchTotalCounterResp, ref StringBuilder batchTotalsResp, ref StringBuilder eftTidResp);
        void PortInitialization(EthernetDeviceSettings settings);
    }
}
