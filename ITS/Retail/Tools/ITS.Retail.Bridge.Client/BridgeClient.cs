using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ITS.Common.Logging;
using System.Xml;
using System.Reflection;
using System.Xml.Linq;
using Retail;
using System.Drawing;
using System.Threading;
using ITS.Retail.Bridge.Client;
using System.Xml.Xsl;
using System.Globalization;
namespace ITS.Retail.Bridge.Service
{
	public class BridgeClient
	{

		String[] protectionPaths;
		String webserviceUrl;
		String EmailTo ="";
		Guid deviceId;
		Encoding encoding;
		bool deleteFileAfterUpload;
		bool createUsersForCustomers;
		bool createUsersForStores;
		string customerUserRoleDescription;
		string storeUserRoleDescription;
		List<FileSystemWatcher> watchers;
		static bool Processing;
		private static Timer runner;
		private static TimerCallback runnerCallback;
		public string SMTPHost = "";
        public bool SendEmailsForImagesOnlyOnError = true;
		public string Port = "";
		public string EmailFrom = "";
		public string EmailUsername = "";
		public string EmailPassword = "";
        public string OwnerTaxCode = "";
		public string Domain = "";
        public string ValidImageFiles = "jpg,jpeg,jpe,png,bmp";
        public string ValidTextFiles = "txt,csv";
        public bool MultipleItemCategoryTrees;
        public string RootItemCategoryCode = "";
        public bool MakeZeroPricesUserDefinable = false;
        public bool OneStorePerCustomer = false;
		public bool EnableSSL;
        CultureInfo cultureInfo;


		bool display;

		bool everythingOK;
		public bool EverythingOK
		{
			get
			{
				return everythingOK;
			}
		}


		private String decodeXml(string encodedString)
		{

			return encodedString;
		}

		public enum Mode
		{
			FILEWATCHER,
			EXPORT
		}

		/// <summary>
		/// Console Application Main function
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <param name="exportDirectory"></param>
		/// <param name="display"></param>
		public BridgeClient(Mode mode, string username = "", string password = "", string storeCode = "", string exportDirectory = "", bool display = false)
		{
			this.display = display;

			if (mode == Mode.FILEWATCHER)
			{
				InitializeFileWatcher();
			}
			else if (mode == Mode.EXPORT)
			{
				Export(username, password, storeCode, exportDirectory);
			}
		}

		private void InitializeFileWatcher()
		{
			Logger.SetLogFilePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\FileWatcherlog.txt");
			try
			{
				DualLogger.Info("Retail Bridge Service", "Initialization", "Start", null, display);
				String encodedFile, decodedFile;
				StreamReader sr = new StreamReader(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\settings.xml");
				encodedFile = sr.ReadToEnd();
				decodedFile = decodeXml(encodedFile);


				XDocument xml = XDocument.Parse(decodedFile);
				IEnumerable<XElement> allNodes = xml.Element("settings").Elements();


				IEnumerable<XElement> nodes = allNodes.Where(g => g.Name == "WebService");
				if (nodes.Count() != 1)
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "Url not found");
					everythingOK = false;
					return;
				}
				webserviceUrl = nodes.First().Value;

				nodes = allNodes.Where(g => g.Name == "Directory");
				if (nodes.Count() < 1)
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "No Direcotories found");
					everythingOK = false;
					return;
				}
				IEnumerable<String> pathEnum = nodes.Select(g => g.Value);
				protectionPaths = pathEnum.ToArray();

