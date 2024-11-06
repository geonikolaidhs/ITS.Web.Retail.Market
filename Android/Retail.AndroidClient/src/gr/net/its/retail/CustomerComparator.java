package gr.net.its.retail;

import gr.net.its.retail.data.Customer;
import gr.net.its.retail.data.DatabaseHelper;

import java.util.Comparator;

public class CustomerComparator implements Comparator<Customer>
{
    private DatabaseHelper dbHelper; 
    
    public CustomerComparator(DatabaseHelper dbHelper)
    {
	this.dbHelper= dbHelper;
    }
    
    public int compare(Customer customer1, Customer customer2)
    {
	boolean hasOrder1 = customer1.hasOrder(dbHelper);
	boolean hasOrder2 = customer2.hasOrder(dbHelper);
	if(hasOrder1 && hasOrder2)
	{
	    return 0;
	}
	else if (!hasOrder1 && !hasOrder2)
	{
	    return 0;
	}
	else if(hasOrder1)
	{
	    return -1;
	}
	else
	{
	    return 1;
	}
    }

}
