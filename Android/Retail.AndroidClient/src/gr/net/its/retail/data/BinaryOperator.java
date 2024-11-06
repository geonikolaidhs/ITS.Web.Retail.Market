package gr.net.its.retail.data;

public class BinaryOperator
{
    String fieldName;
    Object fieldValue;
    FilterType filterType;
    
    public BinaryOperator(String fieldName,Object fieldValue,FilterType filterType)
    {
	this.fieldName = fieldName;
	this.fieldValue = fieldValue;
	this.filterType = filterType;
    }
    
    public BinaryOperator(String fieldName,Object fieldValue)
    {
	this.fieldName = fieldName;
	this.fieldValue = fieldValue;
	this.filterType = FilterType.EQUALS;
    }
    
    public String getFieldName()
    {
        return fieldName;
    }
    public void setFieldName(String fieldName)
    {
        this.fieldName = fieldName;
    }
    public Object getFieldValue()
    {
        return fieldValue;
    }
    public void setFieldValue(Object fieldValue)
    {
        this.fieldValue = fieldValue;
    }
    public FilterType getFilterType()
    {
        return filterType;
    }
    public void setFilterType(FilterType filterType)
    {
        this.filterType = filterType;
    }
}
