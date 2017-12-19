using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls
{
    /// <summary>
    /// Represents a collection of PropertySettingGroup objects.
    /// </summary>
    public class PropertySettingGroupCollection : List<PropertySettingGroup>
    {
        /// <summary>
        /// Initializes a new instance of the PropertySettingGroupCollection class.
        /// </summary>
        public PropertySettingGroupCollection() : base(1) { }

        /// <summary>
        /// Initializes a new instance of the PropertySettingGroupCollection class
        /// using proposed capacity.
        /// </summary>
        /// <param name="capacity"></param>
        public PropertySettingGroupCollection(int capacity) : base(capacity) { }
    }
}
