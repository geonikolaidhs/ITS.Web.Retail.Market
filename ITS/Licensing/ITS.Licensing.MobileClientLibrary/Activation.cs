using System;

using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ITS.Licensing.MobileClientLibrary
{
    public partial class Activation : Form
    {
        public Activation()
        {
            InitializeComponent();
        }
        ITS.Licensing.MobileClientLibrary.MobileClientLibrary.LicenseStatus status;

        public ITS.Licensing.MobileClientLibrary.MobileClientLibrary.LicenseStatus Status
        {
            get
            {
                return status;
            }
        }
        
    }
}