package gr.net.its.retail.data;

public class ItemPrice
{
    public Item item;
    public double price,qty;
    @Override
    public String toString()
    {
        return item == null ? "" : item.getName();
    }
    
}
