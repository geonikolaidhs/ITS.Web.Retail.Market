using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using ITS.POS.Tools.FormDesigner.Loader;


namespace ITS.POS.Tools.FormDesigner.Host
{
    /// <summary>
    /// Inherits from DesignSurface and hosts the RootComponent and 
    /// all other designers. It also uses loaders (BasicDesignerLoader
    /// or CodeDomDesignerLoader) when required. It also provides various
    /// services to the designers. Adds MenuCommandService which is used
    /// for Cut, Copy, Paste, etc.
    /// </summary>
    public class HostSurface : DesignSurface
    {
        private ISelectionService _selectionService;
        public string FileName { get; set; }

        public HostSurface()
            : base()
        {
            AddService(typeof(IMenuCommandService), new MenuCommandService(this));
            AddService(typeof(IResourceService), new ResourcesDesigner(this));
            AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService(this));

            
        }
        public HostSurface(IServiceProvider parentProvider)
            : base(parentProvider)
        {
            AddService(typeof(IMenuCommandService), new MenuCommandService(this));
            AddService(typeof(IResourceService), new ResourcesDesigner(this));
            AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService(this));
        }

        public void SetResourcesFile(string file)
        {
            ResourcesDesigner rs = GetService(typeof(IResourceService)) as ResourcesDesigner;
            rs.Filename = file;
        }

        internal void Initialize()
        {
            Control control = null;
            IDesignerHost host = (IDesignerHost)GetService(typeof(IDesignerHost));

            if (host == null)
            {
                return;
            }
            try
            {
                if (host.RootComponent == null)
                {
                }
                Type hostType = host.RootComponent.GetType();
                if (hostType == typeof(Form) || hostType.IsSubclassOf(typeof(Form)))
                {
                    control = View as Control;
                    control.BackColor = Color.White;
                }
                else
                {
                    if (hostType == typeof(UserControl))
                    {
                        control = View as Control;
                        control.BackColor = Color.White;
                    }
                    else
                    {
                        if (hostType == typeof(Component))
                        {
                            control = View as Control;
                            control.BackColor = Color.FloralWhite;
                        }
                        else
                        {
                            throw new Exception("Undefined Host Type: " + hostType.ToString());
                        }
                    }
                }
                _selectionService = (ISelectionService)(ServiceContainer.GetService(typeof(ISelectionService)));
                _selectionService.SelectionChanged += new EventHandler(selectionService_SelectionChanged);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        public BasicDesignerLoader Loader { get; set; }

        /// <summary>
        /// When the selection changes this sets the PropertyGrid's selected component 
        /// </summary>
        private void selectionService_SelectionChanged(object sender, EventArgs e)
        {
            if (_selectionService != null)
            {
                ICollection selectedComponents = _selectionService.GetSelectedComponents();
                PropertyGrid propertyGrid = (PropertyGrid)GetService(typeof(PropertyGrid));


                object[] comps = new object[selectedComponents.Count];
                int i = 0;

                foreach (Object o in selectedComponents)
                {
                    comps[i] = o;
                    i++;
                }

                propertyGrid.SelectedObjects = comps;
            }
        }

        public void AddService(Type type, object serviceInstance)
        {
            ServiceContainer.AddService(type, serviceInstance);
        }
    }
}
