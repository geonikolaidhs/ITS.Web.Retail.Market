package gr.net.its.retail;

import java.sql.SQLException;
import java.util.List;
import java.util.UUID;

import android.content.Context;

import com.j256.ormlite.stmt.PreparedQuery;
import com.j256.ormlite.stmt.QueryBuilder;

import gr.net.its.common.Utilities;
import gr.net.its.retail.data.*;

public class DocumentHelper
{

	/*
	 * Checks if the selected barcode already exists in the document and returns
	 * the DocumentDetail if it's found. Else returns null
	 */
	// public static DocumentDetail checkIfBarcodeExists(DocumentHeader header,
	// Barcode barcode, DatabaseHelper dbHelper) throws SQLException
	// {
	// // reload if lazy loaded
	// if (header.getCustomer() == null)
	// {
	// header = dbHelper.getDocumentHeaders().queryForId(header.getID());
	// }
	// for (DocumentDetail detail : header.getDetails())
	// {
	// if (detail.getBarcode() == null)
	// {
	// detail = dbHelper.getDetails().queryForId(detail.getID());
	// }
	//
	// if (detail.getBarcode().equals(barcode.getCode()))
	// {
	// return detail;
	// }
	// }
	// return null;
	// }

	/*
	 * Checks if the selected item with the same price already exists in the
	 * document and returns the DocumentDetail if it's found. Else returns null
	 */
	public static DocumentDetail checkIfBarcodeExists(DocumentHeader header, Barcode barcode, Item item, DatabaseHelper dbHelper, ApplicationSettings appSettings) throws SQLException
	{
		// reload if lazy loaded
		if (header.getCustomer() == null)
		{
			header = dbHelper.getDocumentHeaders().queryForId(header.getID());
		}

		Customer customer = dbHelper.getCustomers().queryForId(header.getCustomer().getID());
		PriceCatalog pc = dbHelper.getPriceCatalogDao().queryForId(customer.getPc().getID());
		double price = dbHelper.getItemPrice(pc, barcode, item);
		price = Utilities.RoundToDigits(price, appSettings.getComputeDigits());
		PriceCatalogDetail pcd = dbHelper.getItemPriceDetail(pc, barcode, item);
		for (DocumentDetail detail : header.getDetails())
		{
			if (detail.getBarcode() == null)
			{
				detail = dbHelper.getDetails().queryForId(detail.getID());
			}

			if (detail.getItem().getID() == item.getID() && detail.getItemPrice() == price && detail.getFirstDiscount() == pcd.getDiscount())
			{
				return detail;
			}
		}
		return null;
	}

