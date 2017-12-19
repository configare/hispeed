using System;
using System.Collections.Generic;
using System.Text;
using Telerik.WinControls.Primitives;
using System.Windows.Forms;
using Telerik.WinControls.Layouts;
using System.Drawing;
using System.ComponentModel;
using Telerik.WinControls.Design;
using System.Drawing.Drawing2D;

namespace Telerik.WinControls.UI
{
    public enum SplitterCollapsedState
    {
        None,
        Previous,
        Next
    }

    public class SplitterElement : RadElement
    {
        #region Nested Types

        class SplitterLayout : LayoutPanel
        {
            private SplitterElement owner;

            public SplitterLayout(SplitterElement owner)
            {
                this.owner = owner;
            }

            protected override SizeF MeasureOverride(SizeF availableSize)
            {
                availableSize = new SizeF(this.owner.SplitterWidth, this.owner.SplitterWidth);

                if (this.Children.Count == 0)
                {
                    return base.MeasureOverride(availableSize);
                }

                for (int i = 0; i < this.Children.Count; i++)
                {
                    int splitterThumbHeight = this.owner.ThumbLength;
                    if (this.Children[i].MinSize.Height != 0)
                    {
                        splitterThumbHeight = this.Children[i].MinSize.Height;
                    }

                    SizeF buttonSize = (this.owner.Dock == DockStyle.Left || this.owner.Dock == DockStyle.Right) ? new SizeF(availableSize.Width, splitterThumbHeight) :
                        new SizeF(splitterThumbHeight, availableSize.Height);

                    this.Children[i].Measure(buttonSize);
                }

                return availableSize;
            }

            protected override SizeF ArrangeOverride(SizeF finalSize)
            {
                if (this.Children.Count == 0)
                {
                    return base.ArrangeOverride(finalSize);
                }

                int len = 0;
                for (int i = 0; i < this.Children.Count; i++)
                {
                    if (Children[i].Visibility == ElementVisibility.Visible)
                    {
                        len += this.owner.ThumbLength;
                    }
                }

                float start = (this.owner.Dock == DockStyle.Left || this.owner.Dock == DockStyle.Right) ? (finalSize.Height - len) / 2 : (finalSize.Width - len) / 2;
                for (int i = 0; i < this.Children.Count; i++)
                {
                    if (this.Children[i].Visibility != ElementVisibility.Visible) continue;

                    if (this.owner.Dock == DockStyle.Left || this.owner.Dock == DockStyle.Right)
                    {
                        this.Children[i].Arrange(new RectangleF(0, start, finalSize.Width, this.owner.ThumbLength));
                    }
                    else
                    {
                        this.Children[i].Arrange(new RectangleF(start, 0, this.owner.ThumbLength, finalSize.Height));
                    }

                    start += this.owner.ThumbLength;
                }

                return finalSize;
            }
        }

        #endregion

        #region Fields

        private const int DefaultThumbLength = 50;
        private RadButtonElement prevSplitterButton;
        private ArrowPrimitive prevSplitterArrow;
        private FillPrimitive backgroundFill;
       
        private RadButtonElement nextSplitterButton;
        private ArrowPrimitive nextSplitterArrow;
        private int thumbLength = DefaultThumbLength;
        private object left;
        private object right;
        private bool fixedSplitter;

        #endregion

        #region Constructor/Initialization

