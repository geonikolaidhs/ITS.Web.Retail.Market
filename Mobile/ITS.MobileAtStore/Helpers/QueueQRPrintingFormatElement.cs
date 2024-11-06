using System;

using System.Collections.Generic;
using System.Text;
using ITS.MobileAtStore.ObjectModel;
using System.Xml;
using System.Diagnostics;
using ITS.MobileAtStore.AuxilliaryClasses;
using System.Xml.Serialization;
using System.IO;

namespace ITS.MobileAtStore.Helpers
{

    public class MyStringBuilder
    {
        private StringBuilder builder;
        public MyStringBuilder()
        {
            builder = new StringBuilder();
        }
        public MyStringBuilder(int size)
        {

            builder = new StringBuilder(size);
        }
        public void Append(string text)
        {
            //DateTime start, stop;
            //start = DateTime.Now;
            builder.Append(text);
            //stop = DateTime.Now;
            //Debug.WriteLine(string.Format("Appending took {0} ticks. Text Length {1}. Builder Length {2}", stop.Ticks - start.Ticks, text.Length, builder.Length));
        }
        public override string ToString()
        {
            //DateTime start, stop;
            //start = DateTime.Now;
            //string str = builder.ToString();
            //stop = DateTime.Now;
            //Debug.WriteLine(string.Format("Appending took {0} ticks. Text Length {1}.", stop.Ticks - start.Ticks, str.Length));
            return builder.ToString();
        }
    }

    public abstract class QueueQRPrintingFormatElement
    {
        public QueueQRPrintingFormatElementType ElementType { get; set; }

        public abstract string GetFormat(DocumentHeaderContainer documentheader, QueueQRPrintingFormat settings);

        public static QueueQRPrintingFormatElement GetElement(XmlNode node)
        {
            switch (node.Name)
            {
                case "OrientationFix":
                    return new OrientationFixQEPrintingFormatElemet();
                case "LargeText":
                    return new LargeTextQueueQRPrintingFormatElement() { Text = node.InnerText };
                case "LargestText":
                    return new LargestTextQueueQRPrintingFormatElement() { Text = node.InnerText };
                case "AdaptiveText":
                    {
                        string size = node.Attributes["size"] == null ? "" : node.Attributes["size"].Value;
                        string fontParameters = node.Attributes["fontParameters"] == null ? "" : node.Attributes["fontParameters"].Value;
                        return new AdaptiveTextQueueQRPrintingFormatElement(size, fontParameters) { Text = node.InnerText };
                    }
                case "NormalText":
                    return new NormalTextQueueQRPrintingFormatElement() { Text = node.InnerText };
                case "QRBlock":
                    {
                        string prefix = node.Attributes["prefix"] == null ? "" : node.Attributes["prefix"].Value;
                        string suffix = node.Attributes["suffix"] == null ? "" : node.Attributes["suffix"].Value;
                        string separator = node.Attributes["separator"] == null ? "" : node.Attributes["separator"].Value;
                        string type = node.Attributes["type"] == null ? "" : node.Attributes["type"].Value;
                        string qtySeparator = node.Attributes["qtySeparator"] == null ? "" : node.Attributes["qtySeparator"].Value;
                        string barcodePrefix = node.Attributes["barcodePrefix"] == null ? "" : node.Attributes["barcodePrefix"].Value;
                        string format = node.InnerText;
                        TextQueueQRPrintingFormatElement element = type == "LargeText" ? new LargeTextQueueQRPrintingFormatElement() as TextQueueQRPrintingFormatElement : new NormalTextQueueQRPrintingFormatElement() as TextQueueQRPrintingFormatElement;
                        return new QRBlockQueueQRPrintingFormatElement(element, format, prefix, suffix, separator, qtySeparator, barcodePrefix);
                    }
                case "NotFoundItem":
                    {
                        string type = node.Attributes["type"] == null ? "" : node.Attributes["type"].Value;
                        TextQueueQRPrintingFormatElement element = type == "LargeText" ? new LargeTextQueueQRPrintingFormatElement() as TextQueueQRPrintingFormatElement : new NormalTextQueueQRPrintingFormatElement() as TextQueueQRPrintingFormatElement;
                        return new NotFoundItemQueueQRPrintingFormatElement(element, node.InnerText);
                    }
                case "Summary":
                    {
                        string type = node.Attributes["type"] == null ? "" : node.Attributes["type"].Value;
                        TextQueueQRPrintingFormatElement element = type == "LargeText" ? new LargeTextQueueQRPrintingFormatElement() as TextQueueQRPrintingFormatElement : new NormalTextQueueQRPrintingFormatElement() as TextQueueQRPrintingFormatElement;
                        return new SummaryQueueQRPrintingFormatElement(element, node.InnerText);
                    }
                default:
                    return null;
            }
        }
    }

