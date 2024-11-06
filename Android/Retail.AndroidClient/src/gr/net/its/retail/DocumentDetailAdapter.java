package gr.net.its.retail;

import gr.net.its.common.Utilities;
import gr.net.its.retail.data.DocumentDetail;
import gr.net.its.retail.*;
import android.R.anim;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.graphics.Color;
import android.graphics.Typeface;
import android.graphics.drawable.Drawable;
import android.text.InputType;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;

public class DocumentDetailAdapter extends ArrayAdapter<DocumentDetail>
{

	private DocumentDetail[] docDetailsArray;
	private Context mContext;

	public DocumentDetailAdapter(Context context, int textViewResourceId, DocumentDetail[] objects)
	{
		super(context, textViewResourceId, objects);
		docDetailsArray = objects;
		mContext = context;
	}

	private String getTrimmedString(String input, int totalLength)
	{

		return (input.length() <= totalLength) ? input : input.substring(0, totalLength - 3) + "...";
	}

	private void deleteDetail(DocumentDetail item)
	{
		DocumentHelper.DeleteItem(item.getHeader(), item, ((MainActivity) mContext).dbHelper);
		((MainActivity) mContext).UpdateDocumentDetailsView();
	}

	private void changeSecondDiscount(final DocumentDetail detail)
	{
		AlertDialog alert = new AlertDialog.Builder(mContext).create();

		alert.setTitle(detail.getItem().getName() + "\r\n" + mContext.getResources().getString(R.string.price) + ":" + RetailHelper.euroValue.format(detail.getItemPrice()));
		alert.setMessage(mContext.getResources().getString(R.string.extradiscount));

		// Set an EditText view to get user input
		final EditText input = new EditText(alert.getContext());
		input.setInputType(InputType.TYPE_CLASS_NUMBER | InputType.TYPE_NUMBER_FLAG_DECIMAL);
		input.setText(RetailHelper.percent.format(detail.getSecondDiscount()).replace("%", ""));
		input.setSelectAllOnFocus(true);
		alert.setView(input);
		alert.setButton(mContext.getResources().getString(R.string.cancel), new DialogInterface.OnClickListener()
		{
			public void onClick(DialogInterface dialog, int whichButton)
			{
				// Canceled.
			}
		});

		alert.setButton2(mContext.getResources().getString(R.string.ok), new DialogInterface.OnClickListener()
		{
			public void onClick(DialogInterface dialog, int whichButton)
			{
				try
				{
					boolean isLinked = detail.getLinkedLine() != null;
					double newSecondDisount = Double.parseDouble(input.getText().toString()) / 100;
					DocumentHelper.ChangeDetailSecondDiscount(detail.getHeader(), detail, newSecondDisount, ((MainActivity) mContext).dbHelper, isLinked, ((MainActivity) mContext).appsettings);
					((MainActivity) mContext).UpdateDocumentDetailsView();
				}
				catch (Exception e)
				{
					AlertDialog ad = new AlertDialog.Builder(mContext).create();
					ad.setCancelable(false); // This blocks the 'BACK' button
					ad.setMessage(mContext.getResources().getString(R.string.invalidvalue));
					ad.setButton(mContext.getResources().getString(R.string.ok), new DialogInterface.OnClickListener()
					{
						public void onClick(DialogInterface dialog, int which)
						{
							dialog.dismiss();
							((MainActivity) mContext).UpdateDocumentDetailsView();
						}
					});
					ad.show();

				}
			}
		});

		alert.show();
	}

