using ITS.POS.Tools.FormDesigner.Loader;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;



namespace ITS.POS.Tools.FormDesigner.Host
{
    /// <summary>
    /// Hosts the HostSurface which inherits from DesignSurface.
    /// </summary>
    public class HostControl : System.Windows.Forms.UserControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private HostSurface _hostSurface;
        private ResourcesDesigner _resourcesDesigner;
        public string FileName { get; set; }
        public string TempCCUObjectStateFileName { get; set; }
        public string TempResourcesFileName { get; set; }

        public HostControl(HostSurface hostSurface, string fileName = null)
        {
            FileName = fileName;
            InitializeComponent(hostSurface);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent(HostSurface hostSurface)
        {
            this.Name = "HostControl";
            this.Size = new System.Drawing.Size(268, 224);
            try
            {
                if (hostSurface == null)
                {
                    return;
                }
                _hostSurface = hostSurface;
                IResourceService rs = (IResourceService)_hostSurface.GetService(typeof(IResourceService));

                if (rs == null)
                {
                    _hostSurface.AddService(typeof(IResourceService), new ResourcesDesigner(_hostSurface));
                    rs = (ResourcesDesigner)_hostSurface.GetService(typeof(IResourceService));
                }

                _resourcesDesigner = rs as ResourcesDesigner;
               // string mainClass = "ITS.POS.Client.frmMainCustom";// + (this._hostSurface.Loader as CodeDomHostLoader).GetLoaderHost().RootComponent.Site.Name;
                string resourceFile = this.TempResourcesFileName;
                _resourcesDesigner.Filename = resourceFile;

                Control control = _hostSurface.View as Control;

                control.Parent = this;
                control.Dock = DockStyle.Fill;
                control.Visible = true;
                
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }
        public HostSurface HostSurface
        {
            get
            {
                return _hostSurface;
            }
        }
        public IDesignerHost DesignerHost
        {
            get
            {
                return (IDesignerHost)_hostSurface.GetService(typeof(IDesignerHost));
            }
        }
    }
}