				nodes = allNodes.Where(g => g.Name == "EmailSettings").Elements().Where(g => g.Name == "To");
				if (nodes.Count() == 1)
				{
					EmailTo = nodes.First().Value;
				}
				nodes = allNodes.Where(g => g.Name == "EmailSettings").Elements().Where(g => g.Name == "SMTPHost");
				if (nodes.Count() == 1)
				{
					SMTPHost = nodes.First().Value;
				}
                nodes = allNodes.Where(g => g.Name == "EmailSettings").Elements().Where(g => g.Name == "SendEmailsForImagesOnlyOnError");
                if (nodes.Count() == 1)
                {
                    bool.TryParse(nodes.First().Value, out SendEmailsForImagesOnlyOnError);
                }
				nodes = allNodes.Where(g => g.Name == "EmailSettings").Elements().Where(g => g.Name == "Port");
				if (nodes.Count() == 1)
				{
					Port = nodes.First().Value;
				}
				nodes = allNodes.Where(g => g.Name == "EmailSettings").Elements().Where(g => g.Name == "From");
				if (nodes.Count() == 1)
				{
					EmailFrom = nodes.First().Value;
				}
				nodes = allNodes.Where(g => g.Name == "EmailSettings").Elements().Where(g => g.Name == "Username");
				if (nodes.Count() == 1)
				{
					EmailUsername = nodes.First().Value;
				}
				nodes = allNodes.Where(g => g.Name == "EmailSettings").Elements().Where(g => g.Name == "Password");
				if (nodes.Count() == 1)
				{
					EmailPassword = nodes.First().Value;
				}
				nodes = allNodes.Where(g => g.Name == "EmailSettings").Elements().Where(g => g.Name == "Domain");
				if (nodes.Count() == 1)
				{
					Domain = nodes.First().Value;
				}
				nodes = allNodes.Where(g => g.Name == "EmailSettings").Elements().Where(g => g.Name == "EnableSSL");
				if (nodes.Count() == 1)
				{
					try
					{
						EnableSSL = bool.Parse(nodes.First().Value);
					}
					catch (Exception e)
					{
                        string errorMessage = e.Message;
						EnableSSL = false;
					}
				}

