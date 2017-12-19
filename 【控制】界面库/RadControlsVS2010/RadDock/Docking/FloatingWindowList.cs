using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Telerik.WinControls.UI.Docking
{
    /// <summary>
    /// A serializable collection of <see cref="FloatingWindow">FloatingWindow</see> instances.
    /// </summary>
    public class FloatingWindowList : Collection<SerializableFloatingWindow>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(object item)
        {
            base.Add((SerializableFloatingWindow)item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="floatingWindows"></param>
        public void AddRange(SerializableFloatingWindow[] floatingWindows)
        {
            foreach(SerializableFloatingWindow window in floatingWindows)
            {
                base.Add(window);
            }
        }
    }
}
