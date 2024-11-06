package gr.net.its.retail;

import java.util.List;

import gr.net.its.retail.data.*;
import android.content.Context;
import android.graphics.drawable.Drawable;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.TextView;

public class ItemGridAdapter extends ArrayAdapter<Item>
{
	private List<Item> itemsArray;
	private Context mContext;
	private boolean showItemCode;

	public ItemGridAdapter(Context context, int textViewResourceId, List<Item> objects)
	{
		super(context, textViewResourceId, objects);
		mContext = context;
		itemsArray = objects;
		this.showItemCode = false;

	}

	public ItemGridAdapter(Context context, int textViewResourceId, List<Item> objects, boolean showItemCode)
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
			v = vi.inflate(R.layout.item_grid_element, null);
		}
		

		final Item item = (itemsArray != null && position < itemsArray.size()) ? itemsArray.get(position) : null;

		if (item != null)
		{		
			ImageView imgItemImage = (ImageView)v.findViewById(R.id.imgItemImage);
			TextView lblMainText = (TextView)v.findViewById(R.id.lblMainText);
			TextView lblMinorText = (TextView)v.findViewById(R.id.lblMinorText);
			if(imgItemImage!=null)
			{
				if(false)//!item.getImageOid().equals("00000000-0000-0000-0000-000000000000")) //TODO: support images
				{
					String img = MainActivity.IMAGES_PATH + "64/img_"+item.getImageOid().replace("-", "")+".png";
					Drawable d  = Drawable.createFromPath(img);
					imgItemImage.setImageDrawable(d);
				}	
				else
				{
					imgItemImage.setImageResource(R.raw.no_image_small);
				}
			}
			if(lblMainText!=null)
			{
				//if(item.getName().length()<=24)
					lblMainText.setText(item.getName());
				//else
				//	lblMainText.setText(item.getName().substring(0, 19)+"...");
			}
			if(lblMinorText!=null)
			{
				lblMinorText.setText(item.getCode());
			}
				
		}
		return v;
	}
}
