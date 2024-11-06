using ITS.Retail.Platform.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;

namespace ITS.Retail.WebClient
{
    //WebserviceExceptionHandler
    public class WebserviceExceptionHandler : SoapExtension
    {
        //private static readonly ILog logger = LogManager.GetLogger("My Service");

        public WebserviceExceptionHandler()
            : base()
        {
        }

        Stream oldStream;
        Stream newStream;

        public override Stream ChainStream(Stream stream)
        {
            oldStream = stream;
            newStream = new MemoryStream();
            return newStream;
        }

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            return methodInfo.Name;
        }

        public override object GetInitializer(Type WebServiceType)
        {
            return WebServiceType.Name;
        }

        public override void Initialize(object initializer)
        {
        }

        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.BeforeSerialize:
                    break;
                case SoapMessageStage.AfterSerialize:
                    WriteOutput(message);
                    break;
                case SoapMessageStage.BeforeDeserialize:
                    WriteInput(message);
                    break;
                case SoapMessageStage.AfterDeserialize:
                    break;
            }
        }

        public void WriteOutput(SoapMessage message)
        {
            try
            {
                if (message != null && message.MethodInfo != null)
                {
                    string soapString = (message is SoapServerMessage) ? "SoapResponse" : "SoapRequest";
                    string header = soapString + ": " + message.MethodInfo.Name + "\n";
                    newStream.Position = 0;
                    String streamMessage = GetString(newStream);
                    if (message.Exception != null)
                    {
                        MvcApplication.WRMLogModule.Log(message.Exception, header + Environment.NewLine + "Message:" + streamMessage, KernelLogLevel.Info);
                        return;
                    }
                    
                    MvcApplication.WRMLogModule.Log(header + Environment.NewLine + "Message:" + streamMessage, KernelLogLevel.Debug);

                    Copy(newStream, oldStream);
                }
            }
            catch (Exception ex)
            {
                MvcApplication.WRMLogModule.Log(ex, "Error Writing Output",KernelLogLevel.Info);
            }

        }

        public void WriteInput(SoapMessage message)
        {
            try
            {
                Copy(oldStream, newStream);
                string soapString = (message is SoapServerMessage) ? "SoapRequest" : "SoapResponse";
                var header = soapString + ": MethodNotAvailable\n";

                string getString = GetString(newStream);

                MvcApplication.WRMLogModule.Log(header + Environment.NewLine + "Message:" + getString,KernelLogLevel.Debug);

            }
            catch (Exception ex)
            {
                MvcApplication.WRMLogModule.Log(ex, "Error Writing Input",KernelLogLevel.Info);
            }
        }

        string GetString(Stream stream)
        {
            stream.Position = 0;
            var sb = new StringBuilder();
            var w = new StringWriter(sb);

            stream.Position = 0;
            Copy(stream, w);
            stream.Position = 0;
            return sb.ToString();

        }

        void Copy(Stream from, TextWriter to)
        {
            var reader = new StreamReader(from);
            to.WriteLine(reader.ReadToEnd());
            to.Flush();
        }

        void Copy(Stream from, Stream to)
        {
            TextReader reader = new StreamReader(from);
            TextWriter writer = new StreamWriter(to);
            writer.WriteLine(reader.ReadToEnd());
            writer.Flush();
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class LogExtensionAttribute : SoapExtensionAttribute
    {
        public override Type ExtensionType
        {
            get { return typeof(WebserviceExceptionHandler); }
        }

        public override int Priority { get; set; }
    }
}
