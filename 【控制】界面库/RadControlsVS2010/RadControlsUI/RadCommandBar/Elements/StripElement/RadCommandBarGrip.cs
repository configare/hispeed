using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Paint;
using System.Diagnostics;
using System.ComponentModel;
using Telerik.WinControls.Styles;

namespace Telerik.WinControls.UI
{
    public class RadCommandBarGrip : RadCommandBarVisualElement
    {

        public static RoutedEvent BeginDraggingEvent =
            RegisterRoutedEvent("BeginDraggingEvent", typeof(RadToolStripGripElement));

        public static RoutedEvent EndDraggingEvent =
            RegisterRoutedEvent("EndDraggingEvent", typeof(RadToolStripGripElement));

        public static RoutedEvent DraggingEvent =
            RegisterRoutedEvent("DraggingEvent", typeof(RadToolStripGripElement));
         

        #region Consts

        private const int defaultNumberOfDots = 4;

        #endregion

        #region Fields
        protected CommandBarStripElement owner;
        private float currentX;
        private float currentY; 
        private bool isDragging = false; 
        private float dotSize = 2F;
        private float dotSpacing = 2F;
        private float shadowOffset = 0.1F;
        private int cachedNumberOfDots = defaultNumberOfDots;
        
        private PointF delta = PointF.Empty;
        //used to set the delta
        private PointF oldMouseLocation = PointF.Empty;
        //used in the beginning of drag
        private Point oldMousePosition;
        #endregion

        #region CStors

        static RadCommandBarGrip()
        {
            ItemStateManagerFactoryRegistry.AddStateManagerFactory(new ItemStateManagerFactory(), typeof(RadCommandBarGrip));
        }

        public RadCommandBarGrip(CommandBarStripElement owner)
        {
            this.owner = owner;
        }

        #endregion

        #region Overrides

        protected override void CreateChildElements()
        {
            base.CreateChildElements();
            this.MinSize = new Size(5, 25);
            this.DrawBorder = false;
            this.DrawFill = false;
            this.NumberOfDots = 0;

        }

        protected override void PaintElement(IGraphics graphics, float angle, SizeF scale)
        {
            base.PaintElement(graphics, angle, scale);
            this.PaintDots(graphics, angle, scale);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseHover(e);
            Cursor.Current = Cursors.SizeAll;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);          
        }