				nodes = allNodes.Where(g => g.Name == "DeviceID");
				if (nodes.Count() != 1)
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "email not found");
					everythingOK = false;
					return;
				}
				deviceId = new Guid(nodes.First().Value);

				nodes = allNodes.Where(g => g.Name == "Encoding");
				if (nodes.Count() != 1)
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "Encoding not found");
					everythingOK = false;
					return;
				}

                int codepage = 0;
                if (int.TryParse(nodes.First().Value, out codepage))
                {
                    encoding = Encoding.GetEncoding(codepage);
                }
                else
                {
                    encoding = Encoding.GetEncoding(nodes.First().Value);
                }


                cultureInfo = Thread.CurrentThread.CurrentCulture;
                nodes = allNodes.Where(g => g.Name == "Locale");
                if (nodes.Count() != 1 || nodes.First().Value == null)
                {
                    DualLogger.Warn("Retail Bridge Service", "Initialization Warning", "Locale not found. Default locale will be used.");
                }
                else
                {
                    try
                    {
                        cultureInfo = CultureInfo.GetCultureInfo(nodes.First().Value);
                    }
                    catch(Exception e)
                    {
                        string errorMessage = e.Message;
                        DualLogger.Warn("Retail Bridge Service", "Initialization Warning", "Locale '"+nodes.First().Value+"' is invalid. Default locale will be used.");
                    }
                }


                nodes = allNodes.Where(g => g.Name == "OwnerTaxCode");
                if (nodes.Count() != 1)
                {
                    DualLogger.Error("Retail Bridge Service", "Initialization Failed", "OwnerTaxCode not found");
                    everythingOK = false;
                    return;
                }

                OwnerTaxCode = nodes.First().Value;

                nodes = allNodes.Where(g => g.Name == "ValidTextFiles");
                if (nodes.Count() != 1)
                {
                    DualLogger.Error("Retail Bridge Service", "Initialization Warning", "ValidTextFiles not found. Default types will be used: " + ValidTextFiles);
                }
                else
                {
                    ValidTextFiles = nodes.First().Value;
                }

                nodes = allNodes.Where(g => g.Name == "ValidImageFiles");
                if (nodes.Count() != 1)
                {
                    DualLogger.Warn("Retail Bridge Service", "Initialization Warning", "ValidImageFiles not found. Default types will be used: " + ValidImageFiles);
                }
                else
                {
                    ValidImageFiles = nodes.First().Value;
                }


                nodes = allNodes.Where(g => g.Name == "MultipleItemCategoryTrees");
                if (nodes.Count() != 1)
                {
                    DualLogger.Error("Retail Bridge Service", "Initialization Failed", "MultipleItemCategoryTrees not found");
                    everythingOK = false;
                    return;
                }

                if (!bool.TryParse(nodes.First().Value, out MultipleItemCategoryTrees))
                {
                    DualLogger.Error("Retail Bridge Service", "Initialization Failed", "MultipleItemCategoryTrees value '" + nodes.First().Value + "' was not valid. Valid values are: true, false");
                    everythingOK = false;
                    return;
                }


                nodes = allNodes.Where(g => g.Name == "MakeZeroPricesUserDefinable");
                if (nodes.Count() != 1)
                {
                    DualLogger.Error("Retail Bridge Service", "Initialization Failed", "MakeZeroPricesUserDefinable not found");
                    everythingOK = false;
                    return;
                }

                if (!bool.TryParse(nodes.First().Value, out MakeZeroPricesUserDefinable))
                {
                    DualLogger.Error("Retail Bridge Service", "Initialization Failed", "MakeZeroPricesUserDefinable value '" + nodes.First().Value + "' was not valid. Valid values are: true, false");
                    everythingOK = false;
                    return;
                }

                if (!bool.TryParse(nodes.First().Value, out OneStorePerCustomer))
                {
                    DualLogger.Error("Retail Bridge Service", "Initialization Failed", "OneStorePerCustomer value '" + nodes.First().Value + "' was not valid. Valid values are: true, false");
                    everythingOK = false;
                    return;
                }

                nodes = allNodes.Where(g => g.Name == "RootItemCategoryCode");
                if (nodes.Count() != 1)
                {
                    DualLogger.Error("Retail Bridge Service", "Initialization Failed", "RootItemCategoryCode not found");
                    everythingOK = false;
                    return;
                }

                RootItemCategoryCode = nodes.First().Value;

				nodes = allNodes.Where(g => g.Name == "DeleteFileAfterUpload");
				if (nodes.Count() != 1)
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "DeleteFileAfterUpload not found");
					everythingOK = false;
					return;
				}


				if (!bool.TryParse(nodes.First().Value, out deleteFileAfterUpload))
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "DeleteFileAfterUpload value '" + nodes.First().Value + "' was not valid. Valid values are: true, false");
					everythingOK = false;
					return;
				}

				nodes = allNodes.Where(g => g.Name == "CreateUsersForCustomers");
				if (nodes.Count() != 1)
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "CreateUsersForCustomers not found");
					everythingOK = false;
					return;
				}
				if (!bool.TryParse(nodes.First().Value, out createUsersForCustomers))
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "CreateUsersForCustomers value '" + nodes.First().Value + "' was not valid. Valid values are: true, false");
					everythingOK = false;
					return;
				}

				nodes = allNodes.Where(g => g.Name == "CreateUsersForStores");
				if (nodes.Count() != 1)
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "CreateUsersForCustomers not found");
					everythingOK = false;
					return;
				}
				if (!bool.TryParse(nodes.First().Value, out createUsersForStores))
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "CreateUsersForStores value '" + nodes.First().Value + "' was not valid. Valid values are: true, false");
					everythingOK = false;
					return;
				}

				nodes = allNodes.Where(g => g.Name == "CustomerUserRoleDescription");
				if (nodes.Count() != 1)
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "CustomerUserRoleDescription not found");
					everythingOK = false;
					return;
				}
				customerUserRoleDescription = nodes.First().Value;

				nodes = allNodes.Where(g => g.Name == "StoreUserRoleDescription");
				if (nodes.Count() != 1)
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "CustomerUserRoleDescription not found");
					everythingOK = false;
					return;
				}
				storeUserRoleDescription = nodes.First().Value;

				DualLogger.Info("Retail Bridge Service", "Initialization", "Complete Successfully", null, display);

				DualLogger.Info("Retail Bridge Service", "Setting Watchers", "Begin", null, display);

				watchers = new List<FileSystemWatcher>();

				foreach (string path in protectionPaths)
				{
					FileSystemWatcher fileWatcher = new FileSystemWatcher();
					fileWatcher.Path = path;
					fileWatcher.Filter = "*.*";
					fileWatcher.IncludeSubdirectories = false;
					fileWatcher.EnableRaisingEvents = true;
					fileWatcher.Created += FileCreated;
					watchers.Add(fileWatcher);


				}

				if (deleteFileAfterUpload)
				{
					runnerCallback = new TimerCallback(RegularFileCheckerThread);
					runner = new Timer(runnerCallback, null, 1000, 600000);
				}

			}
			catch (Exception ex)
			{
				Logger.Error("Retail Bridge Service", "Initialization Failed", ex.Message);
				everythingOK = false;
			}
		}

		private int CheckFileHasCopied(string FilePath)
		{
			try
			{
				if (File.Exists(FilePath))
					using (File.OpenRead(FilePath))
					{
						return 0;
					}
				else
					return 1;
			}
			catch (Exception)
			{
				return 2;
			}
		}

		private void RegularFileCheckerThread(object state)
		{
			try
			{
                if (protectionPaths == null)
                {
                    return;
                }
				foreach (string path in protectionPaths)
				{
                    //string[] files = Directory.GetFiles(path);
                    DirectoryInfo directoryInfo = new DirectoryInfo(path);
                    string [] files = directoryInfo.GetFiles().OrderBy( unorderedFile => unorderedFile.LastWriteTime).Select( orderedFile => orderedFile.FullName ).ToArray();
					if (files.Length > 0 && !Processing && deleteFileAfterUpload)
					{
						foreach (string file in files)
						{
							ProcessFile(file);
						}
					}
				}
			}
			catch (Exception e)
			{
				DualLogger.Info(this, "RegularFileCheckerThread", "Exception :" + e.Message, null, display);
			}
		}



		private void Export(string username, string password, string store, string exportDirectory)
		{
			try
			{

				Logger.SetLogFilePath(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\Exportlog.txt");
				StreamReader sr = new StreamReader(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\settings.xml");
				String encodedFile, decodedFile;
				encodedFile = sr.ReadToEnd();
				decodedFile = decodeXml(encodedFile);


				XDocument xml = XDocument.Parse(decodedFile);
				IEnumerable<XElement> allNodes = xml.Element("settings").Elements();


				IEnumerable<XElement> nodes = allNodes.Where(g => g.Name == "WebService");
				if (nodes.Count() != 1)
				{
					DualLogger.Error("Retail Bridge Service", "Initialization Failed", "Url not found");
					everythingOK = false;
					return;
				}
				webserviceUrl = nodes.First().Value;

				#region Read or Create ExportHistory.xml

				XmlDocument exportHistoryXml = new XmlDocument();

				try
				{
					exportHistoryXml.Load(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\ExportHistory.xml");
				}
				catch (FileNotFoundException)
				{
					XmlDeclaration dec = exportHistoryXml.CreateXmlDeclaration("1.0", "utf-8", null);
					exportHistoryXml.AppendChild(dec);// Create the root element
					XmlElement root = exportHistoryXml.CreateElement("ExportHistory");
					exportHistoryXml.AppendChild(root);
					XmlElement documentTable = exportHistoryXml.CreateElement("Table");
					XmlElement entityName = exportHistoryXml.CreateElement("EntityName");
					entityName.InnerText = "DocumentHeader";
					XmlElement latestExport = exportHistoryXml.CreateElement("LatestExport");
					latestExport.InnerText = "0";
					documentTable.AppendChild(entityName);
					documentTable.AppendChild(latestExport);
					root.AppendChild(documentTable);
				}

				long currentVersion = 0;
				long latestVersion = 0;
				XmlNodeList tables = exportHistoryXml.GetElementsByTagName("Table");
				foreach (XmlNode table in tables)
				{
					if (table.ChildNodes[0].InnerText == "DocumentHeader")
					{
						long.TryParse(table.ChildNodes[1].InnerText, out currentVersion);
						break;
					}
				}
				#endregion

				string filename = "";
				string backupFileName = "";
				#region Service Call
				using (BridgeWebService.RetailBridgeService webService = new BridgeWebService.RetailBridgeService())
				{

					webService.Url = webserviceUrl;
					webService.Timeout = 5 * 60 * 1000; // 5 minute
					XmlDocument serviceXmlResult = new XmlDocument();
					XmlDeclaration dec = serviceXmlResult.CreateXmlDeclaration("1.0", "utf-8", null);
					serviceXmlResult.AppendChild(dec);// Create the root element
					XmlNode result = webService.GetOrders(username, password, store, currentVersion, out latestVersion);
					if (result == null)
					{
						Logger.Error("Retail Bridge Service", "Export", "Export failed; User credentials are invalid or User is not a supplier.");
						everythingOK = false;
						return;
					}
					serviceXmlResult.LoadXml(result.OuterXml);
					//filename = exportDirectory + String.Format("\\export-{0:yyyy-MM-dd_hh-mm-ss-tt}.xml", DateTime.Now);
					filename = exportDirectory + "\\export.xml";
					backupFileName = exportDirectory + String.Format("\\export-{0:yyyy-MM-dd_hh-mm-ss-tt}.xml", DateTime.Now);
					if (File.Exists(filename))
					{
						File.Move(filename, backupFileName);
					}
					serviceXmlResult.Save(filename);
					DualLogger.Info(this, "Export", "Export completed successfully", null, display);
				}
				#endregion

				tables = exportHistoryXml.GetElementsByTagName("Table");
				foreach (XmlNode table in tables)
				{
					if (table.ChildNodes[0].InnerText == "DocumentHeader")
					{
						table.ChildNodes[1].InnerText = latestVersion.ToString();
						break;
					}
				}
				exportHistoryXml.Save(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\ExportHistory.xml");
				XslCompiledTransform transform = new XslCompiledTransform();
				transform.Load("export.xsl");
				if (File.Exists(Path.ChangeExtension(filename, ".txt")))
				{
					File.Move(Path.ChangeExtension(filename, ".txt"), Path.ChangeExtension(backupFileName, ".txt"));
				}
				transform.Transform(filename, Path.ChangeExtension(filename, ".txt"));
			}
			catch (Exception ex)
			{
				Logger.Error("Retail Bridge Service", "Export Failed", ex.Message);
				everythingOK = false;
			}
		}


		void FileCreated(object sender, FileSystemEventArgs e)
		{
			while (CheckFileHasCopied(e.FullPath) != 0)
			{
				DualLogger.Info(this, "FileCreated", "file: " + e.Name + " is still being copied.", null, display);
				Thread.Sleep(2000);
			}
			ProcessFileDelegate file = new ProcessFileDelegate(ProcessFile);
			file.BeginInvoke(e.FullPath, null, null);
		}

		delegate void ProcessFileDelegate(String fileName);
		void ProcessFile(String fileName)
		{
			Processing = true;
			DualLogger.Info(this, "ProcessFile", "Start process file :" + fileName, null, display);
			try
			{
				FileInfo fileInfo = new FileInfo(fileName);
				BridgeWebService.FileType ft;
				string fileCompressed = "";
				if (!fileInfo.Exists)
				{
					DualLogger.Info(this, "ProcessFile", "file :" + fileName + " has been removed before upload started.", null, display);
					
					Processing = false;
					try
					{
						MailHelper.SendMailMessage(EmailFrom, EmailTo.Split(',').ToList(), "[" + DateTime.Now + "] BridgeClient Results: Error", "[" + DateTime.Now + "] File '" + fileName + "' has been removed before upload started.", SMTPHost, null, null, EmailUsername, EmailPassword, Domain, EnableSSL, Port);
					}
					catch (Exception e)
					{
						DualLogger.Info(this, "ProcessFile", "Error Sending Email :" + e.Message, null, display);
					}
					return;
				}
				string V = fileInfo.Extension.ToUpper();
                string[] validImageFilesUppercase = ValidImageFiles.Split(',').Select( x=> x.ToUpper()).ToArray();
                string[] validTextFilesUppercase = ValidTextFiles.Split(',').Select(x => x.ToUpper()).ToArray();

                if (validImageFilesUppercase.Contains(V.TrimStart('.'))) //== ".JPG" || V == ".JPEG" || V == ".JPE" || V == ".PNG" || V == ".BMP")
				{
					Image img = Image.FromFile(fileName);
					fileCompressed = CompressionHelper.CompressLZMA(img);
					ft = BridgeWebService.FileType.IMAGE;
					img.Dispose();
				}
                else if (validTextFilesUppercase.Contains(V.TrimStart('.')))//(V == ".TXT" || V == ".CSV")
				{
					ft = BridgeWebService.FileType.TEXT;
					try
					{
						using (StreamReader sr = new StreamReader(fileName, encoding))
						{
							fileCompressed = CompressionHelper.CompressLZMA(sr.ReadToEnd());

						}
					}
					catch (IOException ) //File still in use by event handler or system. Waiting 1,5 seconds to release
					{
						Thread.Sleep(1500);
						using (StreamReader sr = new StreamReader(fileName, encoding))
						{
							fileCompressed = CompressionHelper.CompressLZMA(sr.ReadToEnd());

						}
					}
				}
				else
				{
					ft = BridgeWebService.FileType.IMAGE;
					fileCompressed = "";
					DualLogger.Info(this, "ProcessFile", "file :" + fileName + " is not supported", null, display);
					Processing = false;
					try
					{
						MailHelper.SendMailMessage(EmailFrom, EmailTo.Split(',').ToList(), "[" + DateTime.Now + "] BridgeClient Results: Error", "[" + DateTime.Now + "] File '" + fileName + "' is not supported", SMTPHost, null, null, EmailUsername, EmailPassword, Domain, EnableSSL, Port);
					}
					catch (Exception e)
					{
						DualLogger.Info(this, "ProcessFile", "Error Sending Email :" + e.Message, null, display);
					}
					return;
				}

				using (BridgeWebService.RetailBridgeService webService = new BridgeWebService.RetailBridgeService())
				{

					webService.Url = webserviceUrl;
					webService.Timeout = 1 * 60 * 1000; // 1 minute
                    string result = webService.PostFile(ft, fileCompressed, Path.GetFileName(fileName), encoding == null ? "" : encoding.WebName, deviceId, EmailTo, "", createUsersForCustomers, createUsersForStores, customerUserRoleDescription, storeUserRoleDescription, cultureInfo.Name, OwnerTaxCode, MultipleItemCategoryTrees, RootItemCategoryCode, MakeZeroPricesUserDefinable, SendEmailsForImagesOnlyOnError,OneStorePerCustomer);
					DualLogger.Info(this, "ProcessFile", "file :" + fileName + " has been processed. Result:" + result, null, display);
					Processing = false;
					try
					{
                        if (ft != BridgeWebService.FileType.IMAGE || SendEmailsForImagesOnlyOnError == false)
                        {
                            MailHelper.SendMailMessage(EmailFrom, EmailTo.Split(',').ToList(), "[" + DateTime.Now + "] BridgeClient Results: Success", "[" + DateTime.Now + "] File '" + fileName + "' was uploaded succesfully.", SMTPHost, null, null, EmailUsername, EmailPassword, Domain, EnableSSL, Port);
                        }
					}
					catch (Exception e)
					{
						DualLogger.Info(this, "ProcessFile", "Error Sending Email :" + e.Message, null, display);
					}
				}
				if (File.Exists(fileName) && deleteFileAfterUpload)
				{
					File.Delete(fileName);
				}
			}
			catch (Exception ex)
			{
				Processing = false;
				DualLogger.Info(this, "ProcessFile", "Problem processing file:" + fileName, ex, display);
				MailHelper.SendMailMessage(EmailFrom, EmailTo.Split(',').ToList(), "[" + DateTime.Now + "] BridgeClient Results: Error", "[" + DateTime.Now + "] Problem processing file '" + fileName + "'. An exception was thrown: " + ex.Message, SMTPHost, null, null, EmailUsername, EmailPassword, Domain, EnableSSL, Port);
				return;
			}
		}



	}
}
