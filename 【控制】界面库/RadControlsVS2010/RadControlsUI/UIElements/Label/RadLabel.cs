using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using Telerik.WinControls.Themes.Design;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using System.Security;
using System.Security.Permissions;
using Telerik.WinControls.Primitives;
using System.Drawing;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
    [RadThemeDesignerData(typeof(RadLabelThemeDesignerData))]
    [Description("Provides run-time information or descriptive text for a control")]
    [DefaultBindingProperty("Text"), DefaultEvent("Click"), DefaultProperty("Text")]
    [ToolboxItem(true)]
    public class RadLabel : RadControl
    {
        private bool tabStop = false;

        public RadLabel()
        {
            this.AutoSize = true;
            this.TabStop = false;

        }

        [DefaultValue(false)]
        public new bool TabStop
        {
            get
            {
                return this.tabStop;
            }
            set
            {
                this.tabStop = value;
                base.TabStop = this.tabStop;
            }
        }

        /// <summary>
        /// Gets or sets the position of text and image relative to each other. 
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint), Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the position of text and image relative to each other.")]
        [RadDefaultValue("TextImageRelation", typeof(RadLabelElement))]
        public TextImageRelation TextImageRelation
        {
            get
            {
                return this.labelElement.TextImageRelation;
            }
            set
            {
                this.labelElement.TextImageRelation = value;
            }
        }

        //true if the text should wrap to the available layout rectangle
        //otherwise, false. The default is true
        [RadPropertyDefaultValue("TextWrap", typeof(RadLabelElement)), Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        [DefaultValue(true)]
        public bool TextWrap
        {
            get
            {
                return this.labelElement.TextWrap;
            }
            set
            {
                this.labelElement.TextWrap = value;
            }
        }

        /// <summary>
        /// Gets or sets the alignment of image content on the drawing surface. 
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint),
        Category(RadDesignCategory.AppearanceCategory), Localizable(true),
        Description("Gets or sets the alignment of image content on the drawing surface.")]
        [RadDefaultValue("ImageAlignment", typeof(RadLabelElement))]
        public System.Drawing.ContentAlignment ImageAlignment
        {
            get
            {
                return this.labelElement.ImageAlignment;
            }
            set
            {
                this.labelElement.ImageAlignment = value;
            }
        }



        /// <commentsfrom cref="RadButtonItem.ImageKey" filter=""/>
        [RadDefaultValue("ImageKey", typeof(RadLabelElement)),
        Category(RadDesignCategory.AppearanceCategory),
       RadDescription("ImageKey", typeof(RadLabelElement)),
        RefreshProperties(RefreshProperties.All), Localizable(true),
        RelatedImageList("ImageList"),
        Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.RadImageKeyConverterString)]
        public virtual string ImageKey
        {
            get
            {
                return this.labelElement.ImageKey;
            }
            set
            {
                this.labelElement.ImageKey = value;
            }
        }

        /// <commentsfrom cref="RadLabelElement.ImageIndex" filter=""/>
        [RadDefaultValue("ImageIndex", typeof(RadLabelElement)),
        Category(RadDesignCategory.AppearanceCategory),
        RadDescription("ImageIndex", typeof(RadLabelElement)),
        RefreshProperties(RefreshProperties.All),
        RelatedImageList("ImageList"),
        Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString), Localizable(true)]
        public int ImageIndex
        {
            get
            {
                return this.labelElement.ImageIndex;
            }
            set
            {
                this.labelElement.ImageIndex = value;
            }
        }


        /// <commentsfrom cref="RadButtonItem.Image" filter=""/>        
        [RefreshProperties(RefreshProperties.All), Localizable(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [RadDescription("Image", typeof(RadLabelElement))]
        [RadDefaultValue("Image", typeof(RadLabelElement))]
        public Image Image
        {
            get
            {
                return this.labelElement.Image;
            }
            set
            {
                this.labelElement.Image = value;

                if (this.labelElement.Image != null)
                    this.ImageList = null;
            }
        }

        [DefaultValue(true)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        protected override void OnNotifyPropertyChanged(string propertyName)
        {
            if (propertyName == "AutoSize")
            {
                //SetChildrenAutoSize(this.RootElement);

                if (!this.AutoSize)
                {
                    this.RootElement.StretchHorizontally = true;
                    this.RootElement.StretchVertically = true;
                }
                else
                {
                    this.RootElement.StretchHorizontally = false;
                    this.RootElement.StretchVertically = false;
                }
            }
            else 
            if (propertyName == "Text")
            {
                this.OnTextChanged(EventArgs.Empty);
            }

            base.OnNotifyPropertyChanged(propertyName);
        }

        private RadLabelElement labelElement;

        /// <summary>
        /// Gets the instance of RadLabelElement wrapped by this control. RadLabelElement
        /// is the main element in the hierarchy tree and encapsulates the actual functionality of RadLabel.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadLabelElement LabelElement
        {
            get
            {
                return this.labelElement;
            }
        }

        /// <summary>
        /// Gets or sets the text associated with this item. 
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the text associated with this item."),
        Bindable(true),
        SettingsBindable(true),
        //Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))
        Editor("Telerik.WinControls.UI.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
        public override string Text
        {
            get
            {
                return this.labelElement.Text;
            }
            set
            {
                this.labelElement.Text = value;
                this.OnNotifyPropertyChanged("Text");
            }
        }


        /// <summary>
        /// Gets or sets the alignment of text content on the drawing surface. 
        /// </summary>
        [RefreshProperties(RefreshProperties.Repaint), Localizable(true),
        Category(RadDesignCategory.AppearanceCategory),
        Description("Gets or sets the alignment of text content on the drawing surface.")]
        [RadDefaultValue("TextAlignment", typeof(RadLabelElement))]
        public System.Drawing.ContentAlignment TextAlignment
        {
            get
            {
                return this.labelElement.TextAlignment;
            }
            set
            {
                this.labelElement.TextAlignment = value;
            }
        }

        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(false)]
        public bool BorderVisible
        {
            get
            {
                return this.labelElement.BorderVisible;
            }
            set
            {
                this.labelElement.BorderVisible = value;
            }
        }

        [UIPermission(SecurityAction.LinkDemand, Window = UIPermissionWindow.AllWindows)]
        protected override bool ProcessMnemonic(char charCode)
        {
            if (!this.UseMnemonic || !Control.IsMnemonic(charCode, this.Text))
            {
                return false;
            }
            if (!this.Enabled || !this.Visible)
            {
                return false;
            }
            Control parentInternal = this.Parent;
            if (parentInternal != null)
            {
                try
                {
                    if (parentInternal.SelectNextControl(this, true, false, true, false) &&
                        !parentInternal.ContainsFocus)
                    {
                        parentInternal.Focus();
                    }
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
            }
            return true;
        }

        private void SetChildrenAutoSize(RadElement element)
        {
            element.AutoSize = true;
            foreach (RadElement child in element.Children)
            {
                SetChildrenAutoSize(child);
            }
        }


        /// <commentsfrom cref="TextPrimitive.UseMnemonic" filter=""/>
        [RadDefaultValue("UseMnemonic", typeof(TextPrimitive))]
        [Description("If true, the first character preceded by an ampersand (&&) will be used as the label's mnemonic key")]
        [Category(RadDesignCategory.AppearanceCategory), Localizable(true)]
        //[RefreshProperties(RefreshProperties.Repaint)]
        //[NotifyParentProperty(true)]
        public bool UseMnemonic
        {
            get
            {
                return this.labelElement.UseMnemonic;
            }
            set
            {
                this.labelElement.UseMnemonic = value;
            }
        }

        protected override void CreateChildItems(RadElement parent)
        {
            if (this.labelElement == null)
                this.labelElement = new RadLabelElement();

            this.RootElement.Children.Add(labelElement);
            base.CreateChildItems(parent);
        }

        protected override void InitializeRootElement(RootRadElement rootElement)
        {
            base.InitializeRootElement(rootElement);
            rootElement.StretchHorizontally = false;
            rootElement.StretchVertically = false;
        }

        protected override RootRadElement CreateRootElement()
        {
            return new RadLabelRootElement();
        }

        public class RadLabelRootElement : RootRadElement
        {
            protected internal override bool? ShouldSerializeProperty(PropertyDescriptor property)
            {
                if (property.Name == "StretchHorizontally" ||
                    property.Name == "StretchVertically")
                    return false;
                bool? res = base.ShouldSerializeProperty(property);
                if (property.Name == "AutoSizeMode")
                    res = false;
                return res;
            }
        }
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new RadLabelAccessibleObject(this);
        }

        [ComVisible(true)]
        public class RadLabelAccessibleObject : Control.ControlAccessibleObject
        {
            // Methods
            public RadLabelAccessibleObject(RadLabel owner)
                : base(owner)
            {
            }

            // Properties
            public override AccessibleRole Role
            {
                get
                {                    
                    return AccessibleRole.StaticText;
                }
            }
        }
    }
}
