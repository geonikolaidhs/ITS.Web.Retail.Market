using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware
{
    public enum BarCodeSymbology
    {
        /// <summary>
        /// The service object cannot determine the barcode symbology
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// UPC-A
        /// </summary>
        Upca = 101,
        /// <summary>
        /// UPC-E
        /// </summary>
        Upce = 102,
        /// <summary>
        /// EAN/JAN-8
        /// </summary>
        EanJan8 = 103,
        /// <summary>
        /// EAN/JAN-13
        /// </summary>
        EanJan13 = 104,
        /// <summary>
        /// Standard (or discrete) 2 of 5
        /// </summary>
        TF = 105,
        /// <summary>
        /// Interleaved 2 of 5
        /// </summary>
        Itf = 106,
        /// <summary>
        /// CodaBar
        /// </summary>
        Codabar = 107,  
        /// <summary>
        /// Code 39
        /// </summary>
        Code39 = 108, 
        /// <summary>
        /// Code 93
        /// </summary>
        Code93 = 109,
        /// <summary>
        /// Code 128
        /// </summary>
        Code128 = 110,   
        /// <summary>
        /// UPC-A with supplemental barcode
        /// </summary>
        Upcas = 111,
        /// <summary>
        /// UPC-E with supplemental barcode
        /// </summary>
        Upces = 112,  
        /// <summary>
        /// UPC 1-digit supplement
        /// </summary>
        Upcd1 = 113,  
        /// <summary>
        /// UPC 2-digit supplement
        /// </summary>
        Upcd2 = 114,
        /// <summary>
        /// UPC 3-digit supplement
        /// </summary>
        Upcd3 = 115,   
        /// <summary>
        /// UPC 4-digit supplement
        /// </summary>
        Upcd4 = 116,
        /// <summary>
        /// UPC 5-digit supplement
        /// </summary>
        Upcd5 = 117, 
        /// <summary>
        /// EAN-8
        /// </summary>
        Ean8S = 118, 
        /// <summary>
        /// EAN-13
        /// </summary>
        Ean13S = 119,  
        /// <summary>
        /// EAN-128
        /// </summary>
        Ean128 = 120,
        /// <summary>
        /// OCRa
        /// </summary>
        Ocra = 121,   
        /// <summary>
        /// OCRb
        /// </summary>
        Ocrb = 122, 
        /// <summary>
        /// Code 128 with parsing
        /// </summary>
        Code128Parsed = 123,    
        /// <summary>
        /// Reduced space symbology
        /// </summary>
        [Obsolete("Replaced by Gs1DataBar")]
        Rss14 = 131,
        Gs1DataBar = 131,
        Gs1DataBarExpanded = 132,  
        /// <summary>
        /// Reduced space symbology - expanded
        /// </summary>
        [Obsolete("Replaced by Gs1DataBarExpanded")]
        RssExpanded = 132,
        Gs1DataBarStackedOmnidirectional = 133,
        Gs1DataBarExpandedStacked = 134,  
        /// <summary>
        /// Composite Component A--up to 56 characters of data
        /// </summary>
        Cca = 151,  
        /// <summary>
        /// Composite Component B--up to 338 characters of data
        /// </summary>
        Ccb = 152,   
        /// <summary>
        /// Composite Conponent C--up to 2361 characters of data
        /// </summary>
        Ccc = 153,  
        /// <summary>
        /// PDF417
        /// </summary>
        Pdf417 = 201,
        /// <summary>
        /// Maxicode
        /// </summary>
        Maxicode = 202,  
        /// <summary>
        /// DataMatrix
        /// </summary>
        DataMatrix = 203,    
        /// <summary>
        /// QRCode
        /// </summary>
        QRCode = 204,  
        /// <summary>
        /// MicroQRCode
        /// </summary>
        MicroQRCode = 205,   
        /// <summary>
        /// Aztec
        /// </summary>
        Aztec = 206,  
        /// <summary>
        /// MicroPDF417
        /// </summary>
        MicroPDF417 = 207,   
        /// <summary>
        /// If greater than or equal to this type, the service object has returned an
        /// undefined symbology
        /// </summary>
        Other = 501,
    }
}
