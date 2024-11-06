package gr.net.its.retail.data;



import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class PriceCatalog implements IRetailPersistent
{
	@DatabaseField(generatedId=true )
	private long ID;

	@DatabaseField
	private String Code;

	@DatabaseField
	private String Name;

	@DatabaseField
	private long CreatedOn;

	@DatabaseField(foreign = true, foreignAutoRefresh = true)
	private PriceCatalog parent;

	public PriceCatalog getParent()
	{
		return parent;
	}

	public void setParent(PriceCatalog parent)
	{
		this.parent = parent;
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

	public String getRemoteGuid()
	{
		return remoteGuid;
	}

	public void setRemoteGuid(String remoteGuid)
	{
		this.remoteGuid = remoteGuid;
	}

	public long getParentCatalogID()
	{
		return this.parent.getID();
	}

	@DatabaseField(index = true)
	private long UpdatedOn;

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
	
	public String toString()
	{
		return Code + " - "+ Name;
	}
}
