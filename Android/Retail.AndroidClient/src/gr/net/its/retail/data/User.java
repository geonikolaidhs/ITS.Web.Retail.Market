package gr.net.its.retail.data;


import com.j256.ormlite.dao.ForeignCollection;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable(tableName = "User")
public class User implements IRetailPersistent
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

	@DatabaseField
	private String username;

	@DatabaseField
	private String password;

	@DatabaseField
	private UserType usertype;
	
	public ForeignCollection<UserStoreAccess> getUserStoreAccess()
	{
	    return userStoreAccess;
	}

	public void setUserStoreAccess(ForeignCollection<UserStoreAccess> userStoreAccess)
	{
	    this.userStoreAccess = userStoreAccess;
	}

	@ForeignCollectionField
	private  ForeignCollection<UserStoreAccess> userStoreAccess;

	public String getPassword()
	{
	    return password;
	}

	private String encodePassword(String plainpassword)
	{
		return plainpassword;
	}

	public void setPassword(String newpass)
	{
		password = encodePassword(newpass);
	}

	public boolean isSamePassword(String pass2check)
	{
		return (this.password.equals(encodePassword(pass2check)));
	}

	// Getter & Setters
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

	public String getUsername()
	{
		return username;
	}

	public void setUsername(String username)
	{
		this.username = username;
	}

	public UserType getUsertype()
	{
		return usertype;
	}

	public void setUsertype(UserType usertype)
	{
		this.usertype = usertype;
	}

}
