using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Kernel
{
    public class PlatformPriceCatalogDetailService
    {
        public PlatformPriceCatalogDetailService()
        {

        }

        public ValidationPriceCatalogDetailTimeValuesResult ValidatePriceCatalogDetailTimeValues(IEnumerable<IPriceCatalogDetailTimeValue> priceCatalogDetailTimeValues)
        {
            IPriceCatalogDetailTimeValue erroneous = priceCatalogDetailTimeValues.FirstOrDefault(x => x.TimeValueValidFromDate > x.TimeValueValidUntilDate);
            if (erroneous != null)
            {
                return new ValidationPriceCatalogDetailTimeValuesResult()
                {
                    From = erroneous.TimeValueValidFromDate,
                    To = erroneous.TimeValueValidUntilDate,
                    FromGreaterThanTo = true
                };
            }
            IEnumerable<IPriceCatalogDetailTimeValue> activePriceCatalogDetailTimeValues = priceCatalogDetailTimeValues.Where(timeValue => timeValue.IsActive);
            if (activePriceCatalogDetailTimeValues.Count() > 1)
            {
                foreach (IPriceCatalogDetailTimeValue timeValueRange in activePriceCatalogDetailTimeValues)
                {
                    if (activePriceCatalogDetailTimeValues.Any(x => x != timeValueRange && (
                                             (x.TimeValueValidFromDate <= timeValueRange.TimeValueValidFromDate
                                          && x.TimeValueValidUntilDate >= timeValueRange.TimeValueValidFromDate
                                          && x.TimeValueValidUntilDate <= timeValueRange.TimeValueValidUntilDate)
                                          || ( x.TimeValueValidFromDate == timeValueRange.TimeValueValidFromDate 
                                            && x.TimeValueValidUntilDate == timeValueRange.TimeValueValidUntilDate ))

                                          ))
                    {
                        return new ValidationPriceCatalogDetailTimeValuesResult()
                        {
                            From = timeValueRange.TimeValueValidFromDate,
                            To = timeValueRange.TimeValueValidUntilDate,
                            PartialOverlap = true
                        };
                    }
                }
            }

            return null;
        }
    }

    public class ValidationPriceCatalogDetailTimeValuesResult
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public bool PartialOverlap { get; set; }
    public bool FromGreaterThanTo { get; set; }
}
}
