using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Paint;

namespace Telerik.WinControls.UI
{
    public class TreeNodeLineElement : TreeViewVisual
    {
        #region Nested Types

        /// <summary>
        /// Defines the differen link styles
        /// </summary>
        public enum LinkType
        {
            VerticalLine,
            HorizontalLine,
            RightBottomAngleShape,
            LeftTopAngleShape,
            TShape,
            RightTopAngleShape
        }

        #endregion

        #region Fields

        private Size arrowSize;
        private TreeNodeElement nodeElement;

        #endregion

        #region Dependency Properties

        public static RadProperty TypeProperty = RadProperty.Register("Type", typeof(LinkType), typeof(TreeNodeLineElement),
            new RadElementPropertyMetadata(LinkType.VerticalLine, ElementPropertyOptions.AffectsDisplay));

        public static RadProperty LineStyleProperty = RadProperty.Register("LineStyle", typeof(DashStyle), typeof(TreeNodeLineElement),
            new RadElementPropertyMetadata(DashStyle.Solid, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes a new instance of the <see cref="GridLinkItem"/> class.
        /// </summary>
        public TreeNodeLineElement()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeNodeLineElement"/> class.
        /// </summary>
        /// <param name="nodeElement">The table element.</param>
        public TreeNodeLineElement(TreeNodeElement nodeElement)
        {
            this.nodeElement = nodeElement;
        }

        protected override void InitializeFields()
        {
            base.InitializeFields();

            this.BypassLayoutPolicies = true;
            this.ShouldHandleMouseInput = false;
            this.StretchHorizontally = false;
            this.StretchVertically = true;
            this.ShouldPaint = true;
            this.FitToSizeMode = RadFitToSizeMode.FitToParentBounds;
            this.LineStyle = DashStyle.Dot;
            this.ForeColor = Color.Gray;
        }

        #endregion

        #region Properites

        /// <summary>
        /// Gets or sets the size of the arrow. Used to calculate pixel-perfect results.
        /// </summary>
        internal Size ArrowSize
        {
            get
            {
                return this.arrowSize;
            }
            set
            {
                this.arrowSize = value;
            }
        }

        /// <summary>
        /// Gets or sets a value determining the shape of the link
        /// </summary>
        public LinkType Type
        {
            get { return (LinkType)this.GetValue(TypeProperty); }
            set { this.SetValue(TypeProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value determining the style of the link lines
        /// </summary>
        public DashStyle LineStyle
        {
            get { return (DashStyle)this.GetValue(LineStyleProperty); }
            set { this.SetValue(LineStyleProperty, value); }
        }

        #endregion

        #region Layout

        protected override SizeF MeasureOverride(SizeF availableSize)
        {
            SizeF desiredSize = base.MeasureOverride(availableSize);

            if (this.nodeElement != null)
            {
                desiredSize.Width = this.nodeElement.TreeViewElement.TreeIndent;
            }

            return desiredSize;
        }

        #endregion

        #region Paint

        protected override void PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
            base.PaintElement(graphics, angle, scale);

            switch (this.Type)
            {
                case LinkType.VerticalLine:
                    this.PaintVerticalLine(graphics);
                    break;
                case LinkType.HorizontalLine:
                    this.PaintHorizontalLine(graphics);
                    break;
                case LinkType.RightBottomAngleShape:
                    this.PaintRightBottomAngleShape(graphics);
                    break;
                case LinkType.LeftTopAngleShape:
                    this.PaintLeftTopAngleShape(graphics);
                    break;
                case LinkType.TShape:
                    this.PaintTShape(graphics);
                    break;
                case LinkType.RightTopAngleShape:
                    this.PaintRightTopAngleShape(graphics);
                    break;
            }
        }

        protected virtual void PaintHorizontalLine(IGraphics graphics)
        {
            int x = 0;
            int y = this.Size.Height / 2;
            graphics.DrawLine(this.ForeColor, this.LineStyle, x, y, this.Size.Width, y);
        }

        protected virtual void PaintVerticalLine(IGraphics graphics)
        {
            Size size = this.Size;
            int x = size.Width / 2;
            int y = 0;
            graphics.DrawLine(this.ForeColor, this.LineStyle, x, y, x, size.Height);
        }

        protected virtual void PaintRightBottomAngleShape(IGraphics graphics)
        {
            Size size = this.Size;
            int x = size.Width / 2;
            int y = size.Height / 2;

            graphics.DrawLine(this.ForeColor, this.LineStyle, x, 0, x, y);

            if (this.RightToLeft)
            {
                graphics.DrawLine(this.ForeColor, this.LineStyle, 0, y, x, y);
            }
            else
            {
                graphics.DrawLine(this.ForeColor, this.LineStyle, x, y, size.Width - 1, y);
            }
        }

        protected virtual void PaintRightTopAngleShape(IGraphics graphics)
        {
            Size size = this.Size;
            int x = size.Width / 2;
            int y = size.Height / 2;

            if (this.RightToLeft)
            {
                graphics.DrawLine(this.ForeColor, this.LineStyle, size.Width / 2, y, size.Width / 2, size.Height);
                graphics.DrawLine(this.ForeColor, this.LineStyle, 0, y, size.Width / 2 - 1, y);
            }
            else
            {
                graphics.DrawLine(this.ForeColor, this.LineStyle, x, y, x, size.Height);
                graphics.DrawLine(this.ForeColor, this.LineStyle, x + 1, y, size.Width, y);
            }
        }

        protected virtual void PaintLeftTopAngleShape(IGraphics graphics)
        {
            Size size = this.Size;
            int arrowFix = 0;
            if (size.Width % 2 > 0)
            {
                arrowFix = (size.Width - this.arrowSize.Width) % 2;
            }
            int x = size.Width / 2 + arrowFix;
            int y = size.Height / 2;

            graphics.DrawLine(this.ForeColor, this.LineStyle, x, y, x, size.Height - 1);

            if (this.RightToLeft)
            {
                graphics.DrawLine(this.ForeColor, this.LineStyle, x, y, size.Width - 1, y);
            }
            else
            {
                graphics.DrawLine(this.ForeColor, this.LineStyle, x, y, 0, y);
            }
        }

        protected virtual void PaintTShape(IGraphics graphics)
        {
            Size size = this.Size;
            int x = size.Width / 2;
            int y = size.Height / 2;

            if (this.RightToLeft)
            {
                //graphics.DrawLine(this.ForeColor, this.LineStyle, size.Width, 0, size.Width, size.Height);
                //graphics.DrawLine(this.ForeColor, this.LineStyle, x, y, size.Width - 1, y);

                graphics.DrawLine(this.ForeColor, this.LineStyle, size.Width / 2, 0, size.Width / 2, size.Height);
                graphics.DrawLine(this.ForeColor, this.LineStyle, 0, y, size.Width / 2 - 1, y);
            }
            else
            {
                graphics.DrawLine(this.ForeColor, this.LineStyle, x, 0, x, size.Height);
                graphics.DrawLine(this.ForeColor, this.LineStyle, x + 1, y, size.Width, y);
            }
        }

        #endregion
    }
}