	public static DocumentDetail computeDocumentLine(Customer customer, Barcode barcode, double qty, double firstDiscount, double secondDiscount, DatabaseHelper dbHelper, boolean isLinkedLine, ApplicationSettings appSettings)
	{
		try
		{
			if (barcode.getCode() == null)
			{
				barcode = dbHelper.getBarcodes().queryForId(barcode.getID());
			}
			DocumentDetail detail = new DocumentDetail();
			detail.setRemoteDeviceDocumentDetailGuid(UUID.randomUUID().toString());
			// detail.setID(Long.randomLong());

			PriceCatalog pc = dbHelper.getPriceCatalogDao().queryForId(customer.getPc().getID());
			Item item = dbHelper.getItems().queryForId(barcode.getItem().getID());
			detail.setItem(item);
			detail.setBarcode(barcode.getCode());
			double price = dbHelper.getItemPrice(pc, barcode, item);
			price = Utilities.RoundToDigits(price, appSettings.getComputeDigits());
			detail.setItemPrice(price);
			// TODO : customer Discounts

			if (item.getVatCategory() == null && customer.getVl() == null)
			{
				return null;
			}

			VATCategory vatCat = dbHelper.getVATCategoryDao().queryForId(item.getVatCategory().getID());
			VATLevel vatLevel = dbHelper.getVATLevelDao().queryForId(customer.getVl().getID());
			VATCategory cat = dbHelper.getVATCategoryDao().queryForId(item.getVatCategory().getID());
			QueryBuilder<VATFactor, Long> qb = dbHelper.getVATFactorDao().queryBuilder();
			qb.where().eq("VatLevel_id", vatLevel.getID()).and().eq("VatCategory_id", vatCat.getID());
			PreparedQuery<VATFactor> pq = qb.prepare();
			VATFactor factor = dbHelper.getVATFactorDao().queryForFirst(pq);

			detail.setVatFactor(factor.getVatFactor());
			detail.setQty(qty);
			detail.setFirstDiscount(firstDiscount);
			detail.setSecondDiscount(secondDiscount);
			double afterFirstDiscount = detail.getItemPrice() * (1 - firstDiscount);
			double afterSecondDiscount = afterFirstDiscount * (1 - secondDiscount);
			double afterDocumentDiscount = afterSecondDiscount;
			detail.setVatAmount(afterDocumentDiscount * detail.getVatFactor());
			detail.setFinalUnitPrice(afterDocumentDiscount);
			detail.setTotalDiscount(qty * (detail.getItemPrice() - detail.getFinalUnitPrice()));
			detail.setNetTotal(qty * detail.getItemPrice());
			detail.setNetTotalAfterDiscount(detail.getNetTotal() - detail.getTotalDiscount());
			detail.setTotalVatAmount(detail.getNetTotalAfterDiscount() * detail.getVatFactor());
			detail.setGrossTotal(detail.getNetTotalAfterDiscount() + detail.getTotalVatAmount());

			dbHelper.getDetails().create(detail);
			return detail;
		}
		catch (Exception e)
		{
			e.printStackTrace();
			return null;
		}
	}

	public static DocumentHeader AddLinkedItems(DocumentHeader documentHeader, DocumentDetail documentDetail, DatabaseHelper dbHelper, ApplicationSettings appSettings) throws Exception
	{
		PriceCatalog currentPriceCatalog = documentHeader.getCustomer().getPc();

		QueryBuilder<LinkedItem, Long> qb = dbHelper.getLinkedItemDao().queryBuilder();
		qb.where().eq("itemGuid", documentDetail.getItem().getRemoteOid().toString());
		PreparedQuery<LinkedItem> pq = qb.prepare();
		List<LinkedItem> linkedItems = dbHelper.getLinkedItemDao().query(pq);

		for (LinkedItem linked_item : linkedItems)
		{

			Item subItem = dbHelper.getItemByRemoteGuid(linked_item.getSubItemGuid());
			// .getItems().queryForId(Long.fromString(linked_item.getSubItemGuid()));
			PriceCatalogDetail tempPriceCatalogDetail = null;

			if (subItem.getDefaultBarcodeObject() != null)
			{
				Barcode bc = subItem.getDefaultBarcodeObject();
				if (bc.getCode() == null)
				{
					bc = dbHelper.getBarcodes().queryForId(bc.getID());
				}
				tempPriceCatalogDetail = dbHelper.getItemPriceDetail(currentPriceCatalog, bc, subItem);
			}
			if (tempPriceCatalogDetail == null && subItem.getCode() != null)
			{
				Barcode bcFromItemCode = dbHelper.getBarcodeFromItemCode(subItem, appSettings);
				tempPriceCatalogDetail = dbHelper.getItemPriceDetail(currentPriceCatalog, bcFromItemCode, subItem);
			}

			if (tempPriceCatalogDetail == null)
			{
				// Προσθήκη του συνδεδεμένου είδους με μηδενική τιμή
				double discount = tempPriceCatalogDetail == null ? .0 : tempPriceCatalogDetail.getDiscount();
				Barcode search_barcode = subItem.getDefaultBarcodeObject();
				if (search_barcode == null)
				{
					String search_code_str = subItem.getCode();
					if (appSettings.isBarcodePadding())
					{
						search_code_str = RetailHelper.GetPaddedBarcode(search_code_str, appSettings);
					}
					search_barcode = dbHelper.<Barcode> findObject(Barcode.class, "Code", search_code_str, FilterType.EQUALS);
				}

				if (search_barcode == null)
				{
					// δε μπορώ να βρω τιμή με τίποτα οπότε τέλος
					DeleteItem(documentHeader, documentDetail, dbHelper);
					throw new Exception();
				}
				DocumentDetail tempDocumentDetail = computeDocumentLine(documentHeader.getCustomer(), search_barcode, documentDetail.getQty(), 0, 0, dbHelper, true, appSettings);
				tempDocumentDetail.setLinkedLine(documentDetail);
				if (tempDocumentDetail.getItemPrice() < 0)
				{
					tempDocumentDetail.setItemPrice(0);
				}
				tempDocumentDetail.setHeader(documentHeader);
				dbHelper.getDetails().createOrUpdate(tempDocumentDetail);
				return documentHeader;
			}
			DocumentDetail tempDocumentDetail2 = computeDocumentLine(documentHeader.getCustomer(), tempPriceCatalogDetail.getBc(), documentDetail.getQty(), tempPriceCatalogDetail.getDiscount(), 0, dbHelper, true, appSettings);
			tempDocumentDetail2.setLinkedLine(documentDetail);
			if (tempDocumentDetail2.getItemPrice() < 0)
			{
				tempDocumentDetail2.setItemPrice(0);
			}
			tempDocumentDetail2.setHeader(documentHeader);
			dbHelper.getDetails().createOrUpdate(tempDocumentDetail2);
		}
		return documentHeader;
	}

