using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ITS.POS.Client.Synchronization
{
    public class CustomThread
    {
        private int sleepMilisseconds = 500;
        private Thread _innerThread;
        public ThreadType Type {get; set;}
        private volatile bool _isAborted = false ;
        public bool IsAborted
        {
            get
            {
                return _isAborted;
            }
        }

        private volatile bool forceUpdate = false;
        public bool ForceUpdate
        {
            get
            {
                return forceUpdate;
            }
            set
            {
                forceUpdate = value;
            }
        }

        public bool IsAlive
        {
            get
            {
                return _innerThread.IsAlive;
            }
        }

        public CustomThread(ThreadStart threadStart, ThreadType type)
        {
            _innerThread= new Thread(threadStart);
            this.Type = type;
        }

        /// <summary>
        /// Waits until the thread has ended.
        /// Warning: Depending on the thread, it may have to be aborted first or it will never end.
        /// </summary>
        public void WaitToEnd()
        {
            while (_innerThread.IsAlive)
            {
                Thread.Sleep(1000);
            }
        }

        public void Start()
        {
            _innerThread.Start();
        }

        /// <summary>
        /// Flags the thread for abortion. It must be handled in the thread's code properly to actually abort.
        /// </summary>
        public void Abort()
        {
            _isAborted = true;
        }

        public void Sleep(int miliseconds)
        {
            for (int i = 0; i < miliseconds; i += sleepMilisseconds)
            {
                if (_isAborted || ForceUpdate)
                {
                    break;
                }
                Thread.Sleep(sleepMilisseconds);
            }
        }

        public bool Join(int millisecondsTimeout)
        {
            if(_innerThread != null)
            {
                return _innerThread.Join(millisecondsTimeout);
            }
            {
                return false;
            }
        }

    }
}
