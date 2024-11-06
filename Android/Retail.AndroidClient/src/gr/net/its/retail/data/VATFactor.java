package gr.net.its.retail.data;


import com.j256.ormlite.field.DatabaseField;

public class VATFactor implements IRetailPersistent
{
	@DatabaseField(foreign = true)
	private VATLevel VatLevel;
	@DatabaseField(foreign = true)
	private VATCategory VatCategory;
	@DatabaseField( generatedId=true)
	private long id;
	@DatabaseField
	private long UpdatedOn;

	public VATLevel getVatLevel()
	{
		return VatLevel;
	}

	public void setVatLevel(VATLevel vatLevel)
	{
		VatLevel = vatLevel;
	}

	public VATCategory getVatCategory()
	{
		return VatCategory;
	}

	public void setVatCategory(VATCategory vatCategory)
	{
		VatCategory = vatCategory;
	}

	public long getID()
	{
		return id;
	}

	public void setID(long id)
	{
		this.id = id;
	}

	public long getUpdatedOn()
	{
		return UpdatedOn;
	}

	public void setUpdatedOn(long updatedOn)
	{
		UpdatedOn = updatedOn;
	}

	public String getRemoteGuid()
	{
		return remoteGuid;
	}

	public void setRemoteGuid(String remoteGuid)
	{
		this.remoteGuid = remoteGuid;
	}

	public double getVatFactor()
	{
		return vatFactor;
	}

	public void setVatFactor(double vatFactor)
	{
		this.vatFactor = vatFactor;
	}

	
	public String getVatLevelRemoteGuid()
	{
		return vatLevelRemoteGuid;
	}

	public void setVatLevelRemoteGuid(String vatLevelRemoteGuid)
	{
		this.vatLevelRemoteGuid = vatLevelRemoteGuid;
	}

	public String getVatCategoryRemoteGuid()
	{
		return vatCategoryRemoteGuid;
	}

	public void setVatCategoryRemoteGuid(String vatCategoryRemoteGuid)
	{
		this.vatCategoryRemoteGuid = vatCategoryRemoteGuid;
	}


	@DatabaseField(index = true)
	private String remoteGuid;
	
	@DatabaseField
	private String vatLevelRemoteGuid;
	
	@DatabaseField
	private String vatCategoryRemoteGuid;
	
	@DatabaseField
	private double vatFactor;
}
