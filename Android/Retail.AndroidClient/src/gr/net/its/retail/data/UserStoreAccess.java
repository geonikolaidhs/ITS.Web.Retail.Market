package gr.net.its.retail.data;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;


@DatabaseTable(tableName = "UserStoreAccess")
public class UserStoreAccess implements IRetailPersistent
{

	// Database fields
	@DatabaseField
	private long CreatedOn;
	@DatabaseField
	private long UpdatedOn;
	@DatabaseField( generatedId=true)
	private long ID;
	
	@DatabaseField(index = true)
	private String remoteGuid;
	
	@DatabaseField(foreign = true)
	private User user;
	
	@DatabaseField(foreign = true)
	private Store store;

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

	public String getRemoteGuid()
	{
	    return remoteGuid;
	}

	public void setRemoteGuid(String remoteGuid)
	{
	    this.remoteGuid = remoteGuid;
	}

	public User getUser()
	{
	    return user;
	}

	public void setUser(User user)
	{
	    this.user = user;
	}

	public Store getStore()
	{
	    return store;
	}

	public void setStore(Store store)
	{
	    this.store = store;
	}


	
	

}
