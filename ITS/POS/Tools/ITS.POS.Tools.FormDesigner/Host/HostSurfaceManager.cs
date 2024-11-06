using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.IO;
using System.Windows.Forms;
using Ionic.Zip;
using ITS.POS.Tools.FormDesigner.Loader;


namespace ITS.POS.Tools.FormDesigner.Host
{
    public enum LoaderType
    {
        CodeDomDesignerLoader = 2,
    }

    /// <summary>
    /// Manages numerous HostSurfaces. Any services added to HostSurfaceManager
    /// will be accessible to all HostSurfaces
    /// </summary>
    public class HostSurfaceManager : DesignSurfaceManager
    {
        public PropertyGridHost PropertyGridHost { get; private set; }

        public HostSurfaceManager()
            : base()
        {
            this.PropertyGridHost = new PropertyGridHost(this);
            AddService(typeof(INameCreationService), new NameCreationService());
            ActiveDesignSurfaceChanged += new ActiveDesignSurfaceChangedEventHandler(HostSurfaceManager_ActiveDesignSurfaceChanged);
            
        }

        protected override DesignSurface CreateDesignSurfaceCore(IServiceProvider parentProvider)
        {
            return new HostSurface(parentProvider);
        }

        /// <summary>
        /// Gets a new HostSurface and loads it with the appropriate type of
        /// root component. 
        /// </summary>
        //private HostControl GetNewHost(Type rootComponentType)
        //{
        //    HostSurface hostSurface = (HostSurface)CreateDesignSurface(ServiceContainer);

        //    if (rootComponentType == typeof(Form) || rootComponentType.IsSubclassOf(typeof(Form)))
        //    {
        //        hostSurface.BeginLoad(rootComponentType);
        //    }
        //    else
        //    {
        //        throw new Exception("Undefined Host Type: " + rootComponentType.ToString());

        //    }
        //    hostSurface.Initialize();
        //    ActiveDesignSurface = hostSurface;
        //    return new HostControl(hostSurface, null);
        //}

        /// <summary>
        /// Gets a new HostSurface and loads it with the appropriate type of
        /// root component. Uses the appropriate Loader to load the HostSurface.
        /// </summary>
        public HostControl GetNewHost(Type rootComponentType, LoaderType loaderType)
        {
            HostSurface hostSurface = (HostSurface)CreateDesignSurface(ServiceContainer);

            switch (loaderType)
            {
                case LoaderType.CodeDomDesignerLoader :
                    CodeDomHostLoader codeDomHostLoader = new CodeDomHostLoader(rootComponentType);
                    hostSurface.BeginLoad(codeDomHostLoader);
                    hostSurface.Loader = codeDomHostLoader;
                    break;

                default:
                    throw new Exception("Loader is not defined: " + loaderType.ToString());
            }

            hostSurface.Initialize();
            return new HostControl(hostSurface);
        }

        /// <summary>
        /// Opens an Xml file and loads it up using BasicHostLoader (inherits from
        /// BasicDesignerLoader)
        /// </summary>
        public HostControl GetNewHost(string fileName, ref string tempCCUObjectStateFileName,ref string tempResourceFileName)
        {
            if (fileName == null || !File.Exists(fileName))
            {
                MessageBox.Show("FileName is incorrect: " + fileName);
            }
            if (fileName.EndsWith("itsform") || fileName.EndsWith("itssform"))
            {
                ReadOptions opts = new ReadOptions();
                using (Ionic.Zip.ZipFile zip = ZipFile.Read(fileName, new ReadOptions()))
                {
                    foreach (ZipEntry entry in zip.Entries)
                    {
                        entry.Extract(Path.GetTempPath(),ExtractExistingFileAction.OverwriteSilently);
                        if (Path.GetExtension(Path.GetTempPath() + entry.FileName) == ".xml")
                        {
                            string newFileName = Path.ChangeExtension(Path.GetTempFileName(),".xml");

                            File.Move(Path.GetTempPath() + entry.FileName, newFileName);

                            tempCCUObjectStateFileName = newFileName;

                        }
                        else if (Path.GetExtension(Path.GetTempPath() + entry.FileName) == ".resources")
                        {
                            string newFileName = Path.ChangeExtension(Path.GetTempFileName(), ".resources");

                            File.Move(Path.GetTempPath() + entry.FileName, newFileName);

                            tempResourceFileName = newFileName;
                        }
                    }
                }
                

                HostSurface hostSurface = (HostSurface)CreateDesignSurface(ServiceContainer);

                CodeDomHostLoader codeDomHostLoader = new CodeDomHostLoader(fileName);
                codeDomHostLoader.TempCCUObjectStateFileName = tempCCUObjectStateFileName;
                codeDomHostLoader.TempResourcesFileName = tempResourceFileName;

                //string mainClass = "ITS.POS.Client.frmMainCustom";
                string resourceFile = tempResourceFileName;

                hostSurface.SetResourcesFile(resourceFile);
                hostSurface.BeginLoad(codeDomHostLoader);
                hostSurface.Loader = codeDomHostLoader;
                hostSurface.Initialize();

                return new HostControl(hostSurface, fileName) { TempCCUObjectStateFileName = tempCCUObjectStateFileName, TempResourcesFileName = tempResourceFileName };
            }
            throw new Exception("File cannot be opened. Please check the type or extension of the file.");
        }

        public void AddService(Type type, object serviceInstance)
        {
            ServiceContainer.AddService(type, serviceInstance);
        }

        /// <summary>
        /// Uses the OutputWindow service and writes out to it.
        /// </summary>
        private void HostSurfaceManager_ActiveDesignSurfaceChanged(object sender, ActiveDesignSurfaceChangedEventArgs e)
        {
            Designer.ToolWindows.OutputWindow o = GetService(typeof(Designer.ToolWindows.OutputWindow)) as Designer.ToolWindows.OutputWindow;
            o.RichTextBox.Text += "New host added.\n";
        }
    }
}
