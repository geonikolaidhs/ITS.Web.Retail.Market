package gr.net.its.retail.data;


import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class VATLevel implements IRetailPersistent
{
	@DatabaseField
	private String name;
	@DatabaseField(generatedId=true)
	private long id;
	@DatabaseField
	private long UpdatedOn;
	@DatabaseField(index = true)
	private String remoteGuid;

	@DatabaseField
	private boolean Default;

	public boolean isDefault()
	{
		return Default;
	}

	public void setDefault(boolean default1)
	{
		Default = default1;
	}

	public String getName()
	{
		return name;
	}

	public void setName(String name)
	{
		this.name = name;
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
}