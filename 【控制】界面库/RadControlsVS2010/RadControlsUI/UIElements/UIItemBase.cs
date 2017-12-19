using System.Drawing;
using Telerik.WinControls.Layouts;
using Telerik.WinControls.Paint;
using Telerik.WinControls.Primitives;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// Base class for some RadItems, used in RadTreeView, RadPanelBar, RadCalendar, etc. Incorporates basic functionality for paiting gradient 
    /// background and borders the same way FillPrimitive and BorderPrimitive do.
    /// </summary>
    public abstract class UIItemBase : RadItem, IPrimitiveElement, IFillElement, IBorderElement
    {
        private FillPrimitiveImpl fillPrimitiveImpl;
        private BorderPrimitiveImpl borderPrimitiveImpl;

        //keep consistency in BitState keys declaration
        internal const ulong UIItemBaseLastStateKey = RadItemLastStateKey;

        protected UIItemBase()
        {
        }

        internal FillPrimitiveImpl FillPrimitiveImpl
        {
            get
            {
                if (this.fillPrimitiveImpl == null)
                {
                    this.fillPrimitiveImpl = new FillPrimitiveImpl(this, this);
                }

                return fillPrimitiveImpl;
            }
        }

        internal BorderPrimitiveImpl BorderPrimitiveImpl
        {
            get
            {
                if (this.borderPrimitiveImpl == null)
                {
                    this.borderPrimitiveImpl = new BorderPrimitiveImpl(this, this);
                }

                return this.borderPrimitiveImpl;
            }
        }

        protected virtual void PaintFill(IGraphics graphics, float angle, SizeF scale)
        {
            RectangleF rect = this.GetFillPaintRect(angle, scale);
            this.PaintFill(graphics, angle, scale, rect);
        }

        protected virtual RectangleF GetFillPaintRect(float angle, SizeF scale)
        {
            Size size = this.Size;
            RectangleF rect = new RectangleF(0, 0, size.Width, size.Height);

            return this.GetPatchedRect(rect, angle, scale);
        }

        protected virtual void PaintFill(IGraphics graphics, float angle, SizeF scale, RectangleF prefferedRectangle)
        {
            this.FillPrimitiveImpl.PaintFill(graphics, angle, scale, prefferedRectangle);
        }

        protected virtual void PaintBorder(IGraphics graphics, float angle, SizeF scale)
        {
            RectangleF rect = this.GetBorderPaintRect(angle, scale);
            this.PaintBorder(graphics, angle, scale, rect);
        }

        protected virtual RectangleF GetBorderPaintRect(float angle, SizeF scale)
        {
            float leftTopBorderWidth = 0;
            float bottomRightBorderWidth = 0;

            if (this.DrawBorder)
            {
                float border = this.BorderWidth;
                int halfWidth = (int)border / 2;
                float gdiBottomRightFix = border % 2;

                leftTopBorderWidth = halfWidth;
                bottomRightBorderWidth = halfWidth + gdiBottomRightFix;
            }

            Size size = this.Size;
            RectangleF rect = new RectangleF(leftTopBorderWidth, leftTopBorderWidth, size.Width - (leftTopBorderWidth + bottomRightBorderWidth), size.Height - (leftTopBorderWidth + bottomRightBorderWidth));

            return this.GetPatchedRect(rect, angle, scale);
        }

        protected virtual void PaintBorder(IGraphics graphics, float angle, SizeF scale, RectangleF preferedRectangle)
        {
            this.BorderPrimitiveImpl.PaintBorder(graphics, angle, scale, preferedRectangle);
        }

        protected virtual bool ShouldUsePaintBuffer()
        {
            return this.DrawFill;
        }

        public abstract bool DrawFill
        {
            get;
            set;
        }

        public abstract bool DrawBorder
        {
            get;
            set;
        }

        public virtual float GetPaintingBorderWidth()
        {
            if (this.Parent != null)
            {
                return this.Parent.BorderThickness.Left;
            }

            return 0;
        }

        protected override void OnPropertyChanged(RadPropertyChangedEventArgs e)
        {
            this.FillPrimitiveImpl.InvalidateFillCache(e.Property);
            base.OnPropertyChanged(e);
        }

        protected override void OnBoundsChanged(RadPropertyChangedEventArgs e)
        {
            this.FillPrimitiveImpl.OnBoundsChanged((Rectangle)e.OldValue);
            base.OnBoundsChanged(e);
        }

        #region IPrimitiveElement Members

        //Interface methods implemetned implicitly
        //Size IPrimitiveElement.Size
        //{
        //    get { return this.Size; }
        //}

        //RadElement IPrimitiveElement.Parent
        //{
        //    get { throw new NotImplementedException(); }
        //}

        //RadElementTree IPrimitiveElement.ElementTree
        //{
        //    get { throw new NotImplementedException(); }
        //}

        bool IPrimitiveElement.ShouldUsePaintBuffer()
        {
            return this.ShouldUsePaintBuffer();
        }

        bool IPrimitiveElement.IsDesignMode
        {
            get { return this.IsDesignMode; }
        }

        float IPrimitiveElement.BorderThickness
        {
            get
            {
                return this.GetPaintingBorderWidth();
            }
        }

        RectangleF IPrimitiveElement.GetPaintRectangle(float left, float angle, SizeF scale)
        {
            return base.GetPaintRectangle(left, angle, scale);
        }

        RectangleF IPrimitiveElement.GetExactPaintingRectangle(float angle, SizeF scale)
        {
            return base.GetPatchedRect(new RectangleF(0, 0, this.Size.Width - 1, this.Size.Height - 1), angle, scale);
        }

        ElementShape IPrimitiveElement.GetCurrentShape()
        {
            return this.GetCurrentShape();
        }

        #endregion

        #region IFillElement Members


        public abstract Color BackColor2
        {
            get;
            set;
        }

        public abstract Color BackColor3
        {
            get;
            set;
        }

        public abstract Color BackColor4
        {
            get;
            set;
        }

        public abstract int NumberOfColors
        {
            get;
            set;
        }

        public abstract float GradientAngle
        {
            get;
            set;
        }

        public abstract float GradientPercentage
        {
            get;
            set;
        }

        public abstract float GradientPercentage2
        {
            get;
            set;
        }

        public abstract GradientStyles GradientStyle
        {
            get;
            set;
        }

        #endregion

        public abstract Color BorderColor { get; set; }
        public abstract Color BorderColor2 { get; set; }
        public abstract Color BorderColor3 { get; set; }
        public abstract Color BorderColor4 { get; set; }

        public abstract Color BorderInnerColor { get; set; }
        public abstract Color BorderInnerColor2 { get; set; }
        public abstract Color BorderInnerColor3 { get; set; }
        public abstract Color BorderInnerColor4 { get; set; }

        public abstract BorderBoxStyle BorderBoxStyle { get; set; }

        public abstract BorderDrawModes BorderDrawMode { get; set; }

        public abstract GradientStyles BorderGradientStyle { get; set; }

        public abstract float BorderGradientAngle { get; set; }

        public abstract Color BorderLeftColor { get; set; }
        public abstract Color BorderLeftShadowColor { get; set; }

        public abstract Color BorderTopColor { get; set; }
        public abstract Color BorderTopShadowColor { get; set; }

        public abstract Color BorderRightColor { get; set; }
        public abstract Color BorderRightShadowColor { get; set; }

        public abstract Color BorderBottomColor { get; set; }
        public abstract Color BorderBottomShadowColor { get; set; }

        public abstract float BorderLeftWidth { get; set; }
        public abstract float BorderTopWidth { get; set; }
        public abstract float BorderRightWidth { get; set; }
        public abstract float BorderBottomWidth { get; set; }

        #region IBorderElement

        Color IBorderElement.ForeColor
        {
            get { return this.BorderColor; }
        }

        Color IBorderElement.ForeColor2
        {
            get { return this.BorderColor2; }
        }

        Color IBorderElement.ForeColor3
        {
            get { return this.BorderColor3; }
        }

        Color IBorderElement.ForeColor4
        {
            get { return this.BorderColor4; }
        }

        Color IBorderElement.InnerColor
        {
            get { return this.BorderInnerColor; }
        }

        Color IBorderElement.InnerColor2
        {
            get { return this.BorderInnerColor2; }
        }

        Color IBorderElement.InnerColor3
        {
            get { return this.BorderInnerColor3; }
        }

        Color IBorderElement.InnerColor4
        {
            get { return this.BorderInnerColor4; }
        }

        BorderBoxStyle IBorderElement.BoxStyle
        {
            get { return this.BorderBoxStyle; }
        }

        GradientStyles IBorderElement.GradientStyle
        {
            get { return this.BorderGradientStyle; }
        }

        float IBorderElement.GradientAngle
        {
            get { return this.BorderGradientAngle; }
        }
        #endregion

        #region IBoxStyle Members

        Color IBoxStyle.LeftColor
        {
            get
            {
                return this.BorderLeftColor;
            }
            set
            {
            }
        }



        Color IBoxStyle.LeftShadowColor
        {
            get
            {
                return this.BorderLeftShadowColor;
            }
            set
            {
            }
        }

        Color IBoxStyle.TopColor
        {
            get
            {
                return this.BorderTopColor;
            }
            set
            {
            }
        }

        Color IBoxStyle.TopShadowColor
        {
            get
            {
                return this.BorderTopShadowColor;
            }
            set
            {
            }
        }

        Color IBoxStyle.RightColor
        {
            get
            {
                return this.BorderRightColor;
            }
            set
            {
            }
        }

        Color IBoxStyle.RightShadowColor
        {
            get
            {
                return this.BorderRightShadowColor;
            }
            set
            {
            }
        }

        Color IBoxStyle.BottomColor
        {
            get
            {
                return this.BorderBottomColor;
            }
            set
            {
            }
        }

        Color IBoxStyle.BottomShadowColor
        {
            get
            {
                return this.BorderBottomShadowColor;
            }
            set
            {
            }
        }

        #endregion

        #region IBoxElement Members

        float IBoxElement.Width
        {
            get
            {
                return this.BorderWidth;
            }
            set
            {
            }
        }

        public abstract float BorderWidth { get; set; }

        float IBoxElement.LeftWidth
        {
            get
            {
                return this.BorderLeftWidth;
            }
            set
            {
            }
        }

        float IBoxElement.TopWidth
        {
            get
            {
                return this.BorderTopWidth;
            }
            set
            {
            }
        }

        float IBoxElement.RightWidth
        {
            get
            {
                return this.BorderRightWidth;
            }
            set
            {
            }
        }

        float IBoxElement.BottomWidth
        {
            get
            {
                return this.BorderBottomWidth;
            }
            set
            {
            }
        }

        SizeF IBoxElement.Offset
        {
            get
            {
                if (this.BorderBoxStyle == BorderBoxStyle.SingleBorder)
                    return new SizeF(this.BorderWidth, this.BorderWidth);
                else
                    return new SizeF(this.BorderLeftWidth, this.BorderTopWidth);
            }
        }

        SizeF IBoxElement.BorderSize
        {
            get
            {
                IBoxElement me = this as IBoxElement;

                return new SizeF(me.HorizontalWidth, me.VerticalWidth);
            }
        }

        float IBoxElement.HorizontalWidth
        {
            get
            {
                if (this.BorderBoxStyle == BorderBoxStyle.SingleBorder)
                    return 2 * this.BorderWidth;
                else
                    return this.BorderLeftWidth + this.BorderRightWidth;
            }
        }

        float IBoxElement.VerticalWidth
        {
            get
            {
                if (this.BorderBoxStyle == BorderBoxStyle.SingleBorder)
                    return 2 * this.BorderWidth;
                else
                    return this.BorderTopWidth + this.BorderBottomWidth;
            }
        }

        #endregion
    }

}
