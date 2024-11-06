using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ITS.Retail.Platform.Tests.Common
{
    public abstract class Fixture
    {
        public abstract void Initialize();
        public abstract void Dispose();

        public static Dictionary<Type, Fixture> InitializedFixtures { get; set; }

        public static Fixture Current 
        {
            get
            {
                if (InitializedFixtures == null)
                {
                    InitializedFixtures = new Dictionary<Type, Fixture>();
                }

                StackTrace stackTrace = new StackTrace();
                MethodBase callingMethod = stackTrace.GetFrame(1).GetMethod();
                Type callingType = callingMethod.DeclaringType;

                if (InitializedFixtures.ContainsKey(callingType))
                {
                    return InitializedFixtures[callingType];
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
