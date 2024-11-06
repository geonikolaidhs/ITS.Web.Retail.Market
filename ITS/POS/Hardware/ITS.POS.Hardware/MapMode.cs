using System;
using System.Collections.Generic;
using System.Text;

namespace ITS.POS.Hardware
{
    public enum MapMode
    {
        /// <summary>
        /// The device's dot width.
        /// </summary>
        Dots = 1,

        /// <summary>
        /// 1/1440 of an inch
        /// </summary>
        Twips = 2,

        /// <summary>
        /// 0.001 inch
        /// </summary>
        English = 3,

        /// <summary>
        /// 0.01 millimeter
        /// </summary>
        Metric = 4,
    }
}
