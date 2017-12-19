using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Telerik.WinControls.Design;
using Telerik.WinControls.Themes.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a groupbox. The group box major purpose is to define a radio buttons group. The RadGroupBox does not support scrolling.  
    /// The control is highly customizable using themes.    
    /// </summary>
    [TelerikToolboxCategory(ToolboxGroupStrings.ContainersGroup)]
    [ToolboxItem(true)]
    [RadThemeDesignerData(typeof(RadGroupBoxThemeDesignerData))]
    [Designer(DesignerConsts.RadGroupBoxControlDesignerString)]
    public class RadGroupBox : RadControl
    {
        #region Fields
        private RadGroupBoxElement groupBoxElement; 
        #endregion

        #region Constructors
        /// <summary>
        /// Parameterless contstructor.
        /// </summary>
        public RadGroupBox()
        {
            this.SetStyle(ControlStyles.Selectable, false);
            base.SetStyle(ControlStyles.ContainerControl, true);
            this.AccessibleRole = AccessibleRole.Grouping;
        } 
        #endregion

        #region Properties
        /// <summary>
        /// Gets the groupbox element.
        /// </summary>
        [Browsable(false)]
        public RadGroupBoxElement GroupBoxElement
        {
            get
            {
                return this.groupBoxElement;
            }
        }

        protected override Size DefaultSize
        {
            get
            {
                return new Size(200, 100);
            }
        }

        /// <summary>
        /// Gets or sets the groupbox style - Standard or Office.
        /// </summary>
        [DefaultValue(RadGroupBoxStyle.Standard)]
        [Category("Appearance")]
        public RadGroupBoxStyle GroupBoxStyle
        {
            get
            {
                return this.groupBoxElement.GroupBoxStyle;
            }

            set
            {
                this.groupBoxElement.GroupBoxStyle = value;
            }
        }

        /// <summary>
        /// Gets or sets the header position - Top, Right, Bottom, Left
        /// </summary>
        [DefaultValue(HeaderPosition.Top)]
        [Category("Appearance")]
        public HeaderPosition HeaderPosition
        {
            get
            {
                return this.groupBoxElement.HeaderPosition;
            }

            set
            {
                this.groupBoxElement.HeaderPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the header alignment - Near, Center, Far.
        /// </summary>
        [DefaultValue(HeaderAlignment.Near)]
        [Category("Appearance")]
        public HeaderAlignment HeaderAlignment
        {
            get
            {
                return this.groupBoxElement.HeaderAlignment;
            }

            set
            {
                this.groupBoxElement.HeaderAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the header margin.
        /// </summary>
        [Category("Appearance")]
        public Padding HeaderMargin
        {
            get
            {
                return this.groupBoxElement.HeaderMargin;
            }

            set
            {
                this.groupBoxElement.HeaderMargin = value;
            }
        }

        /// <summary>
        /// Gets or sets footer visibility.
        /// </summary>
        [DefaultValue(ElementVisibility.Collapsed)]
        [Category("Appearance")]
        public ElementVisibility FooterVisibility
        {
            get
            {
                return this.groupBoxElement.FooterVisibile;
            }

            set
            {
                this.groupBoxElement.FooterVisibile = value;
            }
        }

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        [Category("Appearance")]
        [Localizable(true)]
        public string HeaderText
        {
            get
            {
                return this.groupBoxElement.HeaderText;
            }

            set
            {
                this.groupBoxElement.HeaderText = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer text.
        /// </summary>
        [DefaultValue("Footer Text")]
        [Category("Appearance")]
        public string FooterText
        {
            get
            {
                return this.groupBoxElement.FooterText;
            }

            set
            {
                this.groupBoxElement.FooterText = value;
            }
        }

        /// <summary>
        /// Gets or sets the header image.
        /// </summary>
        [DefaultValue(null)]
        [Category("Appearance")]
        public Image HeaderImage
        {
            get
            {
                return this.groupBoxElement.HeaderImage;
            }

            set
            {
                this.groupBoxElement.HeaderImage = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer image.
        /// </summary>
        [DefaultValue(null)]
        [Category("Appearance")]
        public Image FooterImage
        {
            get
            {
                return this.groupBoxElement.FooterImage;
            }

            set
            {
                this.groupBoxElement.FooterImage = value;
            }
        }

        public override System.Windows.Forms.RightToLeft RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }

            set
            {
                base.RightToLeft = value;

                if (base.RightToLeft == RightToLeft.Yes)
                {
                    this.groupBoxElement.RightToLeft = true;
                }
                else
                {
                    this.groupBoxElement.RightToLeft = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        [Category("Appearance")]
        [Localizable(true)]
        public override string Text
        {
            get
            {
                return this.HeaderText;
            }

            set
            {
                this.HeaderText = value;
                OnTextChanged(new EventArgs());
            }
        }

        public override System.Drawing.Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }

            set
            {
                base.BackgroundImage = value;
            }
        }

        public override System.Windows.Forms.ContextMenu ContextMenu
        {
            get
            {
                return base.ContextMenu;
            }

            set
            {
                base.ContextMenu = value;
            }
        }

        public override System.Drawing.Font Font
        {
            get
            {
                return base.Font;
            }

            set
            {
                base.Font = value;
            }
        }

        public override System.Windows.Forms.ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }

            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        /// <summary>
        /// Gets or sets the header image key.
        /// </summary>
        // [DefaultValue("")]
        // [Category("Appearance")]

        [RadDefaultValue("HeaderImageKey", typeof(RadGroupBoxElement)),
        Category(RadDesignCategory.AppearanceCategory),
        RadDescription("HeaderImageKey", typeof(RadGroupBoxElement)),
        RefreshProperties(RefreshProperties.All), Localizable(true),
        RelatedImageList("ImageList"),
        Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.RadImageKeyConverterString)]
        public string HeaderImageKey
        {
            get
            {
                return this.groupBoxElement.HeaderImageKey;
            }

            set
            {
                this.groupBoxElement.HeaderImageKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the header image index.
        /// </summary>
        [RadDefaultValue("HeaderImageIndex", typeof(RadGroupBoxElement)),
         Category(RadDesignCategory.AppearanceCategory),
         RadDescription("HeaderImageIndex", typeof(RadGroupBoxElement)),
         RefreshProperties(RefreshProperties.All),
         RelatedImageList("ImageList"),
         Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
         TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString), Localizable(true)]

        public int HeaderImageIndex
        {
            get
            {
                return this.groupBoxElement.HeaderImageIndex;
            }

            set
            {
                this.groupBoxElement.HeaderImageIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer image key.
        /// </summary>
        [RadDefaultValue("FooterImageKey", typeof(RadGroupBoxElement)),
        Category(RadDesignCategory.AppearanceCategory),
        RadDescription("FooterImageKey", typeof(RadGroupBoxElement)),
        RefreshProperties(RefreshProperties.All), Localizable(true),
        RelatedImageList("ImageList"),
        Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.RadImageKeyConverterString)]
        public string FooterImageKey
        {
            get
            {
                return this.groupBoxElement.FooterImageKey;
            }

            set
            {
                this.groupBoxElement.FooterImageKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer image index.
        /// </summary>
        [RadDefaultValue("FooterImageIndex", typeof(RadGroupBoxElement)),
         Category(RadDesignCategory.AppearanceCategory),
         RadDescription("FooterImageIndex", typeof(RadGroupBoxElement)),
         RefreshProperties(RefreshProperties.All),
         RelatedImageList("ImageList"),
         Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
         TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString), Localizable(true)]

        public int FooterImageIndex
        {
            get
            {
                return this.groupBoxElement.FooterImageIndex;
            }

            set
            {
                this.groupBoxElement.FooterImageIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the header text image relation.
        /// </summary>
        [DefaultValue(System.Windows.Forms.TextImageRelation.Overlay)]
        [Category("Appearance")]
        public TextImageRelation HeaderTextImageRelation
        {
            get
            {
                return this.groupBoxElement.HeaderTextImageRelation;
            }

            set
            {
                this.groupBoxElement.HeaderTextImageRelation = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer text image relation.
        /// </summary>
        [DefaultValue(System.Windows.Forms.TextImageRelation.Overlay)]
        [Category("Appearance")]
        public TextImageRelation FooterTextImageRelation
        {
            get
            {
                return this.groupBoxElement.FooterTextImageRelation;
            }

            set
            {
                this.groupBoxElement.FooterTextImageRelation = value;
            }
        }

        /// <summary>
        /// Gets or sets the header text alignment.
        /// </summary>
        [DefaultValue(System.Drawing.ContentAlignment.MiddleLeft)]
        [Category("Appearance")]
        public ContentAlignment HeaderTextAlignment
        {
            get
            {
                return this.groupBoxElement.HeaderTextAlignment;
            }

            set
            {
                this.groupBoxElement.HeaderTextAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer text alignment.
        /// </summary>
        [DefaultValue(System.Drawing.ContentAlignment.MiddleLeft)]
        [Category("Appearance")]
        public ContentAlignment FooterTextAlignment
        {
            get
            {
                return this.groupBoxElement.FooterTextAlignment;
            }

            set
            {
                this.groupBoxElement.FooterTextAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the header image alignment.
        /// </summary>
        [DefaultValue(System.Drawing.ContentAlignment.MiddleLeft)]
        [Category("Appearance")]
        public ContentAlignment HeaderImageAlignment
        {
            get
            {
                return this.groupBoxElement.HeaderImageAlignment;
            }

            set
            {
                this.groupBoxElement.HeaderImageAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer image alignment.
        /// </summary>
        [DefaultValue(System.Drawing.ContentAlignment.MiddleLeft)]
        [Category("Appearance")]
        public ContentAlignment FooterImageAlignment
        {
            get
            {
                return this.groupBoxElement.FooterImageAlignment;
            }

            set
            {
                this.groupBoxElement.FooterImageAlignment = value;
            }
        }
        #endregion

        #region Methods
        protected override void CreateChildItems(RadElement parent)
        {
            this.groupBoxElement = new RadGroupBoxElement();
            this.RootElement.Children.Add(this.groupBoxElement);
            base.CreateChildItems(parent);
            this.FooterText = "Footer Text";

            //apply default padding to the control, so that Docked controls do not overlap the header
            this.Padding = new Padding(2, 18, 2, 2);
            this.RootElement.Padding = new Padding(2, 18, 2, 2);
        } 
        #endregion
    }
}


