package gr.net.its.retail.data;



import java.sql.SQLException;

import gr.net.its.common.Utilities;
import gr.net.its.retail.MainActivity;

import com.j256.ormlite.dao.ForeignCollection;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class Customer implements IRetailPersistent
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

	@DatabaseField(generatedId=true)
	private long ID;

	public void setID(long iD)
	{
		ID = iD;
	}

	@DatabaseField
	private String CompanyName;

	@DatabaseField
	private String lowerCompanyName;
	
	public String getLowerCompanyName()
	{
		return lowerCompanyName;
	}

	@DatabaseField
	private String TaxCode;

	@DatabaseField
	private String DefaultAddress;

	@DatabaseField
	private String DefaultPhone;

	@DatabaseField(index = true)
	private String remoteGuid;

	@DatabaseField(foreign = true)
	private PriceCatalog pc;

	@DatabaseField(foreign = true)
	private VATLevel vl;
	
	@DatabaseField(foreign = true,index = true)
	private Store store;
	
	@DatabaseField
	private String Code;
	
	@DatabaseField
	private String pcRemoteGuid;
	
	@DatabaseField
	private String storeRemoteGuid;
	
	@DatabaseField
	private String vatLevelRemoteGuid;

	public String getStoreRemoteGuid()
	{
		return storeRemoteGuid;
	}

	public void setStoreRemoteGuid(String storeRemoteGuid)
	{
		this.storeRemoteGuid = storeRemoteGuid;
	}

	public String getVatLevelRemoteGuid()
	{
		return vatLevelRemoteGuid;
	}

	public void setVatLevelRemoteGuid(String vatLevelRemoteGuid)
	{
		this.vatLevelRemoteGuid = vatLevelRemoteGuid;
	}

	public String getPcRemoteGuid()
	{
	    return pcRemoteGuid;
	}

	public void setPcRemoteGuid(String pcRemoteGuid)
	{
	    this.pcRemoteGuid = pcRemoteGuid;
	}

	public VATLevel getVl()
	{
		return vl;
	}

	public void setVl(VATLevel vl)
	{
		this.vl = vl;
	}

	public PriceCatalog getPc()
	{
		return pc;
	}

	public void setPc(PriceCatalog pc)
	{
		this.pc = pc;
	}

	public String getRemoteGuid()
	{
		return remoteGuid;
	}

	public void setRemoteGuid(String remoteGuid)
	{
		this.remoteGuid = remoteGuid;
	}

	@ForeignCollectionField
	private ForeignCollection<DocumentHeader> headers;

	public ForeignCollection<DocumentHeader> getDocumentHeaders()
	{
		return headers;
	}

	public String getCompanyName()
	{
		return CompanyName;
	}

	public void setCompanyName(String companyName)
	{
		CompanyName = companyName;
		lowerCompanyName = companyName.toLowerCase();
	}

	public String getTaxCode()
	{
		return TaxCode;
	}

	public void setTaxCode(String taxCode)
	{
		TaxCode = taxCode;
	}

	public String getDefaultAddress()
	{
		return DefaultAddress;
	}

	public void setDefaultAddress(String defaultAddress)
	{
		DefaultAddress = defaultAddress;
	}

	public String getDefaultPhone()
	{
		return DefaultPhone;
	}

	public void setDefaultPhone(String defaultPhone)
	{
		DefaultPhone = defaultPhone;
	}

	public long getID()
	{
		return ID;
	}

	public String toString()
	{
		return CompanyName;
	}

	public Store getStore()
	{
	    return store;
	}

	public void setStore(Store store)
	{
	    this.store = store;
	}

	public String getCode()
	{
		return Utilities.TrimLeft(Code, '0');
	    
	}

	public void setCode(String code)
	{
	    Code = code;
	}

	
	@ForeignCollectionField//(eager= true)
	private ForeignCollection<Address> Addresses;

	public ForeignCollection<Address> getAddresses()
	{
		return Addresses;
	}
	
	public boolean hasOrder(DatabaseHelper dbHelper)
	{
	    boolean hasOrder = false;
	    
	    ForeignCollection<DocumentHeader> heads = this.getDocumentHeaders();
	    if (heads == null)
	    {
		Customer customer2 = null;
		try
		{
		    customer2 = dbHelper.getCustomers().queryForId(this.getID());
		    heads = customer2.getDocumentHeaders();
		}
		catch (SQLException e)
		{
		    e.printStackTrace();
		}
	    }
	    if (heads.size() > 0)
	    {
		DocumentHeader head = heads.iterator().next();
		if (head.getDeliveryAddress() == null)
		{
		    try
		    {
			head = dbHelper.getDocumentHeaders().queryForId(head.getID());
		    }
		    catch (Exception e)
		    {
			e.printStackTrace();
		    }

		}
		hasOrder =  head.getDetails().size() > 0;
	    }
	    
	    return hasOrder;
	}
}
