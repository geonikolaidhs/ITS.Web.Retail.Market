using ITS.Retail.Platform.Kernel;
using ITS.Retail.Platform.Kernel.Model;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Promotions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.Data.Filtering;
using ITS.POS.Resources;
using ITS.Retail.Platform.Kernel.Promotions;

namespace ITS.Retail.Platform.Kernel
{
    /// <summary>
    /// Platform-wide common promotion business logic handler.
    /// </summary>
    public class PlatformPromotionService : IPlatformPromotionService
    {
        public const int PRICE_CATALOG_DISCOUNT_PRIORITY = -1;

        public const int DOCUMENT_HEADER_DISCOUNT_PRIORITY = 999999999;

        public const int LOYALTY_DISCOUNT_PRIORITY = DOCUMENT_HEADER_DISCOUNT_PRIORITY - 1;

        public const int CUSTOMER_DISCOUNT_PRIORITY = LOYALTY_DISCOUNT_PRIORITY - 1;

        public const int DEFAULT_DOCUMENT_DISCOUNT_PRIORITY = CUSTOMER_DISCOUNT_PRIORITY - 1;

        public const int PROMOTIONS_DOCUMENT_DISCOUNT_PRIORITY = DEFAULT_DOCUMENT_DISCOUNT_PRIORITY - 1;

        public const int PROMOTIONS_LINE_DISCOUNT_PRIORITY = PROMOTIONS_DOCUMENT_DISCOUNT_PRIORITY - 1;


        public const eDiscountSource PROMOTION_LINE_DISCOUNT_SOURCE = eDiscountSource.PROMOTION_LINE_DISCOUNT;
        public const eDiscountSource PROMOTION_DOCUMENT_DISCOUNT_SOURCE = eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT;

        private IIntermediateDocumentService IntermediateDocumentService { get; set; }
        private IIntermediateModelManager IntermediateModelManager { get; set; }
        private IPlatformRoundingHandler PlatformRoundingHandler { get; set; }
        private IPlatformDocumentDiscountService PlatformDocumentDiscountService { get; set; }
        private Guid DefaultCustomerOid { get; set; }

        public PlatformPromotionService(IIntermediateDocumentService intermediateDocumentService, IIntermediateModelManager intermediateModelManager,
            IPlatformRoundingHandler platformRoundingHandler, IPlatformDocumentDiscountService platformDocumentDiscountService,
            Guid defaultCustomerOid)
        {
            IntermediateDocumentService = intermediateDocumentService;
            IntermediateModelManager = intermediateModelManager;
            PlatformRoundingHandler = platformRoundingHandler;
            DefaultCustomerOid = defaultCustomerOid;
            PlatformDocumentDiscountService = platformDocumentDiscountService;
        }

        /// <summary>
        /// Clears all the applied promotions to the given document header.
        /// </summary>
        /// <param name="header"></param>
        public void ClearDocumentPromotions(IDocumentHeader header)
        {
            if (header.DocumentPromotions.Count() > 0)
            {
                header.Session.Delete(header.DocumentPromotions);
                header.PromotionPoints = 0;
                header.PromotionsDiscountAmount = 0;
                header.PromotionsDiscountPercentage = 0;
                header.PromotionsDiscountPercentagePerLine = 0;

                foreach (IDocumentDetail detail in header.DocumentDetails)
                {
                    List<IDocumentDetailDiscount> discountsToDelete = detail.DocumentDetailDiscounts
                                                        .Where(disc => disc.DiscountSource == eDiscountSource.PROMOTION_LINE_DISCOUNT
                                                            || disc.DiscountSource == eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT).ToList();
                    if (discountsToDelete.Count > 0)
                    {
                        header.Session.Delete(discountsToDelete.ToList());
                        IntermediateDocumentService.RecalculateDocumentDetail(detail, header);
                    }
                }

                IntermediateDocumentService.RecalculateDocumentCosts(header, false);
                header.Session.CommitTransaction();
            }
        }

        /// <summary>
        /// Executes the promotions logic for the given document header.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="priceCatalog"></param>
        /// <param name="ownerApplicationSettings"></param>
        /// <param name="activeAtDate"></param>
        /// <param name="includeTestPromotions"></param>
        public void ExecutePromotions(IDocumentHeader header, IPriceCatalogPolicy priceCatalogPolicy, IOwnerApplicationSettings ownerApplicationSettings, DateTime activeAtDate, bool includeTestPromotions)
        {
            List<DenormalizedDocumentDataLine> denormalizedHeaderLines = this.DenormalizeDocument(header);
            List<DenormalizedCategory> itemCategoriesInPromotions = new List<DenormalizedCategory>();
            List<DenormalizedCategory> customerCategoriesInPromotions;
            IEnumerable<IPromotion> promotions = this.GetDocumentValidPromotions(priceCatalogPolicy, header, activeAtDate, ownerApplicationSettings.PromotionExecutionPriority, out itemCategoriesInPromotions, out customerCategoriesInPromotions, ownerApplicationSettings, includeTestPromotions);

            this.ClearDocumentPromotions(header);

            Dictionary<IPromotion, int> promotionsCount = promotions.Distinct().ToDictionary(prom => prom, prom => promotions.Count(x => x == prom));

            foreach (KeyValuePair<IPromotion, int> promotionWithCount in promotionsCount)
            {
                List<IDocumentDetail> linesToIgnore = new List<IDocumentDetail>();
                for (int i = 0; i < promotionWithCount.Value; i++)
                {
                    decimal totalGain = this.ApplyPromotionExecutions(header, promotionWithCount.Key, itemCategoriesInPromotions, ownerApplicationSettings);
                    if (totalGain >= 0)
                    {
                        IDocumentPromotion documentPromotion = IntermediateModelManager.CreatePersistentObject<IDocumentPromotion>();
                        documentPromotion.DocumentHeader = header;
                        documentPromotion.TotalGain = totalGain;
                        documentPromotion.PromotionOid = promotionWithCount.Key.Oid;
                        documentPromotion.PromotionCode = promotionWithCount.Key.Code;
                        documentPromotion.PromotionDescription = promotionWithCount.Key.Description;

                        documentPromotion.Save();
                    }
                }
            }

            IDiscountType promotionsDocumentDiscountType = IntermediateModelManager.GetObjectByKey<IDiscountType>(header.PromotionsDocumentDiscountTypeOid);
            if (promotionsDocumentDiscountType != null)
            {
                ////Because header has not been recalculated yet, we take the gross total before PROMOTION_DOCUMENT_DISCOUNT
                ////and subtract the promotion discounts applied to the lines (lines have been recalculated) to get the real "gross total before discount"

                decimal linePromotionsDiscount = header.DiscountableDocumentDetails().Sum(detail => detail.PromotionsLineDiscountsAmount);

                decimal validLinesGrossTotalBeforeDocumentDiscount = PlatformDocumentDiscountService.GetDocumentDetailsSumOfGrossTotalBeforeDiscountBySource(header, eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT)
                                                                    - linePromotionsDiscount;

                decimal grossTotalBeforeDocumentDiscount = PlatformDocumentDiscountService.GetDocumentHeaderGrossTotalBeforeDiscountBySource(header, eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT)
                                                            - header.PromotionsDiscountAmount - linePromotionsDiscount;

                if (validLinesGrossTotalBeforeDocumentDiscount > 0 && validLinesGrossTotalBeforeDocumentDiscount >= header.PromotionsDiscountAmount)
                {
                    if (promotionsDocumentDiscountType.eDiscountType == eDiscountType.PERCENTAGE)
                    {
                        decimal desiredDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(grossTotalBeforeDocumentDiscount * header.PromotionsDiscountPercentage);
                        header.PromotionsDiscountPercentagePerLine = PlatformRoundingHandler.RoundDisplayValue(desiredDiscountAmount / validLinesGrossTotalBeforeDocumentDiscount);
                        header.PromotionsDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(grossTotalBeforeDocumentDiscount * header.PromotionsDiscountPercentage);
                    }
                    else
                    {
                        header.PromotionsDiscountPercentage = header.PromotionsDiscountAmount / validLinesGrossTotalBeforeDocumentDiscount;
                        header.PromotionsDiscountPercentagePerLine = header.PromotionsDiscountPercentage;
                    }
                }
                else
                {
                    ////Promotion header discount CANNOT be applied
                    header.PromotionsDiscountAmount = 0;
                    header.PromotionsDiscountPercentage = 0;
                    header.PromotionsDiscountPercentagePerLine = 0;
                }
            }

            IntermediateDocumentService.RecalculateDocumentCosts(header, true, false);
            IntermediateDocumentService.FixPromotionsDocumentDiscountDeviations(header);

            if (header.GrossTotal < 0)
            {
                throw new DocumentNegativeTotalException();
            }

            header.Session.CommitTransaction();
        }

