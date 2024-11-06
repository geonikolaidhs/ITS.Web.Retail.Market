package gr.net.its.retail.data;

import com.j256.ormlite.dao.ForeignCollection;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class Address implements IRetailPersistent
{
	@DatabaseField
	private long UpdatedOn;

	public long getUpdatedOn()
	{
		return UpdatedOn;
	}

	public void setUpdatedOn(long updatedOn)
	{
		UpdatedOn = updatedOn;
	}

	@DatabaseField(generatedId = true)
	private long ID;

	public void setID(long iD)
	{
		ID = iD;
	}

	@DatabaseField(foreign = true)
	private Customer customer;

	@DatabaseField
	private String Address;

	public Customer getCustomer()
	{
		return customer;
	}

	public void setCustomer(Customer customer)
	{
		this.customer = customer;
	}

	public String getAddress()
	{
		return Address;
	}

	public void setAddress(String address)
	{
		Address = address;
	}
	
	@DatabaseField//(index = true)
	private String customerRemoteGuid;

	public String getCustomerRemoteGuid()
	{
		return customerRemoteGuid;
	}

	public void setCustomerRemoteGuid(String customerRemoteGuid)
	{
		this.customerRemoteGuid = customerRemoteGuid;
	}

	@DatabaseField//(index = true)
	private String remoteGuid;

	public String getRemoteGuid()
	{
		return remoteGuid;
	}

	public void setRemoteGuid(String remoteGuid)
	{
		this.remoteGuid = remoteGuid;
	}

	@Override
	public String toString()
	{
		
		return Address;
	}
	
	

}
