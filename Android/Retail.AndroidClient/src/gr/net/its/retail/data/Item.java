package gr.net.its.retail.data;

import com.j256.ormlite.dao.ForeignCollection;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable(tableName = "Item")
public class Item implements IRetailPersistent
{

	// Database fields
	@DatabaseField
	private Long CreatedOn;
	@DatabaseField(index = true)
	private long UpdatedOn;

	public long getUpdatedOn()
	{
		return UpdatedOn;
	}

	public void setUpdatedOn(long updatedOn)
	{
		UpdatedOn = updatedOn;
	}

	@DatabaseField(generatedId = true)
	private long ID;

	public void setID(long iD)
	{
		ID = iD;
	}

	@DatabaseField
	private String Code;

	@DatabaseField(index = true)
	private String Name;

	@DatabaseField
	private String loweCaseName;

	public void setLoweCaseName(String loweCaseName)
	{
		this.loweCaseName = loweCaseName;
	}

	public String getLoweCaseName()
	{
		return loweCaseName;
	}

	@ForeignCollectionField
	private ForeignCollection<Barcode> barcodes;

	@DatabaseField(index = true)
	private String remoteGuid;

	@DatabaseField
	private String imageOid;

	@DatabaseField
	private String defaultBarcode;

	@DatabaseField
	private String defaultBarcodeRemoteGuid;

	@DatabaseField
	private boolean isactive;

	public boolean isActive()
	{
		return this.isactive;
	}

	public void setActive(boolean active)
	{
		this.isactive = active;
	}

	public String getDefaultBarcodeRemoteGuid()
	{
		return defaultBarcodeRemoteGuid;
	}

	public void setDefaultBarcodeRemoteGuid(String defaultBarcodeRemoteGuid)
	{
		this.defaultBarcodeRemoteGuid = defaultBarcodeRemoteGuid;
	}

	@DatabaseField(foreign = true)
	// , foreignAutoRefresh = true)
	private Barcode defaultBarcodeObject;

	@DatabaseField(foreign = true)
	private VATCategory vatCategory;

	@DatabaseField
	private String vatCategoryRemoteGuid;

	public String getVatCategoryRemoteGuid()
	{
		return vatCategoryRemoteGuid;
	}

	public void setVatCategoryRemoteGuid(String vatCategoryRemoteGuid)
	{
		this.vatCategoryRemoteGuid = vatCategoryRemoteGuid;
	}

	public VATCategory getVatCategory()
	{
		return vatCategory;
	}

	public void setVatCategory(VATCategory vatCategory)
	{
		this.vatCategory = vatCategory;
	}

	public Boolean isVatIncluded()
	{
		return VatIncluded;
	}

	public void setVatIncluded(Boolean vatIncluded)
	{
		VatIncluded = vatIncluded;
	}

	@DatabaseField
	private Boolean VatIncluded;

	@DatabaseField
	private long InsertedOn;

	@DatabaseField
	private double packingQty;

	@DatabaseField
	private double maxOrderQty;

	public double getPackingQty()
	{
		return packingQty;
	}

	public void setPackingQty(double packingQty)
	{
		this.packingQty = packingQty;
	}

	public double getMaxOrderQty()
	{
		return maxOrderQty;
	}

	public void setMaxOrderQty(double maxOrderQty)
	{
		this.maxOrderQty = maxOrderQty;
	}

	public long getInsertedOn()
	{
		return InsertedOn;
	}

	public void setInsertedOn(long insertedOn)
	{
		InsertedOn = insertedOn;
	}

	public Barcode getDefaultBarcodeObject()
	{
		return defaultBarcodeObject;
	}

	public void setDefaultBarcodeObject(Barcode defaultBarcodeObject)
	{
		this.defaultBarcodeObject = defaultBarcodeObject;
	}

	public String getDefaultBarcode()
	{
		return defaultBarcode;
	}

	public void setDefaultBarcode(String defaultBarcode)
	{
		this.defaultBarcode = defaultBarcode;
	}

	public String getImageOid()
	{
		return imageOid;
	}

	public void setImageOid(String imageOid)
	{
		this.imageOid = imageOid;
	}

	// Getters & Setters
	public String getCode()
	{
		return Code;
	}

	public void setCode(String code)
	{
		Code = code;
	}

	public String getName()
	{
		return Name;
	}

	public void setName(String name)
	{
		Name = name;
	}

	public long getID()
	{
		return ID;
	}

	public ForeignCollection<Barcode> getBarcodes()
	{
		return barcodes;
	}

	public String getRemoteOid()
	{
		return remoteGuid;
	}

	public void setRemoteOid(String remoteOid)
	{
		this.remoteGuid = remoteOid;
	}

	// Constructors
	public Item()
	{

	}

	public Item(String Code, String Name)
	{
		this.setCode(Code);
		this.setName(Name);
	}

	@Override
	public String toString()
	{
		return Name;
	}

	public String getRemoteGuid()
	{
		return remoteGuid;
	}

}
