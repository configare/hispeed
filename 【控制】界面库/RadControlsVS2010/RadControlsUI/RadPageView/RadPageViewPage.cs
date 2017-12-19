using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.ComponentModel.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Represents a page in a RadPageView instance.
    /// </summary>
    [ToolboxItem(false)]
    [Designer(DesignerConsts.RadPageViewPageDesignerString)]
    public class RadPageViewPage : Panel
    {
        #region Fields

        private RadPageView owner;
        private RadPageViewItem item;
        private Image image;
        private Color localBackColor;

        private bool isContentVisible = false;
        private string title;
        private string description;
        private string toolTipText;
        private ContentAlignment textAlignment;
        private ContentAlignment imageAlignment;
        private TextImageRelation textImageRelation;

        #endregion

        #region Constructor/Initializers

        public RadPageViewPage()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);

            //page is hidden by default
            this.Visible = false;

            this.textImageRelation = TextImageRelation.ImageBeforeText;
            this.imageAlignment = ContentAlignment.MiddleLeft;
            this.textAlignment = ContentAlignment.MiddleLeft;
        }

        protected internal virtual void Attach(RadPageView view)
        {
            this.owner = view;
        }

        protected internal virtual void Detach()
        {
            this.owner = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Detach();
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the title of the Page. Title appears in the Header area of the owning RadPageView.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(TextImageRelation.ImageBeforeText)]
        [Description("Gets or sets the title of the Page. Title appears in the Header area of the owning RadPageView.")]
        public TextImageRelation TextImageRelation
        {
            get
            {
                return this.textImageRelation;
            }
            set
            {
                if (this.textImageRelation == value)
                {
                    return;
                }

                this.textImageRelation = value;
                this.OnTextImageRelationChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the title of the Page. Title appears in the Header area of the owning RadPageView.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        [Description("Gets or sets the title of the Page. Title appears in the Header area of the owning RadPageView.")]
        public ContentAlignment ImageAlignment
        {
            get
            {
                return this.imageAlignment;
            }
            set
            {
                if (this.imageAlignment == value)
                {
                    return;
                }

                this.imageAlignment = value;
                this.OnImageAlignmentChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the title of the Page. Title appears in the Header area of the owning RadPageView.
        /// </summary>
        [Category(RadDesignCategory.AppearanceCategory)]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        [Description("Gets or sets the title of the Page. Title appears in the Header area of the owning RadPageView.")]
        public ContentAlignment TextAlignment
        {
            get
            {
                return this.textAlignment;
            }
            set
            {
                if (this.textAlignment == value)
                {
                    return;
                }

                this.textAlignment = value;
                this.OnTextAlignmentChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the title of the Page. Title appears in the Header area of the owning RadPageView.
        /// </summary>
        [Description("Gets or sets the title of the Page. Title appears in the Header area of the owning RadPageView.")]
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                if (this.title == value)
                {
                    return;
                }

                this.title = value;
                this.OnTitleChanged(EventArgs.Empty);
            }
        }

        private bool ShouldSerializeTitle()
        {
            return !string.IsNullOrEmpty(this.title);
        }

        /// <summary>
        /// Gets or sets the title of the Page. Title appears in the Header area of the owning RadPageView.
        /// </summary>
        [Description("Gets or sets the title of the Page. Title appears in the Header area of the owning RadPageView.")]
        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                if (this.description == value)
                {
                    return;
                }

                this.description = value;
                this.OnDescriptionChanged(EventArgs.Empty);
            }
        }

        private bool ShouldSerializeDescription()
        {
            return !string.IsNullOrEmpty(this.title);
        }

        /// <summary>
        /// Gets or sets the length of the current <see cref="RadPageViewPage"/>. The length
        /// represents the fixed amount of space the page will take when the layout of the control is performed.
        /// Note: This property is only functional when the <see cref="RadPageView"/> control
        /// is in ExplorerBar mode and its content size mode is set to FixedLength.
        /// </summary>
        [Description("Gets or sets the length of the current page. The length represents the fixed amount of space in pixels the page will take when the layout"
            +" of the control is performed. Note: This property is only functional when the control is in ExplorerBar mode and its content size mode is set to FixedLength.")]
        public int PageLength
        {
            get
            {
                if (this.item != null)
                {
                    return this.item.PageLength;
                }
                return -1;
            }
            set
            {
                if (this.item != null)
                {
                    this.item.PageLength = value;
                }
            }
        }

        private bool ShouldSerializePageLength()
        {
            if (this.owner == null)
                return false;

            RadPageViewExplorerBarElement element = this.owner.ViewElement as RadPageViewExplorerBarElement;

            if (element == null || this.item == null)
                return false;

            if (element.ContentSizeMode == ExplorerBarContentSizeMode.FixedLength)
            {
                if (this.item.PageLength != RadPageViewExplorerBarItem.DEFAULT_PAGE_LENGTH)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Gets or sets a boolean value determining whether the content of the current
        /// <see cref="RadPageViewPage"/> is visible. This property is only functional in the
        /// when the <see cref="RadPageView"/> control is in ExplorerBar view mode.
        /// </summary>
        [Browsable(false)]
        public bool IsContentVisible
        {
            get
            {
                return this.isContentVisible;
            }
            set
            {
                if (value != this.isContentVisible)
                {
                    this.isContentVisible = value;
                    this.OnIsContentVisibleChanged();
                }
            }
        }

        protected virtual void OnIsContentVisibleChanged()
        {
            if (this.item != null)
            {
                this.item.IsContentVisible = this.isContentVisible;
            }
        }

        private bool ShouldSerializeIsContentVisible()
        {
            if (this.owner == null)
                return false;

            if (this.owner.ViewElement is RadPageViewExplorerBarElement)
            {
                return this.isContentVisible;
            }

            return false;
        }

        /// <summary>
        /// Gets or sets the tooltip to be displayed when the item hovers page's associated item.
        /// </summary>
        [Description("Gets or sets the tooltip to be displayed when the item hovers page's associated item.")]
        public string ToolTipText
        {
            get
            {
                return this.toolTipText;
            }
            set
            {
                if (this.toolTipText == value)
                {
                    return;
                }

                this.toolTipText = value;
                this.OnToolTipTextChanged(EventArgs.Empty);
            }
        }

        private bool ShouldSerializeToolTipText()
        {
            return !string.IsNullOrEmpty(this.toolTipText);
        }

        public override Color BackColor
        {
            get
            {
                if (this.localBackColor != Color.Empty)
                {
                    return this.localBackColor;
                }

                if (this.owner != null)
                {
                    if (this.owner.PageBackColor != Color.Empty)
                    {
                        return this.owner.PageBackColor;
                    }

                    RadPageViewContentAreaElement contentAreaElement = this.owner.ViewElement.GetContentAreaForItem(this.item);

                    if (contentAreaElement != null &&
                        contentAreaElement.ElementState == ElementState.Loaded)
                    {
                        return contentAreaElement.BackColor;
                    }
                }

                return base.BackColor;
            }
            set
            {
                this.localBackColor = value;
                base.BackColor = value;
            }
        }

        private bool ShouldSerializeBackColor()
        {
            return this.localBackColor != Color.Empty;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = DockStyle.None;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new int TabIndex
        {
            get
            {
                return base.TabIndex;
            }
            set
            {
                base.TabIndex = value;
            }
        }

        /// <summary>
        /// Gets or sets the text to be displayed in the associated item.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
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

        /// <summary>
        /// Gets the RadPageView instance that owns this page.
        /// </summary>
        [Browsable(false)]
        public RadPageView Owner
        {
            get
            {
                return this.owner;
            }
        }

        /// <summary>
        /// Gets the RadPageViewItem instance which is the UI representation of this page.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadPageViewItem Item
        {
            get
            {
                return this.item;
            }
            internal set
            {
                this.item = value;
            }
        }

        /// <summary>
        /// Gets or sets the image to be displayed by the associated RadPageViewItem instance.
        /// </summary>
        [DefaultValue(null)]
        [Description("Gets or sets the image to be displayed by the associated RadPageViewItem instance.")]
        public Image Image
        {
            get
            {
                return this.image;
            }
            set
            {
                if (this.image == value)
                {
                    return;
                }

                this.image = value;
                this.OnImageChanged(EventArgs.Empty);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override AnchorStyles Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                base.Anchor = value;
            }
        }

        #endregion

        #region UI Synchronization

        public override string ToString()
        {
            return "RadPageViewPage [" + this.Text + "]";
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (this.item != null)
            {
                this.item.Enabled = this.Enabled;
            }
        }

        protected internal virtual void OnPageBackColorChanged(EventArgs e)
        {
            this.OnBackColorChanged(EventArgs.Empty);
        }

        protected virtual void OnTextImageRelationChanged(EventArgs e)
        {
            if (this.item != null)
            {
                this.item.TextImageRelation = this.textImageRelation;
            }
        }

        protected virtual void OnImageAlignmentChanged(EventArgs e)
        {
            if (this.item != null)
            {
                this.item.ImageAlignment = this.imageAlignment;
            }
        }

        protected virtual void OnTextAlignmentChanged(EventArgs e)
        {
            if (this.item != null)
            {
                this.item.TextAlignment = this.textAlignment;
            }
        }

        protected virtual void OnDescriptionChanged(EventArgs e)
        {
            if (this.item != null)
            {
                this.item.Description = this.description;
            }
        }

        protected virtual void OnToolTipTextChanged(EventArgs e)
        {
            if (this.item != null)
            {
                this.item.ToolTipText = this.toolTipText;
            }
        }

        protected virtual void OnTitleChanged(EventArgs e)
        {
            if (this.item != null)
            {
                this.item.Title = this.title;
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            if (this.item != null)
            {
                this.item.Text = this.Text;
            }
        }


        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
        }


        protected virtual void OnImageChanged(EventArgs e)
        {
            if (this.item != null)
            {
                this.item.Image = this.image;
            }
        }

        #endregion
    }
}
