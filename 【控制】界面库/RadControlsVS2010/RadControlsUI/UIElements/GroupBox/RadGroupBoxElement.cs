using System;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{

    public class RadGroupBoxElement : LightVisualElement
    {
        #region Fields
        private GroupBoxHeader header;
        private GroupBoxContent content;
        private GroupBoxFooter footer;
        #endregion

        #region Constructors
        /// <summary>
        /// Static constructor - initializes themes.
        /// </summary>
        static RadGroupBoxElement()
        {
            new Themes.ControlDefault.ControlDefault_Telerik_WinControls_UI_RadGroupBox().DeserializeTheme();

            
        } 
        #endregion

        #region Properties
        public override bool RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
                this.Children[1].RightToLeft = value;
            }
        }

        /// <summary>
        /// Gets or sets the header margin.
        /// </summary>
        public Padding HeaderMargin
        {
            get
            {
                return this.header.Margin;
            }

            set
            {
                this.header.Margin = value;
            }
        }

        /// <summary>
        /// Gets or sets the header text.
        /// </summary>
        [Editor("Telerik.RadMarkupEditor.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
        public string HeaderText
        {
            get
            {
                return this.header.TextPrimitive.Text;
            }

            set
            {
                this.header.TextPrimitive.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer text.
        /// </summary>
        [Editor("Telerik.RadMarkupEditor.TextOrHtmlSelector, Telerik.WinControls.RadMarkupEditor, Version=" + VersionNumber.Number + ", Culture=neutral, PublicKeyToken=5bb2a467cbec794e", typeof(UITypeEditor))]
        public string FooterText
        {
            get
            {
                return this.footer.TextPrimitive.Text;
            }

            set
            {
                this.footer.TextPrimitive.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the header image.
        /// </summary>
        public Image HeaderImage
        {
            get
            {
                return this.header.ImagePrimitive.Image;
            }

            set
            {
                this.header.ImagePrimitive.Image = value;
            }
        }
        /// <summary>
        /// Gets or sets the footer image.
        /// </summary>
        [DefaultValue(null)]
        public Image FooterImage
        {
            get
            {
                return this.footer.ImagePrimitive.Image;
            }

            set
            {
                this.footer.ImagePrimitive.Image = value;
            }
        }

        /// <summary>
        /// Gets or sets the header text image relation.
        /// </summary>
        public TextImageRelation HeaderTextImageRelation
        {
            get
            {
                return this.header.ImageAndTextLayout.TextImageRelation;
            }

            set
            {
                this.header.ImageAndTextLayout.TextImageRelation = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer text image relation.
        /// </summary>
        public TextImageRelation FooterTextImageRelation
        {
            get
            {
                return this.footer.ImageAndTextLayout.TextImageRelation;
            }

            set
            {
                this.footer.ImageAndTextLayout.TextImageRelation = value;
            }
        }

        /// <summary>
        /// Gets or sets the header text alignment.
        /// </summary>
        public ContentAlignment HeaderTextAlignment
        {
            get
            {
                return this.header.ImageAndTextLayout.TextAlignment;
            }

            set
            {
                this.header.ImageAndTextLayout.TextAlignment = value;

            }
        }

        /// <summary>
        /// Gets or sets the footer text alignment.
        /// </summary>
        public ContentAlignment FooterTextAlignment
        {
            get
            {
                return this.footer.ImageAndTextLayout.TextAlignment;
            }

            set
            {
                this.footer.ImageAndTextLayout.TextAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the header image alignment.
        /// </summary>
        public ContentAlignment HeaderImageAlignment
        {
            get
            {
                return this.header.ImageAndTextLayout.ImageAlignment;
            }

            set
            {
                this.header.ImageAndTextLayout.ImageAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the footer image alignment.
        /// </summary>
        public ContentAlignment FooterImageAlignment
        {
            get
            {
                return this.footer.ImageAndTextLayout.ImageAlignment;
            }

            set
            {
                this.footer.ImageAndTextLayout.ImageAlignment = value;
            }
        }

        /// <summary>
        /// Gets or sets the header image key.
        /// </summary>
        public string HeaderImageKey
        {
            get
            {
                return this.header.ImagePrimitive.ImageKey;
            }

            set
            {
                this.header.ImagePrimitive.ImageKey = value;
                this.header.ImagePrimitive.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the header image index.
        /// </summary>
        public int HeaderImageIndex
        {
            get
            {
                return this.header.ImagePrimitive.ImageIndex;
            }
            set
            {
                this.header.ImagePrimitive.ImageIndex = value;
                this.header.ImagePrimitive.InvalidateMeasure();

            }
        }

        /// <summary>
        /// Gets or sets the footer image key.
        /// </summary>
        public string FooterImageKey
        {
            get
            {
                return this.footer.ImagePrimitive.ImageKey;
            }

            set
            {
                this.footer.ImagePrimitive.ImageKey = value;
                this.footer.ImagePrimitive.InvalidateMeasure();
            }
        }

        /// <summary>
        /// Gets or sets the footer image index.
        /// </summary>
        public int FooterImageIndex
        {
            get
            {
                return this.footer.ImagePrimitive.ImageIndex;
            }

            set
            {
                this.footer.ImagePrimitive.ImageIndex = value;
                this.footer.ImagePrimitive.InvalidateMeasure();
            }
        }

        public GroupBoxHeader Header
        {
            get
            {
                return header;
            }

        }

        public GroupBoxContent Content
        {
            get
            {
                return content;
            }
        }

        public GroupBoxFooter Footer
        {
            get
            {
                return footer;
            }
        }

        public HeaderAlignment HeaderAlignment
        {
            get
            {
                return (HeaderAlignment)this.GetValue(HeaderAlignmentProperty);
            }

            set
            {
                if ((HeaderAlignment)this.GetValue(HeaderAlignmentProperty) != value)
                {
                    this.SetValue(HeaderAlignmentProperty, value);
                }
            }
        }


        [Browsable(false)]
        public int InvalidateMeasureInMainLayout
        {
            get
            {
                return (int)this.GetValue(InvalidateMeasureInMainLayoutProperty);
            }

            set
            {
                this.SetValue(InvalidateMeasureInMainLayoutProperty, value);
            }
        }


        public HeaderPosition HeaderPosition
        {
            get
            {
                return this.Header.HeaderPosition;
            }

            set
            {
                this.Header.HeaderPosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the group box style - Standard, or Office.  
        /// </summary>
        public RadGroupBoxStyle GroupBoxStyle
        {
            get
            {
                return header.GroupBoxStyle;
            }

            set
            {
                if (header.GroupBoxStyle != value)
                {
                    header.GroupBoxStyle = value;
                }
            }
        }

        public ElementVisibility FooterVisibile
        {
            get
            {
                return this.footer.Visibility;
            }

            set
            {
                if (this.footer.Visibility != value)
                {
                    if (value == ElementVisibility.Hidden)
                    {
                        this.footer.Visibility = ElementVisibility.Collapsed;
                    }
                    else
                    {
                        this.footer.Visibility = value;
                    }

                }

            }
        } 
        #endregion

        #region RadProperties
        public static RadProperty HeaderAlignmentProperty = RadProperty.Register("HeaderAlignment", typeof(HeaderAlignment), typeof(RadGroupBoxElement),
          new RadElementPropertyMetadata(HeaderAlignment.Near, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsArrange));

        [Browsable(false)]
        public static RadProperty InvalidateMeasureInMainLayoutProperty = RadProperty.Register("InvalidateMeasureInMainLayout", typeof(int), typeof(RadGroupBoxElement),
          new RadElementPropertyMetadata(0, ElementPropertyOptions.InvalidatesLayout | ElementPropertyOptions.AffectsMeasure | ElementPropertyOptions.AffectsArrange));

        #endregion

        #region Methods
        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.Class = "GroupBoxElement";
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.header = new GroupBoxHeader();
            this.content = new GroupBoxContent();
            this.footer = new GroupBoxFooter();

            this.Children.Add(this.content);
            this.Children.Add(this.header);
            this.Children.Add(this.footer);
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            base.MeasureOverride(availableSize);
            SizeF desiredSize = SizeF.Empty;

            if (this.Footer.Visibility == ElementVisibility.Visible)
            {
                desiredSize.Height += Footer.DesiredSize.Height;
            }

            if (HeaderPosition == HeaderPosition.Top || HeaderPosition == HeaderPosition.Bottom)
            {
                desiredSize.Height += Header.DesiredSize.Height;
            }
            else
            {
                desiredSize.Width += Header.DesiredSize.Width;
            }

            desiredSize.Height += Content.DesiredSize.Height;
            desiredSize.Width += Content.DesiredSize.Width;

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            Padding headerMargin = new Padding(0, 0, 0, 0);

            float VPositionCoef = 0.0f;
            float HPositionCoef = 0.0f;

            if (GroupBoxStyle == RadGroupBoxStyle.Standard)
            {
                if (this.HeaderPosition == HeaderPosition.Top || this.HeaderPosition == HeaderPosition.Bottom)
                {
                    VPositionCoef = 0.5f;

                    switch (HeaderAlignment)
                    {
                        case HeaderAlignment.Center:
                            {
                                HPositionCoef = 0.5f;
                                break;
                            }
                        case HeaderAlignment.Near:
                            {
                                HPositionCoef = 0f;
                                headerMargin = new Padding(10, 0, 0, 0);
                                break;
                            }
                        case HeaderAlignment.Far:
                            {
                                HPositionCoef = 1f;
                                headerMargin = new Padding(0, 0, 10, 0);
                                break;
                            }
                    }
                }
                else
                {
                    HPositionCoef = 0.5f;

                    switch (HeaderAlignment)
                    {
                        case HeaderAlignment.Center:
                            {
                                VPositionCoef = 0.5f;
                                break;
                            }
                        case HeaderAlignment.Near:
                            {
                                VPositionCoef = 0f;
                                headerMargin = new Padding(0, 10, 0, 0);
                                break;
                            }
                        case HeaderAlignment.Far:
                            {
                                VPositionCoef = 1f;
                                headerMargin = new Padding(0, 0, 0, 10);
                                break;
                            }
                    }
                }
            }
            else
            {
                if (this.HeaderPosition == HeaderPosition.Top || this.HeaderPosition == HeaderPosition.Bottom)
                {
                    VPositionCoef = 1f;
                    HPositionCoef = 0.0f;
                }
                else
                {
                    HPositionCoef = 1f;
                    VPositionCoef = 0f;
                }
            }

            SizeF contentAndHeaderFinalSize = SizeF.Empty;
            contentAndHeaderFinalSize = new SizeF(finalSize.Width, finalSize.Height - footer.DesiredSize.Height);

            foreach (RadElement element in this.Children)
            {
                if (element == this.Footer)
                {
                    RectangleF footerPosition = new RectangleF(0, finalSize.Height - footer.DesiredSize.Height, finalSize.Width, footer.DesiredSize.Height);
                    element.Arrange(footerPosition);
                }
                else
                    if (element == this.Header)
                    {
                        float headerX = 0, headerY = 0, headerW = header.DesiredSize.Width, headerH = header.DesiredSize.Height;

                        if (this.GroupBoxStyle == RadGroupBoxStyle.Standard)
                        {
                            switch (this.HeaderPosition)
                            {
                                case HeaderPosition.Top:
                                    {
                                        headerX = (contentAndHeaderFinalSize.Width - element.DesiredSize.Width) * HPositionCoef;
                                        break;
                                    }
                                case HeaderPosition.Left:
                                    {
                                        headerY = (contentAndHeaderFinalSize.Height - element.DesiredSize.Height) * VPositionCoef; break;
                                    }
                                case HeaderPosition.Right:
                                    {
                                        headerX = contentAndHeaderFinalSize.Width - element.DesiredSize.Width;
                                        headerY = (contentAndHeaderFinalSize.Height - element.DesiredSize.Height) * VPositionCoef;
                                        break;
                                    }
                                case HeaderPosition.Bottom:
                                    {
                                        headerY = contentAndHeaderFinalSize.Height - element.DesiredSize.Height;
                                        headerX = (contentAndHeaderFinalSize.Width - element.DesiredSize.Width) * HPositionCoef;
                                        break;
                                    }
                            }
                        }
                        else
                        {
                            switch (this.HeaderPosition)
                            {
                                case HeaderPosition.Top:
                                    {
                                        headerW = finalSize.Width;
                                        break;
                                    }
                                case HeaderPosition.Left:
                                    {
                                        headerH = (footer.Visibility != ElementVisibility.Collapsed) ? finalSize.Height - this.footer.Size.Height : finalSize.Height;
                                        break;
                                    }
                                case HeaderPosition.Right:
                                    {
                                        headerH = (footer.Visibility != ElementVisibility.Collapsed) ? finalSize.Height - this.footer.Size.Height : finalSize.Height;
                                        headerX = finalSize.Width - header.DesiredSize.Width;
                                        break;
                                    }
                                case HeaderPosition.Bottom:
                                    {
                                        headerW = finalSize.Width;
                                        headerY = contentAndHeaderFinalSize.Height - element.DesiredSize.Height; headerX = (contentAndHeaderFinalSize.Width - element.DesiredSize.Width) * HPositionCoef;
                                        break;
                                    }
                            }
                        }

                        RectangleF headerPosition;
                        headerPosition = new RectangleF(headerX + headerMargin.Left - headerMargin.Right, headerY + headerMargin.Top - headerMargin.Bottom, headerW, headerH);
                        element.Arrange(headerPosition);
                    }
                    else if (element == this.Content)
                    {
                        float contentX = 0, contentY = 0, contentW = contentAndHeaderFinalSize.Width, contentH = contentAndHeaderFinalSize.Height;

                        if (this.GroupBoxStyle == RadGroupBoxStyle.Standard)
                        {
                            switch (this.HeaderPosition)
                            {
                                case HeaderPosition.Top: { contentY += VPositionCoef * Header.DesiredSize.Height; contentH -= VPositionCoef * Header.DesiredSize.Height; break; }
                                case HeaderPosition.Left: { contentX += HPositionCoef * Header.DesiredSize.Width; contentW -= HPositionCoef * Header.DesiredSize.Width; break; }
                                case HeaderPosition.Right: { contentW -= HPositionCoef * Header.DesiredSize.Width; break; }
                                case HeaderPosition.Bottom: { contentH -= VPositionCoef * Header.DesiredSize.Height; break; }
                            }
                        }
                        else
                        {
                            switch (this.HeaderPosition)
                            {
                                case HeaderPosition.Top:
                                    {
                                        contentY += VPositionCoef * Header.DesiredSize.Height; contentH -= VPositionCoef * Header.DesiredSize.Height;
                                        break;
                                    }
                                case HeaderPosition.Left:
                                    {
                                        contentX += HPositionCoef * header.DesiredSize.Width; contentW -= HPositionCoef * header.DesiredSize.Width;
                                        break;
                                    }
                                case HeaderPosition.Right:
                                    {
                                        contentW -= HPositionCoef * header.DesiredSize.Width;
                                        break;
                                    }
                                case HeaderPosition.Bottom:
                                    {
                                        contentH -= VPositionCoef * Header.DesiredSize.Height;
                                        break;
                                    }
                            }
                        }

                        RectangleF contentPosition = new RectangleF((float)Math.Floor(contentX), (float)Math.Floor(contentY),
                            (float)Math.Floor(contentW), (float)Math.Floor(contentH));
                        element.Arrange(contentPosition);
                    }
                    else
                    {
                        element.Arrange(new RectangleF(Point.Empty, finalSize));
                    }
            }

            return finalSize;
        }              
        #endregion
    }
}