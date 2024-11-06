using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using DevExpress.Xpo;
using ITS.POS.Client.Actions.ActionParameters;
using ITS.POS.Client.Kernel;
using ITS.Retail.Platform.Enumerations;
using ITS.POS.Client.Synchronization;
using ITS.POS.Client.Helpers;
using ITS.POS.Hardware;
using ITS.Retail.Platform;
using System.Windows.Forms;
using ITS.POS.Model.Settings;
using ITS.POS.Model.Master;
using ITS.POS.Model.Versions;
using ITS.POS.Model.Transactions;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;

namespace ITS.POS.Client.Actions
{
    /// <summary>
    /// Posts the application's current status to the server and executes any commands issued by the server. For internal use (not directly invoked by the user)
    /// </summary>
    public class ActionPostStatus : Action
    {
        public ActionPostStatus(IPosKernel kernel) : base(kernel)
        {
        }

        public override eActions ActionCode
        {
            get { return eActions.POST_STATUS; }
        }

        public override bool RequiresParameters
        {
            get { return false; }
        }

        protected override eKeyStatus DefaultKeyStatusRequirement
        {
            get
            {
                return eKeyStatus.UNKNOWN;
            }
        }

        public override eMachineStatus ValidMachineStatuses
        {
            get { return eMachineStatus.UNKNOWN | eMachineStatus.SALE | eMachineStatus.PAUSE | eMachineStatus.OPENDOCUMENT_PAYMENT | eMachineStatus.OPENDOCUMENT | eMachineStatus.DAYSTARTED | eMachineStatus.CLOSED; }
        }


