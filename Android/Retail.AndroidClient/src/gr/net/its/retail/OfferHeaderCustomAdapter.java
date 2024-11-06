package gr.net.its.retail;

import gr.net.its.common.Utilities;
import gr.net.its.retail.data.Offer;

import java.text.SimpleDateFormat;
import java.util.GregorianCalendar;
import java.util.List;

import android.content.Context;
import android.graphics.Color;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.TextView;

public class OfferHeaderCustomAdapter extends ArrayAdapter<Offer>
{

	private List<Offer> offersArray;
	private Context mContext;
	private SimpleDateFormat dateFormat = new SimpleDateFormat("dd-MM-yy");

	public OfferHeaderCustomAdapter(Context context, int textViewResourceId, List<Offer> objects)
	{
		super(context, textViewResourceId, objects);
		mContext = context;
		offersArray = objects;
	}

	@Override
	public View getView(int position, View convertView, ViewGroup parent)
	{
		View v = convertView;
		if (v == null)
		{

			LayoutInflater vi = (LayoutInflater) mContext.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			v = vi.inflate(R.layout.offer_header_detail, null);
		}

		final Offer offer = (offersArray != null && position < offersArray.size()) ? offersArray.get(position) : null;
		if (offer != null)
		{
			TextView lblOfferHeader = (TextView) v.findViewById(R.id.lblOfferHeader);
			TextView lblOfferHeaderFrom = (TextView) v.findViewById(R.id.lblOfferHeaderFrom);
			TextView lblOfferHeaderTo = (TextView) v.findViewById(R.id.lblOfferHeaderTo);

			if (lblOfferHeader != null)
			{
				// custCode.setTextColor((showBold)?Color.BLUE: Color.GREEN);
				lblOfferHeader.setText(offer.toString());
				lblOfferHeader.setTextColor(Color.BLACK);
			}

			if (lblOfferHeaderFrom != null)
			{
				// custTaxCode.setTextColor((showBold)?Color.BLUE: Color.GREEN);
				long offerfrom = offer.getStartDate();
				GregorianCalendar cal = new GregorianCalendar();
				cal.setTimeInMillis(Utilities.convertTicksToUnixTimestamp(offerfrom));
								
				lblOfferHeaderFrom.setText(dateFormat.format(cal.getTime()));				
				lblOfferHeaderFrom.setTextColor(Color.BLACK);
			}

			if (lblOfferHeaderTo != null)
			{
				long offerto = offer.getEndDate();
				GregorianCalendar cal = new GregorianCalendar();
				cal.setTimeInMillis(Utilities.convertTicksToUnixTimestamp(offerto));
				lblOfferHeaderTo.setText(dateFormat.format(cal.getTime()));
				lblOfferHeaderTo.setTextColor(Color.BLACK);
			}
		}
		return v;
	}

}
