//using System;

//using System.Collections.Generic;
//using System.Text;
//using ITS.MobileAtStore.ObjectModel;

//namespace ITS.MobileAtStore.Helpers
//{
//    public static class LabelManager
//    {
//        private static string Separator = ",";
//        private static string Prefix = ",";
//        private static string QrCode = "^XA ^LL415 ^FO,0,0 ^BQ,2,4,Q ^FDQA,{0}^FS ^XZ";
//        private static int QRBlockSize = 25;

//        public static string CreateLabel(Header header)
//        {
 
//        }

//        public static string CreateQRCode(Header header)
//        {       
//            if (header.Lines.Count == 0)
//            {
//                throw new Exception("Δεν έχετε εισάγει είδη");
//            }
//            int maxBlockCounter = (int)Math.Ceiling(header.Lines.Count * 1.0 / QRBlockSize), blockCounter = 0;
//            MyStringBuilder finalString = new MyStringBuilder((maxBlockCounter + 1) * QRBlockSize * QrCode.Length);
//            MyStringBuilder internalString = null, barcodeString = null;
//            for (int i = 0; i < header.Lines.Count; i++)
//            {
//                if (i % QRBlockSize == 0)
//                {
//                    if (i > 0)
//                    {                        
//                        internalString.Append(string.Format(QrCode, barcodeString.ToString()));
//                        finalString.Append(internalString.ToString());
//                    }
//                    internalString = new MyStringBuilder(2 * QRBlockSize * QrCode.Length);
//                    barcodeString = new MyStringBuilder(2 * QRBlockSize * QrCode.Length);
//                    blockCounter++;
//                    TextElementType.Text = string.Format(this.Format, Math.Min(settings.QRBlockSize, documentheader.ActiveLines.Count - i), blockCounter, maxBlockCounter);
//                    internalString.Append(TextElementType.GetFormat(documentheader, settings));
//                    barcodeString.Append("QR");
//                    barcodeString.Append("C" + documentheader.Header.CustomerCode+"C");                    
//                }
//                barcodeString.Append(Separator);           
//                barcodeString.Append(documentheader.ActiveLines[i].ScannedCode);     
//                barcodeString.Append(QtySeparator);
//                barcodeString.Append(documentheader.ActiveLines[i].Qty1.ToString());             
//            }
//            internalString.Append(string.Format(settings.QrCode, barcodeString.ToString()));
//            finalString.Append(internalString.ToString());
//            return finalString.ToString();
//        }
        
//    }
//}
