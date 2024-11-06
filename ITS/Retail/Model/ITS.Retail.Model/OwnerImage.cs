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
     [Updater(Order = 45,
       Permissions = eUpdateDirection.MASTER_TO_STORECONTROLLER)]
    public class OwnerImage : LookupField
    {
         public OwnerImage()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        public OwnerImage(Session session)
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

        [ValueConverter(typeof(DevExpress.Xpo.Metadata.ImageValueConverter))]
        public Image Image {
            get {
                return _Image;
            }
            set {
                SetPropertyValue("Image", ref _Image, value);
            }
        }

        public Guid OwnerApplicationSettingsOid
        {
            get {
                return _OwnerApplicationSettingsOid;
            }
            set {
                SetPropertyValue("OwnerApplicationSettingsOid", ref _OwnerApplicationSettingsOid, value);
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
        private Guid _OwnerApplicationSettingsOid;
        private Image _Image;
    }
}
