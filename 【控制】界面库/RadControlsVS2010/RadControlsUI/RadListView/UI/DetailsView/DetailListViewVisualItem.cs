using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Collections.Specialized;
using Telerik.WinControls.Layouts;

namespace Telerik.WinControls.UI
{
    public class DetailListViewVisualItem : BaseListViewVisualItem
    {
        #region Fields

        DetailListViewColumnContainer cellContainer;

        #endregion

        #region Overrides

        public override void Attach(ListViewDataItem data, object context)
        {
            base.Attach(data, context);

            cellContainer.Context = dataItem.Owner.ViewElement as DetailListViewElement;
            cellContainer.DataProvider = cellContainer.Context.ColumnScroller;
            cellContainer.Context.ColumnScroller.ScrollerUpdated += new EventHandler(ColumnScroller_ScrollerUpdated);
            foreach (DetailListViewDataCellElement cell in this.cellContainer.Children)
            {
                cell.Synchronize();
            }
            
        }

        public override void Detach()
        {
            base.Detach();
            cellContainer.Context.ColumnScroller.ScrollerUpdated -= new EventHandler(ColumnScroller_ScrollerUpdated);
        }

        protected override void SynchronizeProperties()
        {
            base.SynchronizeProperties();

            foreach (DetailListViewDataCellElement cell in this.cellContainer.Children)
            {
                cell.Synchronize();
            }
        } 

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.cellContainer = new DetailListViewColumnContainer();
            this.cellContainer.StretchHorizontally = true;
            this.cellContainer.StretchVertically = true;
            this.cellContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.cellContainer.ElementProvider = new DetailListViewDataCellElementProvider(this);
            this.Children.Add(cellContainer);
        }

        protected override System.Drawing.SizeF ArrangeOverride(System.Drawing.SizeF finalSize)
        {
            RectangleF clientRect = GetClientRectangle(finalSize);
            RectangleF finalRect = new RectangleF(PointF.Empty, finalSize);

            if (this.toggleElement.Visibility != ElementVisibility.Collapsed)
            {
                this.toggleElement.Arrange(new RectangleF(new PointF(clientRect.X, clientRect.Y + (clientRect.Height - this.toggleElement.DesiredSize.Height) / 2), this.toggleElement.DesiredSize));
            }

            finalRect.Width -= this.toggleElement.DesiredSize.Width;
            finalRect.X += this.toggleElement.DesiredSize.Width;

            this.cellContainer.Arrange(finalRect);

            if (this.IsInEditMode)
            {
               RadItem editorElement = this.GetEditorElement(this.editor);
               editorElement.Arrange(this.GetEditorArrangeRectangle(clientRect)); 
            }

            return finalSize;
        }

        protected override RectangleF GetEditorArrangeRectangle(RectangleF clientRect)
        {
            foreach (RadElement element in this.cellContainer.Children)
            {
                DetailListViewDataCellElement cell = element as DetailListViewDataCellElement;
                if (cell != null && cell.Data.Current)
                {
                    RectangleF rect = cell.BoundingRectangle;
                    rect.X += this.cellContainer.BoundingRectangle.X;
                    rect.Width = Math.Min(rect.Width, clientRect.Width - this.cellContainer.BoundingRectangle.X);
                    return rect;
                }
            }

            return RectangleF.Empty;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Data == null)
            {
                return SizeF.Empty;
            }
            RectangleF clientRect = GetClientRectangle(availableSize);
             
            SizeF desiredSize = base.MeasureOverride(LayoutUtils.InfinitySize);

            desiredSize.Width = cellContainer.DesiredSize.Width + toggleElement.DesiredSize.Width;

            if (this.Data.Size.Height > 0)
            {
                desiredSize.Height = this.Data.Size.Height;
            }
            
            RadListViewElement listViewElement = this.Data.Owner;

            if (listViewElement != null && !listViewElement.AllowArbitraryItemHeight)
            {
                desiredSize.Height = listViewElement.ItemSize.Height;
            }
            
            if ((listViewElement != null && listViewElement.FullRowSelect) ||
                desiredSize.Width > clientRect.Width)
            {
                desiredSize.Width = clientRect.Width;
            }

            this.Data.ActualSize = desiredSize.ToSize();
            this.cellContainer.Measure(this.Data.ActualSize);

            SizeF clientSize = clientRect.Size;

            if (this.dataItem.Owner.ShowCheckBoxes)
            {
                this.toggleElement.Visibility = ElementVisibility.Visible;
                this.toggleElement.Measure(clientSize);
            }
            else
            {
                this.toggleElement.Visibility = ElementVisibility.Collapsed;
            }

            RadItem editorElement = this.GetEditorElement(editor);

            SizeF sizef = new SizeF(clientSize.Width - this.toggleElement.DesiredSize.Width, clientSize.Height);

            if (IsInEditMode && editorElement != null)
            {
                editorElement.Measure(new SizeF(
                    Math.Min(this.Data.Owner.CurrentColumn.Width, availableSize.Width),
                    sizef.Height));
                desiredSize.Height = Math.Max(desiredSize.Height, editorElement.DesiredSize.Height);
            }

            this.layoutManagerPart.Measure(sizef);

            return desiredSize;
        }

        public override string Text
        {
            get
            {
                return String.Empty;
            }
            set
            {
                base.Text = value;
            }
        }

        public override bool IsCompatible(ListViewDataItem data, object context)
        {
            if (!(data is ListViewDataItemGroup) && data.Owner.ViewType == ListViewType.DetailsView)
            {
                return true;
            }

            return false;
        }

        #endregion

        #region Event Handlers

        void ColumnScroller_ScrollerUpdated(object sender, EventArgs e)
        {
            this.cellContainer.InvalidateMeasure();
        }

        #endregion
    }
}