        protected override void OnMouseMove(MouseEventArgs e)
        { 
            base.OnMouseMove(e);
            
            Cursor.Current = Cursors.SizeAll;
            if (!this.isDragging && e.Button == MouseButtons.Left && this.IsMouseDown &&
                (Math.Abs(e.X - oldMousePosition.X) >= 1 || Math.Abs(e.Y - oldMousePosition.Y)>= 1))
            { 
                this.BeginDrag(e);
            }

            if (this.isDragging)
            {
                this.Capture = true;
                this.ElementTree.Control.Capture = true;
                PointF location = (this.orientation == System.Windows.Forms.Orientation.Horizontal)?
                    new PointF(e.X - currentX, e.Y):
                    new PointF( e.X, e.Y - currentY);
                CommandBarRowElement parentRow = (this.owner.Parent as CommandBarRowElement);
                if (parentRow != null && parentRow.RightToLeft && parentRow.Orientation == System.Windows.Forms.Orientation.Horizontal)
                {
                    location.X = parentRow.Size.Width - location.X;
                }

                this.owner.DesiredLocation = location;
                this.delta = new PointF(location.X - oldMouseLocation.X, location.Y - oldMouseLocation.Y);
                this.oldMouseLocation = location; 
                this.OnDragging(e);
            }
        }
         

        
         
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.oldMousePosition = e.Location;
            //this.BeginDrag(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (this.isDragging)
            {
                this.EndDrag();
            }
        }

        #endregion

        #region Dependency properties
        public static RadProperty NumberOfDotsProperty = RadProperty.Register(
            "NumberOfDots", typeof(int), typeof(RadCommandBarGrip), new RadElementPropertyMetadata(
                defaultNumberOfDots, ElementPropertyOptions.AffectsDisplay));

        #endregion

        #region Properties
         
        /// <summary>
        /// Gets the delta of the drag.
        /// </summary>
        public PointF Delta
        {
            get
            {
                return delta;
            } 
        }

        /// <summary>
        /// Gets whether the item is being dragged.
        /// </summary>
        public bool IsDrag
        {
            get
            {
                return isDragging;
            } 
        }

        /// <summary>
        /// Gets or sets the orientation of the grip element.
        /// </summary>
        public override Orientation Orientation
        {
            get
            {
                return base.Orientation;
            }
            set
            {
                this.orientation = value;
                this.AngleTransform = (value == Orientation.Horizontal) ? 0f : 90f;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="CommandBarStripElement"/> that owns the grip element.
        /// </summary>
        public CommandBarStripElement Owner
        {
            get { return this.owner; }
            set { this.owner = value; }
        }

        /// <summary>
        /// Gets or sets the size of the painted dots.
        /// </summary>
        public float DotSize
        {
            get { return dotSize; }
            set { dotSize = value; }
        }

        /// <summary>
        /// Gets or sets the space between dots.
        /// </summary>
        public float DotSpacing
        {
            get { return dotSpacing; }
            set { dotSpacing = value; }

        }

        /// <summary>
        /// Gets or sets the shadow offset of the dots.
        /// </summary>
        public float ShadowOffset
        {
            get { return shadowOffset; }
            set { shadowOffset = value; }

        }

        /// <summary>
        /// Gets or sets the number of dots.
        /// </summary>
        public virtual int NumberOfDots
        {
            get
            {
                return this.cachedNumberOfDots;
            }
            set
            {
                this.cachedNumberOfDots = value;
                SetValue(NumberOfDotsProperty, value);
            }
        }

        #endregion
         
        #region Events management
        
        /// <summary>
        /// Raises a bubble event to notify its parents about the beginning of a drag.
        /// </summary>
        /// <param name="args">A <see cref="T:System.ComponentModel.CancelEventArgs"/> that contains the
        /// event data.</param>
        /// <returns>true if the drag should be canceled, false otherwise.</returns>
        protected virtual bool OnBeginDragging(CancelEventArgs args)
        { 
            if (!args.Cancel)
            {
                RoutedEventArgs routedCancelEventArgs =
                                        new RoutedEventArgs(args, BeginDraggingEvent); 
                this.RaiseBubbleEvent(this.owner, routedCancelEventArgs);
            }
            return args.Cancel;
        }

        /// <summary>
        /// Raises a bubble event to notify its parents about the end of a drag.
        /// </summary>
        /// <param name="args">A <see cref="T:System.EventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnEndDragging(EventArgs args)
        { 
            RoutedEventArgs routedEventArgs =
                                        new RoutedEventArgs(args, EndDraggingEvent);
            this.RaiseBubbleEvent(this, routedEventArgs);
        }

        /// <summary>
        /// Raises a bubble event to notify its parents about the drag.
        /// </summary>
        /// <param name="args">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the
        /// event data.</param>
        protected virtual void OnDragging(MouseEventArgs args)
        { 
            RoutedEventArgs routedMouseEventArgs =
                                        new RoutedEventArgs(args, DraggingEvent);
            this.RaiseBubbleEvent(this, routedMouseEventArgs);
        }
         
        #endregion

        #region Helpers

        /// <summary>
        /// Paints the dots of the grip element.
        /// </summary>
        /// <param name="g">The IGraphics object where the element should be painted.</param>
        /// <param name="angle">The angle under which the element should be painted.</param>
        /// <param name="scale">The factor of scaling the element.</param>
        protected virtual void PaintDots(IGraphics g, float angle, SizeF scale)
        {
            if (this.NumberOfDots == 0)
            {
                return;
            }

            float estimatedHeight = this.NumberOfDots * this.DotSize + (this.NumberOfDots - 1) * this.DotSpacing;
            float startY = (float)Math.Floor((this.DesiredSize.Height - estimatedHeight) / 2);
            float startX = (float)Math.Floor((this.DesiredSize.Width - this.DotSize) / 2);

            RectangleF dotRect = new RectangleF(startX, startY, this.DotSize, this.DotSize);

            for (int i = 0; i < this.NumberOfDots; i++)
            {
                RectangleF shadowRect = new RectangleF(dotRect.X + this.ShadowOffset, dotRect.Y + this.ShadowOffset, this.DotSize, this.DotSize);

                g.FillRectangle(shadowRect, BackColor2);
                g.FillRectangle(dotRect, BackColor);
                dotRect.Y += this.DotSpacing + this.DotSize;
            }
        }

        #endregion

        #region Drag and Drop helpers
     
        protected internal void BeginDrag(MouseEventArgs e)
        {
            if (!this.Owner.EnableDragging)
            {
                return;
            }

            if(this.OnBeginDragging(new CancelEventArgs()))
            {
                return;
            } 
            this.isDragging = true;
            this.Capture = true;
            this.isDragging = true;

            if (this.RightToLeft)
            {
                this.currentX = e.X - (owner.ControlBoundingRectangle.Location.X+owner.ControlBoundingRectangle.Width);
                this.currentY = e.Y - owner.ControlBoundingRectangle.Location.Y;
            }
            else
            {
                this.currentX = e.X - owner.ControlBoundingRectangle.Location.X;
                this.currentY = e.Y - owner.ControlBoundingRectangle.Location.Y;
            }
        }

        protected internal void EndDrag()
        {
            this.Capture = false;
            this.isDragging = false;
            this.OnEndDragging(new EventArgs());
        }
        #endregion
    }
}
