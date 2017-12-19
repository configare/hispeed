using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Enumerations;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Drawing.Design;

namespace Telerik.WinControls.UI
{
	///<exclude/>
	/// <summary>Represents checkmark.</summary>
	public class RadRadiomark : LightVisualElement
	{
		private ImagePrimitive imageElement;
		private RadioPrimitive checkElement;

		/// <summary>
		/// Registers the CheckState dependency property
		/// </summary>
		public static RadProperty CheckStateProperty = RadProperty.Register("CheckState", typeof(ToggleState), typeof(RadRadiomark),
			new RadElementPropertyMetadata(ToggleState.Off, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout |
			ElementPropertyOptions.AffectsDisplay)
			);

		/// <summary>
		/// Registers the IsImage dependency property
		/// </summary>
		public static RadProperty IsImageProperty = RadProperty.Register("IsImageElement", typeof(bool), typeof(RadRadiomark),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		/// <summary>
		/// Registers the IsCheckMark dependency property
		/// </summary>
		public static RadProperty IsCheckmarkProperty = RadProperty.Register("IsCheckElement", typeof(bool), typeof(RadRadiomark),
			new RadElementPropertyMetadata(false, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		/// <summary>
		/// Gets an instance of the check element
		/// </summary>
		internal protected RadioPrimitive CheckElement
		{
			get { return this.checkElement; }
		}

		/// <summary>
		/// Gets an instance of Image element
		/// </summary>
		internal protected ImagePrimitive ImageElement
		{
			get { return this.imageElement; }
		}

		/// <summary>Gets or sets value indicating RadRadiomark checkstate.</summary>
		public ToggleState CheckState
		{
			get
			{
				return (ToggleState)this.GetValue(CheckStateProperty);
			}
			set
			{
				this.SetValue(CheckStateProperty, value);
			}
		}

		protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Shape = new EllipseShape();
        }

		private void AsureImageAndCheckElements()
		{
			if (this.imageElement == null || this.checkElement == null)
			{
				foreach (RadElement child in this.GetChildren(ChildrenListOptions.Normal))
				{
					if ((bool)child.GetValue(IsImageProperty))
					{
						this.imageElement = child as ImagePrimitive;
					}
					else if ((bool)child.GetValue(IsCheckmarkProperty))
					{
						this.checkElement = child as RadioPrimitive;
					}
				}
			}
			this.SetCheckState();
		}

		/// <summary>
		/// Initializes the newly added children if needed. 
		/// </summary>
		/// <param name="child"></param>
		/// <param name="changeOperation"></param>
		protected override void OnChildrenChanged(RadElement child, ItemsChangeOperation changeOperation)
		{
			this.AsureImageAndCheckElements();
			base.OnChildrenChanged(child, changeOperation);
		}

		/// <summary>
		/// handles the properties behavior when a property value is changed.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == CheckStateProperty)
			{
				SetCheckState();
			}
			if (e.Property == RadRadiomark.ImageProperty)
			{
				this.imageElement.Image = (Image)e.NewValue;
				SetCheckState();
			}
			if (e.Property == RadRadiomark.ImageIndexProperty)
			{
				this.imageElement.ImageIndex = (int)e.NewValue;
				SetCheckState();
			}
			if (e.Property == RadRadiomark.ImageKeyProperty)
			{
				this.imageElement.ImageKey = (string)e.NewValue;
				SetCheckState();
			}
		}

        public override Size GetPreferredSizeCore(Size proposedSize)
        {
            return new Size(13, 13);
        }
		
		/// <summary>
		/// Sets the toggle state of the RadioMark
		/// </summary>
		protected virtual void SetCheckState()
		{
			if ((this.CheckState == ToggleState.On) || (this.CheckState == ToggleState.Indeterminate))
			{
				if (this.imageElement != null && !this.imageElement.IsEmpty)
				{
					this.imageElement.Visibility = ElementVisibility.Visible;
				}
				else if (checkElement != null && (this.imageElement == null || this.imageElement.IsEmpty))
				{
					this.checkElement.Visibility = ElementVisibility.Visible;
				}

			}
			else
			{
				if (this.imageElement != null && !this.imageElement.IsEmpty)
				{
					this.imageElement.Visibility = ElementVisibility.Hidden;
				}
				if (this.checkElement != null)
					this.checkElement.Visibility = ElementVisibility.Hidden;
			}
		}

        internal override bool VsbVisible
        {
            get
            {
                return false;
            }
        }
	}
}