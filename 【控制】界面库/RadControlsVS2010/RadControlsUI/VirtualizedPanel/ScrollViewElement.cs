using System;
using System.Drawing;
using System.Windows.Forms;

namespace Telerik.WinControls.UI
{
    public class ScrollViewElement<T> : LightVisualElement where T : RadElement, new()
    {
        RadScrollBarElement hscrollBar;
        RadScrollBarElement vscrollBar;
        T viewElement;

        #region Initialization

        protected override void InitializeFields()
        {
            base.InitializeFields();
            this.StretchHorizontally = true;
            this.StretchVertically = true;
        }

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.viewElement = this.CreateViewElement();
            this.InitializeViewElement(this.viewElement);

            this.Children.Add(viewElement);

            this.hscrollBar = this.CreateScrollBarElement();
            this.hscrollBar.ScrollType = ScrollType.Horizontal;
            this.hscrollBar.MinSize = new Size(0, SystemInformation.HorizontalScrollBarHeight);
            this.hscrollBar.ScrollTimerDelay = 1;
            this.Children.Add(hscrollBar);

            this.vscrollBar = this.CreateScrollBarElement();
            this.vscrollBar.ScrollType = ScrollType.Vertical;
            this.vscrollBar.MinSize = new Size(SystemInformation.VerticalScrollBarWidth, 0);
            this.vscrollBar.ScrollTimerDelay = 1;
            this.Children.Add(vscrollBar);
        }

        protected virtual RadScrollBarElement CreateScrollBarElement()
        {
            return new RadScrollBarElement();
        }

        protected virtual T CreateViewElement()
        {
            return new T();
        }

        /// <summary>
        /// This method provides a chance to initialize the ViewElement object.
        /// </summary>
        /// <param name="viewElement"></param>
        protected virtual void InitializeViewElement(T viewElement)
        {

        }

        #endregion

        #region Properties

        public RadScrollBarElement HScrollBar
        {
            get { return this.hscrollBar; }
        }

        public RadScrollBarElement VScrollBar
        {
            get { return this.vscrollBar; }
        }

        public T ViewElement
        {
            get
            {
                return this.viewElement;
            }

            set
            {
                if (this.viewElement == value)
                {
                    return;
                }

                this.viewElement = value;
            }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            RectangleF clientRect = GetClientRectangle(availableSize);
            this.layoutManagerPart.Measure(clientRect.Size);
            SizeF desiredSize = this.MeasureView(clientRect.Size);
            return desiredSize;
        }

        protected virtual SizeF MeasureView(SizeF availableSize)
        {
            Padding clientOffset = this.GetClientOffset(true);
            SizeF desiredSize = new SizeF(clientOffset.Horizontal, clientOffset.Vertical);

            ElementVisibility oldHScrollbarVisibility = this.hscrollBar.Visibility;

            if (hscrollBar.Visibility != ElementVisibility.Collapsed)
            {
                hscrollBar.Measure(availableSize);
                desiredSize.Height += hscrollBar.DesiredSize.Height;
                availableSize.Height -= hscrollBar.DesiredSize.Height;
            }

            SizeF hscrollbarDesiredSize = hscrollBar.DesiredSize;

            ElementVisibility oldVScrollbarVisibility = this.vscrollBar.Visibility;

            if (vscrollBar.Visibility != ElementVisibility.Collapsed)
            {
                vscrollBar.Measure(availableSize);
                desiredSize.Width += vscrollBar.DesiredSize.Width;
                availableSize.Width -= vscrollBar.DesiredSize.Width;
            }

            SizeF vscrollbarDesiredSize = vscrollBar.DesiredSize;

            viewElement.Measure(availableSize);

            bool doubleMeasure = false;

            if (oldHScrollbarVisibility != this.hscrollBar.Visibility)
            {
                if (oldHScrollbarVisibility == ElementVisibility.Visible)
                {
                    desiredSize.Height -= hscrollbarDesiredSize.Height;
                    availableSize.Height += hscrollbarDesiredSize.Height;
                }
                else
                {
                    hscrollBar.Measure(availableSize);
                    desiredSize.Height += hscrollBar.DesiredSize.Height;
                    availableSize.Height -= hscrollBar.DesiredSize.Height;
                }

                doubleMeasure = true;
            }

            if (oldVScrollbarVisibility != this.vscrollBar.Visibility)
            {
                if (oldVScrollbarVisibility == ElementVisibility.Visible)
                {
                    desiredSize.Width -= vscrollbarDesiredSize.Width;
                    availableSize.Width += vscrollbarDesiredSize.Width;
                }
                else
                {
                    vscrollBar.Measure(availableSize);
                    desiredSize.Width += vscrollBar.DesiredSize.Width;
                    availableSize.Width -= vscrollBar.DesiredSize.Width;
                }

                doubleMeasure = true;
            }

            if (doubleMeasure)
            {
                viewElement.Measure(availableSize);
            }

            desiredSize.Width += viewElement.DesiredSize.Width;
            desiredSize.Height += viewElement.DesiredSize.Height;

            return desiredSize;
        }

