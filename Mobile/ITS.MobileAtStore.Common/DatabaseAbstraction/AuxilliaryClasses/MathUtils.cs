using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.MobileAtStore.Common.DatabaseAbstraction.AuxilliaryClasses
{
    public static class MathUtils
    {
        public static int Pow(int value, int exp)
        {
            int v2, v3, v4, v8;
            switch (exp)
            {
                case 0: return 1;
                case 1: return value;
                case 2: return value * value;
                case 3: v2 = value * value;
                    return value * v2;
                case 4: v2 = value * value;
                    return v2 * v2;
                case 5: v2 = value * value;
                    v3 = v2 * value;
                    return v2 * v3;
                case 6: v2 = value * value;
                    v3 = v2 * value;
                    return v3 * v3;
                case 7: v2 = value * value;
                    v3 = v2 * value;
                    return v2 * v2 * v3;
                case 8: v2 = value * value;
                    v4 = v2 * v2;
                    return v4 * v4;
                case 16:
                    v2 = value * value;
                    v4 = v2 * v2;
                    v8 = v4 * v4;
                    return v8 * v8;
                default:
                    v2 = exp < 16 ? 8 : 16;
                    return Pow(value, v2) * Pow(value, exp - v2);

            }
        }
    }
}
