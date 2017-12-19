using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    public enum UseSystemSkinMode
    {
        /// <summary>
        /// Mode is inherited by the parent chain.
        /// </summary>
        Inherit,
        /// <summary>
        /// Only direct element can use skins, value cannot be used for inheritance
        /// </summary>
        YesLocal,
        /// <summary>
        /// The element and all its descendants may use skins.
        /// </summary>
        YesInheritable,
        /// <summary>
        /// Only direct element is forbidden to use skins, its children can compose this value up from the parent chain.
        /// </summary>
        NoLocal,
        /// <summary>
        /// Element and all its descendants are forbidden to use system skins.
        /// </summary>
        NoInheritable,
    }
}
