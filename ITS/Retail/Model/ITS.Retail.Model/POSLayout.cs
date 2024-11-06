using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using System.IO;
using System.Drawing;

namespace ITS.Retail.Model
{
    public class POSLayout : Lookup2Fields
    {
        public POSLayout()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public POSLayout(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
            IsActive = true;
        }

        // Fields...
        private int _MainLayoutFileSize;
        private Byte[] _MainLayout;
        private Image _MainLayoutImage;
		private string _MainLayoutFileName;
        private byte[] _SecondaryLayout;
        private int _SecondaryLayoutFileSize;
        private string _SecondaryLayoutFileName;
        private Image _SecondaryLayoutImage;

        [Size(SizeAttribute.Unlimited)]
        public Byte[] MainLayout
        {
            get
            {
                return _MainLayout;
            }
            set
            {
                SetPropertyValue("MainLayout", ref _MainLayout, value);
            }
        }

        
        public int MainLayoutFileSize
        {
        	get
        	{
        		return _MainLayoutFileSize;
        	}
        	set
        	{
                SetPropertyValue("MainLayoutFileSize", ref _MainLayoutFileSize, value);
        	}
        }

		public string MainLayoutFileName
		{
			get
			{
				return _MainLayoutFileName;
			}
			set
			{
                SetPropertyValue("MainLayoutFileName", ref _MainLayoutFileName, value);
			}
		}


        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter))]
        public Image MainLayoutImage
        {
            get
            {
                return _MainLayoutImage;
            }
            set
            {
                SetPropertyValue("MainLayoutImage", ref _MainLayoutImage, value);
            }
        }

        [Size(SizeAttribute.Unlimited)]
        public Byte[] SecondaryLayout
        {
            get
            {
                return _SecondaryLayout;
            }
            set
            {
                SetPropertyValue("SecondaryLayout", ref _SecondaryLayout, value);
            }
        }


        public int SecondaryLayoutFileSize
        {
            get
            {
                return _SecondaryLayoutFileSize;
            }
            set
            {
                SetPropertyValue("SecondaryLayoutFileSize", ref _SecondaryLayoutFileSize, value);
            }
        }

        public string SecondaryLayoutFileName
        {
            get
            {
                return _SecondaryLayoutFileName;
            }
            set
            {
                SetPropertyValue("SecondaryLayoutFileName", ref _SecondaryLayoutFileName, value);
            }
        }


        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter))]
        public Image SecondaryLayoutImage
        {
            get
            {
                return _SecondaryLayoutImage;
            }
            set
            {
                SetPropertyValue("SecondaryLayoutImage", ref _SecondaryLayoutImage, value);
            }
        }

		[Association("POS-POSLayouts")]
		public XPCollection<POS> POSs
		{
			get
			{
				return GetCollection<POS>("POSs");
			}
		}
    }
}