        /// <summary>
        /// Applies the executions of the given promotion to the given document header.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="promotion"></param>
        /// <param name="itemCategoriesInPromotions"></param>
        /// <param name="ownerApplicationSettings"></param>
        /// <returns></returns>
        private decimal ApplyPromotionExecutions(IDocumentHeader header, IPromotion promotion, List<DenormalizedCategory> itemCategoriesInPromotions, IOwnerApplicationSettings ownerApplicationSettings)
        {
            decimal totalCustomerValue = 0;
            UnitOfWork uow = promotion.Session as UnitOfWork;

            IEnumerable<IPromotionExecution> promotionExecutions = IntermediateModelManager.GetCollection<IPromotionExecution>(new BinaryOperator("Promotion", promotion.Oid));
            foreach (IPromotionExecution execution in promotionExecutions)
            {
                totalCustomerValue += this.CalculatePromotionExecutionValue(header, promotion, execution, true, itemCategoriesInPromotions, ownerApplicationSettings);
            }
            return totalCustomerValue;
        }

        /// <summary>
        /// Gets a list of the promotions that are valid for the given situation.
        /// </summary>
        /// <param name="priceCatalog"></param>
        /// <param name="header"></param>
        /// <param name="date"></param>
        /// <param name="priority"></param>
        /// <param name="itemCategoriesInPromotions"></param>
        /// <param name="customerCategoriesInPromotions"></param>
        /// <param name="ownerApplicationSettings"></param>
        /// <param name="includeTestPromotions"></param>
        /// <returns></returns>
        private IEnumerable<IPromotion> GetDocumentValidPromotions(/*IPriceCatalog priceCatalog,*/IPriceCatalogPolicy priceCatalogPolicy,
            IDocumentHeader header,
            DateTime date,
            ePromotionExecutionPriority priority,
            out List<DenormalizedCategory> itemCategoriesInPromotions,
            out List<DenormalizedCategory> customerCategoriesInPromotions,
            IOwnerApplicationSettings ownerApplicationSettings,
            bool includeTestPromotions)
        {
            if (priceCatalogPolicy == null)
            {
                throw new Exception(POSClientResources.STORE_HAS_NO_DEFAULT_PRICECATALOG);
            }

            IEnumerable<IPriceCatalogPolicyPromotion> priceCatalogPolicyPromotions = priceCatalogPolicy.PriceCatalogPolicyPromotions;
            CriteriaOperator promotionsCriteria = CriteriaOperator.And(new BinaryOperator("StartDate", date.Date, BinaryOperatorType.LessOrEqual),
                                     new BinaryOperator("EndDate", date.Date, BinaryOperatorType.GreaterOrEqual),
                                     new BinaryOperator("IsActive", true),
                                     new InOperator("Oid", priceCatalogPolicyPromotions.Select(x => x.PromotionOid)));

            if (includeTestPromotions == false)
            {
                promotionsCriteria = CriteriaOperator.And(promotionsCriteria, new BinaryOperator("TestMode", false));
            }

            IEnumerable<IPromotion> promotions = IntermediateModelManager.GetCollection<IPromotion>(promotionsCriteria);

            promotions = promotions.Where(prom => prom.StartTime.TimeOfDay <= date.TimeOfDay && prom.EndTime.TimeOfDay >= date.TimeOfDay
                                                  && prom.ActiveDaysOfWeek.HasFlag(date.DayOfWeek.ToDaysOfWeek()));

            List<DenormalizedDocumentDataLine> denormalizedLines = this.DenormalizeDocument(header);
            Dictionary<IPromotion, IPromotionCriteria> filteredPromotions = new Dictionary<IPromotion, IPromotionCriteria>();
            itemCategoriesInPromotions = new List<DenormalizedCategory>();
            customerCategoriesInPromotions = new List<DenormalizedCategory>();
            foreach (IPromotion IPromotion in promotions)
            {
                IPromotionCriteria filter = this.BuildPromotionCriteria(IPromotion, itemCategoriesInPromotions, customerCategoriesInPromotions);
                bool criteriaMet = filter.MeetCriteria(denormalizedLines, priority);
                if (criteriaMet)
                {
                    filteredPromotions.Add(IPromotion, filter);
                }
            }

            IEnumerable<IPromotion> validPromotions = this.ResolvePromotionConflicts(header, denormalizedLines, filteredPromotions, itemCategoriesInPromotions, ownerApplicationSettings);
            return validPromotions;
        }

