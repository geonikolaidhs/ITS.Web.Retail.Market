package gr.net.its.common;

import java.io.*;
import java.net.URL;
import java.net.URLConnection;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.List;
import java.util.zip.*;

import javax.xml.transform.Source;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerException;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.stream.StreamResult;
import javax.xml.transform.stream.StreamSource;

import com.j256.ormlite.dao.Dao;
import com.j256.ormlite.dao.GenericRawResults;

import gr.net.its.retail.R;
import gr.net.its.retail.data.Customer;
import android.app.AlertDialog;
import android.content.Context;
import android.content.DialogInterface;
import android.graphics.Color;
import android.view.Gravity;
import android.webkit.WebView;
import android.widget.LinearLayout;
import android.widget.TextView;
import android.widget.Toast;

public class Utilities {
	public static String PadLeft(String stringToPad, int length,
			char paddingChar) {
		int strLength = stringToPad.length();
		int difference = length - strLength;
		if (difference <= 0) {
			return stringToPad;
		}
		String stringToAdd = "";
		for (int i = 0; i < difference; i++) {
			stringToAdd += paddingChar;
		}
		return stringToAdd.concat(stringToPad);
	}

	public static double RoundToDigits(double value, int places) {
		if (places < 0)
			throw new IllegalArgumentException();

		long factor = (long) Math.pow(10, places);
		value = value * factor;
		long tmp = Math.round(value);
		return (double) tmp / factor;
	}

	public static String TrimLeft(String value, char trimChar) {
		String returnValue = value.replaceAll("^" + trimChar + "+", "");
		return returnValue;
	}

	public static String TrimRight(String value, char trimChar) {
		String returnValue = value.replaceAll(trimChar + "+$", "");
		return returnValue;
	}

	public static void ShowUninterruptingMessage(Context context,
			String message, int duration) {
		ShowUninterruptingMessage(context, message, duration,
				(int)context.getResources().getDimension(
				R.dimen.toast_message_size));
	}

	public static void ShowUninterruptingMessage(Context context,
			String message, int duration, int textSize) {
		int loops = (int) Math.round(duration / 3.5); // Toast.LENGTH_LONG
		// equals 3.5 seconds
		Toast toast = null;
		if (loops <= 0) {
			if (duration <= 2) {
				toast = Toast.makeText(context, message, Toast.LENGTH_SHORT);

			} else {
				toast = Toast.makeText(context, message, Toast.LENGTH_LONG);
			}
			LinearLayout toastLayout = (LinearLayout) toast.getView();
			TextView toastTV = (TextView) toastLayout.getChildAt(0);
			toastTV.setTextSize(textSize);
			toastTV.setBackgroundColor(Color.parseColor("#ef4135"));
			toast.setGravity(Gravity.TOP | Gravity.CENTER, 0, 0);
			toast.show();
		} else {
			for (int i = 0; i < loops; i++) {
				toast = Toast.makeText(context, message, Toast.LENGTH_LONG);
				toast.setGravity(Gravity.TOP | Gravity.CENTER, 0, 0);
				LinearLayout toastLayout = (LinearLayout) toast.getView();
				TextView toastTV = (TextView) toastLayout.getChildAt(0);
				toastTV.setTextSize(textSize);
				toast.show();
			}

		}
	}

	public static void ShowSimpleDialog(Context context, String message) {
		ShowSimpleDialog(context, message, "",
				context.getString(android.R.string.ok), false);
	}

	public static void ShowSimpleDialog(Context context, String message,
			String title) {
		ShowSimpleDialog(context, message, title,
				context.getString(android.R.string.ok), false);
	}

	public static void ShowSimpleDialog(Context context, String message,
			String title, String buttonText) {
		ShowSimpleDialog(context, message, title, buttonText, false);
	}

	public static void ShowSimpleDialog(Context context, String message,
			String title, String buttonText, boolean cancelable) {
		AlertDialog ad = new AlertDialog.Builder(context).create();
		ad.setCancelable(cancelable); // This blocks the 'BACK' button
		ad.setTitle(title);
		ad.setMessage(message);
		ad.setButton(buttonText, new DialogInterface.OnClickListener() {
			public void onClick(DialogInterface dialog, int which) {
				// txtCode.setText("");
				dialog.dismiss();
			}
		});
		ad.show();
	}

