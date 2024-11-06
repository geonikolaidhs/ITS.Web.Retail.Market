package gr.net.its.retail.data;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.field.ForeignCollectionField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable(tableName = "OfferDetail")
public class OfferDetail implements IRetailPersistent
{
    @DatabaseField(generatedId = true)
    private long ID;
    
    @DatabaseField
    private long CreatedOn;
    
    @DatabaseField
    private long UpdatedOn;

    @DatabaseField
    private String remoteGuid;
    
    @DatabaseField(foreign = true, foreignAutoRefresh = true,maxForeignAutoRefreshLevel = 2)
    private Item Item;
    
    @DatabaseField
    private String ItemRemoteGuid;

    @DatabaseField(foreign = true, foreignAutoRefresh = true)
    private Offer Offer;
    
    @DatabaseField
    private String OfferRemoteGuid;
   
    @DatabaseField
    private boolean Active;

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

    public Item getItem()
    {
        return Item;
    }

    public void setItem(Item item)
    {
        Item = item;
    }

    public String getItemRemoteGuid()
    {
        return ItemRemoteGuid;
    }

    public void setItemRemoteGuid(String itemRemoteGuid)
    {
        ItemRemoteGuid = itemRemoteGuid;
    }

    public Offer getOffer()
    {
        return Offer;
    }

    public void setOffer(Offer offer)
    {
        Offer = offer;
    }

    public String getOfferRemoteGuid()
    {
        return OfferRemoteGuid;
    }

    public void setOfferRemoteGuid(String offerRemoteGuid)
    {
        OfferRemoteGuid = offerRemoteGuid;
    }

    public boolean isActive()
    {
        return Active;
    }

    public void setActive(boolean active)
    {
        Active = active;
    }
    
    @Override
    public String toString()
    {
    
	if(Item != null)
	{
	    return Item.getName();
	}
	else
	{
	    return "";
	}
        
    }

}