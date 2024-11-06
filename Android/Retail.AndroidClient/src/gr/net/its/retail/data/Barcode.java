package gr.net.its.retail.data;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class Barcode implements IRetailPersistent
{
    @DatabaseField(generatedId = true)
    private long ID;

    @DatabaseField(index = true)
    private String Code;
    
    @DatabaseField(foreign = true)
    private MeasurementUnit MeasurementUnit;
    
    @DatabaseField(index = true)
    private String measurementUnitRemoteGuid;

    public String getMeasurementUnitRemoteGuid()
	{
		return measurementUnitRemoteGuid;
	}

	public void setMeasurementUnitRemoteGuid(String measurementUnitGuid)
	{
		this.measurementUnitRemoteGuid = measurementUnitGuid;
	}

	public MeasurementUnit getMeasurementUnit()
	{
		return MeasurementUnit;
	}

	public void setMeasurementUnit(MeasurementUnit measurementUnit)
	{
		MeasurementUnit = measurementUnit;
	}

	@DatabaseField(foreign = true)
    // , foreignAutoRefresh = true)
    private Item item;

    @DatabaseField(index = true)
    private String itemRemoteGuid;

    public String getItemRemoteGuid()
    {
        return itemRemoteGuid;
    }

    public void setItemRemoteGuid(String itemRemoteGuid)
    {
        this.itemRemoteGuid = itemRemoteGuid;
    }

    @DatabaseField
    private long CreatedOn;
    @DatabaseField(index = true)
    private long UpdatedOn;

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

    private boolean IsDefault;

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

    public Item getItem()
    {
	return item;
    }

    public void setItem(Item item)
    {
	this.item = item;
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

    public boolean isIsDefault()
    {
	return IsDefault;
    }

    public void setIsDefault(boolean isDefault)
    {
	IsDefault = isDefault;
    }

    public Barcode()
    {

    }

}
