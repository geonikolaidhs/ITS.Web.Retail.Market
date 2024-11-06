/*package gr.net.its.common;

public class InputFilterDecimalMinMax
{

}*/

package gr.net.its.common;

import android.text.*;

public class InputFilterDecimalMinMax implements InputFilter {
	 
	private double min, max;
	private int decimals;
 
	public InputFilterDecimalMinMax(double min, double max, int decimals) {
		this.min = min;
		this.max = max;
		this.decimals = decimals;
	}
 
	public InputFilterDecimalMinMax(String min, String max, String decimals) {
		this.min = Double.parseDouble(min);
		this.max = Double.parseDouble(max);
		this.decimals = Integer.parseInt(decimals);
	}
 
	public CharSequence filter(CharSequence source, int start, int end, Spanned dest, int dstart, int dend) {	
		try {
			String strInput = dest.toString() + source.toString();
			double input = Double.parseDouble(strInput);
			if (isInRange(min, max, input, strInput, decimals))
				return null;
		} catch (NumberFormatException nfe) { }		
		return "";
	}
 
	private boolean isInRange(double mn, double mx, double input, String stringInput, int dec)
	{
		if(mn> input)
			return false;
		if(mx<input)
			return false;
		if(stringInput.contains("."))
		{
			if(dec <1)
				return false;
			int pos = stringInput.indexOf(".");
			int len = stringInput.length();
			if(pos-len>dec)
				return false;
			return true;
		}
		else
			return true;
	}
	
	/*private boolean isInRange(int a, int b, int c) {
		return b > a ? c >= a && c <= b : c >= b && c <= a;
	}*/
}
