using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.VisualStyles;

namespace Telerik.WinControls
{
    /// <summary>
    /// Defines a visual element which may be displayed using system skins (UxTheme semantic).
    /// </summary>
    public interface ISupportSystemSkin
    {
        /// <summary>
        /// Gets the VisualStyleElement which represents the current state of this instance for Windows XP.
        /// </summary>
        /// <returns></returns>
        VisualStyleElement GetXPVisualStyle();
        /// <summary>
        /// Gets the VisualStyleElement which represents the current state of this instance for Windows Vista.
        /// </summary>
        /// <returns></returns>
        VisualStyleElement GetVistaVisualStyle();
        /// <summary>
        /// Determines whether to use system skins or not.
        /// If this is false, the default TPF rendering will be used.
        /// If this is true and there is no system skin enabled, TPF rendering will be used.
        /// </summary>
        UseSystemSkinMode UseSystemSkin
        {
            get;
            set;
        }
    }
}
