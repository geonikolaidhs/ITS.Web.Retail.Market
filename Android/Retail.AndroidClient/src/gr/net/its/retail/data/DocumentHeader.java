package gr.net.its.retail.data;


import com.j256.ormlite.dao.ForeignCollection;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class DocumentHeader
{

	@DatabaseField
	private long CreatedOn;

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

	public Customer getCustomer()
	{
		return customer;
	}

	public void setCustomer(Customer customer)
	{
		this.customer = customer;
	}

	public long getDocumentDate()
	{
		return DocumentDate;
	}

	public void setDocumentDate(long documentDate)
	{
		DocumentDate = documentDate;
	}

	public double getDocumentDiscount()
	{
		return DocumentDiscount;
	}

	public double getGrossTotal()
	{
		return GrossTotal;
	}

	public double getVatAmount1()
	{
		return VatAmount1;
	}

	public double getVatFactor1()
	{
		return VatFactor1;
	}

	public double getVatAmount2()
	{
		return VatAmount2;
	}

	public double getVatFactor2()
	{
		return VatFactor2;
	}

	public double getVatAmount3()
	{
		return VatAmount3;
	}

	public double getVatFactor3()
	{
		return VatFactor3;
	}

	public double getVatAmount4()
	{
		return VatAmount4;
	}

	public double getVatFactor4()
	{
		return VatFactor4;
	}

	public double getTotalVatAmount()
	{
		return TotalVatAmount;
	}

	public double getTotalDiscountAmount()
	{
		return TotalDiscountAmount;
	}

	public double getNetTotal()
	{
		return NetTotal;
	}

	public double getNetTotalAfterDiscount()
	{
		return NetTotalAfterDiscount;
	}

	public ForeignCollection<DocumentDetail> getDetails()
	{
		return Details;
	}
	
	public String getDeliveryAddress()
	{
	    return DeliveryAddress;
	}

	public void setDeliveryAddress(String deliveryAddress)
	{
	    DeliveryAddress = deliveryAddress;
	}


	@DatabaseField
	private long UpdatedOn;
	@DatabaseField( generatedId=true)
	private long ID;

	@DatabaseField(foreign = true, foreignAutoRefresh = true)
	private Customer customer;

	@DatabaseField
	private long DocumentDate;
	
	@DatabaseField
	private String DeliveryAddress;

	@DatabaseField
	private double DocumentDiscount;
	@DatabaseField
	private double GrossTotal;
	@DatabaseField
	private double VatAmount1;
	@DatabaseField
	private double VatFactor1;
	@DatabaseField
	private double VatAmount2;
	@DatabaseField
	private double VatFactor2;
	@DatabaseField
	private double VatAmount3;
	@DatabaseField
	private double VatFactor3;
	@DatabaseField
	private double VatAmount4;
	@DatabaseField
	private double VatFactor4;
	@DatabaseField
	private double TotalVatAmount;
	@DatabaseField
	private double TotalDiscountAmount;
	@DatabaseField
	private double NetTotal;
	@DatabaseField
	private String CreatedBy;
	@DatabaseField
	private boolean AllowMultipleLines;  
	@DatabaseField
	private String Comments;
	
	@DatabaseField
	private double NetTotalAfterDiscount;
	
	@DatabaseField
	private String RemoteDeviceDocumentHeaderGuid;
	
	

	public String getRemoteDeviceDocumentHeaderGuid()
	{
		return RemoteDeviceDocumentHeaderGuid;
	}

	public void setRemoteDeviceDocumentHeaderGuid(String remoteDeviceDocumentHeaderGuid)
	{
		RemoteDeviceDocumentHeaderGuid = remoteDeviceDocumentHeaderGuid;
	}


	@ForeignCollectionField(eager= true)
	private ForeignCollection<DocumentDetail> Details;

	@DatabaseField(foreign = true, foreignAutoRefresh = true)
	private DocumentStatus documentStatus;
	
	

	public DocumentStatus getDocumentStatus()
	{
		return documentStatus;
	}

	public void setDocumentStatus(DocumentStatus documentStatus)
	{
		this.documentStatus = documentStatus;
	}

	public void UpdateSums()
	{
		DocumentDiscount = GrossTotal = TotalVatAmount = TotalDiscountAmount = NetTotal = NetTotalAfterDiscount = 0;
		for (int i = 0; i < Details.size(); i++)
		{
			DocumentDetail docline1 = (DocumentDetail) Details.toArray()[i];

			NetTotal += docline1.getNetTotal();
			GrossTotal += docline1.getGrossTotal();
			TotalDiscountAmount += docline1.getTotalDiscount();
			TotalVatAmount += docline1.getTotalVatAmount();
			NetTotalAfterDiscount += docline1.getNetTotalAfterDiscount();
		}
	}

	public String getCreatedBy()
	{
	    return CreatedBy;
	}

	public void setCreatedBy(String createdBy)
	{
	    CreatedBy = createdBy;
	}

	public boolean getAllowMultipleLines()
	{
	    return AllowMultipleLines;
	}

	public void setAllowMultipleLines(boolean allowMultipleLines)
	{
	    AllowMultipleLines = allowMultipleLines;
	}

	public String getComments()
	{
	    return Comments;
	}

	public void setComments(String comments)
	{
	    Comments = comments;
	}

}
