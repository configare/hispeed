using System;
using Telerik.WinControls.Data;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.Design;
using Telerik.WinControls.Interfaces;
using System.Collections.Specialized;
using Telerik.WinControls.Enumerations;
using System.Drawing.Design;
using System.Windows.Forms; 

namespace Telerik.WinControls.UI
{
    [TypeConverter(typeof(ListViewDataItemTypeConverter))]
    public class ListViewDataItem :
    IDataItem,
    INotifyPropertyChanged,
    INotifyPropertyChangingEx,
    IDisposable
    {
        #region State

        protected const int IsSelectedState = 1;
        protected const int IsEnabledState = IsSelectedState << 1;
        protected const int IsVisibleState = IsEnabledState << 1;
        protected const int IsLastInRowState = IsVisibleState << 1;
        protected const int IsCurrentState = IsLastInRowState << 1;
        protected const int IsMeasureValidState = IsCurrentState << 1;
        protected const int MajorBitState = IsMeasureValidState;

        protected BitVector32 bitState = new BitVector32();

        #endregion

        #region Fields

        protected object dataBoundItem;
        protected RadListViewElement owner;
        private ListViewDataItemStyle style;
        private Image image = null;
        private object tag;
        private object unboundValue;
        private ListViewDataItemGroup group;
        private ListViewDetailsCache cache; 
        private Size actualSize;
        private Size? customSize = null;
        private int imageIndex = -1;
        private string imageKey;
        private ToggleState checkState = ToggleState.Off;
        private ListViewSubDataItemCollection subItems;

        #endregion

        #region Constructors

        public ListViewDataItem()
        {
            cache = new ListViewDetailsCache();
            this.Visible = true;
            this.Enabled = true;
        }

        public ListViewDataItem(string text) : this()
        {
            this.Text = text;
        }

        public ListViewDataItem(object value) : this()
        {
            this.Value = value;
        }

        public ListViewDataItem(string text, string[] values) : this(text)
        {
            foreach (string value in values)
            {
                this.SubItems.Add(value);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///Gets or sets the key for the left image associated with this list view item.
        /// </summary>
        /// <seealso cref="Image">Image Property</seealso>
        /// <seealso cref="ImageIndex">ImageIndex Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(null)]
        [RelatedImageList("ListView.ImageList"),
        Editor(DesignerConsts.ImageKeyEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.RadImageKeyConverterString)]
        public string ImageKey
        {
            get
            {
                return this.imageKey;
            }
            set
            {
                if (this.imageKey != value && !this.OnNotifyPropertyChanging("ImageKey"))
                {
                    this.imageKey = value;
                    this.imageIndex = -1;
                    this.OnNotifyPropertyChanged("ImageKey");
                }
            }
        }

        /// <summary>
        /// Gets or sets the left image list index value of the image displayed.
        /// </summary>
        /// <seealso cref="Image">Image Property</seealso>
        /// <seealso cref="ImageKey">ImageKey Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(-1)]
        [Description("Gets or sets the left image list index value of the image displayed.")]
        [RelatedImageList("ListView.ImageList"),
        Editor(DesignerConsts.ImageIndexEditorString, typeof(UITypeEditor)),
        TypeConverter(DesignerConsts.NoneExcludedImageIndexConverterString)]
        public virtual int ImageIndex
        {
            get
            {
                return this.imageIndex;
            }
            set
            {
                if (this.imageIndex != value && !this.OnNotifyPropertyChanging("ImageIndex"))
                {
                    this.imageIndex = value;
                    this.OnNotifyPropertyChanged("ImageIndex");
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(typeof(Size),"0, 0")]
        public Size Size
        {
            get
            {
                return this.customSize ?? Size.Empty;
            }
            set
            {
                if (value != this.Size)
                {
                    this.customSize = value;
                }
            }
        }
         
        /// <summary>
        /// Gets or sets a value that indicates if this item is current.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        [Description("Gets or sets a value that indicates if this item is current.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool Current
        {
            get
            {
                return this.bitState[IsCurrentState];
            }
            internal set
            {
                if (this.Current != value && !this.OnNotifyPropertyChanging("Current"))
                {
                    this.bitState[IsCurrentState] = value;
                    this.OnNotifyPropertyChanged("Current");
                }
            }
        }

        internal bool IsMeasureValid
        {
            get
            {
                return this.bitState[IsMeasureValidState] && this.owner.IsItemsMeasureValid;
            }
            set
            {
                this.bitState[IsMeasureValidState] = value;
            }
        }

        internal bool IsLastInRow
        {
            get
            {
                return this.bitState[IsLastInRowState];
            }
            set
            {
                this.bitState[IsLastInRowState] = value;
            }
        }

        [Browsable(false)]
        public Size ActualSize
        {
            get
            {
                return actualSize;
            }
            internal set
            {
                if (this.actualSize != value && !this.OnNotifyPropertyChanging("ActualSize"))
                {
                    this.actualSize = value;
                    this.OnNotifyPropertyChanged("ActualSize");
                }

                this.IsMeasureValid = true;
            }
        }

        internal ListViewDetailsCache Cache
        {
            get
            {
                return cache;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has style.
        /// </summary>
        /// <value><c>true</c> if this instance has style; otherwise, <c>false</c>.</value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(false)]
        public bool HasStyle
        {
            get
            {
                return this.style != null;
            }
        }

        private ListViewDataItemStyle ItemStyle
        {
            get
            {
                if (this.style == null)
                {
                    style = new ListViewDataItemStyle();
                }

                return style;
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(null)]
        public virtual ListViewDataItemGroup Group
        {
            get
            {
                return group;
            }
            set
            {
                if (this.group != value)
                {
                    this.SetGroupCore(value);
                }
            }
        }

        [Category("Data"),
        Localizable(false),
        Bindable(true),
        DefaultValue((string)null),
        TypeConverter(typeof(StringConverter)),
        Description("Tag object that can be used to store user data, corresponding to the element")]
        public object Tag
        {
            get
            {
                return this.tag;
            }
            set
            {
                if (this.tag != value && !this.OnNotifyPropertyChanging("Tag"))
                {
                    this.tag = value;
                    this.OnNotifyPropertyChanged("Tag");
                }
            }
        }

        [Browsable(false)]
        public virtual bool IsDataBound
        {
            get
            {
                return this.owner != null && this.owner.DataSource != null;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadListView ListView
        {
            get
            {
                if (this.owner != null)
                {
                    return this.owner.ElementTree.Control as RadListView;
                }
                else
                {
                    return null;
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RadListViewElement Owner
        {
            get
            {
                return this.owner;
            }
            internal set
            {
                if (owner == value)
                {
                    return;
                }

                this.owner = value;

                this.OnNotifyPropertyChanged("Owner");
            }
        }

        /// <summary>
        /// Gets or sets a value for the property indicated by ValueMember if in bound mode, and private value in unbound mode.
        /// Trying to explicitly set this property in bound mode will result in an InvalidOperationException.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object Value
        {
            get
            {
                if (this.IsDataBound)
                {
                    return this.GetBoundValue();
                }
                else
                {
                    return this.GetUnboundValue();
                }
            }
            set
            {
                if (this.IsDataBound)
                {
                    this.SetBoundValue(value);
                }
                else
                {
                    this.SetUnboundValue(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        [Category("Appearance")]
        [DefaultValue(null)]
        [Description("Gets or sets the text displayed in the label of the list item.")]
        public virtual string Text
        {
            get
            { 
                if (this.dataBoundItem != null)
                {
                    object displayValue = this.GetDisplayValue();
                    return displayValue != null ? Convert.ToString(displayValue) : Convert.ToString(this.dataBoundItem);
                }
                else
                {
                    return Convert.ToString(this.Value);
                } 
            }
            set
            {
                if (this.Text != value && !this.OnNotifyPropertyChanging("Text"))
                {
                    this.Value = value;
                    this.OnNotifyPropertyChanged("Text");
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that indicates if this item is selected.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(false)]
        [Description("Gets or sets a value that indicates if this item is selected.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool Selected
        {
            get
            {
                return bitState[IsSelectedState];
            }

            internal set
            {
                if (value != this.Selected && !this.OnNotifyPropertyChanging("Selected"))
                {
                    bitState[IsSelectedState] = value;
                    this.OnNotifyPropertyChanged("Selected");
                }
            }
        }

        [Browsable(true)]
        [DefaultValue(true)]
        [RadPropertyDefaultValue("Enabled", typeof(RadElement))]
        [Description("Gets or sets whether this item responds to GUI events.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool Enabled
        {
            get
            {
                return bitState[IsEnabledState];
            }

            set
            {
                if (value != this.Enabled && !this.OnNotifyPropertyChanging("Enabled"))
                {
                    bitState[IsEnabledState] = value;
                    this.OnNotifyPropertyChanged("Enabled");
                }
            }
        }

        /// <summary>
        /// Gets a value that indicates if this item is currently visible.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(true)]
        [Description("Gets a value that indicates if this item is currently visible.")]
        public bool Visible
        {
            get
            {
                return this.bitState[IsVisibleState];
            }
            set
            {
                if (this.Visible != value && !this.OnNotifyPropertyChanging("Visible"))
                {
                    this.bitState[IsVisibleState] = value;
                    this.OnNotifyPropertyChanged("Visible");
                }
            }
        }

        /// <summary>
        /// Gets a value that indicating the current check state of the item.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(ToggleState.Off)]
        [Description("Gets a value that indicating the current check state of the item.")]
        public virtual ToggleState CheckState
        {
            get
            {
                return this.checkState;
            }
            set
            {
                if (this.checkState != value && !this.OnNotifyPropertyChanging("CheckState"))
                {
                    this.checkState = value;
                    this.OnNotifyPropertyChanged("CheckState");
                }
            }
        }

        [Browsable(true)]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Image Image
        {
            get
            {
                if (this.image != null)
                {
                    return this.image;
                }

                if (this.owner == null)
                {
                    return null;
                }

                int index = (this.ImageIndex >= 0) ? this.ImageIndex : this.owner.ImageIndex;
                if (index >= 0 && string.IsNullOrEmpty(this.ImageKey))
                {
                    RadControl control = this.owner.ElementTree.Control as RadControl;
                    if (control != null)
                    {
                        ImageList imageList = control.ImageList;
                        if (imageList != null && index < imageList.Images.Count)
                        {
                            return imageList.Images[index];
                        }
                    }
                }

                string treeKey = null;
                if (this.owner != null)
                {
                    treeKey = this.owner.ImageKey;
                }

                string key = (string.IsNullOrEmpty(this.ImageKey)) ? treeKey : this.ImageKey;
                if (!string.IsNullOrEmpty(key))
                {
                    RadControl control = this.owner.ElementTree.Control as RadControl;
                    if (control != null)
                    {
                        ImageList imageList = control.ImageList;
                        if (imageList != null && imageList.Images.Count > 0 && imageList.Images.ContainsKey(key))
                        {
                            return imageList.Images[key];
                        }
                    }
                }

                return null;
            }
            set
            {
                if (this.image != value && !this.OnNotifyPropertyChanging("Image"))
                {
                    this.image = value;
                    this.OnNotifyPropertyChanged("Image"); 
                }
            }
        }
         
        /// <summary>
        /// This collection is used for adding items at design time. It should not be used in runtime.
        /// </summary>
        [Browsable(true)]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design", "System.Drawing.Design.UITypeEditor, System.Drawing")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Description("This collection is used for adding items at design time. It should not be used in runtime.")]
        public virtual ListViewSubDataItemCollection SubItems
        {
            get
            {
                if (this.subItems == null)
                {
                    this.subItems = new ListViewSubDataItemCollection(this);
                }

                return this.subItems;
            }
        }

        #endregion

        #region Style Properties

        [Browsable(true)]
        [DefaultValue(TextImageRelation.Overlay)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public TextImageRelation TextImageRelation
        {
            get
            {
                if (this.HasStyle)
                {
                    return this.ItemStyle.TextImageRelation;
                }

                return ListViewDataItemStyle.DefaultTextImageRelation;
            }
            set
            {
                if (this.TextImageRelation != value && !this.OnNotifyPropertyChanging("TextImageRelation"))
                {
                    this.ItemStyle.TextImageRelation = value;
                    this.OnNotifyPropertyChanged("TextImageRelation");
                }
            }
        }

        [Browsable(true)]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContentAlignment TextAlignment
        {
            get
            {
                if (this.HasStyle)
                {
                    return this.ItemStyle.TextAlignment;
                }

                return ListViewDataItemStyle.DefaultTextAlignment;
            }
            set
            {
                if (this.TextAlignment != value && !this.OnNotifyPropertyChanging("TextAlignment"))
                {
                    this.ItemStyle.TextAlignment = value;
                    this.OnNotifyPropertyChanged("TextAlignment");
                }
            }
        }

        [Browsable(true)]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ContentAlignment ImageAlignment
        {
            get
            {
                if (this.HasStyle)
                {
                    return this.ItemStyle.ImageAlignment;
                }

                return ListViewDataItemStyle.DefaultImageAlignment;
            }
            set
            {
                if (this.ImageAlignment != value && !this.OnNotifyPropertyChanging("ImageAlignment"))
                {
                    this.ItemStyle.ImageAlignment = value;
                    this.OnNotifyPropertyChanged("ImageAlignment");
                }
            }
        }

        [Browsable(true)]
        [DefaultValue(null)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Font Font
        {
            get
            {
                if (this.HasStyle)
                {
                    return this.ItemStyle.Font;
                }

                return ListViewDataItemStyle.DefaultFont;
            }

            set
            {
                if (this.Font != value && !this.OnNotifyPropertyChanging("Font"))
                {
                    this.ItemStyle.Font = value;
                    this.OnNotifyPropertyChanged("Font");
                }
            }
        }

        [Browsable(true)]
        [DefaultValue(typeof(Color), "")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ForeColor
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.ForeColor; 
                }

                return ListViewDataItemStyle.DefaultForeColor;
            }

            set
            {
                if (this.ForeColor != value && !this.OnNotifyPropertyChanging("ForeColor"))
                {
                    this.ItemStyle.ForeColor = value;
                    this.OnNotifyPropertyChanged("ForeColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the list node. Color type represents an ARGB color.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientStyle">NumberOfColors Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the first back color.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public Color BackColor
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.BackColor;
                }
                return ListViewDataItemStyle.DefaultBackColor;
            }
            set
            {
                if (BackColor != value && !this.OnNotifyPropertyChanging("BackColor"))
                {
                    this.ItemStyle.BackColor = value;
                    OnNotifyPropertyChanged("BackColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the list item. This property is applicable to radial, glass,
        /// office glass, gel, and vista gradients.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientStyle">NumberOfColors Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the backcolor of the list item.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public Color BackColor2
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.BackColor2;
                }
                return ListViewDataItemStyle.DefaultBackColor2;
            }
            set
            {
                if (BackColor2 != value && !this.OnNotifyPropertyChanging("BackColor2"))
                {
                    this.ItemStyle.BackColor2 = value;
                    OnNotifyPropertyChanged("BackColor2");
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the list item. This property is applicable to radial, glass,
        /// office glass, and vista gradients.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientStyle">NumberOfColors Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the backcolor of the list item.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public Color BackColor3
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.BackColor3;
                }
                return ListViewDataItemStyle.DefaultBackColor3;
            }
            set
            {
                if (BackColor3 != value && !this.OnNotifyPropertyChanging("BackColor3"))
                {
                    this.ItemStyle.BackColor3 = value;
                    OnNotifyPropertyChanged("BackColor3");
                }
            }
        }

        /// <summary>
        /// Gets or sets the backcolor of the list item. This property is applicable to radial, glass,
        /// office glass, and vista gradients.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientStyle">NumberOfColors Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the backcolor of the list item.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public Color BackColor4
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.BackColor4;
                }
                return ListViewDataItemStyle.DefaultBackColor4;
            }
            set
            {
                if (BackColor4 != value  && !this.OnNotifyPropertyChanging("BackColor4"))
                {
                    this.ItemStyle.BackColor4 = value;
                    OnNotifyPropertyChanged("BackColor4");
                }
            }
        }

        /// <summary>
        /// Gets or sets the border color of the list item.
        /// </summary>		
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(typeof(Color), "")]
        [Description("Gets or sets the border color of the list item.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Browsable(true)]
        public Color BorderColor
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.BorderColor;
                }
                return ListViewDataItemStyle.DefaultBorderColor;
            }
            set
            {
                if (BorderColor != value && !this.OnNotifyPropertyChanging("BorderColor"))
                {
                    this.ItemStyle.BorderColor = value;
                    OnNotifyPropertyChanged("BorderColor");
                }
            }
        }

        /// <summary>
        /// Gets or sets gradient angle for linear gradient.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientPercentage">GradientPercentage Property</seealso>
        /// <seealso cref="GradientPercentage2">GradientPercentage2 Property</seealso>
        /// <seealso cref="NumberOfColors">NumberOfColors Property</seealso>
        /// <value>The default value is 90.0.</value>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(90.0f)]
        [Description("Gets or sets gradient angle for linear gradient.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float GradientAngle
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.GradientAngle;
                }
                return ListViewDataItemStyle.DefaultGradientAngle;
            }
            set
            {
                if (GradientAngle != value && !OnNotifyPropertyChanging("GradientAngle"))
                {
                    this.ItemStyle.GradientAngle = value;
                    OnNotifyPropertyChanged("GradientAngle");
                }
            }
        }

        /// <summary>
        /// Gets or sets GradientPercentage for linear, glass, office glass, gel, vista, and
        /// radial gradients.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientPercentage2">GradientPercentage2 Property</seealso>
        /// <seealso cref="GradientAngle">GradientAngle Property</seealso>
        /// <seealso cref="NumberOfColors">NumberOfColors Property</seealso>
        /// <value>The default value is 0.5.</value>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(0.5f)]
        [Description("Gets or sets GradientPercentage for linear, glass, office glass, gel, vista and radial gradients.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float GradientPercentage
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.GradientPercentage;
                }
                return ListViewDataItemStyle.DefaultGradientPercentage;
            }
            set
            {
                if (GradientPercentage != value && !OnNotifyPropertyChanging("GradientPercentage"))
                {
                    this.ItemStyle.GradientPercentage = value;
                    OnNotifyPropertyChanged("GradientPercentage");
                }
            }
        }

        /// <summary>
        /// Gets or sets GradientPercentage for office glass, vista, and radial
        /// gradients.
        /// </summary>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <seealso cref="GradientPercentage">GradientPercentage Property</seealso>
        /// <seealso cref="GradientAngle">GradientAngle Property</seealso>
        /// <seealso cref="NumberOfColors">NumberOfColors Property</seealso>
        /// <value>The default value is 0.5.</value>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(0.5f)]
        [Description("Gets or sets GradientPercentage for office glass, vista, and radial gradients.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float GradientPercentage2
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.GradientPercentage2;
                }
                return ListViewDataItemStyle.DefaultGradientPercentage2;
            }
            set
            {
                if (GradientPercentage2 != value && !OnNotifyPropertyChanging("GradientPercentage2"))
                {
                    this.ItemStyle.GradientPercentage2 = value;
                    OnNotifyPropertyChanged("GradientPercentage2");
                }
            }
        }

        /// <summary>
        /// Gets and sets the gradient style. The possible values are defined in the gradient
        /// style enumeration: solid, linear, radial, glass, office glass, gel, and vista.
        /// </summary>
        /// <value>
        ///     The default value is
        ///     <see cref="Telerik.WinControls.GradientStyles">GradientStyles.Linear</see>.
        /// </value>
        /// <seealso cref="GradientStyles">GradientStyles Enumeration</seealso>
        /// <seealso cref="GradientPercentage">GradientPercentage Property</seealso>
        /// <seealso cref="GradientPercentage2">GradientPercentage2 Property</seealso>
        /// <seealso cref="GradientAngle">GradientAngle Property</seealso>
        /// <seealso cref="NumberOfColors">NumberOfColors Property</seealso>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(GradientStyles.Linear)]
        [Description("Gets or sets the gradient angle.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public GradientStyles GradientStyle
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.GradientStyle;
                }
                return ListViewDataItemStyle.DefaultGradientStyle;
            }
            set
            {
                if (GradientStyle != value && !OnNotifyPropertyChanging("GradientStyle"))
                {
                    this.ItemStyle.GradientStyle = value;
                    OnNotifyPropertyChanged("GradientStyle");
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of used colors in the gradient effect.
        /// </summary>
        /// <seealso cref="BackColor">BackColor Property</seealso>
        /// <seealso cref="BackColor2">BackColor2 Property</seealso>
        /// <seealso cref="BackColor3">BackColor3 Property</seealso>
        /// <seealso cref="BackColor4">BackColor4 Property</seealso>
        /// <seealso cref="GradientStyle">GradientStyle Property</seealso>
        /// <value>The default value is 4.</value>
        [NotifyParentProperty(true)]
        [Category("Appearance"), DefaultValue(4)]
        [Description("Gets or sets the number of used colors in the gradient effect.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int NumberOfColors
        {
            get
            {
                if (HasStyle)
                {
                    return this.ItemStyle.NumberOfColors;
                }
                return ListViewDataItemStyle.DefaultNumberOfColors;
            }
            set
            {
                if (NumberOfColors != value && !OnNotifyPropertyChanging("NumberOfColors"))
                {
                    this.ItemStyle.NumberOfColors = value;
                    OnNotifyPropertyChanged("NumberOfColors");
                }
            }
        }

        #endregion

        #region IDataItem Members

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object DataBoundItem
        {
            get
            {
                return this.dataBoundItem;
            }
            set
            {
                this.SetDataBoundItem(false, value);
            }
        }

        [Browsable(false)]
        public int FieldCount
        {
            get
            {
                if (owner.ViewType == ListViewType.DetailsView)
                    return this.owner.Columns.Count;
                else return 1;
            }
        }

        public object this[string name]
        {
            get
            {
                if (this.owner != null && this.owner.Columns.Contains(name))
                    return this[this.owner.Columns[name]];
                else return this.Value;
            }
            set
            {
                if (this.owner != null && this.owner.Columns.Contains(name))
                    this[this.owner.Columns[name]] = value;
                else this.Value = value;
            }
        }

        public object this[int index]
        {
            get
            {
                if (this.owner!=null && this.owner.Columns.Count > index)
                    return this[this.owner.Columns[index]];
                else return this.Value;
            }
            set
            {
                if (this.owner != null && this.owner.Columns.Count > index)
                    this[this.owner.Columns[index]] = value;
                else this.Value = value;
            }
        }

        public object this[ListViewDetailColumn column]
        {
            get
            {
                if (column.Accessor[this] == null && 
                    this.subItems != null && 
                    this.owner.Columns.IndexOf(column)< this.subItems.Count)
                {
                    if (this.owner.IsDesignMode)
                    {
                        return this.subItems[this.owner.Columns.IndexOf(column)];
                    }

                    column.Accessor.SuspendItemNotifications();
                    column.Accessor[this] = this.subItems[this.owner.Columns.IndexOf(column)];
                    column.Accessor.ResumeItemNotifications();
                }

                return column.Accessor[this];
            }
            set
            {
                column.Accessor[this] = value;
            }
        }

        public int IndexOf(string name)
        {
            if (this.owner != null && this.owner.Columns.Contains(name))
            {
                return this.owner.Columns.IndexOf(name);
            }

            return 0;
        }

        #endregion

        #region Virtual Methods

        protected virtual object GetBoundValue()
        {
            if (String.IsNullOrEmpty(this.owner.ValueMember))
            {
                if (String.IsNullOrEmpty(this.owner.DisplayMember))
                {
                    return this.dataBoundItem;
                }

                return this.GetValue(this.owner.DisplayMember);
            }

            return this.GetValue(this.owner.ValueMember);
        }

        /// <summary>
        /// Gets a value for the Value property in unbound mode.
        /// </summary>
        /// <returns>Returns an object reference pointing to the value of the Value property in unbound mode.</returns>
        protected virtual object GetUnboundValue()
        {
            return this.unboundValue;
        }

        /// <summary>
        /// This method is called when setting the Value property of a RadListDataItem when it is in unbound mode.
        /// </summary>
        /// <param name="value">The value to set the Value property to.</param>
        protected virtual void SetUnboundValue(object value)
        {
            if (this.unboundValue != value && !this.OnNotifyPropertyChanging("Value"))
            {
                this.unboundValue = value;
                this.OnNotifyPropertyChanged("Value");
            }
        }
         
        protected virtual void SetBoundValue(object value)
        {
            if (!String.IsNullOrEmpty(this.owner.ValueMember))
            {
                this.owner.ListSource.SetBoundValue(this,this.owner.ValueMember,value);
            }
            else if (!String.IsNullOrEmpty(this.owner.DisplayMember))
            {
                this.owner.ListSource.SetBoundValue(this, this.owner.DisplayMember, value);
            }
        }

        protected internal virtual void SetDataBoundItem(bool dataBinding, object value)
        {
            if (!dataBinding && this.owner != null && this.owner.DataSource != null)
            {
                throw new InvalidOperationException("DataBoundItem can not be set explicitly in bound mode.");
            }

            this.dataBoundItem = value;
            if (this.Owner != null && this.Owner.DataSource != null)
            {
                this.Owner.OnItemDataBound(this);
            }

            this.OnNotifyPropertyChanged("DataBoundItem");
        }

        internal void SetGroupCore(ListViewDataItemGroup value, bool changeGroupCollection)
        {
            if (changeGroupCollection)
            {
                this.SetGroupCore(value);
            }
            else
            {
                this.group = value;
            }
        }

        private void SetGroupCore(ListViewDataItemGroup value)
        {
            if (this.OnNotifyPropertyChanging("Group"))
            {
                return;
            }

            if (this.group != null)
            {
                this.group.Items.innerList.Remove(this);
            }

            this.group = value;

            if (this.group != null)
            {
                this.group.Items.innerList.Add(this);
            }

            this.OnNotifyPropertyChanged("Group");
        }

        protected object GetValue(string propertyName)
        {
            object value = null;

            try
            {
                string[] names = propertyName.Split('.');
                if (names.Length > 1)
                {
                    value = GetSubPropertyValue(propertyName, this.DataBoundItem);
                }
                else
                {
                    value = this.owner.ListSource.GetBoundValue(this.DataBoundItem, propertyName);
                }
            }
            catch (ArgumentException)
            {
                value = this.DataBoundItem;
                this.owner.DisplayMember = "";
                this.owner.ValueMember = "";
            }

            return value;
        }

        private object GetSubPropertyValue(string propertyPath, object dataObject)
        {
            PropertyDescriptor innerDescriptor = null;
            object innerObject = null;
            this.GetSubPropertyByPath(propertyPath, dataObject, out innerDescriptor, out innerObject);

            if (innerDescriptor != null)
            {
                return innerDescriptor.GetValue(innerObject);
            }

            return null;
        }

        private void GetSubPropertyByPath(string propertyPath, object dataObject, out PropertyDescriptor innerDescriptor, out object innerObject)
        {
            string[] names = propertyPath.Split('.');
            innerDescriptor = this.owner.ListSource.BoundProperties[names[0]];

            innerObject = innerDescriptor.GetValue(dataObject);
            for (int index = 1; index < names.Length && (innerDescriptor != null); index++)
            {
                innerDescriptor = innerDescriptor.GetChildProperties()[names[index]];
                if (!(index + 1 == names.Length))
                {
                    innerObject = innerDescriptor.GetValue(innerObject);
                }
            }
        }

        protected object GetDisplayValue()
        {
            if (string.IsNullOrEmpty(this.owner.DisplayMember))
            {
                return this.GetFormattedValue(this.DataBoundItem);
            }

            object value = null;
            try
            {
                string[] names = this.owner.DisplayMember.Split('.');
                if (names.Length > 1)
                {
                    value = GetSubPropertyValue(this.owner.DisplayMember, this.DataBoundItem);
                }
                else
                {
                    value = this.owner.ListSource.GetBoundValue(this.DataBoundItem, this.owner.DisplayMember);
                }
            }
            catch (ArgumentException)
            {
                value = this.DataBoundItem;
                this.owner.DisplayMember = "";
                this.owner.ValueMember = "";
            }

            if (value == null)
            {
                value = "";
            }

            return this.GetFormattedValue(value);
        }

        protected virtual string GetFormattedValue(object value)
        {
            return value.ToString();
        }

        #endregion

        #region INotifyPropertyChanged and INotifyPropertyChangingEx

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandlerEx PropertyChanging;

        protected void OnNotifyPropertyChanged(string propertyName)
        {
            OnNotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnNotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Text" || args.PropertyName == "Value")
            {
                this.IsMeasureValid = false;
            }

            if (args.PropertyName == "Selected" && this.owner != null)
            { 
                if(!(this is ListViewDataItemGroup))
                {
                    this.owner.SelectedItems.ProcessSelectedItem(this);
                }
                if (this.group != null)
                {
                    this.group.OnItemSelectedChanged();
                }
            }
            if (args.PropertyName == "CheckState" && this.owner != null)
            {
                this.owner.CheckedItems.ProcessCheckedItem(this);
                this.owner.OnItemCheckedChanged(new ListViewItemEventArgs(this));
            }

            if (args.PropertyName == "Owner" && this.group!= null && this.owner != this.group.owner)
            {
                this.Group = null;
            }

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, args);
            }

            if ((args.PropertyName == "Visible" || args.PropertyName == "Group") && this.owner != null)
            {
                this.owner.ViewElement.ViewElement.InvalidateMeasure();
                this.owner.ViewElement.ViewElement.UpdateLayout();
                this.owner.ViewElement.Scroller.UpdateScrollRange();
            }

            if (args.PropertyName == "Expanded" && this.owner!=null)
            {
                this.owner.ViewElement.ViewElement.Invalidate();
            }
        }

        protected bool OnNotifyPropertyChanging(string propertyName)
        {
            PropertyChangingEventArgsEx args = new PropertyChangingEventArgsEx(propertyName);

            return OnNotifyPropertyChanging(args);
        }
  
        protected virtual bool OnNotifyPropertyChanging(PropertyChangingEventArgsEx args)
        {
            if (args.PropertyName == "CheckState" && this.owner != null)
            {
                args.Cancel = this.owner.OnItemCheckedChanging(new ListViewItemCancelEventArgs(this));
            }

            if (this.PropertyChanging != null)
            { 
                this.PropertyChanging(this, args);
                return args.Cancel;
            }

            return false;
        }

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
        }

        #endregion
    }
}