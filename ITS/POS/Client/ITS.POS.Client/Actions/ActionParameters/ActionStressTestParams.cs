using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Actions.ActionParameters
{
    public class ActionStressTestParams : ActionParams
    {
        public int NumberOfReceipts { get; protected set; }
        public int ItemsPerReceipt { get; protected set; }
        public bool RandomCustomer { get; protected set; }
        public bool RandomPayment { get; protected set; }
        public bool RandomCancelLines { get; protected set; }
        public bool RandomCancelDocument { get; protected set; }
        public bool RandomProforma { get; protected set; }

        public override eActions ActionCode
        {
            get { return eActions.STRESS_TEST; }
        }

        public ActionStressTestParams(int numberOfReceipts,int itemsPerReceipt,bool randomCustomer, bool randomPayment,bool randomCancelLines ,
                                        bool randomCancelDocument, bool randomProforma)
        {
            this.NumberOfReceipts = numberOfReceipts;
            this.ItemsPerReceipt = itemsPerReceipt;
            this.RandomCustomer = randomCustomer;
            this.RandomPayment = randomPayment;
            this.RandomCancelLines = randomCancelLines;
            this.RandomCancelDocument = randomCancelDocument;
            this.RandomProforma = randomProforma;
        }
    }
}
