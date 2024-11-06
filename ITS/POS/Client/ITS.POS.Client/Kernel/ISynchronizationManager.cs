using DevExpress.Xpo;
using ITS.POS.Client.Synchronization;
using ITS.Retail.Platform.Enumerations;
using ITS.Retail.Platform.Kernel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.POS.Client.Kernel
{
    interface ISynchronizationManager : IKernelModule
    {
        bool ServiceIsAlive { get; set; }
        ConcurrentDictionary<Type, bool> EntitiesToForceDownload { get; }
        CustomThread UpdateStatusThread { get; set; }
        CustomThread AutoFocusThread { get; set; }
        CustomThread GetUpdatesThread { get; set; }
        CustomThread PublishStatusThread { get; set; }
        CustomThread PostSynchronizationInfoThread { get; set; }
        PostThread PostTransactionsThread { get; set; }
        eDownloadingStatus CurrentDownloadingStatus { get; }
        Type CurrentUploadingType { get; }
        Type CurrentDownloadingType { get; }
        eUploadingStatus CurrentUploadingStatus { get; }
        int CurrentDownloadingProgress { get; }
        int CurrentUploadingProgress { get; }
        bool UpdateKeyMappings { get; set; }
        void SetVersion(Type type, DateTime ver, UnitOfWork versionsUow);
        bool PostTransactionsAndPauseThread(int retryNo);
        void ResumePostTransactionThread();
        void CheckDocumentSequenceNumber(Guid documentSeries, out int localNumber, out int remoteNumber);
        void CheckZReportSequenceNumber(Guid terminalOid, out int localNumber, out int remoteNumber);
    }
}