    public abstract class TextQueueQRPrintingFormatElement : QueueQRPrintingFormatElement
    {
        public string Text { get; set; }
    }

    public class NormalTextQueueQRPrintingFormatElement : TextQueueQRPrintingFormatElement
    {
        public NormalTextQueueQRPrintingFormatElement() { ElementType = QueueQRPrintingFormatElementType.NormalText; }

        public override string GetFormat(DocumentHeaderContainer documentheader, QueueQRPrintingFormat settings)
        {
            return string.Format(settings.NormalText, this.Text);
        }
    }

    public class AdaptiveTextQueueQRPrintingFormatElement : TextQueueQRPrintingFormatElement
    {
        protected string ElementSize {get;set;}
        protected string FontParameters { get; set; }

        public AdaptiveTextQueueQRPrintingFormatElement(string elementsize, string fontparameters)
        {
            ElementType = QueueQRPrintingFormatElementType.Adaptive;
            ElementSize = (string.IsNullOrEmpty(elementsize.Trim())) ? "15" : elementsize;
            FontParameters = (string.IsNullOrEmpty(fontparameters.Trim())) ? "0,15" : fontparameters;
            
        }

        public override string GetFormat(DocumentHeaderContainer documentheader, QueueQRPrintingFormat settings)
        {
            return string.Format(settings.AdaptiveText, this.Text, this.ElementSize, this.FontParameters);
        }
    }

    public class LargeTextQueueQRPrintingFormatElement : TextQueueQRPrintingFormatElement
    {
        public LargeTextQueueQRPrintingFormatElement() { ElementType = QueueQRPrintingFormatElementType.LargeText; }
        public override string GetFormat(DocumentHeaderContainer documentheader, QueueQRPrintingFormat settings)
        {
            return string.Format(settings.LargeText, this.Text);
        }
    }

    public class LargestTextQueueQRPrintingFormatElement : TextQueueQRPrintingFormatElement
    {
        public LargestTextQueueQRPrintingFormatElement() { ElementType = QueueQRPrintingFormatElementType.Largest; }
        public override string GetFormat(DocumentHeaderContainer documentheader, QueueQRPrintingFormat settings)
        {
            return string.Format(settings.LargestText, this.Text);
        }
    }

    public class QRBlockQueueQRPrintingFormatElement : QueueQRPrintingFormatElement
    {
        protected TextQueueQRPrintingFormatElement TextElementType { get; set; }
        //protected int BlockSize { get; set; }
        protected string Prefix { get; set; }
        protected string Suffix { get; set; }
        protected string Separator { get; set; }
        protected string QtySeparator { get; set; }
        protected string Format { get; set; }
        protected string BarcodePrefix { get; set; }

        public QRBlockQueueQRPrintingFormatElement(TextQueueQRPrintingFormatElement textElementType, string format, string prefix, string suffix, string separator, string qtyseparator, string barcodePrefix)
        {
            ElementType = QueueQRPrintingFormatElementType.QRBlock;
            // BlockSize = blockSize;
            TextElementType = textElementType;
            Prefix = prefix;
            Suffix = suffix;
            Separator = separator;
            QtySeparator = qtyseparator;
            Format = format;
            BarcodePrefix = barcodePrefix;
        }    

