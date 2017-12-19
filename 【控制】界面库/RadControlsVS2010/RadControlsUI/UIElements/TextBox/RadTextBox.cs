using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Telerik.WinControls.Design;
using System.Drawing.Design;
using System.Windows.Forms.VisualStyles;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using System.Windows.Forms.Design;
using System.Drawing;
using Telerik.WinControls.Themes.Design;
using System.Security.Permissions;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
	/// <exclude/>
	/// <summary>
	///     Represents a RadTextBox. The RadTextBox control serves as a simple wrapper for
	///     <see cref="RadTextBoxElement">RadTextBoxElement class</see> which in turn wraps
	///     <see cref="RadTextBoxItem">RadTextBoxItem Class</see>. All logic and presentation
	///     features are implemented in a parallel hierarchy of objects. For this reason,
	///     <see cref="RadTextBoxElement">RadTextBoxElement class</see> may be nested in any
	///     other telerik control, item, or element. RadTextBox acts to transfer events to and
	///     from its corresponding instance of the
	///     <see cref="RadTextBoxElement">RadTextBoxElement class</see>.
	/// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.EditorsGroup)]
    [Telerik.WinControls.Themes.Design.RadThemeDesignerData(typeof(RadTextBoxDesignTimeData))]
	[ToolboxItem(true)]
    [Description("Enables the user to enter text, and provides multiline editing and password character masking")]
	[DefaultProperty("Text"), DefaultEvent("TextChanged")]
	public class RadTextBox : RadTextBoxBase
	{
        // Fields
        protected RadTextBoxElement textBoxElement = null;
        protected RadTextBoxItem textBoxItem = null;
        private bool textElementInitialized = false;

        #region Constructors
        /// <summary>
        /// Represents RadTextBox's constructor
        /// </summary>
        public RadTextBox()
        {
            this.textBoxItem = this.TextBoxElement.TextBoxItem;
            this.WireHostEvents();
        }

        #endregion
         
		#region Properties

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        internal override RadTextBoxItem TextBoxItem
        {
            get
            {
                return this.textBoxItem;
            }
        }

        /// <summary>
        /// Gets the instance of RadTextBoxElement wrapped by this control. RadTextBoxElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadTextBox.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RadTextBoxElement TextBoxElement
        {
            get
            {
                return this.textBoxElement;
            }
        }

		/// <summary>
		/// Gets or sets a value indicating whether to show the bottom part of characters, clipped 
		/// due to font name or size particularities
		/// </summary>
		[Category(RadDesignCategory.AppearanceCategory)]
		[RadDescription("UseGenericBorderPaint", typeof(RadTextBoxElement))]
		[RadDefaultValue("UseGenericBorderPaint", typeof(RadTextBoxElement))]
		public bool UseGenericBorderPaint
		{
            get
            {
                return this.TextBoxElement.UseGenericBorderPaint;
            }
			set
			{
				this.TextBoxElement.UseGenericBorderPaint = value;
			}
		}

		#endregion

        /// <summary>
        /// Initializes textbox's children
        /// </summary>
        protected override internal void InitializeTextElement()
        {
            if (this.textElementInitialized)
            {
                return;
            }
            this.textBoxElement = new RadTextBoxElement();
            this.textBoxElement.ShowBorder = true;
            this.textElementInitialized = true;
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element == this.textBoxElement)
            {
                return true;
            }

            return base.ControlDefinesThemeForElement(element);
        }

        /// <summary>
        /// Creates the child items and initializes them
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            this.InitializeTextElement();
            this.RootElement.Children.Add(this.textBoxElement);
        }

		protected internal override void WireHostEvents()
		{
			base.WireHostEvents();

            this.textBoxItem.KeyDown += new KeyEventHandler(textBoxItem_KeyDown);
			this.textBoxItem.KeyUp += new KeyEventHandler(textBoxItem_KeyUp);
			this.textBoxItem.KeyPress += new KeyPressEventHandler(textBoxItem_KeyPress);
		}

		protected internal override void UnwireHostEvents()
		{
			base.UnwireHostEvents();

			this.textBoxItem.KeyDown -= new KeyEventHandler(textBoxItem_KeyDown);
			this.textBoxItem.KeyUp -= new KeyEventHandler(textBoxItem_KeyUp);
			this.textBoxItem.KeyPress -= new KeyPressEventHandler(textBoxItem_KeyPress);
        }

        #region Event handling

        private void textBoxItem_KeyDown(object sender, KeyEventArgs e)
		{
			base.CallBaseOnKeyDown(e);
		}

		private void textBoxItem_KeyUp(object sender, KeyEventArgs e)
		{
			base.CallBaseOnKeyUp(e);
		}

		private void textBoxItem_KeyPress(object sender, KeyPressEventArgs e)
		{
			base.CallBaseOnKeyPress(e);
        }

        #endregion

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadTextBoxAccessibleObject(this);
        }       
    }
}