	private void changeDetailQty(final DocumentDetail detail)
	{
		AlertDialog alert = new AlertDialog.Builder(mContext).create();

		// alert.setTitle(detail.getItem().getName() + "\r\n" +
		// mContext.getResources().getString(R.string.price) + ":" +
		// RetailHelper.euroValue.format(detail.getItemPrice()));
		alert.setTitle(detail.getItem().getName() + ", " + mContext.getResources().getString(R.string.price) + ": " + RetailHelper.euroValue.format(detail.getItemPrice()) + ", " + mContext.getResources().getString(R.string.packingQty) + ": " + detail.getItem().getPackingQty());
		alert.setMessage(mContext.getResources().getString(R.string.changeQuantity));

		// Set an EditText view to get user input
		final EditText input = new EditText(alert.getContext());
		input.setInputType(InputType.TYPE_CLASS_NUMBER | InputType.TYPE_NUMBER_FLAG_DECIMAL);
		input.setText("" + detail.getQty());
		input.setSelectAllOnFocus(true);
		alert.setView(input);

		alert.setButton(mContext.getResources().getString(R.string.cancel), new DialogInterface.OnClickListener()
		{
			public void onClick(DialogInterface dialog, int whichButton)
			{
				// Canceled.
			}
		});

		alert.setButton2(mContext.getResources().getString(R.string.ok), new DialogInterface.OnClickListener()
		{
			public void onClick(DialogInterface dialog, int whichButton)
			{
				boolean isLinked = detail.getLinkedLine() != null;
				try
				{
					double newQty = Double.parseDouble(input.getText().toString());
					if (newQty == 0)
					{
						DocumentHelper.DeleteItem(detail.getHeader(), detail, ((MainActivity) mContext).dbHelper);
						((MainActivity) mContext).UpdateDocumentDetailsView();
					}
					else
					{
						DocumentHelper.ChangeDetail(detail.getHeader(), detail, newQty, detail.getSecondDiscount(), ((MainActivity) mContext).dbHelper, isLinked, ((MainActivity) mContext).appsettings, mContext);
						((MainActivity) mContext).UpdateDocumentDetailsView();
					}

					((MainActivity) mContext).invalidateDocumentDetailGrid();

				}
				catch (Exception e)
				{
					AlertDialog ad = new AlertDialog.Builder(mContext).create();
					ad.setCancelable(false); // This blocks the 'BACK' button
					ad.setMessage(e.getMessage());
					ad.setButton(mContext.getResources().getString(R.string.ok), new DialogInterface.OnClickListener()
					{
						public void onClick(DialogInterface dialog, int which)
						{
							dialog.dismiss();
						}
					});
					ad.show();

				}
			}
		});

		alert.show();

	}

