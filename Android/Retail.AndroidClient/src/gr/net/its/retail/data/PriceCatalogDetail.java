package gr.net.its.retail.data;


import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class PriceCatalogDetail implements IRetailPersistent
{
	@DatabaseField(generatedId=true )
	private long ID;

	@DatabaseField
	private String Code;

	@DatabaseField(index = true)
	private String remoteGuid;

	public String getRemoteGuid()
	{
		return remoteGuid;
	}

	public void setRemoteGuid(String remoteGuid)
	{
		this.remoteGuid = remoteGuid;
	}

	@DatabaseField
	private long CreatedOn;

	public long getID()
	{
		return ID;
	}

	public void setID(long iD)
	{
		ID = iD;
	}

	public String getCode()
	{
		return Code;
	}

	public void setCode(String code)
	{
		Code = code;
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

	public PriceCatalog getPc()
	{
		return pc;
	}

	public void setPriceCatalog(PriceCatalog pc)
	{
		this.pc = pc;
	}

	public Barcode getBc()
	{
		return bc;
	}

	public void setBarcode(Barcode bc)
	{
		this.bc = bc;
	}

	public double getPrice()
	{
		return price;
	}

	public void setPrice(double price)
	{
		this.price = price;
	}

	public double getDiscount()
	{
		return discount;
	}

	public void setDiscount(double discount)
	{
		this.discount = discount;
	}

	public boolean isVATIncluded()
	{
		return VATIncluded;
	}

	public void setVATIncluded(boolean vATIncluded)
	{
		VATIncluded = vATIncluded;
	}

	@DatabaseField(index = true)
	private long UpdatedOn;

	@DatabaseField
	private String barcodeRemoteGuid;
	
	@DatabaseField
	private String itemRemoteGuid;
	
	@DatabaseField
	private String pcRemoteGuid;
	
	
	public String getPcRemoteGuid()
	{
		return pcRemoteGuid;
	}

	public void setPcRemoteGuid(String pcRemoteGuid)
	{
		this.pcRemoteGuid = pcRemoteGuid;
	}

	public String getBarcodeRemoteGuid()
	{
	    return barcodeRemoteGuid;
	}

	public void setBarcodeRemoteGuid(String barcodeRemoteGuid)
	{
	    this.barcodeRemoteGuid = barcodeRemoteGuid;
	}

	public String getItemRemoteGuid()
	{
	    return itemRemoteGuid;
	}

	public void setItemRemoteGuid(String itemRemoteGuid)
	{
	    this.itemRemoteGuid = itemRemoteGuid;
	}

	@DatabaseField(foreign = true)
	private PriceCatalog pc;

	@DatabaseField(foreign = true,index = true)
	private Barcode bc;

	@DatabaseField(foreign = true)
	private Item item;

	public Item getItem()
	{
		return item;
	}

	public void setItem(Item item)
	{
		this.item = item;
	}

	@DatabaseField
	private double price;

	@DatabaseField
	private double discount;

	@DatabaseField
	private boolean VATIncluded;
}