        protected override void ExecuteCore(ActionParameters.ActionParams parameters = null, bool dontCheckPermissions = false)
        {
            using (WebService.POSUpdateService ws = new WebService.POSUpdateService())
            {
                IAppContext appContext = Kernel.GetModule<IAppContext>();
                IConfigurationManager config = Kernel.GetModule<IConfigurationManager>();
                ISessionManager sessionManager = Kernel.GetModule<ISessionManager>();
                IActionManager actionManager = this.Kernel.GetModule<IActionManager>();
                ws.Url = config.StoreControllerWebServiceURL;
                ws.Timeout = 6000;
                ISynchronizationManager SynchronizationManager = Kernel.GetModule<ISynchronizationManager>();
                try
                {
                    WebService.POSCommandSet cmds;
                    bool retryFound = false;
                    do
                    {
                        WebService.eMachineStatus status;
                        status = (WebService.eMachineStatus)appContext.GetMachineStatus();
                        cmds = ws.SendPosStatus(config.CurrentTerminalOid, status);
                        SynchronizationManager.ServiceIsAlive = true;

                        foreach (WebService.SerializableTupleOfePosCommandString commandTuple in cmds.Commands)
                        {
                            if (commandTuple.Item1 != WebService.ePosCommand.NONE)
                            {

                                if (commandTuple.Item1.HasFlag(WebService.ePosCommand.RESTART_POS))
                                {
                                    ProcessStartInfo proc = new ProcessStartInfo();
                                    proc.WindowStyle = ProcessWindowStyle.Hidden;
                                    proc.FileName = "cmd";
                                    proc.Arguments = "/C shutdown -f -r -t 0";
                                    Process.Start(proc);
                                }

                                if (commandTuple.Item1.HasFlag(WebService.ePosCommand.ISSUE_X))
                                {
                                    // ActionIssueXReport
                                    appContext.MainForm.Invoke((MethodInvoker)delegate ()
                                    {
                                        actionManager.GetAction(eActions.ISSUE_X).Execute(new ActionIssueXReportParams(false), true);
                                    });
                                }

                                if (commandTuple.Item1.HasFlag(WebService.ePosCommand.ISSUE_Z))
                                {
                                    appContext.MainForm.Invoke((MethodInvoker)delegate ()
                                    {
                                        actionManager.GetAction(eActions.ISSUE_Z).Execute(new ActionIssueZReportParams(false, false), true);
                                    });
                                }
                                if (commandTuple.Item1.HasFlag(WebService.ePosCommand.RETRY_IMMEDIATE))
                                {
                                    retryFound = true;
                                    break; //not sure
                                }

                                #region
                                /// <summary>
                                /// Code manages sql commands from SC to POS
                                /// </summary>
                                if (commandTuple.Item1.HasFlag(WebService.ePosCommand.EXECUTE_POS_SQL))
                                {
                                    String commandType = String.Empty;
                                    String command = String.Empty;
                                    try
                                    {
                                        UnitOfWork uow;
                                        String[] arrayCommandsParameters = commandTuple.Item2.Split('?');
                                        String dbType = arrayCommandsParameters[0];
                                        int tm = 6;
                                        if (arrayCommandsParameters.Count() > 1)
                                        {
                                            int.TryParse(arrayCommandsParameters[1], out tm);
                                        }
                                        ws.Timeout = tm * 1000;

                                        String queryString = arrayCommandsParameters[2].TrimStart();
                                        command = queryString;

                                        switch (dbType.ToUpper())
                                        {
                                            case "POS_SETTINGS":
                                                uow = sessionManager.GetSession<DocumentType>();
                                                break;
                                            case "POS_MASTER":
                                                uow = sessionManager.GetSession<Item>();
                                                break;
                                            case "POS_VERSION":
                                                uow = sessionManager.GetSession<TableVersions>();
                                                break;
                                            case "POS_TRANSCATION":
                                                uow = sessionManager.GetSession<DocumentHeader>();
                                                break;
                                            default:
                                                uow = sessionManager.GetSession<DocumentType>();
                                                break;

                                        }

                                        var result = uow.ExecuteQuery(queryString);
                                        StringBuilder sb = new StringBuilder();
                                        string statementType = queryString.Substring(0, 6);

                                        if (statementType.ToUpper() == "SELECT")
                                        {
                                            if (result != null)
                                            {
                                                if (result.ResultSet != null && result.ResultSet.Length > 0)
                                                {
                                                    foreach (var row in result.ResultSet[0].Rows)
                                                    {
                                                        foreach (var val in row.Values)
                                                        {
                                                            if (val != null)
                                                            {
                                                                sb.Append(val.ToString());
                                                                sb.Append(",");
                                                            }
                                                        }
                                                        sb.Append(Environment.NewLine);
                                                        sb.Append("|");
                                                    }
                                                }
                                            }
                                        }
                                        else if (statementType.ToUpper() == "UPDATE")
                                        {
                                            sb.Append("Success");
                                        }
                                        else if (statementType.ToUpper() == "DELETE")
                                        {
                                            sb.Append("Success");
                                        }
                                        if (String.IsNullOrEmpty(sb.ToString()))
                                        {
                                            sb.Append("No Results");
                                        }
                                        SendCommandResult(config, ePosCommand.EXECUTE_POS_SQL.ToString(), command, sb.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        SendCommandResult(config, ePosCommand.EXECUTE_POS_SQL.ToString(), command, ex.Message);
                                    }
                                }
                                #endregion


                                #region
                                /// <summary>
                                /// Downloads and Install a new version from SC
                                /// Keeps backup the pos folder 
                                /// Uses the Same Configuration Files & Pos Databases
                                /// </summary>
                                /// 
                                if (commandTuple.Item1.HasFlag(WebService.ePosCommand.POS_UPDATE))
                                {
                                    String[] arrayCommandsParameters = commandTuple.Item2.Split('?');
                                    int tm = 6;
                                    if (arrayCommandsParameters.Count() > 1)
                                    {
                                        int.TryParse(arrayCommandsParameters[0], out tm);
                                    }
                                    ws.Timeout = tm * 1000;
                                    string output = string.Empty;

                                    try
                                    {
                                        System.IO.DirectoryInfo posDir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                                        DirectoryInfo parentDir = posDir.Parent;
                                        bool exists = System.IO.Directory.Exists(parentDir.FullName + "\\" + "posBackUp");
                                        string bckDir = parentDir.FullName + "\\" + "posBackUp";
                                        if (!exists)
                                        {
                                            Directory.CreateDirectory(bckDir);
                                        }
                                        else
                                        {
                                            DeleteAllFromDir(bckDir);

                                        }
                                        string sourceDirectory = posDir.FullName.ToString();
                                        string targetDirectory = bckDir;
                                        string UpdatedDir = string.Empty;
                                        try
                                        {
                                            Copy(sourceDirectory, targetDirectory);
                                            UpdatedDir = parentDir.FullName + "\\" + "newDir";
                                            if (!Directory.Exists(UpdatedDir))
                                            {
                                                Directory.CreateDirectory(UpdatedDir);
                                            }
                                            else
                                            {
                                                DeleteAllFromDir(UpdatedDir);
                                            }

                                            string confDir = UpdatedDir + "\\" + "Configuration";
                                            if (!Directory.Exists(confDir))
                                            {
                                                Directory.CreateDirectory(confDir);
                                            }
                                            else
                                            {
                                                DeleteAllFromDir(confDir);
                                            }

                                            Copy(targetDirectory + "\\" + "Configuration", UpdatedDir + "\\" + "Configuration");
                                            string[] files = System.IO.Directory.GetFiles(targetDirectory);

                                            foreach (string file in files)
                                            {
                                                string fileName = System.IO.Path.GetFileName(file);
                                                string destFile = System.IO.Path.Combine(UpdatedDir, fileName);

                                                if (fileName.ToUpper().Contains("POSMASTER"))
                                                {
                                                    System.IO.File.Copy(file, destFile, true);
                                                }
                                                if (fileName.ToUpper().Contains("POSETTINGS"))
                                                {
                                                    System.IO.File.Copy(file, destFile, true);
                                                }
                                                if (fileName.ToUpper().Contains("POSTRANSACTIONS"))
                                                {
                                                    System.IO.File.Copy(file, destFile, true);
                                                }
                                                if (fileName.ToUpper().Contains("POSVERSIONS"))
                                                {
                                                    System.IO.File.Copy(file, destFile, true);
                                                }
                                                if (fileName.ToUpper().Contains("LOADER"))
                                                {
                                                    System.IO.File.Copy(file, destFile, true);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            SendCommandResult(config, ePosCommand.POS_UPDATE.ToString(), "POS-UPDATE", ex.Message);
                                        }

                                        string lastFolderName = Path.GetFileName(Path.GetDirectoryName(sourceDirectory));
                                        string OriginalFolderName = new DirectoryInfo(sourceDirectory).Name;
                                        string args = "TASKKILL /F /IM its.pos.client.exe&PING 127.0.0.1&rename " + sourceDirectory + " " + OriginalFolderName + "_old&rename " + UpdatedDir + " " + OriginalFolderName + "&" + sourceDirectory + "\\ITS.POS.Client.Loader.exe";
                                        output = CreateAndRunUpdateBatfile(sourceDirectory, args, config);
                                        if (File.Exists(sourceDirectory + "//" + "Update.bat"))
                                        {
                                            File.Delete(sourceDirectory + "//" + "Update.bat");
                                        }
                                        SendCommandResult(config, ePosCommand.POS_UPDATE.ToString(), "POS-UPDATE", output);
                                    }
                                    catch (Exception ex)
                                    {
                                        SendCommandResult(config, ePosCommand.POS_UPDATE.ToString(), "POS-UPDATE", ex.Message);
                                    }
                                }

                                #endregion



                                #region
                                /// <summary>
                                /// Executes a cmd command in a new Proccess
                                /// </summary>
                                /// 
                                if (commandTuple.Item1.HasFlag(WebService.ePosCommand.EXECUTE_POS_CMD))
                                {
                                    string command = string.Empty;
                                    try
                                    {

                                        String[] arrayCommandsParameters = commandTuple.Item2.Split('?');
                                        int tm = 6;
                                        if (arrayCommandsParameters.Count() > 1)
                                        {
                                            int.TryParse(arrayCommandsParameters[0], out tm);
                                        }
                                        ws.Timeout = tm * 1000;
                                        string output = string.Empty;
                                        command = arrayCommandsParameters[1].TrimStart();
                                        Process process = new Process
                                        {
                                            StartInfo =
                                            {
                                                UseShellExecute = false,
                                                RedirectStandardOutput = true,
                                                RedirectStandardError = true,
                                                CreateNoWindow = true,
                                                FileName = "cmd.exe",
                                                Arguments = "/C "+  command
                                            }
                                        };
                                        process.Start();
                                        process.WaitForExit();
                                        if (process.HasExited)
                                        {
                                            output = process.StandardOutput.ReadToEnd();
                                        }
                                        SendCommandResult(config, ePosCommand.EXECUTE_POS_CMD.ToString(), command, output);
                                    }
                                    catch (Exception ex)
                                    {
                                        SendCommandResult(config, ePosCommand.EXECUTE_POS_CMD.ToString(), command, ex.Message);
                                    }
                                }
                                #endregion



                                #region
                                /// <summary>
                                /// Executes a cmd command in a new Proccess
                                /// </summary>                                
                                if (commandTuple.Item1.HasFlag(WebService.ePosCommand.POS_APPLICATION_RESTART))
                                {
                                    try
                                    {
                                        String[] arrayCommandsParameters = commandTuple.Item2.Split('?');
                                        int tm = 6;
                                        if (arrayCommandsParameters.Count() > 1)
                                        {
                                            int.TryParse(arrayCommandsParameters[0], out tm);
                                        }
                                        ws.Timeout = tm * 1000;
                                        string output = string.Empty;
                                        System.IO.DirectoryInfo posDir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
                                        string args = "TASKKILL /F /IM its.pos.client.exe&PING 127.0.0.1&" + posDir.FullName.ToString() + "\\ITS.POS.Client.Loader.exe";
                                        output = CreateAndRunRestartApplicationBat(posDir.FullName.ToString(), args, config);
                                        if (File.Exists(posDir.FullName.ToString() + "//" + "RestartApp.bat"))
                                        {
                                            File.Delete(posDir.FullName.ToString() + "//" + "RestartApp.bat");
                                        }
                                        SendCommandResult(config, ePosCommand.POS_APPLICATION_RESTART.ToString(), ePosCommand.POS_APPLICATION_RESTART.ToString(), output);
                                    }
                                    catch (Exception ex)
                                    {
                                        SendCommandResult(config, ePosCommand.POS_APPLICATION_RESTART.ToString(), ePosCommand.POS_APPLICATION_RESTART.ToString(), ex.Message);
                                    }
                                }
                                #endregion



                                #region
                                /// <summary>
                                /// Reload Entities to Pos
                                /// </summary>  
                                if (commandTuple.Item1.HasFlag(WebService.ePosCommand.RELOAD_ENTITIES))
                                {
                                    string cmdParams = commandTuple.Item2;
                                    string[] entityNames = cmdParams.Split(',');
                                    using (UnitOfWork versionUow = VersionsConnectionHelper.GetNewUnitOfWork())
                                    {
                                        foreach (string entityName in entityNames)
                                        {
                                            bool commitSuccessfull = false;
                                            int tries = 0;
                                            int maxTries = 20;
                                            do
                                            {
                                                try
                                                {
                                                    SynchronizationManager.SetVersion(UtilsHelper.GetEntityByName(entityName), DateTime.MinValue, versionUow);
                                                    commitSuccessfull = true;
                                                }
                                                catch (Exception)  //Just in case we get optimistic lock field errors.
                                                {
                                                    versionUow.RollbackTransaction();
                                                    commitSuccessfull = false;
                                                    Thread.Sleep(1000);
                                                }
                                                tries++;
                                            } while (!commitSuccessfull && tries < maxTries);
                                        }
                                    }
                                }
                                #endregion

                            }
                        }

                    } while (retryFound);
                }
                catch
                {
                    SynchronizationManager.ServiceIsAlive = false;
                }
            }
        }

        private static void SendCommandResult(IConfigurationManager config, String CommandType, String command, String result)
        {
            using (WebService.POSUpdateService wbs = new WebService.POSUpdateService())
            {
                wbs.Url = config.StoreControllerWebServiceURL;
                wbs.Timeout = 6000;
                wbs.CreateCommandResult(config.CurrentTerminalOid, CommandType, command, result);
            }
        }

        private Process GetProccessByName(String procName)
        {
            Process[] allProcess = Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
            if (allProcess.Count() > 0)
            {
                return allProcess[0];
            }
            else
            {
                return null;
            }
        }



        private static void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        private static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                Console.WriteLine(@"Copying {0}\{1}", target.FullName, fi.Name);
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }


        private static void DeleteAllFromDir(string path)
        {

            System.IO.DirectoryInfo di = new DirectoryInfo(path);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        private static string CreateAndRunUpdateBatfile(String path, String args, IConfigurationManager config)
        {
            try
            {
                string batFilePath = path + "\\" + "Update.bat";
                if (File.Exists(batFilePath))
                {
                    File.Delete(batFilePath);
                }
                else
                {
                    using (FileStream fs = File.Create(batFilePath))
                    {
                        fs.Close();
                    }
                }

                using (StreamWriter sw = new StreamWriter(batFilePath))
                {
                    sw.WriteLine(args);
                }
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                SendCommandResult(config, ePosCommand.POS_UPDATE.ToString(), ePosCommand.POS_UPDATE.ToString(), "POS Update is Executing");
                process = Process.Start(batFilePath);
                process.WaitForExit();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        private static string CreateAndRunRestartApplicationBat(String path, String args, IConfigurationManager config)
        {
            try
            {
                string batFilePath = path + "\\" + "RestartApp.bat";
                if (File.Exists(batFilePath))
                {
                    File.Delete(batFilePath);
                }
                else
                {
                    using (FileStream fs = File.Create(batFilePath))
                    {
                        fs.Close();
                    }
                }
                using (StreamWriter sw = new StreamWriter(batFilePath))
                {
                    sw.WriteLine(args);
                }
                Process process = new Process();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;

                SendCommandResult(config, ePosCommand.POS_APPLICATION_RESTART.ToString(), ePosCommand.POS_APPLICATION_RESTART.ToString(), "RestartApp File is Executing");

                process = Process.Start(batFilePath);
                process.WaitForExit();
                return "ok";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
