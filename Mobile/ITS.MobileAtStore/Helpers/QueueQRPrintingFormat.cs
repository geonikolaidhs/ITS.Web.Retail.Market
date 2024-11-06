using System;

using System.Collections.Generic;
using System.Text;
using ITS.MobileAtStore.ObjectModel;

namespace ITS.MobileAtStore.Helpers
{
    public class DocumentHeaderContainer
    {
        public List<Line> AllLines { get; set; }
        public List<Line> InactiveLines { get; set; }
        public List<Line> ActiveLines { get; set; }
        public Header Header { get; set; }
    }

    public class QueueQRPrintingFormat
    {
        public string AdaptiveText { get; set; }
        public string LargeText { get; set; }
        public string LargestText { get; set; }
        public string NormalText { get; set; }
        public string QrCode { get; set; }
        public string Barcode { get; set; }
        public string OrientationFix { get; set; }
        public int QRBlockSize { get; set; }
        public List<QueueQRPrintingFormatElement> PrintFormat { get; set; }
        public List<string> IgnorePrefixes { get; set; }

        public string GetPrintFormat(Header header)
        {
            DocumentHeaderContainer container = new DocumentHeaderContainer()
            {
                Header = header,
                ActiveLines = header.Lines.FindAll(x=>x.FoundOnline && x.Qty1>0),
                InactiveLines = header.Lines.FindAll(x=>!x.FoundOnline),
                AllLines = header.Lines
            };
            StringBuilder builder = new StringBuilder(100000);
            foreach (QueueQRPrintingFormatElement printElement in PrintFormat)
            {
                builder.Append(printElement.GetFormat(container, this));
            }
            return builder.ToString();
        }
    }
}
