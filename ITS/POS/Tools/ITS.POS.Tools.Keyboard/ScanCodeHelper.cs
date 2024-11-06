using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace ITS.POSKeyboardTool
{
    public static class ScanCodeHelper
    {

        /// <summary>
        /// The MapVirtualKey function translates (maps) a virtual-key code into a scan
        /// code or character value, or translates a scan code into a virtual-key code    
        /// </summary>
        /// <param name="uCode">[in] Specifies the virtual-key code or scan code for a key.
        /// How this value is interpreted depends on the value of the uMapType parameter
        /// </param>
        /// <param name="uMapType">[in] Specifies the translation to perform. The value of this
        /// parameter depends on the value of the uCode parameter.
        /// </param>
        /// <returns>Either a scan code, a virtual-key code, or a character value, depending on
        /// the value of uCode and uMapType. If there is no translation, the return value is zero
        /// </returns>
        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint uCode, MapVirtualKeyMapTypes uMapType);

        public static int GetScanCode(Keys key)
        {
            uint keyCode = Convert.ToUInt32((int)key);
            return MapVirtualKey(keyCode, MapVirtualKeyMapTypes.MAPVK_VK_TO_VSC);
        }

        public static Keys GetKeyCode(uint scanCode)
        {
            return (Keys)MapVirtualKey(scanCode, MapVirtualKeyMapTypes.MAPVK_VSC_TO_VK_EX);
        }
    }
}
