package gr.net.its.retail.data;

import gr.net.its.common.Utilities;

import java.util.Calendar;
import java.util.GregorianCalendar;

public class TDocHead
{
	private long ticks;
	private double netTotal, discountTotal, grossTotal;
	public long getTicks()
	{
		return ticks;
	}
	public void setTicks(long ticks)
	{
		this.ticks = ticks;
	}
	public double getNetTotal()
	{
		return netTotal;
	}
	public void setNetTotal(double netTotal)
	{
		this.netTotal = netTotal;
	}
	public double getDiscountTotal()
	{
		return discountTotal;
	}
	public void setDiscountTotal(double discountTotal)
	{
		this.discountTotal = discountTotal;
	}
	public double getGrossTotal()
	{
		return grossTotal;
	}
	public void setGrossTotal(double grossTotal)
	{
		this.grossTotal = grossTotal;
	}

	public Calendar getFinalizeDate()
	{
		GregorianCalendar g = new GregorianCalendar();
		g.setTimeInMillis(Utilities.convertTicksToUnixTimestamp(ticks));
		return g;
	}
}
