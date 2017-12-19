using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Holds information about the strips in a <c ref="RadCommandBarElement"/>
    /// </summary>
    public class CommandBarStripInfoHolder
    {
        private List<CommandBarStripElement> stripInfoList;

        /// <summary>
        /// Gets a list of <c ref="CommandBarStripElement"/> elements for which the <c ref="StripInfoHolder"/> is storing info.
        /// </summary>
        public List<CommandBarStripElement> StripInfoList
        {
            get
            {
                return stripInfoList;
            }
        }

        public CommandBarStripInfoHolder()
        {
            stripInfoList = new List<CommandBarStripElement>();
        }

        /// <summary>
        /// Adds information about a specific strip to the <c ref="StripInfoHolder"/>
        /// </summary>
        /// <param name="strip">The <c ref="CommandBarStripElement"/> object to add info about.</param>
        public void AddStripInfo(CommandBarStripElement strip)
        {
            if (!stripInfoList.Contains(strip))
            {
                stripInfoList.Add(strip);
            }
        }

        /// <summary>
        /// Removes information about a specific strip from the <c ref="StripInfoHolder"/>
        /// </summary>
        /// <param name="strip">The <c ref="CommandBarStripElement"/> object to remove info about.</param>
        public void RemoveStripInfo(CommandBarStripElement strip)
        {
            stripInfoList.Remove(strip);
        } 
    }
}
