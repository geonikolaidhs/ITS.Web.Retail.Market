using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ITS.POS.Hardware.Common
{

    public enum COM_PORT
    {

        NONE = -1,
        COM1 = 0,
        COM2 = 1,
        COM3 = 2,
        COM4, COM5, COM6, COM7,
        COM8, COM9, COM10, COM11,
        COM12, COM13, COM14, COM15,
        COM16, COM17, COM18, COM19,
        COM20, COM21, COM22, COM23,
        COM24, COM25, COM26, COM27,
        COM28, COM29, COM30, COM31,
        COM32
    }

    public enum BAUD_RATE
    {
        B_9600 = 9600,
        B_19200 = 19200,
        B_115200 = 115200,
        B_DEFAULT = 9600,
    }

    public enum TXN_TYPE
    {
        TXN_SALE = 49,
        TXN_REFUND = 50,
        TXN_VOID = 52,
        TXN_OFFLINE = 56,
        TXN_TOTALS_INQ = 57,
        TXN_BATCH_CLOSE = 53,
        TXN_PING = 48,
        TXN_ADJUST = 65,
        TXN_LOY_INQ = 73,
        TXN_LOY_RDM = 82,
        TXN_LOY_VD_RDM = 86,
    }

    public enum INST_PARTNER
    {
        INST_P_CUSTOMER = 0,
        INST_P_MERCHANT = 1,
        INST_P_ACQUIRER = 2,
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct tagEMV_DATA
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool bEmvUsed;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 33)]
        public string szAppID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string szAppName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 22)]
        public string szCrypto;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct tagLOY_DATA
    {
        [MarshalAs(UnmanagedType.I1)]
        public bool bLoyUsed;
        public byte schemeID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
        public string szLoyRespCode;
        public ulong awardedPoints;
        public ulong consumedPoints;
        public ulong loyBalance;
        public ulong merchantPoints;
        public ulong masterMerchantPoints;
        public ulong adjustedAmount;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct tagPOS_STATIC_DATA
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string szMerchantID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string szTerminalID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string szTermVersion;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct tagBATCH_TOTALS
    {
        public int iBatchNo;
        public int iNumSales;
        public ulong llAmtSales;
        public int iNumVoidSales;
        public ulong llAmtVoidSales;
        public int iNumRefunds;
        public ulong llAmtRefunds;
        public int iNumVoidRefunds;
        public ulong llAmtVoidRefunds;
        [MarshalAs(UnmanagedType.I1)]
        public bool bMismatch;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct tagBATCH_ENTRY
    {
        public int iReceiptNo;
        public byte eTxnType;
        public ulong llAmount;
        public ulong llTipAmount;
        public int iInstallments;
        public int iPostDating;
        public byte eInstPart;
        public int iOnTopAmount;
        public int iSTAN;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
        public string szAuthID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string szBankID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string szTimeStamp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string szPAN;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string szCardProduct;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string szTRM;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string szCashier;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct tagTXN_REQUEST
    {
        public int iReceiptNo;
        public int iECRRefNo;
        public byte eTxnType;
        public ulong llAmount;
        public ulong llTipAmount;
        public int iInstallments;
        public int iPostDating;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string szCashier;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
        public string szAuthID;
        public int schemeID;
        public ulong loyRdmAmt;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct tagTXN_RESPONSE
    {
        public int iReceiptNo;
        public byte eTxnType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
        public string szRespCode;
        public int iBatchNo;
        public ulong llTxnID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string szTimeStamp;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string szRRN;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 7)]
        public string szAuthID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string szBankID;
        public int iOnTopAmount;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string szPAN;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string szCardProduct;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 28)]
        public string szCardHolder;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string szTRM;
        public tagEMV_DATA strEMV_Data;
        public tagLOY_DATA strLOY_Data;
    }

    public static class EdpsLink
    {
        public const string RC_OK = "00";
        public static Dictionary<string, string> ErrorCodes = new Dictionary<string, string>()
        {
            { "00","RC_OΚ" },
            { "01","ΚΛΗΣΗ ΚΕΝΤΡΟΥ ΕΓΚΡ." },
            { "02","ΚΛΗΣΗ ΚΕΝΤΡΟΥ ΕΓΚΡ.(02)" },
            { "03","ΛΑΘΟΣ ΚΩΔ. ΕΠΙΧΕΙΡ" },
            { "04","ΚΡΑΤΗΣΤΕ ΤΗΝ ΚΑΡΤΑ" },
            { "05","ΔΕΝ ΕΓΚΡΙΝΕΤΑΙ (05)" },
            //{ "06","ΔΕΝ ΕΓΚΡΙΝΕΤΑΙ (06)" },
            //{ "07","ΚΡΑΤΗΣΤΕ ΤΗΝ ΚΑΡΤΑ" },
            { "13","ΛΑΘΟΣ ΠΟΣΟ" },
            { "14","ΛΑΘΟΣ ΡΑΝ" },
            { "19","ΕΠΑΝΑΛΑΒΕΤΕ ΣΥΝΑΛΛΑΓΗ" },
            { "25","ΚΑΛΕΣΤΕ ΤΗΛΕΦΩΝΙΚΟ ΚΕΝΤΡΟ (25)" },
            { "30","ΚΑΛΕΣΤΕ ΤΗΛΕΦΩΝΙΚΟ ΚΕΝΤΡΟ (30)" },
            { "31","ΚΑΛΕΣΤΕ ΤΗΛΕΦΩΝΙΚΟ ΚΕΝΤΡΟ (31)" },
            { "33","RC_KEEP_CARD_33" },
            { "41","ΚΡΑΤΗΣΤΕ ΤΗΝ ΚΑΡΤΑ (41)" },
            { "43","ΚΡΑΤΗΣΤΕ ΤΗΝ ΚΑΡΤΑ (43)" },
            { "51","ΔΕΝ ΕΓΚΡΙΝΕΤΑΙ (51)" },
            { "54","Η ΚΑΡΤΑ ΕΧΕΙ ΛΗΞΕΙ (54)" },
            { "55","ΛΑΘΟΣ ΡΙΝ Η CVV (55)" },
            { "58","ΛΑΘΟΣ TXN (58)" },
            { "60","ΚΑΛΕΣΤΕ ΤΗΛΕΦΩΝΙΚΟ ΚΕΝΤΡΟ (60)" },
            { "61","ΔΕΝ ΕΓΚΡΙΝΕΤΑΙ (61)" },
            { "62","Η ΚΑΡΤΑ ΔΕΝ ΥΠΟΣΤΗΡΙΖΕΙ ΔΟΣΕΙΣ" },
            { "63","Η ΚΑΡΤΑ ΔΕΝ ΥΠΟΣΤΗΡΙΖΕΙ ΜΕΤΑΧΡΟΝΟΛΟΓΗΜΕΝΕΣ ΔΟΣΕΙΣ" },
            { "64","ΤΟ ΠΟΣΟ ΤΩΝ ΔΟΣΕΩΝ ΕΙΝΑΙ ΧΑΜΗΛΟ (64)" },
            { "65","ΛΑΘΟΣ ΜΕΤΑΧΡΟΝΟΛΟΓΗΣΗ " },
            { "66","Η ΜΕΤΑΧΡΟΝΟΛΟΓΗΣΗ ΔΕΝ ΥΠΟΣΤΗΡΙΖΕΤΑΙ" },
            { "75","ΛΑΘΟΣ PIN (75)" },
            { "76","ΚΑΛΕΣΤΕ ΤΗΛΕΦΩΝΙΚΟ ΚΕΝΤΡΟ (76)" },
            { "77","ΑΣΥΜΦΩΝΙΑ ΠΑΚΕΤΟΥ" },
            { "78","ΛΑΘΟΣ TXN (78)" },
            { "79","ΑΠΟΡΡΙΨΗ ΛΟΓΩ CVV2" },
            { "80","ΛΑΘΟΣ Α/Α ΠΑΚΕΤΟΥ" },
            { "82","No closed SOC slots" },
            { "83","No susp SOC slots" },
            { "85","ΑΝΥΠΑΡΚΤΗ ΑΠΟΔΕΙΞΗ" },
            { "89","ΛΑΘΟΣ ΚΩΔΙΚΟΣ ΤΕΡΜΑΤΙΚΟΥ" },
            { "91","ΠΡΟΣΩΡΙΝΑ ΑΔΥΝΑΤΗ" },
            { "95","ΜΕΤΑΦΟΡΑ ΠΑΚΕΤΟΥ..." },
            { "96","RC_BATCH_UPLOAD" },
            { "97","Wrong crypto" },
            { "98","RC_INVALID_INSTALLMENTS_VAL" },
            { "C1","ΜΗ ΑΠΟΔΕΚΤΗ ΚΑΡΤΑ (ΠΑΤΗΘΗΚΕ ΤΟ STOP)" },
            { "C2","card entered but not accepted by terminal" },
            { "C3","card has passed its expiry date" },
            { "C4","card entered, but transaction cancelled later on" },
            { "Z1","error setting up TCP connection with host" },
            { "Z2","unable to setup TCP connection within specified time" },
            { "Z3","TCP connection ok, host didn't reply promptly" },
            { "Z7","response cryptogram validation error" },
            { "Z9","empty batch - NOT AN ERROR" },
            { "ZΕ","undefined connection error, after sending message" }
            };
        [DllImport("epos_xenta.dll", EntryPoint = "_EDPOS_InitComm", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int EDPOS_InitComm(COM_PORT port, BAUD_RATE baud, int timeout_sec, ref tagPOS_STATIC_DATA cfgPOS);

        [DllImport("epos_xenta.dll", EntryPoint = "_EDPOS_CloseComm", CallingConvention = CallingConvention.Cdecl)]
        public static extern void EDPOS_CloseComm();

        [DllImport("epos_xenta.dll", EntryPoint = "_EDPOS_DoTestComm", CallingConvention = CallingConvention.Cdecl)]
        public static extern int EDPOS_DoTestComm();

        [DllImport("epos_xenta.dll", EntryPoint = "_EDPOS_DoTransaction", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int EDPOS_DoTransaction(ref tagTXN_REQUEST req, ref tagTXN_RESPONSE resp, byte useTermLPR);


        [DllImport("epos_xenta.dll", EntryPoint = "_EDPOS_DoLoyTransaction", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int EDPOS_DoLoyTransaction(ref tagTXN_REQUEST req, ref tagTXN_RESPONSE resp, char useTermLPR);

        [DllImport("epos_xenta.dll", EntryPoint = "_EDPOS_AskBatchTotals", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int EDPOS_AskBatchTotals(ref tagBATCH_TOTALS totals);

        [DllImport("epos_xenta.dll", EntryPoint = "_EDPOS_DoBatchClose_arr", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int EDPOS_DoBatchClose_arr(
            IntPtr totals,
            IntPtr p_txnLog,
            IntPtr size,
            IntPtr rc,
            IntPtr authID,
            [MarshalAs(UnmanagedType.I1)] bool TermPrinter);

        //(ref tagBATCH_TOTALS totals, tagBATCH_ENTRY[] p_txnLog, ref int size, byte[] rc, byte[] authID, [MarshalAs(UnmanagedType.I1)] bool TermPrinter);

    }
}
