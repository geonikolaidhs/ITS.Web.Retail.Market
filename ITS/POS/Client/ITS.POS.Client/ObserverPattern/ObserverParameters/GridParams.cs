using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Transactions;
using ITS.POS.Client.UserControls;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
	public class GridParams : ObserverParams
	{
		public DocumentHeader DocumentHeader { get; set; }
		public DocumentDetail SelectedDocumentDetail { get; set; }
		public DocumentPayment SelectedDocumentPayment { get; set; }
		public eDisplayMode? DisplayMode { get; set; }
		public eNavigation? Navigation { get; set; }

		public GridParams()
		{
			//Use this constructor to clear the grid
		}

		public GridParams(DocumentHeader header,eDisplayMode displayMode)
		{
			this.DocumentHeader = header;
			this.DisplayMode = displayMode;
		}

		public GridParams(DocumentDetail selectedDocumentDetail)
		{
			this.SelectedDocumentDetail = selectedDocumentDetail;
		}

		public GridParams(DocumentPayment selectedDocumentPayment)
		{
			this.SelectedDocumentPayment = selectedDocumentPayment;
		}

		public GridParams(eNavigation navigation)
		{
			Navigation = navigation;
		}


        public override Type GetObserverType()
        {
            return typeof(IObserverGrid);
        }
    }
}