        protected override SizeF ArrangeOverride(SizeF finalSize)
        {
            RectangleF clientRect = this.GetClientRectangle(finalSize);
            RectangleF viewElementRect = new RectangleF(clientRect.X, clientRect.Y, clientRect.Width, clientRect.Height);

            this.layoutManagerPart.Arrange(clientRect);
            RectangleF hscrollBarRect = ArrangeHScrollBar(ref viewElementRect, clientRect);
            ArrangeVScrollBar(ref viewElementRect, hscrollBarRect, clientRect);

            viewElementRect.Width = Math.Max(1, viewElementRect.Width);
            viewElementRect.Height = Math.Max(1, viewElementRect.Height);

            viewElement.Arrange(viewElementRect);

            return finalSize;
        }

        protected virtual RectangleF ArrangeHScrollBar(ref RectangleF viewElementRect, RectangleF clientRect)
        {
            RectangleF hscrollBarRect = RectangleF.Empty;

            if (hscrollBar.Visibility != ElementVisibility.Collapsed)
            {
                int scrollbarHeight = (int)hscrollBar.DesiredSize.Height;
                if (scrollbarHeight == 0)
                {
                    scrollbarHeight = SystemInformation.HorizontalScrollBarHeight;
                }
                float y = clientRect.Bottom - scrollbarHeight;
                float width = clientRect.Width - vscrollBar.DesiredSize.Width;
                hscrollBarRect = new RectangleF(clientRect.X, y, width, scrollbarHeight);
                if (RightToLeft && vscrollBar.Visibility != ElementVisibility.Collapsed)
                {
                    hscrollBarRect.X += vscrollBar.DesiredSize.Width;
                }
                hscrollBar.Arrange(hscrollBarRect);
                viewElementRect.Height -= hscrollBar.DesiredSize.Height;
            }

            return hscrollBarRect;
        }

        protected virtual void ArrangeVScrollBar(ref RectangleF viewElementRect, RectangleF hscrollBarRect, RectangleF clientRect)
        {
            if (vscrollBar.Visibility != ElementVisibility.Collapsed)
            {
                int scrollbarWidth = (int)vscrollBar.DesiredSize.Width;
                if (scrollbarWidth == 0)
                {
                    scrollbarWidth = SystemInformation.VerticalScrollBarWidth;
                }

                float x = clientRect.Right - scrollbarWidth;
                float height = clientRect.Height - hscrollBarRect.Height;
                RectangleF vscrollBarRect = new RectangleF(x, clientRect.Y, scrollbarWidth, height);
                if (RightToLeft)
                {
                    vscrollBarRect.X = clientRect.X;
                    viewElementRect.X += scrollbarWidth;
                }
                vscrollBar.Arrange(vscrollBarRect);
                viewElementRect.Width -= scrollbarWidth;
            }
        }

        #endregion
    }
}
