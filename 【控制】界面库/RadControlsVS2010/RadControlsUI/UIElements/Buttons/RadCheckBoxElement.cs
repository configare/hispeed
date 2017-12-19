using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Primitives;
using System.Drawing;
using Telerik.WinControls.Design;
using Telerik.WinControls.Enumerations;
using System.ComponentModel;
using VisualStyleElement = System.Windows.Forms.VisualStyles.VisualStyleElement;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a check box element. The <see cref="RadCheckBox">RadCheckBox</see>
    ///     class is a simple wrapper for the RadCheckBoxElement class. The
    ///     <see cref="RadCheckBox">RadCheckBox</see> acts to transfer events to and from its
    ///     corresponding RadCheckBoxElement instance. The radCheckBoxElement which is
    ///     essentially the <see cref="RadCheckBox">RadCheckBox</see> control may be nested in
    ///     other telerik controls.
    /// </summary>
	public class RadCheckBoxElement : RadToggleButtonElement
    {
        #region Dependency properties

        public static RadProperty CheckAlignmentProperty = RadProperty.Register("CheckAlignment", typeof(ContentAlignment), typeof(RadCheckBoxElement),
            new RadElementPropertyMetadata(ContentAlignment.MiddleLeft, ElementPropertyOptions.InvalidatesLayout |
                ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsDisplay));
        
        #endregion

        #region Fields

        protected RadCheckmark checkMarkPrimitive;

        #endregion

        #region Initialization

        static RadCheckBoxElement()
        {
            new Themes.ControlDefault.CheckBox().DeserializeTheme();
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.TextAlignment = ContentAlignment.MiddleLeft;
        }

        protected override void CreateChildElements()
        {
            this.fillPrimitive = new FillPrimitive();
            this.fillPrimitive.Class = "ButtonFill";
            this.fillPrimitive.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;

            this.borderPrimitive = new BorderPrimitive();
            this.borderPrimitive.Class = "ButtonBorder";

            this.textPrimitive = new TextPrimitive();
            this.textPrimitive.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
            this.textPrimitive.BindProperty(TextPrimitive.TextProperty, this, RadCheckBoxElement.TextProperty, PropertyBindingOptions.TwoWay);

            this.imagePrimitive = new ImagePrimitive();
            this.imagePrimitive.SetValue(ImageAndTextLayoutPanel.IsImagePrimitiveProperty, true);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageIndexProperty, this, RadCheckBoxElement.ImageIndexProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageProperty, this, RadCheckBoxElement.ImageProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.BindProperty(ImagePrimitive.ImageKeyProperty, this, RadCheckBoxElement.ImageKeyProperty, PropertyBindingOptions.TwoWay);
            this.imagePrimitive.Class = "RadCheckBoxImagePrimitive";

            this.layoutPanel = new ImageAndTextLayoutPanel();
            this.layoutPanel.UseNewLayoutSystem = this.UseNewLayoutSystem;
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.DisplayStyleProperty, this, RadCheckBoxElement.DisplayStyleProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.ImageAlignmentProperty, this, RadCheckBoxElement.ImageAlignmentProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextAlignmentProperty, this, RadCheckBoxElement.TextAlignmentProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.BindProperty(ImageAndTextLayoutPanel.TextImageRelationProperty, this, RadCheckBoxElement.TextImageRelationProperty, PropertyBindingOptions.OneWay);
            this.layoutPanel.SetValue(CheckBoxLayoutPanel.IsBodyProperty, true);

            this.layoutPanel.Children.Add(this.textPrimitive);
            this.layoutPanel.Children.Add(this.imagePrimitive);

            ImagePrimitive checkImagePrimitive = new ImagePrimitive();
            checkImagePrimitive.SetValue(RadCheckmark.IsImageProperty, true);
            checkImagePrimitive.Class = "RadCheckBoxCheckImage";

            this.checkMarkPrimitive = new RadCheckmark();
            this.checkMarkPrimitive.Class = "CheckMark";
            this.checkMarkPrimitive.SetValue(CheckBoxLayoutPanel.IsCheckmarkProperty, true);
            this.checkMarkPrimitive.BindProperty(RadCheckmark.CheckStateProperty, this, RadCheckBoxElement.ToggleStateProperty, PropertyBindingOptions.OneWay);
            this.checkMarkPrimitive.CheckElement.Class = "RadCheckBoxCheckPrimitive";
            this.checkMarkPrimitive.Children.Add(checkImagePrimitive);

            CheckBoxLayoutPanel checkBoxPanel = new CheckBoxLayoutPanel();
            this.textPrimitive.SetValue(ImageAndTextLayoutPanel.IsTextPrimitiveProperty, true);
            checkBoxPanel.BindProperty(CheckBoxLayoutPanel.AutoSizeModeProperty, this, RadCheckBoxElement.AutoSizeModeProperty, PropertyBindingOptions.OneWay);
            checkBoxPanel.BindProperty(CheckBoxLayoutPanel.CheckAlignmentProperty, this, RadCheckBoxElement.CheckAlignmentProperty, PropertyBindingOptions.OneWay);
            checkBoxPanel.Children.Add(layoutPanel);
            checkBoxPanel.Children.Add(checkMarkPrimitive);

            this.Children.Add(this.fillPrimitive);
            this.Children.Add(checkBoxPanel);
            this.Children.Add(this.borderPrimitive);
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets a value indicating the alignment of the check box.</summary>
        [RadPropertyDefaultValue("CheckAlignment", typeof(RadCheckBoxElement))]
        [Description("Gets or sets a value indicating the alignment of the check box.")]
        public ContentAlignment CheckAlignment
        {
            get
            {
                return (ContentAlignment)this.GetValue(CheckAlignmentProperty);
            }
            set
            {
                this.SetValue(CheckAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets an instance of the <see cref="RadCheckmark"/>class
        /// that represents the check box part of the <see cref="RadCheckBoxElement"/>.
        /// </summary>
        [Browsable(false)]
        public RadCheckmark CheckMarkPrimitive
        {
            get
            {
                return this.checkMarkPrimitive;
            }
        }

        public bool Checked
        {
            get
            {
                return this.ToggleState == Telerik.WinControls.Enumerations.ToggleState.On;
            }
            set
            {
                if (this.Checked != value)
                {
                    this.ToggleState = (value) ?
                        Telerik.WinControls.Enumerations.ToggleState.On :
                        Telerik.WinControls.Enumerations.ToggleState.Off;
                }
            }
        }

        #endregion

        #region ISupportSystemSkin

        protected override bool ShouldPaintChild(RadElement element)
        {
            //checkmark primitive should be handled in different way
            if (this.checkMarkPrimitive != null && this.paintSystemSkin != null)
            {
                this.checkMarkPrimitive.CheckElement.ShouldPaint = this.paintSystemSkin == false;
                this.checkMarkPrimitive.Border.ShouldPaint = this.paintSystemSkin == false;
                this.checkMarkPrimitive.Fill.ShouldPaint = this.paintSystemSkin == false;
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

        public override VisualStyleElement GetXPVisualStyle()
        {
            VisualStyleElement element = null;
            ToggleState state = this.ToggleState;

            if (!Enabled)
            {
                switch (state)
                {
                    case ToggleState.Indeterminate:
                        element = VisualStyleElement.Button.CheckBox.MixedDisabled;
                        break;
                    case ToggleState.Off:
                        element = VisualStyleElement.Button.CheckBox.UncheckedDisabled;
                        break;
                    case ToggleState.On:
                        element = VisualStyleElement.Button.CheckBox.CheckedDisabled;
                        break;
                }
            }
            else
            {
                if (IsMouseDown)
                {
                    switch (state)
                    {
                        case ToggleState.Indeterminate:
                            element = IsMouseOver ? VisualStyleElement.Button.CheckBox.MixedPressed : VisualStyleElement.Button.CheckBox.MixedHot;
                            break;
                        case ToggleState.Off:
                            element = IsMouseOver ? VisualStyleElement.Button.CheckBox.UncheckedPressed : VisualStyleElement.Button.CheckBox.UncheckedHot;
                            break;
                        case ToggleState.On:
                            element = IsMouseOver ? VisualStyleElement.Button.CheckBox.CheckedPressed : VisualStyleElement.Button.CheckBox.CheckedHot;
                            break;
                    }
                }
                else if (IsMouseOver)
                {
                    switch (state)
                    {
                        case ToggleState.Indeterminate:
                            element = VisualStyleElement.Button.CheckBox.MixedHot;
                            break;
                        case ToggleState.Off:
                            element = VisualStyleElement.Button.CheckBox.UncheckedHot;
                            break;
                        case ToggleState.On:
                            element = VisualStyleElement.Button.CheckBox.CheckedHot;
                            break;
                    }
                }
                else
                {
                    switch (state)
                    {
                        case ToggleState.Indeterminate:
                            element = VisualStyleElement.Button.CheckBox.MixedNormal;
                            break;
                        case ToggleState.Off:
                            element = VisualStyleElement.Button.CheckBox.UncheckedNormal;
                            break;
                        case ToggleState.On:
                            element = VisualStyleElement.Button.CheckBox.CheckedNormal;
                            break;
                    }
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

        protected override Rectangle GetFocusRect()
        {
            if (textPrimitive != null)
            {
                return textPrimitive.ControlBoundingRectangle;
            }

            return Rectangle.Empty;
        }
	}
}
