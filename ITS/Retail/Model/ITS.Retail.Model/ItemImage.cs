//-----------------------------------------------------------------------
// <copyright file="ItemStore.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using DevExpress.Xpo;
using System.Drawing;
using DevExpress.Data.Filtering;
using ITS.Retail.Platform.Enumerations;

namespace ITS.Retail.Model
{
    [Updater(Order = 470,
        Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]// | eUpdateDirection.STORECONTROLLER_TO_POS)]
    public class ItemImage: LookupField
    {
        public ItemImage()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public ItemImage(Session session)
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

		public static new CriteriaOperator GetUpdaterCriteria(eUpdateDirection direction, CompanyNew owner, Store store, string deviceID)
		{
			CriteriaOperator crop = null;
			switch (direction)
			{
				case eUpdateDirection.MASTER_TO_STORECONTROLLER:
					if (owner == null)
					{
						throw new Exception("ItemImage.GetUpdaterCriteria(); Error: Owner is null");
					}
					crop = CriteriaOperator.Parse("[<Item>][Oid = ^.ItemOid AND Owner.Oid = '"+owner.Oid+"']"); //inner join 
					break;
			}

			return crop;
		}

        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter))]
        public Image Image {
            get {
                return _Image;
            }
            set {
                SetPropertyValue("Image", ref _Image, value);
            }
        }
        public Guid ItemOid {
            get {
                return _ItemOid;
            }
            set {
                SetPropertyValue("ItemOid", ref _ItemOid, value);
            }
        }
        [Size(SizeAttribute.Unlimited)]
        public string Info {
            get {
                return _Info;
            }
            set {
                SetPropertyValue("Info", ref _Info, value);
            }
        }


        public int ImageWidth {
            get {
                return _ImageWidth;
            }
            set {
                SetPropertyValue("ImageWidth", ref _ImageWidth, value);
            }
        }


        public int ImageHeight {
            get {
                return _ImageHeight;
            }
            set {
                SetPropertyValue("ImageHeight", ref _ImageHeight, value);
            }
        }
        public bool IsValidImageSize() {
            if (Image == null) return true;
            return (Image.Size.Width <= ImageWidth) && (Image.Size.Height <= ImageHeight) ? true : false;
        }

        private int _ImageWidth;
        private int _ImageHeight;
        private string _Info;
        private Guid _ItemOid;
        private Image _Image;
    }

}