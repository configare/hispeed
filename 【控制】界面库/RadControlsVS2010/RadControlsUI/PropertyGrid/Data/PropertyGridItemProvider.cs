using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.UI
{
    public abstract class PropertyGridItemProvider : IDisposable
    {
        private PropertyGridTableElement propertyGridElement;
        private int update = 0;

        public PropertyGridItemProvider(PropertyGridTableElement propertyGridElement)
        {
            this.propertyGridElement = propertyGridElement;
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <returns></returns>
        public abstract IList<PropertyGridItem> GetItems();

        /// <summary>
        /// Sets the current.
        /// </summary>
        /// <param name="item">The item.</param>
        public virtual void SetCurrent(PropertyGridItem item)
        {

        }

        /// <summary>
        /// Gets the tree view.
        /// </summary>
        /// <value>The tree view.</value>
        public PropertyGridTableElement RadPropertyGridElement
        {
            get { return propertyGridElement; }
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public virtual void Reset()
        {
           
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            
        }

        /// <summary>
        /// Suspends the update.
        /// </summary>
        public virtual void SuspendUpdate()
        {
            this.update++;
        }

        /// <summary>
        /// Resumes the update.
        /// </summary>
        public virtual void ResumeUpdate()
        {
            this.update--;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is suspended.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is suspended; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuspended
        {
            get
            {
                return this.update > 0;
            }
        }
    }
}
