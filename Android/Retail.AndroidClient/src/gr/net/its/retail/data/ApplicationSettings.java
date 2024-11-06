package gr.net.its.retail.data;

import gr.net.its.retail.MainActivity;

import com.j256.ormlite.field.DatabaseField;
import com.j256.ormlite.table.DatabaseTable;

@DatabaseTable
public class ApplicationSettings 
{

	@DatabaseField(generatedId=true)
	private long ID;
	@DatabaseField
	private String serviceUrl;
	@DatabaseField
	private boolean barcodePadding;
	@DatabaseField
	private boolean codePadding;
	@DatabaseField
	private char barcodePadChar;
	@DatabaseField
	private char codePadChar;
	@DatabaseField
	private int barcodePadLength;
	@DatabaseField
	private int codePadLength;
	@DatabaseField
	private int computeDigits;
	@DatabaseField
	private int computeValueDigits;
	@DatabaseField
	private int displayDigits;
	@DatabaseField
	private int displayValueDigits;
	@DatabaseField
	private String bluetoothDeviceName; 
	@DatabaseField
	private boolean discountPermited;
	@DatabaseField
	private boolean defaultAllowMultiLines;
	@DatabaseField
	private String pathToXSL;
	@DatabaseField(foreign = true)
	private DocumentStatus defaultDocumentStatus;
	
	@DatabaseField(foreign = true)
	private PriceCatalog defaultPriceCatalog;
	
	@DatabaseField(foreign = true)
	private Store defaultStore;	
	
	public PriceCatalog getDefaultPriceCatalog()
	{
		return defaultPriceCatalog;
	}

	public void setDefaultPriceCatalog(PriceCatalog defaultPriceCatalog)
	{
		this.defaultPriceCatalog = defaultPriceCatalog;
	}

	public Store getDefaultStore()
	{
		return defaultStore;
	}

	public void setDefaultStore(Store defaultStore)
	{
		this.defaultStore = defaultStore;
	}

	public int getBarcodePadLength()
	{
		return barcodePadLength;
	}

	public void setBarcodePadLength(int barcodePadLength)
	{
		this.barcodePadLength = barcodePadLength;
	}

	public int getCodePadLength()
	{
		return codePadLength;
	}

	public void setCodePadLength(int codePadLength)
	{
		this.codePadLength = codePadLength;
	}

	public long getID()
	{
		return ID;
	}

	public void setID(long iD)
	{
		ID = iD;
	}

	public String getServiceUrl()
	{
		return serviceUrl;
	}

	public void setServiceUrl(String serviceUrl)
	{
		this.serviceUrl = serviceUrl;
	}

	public boolean isBarcodePadding()
	{
		return barcodePadding;
	}

	public void setBarcodePadding(boolean barcodePadding)
	{
		this.barcodePadding = barcodePadding;
	}

	public boolean isCodePadding()
	{
		return codePadding;
	}

	public void setCodePadding(boolean codePadding)
	{
		this.codePadding = codePadding;
	}

	public char getBarcodePadChar()
	{
		return barcodePadChar;
	}

	public void setBarcodePadChar(char barcodePadChar)
	{
		this.barcodePadChar = barcodePadChar;
	}

	public char getCodePadChar()
	{
		return codePadChar;
	}

	public void setCodePadChar(char codePadChar)
	{
		this.codePadChar = codePadChar;
	}

	public int getComputeDigits()
	{
	    return computeDigits;
	}

	public void setComputeDigits(int computeDigits)
	{
	    this.computeDigits = computeDigits;
	}

	public int getComputeValueDigits()
	{
	    return computeValueDigits;
	}

	public void setComputeValueDigits(int computeValueDigits)
	{
	    this.computeValueDigits = computeValueDigits;
	}

	public int getDisplayDigits()
	{
	    return displayDigits;
	}

	public void setDisplayDigits(int displayDigits)
	{
	    this.displayDigits = displayDigits;
	}

	public int getDisplayValueDigits()
	{
	    return displayValueDigits;
	}

	public void setDisplayValueDigits(int displayValueDigits)
	{
	    this.displayValueDigits = displayValueDigits;
	}

	public String getBluetoothDeviceName()
	{
	    return bluetoothDeviceName;
	}

	public void setBluetoothDeviceName(String bluetoothDeviceName)
	{
	    this.bluetoothDeviceName = bluetoothDeviceName;
	}

	public boolean isDiscountPermited()
	{
	    return discountPermited;
	}

	public void setDiscountPermited(boolean discountPermited)
	{
	    this.discountPermited = discountPermited;
	}

	public boolean getDefaultAllowMultiLines()
	{
	    return defaultAllowMultiLines;
	}

	public void setDefaultAllowMultiLines(boolean defaultAllowMultiLines)
	{
	    this.defaultAllowMultiLines = defaultAllowMultiLines;
	}

	public String getPathToXSL()
	{
	    return pathToXSL == null || pathToXSL.isEmpty() ? MainActivity.DEFAULT_XSL_PATH : pathToXSL;
	}

	public void setPathToXSL(String pathToXSL)
	{
	    this.pathToXSL = pathToXSL;
	}

	public DocumentStatus getDefaultDocumentStatus()
	{
	    return defaultDocumentStatus;
	}

	public void setDefaultDocumentStatus(DocumentStatus defaultDocumentStatus)
	{
	    this.defaultDocumentStatus = defaultDocumentStatus;
	}

}