        /// <summary>
        /// Calculates the value of the given promotion execution. If createDocumentDetailDiscounts is true, then it will also be applied to the document.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="promotion"></param>
        /// <param name="execution"></param>
        /// <param name="createDocumentDetailDiscounts"></param>
        /// <param name="itemCategoriesInPromotions"></param>
        /// <param name="ownerApplicationSettings"></param>
        /// <returns></returns>
        private decimal CalculatePromotionExecutionValue(IDocumentHeader header, IPromotion promotion,
            IPromotionExecution execution, bool createDocumentDetailDiscounts, List<DenormalizedCategory> itemCategoriesInPromotions, IOwnerApplicationSettings ownerApplicationSettings)
        {
            decimal value = 0;
            IDiscountType discountType = IntermediateModelManager.GetObjectByKey<IDiscountType>(execution.DiscountTypeOid);
            ePromotionExecutionPriority priority = ownerApplicationSettings.PromotionExecutionPriority;
            if (execution is IPromotionItemCategoryExecution)
            {
                IPromotionItemCategoryExecution itemCatExecution = (execution as IPromotionItemCategoryExecution);
                IEnumerable<IItemAnalyticTree> analyticTrees = IntermediateModelManager.GetCollection<IItemAnalyticTree>(new InOperator("Object", header.DocumentDetails.Where(x => x.IsTax == false && x.DoesNotAllowDiscount == false).Select(x => x.ItemOid)));//new XPCollection<ItemAnalyticTree>(SessionManager.GetSession<ItemAnalyticTree>(), new InOperator("Object", header.DocumentDetails.Select(x => x.Item)));
                DenormalizedCategory denormalizedIteCategory = itemCategoriesInPromotions.FirstOrDefault(ic => ic.CategoryID == itemCatExecution.ItemCategoryOid);
                if (denormalizedIteCategory == null)
                {
                    throw new Exception(String.Format("Promotion Execution data error - Item Category '{0}' is not in Promotion '{1}' rule: ", itemCatExecution.ItemCategoryOid, promotion.Oid));
                }
                IEnumerable<Guid> itemsWithCategory = analyticTrees.Where(iat => iat.NodeOid == denormalizedIteCategory.CategoryID ||
                                                                            denormalizedIteCategory.AllChildCategoryIDs.Contains(iat.NodeOid)).Select(iat => iat.ObjectOid).ToList();
                IEnumerable<IDocumentDetail> linesWithCategory = header.DiscountableDocumentDetails().Where(x => itemsWithCategory.Contains(x.ItemOid));
                if (itemCatExecution.ExecutionMode == eItemExecutionMode.DISCOUNT)
                {
                    value = CalculateLinesExecutionDiscountMode(header, promotion, itemCatExecution.OncePerItem, priority, createDocumentDetailDiscounts, discountType,
                        itemCatExecution.Value, itemCatExecution.Percentage, itemCatExecution.Quantity, linesWithCategory);
                }
                else
                {
                    value = CalculateLinesExecutionSetPriceMode(header, promotion, priority, createDocumentDetailDiscounts, discountType, itemCatExecution.Quantity, itemCatExecution.FinalUnitPrice, linesWithCategory);
                }
            }
            else if (execution is IPromotionItemExecution)
            {
                IPromotionItemExecution itemExecution = (execution as IPromotionItemExecution);
                IEnumerable<IDocumentDetail> linesWithItem = header.DiscountableDocumentDetails().Where(x => x.ItemOid == itemExecution.ItemOid);

                if (itemExecution.ExecutionMode == eItemExecutionMode.DISCOUNT)
                {
                    value = CalculateLinesExecutionDiscountMode(header, promotion, itemExecution.OncePerItem, priority, createDocumentDetailDiscounts, discountType, itemExecution.Value, itemExecution.Percentage, itemExecution.Quantity, linesWithItem);
                }
                else
                {
                    value = CalculateLinesExecutionSetPriceMode(header, promotion, priority, createDocumentDetailDiscounts, discountType, itemExecution.Quantity, itemExecution.FinalUnitPrice, linesWithItem);
                }
            }
            else if (execution is IPromotionDocumentExecution)
            {
                IPromotionDocumentExecution documentExecution = (execution as IPromotionDocumentExecution);
                if (discountType != null && (documentExecution.Value > 0 || documentExecution.Percentage > 0) && documentExecution.KeepOnlyPoints == false)
                {
                    value = CalculateHeaderExecutionWithDiscount(header, priority, createDocumentDetailDiscounts, discountType, documentExecution.Value, documentExecution.Percentage, ownerApplicationSettings);
                }
                else if (documentExecution.Points > 0 && documentExecution.KeepOnlyPoints == true)
                {
                    value = CalculateHeaderExecutionWithPoints(header, ownerApplicationSettings, createDocumentDetailDiscounts, documentExecution.Points);
                }
            }
            else if (execution is IPromotionPriceCatalogExecution)
            {
                IPromotionPriceCatalogExecution priceCatalogExecution = execution as IPromotionPriceCatalogExecution;
                if (discountType == null
                  || (priceCatalogExecution.Value <= 0
                   && priceCatalogExecution.Percentage <= 0
                     )
                   )
                {
                    throw new Exception(String.Format(POSClientResources.COULD_NOT_EXECUTE_PROMOTION, promotion.Description));
                }

                CriteriaOperator applicationRulesCriteria = new BinaryOperator("PromotionApplicationRuleGroupOid", promotion.PromotionApplicationRuleGroupOid);
                IEnumerable<IPromotionPriceCatalogApplicationRule> promotionApplicationRules = IntermediateModelManager.GetCollection<IPromotionPriceCatalogApplicationRule>(applicationRulesCriteria);
                if (promotionApplicationRules.Count() <= 0)
                {
                    throw new Exception(String.Format("Could not find PromotionApplicationRules for PromotionApplicationRuleGroup with Oid ", promotion.PromotionApplicationRuleGroupOid));
                }

                IEnumerable<string> promotionPriceCatalogs = promotionApplicationRules.SelectMany(rule => rule.PriceCatalogs.Trim().Split(',').Distinct()).Distinct();

                IEnumerable<IDocumentDetail> discountableDocumentDetails = header.DiscountableDocumentDetails().Where(detail => promotionPriceCatalogs.Contains(detail.PriceCatalog.ToString()));
                value = CalculateLinesPriceCatalogExecutionDiscount(header,
                                                                    promotion,
                                                                    promotionPriceCatalogs,
                                                                    priority,
                                                                    createDocumentDetailDiscounts,
                                                                    discountType,
                                                                    priceCatalogExecution.Value,
                                                                    priceCatalogExecution.Percentage,
                                                                    discountableDocumentDetails
                                                                    );
            }
            else
            {
                throw new NotImplementedException();
            }

            return value;
        }

