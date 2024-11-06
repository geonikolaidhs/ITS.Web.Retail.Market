using System;
using System.IO;
using System.Resources;

namespace ITS.POS.Tools.FormDesigner.Loader
{
    public class ResourcesDesigner : System.ComponentModel.Design.IResourceService, IDisposable
    {
        private IServiceProvider host;

        private IResourceReader reader = null;
        private IResourceWriter writer = null;
        private FileStream readerStream = null;
        private FileStream writerStream = null;
        

        public string Filename { get; set; }

        public void Dispose()
        {
            Dispose(true);
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }
            if (writer != null)
            {
                writer.Dispose();
                writer = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
        ~ResourcesDesigner()
        {
            Dispose(false);
        }
        public ResourcesDesigner(IServiceProvider host)
        {
            this.host = host;
        }

        public System.Resources.IResourceReader GetResourceReader(System.Globalization.CultureInfo info)
        {
            if (String.IsNullOrEmpty(Filename))
            {
                return null;
            }
            if (readerStream != null)
            {
                readerStream.Close();
            }
            readerStream = new FileStream(Filename, FileMode.Open, FileAccess.Read);
            reader = new ResourceReader(readerStream);
            

            return reader;
        }

        public System.Resources.IResourceWriter GetResourceWriter(System.Globalization.CultureInfo info)
        {
            if (String.IsNullOrEmpty(Filename))
            {
                return null;
            }
            if (writerStream != null)
            {
                writerStream.Close();
            }

            writerStream = new FileStream(Filename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            writer = new ResourceWriter(writerStream);
            
            
            return writer;
        }
    }
}