        protected override void CreateChildElements()
        {
            base.CreateChildElements();

            this.backgroundFill = new FillPrimitive();
            this.backgroundFill.Class = "SplitterFill";
            this.backgroundFill.GradientStyle = GradientStyles.Solid;
            this.backgroundFill.SmoothingMode = SmoothingMode.Default;
            this.Children.Add(this.backgroundFill);

            SplitterLayout layout = new SplitterLayout(this);
            this.Children.Add(layout);

            prevSplitterButton = new RadButtonElement();
            prevSplitterButton.Class = "SplitterThumb";
            layout.Children.Add(prevSplitterButton);

            FillPrimitive prevThumbFill = new FillPrimitive();
            prevThumbFill.Class = "SplitterThumbFill";
            prevSplitterButton.Children.Add(prevThumbFill);

            prevSplitterArrow = new ArrowPrimitive();
            prevSplitterArrow.MinSize = new Size(4, 4);
            prevSplitterArrow.Direction = ArrowDirection.Left;
            prevSplitterArrow.Alignment = ContentAlignment.MiddleCenter;
            prevSplitterArrow.Class = "SplitterThumbArrow";
            prevSplitterButton.Children.Add(prevSplitterArrow);

            BorderPrimitive prevThumbBorder = new BorderPrimitive();
            prevThumbBorder.Class = "SplitterThumbBorder";
            prevSplitterButton.Children.Add(prevThumbBorder);

            nextSplitterButton = new RadButtonElement();
            nextSplitterButton.Class = "SplitterThumb";
            layout.Children.Add(nextSplitterButton);

            FillPrimitive nextThumbFill = new FillPrimitive();
            nextThumbFill.Class = "SplitterThumbFill";
            nextSplitterButton.Children.Add(nextThumbFill);

            nextSplitterArrow = new ArrowPrimitive();
            nextSplitterArrow.MinSize = new Size(4, 4);
            nextSplitterArrow.Direction = ArrowDirection.Right;
            nextSplitterArrow.Alignment = ContentAlignment.MiddleCenter;
            nextSplitterArrow.Class = "SplitterThumbArrow";
            nextSplitterButton.Children.Add(nextSplitterArrow);

            BorderPrimitive nextThumbBorder = new BorderPrimitive();
            nextThumbBorder.Class = "SplitterThumbBorder";
            nextSplitterButton.Children.Add(nextThumbBorder);
        }

        #endregion

        #region Rad Properties

        public static RadProperty DockProperty = RadProperty.Register(
            "Dock", typeof(DockStyle), typeof(SplitterElement), new RadElementPropertyMetadata(
                DockStyle.Left, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        public static RadProperty SplitterWidthProperty = RadProperty.Register(
            "SplitterWidth", typeof(int), typeof(SplitterElement), new RadElementPropertyMetadata(
                4, ElementPropertyOptions.AffectsLayout | ElementPropertyOptions.InvalidatesLayout));

        #endregion

        #region CLR Properties

        [Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("Dock", typeof(DockStyle))]
        [RadPropertyDefaultValue("Dock", typeof(SplitterElement))]
        public DockStyle Dock
        {
            get
            {
                return (DockStyle)this.GetValue(DockProperty);
            }
            set
            {
                this.SetValue(DockProperty, value);
            }
        }

        [Browsable(true), Category(RadDesignCategory.BehaviorCategory)]
        [RadDescription("SplitterWidth", typeof(int))]
        [RadPropertyDefaultValue("SplitterWidth", typeof(SplitterElement))]
        public int SplitterWidth
        {
            get
            {
                return (int)this.GetValue(SplitterWidthProperty);
            }
            set
            {
                this.SetValue(SplitterWidthProperty, value);
            }
        }

        public bool Fixed
        {
            get { return fixedSplitter; }
            set { fixedSplitter = value; }
        }

        public object RightNode
        {
            get { return right; }
            set { right = value; }
        }

        public object LeftNode
        {
            get { return left; }
            set { left = value; }
        }

        public RadItem NextNavigationButton
        {
            get { return nextSplitterButton; }
        }

        public RadItem PrevNavigationButton
        {
            get { return prevSplitterButton; }
        }

        public ArrowPrimitive PrevArrow
        {
            get { return prevSplitterArrow; }
        }

        public ArrowPrimitive NextArrow
        {
            get { return nextSplitterArrow; }
        }

        [Browsable(false)]
        public FillPrimitive BackgroundFill
        {
            get
            {
                return this.backgroundFill;
            }
        }

        public int ThumbLength
        {
            get { return thumbLength; }
            set { thumbLength = value; }
        }

        #endregion

        #region Overrides

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.Property == DockProperty)
            {
                if (this.Dock == DockStyle.Left || this.Dock == DockStyle.Right)
                {
                    prevSplitterArrow.Direction = ArrowDirection.Left;
                    nextSplitterArrow.Direction = ArrowDirection.Right;
                }
                else
                {
                    prevSplitterArrow.Direction = ArrowDirection.Down;
                    nextSplitterArrow.Direction = ArrowDirection.Up;
                }
            }
        }

        #endregion
    }
}
