using System;
using System.Drawing;

namespace ITS.POS.Tools.FormDesigner.Host
{
    /// <summary>
    /// Generated random color. It is used by MyRootDesigner
    /// </summary>
    public class RandomUtil
    {
        private static Random rand = null;

        public RandomUtil()
        {
            if ( rand == null )
            {
                InitializeRandoms((int)(DateTime.Now.Ticks % Int32.MaxValue));
            }
        }

        private void InitializeRandoms(int seed)
        {
            rand = new Random(seed);
        }

        public virtual Color GetColor()
        {
            byte rval, gval, bval;

            rval = (byte)GetRange(0, 255);
            gval = (byte)GetRange(0, 255);
            bval = (byte)GetRange(0, 255);

            Color c = Color.FromArgb(rval, gval, bval);

            return c;
        }
        public int GetRange(int nMin, int nMax)
        {
            if (nMin > nMax)
            {
                int nTemp = nMin;

                nMin = nMax;
                nMax = nTemp;
            }

            if (nMax != Int32.MaxValue)
            {
                ++nMax;
            }
            int retVal = rand.Next(nMin, nMax);

            return retVal;
        }
    }
}
