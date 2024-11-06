package gr.net.its.retail.data;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class ItemAnalyticTree implements IRetailPersistent
{
	@DatabaseField
	private long CreatedOn;
	@DatabaseField(index = true)
	private long UpdatedOn;
	@DatabaseField(generatedId = true)
	private long ID;

	@DatabaseField(index = true)
	private String remoteGuid;

	public String getRemoteGuid()
	{
		return remoteGuid;
	}

	public void setRemoteGuid(String remoteGuid)
	{
		this.remoteGuid = remoteGuid;
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

	public long getID()
	{
		return ID;
	}

	public void setID(long iD)
	{
		ID = iD;
	}

	public ItemCategory getItemCategory()
	{
		return itemCategory;
	}

	public void setItemCategory(ItemCategory itemCategory)
	{
		this.itemCategory = itemCategory;
	}

	public Item getItem()
	{
		return item;
	}

	public void setItem(Item item)
	{
		this.item = item;
	}

	@DatabaseField(foreign = true, index = true)
	// , foreignAutoRefresh = true)
	private ItemCategory itemCategory;

	@DatabaseField(foreign = true, index = true)
	// , foreignAutoRefresh = true)
	private Item item;

	@DatabaseField
	private String itemRemoteGuid;

	public String getItemRemoteGuid()
	{
		return itemRemoteGuid;
	}

	public void setItemRemoteGuid(String itemRemoteGuid)
	{
		this.itemRemoteGuid = itemRemoteGuid;
	}

	public String getItemCategoryRemoteGuid()
	{
		return itemCategoryRemoteGuid;
	}

	public void setItemCategoryRemoteGuid(String itemCategoryRemoteGuid)
	{
		this.itemCategoryRemoteGuid = itemCategoryRemoteGuid;
	}

	@DatabaseField
	private String itemCategoryRemoteGuid;

}
