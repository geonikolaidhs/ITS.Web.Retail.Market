package gr.net.its.retail.data;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class MeasurementUnit implements IRetailPersistent
{
	@DatabaseField(generatedId = true)
	long ID; 
	
	@DatabaseField(index = true)
	String remoteGuid;
	
	@DatabaseField(index = true)
	long UpdatedOn;
	
	@DatabaseField
	String Description;
	
	@DatabaseField
	boolean supportDecimals;

	public long getID()
	{
		return ID;
	}

	public void setID(long iD)
	{
		ID = iD;
	}

	public String getRemoteGuid()
	{
		return remoteGuid;
	}

	public void setRemoteGuid(String remoteGuid)
	{
		this.remoteGuid = remoteGuid;
	}

	public long getUpdatedOn()
	{
		return UpdatedOn;
	}

	public void setUpdatedOn(long updatedOn)
	{
		UpdatedOn = updatedOn;
	}

	public String getDescription()
	{
		return Description;
	}

	public void setDescription(String description)
	{
		Description = description;
	}

	public boolean isSupportDecimals()
	{
		return supportDecimals;
	}

	public void setSupportDecimals(boolean supportDecimals)
	{
		this.supportDecimals = supportDecimals;
	}
}
