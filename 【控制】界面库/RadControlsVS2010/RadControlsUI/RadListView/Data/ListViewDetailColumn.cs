using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Telerik.WinControls.UI
{
    [TypeConverter(typeof(ListViewColumnTypeConverter))]
    public class ListViewDetailColumn : RadObject
    {
        #region RadProperties

        public static RadProperty FieldNameProperty = RadProperty.Register("FieldName", 
            typeof(string), typeof(ListViewDetailColumn), new RadElementPropertyMetadata(""));
        
        public static RadProperty HeaderTextProperty = RadProperty.Register("HeaderText", 
            typeof(string), typeof(ListViewDetailColumn), new RadElementPropertyMetadata(null));

        public static RadProperty VisibleProperty = RadProperty.Register("Visible", 
            typeof(bool), typeof(ListViewDetailColumn), new RadElementPropertyMetadata(true));

        public static RadProperty CurrentProperty = RadProperty.Register("Current",
            typeof(bool), typeof(ListViewDetailColumn), new RadElementPropertyMetadata(false));

        public static RadProperty WidthProperty = RadProperty.Register("Width",
            typeof(float), typeof(ListViewDetailColumn), new RadElementPropertyMetadata(200.0f));

        #endregion

        #region Fields

        private RadListViewElement owner;
        private ListViewAccessor accessor;
        private string name;
        private float width = 200;
        private float cachedWidth = 200;
        private float maxWidth = 0;
        private float minWidth = 20;

        #endregion

        #region Constructors

        public ListViewDetailColumn(string name)
            : this(name, name)
        {
        }
         
        public ListViewDetailColumn(string name, string headerText)
           
        {
            this.name = name;
            this.HeaderText = headerText;
            this.accessor = new ListViewAccessor(this);
        }

        protected internal virtual void Initialize()
        { 
            if (this.Owner.IsDataBound)
            {
                this.Accessor = new ListViewBoundAccessor(this);
            }
            else
            {
                this.Accessor = new ListViewAccessor(this);
            }
        }

        #endregion

        #region Properties
 
        /// <summary>
        /// Gets the <see cref="RadListViewElement"/> that owns this column.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets the RadListViewElement that owns this column.")]
        public RadListViewElement Owner 
        {
            get
            {
                return this.owner;
            }
            internal set
            {
                if (value != this.owner)
                {
                    this.owner = value;
                    this.Initialize();
                }
            }
        }

        /// <summary>
        /// Gets the maximum width that the column can be resized to.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(0f)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category(RadDesignCategory.LayoutCategory)]
        [Description("Gets the maximum width that the column can be resized to.")]
        public float MaxWidth
        {
            get
            {
                return maxWidth;
            }
            set
            {
                maxWidth = value;
                UpdateWidth();
            }
        }

        /// <summary>
        /// Gets the minimum width that the column can be resized to.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(20f)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category(RadDesignCategory.LayoutCategory)]
        [Description("Gets the minimum width that the column can be resized to.")]
        public float MinWidth
        {
            get
            {
                return minWidth;
            }
            set
            {
                minWidth = value;
                UpdateWidth();
            }
        }

        /// <summary>
        /// Gets the current width of the column.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(200f)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category(RadDesignCategory.LayoutCategory)]
        [Description("Gets the current width of the column.")]
        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
                UpdateWidth();
            }
        }

        /// <summary>
        /// Gets the name of the field of the bound item corresponding to this column.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Gets the name of the field of the bound item corresponding to this column.")]
        public string FieldName
        {
            get
            {
                return (string)GetValue(FieldNameProperty); 
            }
            internal set 
            {
                SetValue(FieldNameProperty, value); 
            }
        }

        /// <summary>
        /// Gets the name of the column. Must be unique for each column in the same <see cref="RadListViewElement"/>.
        /// </summary>
        [Browsable(true)]
        [Description("Gets the name of the column. Must be unique for each column in the same RadListViewElement")]
        public string Name
        {
            get 
            {
                return this.name; 
            }
        }

        /// <summary>
        /// Gets or sets the text that will be displayed in the header cells.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets the text that will be displayed in the header cells.")]
        public string HeaderText
        {
            get
            {
                return (string)GetValue(HeaderTextProperty);
            }
            set
            {
                SetValue(HeaderTextProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the column is in bound mode.
        /// </summary>
        [Browsable(false)]
        [Description("Gets a value indicating whether the column is in bound mode.")]
        public bool IsDataBound
        {
            get
            {
                return (this.accessor is ListViewBoundAccessor);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column is current.
        /// </summary>
        [Browsable(true)]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Category(RadDesignCategory.BehaviorCategory)]
        [Description("Gets or sets a value indicating whether this column is current.")]
        public bool Current
        {
            get
            {
                return (bool)GetValue(CurrentProperty);
            }
            set
            {
                this.SetValue(CurrentProperty, value);
                if (this.owner == null)
                {
                    return;
                }

                if (value)
                {
                    this.owner.CurrentColumn = this;
                }
                else
                {
                    this.owner.CurrentColumn = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this column will be visible in DetailsView.
        /// </summary>
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [DefaultValue(true)]
        [Category(RadDesignCategory.AppearanceCategory)]
        [Description("Gets or sets a value indicating whether this column will be visible in DetailsView.")]
        public bool Visible
        {
            get
            {
                return (bool)this.GetValue(VisibleProperty);
            }
            set
            {
                this.SetValue(VisibleProperty, value);
            }
        }

        internal ListViewAccessor Accessor
        {
            get
            {
                return this.accessor;
            }
            set
            {
                if (!Object.Equals(this.accessor, value))
                {
                    if (this.accessor != null)
                    {
                        this.accessor.Dispose();
                    }

                    this.accessor = value;
                }
            }
        }
 
        #endregion
        
        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == WidthProperty && this.Owner != null)
            {
                DetailListViewElement detailsView = (this.Owner.ViewElement as DetailListViewElement);

                if (detailsView != null)
                {
                    detailsView.ColumnScroller.UpdateScrollRange();
                    detailsView.ViewElement.InvalidateMeasure();
                }
            }
        }

        private void UpdateWidth()
        {
            if (this.maxWidth > 0)
            {
                width = Math.Min(width, this.maxWidth);
            }

            width = Math.Max(width, this.minWidth);

            if (cachedWidth != width)
            {
                this.SetValue(WidthProperty, width);
                cachedWidth = width;
            }
        }
    }
}
