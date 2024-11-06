using ITS.POS.Resources;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ITS.POS.Client.Kernel
{
    /// <summary>
    /// Handles the localization of controls.
    /// </summary>
    public class ControlLocalizer : IControlLocalizer
    {
        private IFormManager FormManager { get; set; }
        private ILocalizationResolver LocalizationResolver { get; set; }
        private static Dictionary<Control, IEnumerable<Control>> ChildControlsCache = new Dictionary<Control, IEnumerable<Control>>();

        public ControlLocalizer(ILocalizationResolver localizationResolver, IFormManager formManager)
        {
            FormManager = formManager;
            LocalizationResolver = localizationResolver;
        }

        /// <summary>
        /// Localizes a control and all it's child controls.
        /// </summary>
        /// <param name="control"></param>
        public void LocalizeControl(Control control)
        {
            control.Text = LocalizationResolver.ResolveDisplayText(control.Text);
            if (ChildControlsCache.ContainsKey(control) == false)
            {
                ChildControlsCache[control] = FormManager.GetAllChildControls(control);
            }

            foreach (Control childControl in ChildControlsCache[control])
            {
                childControl.Text = LocalizationResolver.ResolveDisplayText(childControl.Text);
            }
        }
        public void ClearDisposedObjectsFromCache(bool clearAll)
        {
            if (clearAll)
            {
                ChildControlsCache.Clear();
            }
            else
            {
                
                ChildControlsCache.Where(control => control.Key.IsDisposed)
                                .Select(control=>control.Key).ToList()
                                .ForEach(
                                        control=>
                                            ChildControlsCache.Remove(control)
                                        );
            }
        }
    }
}
