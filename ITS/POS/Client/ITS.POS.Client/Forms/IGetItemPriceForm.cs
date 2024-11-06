using ITS.POS.Model.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Forms
{
    public interface IGetItemPriceForm : IPOSForm
    {
        Item Item { get; set; }
        decimal Price { get; set; }
    }
}
