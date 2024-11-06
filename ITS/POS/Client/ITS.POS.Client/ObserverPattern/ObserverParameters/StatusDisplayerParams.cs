using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ITS.Retail.Platform.Enumerations;

namespace ITS.POS.Client.ObserverPattern.ObserverParameters
{
    public class StatusDisplayerParams : ObserverParams
    {
        public bool IsConnected { get; set; }
        public eDownloadingStatus DownloadingStatus { get; set; }
        public Type DownloadingType { get; set; }
        public int DownloadProgress { get; set; }
        public eUploadingStatus UploadingStatus { get; set; }
        public Type UploadingType { get; set; }
        public int UploadProgress { get; set; }
        public eMachineStatus MachineStatus { get; set; }


        public override Type GetObserverType()
        {
            return typeof(IObserverStatusDisplayer);
        }

        public StatusDisplayerParams(bool IsConnected, eDownloadingStatus DownloadingStatus, Type DownloadingType, eUploadingStatus UploadingStatus, Type UploadingType,int DownloadProgress,int UploadProgress,eMachineStatus MachineStatus)
        {
            this.IsConnected = IsConnected;
            this.DownloadingStatus = DownloadingStatus;
            this.DownloadingType = DownloadingType;
            this.UploadingStatus = UploadingStatus;
            this.UploadingType = UploadingType;
            this.DownloadProgress = DownloadProgress;
            this.UploadProgress = UploadProgress;
            this.MachineStatus = MachineStatus;
        }
    }
}
