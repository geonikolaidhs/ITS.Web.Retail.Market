package gr.net.its.retail.data;

import java.util.UUID;

public class InvalidItem
{
    private UUID ItemRemoteGuid;
    private InvalidItemReason reason;
    
    public UUID getItemRemoteGuid()
    {
        return ItemRemoteGuid;
    }
    public void setItemRemoteGuid(UUID itemRemoteGuid)
    {
        ItemRemoteGuid = itemRemoteGuid;
    }
    public InvalidItemReason getReason()
    {
        return reason;
    }
    public void setReason(InvalidItemReason reason)
    {
        this.reason = reason;
    }

}
