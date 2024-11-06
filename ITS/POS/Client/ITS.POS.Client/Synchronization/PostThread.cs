using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ITS.POS.Client.Synchronization
{
    public class PostThread : CustomThread
    {
        public PostThread(ThreadStart threadStart, ThreadType type)
            : base(threadStart, type)
        {
        }

        private volatile bool startedForcePost = false;
        public bool StartedForcePost
        {
            get
            {
                return startedForcePost;
            }
            set
            {
                startedForcePost = value;
            }
        }

        private volatile bool completedForcePost = false;
        public bool CompletedForcePost
        {
            get
            {
                return completedForcePost;
            }
            set
            {
                completedForcePost = value;
            }
        }



    }
}