	public static DocumentHeader AddItem(DocumentHeader documentHeader, DocumentDetail documentDetail, DatabaseHelper dbHelper, ApplicationSettings settings)
	{
		try
		{
			documentDetail.setHeader(documentHeader);
			dbHelper.getDetails().createOrUpdate(documentDetail);
			return AddLinkedItems(documentHeader, documentDetail, dbHelper, settings);
		}
		catch (Exception e)
		{
			return null;
		}
	}

	public static DocumentHeader DeleteItem(DocumentHeader documentHeader, DocumentDetail documentDetail, DatabaseHelper dbHelper)
	{
		DeleteLinkedItems(documentHeader, documentDetail, dbHelper);
		try
		{
			dbHelper.getDetails().delete(documentDetail);
		}
		catch (SQLException e)
		{
			e.printStackTrace();
			return null;
		}
		return documentHeader;
	}

	public static DocumentHeader DeleteLinkedItems(DocumentHeader documentHeader, DocumentDetail documentDetail, DatabaseHelper dbHelper)
	{
		try
		{
			for (DocumentDetail detail : documentHeader.getDetails())
			{
				if (detail.getLinkedLine() != null && detail.getLinkedLine().getID() == (documentDetail.getID()))
				{
					dbHelper.getDetails().delete(detail);
				}
			}
			return documentHeader;
		}
		catch (Exception e)
		{
			e.printStackTrace();
			return null;
		}
	}

	public static DocumentHeader ReplaceItem(DocumentHeader documentHeader, DocumentDetail old_value, DocumentDetail new_value, DatabaseHelper dbHelper, ApplicationSettings settings)
	{
		DeleteItem(documentHeader, old_value, dbHelper);
		AddItem(documentHeader, new_value, dbHelper, settings);
		return documentHeader;
	}

