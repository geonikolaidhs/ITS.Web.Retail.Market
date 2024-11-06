using ITS.WRM.Model.Interface.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.Model.Interface
{
    public interface IDocumentPromotion: IBaseObj
    {
        IPromotion Promotion { get; set; }
        IDocumentHeader DocumentHeader { get; set; }
        string PromotionCode { get; set; }
        string PromotionDescription { get; set; }
        decimal TotalGain { get; set; }
        Guid DocumentHeaderOid { get; set; }
    }
}
