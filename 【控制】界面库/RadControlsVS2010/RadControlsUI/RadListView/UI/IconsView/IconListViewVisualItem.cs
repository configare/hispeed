using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    public class IconListViewVisualItem : BaseListViewVisualItem
    {
        public override bool IsCompatible(ListViewDataItem data, object context)
        {
            if (!(data is ListViewDataItemGroup) && data.Owner.ViewType == ListViewType.IconsView)
            {
                return true;
            }

            return false;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.TextWrap = true;
        }

        protected override RectangleF GetEditorArrangeRectangle(RectangleF clientRect)
        {
            return this.layoutManagerPart.RightPart.Bounds;
        }

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            if (this.Data == null)
            {
                return SizeF.Empty;
            }
             
            if (this.dataItem.Owner.ShowCheckBoxes)
            {
                this.toggleElement.Visibility = ElementVisibility.Visible; 
            }
            else
            {
                this.toggleElement.Visibility = ElementVisibility.Collapsed;
            }
             
            SizeF desiredSize = base.MeasureOverride(availableSize);
            desiredSize.Width += this.toggleElement.DesiredSize.Width;

            if (this.Data.Size.Height > 0)
            {
                desiredSize.Height = this.Data.Size.Height;
            }

            if (this.Data.Size.Width > 0)
            {
                desiredSize.Width = this.Data.Size.Width;
            }

            RadListViewElement listViewElement = this.Data.Owner;

            if (listViewElement != null)
            {
                if (!listViewElement.AllowArbitraryItemHeight && !listViewElement.AllowArbitraryItemWidth)
                {
                    desiredSize = listViewElement.ItemSize;
                }
                else if (listViewElement.AllowArbitraryItemHeight && !listViewElement.AllowArbitraryItemWidth)
                {
                    desiredSize = base.MeasureOverride(new SizeF(listViewElement.ItemSize.Width, availableSize.Height));
                    desiredSize.Width = listViewElement.ItemSize.Width;

                    if (this.Data.Size.Height > 0)
                    {
                        desiredSize.Height = this.Data.Size.Height;
                    }
                }
                else if (!listViewElement.AllowArbitraryItemHeight && listViewElement.AllowArbitraryItemWidth)
                {
                    desiredSize = base.MeasureOverride(new SizeF(availableSize.Width, listViewElement.ItemSize.Height));
                    desiredSize.Height = listViewElement.ItemSize.Height;
                    desiredSize.Width += this.toggleElement.DesiredSize.Width;

                    if (this.Data.Size.Width > 0)
                    {
                        desiredSize.Width = this.Data.Size.Width;
                    }
                }
            }
            
            this.Data.ActualSize = desiredSize.ToSize();

            SizeF clientSize = GetClientRectangle(desiredSize).Size;

            RadItem editorElement = this.GetEditorElement(editor);

            SizeF sizef = new SizeF(clientSize.Width - this.toggleElement.DesiredSize.Width, clientSize.Height);

            this.layoutManagerPart.Measure(sizef);

            if (IsInEditMode && editorElement != null)
            {
                editorElement.Measure(this.layoutManagerPart.RightPart.DesiredSize);
            }
             
            return desiredSize;
        }
    }
}
