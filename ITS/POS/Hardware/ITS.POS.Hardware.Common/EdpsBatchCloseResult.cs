using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Hardware.Common
{
    public class EdpsBatchCloseResult
    {
        public EdpsBatchCloseResult()
        {
            Transactions = new List<EdpsBatchTransaction>();
        }

        public EdpsBatchCloseResult(tagBATCH_TOTALS entry, tagBATCH_ENTRY[] transactions, int size)
        {
            Transactions = new List<EdpsBatchTransaction>();
            BatchNumber = entry.iBatchNo;
            NumberOfSales = entry.iNumSales;
            AmountSales = entry.llAmtSales/100.0m;
            NumberOfVoidSales = entry.iNumVoidSales;
            AmountOfVoidSales = entry.llAmtVoidSales / 100.0m;
            NumberOfRefunds = entry.iNumRefunds;
            AmountOfRefunds = entry.llAmtRefunds / 100.0m;
            NumberOfVoidRefunds = entry.iNumVoidRefunds;
            AmountOfVoidRefunds = entry.llAmtVoidRefunds / 100.0m;
            bMismatch = entry.bMismatch;
            for (int i = 0; i < size; i++)
            {
                Transactions.Add(new EdpsBatchTransaction(transactions[i]));
            }
        }

        public int BatchNumber { get; set; }
        public int NumberOfSales { get; set; }
        public decimal AmountSales { get; set; }
        public int NumberOfVoidSales { get; set; }
        public decimal AmountOfVoidSales { get; set; }
        public int NumberOfRefunds { get; set; }
        public decimal AmountOfRefunds { get; set; }
        public int NumberOfVoidRefunds { get; set; }
        public decimal AmountOfVoidRefunds { get; set; }
        public bool bMismatch { get; set; }

        public List<EdpsBatchTransaction> Transactions { get; private set; }
    }

    public class EdpsBatchTransaction
    {
        public EdpsBatchTransaction()
        {

        }

        public EdpsBatchTransaction(tagBATCH_ENTRY entry)
        {
            ReceiptNumber = entry.iReceiptNo;
            TransactionType = (int) entry.eTxnType;
            Amount = entry.llAmount / 100.0m;
            TipAmount = entry.llTipAmount / 100.0m;
            Installments = entry.iInstallments;
            PostDating = entry.iPostDating;
            eInstPart = (int)entry.eInstPart;
            OnTopAmount = entry.iOnTopAmount;
            STAN = entry.iSTAN;
            AuthID = entry.szAuthID;
            BankID = entry.szBankID;
            TimeStamp = entry.szTimeStamp;
            PAN = entry.szPAN;
            CardProduct = entry.szCardProduct;
            TRM = entry.szTRM;
            Cashier = entry.szCashier;
        }

        public int ReceiptNumber { get; set; }
        public int TransactionType { get; set; }
        public decimal Amount { get; set; }
        public decimal TipAmount { get; set; }
        public int Installments { get; set; }
        public int PostDating { get; set; }
        public int eInstPart { get; set; }
        public int OnTopAmount { get; set; }
        public int STAN { get; set; }
        public string AuthID { get; set; }
        public string BankID { get; set; }
        public string TimeStamp { get; set; }
        public string PAN { get; set; }
        public string CardProduct { get; set; }
        public string TRM { get; set; }
        public string Cashier { get; set; }
    }
}
