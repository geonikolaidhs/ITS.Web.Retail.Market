//-----------------------------------------------------------------------
// <copyright file="ApplicationSettings.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Drawing;
using DevExpress.Xpo;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{

    public class ApplicationSettings : BaseObj
    {
        public ApplicationSettings()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public ApplicationSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        //private int _NewItemCurrentDateIndex;
        private int _MainImageWidth;
        private int _MainImageHeight;
        private LogLevel _LogingLevel;

        private int _BigImageSizeMax;
        private int _SmallImageSizeMax;
        private int _BigImageHeight;
        private int _BigImageWidth;
        private int _SmallImageHeight;
        private int _SmallImageWidth;
        /// <summary>
        /// Private Attributes 
        /// </summary>


        private Image _MenuLogo;
        private Image _MainScreenImage;
        
        public int SmallImageSizeMax
        {
            get
            {
                return _SmallImageSizeMax;
            }
            set
            {
                SetPropertyValue("SmallImageSizeMax", ref _SmallImageSizeMax, value);
            }
        }
        public int BigImageSizeMax
        {
            get
            {
                return _BigImageSizeMax;
            }
            set
            {
                SetPropertyValue("BigImageSizeMax", ref _BigImageSizeMax, value);
            }
        }

        public int SmallImageWidth
        {
            get
            {
                return _SmallImageWidth;
            }
            set
            {
                SetPropertyValue("SmallImageWidth", ref _SmallImageWidth, value);
            }
        }
        public int SmallImageHeight
        {
            get
            {
                return _SmallImageHeight;
            }
            set
            {
                SetPropertyValue("SmallImageHeight", ref _SmallImageHeight, value);
            }
        }
        public int BigImageWidth
        {
            get
            {
                return _BigImageWidth;
            }
            set
            {
                SetPropertyValue("BigImageWidth", ref _BigImageWidth, value);
            }
        }
        public int BigImageHeight
        {
            get
            {
                return _BigImageHeight;
            }
            set
            {
                SetPropertyValue("BigImageHeight", ref _BigImageHeight, value);
            }
        }
        
        
        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter))]
        public Image MenuLogo
        {
            get
            {
                return _MenuLogo;
            }
            set
            {
                SetPropertyValue<Image>("MenuLogo", ref _MenuLogo, value);
            }
        }
        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter))]
        public Image MainScreenImage
        {
            get
            {
                return _MainScreenImage;
            }
            set
            {
                SetPropertyValue<Image>("MainScreenImage", ref _MainScreenImage, value);
            }
        }

        //

        


        public LogLevel LogingLevel
        {
            get
            {
                return _LogingLevel;
            }
            set
            {
                SetPropertyValue("LogingLevel", ref _LogingLevel, value);
            }
        }


        public int MainImageHeight
        {
            get
            {
                return _MainImageHeight;
            }
            set
            {
                SetPropertyValue("MainImageHeight", ref _MainImageHeight, value);
            }
        }


        public int MainImageWidth
        {
            get
            {
                return _MainImageWidth;
            }
            set
            {
                SetPropertyValue("MainImageWidth", ref _MainImageWidth, value);
            }
        }

        




        //public override void GetData(Session myses, object item)
        //{
        //    base.GetData(myses, item);
        //    ApplicationSettings apset = item as ApplicationSettings;            
        //    MainScreenImage = apset.MainScreenImage;
        //    MenuLogo = apset.MenuLogo;
        //    MainImageHeight = apset.MainImageHeight;
        //    MainImageWidth = apset.MainImageWidth;
        //}

    }

}