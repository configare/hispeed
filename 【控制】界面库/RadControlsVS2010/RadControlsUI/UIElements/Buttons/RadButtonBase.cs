using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Security.Permissions;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    ///     Represents a base button control. The button control serves as a
    ///     <see cref="RadButtonElement">RadButtonElement Class</see> wrapper. All logic and
    ///     presentation features are implemented in a parallel hierarchy of objects. For this
    ///     reason, <see cref="RadButtonElement">RadButtonElement Class</see> may be nested in
    ///     any other telerik control, item, or element.
    /// </summary>
    [ToolboxItem(false)]
    [Description("Responds to user clicks.")]
    [DefaultBindingProperty("Text"), DefaultEvent("Click"), DefaultProperty("Text")]
    public class RadButtonBase : RadControl
    {
        #region Static Fields

        private static MethodInfo ValidateActiveControlMethod = null;

        #endregion

        #region Fields

        // Fields
        private RadButtonElement buttonElement;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the RadButtonBase class.
        /// </summary>
        public RadButtonBase()
        {
            base.TabStop = true;
            base.SetStyle(ControlStyles.Selectable, true);
            base.SetStyle(ControlStyles.StandardDoubleClick, false);
        }

        protected override void CreateChildItems(RadElement parent)
        {
            if (this.buttonElement == null)
                this.buttonElement = CreateButtonElement();

            this.buttonElement.Click += new EventHandler(ButtonElement_Click);
            this.RootElement.Children.Add(buttonElement);
            this.buttonElement.BindProperty(RadButtonElement.AutoSizeModeProperty, this.RootElement, RootRadElement.AutoSizeModeProperty, PropertyBindingOptions.OneWay);

            base.CreateChildItems(parent);
        }


        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.AutoSizeMode = RadAutoSizeMode.FitToAvailableSize;
            //we want to display focus cues (if OS allows for such) for buttons
            this.ElementTree.ComponentTreeHandler.Behavior.AllowShowFocusCues = true;
        }

        public override bool ControlDefinesThemeForElement(RadElement element)
        {
            if (element is RadButtonElement)
                return true;

            return base.ControlDefinesThemeForElement(element);
        }


        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadButtonAccessibleObject(this);
        }

        /// <summary>
        /// Override this method to create custom main element. By default the main element is an instance of
        /// RadButtonElement.
        /// </summary>
        /// <returns>Instance of the one-and-only child of the root element of RadButton.</returns>
        protected virtual RadButtonElement CreateButtonElement()
        {
            RadButtonElement res = new RadButtonElement();
            res.UseNewLayoutSystem = true;
            return res;
        }

        #endregion

        #region Events

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler DoubleClick
        {
            add
            {
                base.DoubleClick += value;
            }
            remove
            {
                base.DoubleClick -= value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseDoubleClick
        {
            add
            {
                base.MouseDoubleClick += value;
            }

            remove
            {
                base.MouseDoubleClick -= value;
            }
        }

        #endregion

        #region Properties

        protected override Size DefaultSize
        {
            get
            {
                return new Size(130, 24);
            }
        }

        /// <summary>
        /// Includes the trailing space at the end of each line. By default the boundary rectangle returned by the Overload:System.Drawing.Graphics.MeasureString method excludes the space at the end of each line. Set this flag to include that space in measurement.
        /// </summary>
        [DefaultValue(true),
        Description("Includes the trailing space at the end of each line. By default the boundary rectangle returned by the Overload:System.Drawing.Graphics.MeasureString method excludes the space at the end of each line. Set this flag to include that space in measurement."),
        Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        public bool MeasureTrailingSpaces
        {
            get
            {
                return this.ButtonElement.MeasureTrailingSpaces;
            }
            set
            {
                this.ButtonElement.MeasureTrailingSpaces = value;
            }
        }

        /// <summary>
        /// Gets the instance of RadButtonElement wrapped by this control. RadButtonElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadButton.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual RadButtonElement ButtonElement
        {
            get
            {
                return this.buttonElement;
            }
        }

        /// <summary>
        /// Gets or sets the text associated with this item. 
        /// </summary>
        [Category(RadDesignCategory.BehaviorCategory),
        Description("Gets or sets the text associated with this item."),
        Bindable(true), Localizable(true),
        SettingsBindable(true),
            //  Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))
        Editor("Telerik.WinControls.UI.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
        public override string Text
        {
            get
            {
                return this.buttonElement.Text;
            }
            set
            {
                base.Text = value;
                this.buttonElement.Text = value;
            }
        }

        /// <commentsfrom cref="RadButtonItem.Image" filter=""/>        
        [Category(RadDesignCategory.AppearanceCategory),
        RadDescription("Image", typeof(RadButtonElement)),
        RefreshProperties(RefreshProperties.All), Localizable(true)]
        public Image Image
        {
            get
            {
                return this.buttonElement.Image;
            }
            set
            {
                Image image = this.buttonElement.Image;
                if (value == image)
                {
                    return;
                }

                //image changed, reset ImageList and ImageIndex
                this.ImageList = null;
                this.ImageIndex = -1;

                this.buttonElement.Image = value;
            }
        }

        /// <commentsfrom cref="RadButtonItem.ImageIndex" filter=""/>
        [RadDefaultValue("ImageIndex", typeof(RadButtonElement)),
        Category(RadDesignCategory.AppearanceCategory),
        RadDescription("ImageIndex", typeof(RadButtonElement)),
        RefreshProperties(RefreshProperties.All),
        RelatedImageList("ImageList"),
        Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString), Localizable(true)]
        public int ImageIndex
        {
            get
            {
                return this.buttonElement.ImageIndex;
            }
            set
            {
                this.buttonElement.ImageIndex = value;
            }
        }

        /// <commentsfrom cref="RadButtonItem.ImageKey" filter=""/>
        [RadDefaultValue("ImageKey", typeof(RadButtonElement)),
        Category(RadDesignCategory.AppearanceCategory),
        RadDescription("ImageKey", typeof(RadButtonElement)),
        RefreshProperties(RefreshProperties.All), Localizable(true),
        RelatedImageList("ImageList"),
        Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.RadImageKeyConverterString)]
        public virtual string ImageKey
        {
            get
            {
                return this.buttonElement.ImageKey;
            }
            set
            {
                this.buttonElement.ImageKey = value;
            }
        }

        /// <summary>
        /// Specifies the options for display of image and text primitives in the element.
        /// </summary>
        [Browsable(true), Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Specifies the options for display of image and text primitives in the element.")]
        [RadDefaultValue("DisplayStyle", typeof(RadButtonElement))]
        public DisplayStyle DisplayStyle
        {
            get
            {
                return this.buttonElement.DisplayStyle;
            }
            set
            {
                this.buttonElement.DisplayStyle = value;
            }
        }

        //true if the text should wrap to the available layout rectangle
        //otherwise, false. The default is true
        [RadPropertyDefaultValue("TextWrap", typeof(TextPrimitive))]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("True if the text should wrap to the available layout rectangle otherwise, false.")]
        public bool TextWrap
        {
            get
            {
                return this.buttonElement.TextWrap;
            }
            set
            {
                this.buttonElement.TextWrap = value;
            }
        }


        /// <summary>
        /// Gets or sets the position of text and image relative to each other. 
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint), Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the position of text and image relative to each other.")]
        [RadDefaultValue("TextImageRelation", typeof(RadButtonElement))]
        public TextImageRelation TextImageRelation
        {
            get
            {
                return this.buttonElement.TextImageRelation;
            }
            set
            {
                this.buttonElement.TextImageRelation = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of image content on the drawing surface. 
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint),
        Category(RadDesignCategory.AppearanceCategory), Localizable(true),
        Description("Gets or sets the alignment of image content on the drawing surface.")]
        [RadDefaultValue("ImageAlignment", typeof(RadButtonElement))]
        public System.Drawing.ContentAlignment ImageAlignment
        {
            get
            {
                return this.buttonElement.ImageAlignment;
            }
            set
            {
                this.buttonElement.ImageAlignment = value;
            }
        }

        protected virtual ContentAlignment DefaultTextAlignment
        {
            get
            {
                RadPropertyValue value = this.ButtonElement.GetPropertyValue(RadButtonItem.TextAlignmentProperty);
                object res = value.DefaultValueOverride;
                if (res == RadProperty.UnsetValue)
                {
                    return (ContentAlignment)RadButtonItem.TextAlignmentProperty.GetMetadata(this.ButtonElement).DefaultValue;
                }
                else
                {
                    return (ContentAlignment)res;
                }
            }
        }

        /// <summary>
        /// Gets or sets the alignment of text content on the drawing surface. 
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint), Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the alignment of text content on the drawing surface.")]
        public System.Drawing.ContentAlignment TextAlignment
        {
            get
            {
                return this.buttonElement.TextAlignment;
            }
            set
            {
                this.buttonElement.TextAlignment = value;
            }
        }


        /// <summary>
        /// Determines whether the button can be clicked by using mnemonic characters.
        /// </summary>
        [Category("Appearance"),
        Description("Determines whether the button can be clicked by using mnemonic characters."),
        DefaultValue(true)]
        public bool UseMnemonic
        {
            get
            {
                return this.buttonElement.TextElement.UseMnemonic;
            }
            set
            {
                this.buttonElement.TextElement.UseMnemonic = value;
            }
        }

        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {

                this.RootElement.StretchHorizontally = !value;
                this.RootElement.StretchVertically = !value;

                this.RootElement.SaveCurrentStretchModeAsDefault();

                base.AutoSize = value;
            }
        }

        /// <summary>
        /// Indicates focus cues display, when available, based on the corresponding control type and the current UI state.
        /// </summary>
        [Browsable(true), Category("Accessibility")]
        [DefaultValue(true)]
        [Description("Indicates focus cues display, when available, based on the corresponding control type and the current UI state.")]
        public override bool AllowShowFocusCues
        {
            get
            {
                return base.AllowShowFocusCues;
            }
            set
            {
                base.AllowShowFocusCues = value;
            }
        }

        #endregion

        #region Methods

        private bool ShouldSerializeTextAlignment()
        {
            return this.TextAlignment != this.DefaultTextAlignment;
        }

        private void ResetTextAlignment()
        {
            this.TextAlignment = this.DefaultTextAlignment;
        }

        public bool ShouldSerializeImage()
        {
            return this.Image != null && this.ImageList == null;
        }

        public void ResetImage()
        {
            this.Image = null;
        }

        public void PerformClick()
        {
            //Invoking validation of the currently active control in the parent container
            bool validatedControlAllowsFocusChange = false;

            if (ValidateActiveControlMethod == null)
            {
                ValidateActiveControlMethod = typeof(Control).GetMethod("ValidateActiveControl",
                                                              BindingFlags.Instance | BindingFlags.NonPublic);
            }

            bool result = (bool)ValidateActiveControlMethod.Invoke(this, new object[] { validatedControlAllowsFocusChange });
            if ((result || validatedControlAllowsFocusChange) && this.Enabled)
            {
                ((IButtonControl)this.ButtonElement).PerformClick();
            }
        }

        #endregion

        #region Event Handlers

        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessMnemonic(char charCode)
        {
            if ((this.UseMnemonic && TelerikHelper.CanProcessMnemonic(this)) && Control.IsMnemonic(charCode, this.Text))
            {
                this.PerformClick();
                return true;
            }
            return false;
        }

        private void ButtonElement_Click(object sender, EventArgs e)
        {
            base.OnClick(e);
        }

        protected override void OnClick(EventArgs e)
        {
            //override base OnClick and let the button element raise the event
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            this.ButtonElement.Focus();
        }

        #endregion
    }

}