	public static DocumentDetail ChangeDetail(DocumentHeader documentHeader, DocumentDetail detail, double newQty, double newSecondDiscount, DatabaseHelper dbHelper, boolean isLinkedDetail, ApplicationSettings appSettings, Context context) throws Exception
	{
		try
		{
			Barcode bc = dbHelper.findObject(Barcode.class, "Code", detail.getBarcode(), FilterType.EQUALS);
			Item currentItem = detail.getItem();
			if (currentItem.getCode() == null)
			{
				currentItem = dbHelper.getItems().queryForId(currentItem.getID());
			}
//			if (currentItem.getMaxOrderQty() > 0 && currentItem.getMaxOrderQty() < newQty)
//			{
//				throw new Exception(context.getResources().getString(R.string.invalidQty) + "\n" + context.getResources().getString(R.string.maxOrderQtyExceeded).replace("var1", "" + currentItem.getMaxOrderQty()));
//			}

			DocumentDetail dummyDetail = computeDocumentLine(documentHeader.getCustomer(), bc, newQty, detail.getFirstDiscount(), newSecondDiscount, dbHelper, isLinkedDetail, appSettings);
			detail.setFinalUnitPrice(dummyDetail.getFinalUnitPrice());
			detail.setFirstDiscount(dummyDetail.getFirstDiscount());
			detail.setGrossTotal(dummyDetail.getGrossTotal());
			detail.setNetTotal(dummyDetail.getNetTotal());
			detail.setNetTotalAfterDiscount(dummyDetail.getNetTotalAfterDiscount());
			detail.setQty(dummyDetail.getQty());
			detail.setSecondDiscount(dummyDetail.getSecondDiscount());
			detail.setTotalDiscount(dummyDetail.getTotalDiscount());
			detail.setTotalVatAmount(dummyDetail.getTotalVatAmount());
			detail.setUnitPriceAfterDiscount(dummyDetail.getUnitPriceAfterDiscount());
			detail.setVatAmount(dummyDetail.getVatAmount());
			detail.setVatFactor(dummyDetail.getVatFactor());
			dbHelper.getDetails().update(detail);
			QueryBuilder<DocumentDetail, Long> qb = dbHelper.getDetails().queryBuilder();
			qb.where().eq("linkedLine_id", detail.getID());
			PreparedQuery<DocumentDetail> pq = qb.prepare();
			List<DocumentDetail> linkedDetails = dbHelper.getDetails().query(pq);
			for (DocumentDetail linkedDetail : linkedDetails)
			{
				ChangeDetail(documentHeader, linkedDetail, newQty, newSecondDiscount, dbHelper, true, appSettings, context);
			}
			return detail;
		}
		catch (Exception e)
		{
			e.printStackTrace();
			// return null;
			throw e;
		}

	}

	public static DocumentDetail ChangeDetailSecondDiscount(DocumentHeader documentHeader, DocumentDetail detail, double newDiscount, DatabaseHelper dbHelper, boolean isLinkedDetail, ApplicationSettings appSettings)
	{
		try
		{
			Barcode bc = dbHelper.findObject(Barcode.class, "Code", detail.getBarcode(), FilterType.EQUALS);
			DocumentDetail dummyDetail = computeDocumentLine(documentHeader.getCustomer(), bc, detail.getQty(), detail.getFirstDiscount(), newDiscount, dbHelper, isLinkedDetail, appSettings);
			detail.setFinalUnitPrice(dummyDetail.getFinalUnitPrice());
			detail.setFirstDiscount(dummyDetail.getFirstDiscount());
			detail.setGrossTotal(dummyDetail.getGrossTotal());
			detail.setNetTotal(dummyDetail.getNetTotal());
			detail.setNetTotalAfterDiscount(dummyDetail.getNetTotalAfterDiscount());
			detail.setQty(dummyDetail.getQty());
			detail.setSecondDiscount(dummyDetail.getSecondDiscount());
			detail.setTotalDiscount(dummyDetail.getTotalDiscount());
			detail.setTotalVatAmount(dummyDetail.getTotalVatAmount());
			detail.setUnitPriceAfterDiscount(dummyDetail.getUnitPriceAfterDiscount());
			detail.setVatAmount(dummyDetail.getVatAmount());
			detail.setVatFactor(dummyDetail.getVatFactor());
			dbHelper.getDetails().update(detail);
			return detail;
		}
		catch (Exception e)
		{
			e.printStackTrace();
			return null;
		}

	}
}
