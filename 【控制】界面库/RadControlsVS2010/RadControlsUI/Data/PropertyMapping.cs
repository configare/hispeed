using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI.Data
{
    /// <summary>
    /// Represents the simple binding between the property of an item from the data store and 
    /// the property of an item from RadScheduler. The RadScheduler items are events, resources, etc.
    /// </summary>
    public class PropertyMapping
    {
        private string logicalItemProperty = string.Empty;
        private string dataSourceProperty = string.Empty;

        /// <summary>
        /// Initializes a new instance of the SchedulerMapping class that simple-binds the 
        /// indicated property of an item from RadScheduler to the specified item from the data store.
        /// </summary>
        /// <param name="logicalItemProperty">Property name of an item in RadScheduler.</param>
        /// <param name="dataSourceProperty">Property name of an item in the data store.</param>
        public PropertyMapping(string logicalItemProperty, string dataSourceProperty)
        {
            this.logicalItemProperty = logicalItemProperty;
            this.dataSourceProperty = dataSourceProperty;
        }

        /// <summary>
        /// The callback that converts the given value object from the data store to the specified type of the RadScheduler corresponding item.
        /// </summary>
        public ConvertCallback ConvertToLogicalValue;

        /// <summary>
        /// The callback that converts the given value object from a RadScheduler item to the specified type of the data store corresponding item. 
        /// </summary>
        public ConvertCallback ConvertToDataSourceValue;

        /// <summary>
        /// Gets or sets the RadScheduler item property name that is mapped.
        /// </summary>
        public string LogicalItemProperty
        {
            get
            {
                return this.logicalItemProperty;
            }
            set
            {
                this.logicalItemProperty = value;
            }
        }

        /// <summary>
        /// Gets or sets the data store item property name that is mapped.
        /// </summary>
        public string DataSourceItemProperty
        {
            get
            {
                return this.dataSourceProperty;
            }
            set
            {
                this.dataSourceProperty = value;
            }
        }
    }
}
