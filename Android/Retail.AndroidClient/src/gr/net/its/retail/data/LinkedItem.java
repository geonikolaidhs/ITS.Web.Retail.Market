package gr.net.its.retail.data;


import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class LinkedItem implements IRetailPersistent
{
	@DatabaseField(generatedId=true)
	private long ID;

	@DatabaseField(index = true)
	private String remoteGuid;

	@DatabaseField(index = true)
	private String itemGuid;

	@DatabaseField(index = true)
	private String subItemGuid;

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

	public String getItemGuid()
	{
		return itemGuid;
	}

	public void setItemGuid(String itemGuid)
	{
		this.itemGuid = itemGuid;
	}

	public String getSubItemGuid()
	{
		return subItemGuid;
	}

	public void setSubItemGuid(String subItemGuid)
	{
		this.subItemGuid = subItemGuid;
	}

}