	public View getView(int position, View convertView, ViewGroup parent)
	{
		View v = convertView;
		if (v == null)
		{

			LayoutInflater vi = (LayoutInflater) mContext.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			v = vi.inflate(R.layout.row, null);
		}

		final DocumentDetail item = (docDetailsArray != null && position < docDetailsArray.length) ? docDetailsArray[position] : null;
		if (item != null)
		{
			double q = item.getQty();
			ListItemView barcodeView = (ListItemView) v.findViewById(R.id.barcode);
			ListItemView descriptionView = (ListItemView) v.findViewById(R.id.description);
			ListItemView codeView = (ListItemView) v.findViewById(R.id.code);
			ListItemView unitpriceView = (ListItemView) v.findViewById(R.id.unitprice);
			ListItemView qtyView = (ListItemView) v.findViewById(R.id.qty);
			ListItemView tvatView = (ListItemView) v.findViewById(R.id.TotalVatAmount);
			ListItemView grossView = (ListItemView) v.findViewById(R.id.GrossTotal);
			ListItemView discount = (ListItemView) v.findViewById(R.id.discount);
			ImageButton ibDelete = (ImageButton) v.findViewById(R.id.ibDelete);
			Button btnCode = (Button) v.findViewById(R.id.btnCode);
			Button btnQuantity = (Button) v.findViewById(R.id.btnQuantity);
			Button btnExtraDiscount = (Button) v.findViewById(R.id.btnExtraDiscount);

			if (btnExtraDiscount != null && ((MainActivity) mContext).appsettings.isDiscountPermited())
			{
				btnExtraDiscount.setVisibility(View.VISIBLE);
				btnExtraDiscount.setText(RetailHelper.percent.format(item.getSecondDiscount()));
				// btnExtraDiscount.setBackgroundResource(R.drawable.transp_button);
				// btnCode.setBackgroundColor(Color.LTGRAY);
				btnExtraDiscount.setBackgroundResource(android.R.drawable.btn_default);
				btnExtraDiscount.setOnClickListener(new OnClickListener()
				{

					public void onClick(View v)
					{
						// Utilities.ShowUninterruptingMessage(mContext,
						// "TODO", 3);
						changeSecondDiscount(item);
					}

				});
			}
			else
			{
				btnExtraDiscount.setVisibility(View.GONE);
			}

			if (discount != null)
			{
				discount.setText(RetailHelper.percent.format(item.getFirstDiscount()));
			}

			if (codeView != null)
			{
				codeView.setVisibility(View.GONE);
				// codeView.setGravity(Gravity.CENTER);
			}

			if (qtyView != null)
			{
				qtyView.setVisibility(View.GONE);
			}

			if (btnCode != null)
			{

				btnCode.setVisibility(View.VISIBLE);
				btnCode.setText(" " + Utilities.TrimLeft(item.getItem().getCode(), '0'));
				// btnCode.setBackgroundResource(R.drawable.transp_button);
				btnCode.setTextColor(Color.BLACK);
				btnCode.setBackgroundResource(android.R.drawable.btn_default);
				btnCode.setOnClickListener(new OnClickListener()
				{

					public void onClick(View arg0)
					{
						((MainActivity) mContext).DisplayItemInfo(item);
					}
				});
			}

			if (btnQuantity != null)
			{
				btnQuantity.setVisibility(View.VISIBLE);
				btnQuantity.setText(RetailHelper.qtyFormat.format(item.getQty()) + " ");
				btnQuantity.setGravity(Gravity.RIGHT);
				btnQuantity.setBackgroundColor(Color.LTGRAY);
				btnQuantity.setBackgroundResource(android.R.drawable.btn_default);
				btnQuantity.setTextColor(Color.BLACK);
				// btnQuantity.setBackgroundResource(R.drawable.transp_button);
				if (item.getLinkedLine() == null)
				{
					btnQuantity.setOnClickListener(new OnClickListener()
					{

						public void onClick(View arg0)
						{
							changeDetailQty(item);
						}
					});
				}
			}

			if (barcodeView != null)
			{
				barcodeView.setText(Utilities.TrimLeft(item.getBarcode(), '0'));
				if (item.getLinkedLine() == null)
				{
					ibDelete.setVisibility(View.VISIBLE);
					ibDelete.setOnClickListener(new OnClickListener()
					{

						public void onClick(View arg0)
						{
							AlertDialog ad = new AlertDialog.Builder(mContext).create();
							ad.setCancelable(false); // This blocks the 'BACK'
														// button
							ad.setTitle("");
							ad.setMessage(mContext.getString(R.string.deleteConfirmation).replace("var1", item.getItem().getName()));
							ad.setButton(mContext.getString(android.R.string.cancel), new DialogInterface.OnClickListener()
							{
								public void onClick(DialogInterface dialog, int which)
								{
									dialog.dismiss();
								}
							});
							ad.setButton2(mContext.getString(android.R.string.ok), new DialogInterface.OnClickListener()
							{
								public void onClick(DialogInterface dialog, int which)
								{
									dialog.dismiss();
									deleteDetail(item);
								}
							});
							ad.show();
						}
					});
				}
				else
				{
					ibDelete.setVisibility(View.INVISIBLE);
				}
			}
			if (descriptionView != null)
			{
				descriptionView.setText(getTrimmedString(item.getItem().getName(), 45));
				if (q <= 0)
					descriptionView.setTextColor(Color.RED);
				else
					descriptionView.setTextColor(Color.BLACK);
			}

			if (unitpriceView != null)
			{
				unitpriceView.setText(RetailHelper.euroValue.format(item.getItemPrice()));
				if (q <= 0)
					unitpriceView.setTextColor(Color.RED);
				else
					unitpriceView.setTextColor(Color.BLACK);

			}

			if (tvatView != null)
			{
				tvatView.setText(RetailHelper.euroNormal.format(item.getTotalVatAmount()));
				if (q <= 0)
					tvatView.setTextColor(Color.RED);
				else
					tvatView.setTextColor(Color.BLACK);
			}
			if (grossView != null)
			{
				grossView.setText(RetailHelper.euroNormal.format(item.getGrossTotal()));
				if (q <= 0)
					grossView.setTextColor(Color.RED);
				else
					grossView.setTextColor(Color.BLACK);
			}
		}
		return v;
	}
}