        public override string GetFormat(DocumentHeaderContainer documentheader, QueueQRPrintingFormat settings)
        {
            //Separator = ",";
            //Prefix = ",";            
            settings.QrCode = "^XA ^LL415 ^FO,0,0 ^BQ,2,4,Q ^FDQA,{0}^FS ^XZ";
            if (documentheader.ActiveLines.Count == 0)
            {
                throw new Exception("Δεν έχετε εισάγει είδη");
            }
            int maxBlockCounter = (int)Math.Ceiling(documentheader.AllLines.Count * 1.0 / settings.QRBlockSize), blockCounter = 0;
            MyStringBuilder finalString = new MyStringBuilder((maxBlockCounter + 1) * settings.QRBlockSize * settings.QrCode.Length);
            MyStringBuilder internalString = null, barcodeString = null;
            for (int i = 0; i < documentheader.ActiveLines.Count; i++)
            {
                if (i % settings.QRBlockSize == 0)
                {
                    if (i > 0)
                    {                        
                        internalString.Append(string.Format(settings.QrCode, barcodeString.ToString()));
                        finalString.Append(internalString.ToString());
                    }
                    internalString = new MyStringBuilder(2 * settings.QRBlockSize * settings.QrCode.Length);
                    barcodeString = new MyStringBuilder(2 * settings.QRBlockSize * settings.QrCode.Length);
                    blockCounter++;
                    TextElementType.Text = string.Format(this.Format, Math.Min(settings.QRBlockSize, documentheader.ActiveLines.Count - i), blockCounter, maxBlockCounter);
                    internalString.Append(TextElementType.GetFormat(documentheader, settings));
                    barcodeString.Append("-9-9-9-");
                    barcodeString.Append("-" + documentheader.Header.CustomerCode+"-");                   
                }
                barcodeString.Append(Separator);           
                barcodeString.Append(documentheader.ActiveLines[i].ScannedCode);     
                barcodeString.Append(QtySeparator);
                string qty = documentheader.ActiveLines[i].Qty1.ToString().Replace(",",".");
                barcodeString.Append(qty);             
            }
            internalString.Append(string.Format(settings.QrCode, barcodeString.ToString()));
            finalString.Append(internalString.ToString());
            return finalString.ToString();
        }
    }

    public class NotFoundItemQueueQRPrintingFormatElement : QueueQRPrintingFormatElement
    {
        protected TextQueueQRPrintingFormatElement TextElementType { get; set; }
        protected string Format { get; set; }
        public NotFoundItemQueueQRPrintingFormatElement(TextQueueQRPrintingFormatElement textElementType, string format)
        {
            ElementType = QueueQRPrintingFormatElementType.NotFoundItem;
            TextElementType = textElementType;
            Format = format;
        }
        public override string GetFormat(DocumentHeaderContainer documentheader, QueueQRPrintingFormat settings)
        {
            StringBuilder builder = new StringBuilder();
            if (documentheader.InactiveLines.Count > 0)
            {
                TextElementType.Text = string.Format(this.Format, documentheader.InactiveLines.Count);
                builder.Append(TextElementType.GetFormat(documentheader, settings));
                foreach (Line line in documentheader.InactiveLines)
                {
                    for (int i = 0; i < line.Qty1; i++)
                    {
                        builder.Append(string.Format(settings.Barcode, line.ScannedCode));
                    }
                }
            }
            return builder.ToString();
        }
    }

    public class SummaryQueueQRPrintingFormatElement : QueueQRPrintingFormatElement
    {
        protected TextQueueQRPrintingFormatElement TextElementType { get; set; }
        protected string Format { get; set; }
        public SummaryQueueQRPrintingFormatElement(TextQueueQRPrintingFormatElement textElementType, string format)
        {
            ElementType = QueueQRPrintingFormatElementType.Summary;
            TextElementType = textElementType;
            Format = format;
        }
        public override string GetFormat(DocumentHeaderContainer documentheader, QueueQRPrintingFormat settings)
        {
            decimal totalQty=0;
            foreach(Line line in documentheader.Header.Lines)
            {
                totalQty = totalQty + line.Qty1;
            }
            string now = DateTime.Now.ToString();
            TextElementType.Text = string.Format(Format, totalQty, documentheader.AllLines.Count, now);
            return TextElementType.GetFormat(documentheader, settings);
        }
    }

    public class OrientationFixQEPrintingFormatElemet : QueueQRPrintingFormatElement
    {
        public override string GetFormat(DocumentHeaderContainer documentheader, QueueQRPrintingFormat settings)
        {

            return settings.OrientationFix;
        }
    }

}
