using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Xml;

namespace ITS.Service.Guard
{
    public static class ServiceHelper
    {
        private static int _failCounter { get; set; } = 0;
        public static ServiceController GetServiceByName(String serviceName)
        {
            return ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName);
        }

        public static ServiceControllerStatus GetServiceStatus(ServiceController sc)
        {
            sc.Refresh();
            return sc.Status;
        }

        public static String UpdateService(ServiceController sc)
        {
            ServiceControllerStatus status = GetServiceStatus(sc);
            Extensions.UpdateUI("LabelOperation", "Operation In Proggress :  Check Service", "Check Service");
            Extensions.UpdateUI("ServiceStatusLabel", "Service Status :  " + status.ToString());
            String operation = "None";

            switch (status)
            {
                case ServiceControllerStatus.Running:
                    operation = "Check Service";
                    break;
                case ServiceControllerStatus.Stopped:
                    operation = "Start Service";
                    StartService(sc, Settings.getInstance().ServiceWaitTime);
                    break;
                case ServiceControllerStatus.Paused:
                    operation = "Start Service";
                    StartService(sc, Settings.getInstance().ServiceWaitTime);
                    break;
                case ServiceControllerStatus.StopPending:
                    operation = "Check Service";
                    break;
                case ServiceControllerStatus.StartPending:
                    operation = "Check Service";
                    break;
                default:
                    RestartService(sc, Settings.getInstance().ServiceWaitTime);
                    break;
            }
            status = GetServiceStatus(sc);
            if (status != ServiceControllerStatus.Running)
            {
                _failCounter = _failCounter + 1;
                if (_failCounter > 4)
                {
                    KillServiceProccess(sc.ServiceName);
                }
            }
            String operationText = "Last Operation : " + operation + " " + DateTime.Now.ToString();
            Extensions.UpdateUI("ServiceStatusLabel", "Service Status :  " + status.ToString());
            Extensions.UpdateUI("LabelOperation", "Operation In Proggress :  None");
            Extensions.UpdateUI("LabelLastOperation", operationText);
            return sc.ServiceName;

        }


        public static void StartService(ServiceController service, TimeSpan timeout)
        {
            try
            {
                service.Refresh();
                if (service.Status != ServiceControllerStatus.StartPending && service.Status != ServiceControllerStatus.Running)
                {
                    Extensions.UpdateUI("LabelOperation", "Operation In Proggress :  Starting service in .. " + timeout.TotalSeconds.ToString() + " seconds");
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);

                }
            }
            catch (Exception e)
            {
                Extensions.UpdateUI("LabelOperation", "Operation StartService failed with error " + e.Message, "Start Service");
                Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error);
            }
        }

        public static void StopService(ServiceController service, TimeSpan timeout)
        {
            try
            {
                service.Refresh();
                if (service.Status == ServiceControllerStatus.Running)
                {
                    Extensions.UpdateUI("LabelOperation", "Operation In Proggress :  Stoping service in.. " + timeout.TotalSeconds.ToString() + " seconds");
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }
            }
            catch (Exception e)
            {
                Extensions.UpdateUI("LabelOperation", "Operation Stop Service failed with error " + e.Message, "Stop Service");
                Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error);
            }
        }


        public static void RestartService(ServiceController service, TimeSpan timeout)
        {
            try
            {
                service.Refresh();
                if (service.Status == ServiceControllerStatus.Running)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
                }

                if (service.Status != ServiceControllerStatus.StartPending)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                }

            }
            catch (Exception e)
            {
                Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error);
            }
        }


        private static void KillServiceProccess(String serviceName)
        {
            _failCounter = 0;
            try
            {
                foreach (int procId in GetProcessIdByServiceName(serviceName))
                {
                    Process process = Process.GetProcessById(procId);
                    process.Kill();
                }
            }
            catch (Exception e)
            {
                Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error);
            }
        }


        private static List<int> GetProcessIdByServiceName(string serviceName)
        {
            List<int> processesIds = new List<int>();
            try
            {
                string qry = $"SELECT PROCESSID FROM WIN32_SERVICE WHERE NAME = '{serviceName }'";
                var searcher = new ManagementObjectSearcher(qry);
                var managementObjects = new ManagementObjectSearcher(qry).Get();

                foreach (ManagementObject mngntObj in managementObjects)
                {
                    int processId = (int)(uint)mngntObj["PROCESSID"];
                    if (processId != 0)
                    {
                        processesIds.Add((int)(uint)mngntObj["PROCESSID"]);
                    }
                }
            }
            catch (Exception e)
            {
                Program.WriteToWindowsEventLog(e.Message, EventLogEntryType.Error);
            }
            return processesIds;
        }

    }
}
