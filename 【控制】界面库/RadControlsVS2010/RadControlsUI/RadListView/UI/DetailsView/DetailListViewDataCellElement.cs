using System.Drawing;
using System; 

namespace Telerik.WinControls.UI
{
    public class DetailListViewDataCellElement : DetailListViewCellElement
    {
        #region RadProperties

        public static RadProperty SelectedProperty = RadProperty.Register("Selected",
            typeof(bool), typeof(DetailListViewDataCellElement), new RadElementPropertyMetadata(false));

        public static RadProperty CurrentRowProperty = RadProperty.Register("OwnerCurrent",
            typeof(bool), typeof(DetailListViewDataCellElement), new RadElementPropertyMetadata(false));

        #endregion

        #region Fields

        DetailListViewVisualItem row;

        #endregion

        #region Ctors

        public ListViewDataItem Row
        {
            get
            {
                return row.Data;
            }
        }

        static DetailListViewDataCellElement()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ListViewDataCellStateManagerFactory(), typeof(DetailListViewDataCellElement));
        }

        public DetailListViewDataCellElement(DetailListViewVisualItem owner, ListViewDetailColumn column)
            : base(column)
        {
            this.row = owner;
            this.BindProperty(DetailListViewDataCellElement.SelectedProperty, owner, DetailListViewVisualItem.SelectedProperty, PropertyBindingOptions.OneWay);
            this.BindProperty(DetailListViewDataCellElement.CurrentRowProperty, owner, DetailListViewVisualItem.CurrentProperty, PropertyBindingOptions.OneWay);
        }

        #endregion

        #region Overrides

        public override void Synchronize()
        {
            if (this.Data == null || this.row == null || this.row.Data == null)
            {
                return;
            }

            this.Text = Convert.ToString(this.row.Data[this.column]);
            
            if (this.row!= null && this.GetFirstVisibleColumn() == (this.column))
            {
                this.Image = this.row.Data.Image;
            }
            else
            {
                this.ResetValue(ImageProperty);
            }

            this.SynchronizeStyleProperties();

            this.column.Owner.OnCellFormatting(new ListViewCellFormattingEventArgs(this));
        }
 
        protected override bool ProcessDragOver(Point mousePosition, ISupportDrag dragObject)
        {
            return false;
        }

        protected override void DisposeManagedResources()
        {
            this.UnbindProperty(DetailListViewDataCellElement.SelectedProperty);
            this.UnbindProperty(DetailListViewDataCellElement.CurrentRowProperty);
            base.DisposeManagedResources();
        }

        protected override void OnDoubleClick(System.EventArgs e)
        {
            base.OnDoubleClick(e);
            this.row.CallDoDoubleClick(e);
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.row.CallDoClick(e);
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == RadElement.IsMouseOverElementProperty &&
                this.IsInValidState(true) && this.Data != null)
            {
                if (this.Data.Owner.HotTracking)
                {
                    this.row.SetValue(BaseListViewVisualItem.HotTrackingProperty, e.NewValue);
                }
                else
                {
                    this.row.SetValue(BaseListViewVisualItem.HotTrackingProperty, false);
                }
            }
        }

        #endregion

        #region Helpers

        private ListViewDetailColumn GetFirstVisibleColumn()
        {
            if (this.row == null || this.row.Data == null)
            {
                return null;
            }

            DetailListViewElement viewElement = (this.row.Data.Owner.ViewElement as DetailListViewElement);

            if (viewElement == null)
            {
                return this.Data.Owner.Columns.Count > 0 ? this.Data.Owner.Columns[0] : null;
            }

            foreach (ListViewDetailColumn column in viewElement.Owner.Columns)
            {
                if (column.Visible)
                {
                    return column;
                }
            }

            return this.Data.Owner.Columns.Count > 0 ? this.Data.Owner.Columns[0] : null;
        }

        private void SynchronizeStyleProperties()
        {

            if (this.row != null && this.row.Data != null && this.row.Data.HasStyle)
            {
                if (this.row.Data.Font != ListViewDataItemStyle.DefaultFont && this.row.Data.Font != this.Font)
                {
                    this.Font = this.row.Data.Font;
                }

                if (this.row.Data.ForeColor != ListViewDataItemStyle.DefaultForeColor && this.row.Data.ForeColor != this.ForeColor)
                {
                    this.ForeColor = this.row.Data.ForeColor;
                }

                if (this.row.Data.TextAlignment != this.TextAlignment)
                {
                    this.TextAlignment = this.row.Data.TextAlignment;
                }

                if (this.row.Data.TextImageRelation != ListViewDataItemStyle.DefaultTextImageRelation && this.row.Data.TextImageRelation != this.TextImageRelation)
                {
                    this.TextImageRelation = this.row.Data.TextImageRelation;
                }

                if (this.row.Data.ImageAlignment != ListViewDataItemStyle.DefaultImageAlignment && this.row.Data.ImageAlignment != this.ImageAlignment)
                {
                    this.ImageAlignment = this.row.Data.ImageAlignment;
                }
            }
            else
            {
                if (this.GetValueSource(FontProperty) != ValueSource.Style)
                {
                    this.ResetValue(FontProperty);
                }

                if (this.GetValueSource(ForeColorProperty) != ValueSource.Style)
                {
                    this.ResetValue(ForeColorProperty);
                }

                if (this.GetValueSource(TextAlignmentProperty) != ValueSource.Style)
                {
                    this.ResetValue(TextAlignmentProperty);
                    this.SetDefaultValueOverride(TextAlignmentProperty, ListViewDataItemStyle.DefaultTextAlignment);
                }

                if (this.GetValueSource(TextImageRelationProperty) != ValueSource.Style)
                {
                    this.ResetValue(TextImageRelationProperty);
                    this.SetDefaultValueOverride(TextImageRelationProperty, ListViewDataItemStyle.DefaultTextImageRelation);
                }

                if (this.GetValueSource(ImageAlignmentProperty) != ValueSource.Style)
                {
                    this.ResetValue(ImageAlignmentProperty);
                    this.SetDefaultValueOverride(ImageAlignmentProperty, ListViewDataItemStyle.DefaultImageAlignment);
                }
            }
        }

        #endregion
    }
}
