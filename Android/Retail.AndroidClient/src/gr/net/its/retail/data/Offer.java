package gr.net.its.retail.data;

import com.j256.ormlite.dao.ForeignCollection;
import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable(tableName = "Offer")
public class Offer implements IRetailPersistent
{
    @DatabaseField(generatedId = true)
    private long ID;

    @DatabaseField
    private long CreatedOn;

    @DatabaseField
    private long UpdatedOn;

    @DatabaseField
    private String remoteGuid;

    @DatabaseField
    private String Description;

    @DatabaseField
    private String Description2;

    @DatabaseField
    private long StartDate;

    @DatabaseField
    private long EndDate;

    @DatabaseField(foreign = true)
    private PriceCatalog PriceCatalog;

    @DatabaseField
    private String PriceCatalogRemoteGuid;

    @DatabaseField
    private boolean Active;

    @ForeignCollectionField(eager=true ,maxEagerLevel = 3)
    private ForeignCollection<OfferDetail> OfferDetails;

    public long getID()
    {
	return ID;
    }

    public void setID(long iD)
    {
	ID = iD;
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

    public String getDescription()
    {
	return Description;
    }

    public void setDescription(String description)
    {
	Description = description;
    }

    public String getDescription2()
    {
	return Description2;
    }

    public void setDescription2(String description2)
    {
	Description2 = description2;
    }

    public long getStartDate()
    {
	return StartDate;
    }

    public void setStartDate(long startDate)
    {
	StartDate = startDate;
    }

    public long getEndDate()
    {
	return EndDate;
    }

    public void setEndDate(long endDate)
    {
	EndDate = endDate;
    }

    public PriceCatalog getPriceCatalog()
    {
	return PriceCatalog;
    }

    public void setPriceCatalog(PriceCatalog priceCatalog)
    {
	PriceCatalog = priceCatalog;
    }

    public String getPriceCatalogRemoteGuid()
    {
	return PriceCatalogRemoteGuid;
    }

    public void setPriceCatalogRemoteGuid(String priceCatalogRemoteGuid)
    {
	PriceCatalogRemoteGuid = priceCatalogRemoteGuid;
    }

    public boolean isActive()
    {
	return Active;
    }

    public void setActive(boolean active)
    {
	Active = active;
    }

    public ForeignCollection<OfferDetail> getOfferDetails()
    {
	return OfferDetails;
    }

    public void setOfferDetails(ForeignCollection<OfferDetail> offerDetails)
    {
	this.OfferDetails = offerDetails;
    }

    @Override
    public String toString()
    {
	return Description;
    }

}