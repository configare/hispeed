using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Enumerations;

namespace Telerik.WinControls.UI
{
	///<exclude/>	
	/// <summary>
	/// Represents a checkmark element in a menu item.
	/// </summary>
	public class RadMenuCheckmark : RadCheckmark
	{
		public static RadProperty ShowAlwaysBorderProperty = RadProperty.Register("ShowAlwaysBorder", typeof(bool), typeof(RadMenuCheckmark),
			new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        /// <summary>Gets or sets value indicating whether border must be shown.</summary>
        public bool ShowAlwaysBorder
        {
            get
            {
                return (bool)this.GetValue(ShowAlwaysBorderProperty);
            }
            set
            {
                this.SetValue(ShowAlwaysBorderProperty, value);
                SetCheckState();
            }
        }
        
        public static RadProperty ShowAlwaysFillProperty = RadProperty.Register("ShowAlwaysFill", typeof(bool), typeof(RadMenuCheckmark),
					new RadElementPropertyMetadata(true, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

		/// <summary>Gets or sets value indicating whether background must be shown.</summary>
		public bool ShowAlwaysBackground
		{
			get
			{
				return (bool)this.GetValue(ShowAlwaysFillProperty);
			}
			set
			{
				this.SetValue(ShowAlwaysFillProperty, value);
				SetCheckState();
			}
		}
				
		protected override void SetCheckState()
		{
            if ((this.CheckState == ToggleState.On) || (this.CheckState == ToggleState.Indeterminate))
			{
				if (this.ShowAlwaysBorder)
				{
					this.Border.Visibility = ElementVisibility.Visible;
				}
				if (this.ShowAlwaysBackground)
				{
					this.Fill.Visibility = ElementVisibility.Visible;
				}

				if (this.CheckElement != null && (this.ImageElement == null || this.ImageElement.IsEmpty))
				{
					this.CheckElement.Visibility = ElementVisibility.Visible;
				}
			}
			else
			{
				this.Border.Visibility = ElementVisibility.Collapsed;
				this.Fill.Visibility = ElementVisibility.Collapsed;

				if (this.CheckElement != null)
					this.CheckElement.Visibility = ElementVisibility.Collapsed;
			}
		}		
	}
}
