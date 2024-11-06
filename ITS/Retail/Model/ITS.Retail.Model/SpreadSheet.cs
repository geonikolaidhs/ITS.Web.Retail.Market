using ITS.Retail.Platform.Enumerations;
using System;
using System.ComponentModel.DataAnnotations;

//-----------------------------------------------------------------------
// <copyright file="SpreadSheet.cs" company="ITS">
//     Copyright (c) ITS SA.  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ITS.Retail.Model
{
    /// <summary>
    ///
    /// </summary>
    [EntityDisplayName("SpreadSheet", typeof(ResourcesLib.Resources))]
    public class SpreadSheet : BaseObj
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadSheet" /> class.
        /// </summary>
        public SpreadSheet()
            : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpreadSheet"/> class.
        /// </summary>
        /// <param name="session">The session.</param>
        public SpreadSheet(DevExpress.Xpo.Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        /// <summary>
        /// After construction.
        /// </summary>
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        /// <summary>
        /// The _ user
        /// </summary>
        private User _User;

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        /// <value>
        /// The user.
        /// </value>
        [DevExpress.Xpo.Association("User-SpreadSheet"), DevExpress.Xpo.Indexed("GCRecord", Unique = false)]
        public User User
        {
            get
            {
                return _User;
            }
            set
            {
                SetPropertyValue("User", ref _User, value);
            }
        }

        /// <summary>
        /// The _ code
        /// </summary>
        private string _Code;

        /// <summary>
        /// Sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        [DisplayOrder(Order = 1), Required]
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                SetPropertyValue("Code", ref _Code, value);
            }
        }

        /// <summary>
        /// The _ title
        /// </summary>
        private string _Title;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        [DisplayOrder(Order = 3), Required]
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                SetPropertyValue("Title", ref _Title, value);
            }
        }

        /// <summary>
        /// The _ binary file
        /// </summary>
        private byte[] _BinaryFile;

        /// <summary>
        /// Gets or sets the binary file.
        /// </summary>
        /// <value>
        /// The binary file.
        /// </value>
        [DevExpress.Xpo.Size(DevExpress.Xpo.SizeAttribute.Unlimited)]
        public Byte[] BinaryFile
        {
            get
            {
                return _BinaryFile;
            }
            set
            {
                SetPropertyValue("BinaryFile", ref _BinaryFile, value);
            }
        }

        /// <summary>
        /// The _ file name
        /// </summary>
        private string _FileName;

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        [DisplayOrder(Order = 4), Required]
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                SetPropertyValue("FileName", ref _FileName, value);
            }
        }
    }
}