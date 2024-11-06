using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Transactions;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Resources;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Kernel;

namespace ITS.POS.Client.Helpers
{
    static class DocumentDetailDiscountHelper
    {
        public static DocumentDetailDiscountViewModel Convert(DocumentDetailDiscount detail, ISessionManager sessionManager, IPlatformRoundingHandler platformRoundingHandler)
        {
            DocumentDetailDiscountViewModel model = new DocumentDetailDiscountViewModel();
            model.Value = platformRoundingHandler.RoundDisplayValue(detail.Value);
            model.Percentage = detail.Percentage.ToString("p2");
            switch (detail.DiscountSource)
            {
                case eDiscountSource.CUSTOM:
                    DiscountType type = sessionManager.GetObjectByKey<DiscountType>(detail.Type);
                    model.Description = type.Description != null ? type.Description.ToUpperGR() : "";
                    break;
                default:
                    model.Description = detail.DiscountSource.ToLocalizedString().ToUpperGR();
                    break;
            }

            return model;
        }
    }
}
