package gr.net.its.retail.data;

import com.j256.ormlite.dao.ForeignCollection;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable(tableName = "Store")
public class Store implements IRetailPersistent
{

    // Database fields
    @DatabaseField
    private Long CreatedOn;
    @DatabaseField
    private long UpdatedOn;
    @DatabaseField(generatedId = true)
    private long ID;
    @DatabaseField(index = true)
    private String remoteGuid;

    @DatabaseField
    private String code;

    @DatabaseField
    private String name;

    public Long getCreatedOn()
    {
	return CreatedOn;
    }

    public void setCreatedOn(Long createdOn)
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

    public String getCode()
    {
	return code;
    }

    public void setCode(String code)
    {
	this.code = code;
    }

    public String getName()
    {
	return name;
    }

    public void setName(String name)
    {
	this.name = name;
    }

    public boolean isCentralStore()
    {
	return isCentralStore;
    }

    public void setCentralStore(boolean isCentralStore)
    {
	this.isCentralStore = isCentralStore;
    }

    public ForeignCollection<Customer> getCustomers()
    {
	return customers;
    }

    public void setCustomers(ForeignCollection<Customer> customers)
    {
	this.customers = customers;
    }

    public ForeignCollection<UserStoreAccess> getUserStoreAccess()
    {
	return userStoreAccess;
    }

    public void setUserStoreAccess(ForeignCollection<UserStoreAccess> userStoreAccess)
    {
	this.userStoreAccess = userStoreAccess;
    }

    @Override
    public String toString()
    {
	// TODO Auto-generated method stub
	return code + " - " + name;
    }

    @DatabaseField
    private boolean isCentralStore;

    @ForeignCollectionField//(eager = true, maxEagerLevel = 2)
    private ForeignCollection<Customer> customers;

    @ForeignCollectionField//(eager = true, maxEagerLevel = 2)
    private ForeignCollection<UserStoreAccess> userStoreAccess;
}
