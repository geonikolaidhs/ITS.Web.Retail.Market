package gr.net.its.retail.data;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class DocumentStatus implements IRetailPersistent
{

	@DatabaseField
	private long CreatedOn;
	@DatabaseField
	private long UpdatedOn;
	@DatabaseField(generatedId=true)
	private long ID;
	@DatabaseField
	private boolean IsDefault;
	@DatabaseField
	private String Description;
	@DatabaseField(index = true)
	private String remoteGuid;

	public String toString()
	{
		return Description;
	}

	public String getRemoteGuid()
	{
		return remoteGuid;
	}

	public void setRemoteGuid(String remoteGuid)
	{
		this.remoteGuid = remoteGuid;
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

	public long getID()
	{
		return ID;
	}

	public void setID(long iD)
	{
		ID = iD;
	}

	public boolean isIsDefault()
	{
		return IsDefault;
	}

	public void setIsDefault(boolean isDefault)
	{
		IsDefault = isDefault;
	}

	public String getDescription()
	{
		return Description;
	}

	public void setDescription(String description)
	{
		Description = description;
	}

	public boolean equals(DocumentStatus other)
	{
		if (this.ID!=(other.ID))
			return false;
		if (IsDefault != other.IsDefault)
			return false;
		if (!Description.equals(other.Description))
			return false;
		if (!remoteGuid.equals(other.remoteGuid))
			return false;
		return true;
	}
	

}
