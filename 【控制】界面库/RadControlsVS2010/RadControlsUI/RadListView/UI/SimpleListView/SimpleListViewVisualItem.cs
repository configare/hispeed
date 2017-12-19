using System.Diagnostics;
using System.ComponentModel;
using System.Drawing;
using Telerik.WinControls.Layouts;
using System;

namespace Telerik.WinControls.UI
{
    public class SimpleListViewVisualItem : BaseListViewVisualItem
    {
        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.Alignment = ContentAlignment.MiddleLeft;
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

            float indent = 0;

            if (this.Data.Owner.ShowGroups &&
            (this.Data.Owner.EnableCustomGrouping || this.Data.Owner.EnableGrouping) &&
            this.Data.Owner.Groups.Count > 0)
            {
                indent = this.Data.Owner.GroupIndent;
            }

            SizeF desiredSize = base.MeasureOverride(LayoutUtils.InfinitySize);
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

            if (listViewElement != null && !listViewElement.AllowArbitraryItemHeight)
            {
                desiredSize.Height = listViewElement.ItemSize.Height;
            }

            if (listViewElement != null && !listViewElement.AllowArbitraryItemWidth && !listViewElement.FullRowSelect)
            {
                desiredSize.Width = listViewElement.ItemSize.Width;
            }

            if (listViewElement != null && listViewElement.FullRowSelect)
            {
                desiredSize.Width = Math.Max(GetClientRectangle(availableSize).Width, desiredSize.Width + indent);
            }

            SizeF clientSize = GetClientRectangle(desiredSize).Size;

            RadItem editorElement = this.GetEditorElement(editor);

            SizeF sizef = new SizeF(clientSize.Width - this.toggleElement.DesiredSize.Width, clientSize.Height);

            if (IsInEditMode && editorElement != null)
            {
                float editorWidth = Math.Min(clientSize.Width - this.toggleElement.DesiredSize.Width - indent, availableSize.Width - indent);
                editorElement.Measure(new SizeF(editorWidth, float.PositiveInfinity));
                desiredSize.Height = Math.Max(desiredSize.Height, editorElement.DesiredSize.Height);
                sizef.Height = desiredSize.Height;
            }

            this.layoutManagerPart.Measure(sizef);

            this.Data.ActualSize = desiredSize.ToSize();

            return desiredSize;
        }

        protected override void ArrangeContentCore(RectangleF clientRect)
        {
            if (this.Data.Owner.ShowGroups &&
                (this.Data.Owner.EnableCustomGrouping || this.Data.Owner.EnableGrouping) &&
                (this.Data.Owner.Groups.Count > 0) &&
                this.Data.Owner.FullRowSelect)
            {
                clientRect.X += this.Data.Owner.GroupIndent;
                clientRect.Width -= this.Data.Owner.GroupIndent;
            }

            base.ArrangeContentCore(clientRect);
        }

        protected override RectangleF GetEditorArrangeRectangle(RectangleF clientRect)
        {
            RectangleF rect = new RectangleF(clientRect.X + this.toggleElement.DesiredSize.Width, clientRect.Y,
                clientRect.Width - this.toggleElement.DesiredSize.Width, clientRect.Height);

            if (rect.Width > this.Data.Owner.ViewElement.ViewElement.DesiredSize.Width)
            {
                rect.Width = this.Data.Owner.ViewElement.ViewElement.DesiredSize.Width - clientRect.X;
            }

            if (this.Data.Owner.ShowGroups &&
            (this.Data.Owner.EnableCustomGrouping || this.Data.Owner.EnableGrouping) &&
            this.Data.Owner.Groups.Count > 0 &&
            !this.Data.Owner.FullRowSelect)
            {
                rect.Width -= this.Data.Owner.GroupIndent;
            }

            return rect;
        }
    }
}
