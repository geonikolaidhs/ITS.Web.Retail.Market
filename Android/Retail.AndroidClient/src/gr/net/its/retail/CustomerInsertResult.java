package gr.net.its.retail;

public enum CustomerInsertResult
{
    OTHERERROR(-100), NETWORKERROR(-99), GSIS_SERVICE_ERROR(-8),CUSTOMERCODENOTEXIST(-7), PRICECATALOGNOTINSTORE(-6), PRICECATALOGNOTFOUND(-5), USERHASNOSTOREACCESS(-4), INVALIDUSER(-3), TAXCODEFOUNDNOACCESS(-2), TAXCODENOTFOUND(-1), TAXCODEEXISTS(0), TAXCODECREATED(1);

    private final int id;

    CustomerInsertResult(int id)
    {
	this.id = id;
    }

    static CustomerInsertResult fromValue(int value)
    {
	for (CustomerInsertResult my : CustomerInsertResult.values())
	{
	    if (my.id == value)
	    {
		return my;
	    }
	}

	return OTHERERROR;
    }
}