	// public static DialogResult ShowDialog(Context context, String message)
	// {
	// return ShowDialog(context, message, "",
	// context.getString(android.R.string.ok),
	// context.getString(android.R.string.cancel),false);
	// }
	//
	// public static DialogResult ShowDialog(Context context, String message,
	// String title)
	// {
	// return ShowDialog(context, message, title,
	// context.getString(android.R.string.ok),context.getString(android.R.string.cancel),
	// false);
	// }
	//
	// public static DialogResult ShowDialog(Context context, String message,
	// String title, String okButtonText, String cancelButtonText)
	// {
	// return ShowDialog(context, message, title, okButtonText,cancelButtonText,
	// false);
	// }
	//
	// private static DialogResult result;
	//
	// public static DialogResult ShowDialog(Context context, String message,
	// String title, String okButtonText, String cancelButtonText, boolean
	// cancelable)
	// {
	// AlertDialog ad = new AlertDialog.Builder(context).create();
	// ad.setCancelable(cancelable); // This blocks the 'BACK' button
	// ad.setTitle(title);
	// ad.setMessage(message);
	// ad.setButton(cancelButtonText, new DialogInterface.OnClickListener()
	// {
	// public void onClick(DialogInterface dialog, int which)
	// {
	//
	// dialog.dismiss();
	// result = DialogResult.CANCEL;
	// }
	// });
	// ad.setButton2(okButtonText, new DialogInterface.OnClickListener()
	// {
	// public void onClick(DialogInterface dialog, int which)
	// {
	//
	// dialog.dismiss();
	// result = DialogResult.OK;
	// }
	// });
	// ad.show();
	// return result;
	// }

	public static void showWebViewDialog(Context context, String htmlMessage,
			String buttonText) {
		String string = htmlMessage;
		WebView wv = new WebView(context);
		wv.loadDataWithBaseURL(null, string, "text/html", "UTF-8", null);
		wv.setBackgroundColor(Color.WHITE);

		new AlertDialog.Builder(context)
				.setCancelable(false)
				.setView(wv)
				.setNeutralButton(buttonText,
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog,
									int which) {
								dialog.dismiss();
							}

						}).show();
	}

	public static void showWebViewDialog(Context context, WebView webView,
			String buttonText) {
		new AlertDialog.Builder(context)
				.setCancelable(false)
				.setView(webView)
				.setNeutralButton(buttonText,
						new DialogInterface.OnClickListener() {
							public void onClick(DialogInterface dialog,
									int which) {
								dialog.dismiss();
							}

						}).show();

	}

	public static long convertTicksToUnixTimestamp(long ticks) {
		return (ticks - 621355968000000000L) / 10000L;
	}

	public static long convertUnixTimestampToTicks(long timeInMillis) {
		return 621355968000000000L + (timeInMillis - timeInMillis % 86400000) * 10000;
	}

	public static String transformXML(Source xmlSource, Source xsltSource)
			throws TransformerException {

		// Source xmlSource = new
		// StreamSource(this.getResources().openRawResource(R.xml.source));
		// Source xsltSource = new
		// StreamSource(this.getResources().openRawResource(R.xml.products));

		TransformerFactory transFact = TransformerFactory.newInstance();
		Transformer trans = transFact.newTransformer(xsltSource);
		StringWriter output = new StringWriter();
		StreamResult result = new StreamResult(output);
		trans.transform(xmlSource, result);
		return output.toString();
	}

	public static void copyFile(InputStream input, OutputStream output)
			throws IOException {
		byte[] buff = new byte[1024];
		int read = 0;

		try {
			while ((read = input.read(buff)) > 0) {
				output.write(buff, 0, read);
			}
		} finally {
			input.close();
			output.close();
		}
	}

	public static void copyFile(String inputPath, String outputPath)
			throws IOException {
		byte[] buff = new byte[1024];
		int read = 0;
		FileInputStream input = null;
		FileOutputStream output = null;
		try {
			input = new FileInputStream(inputPath);
			output = new FileOutputStream(outputPath);

			while ((read = input.read(buff)) > 0) {
				output.write(buff, 0, read);
			}
		} finally {
			if (input != null)
				input.close();
			if (output != null)
				output.close();
		}
	}

}
