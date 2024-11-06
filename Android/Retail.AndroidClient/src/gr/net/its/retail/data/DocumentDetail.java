package gr.net.its.retail.data;


import com.j256.ormlite.dao.ForeignCollection;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class DocumentDetail
{
	@DatabaseField( generatedId=true)
	private long ID;
	@DatabaseField(foreign = true)
	private Item item;
	@DatabaseField
	private boolean EditOffline;
	@DatabaseField
	private String barcode;
	@DatabaseField
	private double Qty;
	@DatabaseField
	private double ItemPrice;
	@DatabaseField
	private double FinalUnitPrice;
	@DatabaseField
	private double GrossTotal;
	@DatabaseField
	private double NetTotal;
	@DatabaseField
	private double NetTotalAfterDiscount;
	@DatabaseField
	private double FirstDiscount;
	@DatabaseField
	private double SecondDiscount;
	@DatabaseField
	private double TotalDiscount;
	@DatabaseField
	private double TotalVatAmount;
	@DatabaseField
	private double UnitPriceAfterDiscount;
	@DatabaseField
	private double VatAmount;
	@DatabaseField
	private double VatFactor;
	@DatabaseField(foreign = true, foreignAutoRefresh = true)
	private DocumentHeader header;
	@DatabaseField(foreign = true, foreignAutoRefresh = true)
	private DocumentDetail linkedLine;
	
	@DatabaseField
	private String RemoteDeviceDocumentDetailGuid;
	
	

	public String getRemoteDeviceDocumentDetailGuid()
	{
		return RemoteDeviceDocumentDetailGuid;
	}

	public void setRemoteDeviceDocumentDetailGuid(String remoteDeviceDocumentDetailGuid)
	{
		RemoteDeviceDocumentDetailGuid = remoteDeviceDocumentDetailGuid;
	}
	
	public DocumentDetail getLinkedLine()
	{
	    return linkedLine;
	}

	public void setLinkedLine(DocumentDetail linkedLine)
	{
	    this.linkedLine = linkedLine;
	}

	public long getCreatedOn()
	{
	    return CreatedOn;
	}

	public void setCreatedOn(long createdOn)
	{
	    CreatedOn = createdOn;
	}

	public long getUpdatedOn()
	{
	    return UpdatedOn;
	}

	public void setUpdatedOn(long updatedOn)
	{
	    UpdatedOn = updatedOn;
	}

	@DatabaseField
	private long CreatedOn;
	@DatabaseField
	private long UpdatedOn;
	@ForeignCollectionField
	private ForeignCollection<DocumentDetail> ReferenceDocumentDetails;

	public long getID()
	{
		return ID;
	}

	public void setID(long iD)
	{
		ID = iD;
	}

	public Item getItem()
	{
		return item;
	}

	public void setItem(Item item)
	{
		this.item = item;
	}

	public boolean isEditOffline()
	{
		return EditOffline;
	}

	public void setEditOffline(boolean editOffline)
	{
		EditOffline = editOffline;
	}

	public String getBarcode()
	{
		return barcode;
	}

	public void setBarcode(String barcode)
	{
		this.barcode = barcode;
	}

	public double getQty()
	{
		return Qty;
	}

	public void setQty(double qty)
	{
		Qty = qty;
	}

	public double getItemPrice()
	{
		return ItemPrice;
	}

	public void setItemPrice(double itemPrice)
	{
		ItemPrice = itemPrice;
	}

	public double getFinalUnitPrice()
	{
		return FinalUnitPrice;
	}

	public void setFinalUnitPrice(double finalUnitPrice)
	{
		FinalUnitPrice = finalUnitPrice;
	}

	public double getGrossTotal()
	{
		return GrossTotal;
	}

	public void setGrossTotal(double grossTotal)
	{
		GrossTotal = grossTotal;
	}

	public double getNetTotal()
	{
		return NetTotal;
	}

	public void setNetTotal(double netTotal)
	{
		NetTotal = netTotal;
	}

	public double getNetTotalAfterDiscount()
	{
		return NetTotalAfterDiscount;
	}

	public void setNetTotalAfterDiscount(double netTotalAfterDiscount)
	{
		NetTotalAfterDiscount = netTotalAfterDiscount;
	}

	public double getFirstDiscount()
	{
		return FirstDiscount;
	}

	public void setFirstDiscount(double firstDiscount)
	{
		FirstDiscount = firstDiscount;
	}

	public double getSecondDiscount()
	{
		return SecondDiscount;
	}

	public void setSecondDiscount(double secondDiscount)
	{
		SecondDiscount = secondDiscount;
	}

	public double getTotalDiscount()
	{
		return TotalDiscount;
	}

	public void setTotalDiscount(double totalDiscount)
	{
		TotalDiscount = totalDiscount;
	}

	public double getTotalVatAmount()
	{
		return TotalVatAmount;
	}

	public void setTotalVatAmount(double totalVatAmount)
	{
		TotalVatAmount = totalVatAmount;
	}

	public double getUnitPriceAfterDiscount()
	{
		return UnitPriceAfterDiscount;
	}

	public void setUnitPriceAfterDiscount(double unitPriceAfterDiscount)
	{
		UnitPriceAfterDiscount = unitPriceAfterDiscount;
	}

	public double getVatAmount()
	{
		return VatAmount;
	}

	public void setVatAmount(double vatAmount)
	{
		VatAmount = vatAmount;
	}

	public double getVatFactor()
	{
		return VatFactor;
	}

	public void setVatFactor(double vatFactor)
	{
		VatFactor = vatFactor;
	}

	public DocumentHeader getHeader()
	{
		return header;
	}

	public void setHeader(DocumentHeader header)
	{
		this.header = header;
	}

	public ForeignCollection<DocumentDetail> getReferenceDocumentDetails()
	{
		return ReferenceDocumentDetails;
	}
	
}
