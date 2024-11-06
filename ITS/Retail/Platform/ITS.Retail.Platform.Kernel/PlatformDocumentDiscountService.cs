using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.Retail.Platform.Kernel
{
    /// <summary>
    /// Platform-wide common discount handling service.
    /// </summary>
    public class PlatformDocumentDiscountService : IPlatformDocumentDiscountService
    {
        protected UnitOfWork ItemUnitOfWork { get; set; }

        /// <summary>
        /// Updates the given discount.
        /// </summary>
        /// <param name="discount"></param>
        /// <param name="percentage"></param>
        /// <param name="value"></param>
        /// <param name="priority"></param>
        /// <param name="source"></param>
        /// <param name="eDiscountType"></param>
        /// <param name="discountType"></param>
        /// <param name="discardsOtherDiscounts"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public IDocumentDetailDiscount UpdateDiscount(IDocumentDetailDiscount discount, decimal percentage, decimal value, int priority, eDiscountSource source, eDiscountType eDiscountType, IDiscountType discountType, bool discardsOtherDiscounts, string description = null)
        {
            discount.Percentage = percentage;
            discount.Value = value;
            discount.TypeOid = (discountType == null ? Guid.Empty : discountType.Oid);
            discount.Description = description;
            discount.TypeDescription = (discountType == null ? null : discountType.Description.ToUpperGR());
            discount.DiscountType = eDiscountType;
            discount.DiscardsOtherDiscounts = discardsOtherDiscounts;
            discount.Priority = priority;
            discount.DiscountSource = source;
            return discount;
        }

        /// <summary>
        /// Gets the gross total before discount of a document header, returning the appropriate amount for the given source.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public decimal GetDocumentHeaderGrossTotalBeforeDiscountBySource(IDocumentHeader header, eDiscountSource source)
        {
            if (source == eDiscountSource.DOCUMENT)
            {
                ////Applied last
                return header.GrossTotal + header.DocumentDiscountAmount - GetTotalNonDiscountableValue(header);
            }
            else if (source == eDiscountSource.POINTS)
            {
                ////Applied after customer discount
                return header.GrossTotal + header.DocumentDiscountAmount + header.PointsDiscountAmount - GetTotalNonDiscountableValue(header);
            }
            else if (source == eDiscountSource.CUSTOMER)
            {
                ////Applied after default document discount discount
                return header.GrossTotal + header.DocumentDiscountAmount +
                                           header.PointsDiscountAmount +
                                           header.CustomerDiscountAmount -
                                           GetTotalNonDiscountableValue(header);
            }
            else if (source == eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT)
            {
                ////Applied after promotion discounts
                return header.GrossTotal + header.DocumentDiscountAmount +
                                           header.PointsDiscountAmount +
                                           header.CustomerDiscountAmount +
                                           header.DefaultDocumentDiscountAmount -
                                           GetTotalNonDiscountableValue(header);
            }
            else if (source == eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT)
            {
                ////Applied first
                return header.GrossTotal + header.DocumentDiscountAmount +
                                           header.PointsDiscountAmount +
                                           header.CustomerDiscountAmount +
                                           header.DefaultDocumentDiscountAmount +
                                           header.PromotionsDiscountAmount -
                                           GetTotalNonDiscountableValue(header);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets sum of the gross total before discount of a document header's details, returning the appropriate amount for the given source.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="source"></param>
        /// <returns></returns>
        public decimal GetDocumentDetailsSumOfGrossTotalBeforeDiscountBySource(IDocumentHeader header, eDiscountSource source)
        {
            IEnumerable<IDocumentDetail> validDetails = header.DiscountableDocumentDetails();

            if (source == eDiscountSource.DOCUMENT)
            {
                ////Applied last
                return validDetails.Sum(docDet => docDet.GrossTotal +
                                                  docDet.DocumentDiscountAmount);
            }
            else if (source == eDiscountSource.POINTS)
            {
                ////Applied after customer discount
                return validDetails.Sum(docDet => docDet.GrossTotal +
                                                  docDet.DocumentDiscountAmount +
                                                  docDet.PointsDiscountAmount);
            }
            else if (source == eDiscountSource.CUSTOMER)
            {
                ////Applied after default document discount discount
                return validDetails.Sum(docDet => docDet.GrossTotal +
                                                  docDet.PointsDiscountAmount +
                                                  docDet.DocumentDiscountAmount +
                                                  docDet.CustomerDiscountAmount);
            }
            else if (source == eDiscountSource.DEFAULT_DOCUMENT_DISCOUNT)
            {
                ////Applied after promotion discounts
                return validDetails.Sum(docDet => docDet.GrossTotal +
                                                  docDet.PointsDiscountAmount +
                                                  docDet.CustomerDiscountAmount +
                                                  docDet.DocumentDiscountAmount +
                                                  docDet.DefaultDocumentDiscountAmount);
            }
            else if (source == eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT)
            {
                ////Applied first
                return validDetails.Sum(docDet => docDet.GrossTotal +
                                                    docDet.DefaultDocumentDiscountAmount +
                                                    docDet.CustomerDiscountAmount +
                                                    docDet.PointsDiscountAmount +
                                                    docDet.DocumentDiscountAmount +
                                                    docDet.PromotionsDocumentDiscountAmount);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Returns the Non Discountable Value of Document
        /// </summary>
        /// <param name="header"></param>
        public decimal GetTotalNonDiscountableValue(IDocumentHeader document)
        {
            decimal amount = 0;
            foreach (IDocumentDetail det in document.DocumentDetails.Where(x => x.IsTax || x.DoesNotAllowDiscount))
            {
                if (det.IsCanceled == false && det.IsReturn == false)
                    amount += det.GrossTotal;
            }
            return amount;
        }
    }
}