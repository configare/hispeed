using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Design;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents the RadRadioButton control
	/// </summary>
    [System.ComponentModel.ToolboxItem(true)]
	[RadThemeDesignerData(typeof(RadRadioButtonStyleBuilderData))]
	[Description("Enables the user to select a single option from a group of choices when paired with other RadioButtons")]
	public class RadRadioButton : RadToggleButton
	{

        /// <summary>
        /// Gets the instance of RadRadioButtonElement wrapped by this control. RadRadioButtonElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadRadioButton.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new RadRadioButtonElement ButtonElement
		{
			get { return (RadRadioButtonElement)base.ButtonElement; }
		}


		/// <summary>
		/// Gets the default size of RadRadioButton
		/// </summary>
		protected override Size DefaultSize
		{
			get
			{
				return new Size(110, 18);
			}
		}

		/// <summary>Gets or sets a value indicating the alignment of the radio button.</summary>
        [RadPropertyDefaultValue("RadioCheckAlignment", typeof(RadRadioButtonElement))]
        [RadDescription("RadioCheckAlignment", typeof(RadRadioButtonElement))]
		public System.Drawing.ContentAlignment RadioCheckAlignment
		{
			get
			{
				return this.ButtonElement.RadioCheckAlignment;
			}
			set
			{
				this.ButtonElement.RadioCheckAlignment = value;
			}
		}

		/// <summary>
        /// Create main button element that is specific for RadRadioButton.
        /// </summary>
        /// <returns>The element that encapsulates the funtionality of RadRadioButton</returns>
        protected override RadButtonElement CreateButtonElement()
        {
            RadRadioButtonElement res = new RadRadioButtonElement();
            res.UseNewLayoutSystem = true;
            res.RadPropertyChanged += new RadPropertyChangedEventHandler(ButtonElement_RadPropertyChanged);
            res.PropertyChanged += new PropertyChangedEventHandler(ButtonElement_PropertyChanged);
            res.ToggleStateChanging += new StateChangingEventHandler(ButtonElement_ToggleStateChanging);
            res.ToggleStateChanged += new StateChangedEventHandler(ButtonElement_ToggleStateChanged);
            return res;
        }

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			this.Invalidate();
		}
        private void ButtonElement_RadPropertyChanged(object sender, RadPropertyChangedEventArgs e)
		{
			this.Invalidate();
		}

        private void ButtonElement_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.Invalidate();
            if (e.PropertyName == "IsChecked")
            {
                base.OnNotifyPropertyChanged("IsChecked");
            }
		}

		private void ButtonElement_ToggleStateChanging(object sender, StateChangingEventArgs args)
		{
			base.OnToggleStateChanging(args);
		}

		private void ButtonElement_ToggleStateChanged(object sender, StateChangedEventArgs args)
		{
			base.OnToggleStateChanged(args);
            base.OnNotifyPropertyChanged("IsChecked");
		}


        //p.p. 27.07.09 - commented the function
        //TODO must be revisied why rootElement.StretchHorizontally = false;
        //protected override void InitializeRootElement(RootRadElement rootElement)
        //{
        //    base.InitializeRootElement(rootElement);

        //    //rootElement.StretchHorizontally = false;
        //    // rootElement.StretchVertically = false;
        //}

        /// <summary>
        /// Creates the children items 
        /// </summary>
        /// <param name="parent"></param>
        protected override void CreateChildItems(RadElement parent)
        {
            // The base child items creation is customized by overriding CreateButtonElement()
            base.CreateChildItems(parent);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadRadioButtonAccessibleObject(this);
        }
	}
}
