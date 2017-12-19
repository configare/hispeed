using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.Drawing.Design;
using System.ComponentModel;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.Design;
using VisualStyleElement = System.Windows.Forms.VisualStyles.VisualStyleElement;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.UI
{
	/// <summary>
	///     Represents a radio button element. The <see cref="RadRadioButton">RadRadioButton</see>
	///     class is a simple wrapper for the RadRadioButtonElement class. The
	///     <see cref="RadRadioButton">RadRadioButton</see> acts to transfer events to and from its
	///     corresponding RadRadioButtonElement instance. The RadRadioButtonElement which is
	///     essentially the <see cref="RadRadioButton">RadRadioButton</see> control may be nested in
	///     other telerik controls.
	/// </summary>
	public class RadRadioButtonElement : RadToggleButtonElement
	{
		private RadRadiomark checkMarkPrimitive;

		/// <summary>
		/// Registers the RadioCheckAlignment dependency property
		/// </summary>
		public static RadProperty RadioCheckAlignmentProperty = RadProperty.Register("RadioCheckAlignment", typeof(ContentAlignment), typeof(RadRadioButtonElement),
			new RadElementPropertyMetadata(ContentAlignment.MiddleLeft, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsLayout |
			ElementPropertyOptions.AffectsDisplay)
			);

        /// <summary>Gets or sets a value indicating the alignment of the radio-mark according to the text of the button.</summary>
        [Description("Gets or sets a value indicating the alignment of the radio-mark according to the text of the button.")]      
		public ContentAlignment RadioCheckAlignment
		{
			get
			{
				return (ContentAlignment)this.GetValue(RadioCheckAlignmentProperty);
			}
			set
			{
				this.SetValue(RadioCheckAlignmentProperty, value);
			}
		}


		static RadRadioButtonElement()
		{
			new Themes.ControlDefault.RadioButton().DeserializeTheme();
		}

		protected override void OnToggleStateChanged(StateChangedEventArgs e)
		{
			if ((this.ElementTree != null) && (this.ElementTree.Control.Parent != null))
			{
				if (this.ElementTree.Control is RadRadioButton)
				{
					for (int i = 0; i < this.ElementTree.Control.Parent.Controls.Count; i++)
					{
						RadControl control = this.ElementTree.Control.Parent.Controls[i] as RadControl;


						if (control != null && control is RadRadioButton)
						{
							RadRadioButtonElement radioElement = control.RootElement.Children[0] as RadRadioButtonElement;

							if (radioElement != null)
							{
								if ((radioElement != this) && (this.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On))
								{
									radioElement.ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off;
								}
							}
						}
					}
					base.OnToggleStateChanged(e);
					return;
				}
			} 
			
			if (this.Parent != null)
			{
				foreach (RadElement element in this.Parent.Children)
				{
					if (element as RadRadioButtonElement != null)
					{
						if ((element != this) && (this.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On))
						{
							(element as RadRadioButtonElement).ToggleState = Telerik.WinControls.Enumerations.ToggleState.Off;
						}
					}

				}
			}

			base.OnToggleStateChanged(e);
			
		}

	
		/// <summary>
		/// Fires te Click event and handles the toggle logic 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnClick(EventArgs e)
		{
			if (this.ToggleState == Telerik.WinControls.Enumerations.ToggleState.Off)
			{
				base.OnClick(e);
			}
		}

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.TextAlignment = ContentAlignment.MiddleLeft;
        }

		/// <summary>
		/// initializes and adds the child elements
		/// </summary>
		protected override void CreateChildElements()
		{
			this.fillPrimitive = new FillPrimitive();
			this.fillPrimitive.Class = "radioButtonFill";
			this.fillPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

			this.borderPrimitive = new BorderPrimitive();
			this.borderPrimitive.Class = "radioButtonBorder";

			this.textPrimitive = new TextPrimitive();
			this.textPrimitive.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
			this.textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadItem.TextProperty, PropertyBindingOptions.TwoWay);

			this.imagePrimitive = new ImagePrimitive();
			this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
			this.imagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadCheckBoxElement.ImageIndexProperty, PropertyBindingOptions.TwoWay);
			this.imagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadCheckBoxElement.ImageProperty, PropertyBindingOptions.TwoWay);
			this.imagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadCheckBoxElement.ImageKeyProperty, PropertyBindingOptions.TwoWay);
			this.imagePrimitive.Class = "radioButtonLayoutImagePrimitive";

			this.layoutPanel = new ImageAndTextLayoutPanel();
            this.layoutPanel.UseNewLayoutSystem = this.UseNewLayoutSystem;
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.DisplayStyleProperty, this, RadRadioButtonElement.DisplayStyleProperty, PropertyBindingOptions.OneWay);
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.ImageAlignmentProperty, this, RadRadioButtonElement.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextAlignmentProperty, this, RadRadioButtonElement.TextAlignmentProperty, PropertyBindingOptions.OneWay);
			this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextImageRelationProperty, this, RadRadioButtonElement.TextImageRelationProperty, PropertyBindingOptions.OneWay);
			this.layoutPanel.SetValue(CheckBoxLayoutPanel.IsBodyProperty, true);
			this.layoutPanel.Children.Add(this.textPrimitive);
			this.layoutPanel.Children.Add(this.imagePrimitive);

            RadioPrimitive radioPrimitive = new RadioPrimitive();
			radioPrimitive.SetValue(RadRadiomark.IsCheckmarkProperty, true);
			radioPrimitive.Class = "radioButtonPrimitive";

			ImagePrimitive radioImagePrimitive = new ImagePrimitive();
			radioImagePrimitive.SetValue(RadRadiomark.IsImageProperty, true);
			radioImagePrimitive.Class = "radioButtonImagePrimitive";

            this.checkMarkPrimitive = new RadRadiomark();
            this.checkMarkPrimitive.SetValue(CheckBoxLayoutPanel.IsCheckmarkProperty, true);
            this.checkMarkPrimitive.BindProperty(RadRadiomark.CheckStateProperty, this, RadCheckBoxElement.ToggleStateProperty, PropertyBindingOptions.OneWay);
            this.checkMarkPrimitive.Children.Add(radioPrimitive);
            this.checkMarkPrimitive.Children.Add(radioImagePrimitive);
            this.checkMarkPrimitive.NotifyParentOnMouseInput = true;
            this.checkMarkPrimitive.ShouldHandleMouseInput = false;

			CheckBoxLayoutPanel radioButtonPanel = new CheckBoxLayoutPanel();
			this.textPrimitive.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
			radioButtonPanel.BindProperty(CheckBoxLayoutPanel.AutoSizeModeProperty, this, RadRadioButtonElement.AutoSizeModeProperty, PropertyBindingOptions.OneWay);
			radioButtonPanel.BindProperty(CheckBoxLayoutPanel.CheckAlignmentProperty, this, RadRadioButtonElement.RadioCheckAlignmentProperty, PropertyBindingOptions.OneWay);
			radioButtonPanel.Children.Add(layoutPanel);
            radioButtonPanel.Children.Add(this.checkMarkPrimitive);

			this.Children.Add(this.fillPrimitive);
			this.Children.Add(radioButtonPanel);
			this.Children.Add(this.borderPrimitive);
		}

        protected override Rectangle GetFocusRect()
        {
            if (textPrimitive != null)
            {
                return textPrimitive.ControlBoundingRectangle;
            }

            return Rectangle.Empty;
        }

        #region ISupportSystemSkin

        protected override bool ShouldPaintChild(RadElement element)
        {
            if (this.checkMarkPrimitive != null && this.paintSystemSkin != null)
            {
                this.checkMarkPrimitive.SetBitState(ShouldPaintChildrenStateKey, this.paintSystemSkin == false);
                this.checkMarkPrimitive.ShouldPaint = this.paintSystemSkin == false;
            }

            if (this.paintSystemSkin == true)
            {
                return element != this.fillPrimitive && element != this.borderPrimitive;
            }

            return base.ShouldPaintChild(element);
        }

        protected override Rectangle GetSystemSkinPaintBounds()
        {
            if (checkMarkPrimitive != null)
            {
                return checkMarkPrimitive.ControlBoundingRectangle;
            }

            return base.GetSystemSkinPaintBounds();
        }

        public override VisualStyleElement GetVistaVisualStyle()
        {
            return this.GetXPVisualStyle();
        }

        public override VisualStyleElement GetXPVisualStyle()
        {
            VisualStyleElement element = null;
            ToggleState state = this.ToggleState;

            if (!Enabled)
            {
                element = state == ToggleState.On ? VisualStyleElement.Button.RadioButton.CheckedDisabled : 
                                                    VisualStyleElement.Button.RadioButton.UncheckedDisabled;
            }
            else
            {
                if (IsMouseDown)
                {
                    if (IsMouseOver)
                    {
                        element = state == ToggleState.On ? VisualStyleElement.Button.RadioButton.CheckedPressed :
                                                        VisualStyleElement.Button.RadioButton.UncheckedPressed;
                    }
                    else
                    {
                        element = state == ToggleState.On ? VisualStyleElement.Button.RadioButton.CheckedHot :
                                                        VisualStyleElement.Button.RadioButton.UncheckedHot;
                    }
                }
                else if (IsMouseOver)
                {
                    element = state == ToggleState.On ? VisualStyleElement.Button.RadioButton.CheckedHot :
                                                    VisualStyleElement.Button.RadioButton.UncheckedHot;
                }
                else
                {
                    element = state == ToggleState.On ? VisualStyleElement.Button.RadioButton.CheckedNormal :
                                                    VisualStyleElement.Button.RadioButton.UncheckedNormal;
                }
            }

            return element;
        }

        protected override void InitializeSystemSkinPaint()
        {
            base.InitializeSystemSkinPaint();

            //update the forecolor for the text
            this.textPrimitive.ForeColor = SystemSkinManager.Instance.Renderer.GetColor(System.Windows.Forms.VisualStyles.ColorProperty.TextColor);
        }

        #endregion
	}
}
