package gr.net.its.retail;

import java.util.List;

import gr.net.its.retail.data.DatabaseHelper;
import gr.net.its.retail.data.DocumentHeader;
import gr.net.its.retail.data.Item;
import gr.net.its.retail.data.ItemPrice;
import android.content.Context;
import android.graphics.Color;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

public class ItemCustomAdapter extends ArrayAdapter<ItemPrice>
{

	private List<ItemPrice> itemsArray;
	private Context mContext;
	private boolean showItemCode;

	public ItemCustomAdapter(Context context, int textViewResourceId, List<ItemPrice> objects)
	{
		super(context, textViewResourceId, objects);
		mContext = context;
		itemsArray = objects;
		this.showItemCode = false;

	}

	public ItemCustomAdapter(Context context, int textViewResourceId, List<ItemPrice> objects, boolean showItemCode)
	{
		super(context, textViewResourceId, objects);
		mContext = context;
		itemsArray = objects;
		this.showItemCode = showItemCode;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent)
	{
		View v = convertView;
		if (v == null)
		{

			LayoutInflater vi = (LayoutInflater) mContext.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			v = vi.inflate(R.layout.item_row, null);
		}

		final ItemPrice item = (itemsArray != null && position < itemsArray.size()) ? itemsArray.get(position) : null;
		if (item != null)
		{
			TextView itemCode = (TextView) v.findViewById(R.id.itemRow_itemCode);
			TextView itemName = (TextView) v.findViewById(R.id.itemRow_itemName);
			TextView unitPrice = (TextView) v.findViewById(R.id.itemRow_unitPrice);
			TextView orderQty = (TextView) v.findViewById(R.id.itemRow_orderQty);

			if (itemCode != null)
			{
				if (showItemCode)
				{
					itemCode.setVisibility(View.VISIBLE);
					itemCode.setText(item.item.getCode());
					itemCode.setTextColor(Color.BLACK);
				}
				else
				{
					itemCode.setVisibility(View.GONE);
				}
			}

			if (itemName != null)
			{
				itemName.setVisibility(View.VISIBLE);
				itemName.setText(item.item.getName());
				itemName.setTextColor(Color.BLACK);
			}

			if (unitPrice != null)
			{
				unitPrice.setVisibility(View.VISIBLE);
				// unitPrice.setGravity(Gravity.RIGHT);
				unitPrice.setText("" + RetailHelper.euroValue.format(item.price));
				unitPrice.setTextColor(Color.BLACK);
			}

			if (orderQty != null)
			{
				orderQty.setVisibility(View.VISIBLE);
				// orderQty.setGravity(Gravity.RIGHT);
				orderQty.setText("" + RetailHelper.qtyFormat.format(item.qty));
				orderQty.setTextColor(Color.BLACK);
			}
		}
		
		return v;
	}

}
