using System;
using System.Collections.Generic;
using System.Text;

namespace Telerik.WinControls.Design
{
    /// <summary>
    /// This attribute should be used on classes which will be present in the Visual Studio toolbox (i.e. the ones that should also have a <see cref="System.Drawing.ToolboxBitmapAttribute"/> attribute).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class TelerikToolboxCategoryAttribute : Attribute
    {
        string categoryTitle;

        public string CategoryTitle 
        {
            get { return this.categoryTitle; }
            set { this.categoryTitle = value; }
        }

        /// <summary>
        /// Creates a new instance of the ToolboxCategory attribute with the specified title.
        /// </summary>
        /// <param name="categoryTitle">The title of the category where the control will be placed</param>
        public TelerikToolboxCategoryAttribute(string categoryTitle)
        {
            CategoryTitle = categoryTitle;
        }
    }

}