        /// <summary>
        /// Calculates a promotion document execution that affects the document's points. If applyPointsToHeader is true,the calculated points are applied to the document.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="ownerApplicationSettings"></param>
        /// <param name="applyPointsToHeader"></param>
        /// <param name="points"></param>
        /// <returns></returns>
        private decimal CalculateHeaderExecutionWithPoints(IDocumentHeader header, IOwnerApplicationSettings ownerApplicationSettings, bool applyPointsToHeader, decimal points)
        {
            decimal valuePerPoint = 0;
            if (ownerApplicationSettings.DiscountAmount > 0 && ownerApplicationSettings.RefundPoints > 0)
            {
                valuePerPoint = ownerApplicationSettings.DiscountAmount / ownerApplicationSettings.RefundPoints;
            }
            decimal totalValue = valuePerPoint * points;
            if (applyPointsToHeader && header.CustomerOid != DefaultCustomerOid)
            {
                header.PromotionPoints += points;
            }

            IntermediateDocumentService.RecalculateDocumentCosts(header, false);
            return totalValue;
        }

        /// <summary>
        /// Calculates a promotion document execution that applies a document discount. If createDocumentDiscount is true, the calculated discount is applied to the document.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="priority"></param>
        /// <param name="createDocumentDiscount"></param>
        /// <param name="discountType"></param>
        /// <param name="executionValue"></param>
        /// <param name="executionPercentage"></param>
        /// <param name="settings"></param>
        /// <returns></returns>
        private decimal CalculateHeaderExecutionWithDiscount(IDocumentHeader header, ePromotionExecutionPriority priority, bool createDocumentDiscount, IDiscountType discountType, decimal executionValue, decimal executionPercentage, IOwnerApplicationSettings settings)
        {
            decimal documentDiscountAmount = 0;
            if (discountType.eDiscountType == eDiscountType.PERCENTAGE)
            {
                documentDiscountAmount = PlatformRoundingHandler.RoundDisplayValue(executionPercentage *
                    (PlatformDocumentDiscountService.GetDocumentHeaderGrossTotalBeforeDiscountBySource(header, eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT) - header.DiscountableDocumentDetails().Sum(detail => detail.PromotionsLineDiscountsAmount)));
            }
            else
            {
                decimal grossTotalBeforeDocumentDiscounts = PlatformDocumentDiscountService.GetDocumentHeaderGrossTotalBeforeDiscountBySource(header, eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT);

                if ((grossTotalBeforeDocumentDiscounts - executionValue) <= 0)
                {
                    ////Discount cannot be applied
                    documentDiscountAmount = 0;
                }
                else
                {
                    documentDiscountAmount = executionValue;
                }
            }

            if (createDocumentDiscount && documentDiscountAmount > 0)
            {
                decimal headerPromotionsDiscountsAmount = header.PromotionsDiscountPercentage > 0 && header.PromotionsDiscountAmount == 0 ?
                header.PromotionsDiscountPercentage * (PlatformDocumentDiscountService.GetDocumentHeaderGrossTotalBeforeDiscountBySource(header, eDiscountSource.PROMOTION_DOCUMENT_DISCOUNT) - header.DiscountableDocumentDetails().Sum(detail => detail.PromotionsLineDiscountsAmount))
                : header.PromotionsDiscountAmount;

                bool canApplyDiscount = (documentDiscountAmount >= headerPromotionsDiscountsAmount && priority == ePromotionExecutionPriority.BEST_VALUE_FOR_CUSTOMER) ||
                                        (documentDiscountAmount <= headerPromotionsDiscountsAmount && priority == ePromotionExecutionPriority.WORST_VALUE_FOR_CUSTOMER);

                if (canApplyDiscount)
                {
                    ApplyPromotionsDocumentHeaderDiscount(ref header, (discountType.eDiscountType == eDiscountType.PERCENTAGE ? executionPercentage : executionValue), discountType);
                }
            }

            return documentDiscountAmount;
        }

        /// <summary>
        /// Calculates and applies the DocumentDiscountPercentagePerLine that must be applied to all the lines
        /// </summary>
        /// <param name="header"></param>
        /// <param name="discount"></param>
        /// <param name="appSettings"></param>
        /// <param name="discountType"></param>
        private void ApplyPromotionsDocumentHeaderDiscount(ref IDocumentHeader header, decimal discount, IDiscountType discountType)
        {
            if (discountType.eDiscountType == eDiscountType.PERCENTAGE)
            {
                header.PromotionsDiscountPercentage = discount;
            }
            else
            {
                header.PromotionsDiscountAmount = discount;
            }

            header.PromotionsDocumentDiscountTypeOid = discountType.Oid;
        }

