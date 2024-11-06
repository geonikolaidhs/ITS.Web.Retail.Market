package gr.net.its.retail;

import java.text.DecimalFormat;
import java.text.NumberFormat;
import java.util.ArrayList;
import java.util.List;

import gr.net.its.common.Utilities;
import gr.net.its.retail.data.*;

public class RetailHelper
{
	// public static DecimalFormat euro[] = {new DecimalFormat("#,##0 €"),new
	// DecimalFormat("#,##0.0 €"),new DecimalFormat("#,##0.00 €"),new
	// DecimalFormat("#,##0.000 €"),new DecimalFormat("#,##0.0000 €")};
	public static DecimalFormat euroNormal, euroValue;
	public static DecimalFormat qtyFormat = new DecimalFormat("#,##0.##");
	public static NumberFormat percent = NumberFormat.getPercentInstance();

	public static String PrepareXmlToSend(DocumentHeader docHeader, DatabaseHelper dbHelper)
	{

		String toReturn = "";
		try
		{
			// Force reload
			if (docHeader.getCustomer() == null)
			{
				docHeader = dbHelper.getDocumentHeaders().queryForId(docHeader.getID());
			}
			DocumentStatus status = dbHelper.getDocumentStatuses().queryForId(docHeader.getDocumentStatus().getID());
			toReturn = "<?xml version=\"1.0\"?><request  errorcode=\"\" errordescr=\"\">";
			toReturn += "<Header>";
			toReturn += "<item id=\"Status\">" + status.getRemoteGuid() + "</item>"; // TODO
			toReturn += "<item id=\"RemoteDeviceDocumentHeaderGuid\">" + docHeader.getRemoteDeviceDocumentHeaderGuid() + "</item>"; // TODO

			toReturn += "<item id=\"Division\">0</item>";
			toReturn += "<item id=\"companyid\">notreq</item>";
			toReturn += "<item id=\"storeid\">notreq</item>";
			toReturn += "<item id=\"User\">" + docHeader.getCreatedBy() + "</item>";
			toReturn += "<item id=\"customerid\">" + docHeader.getCustomer().getRemoteGuid() + "</item>";
			toReturn += "<item id=\"deliveryAddress\">" + ReplaceInvalidXmlCharacters(docHeader.getDeliveryAddress()) + "</item>";
			toReturn += "<item id=\"finalizeDate\">" + docHeader.getDocumentDate() + "</item>";
			toReturn += "<item id=\"comments\">" + ReplaceInvalidXmlCharacters(docHeader.getComments() == null ? "" : docHeader.getComments()) + "</item>";
			toReturn += "</Header>";
			for (int i = 0; i < docHeader.getDetails().size(); i++)
			{
				DocumentDetail det = (DocumentDetail) (docHeader.getDetails().toArray()[i]);
				if (det.getLinkedLine() == null)
				{
					toReturn += "<lines>";
					toReturn += "<RemoteDeviceDocumentDetailGuid>" + det.getRemoteDeviceDocumentDetailGuid() + "</RemoteDeviceDocumentDetailGuid>";
					toReturn += "<item>" + det.getBarcode() + "</item>";
					toReturn += "<qty>" + ((int) (det.getQty() * 1000)) + "</qty>";
					toReturn += "<discount>" + ((int) (det.getSecondDiscount() * 1000)) + "</discount>";
					toReturn += "<unitPrice>" + det.getItemPrice() * 10000 + "</unitPrice>";
					toReturn += "</lines>";
				}
			}
			toReturn += "</request>";
			return toReturn;
		}
		catch (Exception e)
		{
			e.printStackTrace();
		}

		return "";
	}

	public static String ReplaceInvalidXmlCharacters(String input)
	{
		String output = input.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;").replace("\"", "&quot;");
		return output;
	}

	public static String GetPaddedBarcode(String barcode, ApplicationSettings settings)
	{
		if (settings.isBarcodePadding())
		{
			char paddingChar = settings.getBarcodePadChar();
			int paddingLength = settings.getBarcodePadLength();
			return Utilities.PadLeft(barcode, paddingLength, paddingChar);
		}
		else
		{
			return barcode;
		}
	}

	public static String GetPaddedItemCode(String itemCode, ApplicationSettings settings)
	{
		if (settings.isCodePadding())
		{
			char paddingChar = settings.getCodePadChar();
			int paddingLength = settings.getCodePadLength();
			return Utilities.PadLeft(itemCode, paddingLength, paddingChar);
		}
		else
		{
			return itemCode;

		}
	}

	public static double GetItemExistingQuantity(DocumentHeader order, Item item, DatabaseHelper dbHelper)
	{
		int qty = 0;
		try
		{
			// reload if lazy loaded
			if (order.getCustomer() == null)
			{
				order = dbHelper.getDocumentHeaders().queryForId(order.getID());
			}

			for (DocumentDetail detail : order.getDetails())
			{

				if (detail.getItem() == null)
				{
					detail = dbHelper.getDetails().queryForId(detail.getID());
				}

				Item detailItem = detail.getItem();
				if (detailItem.getID() == item.getID())
				{
					if (detailItem.getCode() == null)
					{
						detailItem = dbHelper.getItems().queryForId(detailItem.getID());
					}
					qty += detail.getQty();
				}
			}
		}

		catch (Exception e)
		{
			e.printStackTrace();
			return -1;
		}

		return qty;
	}

	public static List<Item> GetOfferItems(Offer offer, DatabaseHelper dbHelper, String filter)
	{
		List<Item> itemsOfOffer = new ArrayList<Item>();
		for (OfferDetail detail : offer.getOfferDetails())
		{
			Item itm = detail.getItem();
			try
			{
				
				if (itm.getLoweCaseName() == null)
				{
					itm = dbHelper.getItems().queryForId(itm.getID());
				}
				if (filter != null && filter != "")
				{
					
					if (itm.getLoweCaseName().indexOf(filter.toLowerCase()) < 0 && itm.getCode().indexOf(filter) < 0)
						continue;
				}
			}
			catch (Exception ex)
			{
				ex.printStackTrace();
			}
			if(itm!=null)
			{
				itemsOfOffer.add(itm);
			}
		}

		return itemsOfOffer;
	}

	public static void GetItemsExistingQuantity(List<ItemPrice> items, DocumentHeader order, DatabaseHelper dbHelper)
	{
		if(items == null)
		{
			return;
		}
		for (ItemPrice item : items)
		{
			item.qty = GetItemExistingQuantity(order, item.item, dbHelper);
		}
	}

}
