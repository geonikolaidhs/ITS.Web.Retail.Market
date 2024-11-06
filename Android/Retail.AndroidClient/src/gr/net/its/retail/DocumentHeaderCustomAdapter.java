package gr.net.its.retail;

import java.text.SimpleDateFormat;
import java.util.List;

import com.j256.ormlite.dao.ForeignCollection;

import gr.net.its.retail.data.Customer;
import gr.net.its.retail.data.DocumentHeader;
import gr.net.its.retail.data.TDocHead;
import android.content.Context;
import android.graphics.Color;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

public class DocumentHeaderCustomAdapter extends ArrayAdapter<TDocHead>
{
	private List<TDocHead> docsArray;
	private Context mContext;
	
	public DocumentHeaderCustomAdapter(Context context,  int textViewResourceId, List<TDocHead> objects)
	{
		super(context, textViewResourceId, objects);
		mContext = context;
		docsArray = objects;
	
	}
	
	@Override
	public View getView(int position, View convertView, ViewGroup parent)
	{
		View v = convertView;
		if (v == null)
		{

			LayoutInflater vi = (LayoutInflater) mContext.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			v = vi.inflate(R.layout.document_header_row, null);
		}

		final TDocHead document = (docsArray != null && position < docsArray.size()) ? docsArray.get(position) : null;
		if (document != null)
		{
			TextView documentDate = (TextView) v.findViewById(R.id.docHead_documentDate);
			TextView totalAmount = (TextView) v.findViewById(R.id.docHead_totalAmount);
			TextView totalDiscount = (TextView) v.findViewById(R.id.docHead_totalDiscount);
			TextView totalNetAmount = (TextView) v.findViewById(R.id.docHead_totalNetAmount);

			if(documentDate!= null)
			{			
				SimpleDateFormat dtf = new SimpleDateFormat("dd-MM-yyyy");
				String txt = dtf.format(document.getFinalizeDate().getTime());
				documentDate.setText(txt);	
				documentDate.setTextColor(Color.BLACK);
			}
			
			if(totalAmount != null)
			{
				totalAmount.setText(RetailHelper.euroNormal.format(document.getGrossTotal()));
				totalAmount.setGravity(Gravity.CENTER_VERTICAL|Gravity.RIGHT);
				totalAmount.setTextColor(Color.BLACK);
			}
			if(totalDiscount != null)
			{
				totalDiscount.setText(RetailHelper.euroNormal.format(document.getDiscountTotal()));
				totalDiscount.setGravity(Gravity.CENTER_VERTICAL|Gravity.RIGHT);
				totalDiscount.setTextColor(Color.BLACK);
			}
			if(totalNetAmount != null)
			{
				totalNetAmount.setText(RetailHelper.euroNormal.format(document.getNetTotal()));
				totalNetAmount.setGravity(Gravity.CENTER_VERTICAL|Gravity.RIGHT);
				totalNetAmount.setTextColor(Color.BLACK);
			}
			
		}
		return v;
	}

}
