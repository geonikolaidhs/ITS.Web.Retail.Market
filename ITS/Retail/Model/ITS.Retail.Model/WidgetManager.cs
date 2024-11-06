using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;

namespace ITS.Retail.Model
{
    public class WidgetManager : BaseObj
    {
        public WidgetManager()
            : base() {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public WidgetManager(Session session)
            : base(session) {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place here your initialization code.
        }

        private string _DockPanel;
        /// <summary>
        /// Gets or Sets the value id of Dock Panel.
        /// </summary>
        [Indexed("User;GCRecord", Unique = true)]
        public string DockPanel
        {
            get
            {
                return _DockPanel;
            }
            set
            {
                SetPropertyValue("DockPanel", ref _DockPanel, value);
            }
        }

         private bool _IsVisible;
        /// <summary>
         /// Gets or Sets the value of Dock Panel visible or not.
        /// </summary>
        [Indexed("GCRecord", Unique = false)]
         public bool IsVisible
        {
            get
            {
                return _IsVisible;
            }
            set
            {
                SetPropertyValue("IsVisible", ref _IsVisible, value);
            }
        }

        private int _DockZone;
        /// <summary>
        /// 
        /// </summary>
        [Indexed("GCRecord",Unique = false)]
        public int DockZone
        {
            get
            {
                return _DockZone;
            }
            set
            {
                SetPropertyValue("DockZone", ref _DockZone, value);
            }
        }

        private User _User;
        [Association("User-WidgetManagers"), Indexed("GCRecord", Unique = false)]
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
    }
}