using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// RadWebBrowserElement extends RadWebBrowserItem adding border and background fill.
    /// </summary>
    [ToolboxItem(false), ComVisible(false)]
    public class RadWebBrowserElement : RadItem
    {
        #region Private fields

        private RadWebBrowserItem webBrowserItem;
        private FillPrimitive fillPrimitive;
        private BorderPrimitive borderPrimitive;

        #endregion

        #region Constructors & initialization

        protected override void CreateChildElements()
        {
            fillPrimitive = new FillPrimitive();
            fillPrimitive.Class = "TextBoxFill";
            fillPrimitive.BackColor = Color.White;

            borderPrimitive = new BorderPrimitive();
            borderPrimitive.Class = "TextBoxBorder";

            this.webBrowserItem = new RadWebBrowserItem();
            this.Children.AddRange(webBrowserItem, fillPrimitive, borderPrimitive);

            base.CreateChildElements();
        }

        #endregion

        #region Properties

        /// <commentsfrom cref="RadWebBrowserItem.Url" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadWebBrowserItem.Url"/>
        public Uri Url
        {
            get { return this.WebBrowserItem.Url; }
        }

        /// <commentsfrom cref="RadWebBrowserItem.DocumentText" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadWebBrowserItem.DocumentText"/>
        public string DocumentText
        {
            get { return this.WebBrowserItem.DocumentText; }
            set { this.WebBrowserItem.DocumentText = value; }
        }

        /// <commentsfrom cref="RadWebBrowserItem.DocumentTitle" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadWebBrowserItem.DocumentTitle"/>
        public string DocumentTitle
        {
            get { return this.WebBrowserItem.DocumentTitle; }
        }

        /// <commentsfrom cref="RadWebBrowserItem" filter="##SUMMARY,##OVERLOADS,##REMARKS,##VALUE,##NOTES"/>
        /// <seealso cref="RadWebBrowserItem"/>
        public RadWebBrowserItem WebBrowserItem
        {
            get 
            {
                return this.webBrowserItem;
            }
        }

        /// <summary>
        /// Gets the <see cref="FillPrimitive"/> of the <see cref="RadWebBrowserElement"/>
        /// </summary>
        /// <seealso cref="FillPrimitive"/>
        public FillPrimitive FillPrimitive
        {
            get
            {
                return this.fillPrimitive;
            }
        }

        /// <summary>
        /// Gets the <see cref="BorderPrimitive"/> of the <see cref="RadWebBrowserElement"/>
        /// </summary>
        /// <seealso cref="BorderPrimitive"/>
        public BorderPrimitive BorderPrimitive
        {
            get
            {
                return this.borderPrimitive;
            }
        }


        /// <summary>
        /// Gets or Sets value indicating whether the <see cref="BorderPrimitive"/> is visible
        /// </summary>
        /// <seealso cref="BorderPrimitive"/>
        public bool ShowBorder
        {
            get { return BorderPrimitive.Visibility == ElementVisibility.Visible; }

            set
            {
                if ((BorderPrimitive.Visibility == ElementVisibility.Visible) != value)
                {
                    BorderPrimitive.Visibility = value ? ElementVisibility.Visible : ElementVisibility.Collapsed;
                }
            }
        }

        #endregion
    }
}