        /// <summary>
        /// Calculates a promotion execution that affects document lines by adding a line discount. If applyExecution is true, the execution is applied to the document.
        /// Does not Recompute Header. Header should be recomputed AFTER all the promotions have been executed
        /// </summary>
        /// <param name="header"></param>
        /// <param name="promotion"></param>
        /// <param name="priority"></param>
        /// <param name="applyExecution"></param>
        /// <param name="discountType"></param>
        /// <param name="executionValue"></param>
        /// <param name="executionPercentage"></param>
        /// <param name="itemQty"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        private decimal CalculateLinesExecutionDiscountMode(IDocumentHeader header, IPromotion promotion, bool executeOncePerItemPerPromotion, ePromotionExecutionPriority priority,
                                                      bool applyExecution, IDiscountType discountType, decimal executionValue, decimal executionPercentage, decimal itemQty,
                                                        IEnumerable<IDocumentDetail> lines)
        {
            decimal totalValue = 0;

            if (discountType.eDiscountType == eDiscountType.VALUE)
            {
                lines.ToList().ForEach(line => line.CurrentPromotionDiscountValue = (line.GrossTotalBeforeDocumentDiscount / line.Qty));
                lines = lines.Where(line => line.GrossTotalBeforeDocumentDiscount > 0 && (line.CurrentPromotionDiscountValue >= executionValue)).ToList();
                if (priority == ePromotionExecutionPriority.BEST_VALUE_FOR_CUSTOMER)
                {
                    lines = lines.OrderByDescending(line => line.CurrentPromotionDiscountValue).ToList();
                }
                else if (priority == ePromotionExecutionPriority.WORST_VALUE_FOR_CUSTOMER)
                {
                    lines = lines.OrderBy(line => line.CurrentPromotionDiscountValue).ToList();
                }
            }
            else if (discountType.eDiscountType == eDiscountType.PERCENTAGE)
            {
                lines.ToList().ForEach(line => line.CurrentPromotionDiscountValue = (executionPercentage * (line.GrossTotalBeforeDocumentDiscount + line.PromotionsLineDiscountsAmount) / line.Qty));
                lines = lines.Where(line => line.GrossTotalBeforeDocumentDiscount > 0 &&
                                            (line.GrossTotalBeforeDocumentDiscount >= line.CurrentPromotionDiscountValue)).ToList();
                if (priority == ePromotionExecutionPriority.BEST_VALUE_FOR_CUSTOMER)
                {
                    lines = lines.OrderByDescending(line => line.CurrentPromotionDiscountValue).ToList();
                }
                else if (priority == ePromotionExecutionPriority.WORST_VALUE_FOR_CUSTOMER)
                {
                    lines = lines.OrderBy(line => line.CurrentPromotionDiscountValue).ToList();
                }
            }

            if (executeOncePerItemPerPromotion)
            {
                lines = lines.Where(x => x.DocumentDetailDiscounts.Where(y => y.PromotionOid == promotion.Oid).Count() < x.Qty).ToList();
            }

            decimal remainingQty = itemQty;
            int i = 0;
            var linesList = lines.ToArray();
            while ((remainingQty > 0) && linesList.Length > i)
            {
                IDocumentDetail line = linesList[i];
                i++;
                if (discountType.eDiscountType == eDiscountType.PERCENTAGE)
                {
                    decimal amountPerUnit = (line.GrossTotalBeforeDocumentDiscount + line.PromotionsLineDiscountsAmount) / line.Qty;
                    decimal lineDiscountPerUnit = PlatformRoundingHandler.RoundDisplayValue(amountPerUnit * executionPercentage);
                    decimal value = (remainingQty > line.Qty) ? (lineDiscountPerUnit * line.Qty) : (remainingQty * lineDiscountPerUnit);
                    totalValue += value;
                    if (applyExecution)
                    {
                        IDocumentDetailDiscount detailDiscount = CreatePromotionLineDiscountValue(discountType, value, line, promotion);
                        detailDiscount.PromotionOid = promotion.Oid;
                        detailDiscount.DocumentDetail = line;
                        detailDiscount.Save();
                        IntermediateDocumentService.RecalculateDocumentDetail(line, header);
                    }
                }
                else if (discountType.eDiscountType == eDiscountType.VALUE)
                {
                    decimal value = (remainingQty > line.Qty) ? (executionValue * line.Qty) : (remainingQty * executionValue);
                    totalValue += value;
                    if (applyExecution)
                    {
                        IDocumentDetailDiscount detailDiscount = CreatePromotionLineDiscountValue(discountType, value, line, promotion);
                        detailDiscount.PromotionOid = promotion.Oid;
                        detailDiscount.DocumentDetail = line;
                        detailDiscount.Save();
                        IntermediateDocumentService.RecalculateDocumentDetail(line, header);
                    }
                }
                remainingQty -= line.Qty;
            }

            return totalValue;
        }

        /// <summary>
        /// Creates a promotion line discount.
        /// </summary>
        /// <param name="discountType"></param>
        /// <param name="value"></param>
        /// <param name="detail"></param>
        /// <param name="promotion"></param>
        /// <returns></returns>
        private IDocumentDetailDiscount CreatePromotionLineDiscountValue(IDiscountType discountType, decimal value, IDocumentDetail detail, IPromotion promotion)
        {
            IDocumentDetailDiscount discount = IntermediateModelManager.CreatePersistentObject<IDocumentDetailDiscount>();

            decimal percentage = 0;
            eDiscountSource discountSource = PROMOTION_LINE_DISCOUNT_SOURCE;
            bool discardOtherDiscounts = discountType == null ? false : discountType.DiscardsOtherDiscounts;
            string promoDescription = String.IsNullOrEmpty(promotion.PrintedDescription) ? promotion.Description : promotion.PrintedDescription;
            return PlatformDocumentDiscountService.UpdateDiscount(discount, percentage, value, PROMOTIONS_LINE_DISCOUNT_PRIORITY, discountSource,
                eDiscountType.VALUE, discountType, discardOtherDiscounts, promoDescription);
        }

