using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>Represents a combo box item. The RadComboBox items may contain 
    /// images.
    /// </summary>
    public class RadComboBoxItem : RadListBoxItem
    {
		#region Constructors
        /// <summary>Initializes a new instance of the RadComboBoxItem class.</summary>
        public RadComboBoxItem(): base()
        {
        }

        /// <summary>Initializes a new instance of the RadComboBoxItem class using the displayed string.</summary>
        public RadComboBoxItem(string text)
            : base(text)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RadComboBoxItem class using the displayed text and mouse event
        /// handler for the OnClick event.
        /// </summary>
        public RadComboBoxItem(string text, MouseEventHandler OnClick)
            : base(text, OnClick)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RadComboBoxItem class using the displayed text, and the 
        /// value of the item.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="value"></param>
        public RadComboBoxItem(string text, object value)
            : base(text, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the RadComboBoxItem class using the displayed dataItem, text, and the 
        /// value of the item.
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="text"></param>
        /// <param name="value"></param>
        public RadComboBoxItem(object dataItem, string text, object value)
            : base(dataItem, text, value)
        {
        }
		
		/// <summary>
		/// Initializes a new instance of the RadListBoxItem class using item's text, 
		/// it's description and value.
		/// </summary>
		public RadComboBoxItem(string text, string descriptionText, object value)
			: base(text, descriptionText, value)
		{
		}
		/// <summary>
		/// Initializes a new instance of the RadListBoxItem class using item's text, 
		/// it's description and an onClick handler.
		/// </summary>
		public RadComboBoxItem(string text, string descriptionText, MouseEventHandler OnClick)
			: base(text, descriptionText, OnClick)
		{
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.BitState[IsFocusableStateKey] = false;
            this.ThemeRole = "RadListBoxItem";
        }
		#endregion

        private RadComboBoxElement ownerElement;

        /// <summary>
        /// Gets or sets the combobox owner of the item. 
        /// </summary>
		[Obsolete("This property is obsolete. Please use the OwnerElement property.")]
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadComboBoxElement OwnerComboElement
        {
            get 
            { 
                return this.ownerElement; 
            }
            set 
            { 
                this.ownerElement = value; 
            }
        }


        /// <summary>
        /// Gets or sets the combobox owner of the item. 
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new RadComboBoxElement OwnerElement
        {
            get
            {
                return ownerElement;
            }
            set
            {
                ownerElement = value;
            }
        }

        /// <summary>
        /// Gets or sets the image list index value of the image displayed on the button control.
        /// </summary>
        [RelatedImageList("OwnerElement.ElementTree.Control.ImageList")]
        public override int ImageIndex
        {
            get
            {
                return base.ImageIndex;
            }
            set
            {
                base.ImageIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the key accessor for the image in the ImageList.
        /// </summary>
        [RelatedImageList("OwnerElement.ElementTree.Control.ImageList")]
        public override string ImageKey
        {
            get
            {
                return base.ImageKey;
            }
            set
            {
                base.ImageKey = value;
            }
        }

        [Editor(DesignerConsts.MultilineStringEditorString, typeof(UITypeEditor))]
        [DefaultValue("")]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (this.OwnerElement != null)
            {
                this.OwnerElement.ComboPopupForm.SetActiveItem(this);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            if (this.OwnerElement != null)
            {
                if (this.OwnerElement != null && this.OwnerElement.SelectedItem == this)
                {
                    this.OwnerElement.Text = this.Text;
                }
            }
        }
    }
}
