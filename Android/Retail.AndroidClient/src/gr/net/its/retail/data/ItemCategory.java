package gr.net.its.retail.data;



import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class ItemCategory implements IRetailPersistent
{
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

	public ItemCategory getParent()
	{
		return parent;
	}

	public void setParent(ItemCategory parent)
	{
		this.parent = parent;
	}

	// Database fields
	@DatabaseField
	private long CreatedOn;
	@DatabaseField
	private long UpdatedOn;
	@DatabaseField(generatedId=true)
	private long ID;

	@DatabaseField
	private String Code;

	@DatabaseField
	private String Name;

	@DatabaseField(index = true)
	private String remoteGuid;

	@DatabaseField(index = true)
	private String remoteParentGuid;

	public String getRemoteParentGuid()
	{
		return remoteParentGuid;
	}

	public void setRemoteParentGuid(String remoteParentGuid)
	{
		this.remoteParentGuid = remoteParentGuid;
	}

	public String getRemoteGuid()
	{
		return remoteGuid;
	}

	public void setRemoteGuid(String remoteGuid)
	{
		this.remoteGuid = remoteGuid;
	}

	@DatabaseField(foreign = true,index = true)
	private ItemCategory parent;

	@Override
	public String toString()
	{
		return Name;
	}
}
