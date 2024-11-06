package gr.net.its.retail;

import java.sql.SQLException;
import java.util.List;

import com.j256.ormlite.dao.ForeignCollection;

import gr.net.its.retail.data.Customer;
import gr.net.its.retail.data.DatabaseHelper;
import gr.net.its.retail.data.DocumentHeader;
import gr.net.its.retail.data.Item;
import gr.net.its.retail.data.ItemPrice;
import android.content.Context;
import android.graphics.Color;
import android.graphics.drawable.Drawable;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

public class CustomerCustomAdapter extends ArrayAdapter<Customer>
{

	private List<Customer> customersArray;
	private Context mContext;

	private boolean markCustomerOrders;

	public boolean isMarkCustomerOrders()
	{
		return markCustomerOrders;
	}

	public void setMarkCustomerOrders(boolean markCustomerOrders)
	{
		this.markCustomerOrders = markCustomerOrders;
	}

	public CustomerCustomAdapter(Context context, int textViewResourceId, List<Customer> objects)
	{
		super(context, textViewResourceId, objects);
		mContext = context;
		customersArray = objects;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent)
	{
		View v = convertView;
		if (v == null)
		{

			LayoutInflater vi = (LayoutInflater) mContext.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			v = vi.inflate(R.layout.customer_row, null);
		}

		final Customer customer = (customersArray != null && position < customersArray.size()) ? customersArray.get(position) : null;
		if (customer != null)
		{
			boolean showCheckMark = customer.hasOrder(((MainActivity) mContext).dbHelper) && markCustomerOrders;

			TextView custCode = (TextView) v.findViewById(R.id.customerRow_customerCode);
			TextView custTaxCode = (TextView) v.findViewById(R.id.customerRow_customerTaxCode);
			TextView custCompanyName = (TextView) v.findViewById(R.id.customerRow_customerCompanyName);

			if (custCode != null)
			{
				// custCode.setTextColor((showBold)?Color.BLUE: Color.GREEN);
				custCode.setText(customer.getCode());
				custCode.setTextColor(Color.BLACK);
			}

			if (custTaxCode != null)
			{
				// custTaxCode.setTextColor((showBold)?Color.BLUE: Color.GREEN);
				custTaxCode.setText(customer.getTaxCode());
				if (showCheckMark)
				{
					custTaxCode.setBackgroundResource(R.drawable.bgcustomer);
				}
				else
					custTaxCode.setBackgroundResource(0);

				custTaxCode.setTextColor(Color.BLACK);
			}

			if (custCompanyName != null)
			{
				// custCompanyName.setTextColor((showBold)?Color.BLUE:
				// Color.GREEN);
				custCompanyName.setText(customer.getCompanyName());
				custCompanyName.setTextColor(Color.BLACK);
			}
		}
		return v;
	}

}