        /// <summary>
        ///  Calculates a promotion execution that affects document lines by altering their price. If applyExecution is true, the execution is applied to the document.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="promotion"></param>
        /// <param name="priority"></param>
        /// <param name="applyExecution"></param>
        /// <param name="discountType"></param>
        /// <param name="itemQty"></param>
        /// <param name="unitPrice"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        private decimal CalculateLinesExecutionSetPriceMode(IDocumentHeader header, IPromotion promotion, ePromotionExecutionPriority priority,
                                              bool applyExecution, IDiscountType discountType, decimal itemQty, decimal unitPrice, IEnumerable<IDocumentDetail> lines)
        {
            decimal totalValue = 0;
            if (discountType != null)
            {
                if (discountType.eDiscountType == eDiscountType.VALUE)
                {
                    if (priority == ePromotionExecutionPriority.BEST_VALUE_FOR_CUSTOMER)
                    {
                        lines = lines.OrderByDescending(line => (line.GrossTotalBeforeDocumentDiscount / line.Qty));
                    }
                    else if (priority == ePromotionExecutionPriority.WORST_VALUE_FOR_CUSTOMER)
                    {
                        lines = lines.OrderBy(line => (line.GrossTotalBeforeDocumentDiscount / line.Qty));
                    }
                }
                else
                {
                    throw new Exception(POSClientResources.PROMOTION + ": " + promotion.Code + " - \"" + promotion.Description + "\" (" + promotion.Oid + ")" +
                                        " " + POSClientResources.ONLY_VALUE_TYPE_DISCOUNTS_ARE_ALLOWED_IN_SET_PRICE_EXECUTION);
                }
            }


            lines = lines.Where(x => x.DocumentDetailDiscounts.Where(y => y.PromotionOid == promotion.Oid).Count() < x.Qty);

            decimal remainingQty = itemQty;
            int linePosition = 0;
            while ((remainingQty > 0) && lines.Count() > linePosition)
            {
                IDocumentDetail line = lines.ElementAt(linePosition);
                linePosition++;
                decimal executionValue = line.FinalUnitPrice - unitPrice;
                decimal value = (remainingQty > line.Qty) ? (executionValue * line.Qty) : (remainingQty * executionValue);
                totalValue += value;
                if (applyExecution)
                {
                    IDocumentDetailDiscount detailDiscount = CreatePromotionLineDiscountValue(discountType, value, line, promotion);
                    detailDiscount.PromotionOid = promotion.Oid;
                    detailDiscount.DocumentDetail = line;
                    detailDiscount.Save();
                    IntermediateDocumentService.RecalculateDocumentDetail(line, header);
                    detailDiscount.Session.CommitTransaction();
                }
                remainingQty -= line.Qty;
            }

            return totalValue;
        }

        /// <summary>
        /// Resolves the logical conflicts of the given promotionsWithCriteria list that are valid for the given document and returns the final list of promotions to be executed.
        /// </summary>
        /// <param name="header"></param>
        /// <param name="denormalizedDocument"></param>
        /// <param name="promotionsWithCriteria"></param>
        /// <param name="itemCategoriesInPromotions"></param>
        /// <param name="ownerApplicationSettings"></param>
        /// <returns></returns>
        private IEnumerable<IPromotion> ResolvePromotionConflicts(IDocumentHeader header, List<DenormalizedDocumentDataLine> denormalizedDocument,
            Dictionary<IPromotion, IPromotionCriteria> promotionsWithCriteria, List<DenormalizedCategory> itemCategoriesInPromotions,
            IOwnerApplicationSettings ownerApplicationSettings)
        {
            Dictionary<IPromotion, List<IPromotionConstrain>> constrains = new Dictionary<IPromotion, List<IPromotionConstrain>>();
            ePromotionExecutionPriority priority = ownerApplicationSettings.PromotionExecutionPriority;
            foreach (KeyValuePair<IPromotion, IPromotionCriteria> pair in promotionsWithCriteria)
            {
                constrains.Add(pair.Key, pair.Value.GetConstrains(denormalizedDocument, priority));
            }

            Dictionary<IPromotion, decimal> promotionGains = new Dictionary<IPromotion, decimal>();
            foreach (KeyValuePair<IPromotion, IPromotionCriteria> pair in promotionsWithCriteria)
            {
                IEnumerable<IPromotionExecution> promotionExecutions = IntermediateModelManager.GetCollection<IPromotionExecution>(new BinaryOperator("Promotion", pair.Key.Oid));
                decimal promotionGain = 0;
                List<IDocumentDetail> linesToIgnore = new List<IDocumentDetail>();

                foreach (IPromotionExecution execution in promotionExecutions)
                {
                    promotionGain += this.CalculatePromotionExecutionValue(header, pair.Key, execution, false, itemCategoriesInPromotions, ownerApplicationSettings);
                }
                promotionGains.Add(pair.Key, promotionGain);
            }

            List<IPromotion> orderedPromotions = null;
            if (priority == ePromotionExecutionPriority.BEST_VALUE_FOR_CUSTOMER)
            {
                orderedPromotions = promotionGains.OrderByDescending(prom => prom.Value).Select(prom => prom.Key).ToList();
            }
            else if (priority == ePromotionExecutionPriority.WORST_VALUE_FOR_CUSTOMER)
            {
                orderedPromotions = promotionGains.OrderBy(prom => prom.Value).Select(prom => prom.Key).ToList();
            }
            else
            {
                throw new NotSupportedException();
            }

            List<DenormalizedDocumentDataLine> linesAfterConstrains = denormalizedDocument;
            List<IPromotion> result = new List<IPromotion>();
            int conflictCounter = 0;
            const int MAX_TRIES = 9999;
            int tries = 0;
            Dictionary<IPromotion, int> promotionExecutionTimes = new Dictionary<IPromotion, int>();
            do
            {
                conflictCounter = 0;
                foreach (IPromotion promotion in orderedPromotions)
                {
                    if (promotionExecutionTimes.ContainsKey(promotion) && promotionExecutionTimes[promotion] >= promotion.MaxExecutionsPerReceipt)
                    {
                        conflictCounter++;
                        continue;
                    }

                    List<IPromotionConstrain> promotionConstrains = constrains[promotion];
                    List<DenormalizedDocumentDataLine> currentPromotionAffectedLines = linesAfterConstrains.Clone();
                    foreach (IPromotionConstrain constrain in promotionConstrains)
                    {
                        currentPromotionAffectedLines = constrain.GetLinesAfterConstrain(currentPromotionAffectedLines);
                    }

                    if (currentPromotionAffectedLines.Where(x => x.ItemTotalValue < 0 || x.ItemTotalQuantity < 0 || x.DocumentGrossTotalBeforeDocumentDiscount < 0).Count() > 0)
                    {
                        ////constrain violation Detected
                        conflictCounter++;
                    }
                    else
                    {
                        foreach (IPromotionConstrain constrain in promotionConstrains)
                        {
                            linesAfterConstrains = constrain.GetLinesAfterConstrain(linesAfterConstrains);
                        }
                        result.Add(promotion);
                        if (promotionExecutionTimes.ContainsKey(promotion))
                        {
                            promotionExecutionTimes[promotion] += 1;
                        }
                        else
                        {
                            promotionExecutionTimes[promotion] = 1;
                        }
                        //Go from the start
                        break;
                    }
                }
                tries++;
            } while (conflictCounter < orderedPromotions.Count && tries < MAX_TRIES);

            if (tries == MAX_TRIES)
            {
                throw new Exception("Promotion loop detected - Max tries reached. Promotions: " + String.Join("," + Environment.NewLine, promotionsWithCriteria.Select(pair => pair.Key.Code + " - ")));
            }

            return result;
        }

        /// <summary>
        /// Converts the given document header to a denormalized form to be used with IPromotionCriteria
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private List<DenormalizedDocumentDataLine> DenormalizeDocument(IDocumentHeader header)
        {
            if (header == null)
            {
                throw new ArgumentNullException("header");
            }

            List<DenormalizedDocumentDataLine> result = new List<DenormalizedDocumentDataLine>();
            ICustomer customer = IntermediateModelManager.GetObjectByKey<ICustomer>(header.CustomerOid);
            if (customer == null)
            {
                throw new ArgumentException("Document header has no customer defined", "header");
            }

            foreach (var group in header.DocumentDetails.Where(x => x.IsCanceled == false && x.IsTax == false && x.DoesNotAllowDiscount == false).GroupBy(detail => new { Item = detail.ItemOid, PriceCatalog = detail.PriceCatalog }))
            {
                IEnumerable<IItemAnalyticTree> analyticTrees = IntermediateModelManager.GetCollection<IItemAnalyticTree>(new BinaryOperator("Object", group.Key.Item));
                List<Guid> allItemCategories = analyticTrees.Select(iat => iat.NodeOid).ToList();
                IEnumerable<ICustomerAnalyticTree> customerAnalyticTrees = IntermediateModelManager.GetCollection<ICustomerAnalyticTree>(new BinaryOperator("Object", header.CustomerOid));
                List<Guid> allCustomerCategories = customerAnalyticTrees.Select(x => x.NodeOid).ToList();

                decimal itemTotalQty = group.Sum(x => x.Qty);
                decimal itemTotalValue = group.Sum(x => x.GrossTotalBeforeDocumentDiscount);

                if (itemTotalQty > 0)
                {
                    result.Add(new DenormalizedDocumentDataLine(header, customer, group.Key.Item, allItemCategories, itemTotalQty, itemTotalValue, allCustomerCategories, group.Key.PriceCatalog));
                }
            }

            return result;
        }

        /// <summary>
        /// Builds the criteria for the given promotion.
        /// </summary>
        /// <param name="promotion"></param>
        /// <param name="itemCategoriesInPromotions"></param>
        /// <param name="customerCategoriesInPromotions"></param>
        /// <returns></returns>
        private IPromotionCriteria BuildPromotionCriteria(IPromotion promotion,
            List<DenormalizedCategory> itemCategoriesInPromotions,
            List<DenormalizedCategory> customerCategoriesInPromotions)
        {
            UnitOfWork uow = promotion.Session as UnitOfWork;
            IPromotionApplicationRuleGroup rootGroup = IntermediateModelManager.GetObjectByKey<IPromotionApplicationRuleGroup>(promotion.PromotionApplicationRuleGroupOid);

            if (rootGroup != null)
            {
                IPromotionCriteria resultCriteria = BuildGroupCriteria(rootGroup, itemCategoriesInPromotions, customerCategoriesInPromotions);
                return resultCriteria;
            }
            else
            {
                throw new Exception(String.Format(POSClientResources.ROOT_APPLICATION_RULE_GROUP_NOT_FOUND, promotion.Description));
            }
        }

        /// <summary>
        /// Recursive function that calculates the criteria for the given promotion application rule group.
        /// </summary>
        /// <param name="currentGroup"></param>
        /// <param name="itemCategoriesInPromotions"></param>
        /// <param name="customerCategoriesInPromotions"></param>
        /// <param name="parentGroupCriteria"></param>
        /// <returns></returns>
        private IPromotionCriteria BuildGroupCriteria(IPromotionApplicationRuleGroup currentGroup,
            List<DenormalizedCategory> itemCategoriesInPromotions,
            List<DenormalizedCategory> customerCategoriesInPromotions,
            IPromotionCriteria parentGroupCriteria = null)
        {
            UnitOfWork uow = currentGroup.Session as UnitOfWork;
            IEnumerable<IPromotionApplicationRuleGroup> childRuleGroups = IntermediateModelManager.GetCollection<IPromotionApplicationRuleGroup>(new BinaryOperator("ParentPromotionApplicationRuleGroupOid", currentGroup.Oid));

            IPromotionCriteria resultCriteria = null;
            switch (currentGroup.Operator)
            {
                case eGroupOperatorType.And:
                    resultCriteria = new PromotionAndCriteria();
                    break;

                case eGroupOperatorType.Or:
                    resultCriteria = new PromotionOrCriteria();
                    break;

                case eGroupOperatorType.Not:
                    resultCriteria = new PromotionNotCriteria();
                    break;
            }

            this.BuildGroupApplicationRulesCriteria(currentGroup, itemCategoriesInPromotions, customerCategoriesInPromotions, resultCriteria);

            foreach (IPromotionApplicationRuleGroup childRule in childRuleGroups)
            {
                IPromotionCriteria childCriteria = BuildGroupCriteria(childRule, itemCategoriesInPromotions, customerCategoriesInPromotions, resultCriteria);

                if (resultCriteria is PromotionAndCriteria)
                {
                    (resultCriteria as PromotionAndCriteria).CriteriaList.Add(childCriteria);
                }
                else if (resultCriteria is PromotionOrCriteria)
                {
                    (resultCriteria as PromotionOrCriteria).CriteriaList.Add(childCriteria);
                }
                else if (resultCriteria is PromotionNotCriteria)
                {
                    (resultCriteria as PromotionNotCriteria).CriteriaList.Add(childCriteria);
                }
            }

            if (parentGroupCriteria is PromotionAndCriteria)
            {
                (parentGroupCriteria as PromotionAndCriteria).CriteriaList.Add(resultCriteria);
            }
            else if (parentGroupCriteria is PromotionOrCriteria)
            {
                (parentGroupCriteria as PromotionOrCriteria).CriteriaList.Add(resultCriteria);
            }
            else if (parentGroupCriteria is PromotionNotCriteria)
            {
                (parentGroupCriteria as PromotionNotCriteria).CriteriaList.Add(resultCriteria);
            }

            return resultCriteria;
        }

        /// <summary>
        /// Gets recursivelly all child categories of a given category.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="category"></param>
        /// <returns></returns>
        private List<Guid> GetAllChildCategories<T>(Guid category) where T : ICategoryNode
        {
            List<Guid> result = new List<Guid>();
            IEnumerable<T> childCategories = IntermediateModelManager.GetCollection<T>(new BinaryOperator("ParentOid", category));
            result = childCategories.Select(cat => cat.Oid).ToList();
            foreach (Guid childCategory in result.ToList())
            {
                result.AddRange(GetAllChildCategories<T>(childCategory));
            }

            return result;
        }

        /// <summary>
        /// Calculates the criteria for the given promotion application rule group, at the current group level (without AND/OR other groups).
        /// </summary>
        /// <param name="group"></param>
        /// <param name="itemCategoriesInPromotions"></param>
        /// <param name="customerCategoriesInPromotions"></param>
        /// <param name="groupCriteria"></param>
        private void BuildGroupApplicationRulesCriteria(IPromotionApplicationRuleGroup group,
            List<DenormalizedCategory> itemCategoriesInPromotions,
            List<DenormalizedCategory> customerCategoriesInPromotions,
            IPromotionCriteria groupCriteria)
        {
            UnitOfWork uow = group.Session as UnitOfWork;
            IEnumerable<IPromotionApplicationRule> allRules = IntermediateModelManager.GetCollection<IPromotionApplicationRule>(new BinaryOperator("PromotionApplicationRuleGroup", group.Oid));

            foreach (IPromotionApplicationRule rule in allRules)
            {
                IPromotionCriteria currentRuleFilter = null;
                if (rule is IPromotionItemApplicationRule)
                {
                    IPromotionItemApplicationRule itemRule = (rule as IPromotionItemApplicationRule);
                    currentRuleFilter = new PromotionItemCriteria(itemRule.ItemOid, itemRule.Quantity, itemRule.Value);
                }
                else if (rule is IPromotionItemCategoryApplicationRule)
                {
                    IPromotionItemCategoryApplicationRule itemCategoryRule = (rule as IPromotionItemCategoryApplicationRule);
                    DenormalizedCategory denormItemCategory = itemCategoriesInPromotions.FirstOrDefault(ic => ic.CategoryID == itemCategoryRule.ItemCategoryOid);
                    if (denormItemCategory == null)
                    {
                        denormItemCategory = new DenormalizedCategory(itemCategoryRule.ItemCategoryOid, GetAllChildCategories<IItemCategory>(itemCategoryRule.ItemCategoryOid));
                        itemCategoriesInPromotions.Add(denormItemCategory);
                    }

                    currentRuleFilter = new PromotionItemCategoryCriteria(denormItemCategory, itemCategoryRule.Quantity, itemCategoryRule.Value);
                }
                else if (rule is IPromotionCustomerApplicationRule)
                {
                    IPromotionCustomerApplicationRule customerRule = (rule as IPromotionCustomerApplicationRule);
                    currentRuleFilter = new PromotionCustomerCriteria(customerRule.CustomerOid);
                }
                else if (rule is IPromotionDocumentApplicationRule)
                {
                    IPromotionDocumentApplicationRule documentRule = (rule as IPromotionDocumentApplicationRule);
                    currentRuleFilter = new PromotionDocumentCriteria(documentRule.Value, documentRule.ValueIsRepeating);
                }
                else if (rule is IPromotionCustomerCategoryApplicationRule)
                {
                    IPromotionCustomerCategoryApplicationRule customerRule = rule as IPromotionCustomerCategoryApplicationRule;

                    DenormalizedCategory denormCustomerCategory = customerCategoriesInPromotions.FirstOrDefault(ic => ic.CategoryID == customerRule.CustomerCategoryOid);
                    if (denormCustomerCategory == null)
                    {
                        denormCustomerCategory = new DenormalizedCategory(customerRule.CustomerCategoryOid, GetAllChildCategories<ICustomerCategory>(customerRule.CustomerCategoryOid));
                        customerCategoriesInPromotions.Add(denormCustomerCategory);
                    }
                    currentRuleFilter = new PromotionCustomerCategoryCriteria(denormCustomerCategory);
                }
                else if (rule is IPromotionPriceCatalogApplicationRule)
                {
                    IPromotionPriceCatalogApplicationRule priceCatalogApplicationRule = rule as IPromotionPriceCatalogApplicationRule;
                    currentRuleFilter = new PromotionPriceCatalogCriteria(priceCatalogApplicationRule.PriceCatalogs);
                }

                if (currentRuleFilter == null)
                {
                    throw new NotImplementedException("Filter for " + rule.GetType().Name + " is not implemented");
                }

                if (groupCriteria is PromotionAndCriteria)
                {
                    (groupCriteria as PromotionAndCriteria).CriteriaList.Add(currentRuleFilter);
                }
                else if (groupCriteria is PromotionOrCriteria)
                {
                    (groupCriteria as PromotionOrCriteria).CriteriaList.Add(currentRuleFilter);
                }
                else if (groupCriteria is PromotionNotCriteria)
                {
                    (groupCriteria as PromotionNotCriteria).CriteriaList.Add(currentRuleFilter);
                }
            }
        }

        /// <summary>
        /// Calculates a promotion execution that affects document lines by adding a line discount. If applyExecution is true, the execution is applied to the document.
        /// Does not Recompute Header. Header should be recomputed AFTER all the promotions have been executed
        /// </summary>
        /// <param name="header"></param>
        /// <param name="promotion"></param>
        /// <param name="priority"></param>
        /// <param name="promotionPriceCatalogs"></param>
        /// <param name="applyExecution"></param>
        /// <param name="discountType"></param>
        /// <param name="executionValue"></param>
        /// <param name="executionPercentage"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        private decimal CalculateLinesPriceCatalogExecutionDiscount(IDocumentHeader header, IPromotion promotion, IEnumerable<string> promotionPriceCatalogs, ePromotionExecutionPriority priority,
                                                                    bool applyExecution, IDiscountType discountType,
                                                                    decimal executionValue, decimal executionPercentage,
                                                                    IEnumerable<IDocumentDetail> lines
        )
        {
            if (discountType.eDiscountType == eDiscountType.VALUE)
            {
                lines.ToList().ForEach(line => line.CurrentPromotionDiscountValue = (line.GrossTotalBeforeDocumentDiscount / line.Qty));
                lines = lines.Where(line => line.GrossTotalBeforeDocumentDiscount > 0 && (line.CurrentPromotionDiscountValue >= executionValue)).ToList();
                if (priority == ePromotionExecutionPriority.BEST_VALUE_FOR_CUSTOMER)
                {
                    lines = lines.OrderByDescending(line => line.CurrentPromotionDiscountValue).ToList();
                }
                else if (priority == ePromotionExecutionPriority.WORST_VALUE_FOR_CUSTOMER)
                {
                    lines = lines.OrderBy(line => line.CurrentPromotionDiscountValue).ToList();
                }
            }
            else if (discountType.eDiscountType == eDiscountType.PERCENTAGE)
            {
                lines.ToList().ForEach(line => line.CurrentPromotionDiscountValue = (executionPercentage * (line.GrossTotalBeforeDocumentDiscount + line.PromotionsLineDiscountsAmount) / line.Qty));
                lines = lines.Where(line => line.GrossTotalBeforeDocumentDiscount > 0 &&
                                            (line.GrossTotalBeforeDocumentDiscount >= line.CurrentPromotionDiscountValue)).ToList();
                if (priority == ePromotionExecutionPriority.BEST_VALUE_FOR_CUSTOMER)
                {
                    lines = lines.OrderByDescending(line => line.CurrentPromotionDiscountValue).ToList();
                }
                else if (priority == ePromotionExecutionPriority.WORST_VALUE_FOR_CUSTOMER)
                {
                    lines = lines.OrderBy(line => line.CurrentPromotionDiscountValue).ToList();
                }
            }

            lines = lines.Where(documentDetail => documentDetail.DocumentDetailDiscounts.Where(documentDetailDiscount => documentDetailDiscount.PromotionOid == promotion.Oid).Count() < documentDetail.Qty).ToList();

            decimal totalValue = 0;

            foreach (IDocumentDetail documentDetail in lines)
            {
                decimal value = 0;
                switch (discountType.eDiscountType)
                {
                    case eDiscountType.PERCENTAGE:
                        decimal amountPerUnit = (documentDetail.GrossTotalBeforeDocumentDiscount + documentDetail.PromotionsLineDiscountsAmount) / documentDetail.Qty;
                        value = PlatformRoundingHandler.RoundDisplayValue(amountPerUnit * executionPercentage);
                        break;

                    case eDiscountType.VALUE:
                        value = executionValue;
                        break;

                    default:
                        throw new NotSupportedException(String.Format("CalculateLinesPriceCatalogExecutionDiscount {0}", discountType.eDiscountType));
                }
                totalValue += documentDetail.Qty * value;
                if (value > 0 && applyExecution)
                {
                    IDocumentDetailDiscount detailDiscount = CreatePromotionLineDiscountValue(discountType, value, documentDetail, promotion);
                    detailDiscount.PromotionOid = promotion.Oid;
                    detailDiscount.DocumentDetail = documentDetail;
                    detailDiscount.Save();
                    IntermediateDocumentService.RecalculateDocumentDetail(documentDetail, header);
                }
            }
            return totalValue;
        }
    }
}