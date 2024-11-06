using ITS.Retail.Platform.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.Retail.Platform.Common.Helpers
{
    public class AllNumericPolicies :
        INumericPolicy<int>,
        INumericPolicy<long>,
        INumericPolicy<float>,
        INumericPolicy<double>,
        INumericPolicy<decimal>
    {
        int INumericPolicy<int>.Zero() { return 0; }
        long INumericPolicy<long>.Zero() { return 0; }
        float INumericPolicy<float>.Zero() { return 0.0f; }
        double INumericPolicy<double>.Zero() { return 0.0; }
        decimal INumericPolicy<decimal>.Zero() { return 0; }

        int INumericPolicy<int>.Add(int a, int b) { return a + b; }
        long INumericPolicy<long>.Add(long a, long b) { return a + b; }
        float INumericPolicy<float>.Add(float a, float b) { return a + b; }
        double INumericPolicy<double>.Add(double a, double b) { return a + b; }
        decimal INumericPolicy<decimal>.Add(decimal a, decimal b) { return a + b; }
        // implement all functions from INumericPolicy<> interfaces.

        public static AllNumericPolicies P = new AllNumericPolicies();
    }
}
