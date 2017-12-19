using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    public enum ChunkVisibilityState : int
    {
        Expanded = 1,
        SmallImages = 3,
        NoText = 2,
        Collapsed = 4
    }

    public class CollapsibleAdapterFactory
    {
        /// <summary>
        /// Create a Adapter if possible for Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns>The wrapper for Item </returns>
        public static CollapsibleElement CreateAdapter(RadElement item)
        {
            if (item is CollapsibleElement)
            {
                return (CollapsibleElement)item;
            }

            if (item is RadButtonElement && !(item is ActionButtonElement) && item.Visibility == ElementVisibility.Visible)
            {
                return new CollapsableButtonAdapter((RadButtonElement)item);
            }

            return null;
        }
    }
}
