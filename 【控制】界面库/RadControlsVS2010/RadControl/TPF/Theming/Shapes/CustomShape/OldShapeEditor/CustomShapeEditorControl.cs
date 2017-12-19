using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace Telerik.WinControls.OldShapeEditor
{
    /// <summary>
    /// Represents a shape editor control.
    /// </summary>
	[ToolboxItemAttribute(false)]
    public partial class RadShapeEditorControl : UserControl
    {
		private float zoomFactor = 1;
		//indicate the size of the invisible area
		private int xOverflowOffset, yOverflowOffset;
		//increment of zoomng
		private const float ZOOM_INCREMENT = 0.2f;
		//min and max zooming factors
		private const float MAX_ZOOM_FACTOR = 2;
		private const float MIN_ZOOM_FACTOR = 1;
		//initial grid field width
		private const int FIELD_WIDTH = 20;
		//scaled size of snapping width
		private int DRAWABLE_GRID_LINE_WIDTH;
		int maxWidth, maxHeight;
		int minWidth, minHeight;
		private int xoff, yoff;
		enum PointTypes { Point, ControlPoint, Line };
		private SnapToGrid snapToGrid;
		public PropertyGrid propertyGrid;
        ShapePoint[] dimensionPoints;
        ShapePointBase point = null;
        
		PointTypes pointType = PointTypes.Point;
        Point downPoint, curPoint, oldCurPoint;
        bool mouseDown = false;
        //if true, the next point selected will be used as a reference to set a symmetry of the previously selected point
		bool isSymmetricPointMode = false;
		bool isVerticalSymmetry = false;
		//denotes the new point that was made symmetric, and a point used as a reference for the symmetry
		ShapePointBase newSymmetricPoint, referencePoint;
        private List<ShapePoint> points = new List<ShapePoint>();
        
        public List<ShapePoint> Points
        {
            get { return points; }
        }

        Rectangle dimension = new Rectangle(10, 10, 100, 100);
        public Rectangle Dimension
        {
            get { return dimension; }
            set 
            { 
                dimension = value;

                int midx = dimension.Left + dimension.Width / 2;
                int midy = dimension.Top + dimension.Height / 2;
            }
        }

        public RadShapeEditorControl()
        {
			snapToGrid = new SnapToGrid();
			xoff = yoff = 0;
			xOverflowOffset = yOverflowOffset = 0;
			maxWidth = this.ClientRectangle.Width;
			maxHeight = this.ClientRectangle.Height;
			minWidth = 0;
			minHeight = 0;
			
			//width of the guide line grid
			snapToGrid.FieldWidth = FIELD_WIDTH;
			DRAWABLE_GRID_LINE_WIDTH = (int)snapToGrid.FieldWidth;
			//maximum distance from a guide line in pixels before snapping fires
			snapToGrid.SnapType = SnapToGrid.SnapTypes.Fixed;
			snapToGrid.SnapFixed = 6;
			newSymmetricPoint = referencePoint = null;
            dimensionPoints = new ShapePoint[4];
            for (int i = 0; i < 4; ++i)
                dimensionPoints[i] = new ShapePoint();

            InitializeComponent();

            base.SetStyle(
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.Opaque |
                ControlStyles.ContainerControl |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.ResizeRedraw |
                ControlStyles.UserPaint, true);

            this.contextMenuPoint.RenderMode = ToolStripRenderMode.Professional;
            ((ToolStripProfessionalRenderer)this.contextMenuPoint.Renderer).ColorTable.UseSystemColors = true;

            this.contextMenuLine.RenderMode = ToolStripRenderMode.Professional;
            ((ToolStripProfessionalRenderer)this.contextMenuLine.Renderer).ColorTable.UseSystemColors = true;

            this.menuItemAddPoint.Click += new EventHandler(menuItemAddPoint_Click);
            this.menuItemConvert.Click += new EventHandler(menuItemConvert_Click);
            this.menuItemRemovePoint.Click += new EventHandler(menuItemRemovePoint_Click);
            this.menuItemRemoveLine.Click += new EventHandler(menuItemRemoveLine_Click);
            this.menuItemAnchorLeft.Click += new EventHandler(menuItemAnchorLeft_Click);
            this.menuItemAnchorRight.Click += new EventHandler(menuItemAnchorRight_Click);
            this.menuItemAnchorTop.Click += new EventHandler(menuItemAnchorTop_Click);
            this.menuItemAnchorBottom.Click += new EventHandler(menuItemAnchorBottom_Click);
            this.menuItemConvertLine.Click += new EventHandler(menuItemConvert_Click);
            this.menuItemLeftTopCorner.Click += new EventHandler(menuItemLeftTopCorner_Click);
            this.menuItemLeftBottomCorner.Click += new EventHandler(menuItemLeftBottomCorner_Click);
            this.menuItemRightTopCorner.Click += new EventHandler(menuItemRightTopCorner_Click);
            this.menuItemRightBottomCorner.Click += new EventHandler(menuItemRightBottomCorner_Click);
            this.menuItemLocked.Click += new EventHandler(menuItemLocked_Click);
        }
		/// <summary>
		/// Draws grid lines in the specified rectangle with the specified color
		/// </summary>
		/// <param name="graphics"></param>
		/// <param name="rect"></param>
		/// <param name="color"></param>
		private void DrawGridLines(Graphics graphics, Rectangle rect, Color color)
		{
			SolidBrush sb = new SolidBrush(color);
			Pen pen = new Pen(sb);
			pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
			//offset of the first line from the left and top sides of the control
			xoff = (int)Math.Round(-xOverflowOffset * zoomFactor) % (int)DRAWABLE_GRID_LINE_WIDTH;
			yoff = (int)Math.Round(-yOverflowOffset * zoomFactor) % (int)DRAWABLE_GRID_LINE_WIDTH;
			
			//draw vertical grid lines
			for (int i = 0; i < rect.Width / DRAWABLE_GRID_LINE_WIDTH; ++i)
				graphics.DrawLine(pen, new Point(xoff + i * (int)DRAWABLE_GRID_LINE_WIDTH, 0), new Point(xoff + i * (int)DRAWABLE_GRID_LINE_WIDTH, rect.Bottom));

			//draw horizontal grid lines
			for (int j = 0; j < rect.Height / DRAWABLE_GRID_LINE_WIDTH; ++j)
				graphics.DrawLine(pen, new Point(0, yoff + j * (int)DRAWABLE_GRID_LINE_WIDTH), new Point(rect.Right, yoff + j * (int)DRAWABLE_GRID_LINE_WIDTH));

		}
		/// <summary>
		/// Translates a rectangle in accordance with the offsets due to scrolling
		/// </summary>
		/// <param name="r"></param>
		/// <returns></returns>
		private Rectangle GetRealFromVirtualRect(Rectangle r)
		{
			return new Rectangle(
				(int)Math.Round((r.X - xOverflowOffset) * zoomFactor),
				(int)Math.Round((r.Y - yOverflowOffset) * zoomFactor),
				(int)Math.Round(r.Width*zoomFactor), 
				(int)Math.Round(r.Height*zoomFactor));
		}
		/// <summary>
		/// Translates a point in accordance with the offsets due to scrolling
		/// </summary>
		/// <param name="pt"></param>
		/// <returns></returns>
		private Point GetRealFromVirtualPoint(ShapePointBase pt)
		{
			return new Point(
				(int)Math.Round((pt.GetPoint().X - xOverflowOffset) * zoomFactor),
				(int)Math.Round((pt.GetPoint().Y - yOverflowOffset) * zoomFactor));
		}
        protected override void OnPaint(PaintEventArgs e)
        {
			//fill the entire area with white
			e.Graphics.FillRectangle(Brushes.White, ClientRectangle);
			
            Rectangle rect = ClientRectangle; rect.Width--; rect.Height--;
            //draw a non-movable rectangle encompassing the drawable area
            e.Graphics.DrawRectangle(SystemPens.ControlDark, rect);

			//initialize the matrix to be used when transforming the coordinate system
			//does scaling the translation in accordance with the positions of the scrollbars
			Matrix transformationMatrix = new Matrix(1, 0, 0, 1, xOverflowOffset - hScrollBar1.Value, yOverflowOffset - vScrollBar1.Value);
			transformationMatrix.Scale(zoomFactor, zoomFactor);
			
			e.Graphics.Transform = transformationMatrix;

			//fill the control area
			e.Graphics.FillRectangle(Brushes.LightGray, GetRealFromVirtualRect(Dimension));

			//draw the guides in the entire area
			DrawGridLines(e.Graphics,
					new Rectangle(0, 0,
						(int)Math.Round((ClientRectangle.Width * 1.5)),//+ ClientRectangle.Width
						(int)Math.Round((ClientRectangle.Height * 1.5)))//+ ClientRectangle.Height
					, Color.Black);
			
            Rectangle r = dimension;
            int midx = r.Left + r.Width / 2;
            int midy = r.Top + r.Height / 2;
            //draw the black rectangle denoting the clipping rectangle of the control area
            e.Graphics.DrawRectangle(Pens.Black, GetRealFromVirtualRect(r));
            e.Graphics.FillRectangle(Brushes.Black, GetRealFromVirtualRect(new Rectangle(midx - 4, r.Y - 4, 8, 4)));
            e.Graphics.FillRectangle(Brushes.Black, GetRealFromVirtualRect(new Rectangle(midx - 4, r.Bottom, 8, 4)));
            e.Graphics.FillRectangle(Brushes.Black, GetRealFromVirtualRect(new Rectangle(r.X - 4, midy - 4, 4, 8)));
            e.Graphics.FillRectangle(Brushes.Black, GetRealFromVirtualRect(new Rectangle(r.Right, midy - 4, 4, 8)));

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

			minWidth = minHeight = 0;
			maxWidth = this.ClientRectangle.Width;
			maxHeight = this.ClientRectangle.Height;

            using (Brush dot = new SolidBrush(Color.Red))
            using (Pen line = new Pen(Color.Black, 2))
            using (Pen activeLine = new Pen(Color.Blue, 2))
            using (Brush activeDot = new SolidBrush(Color.Blue))
            using (Pen controlLine = new Pen(Color.Green))
            using (Brush controlDot = new SolidBrush(Color.Green))
            {
				//draw the points and the lines connecting them
                for (int i = 0; i < points.Count; i++)
                {
                    ShapePoint pos1 = points[i];
                    ShapePoint pos2 = i < points.Count - 1 ? points[i + 1] : points[0];

					UpdateMaxSize(pos1);
					UpdateMaxSize(pos2);
					
                    Pen pen = point == pos1 ? activeLine : line;
					//if the line at the point is a bezier curve, draw it together with the tangents and the control points
                    if (pos1.Bezier)
                    {
						//draw the bezier curve
                        e.Graphics.DrawBezier(pen, 
							GetRealFromVirtualPoint(pos1), 
							GetRealFromVirtualPoint(pos1.ControlPoint1),
							GetRealFromVirtualPoint(pos1.ControlPoint2),
							GetRealFromVirtualPoint(pos2));
						
						//draw the tangents to the control points
						e.Graphics.DrawLine(controlLine, GetRealFromVirtualPoint(pos1), GetRealFromVirtualPoint(pos1.ControlPoint1));
						e.Graphics.DrawLine(controlLine, GetRealFromVirtualPoint(pos2), GetRealFromVirtualPoint(pos1.ControlPoint2));
						
						//draw the control points
                        e.Graphics.FillEllipse((pos1.ControlPoint1 == point || pos1.ControlPoint1.Selected) ? activeDot : controlDot,
							GetRealFromVirtualRect(pos1.ControlPoint1.GetBounds(8)));
                        e.Graphics.FillEllipse((pos1.ControlPoint2 == point || pos1.ControlPoint2.Selected) ? activeDot : controlDot,
							GetRealFromVirtualRect(pos1.ControlPoint2.GetBounds(8)));

						//update the maximum size in response to points outside of the drawable area
						UpdateMaxSize(pos1.ControlPoint1);
						UpdateMaxSize(pos1.ControlPoint2);
                    }
                    //draws a straight line
                    else
						e.Graphics.DrawLine(pos1 == point ? activeLine : line, GetRealFromVirtualPoint(pos1), GetRealFromVirtualPoint(pos2));

					//draws the two points at the ends of the line
					e.Graphics.FillEllipse((pos1 == point || pos1.Selected) ? activeDot : dot, GetRealFromVirtualRect(pos1.GetBounds(8)));
					e.Graphics.FillEllipse((pos2 == point || pos2.Selected) ? activeDot : dot, GetRealFromVirtualRect(pos2.GetBounds(8)));
                }
				//draw multiple point selection rectangle
                if (mouseDown)
                {
                    r = new Rectangle(
                        downPoint.X < curPoint.X ? downPoint.X : curPoint.X,
                        downPoint.Y < curPoint.Y ? downPoint.Y : curPoint.Y,
                        Math.Abs(downPoint.X - curPoint.X),
                        Math.Abs(downPoint.Y - curPoint.Y));
					//convert the rectangle location and size from virtual to real
					r.X = (int)Math.Round((r.X) / Math.Pow(zoomFactor, 1));
					r.Y = (int)Math.Round((r.Y) / Math.Pow(zoomFactor, 1));
					
					r.Width = (int)Math.Round(r.Width / Math.Pow(zoomFactor, 1));
					r.Height = (int)Math.Round(r.Height / Math.Pow(zoomFactor, 1));
					
                    activeLine.Width = 1;
                    e.Graphics.DrawRectangle(activeLine, r);
                }
            }
            //set the bounds of the scrollbars to contain all points
            vScrollBar1.Minimum = minHeight;
            hScrollBar1.Minimum = minWidth;
            
            vScrollBar1.Maximum = maxHeight;
            hScrollBar1.Maximum = maxWidth;
            SizeScrollBars();

			//draw little red point indicating the line of snap at a given point
			if (snapToGrid.IsLastSnapped)
				e.Graphics.FillEllipse(new SolidBrush(Color.Red), GetRealFromVirtualRect(new Rectangle((int)snapToGrid.SnappedPoint.X - 2, (int)snapToGrid.SnappedPoint.Y - 2, 4, 4)));
        }
        /// <summary>
        /// Updates the bounds of the drawable area
        /// </summary>
        /// <param name="pt"></param>
        private void UpdateMaxSize(ShapePointBase pt)
        {
			if (pt.X > maxWidth)
				maxWidth = (int)pt.X;
			if (pt.Y > maxHeight)
				maxHeight = (int)pt.Y;
			if(pt.X < minWidth)
				minWidth = (int)pt.X;
			if(pt.Y < minHeight)
				minHeight = (int)pt.Y;
        }

        void RadShapeEditorControl_Load(object sender, EventArgs e)
        {          
            if (points.Count == 0)
            {
                Dimension = new Rectangle(
                    ClientRectangle.X + 20, ClientRectangle.Y + 20,
                    ClientRectangle.Width - 40, ClientRectangle.Height - 40);

                points.Add(new ShapePoint(Dimension.X, Dimension.Y));
                points.Add(new ShapePoint(Dimension.Right, Dimension.Y));
                points.Add(new ShapePoint(Dimension.Right, Dimension.Bottom));
                points.Add(new ShapePoint(Dimension.X, Dimension.Bottom));
            }

			if (propertyGrid != null)
				propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);
        }
        
		void RadShapeEditorControl_MouseDown(object sender, MouseEventArgs e)
        {
			this.Cursor = Cursors.Cross;
            mouseDown = true;
            downPoint = new Point(e.X, e.Y);
			Point scaledTranslatedDownPoint = new Point(
				(int)Math.Round((downPoint.X) / Math.Pow(zoomFactor, 2)) + xOverflowOffset,
				(int)Math.Round((downPoint.Y) / Math.Pow(zoomFactor, 2)) + yOverflowOffset);
            curPoint = downPoint;
            point = null;
			//find the point underneath the mouse
			foreach (ShapePoint pos1 in points)
            {
				if (pos1.IsVisible(scaledTranslatedDownPoint.X, scaledTranslatedDownPoint.Y, (int)Math.Round(8 * zoomFactor)))
                {
                    pointType = PointTypes.Point;
                    point = pos1;
                    break;
                }
                else if (pos1.Bezier)
                {
					if (pos1.ControlPoint1.IsVisible(scaledTranslatedDownPoint.X, scaledTranslatedDownPoint.Y, (int)Math.Round(8 * zoomFactor)))
                    {
                        pointType = PointTypes.ControlPoint;
                        point = pos1.ControlPoint1;
                        break;
                    }
					else if (pos1.ControlPoint2.IsVisible(scaledTranslatedDownPoint.X, scaledTranslatedDownPoint.Y, (int)Math.Round(8 * zoomFactor)))
                    {
                        pointType = PointTypes.ControlPoint;
                        point = pos1.ControlPoint2;
                        break;
                    }
                }
            }
            
            if(isSymmetricPointMode)
            {
				if(point == null)
				{
					isSymmetricPointMode = false;
					return;
				}
				//use the currently selected node as a reference point to make the previously selected node symmetric to it
				referencePoint = point;
				//get the size of the drawable area and compute the coordinates of the new point
				Rectangle r = dimension;
				int midy = r.Top + r.Height / 2;
				int midx = r.Left + r.Width / 2;
					
				if(isVerticalSymmetry)
					newSymmetricPoint.Y = referencePoint.Y + 2 * (midy - point.Y);
				else
					newSymmetricPoint.X = referencePoint.X + 2 * (midx - point.X);

				isSymmetricPointMode = false;
				Refresh();
				return;
            }
			
            if (point == null)
                for (int i = 0; i < points.Count; i++)
                {
                    ShapePoint pos1 = points[i];
                    ShapePoint pos2 = i < points.Count - 1 ? points[i + 1] : points[0];

					if (pos1.IsVisible(pos2, new Point(scaledTranslatedDownPoint.X, scaledTranslatedDownPoint.Y), 3))
                    {
                        pointType = PointTypes.Line;
                        point = pos1;
                        break;
                    }
                }

            if (point != null)
                propertyGrid.SelectedObject = point;
            else
                propertyGrid.SelectedObject = null;

            Refresh();
			//in case the mouse was clicked outside of any point, unselect all points (and their control points)
            if (e.Button == MouseButtons.Left)
                if(point == null || !point.Selected)
                    foreach (ShapePoint pos in points)
                    {
                        pos.Selected = false;
                        pos.ControlPoint1.Selected = false;
                        pos.ControlPoint2.Selected = false;
                    }
			
            if(point != null)
                mouseDown = false;

            if (e.Button == MouseButtons.Right && point != null)
            {
                if (pointType == PointTypes.Point && point is ShapePoint)
                {
                    menuItemAnchorLeft.Checked = (point.Anchor & AnchorStyles.Left) != 0;
                    menuItemAnchorRight.Checked = (point.Anchor & AnchorStyles.Right) != 0;
                    menuItemAnchorTop.Checked = (point.Anchor & AnchorStyles.Top) != 0;
                    menuItemAnchorBottom.Checked = (point.Anchor & AnchorStyles.Bottom) != 0;

                    if (this.points.Count <= 2)
                    {
                        menuItemRemoveLine.Enabled = false;
                        menuItemRemovePoint.Enabled = false;
                    }

                    menuItemLocked.Checked = point.Locked;

					if ((point as ShapePoint).Bezier) contextMenuPoint.Items[1].Text = "Convert to Line";
					else contextMenuPoint.Items[1].Text = "Convert to Bezier Curve";
                      
                    contextMenuPoint.Show(PointToScreen(new Point(e.X, e.Y)));
                }
                else if (pointType == PointTypes.Line)
                {
					if ((point as ShapePoint).Bezier) contextMenuLine.Items[1].Text = "Convert to Line";
					else contextMenuLine.Items[1].Text = "Convert to Bezier Curve";

                    contextMenuLine.Show(PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }
        
		void RadShapeEditorControl_MouseMove(object sender, MouseEventArgs e)
        {
            oldCurPoint = curPoint;
            curPoint = new Point(e.X, e.Y); 
            
            if (e.Button == MouseButtons.Left && oldCurPoint != curPoint)
            {
				//converts the point coordinates from virtual to real
				Point scaledTranslatedDownPoint = new Point(
					(int)Math.Round((curPoint.X) / Math.Pow(zoomFactor, 2)) + xOverflowOffset,
					(int)Math.Round((curPoint.Y) / Math.Pow(zoomFactor, 2)) + yOverflowOffset);
				
				//tests whether the real point is snapped to the grid
				bool snapped = snapToGrid.SnapPtToGrid(
					new Point(
						scaledTranslatedDownPoint.X, 
						scaledTranslatedDownPoint.Y));

				if(point != null && pointType != PointTypes.Line && !point.Locked)
				{
					point.X = (int)snapToGrid.SnappedPoint.X;
					point.Y = (int)snapToGrid.SnappedPoint.Y;
					propertyGrid.Refresh();
				}

                if (!mouseDown)
                {
                    int xdelta = curPoint.X - oldCurPoint.X;
                    int ydelta = curPoint.Y - oldCurPoint.Y;
                    
					//moving groups of points and recalculating the bezier curve in case they are control points
                    foreach (ShapePoint pos in points)
                    {
                        if (pos != point && pos.Selected && !pos.Locked)
                        {
                            pos.X += xdelta;
                            pos.Y += ydelta;
                        }

                        if (pos.Bezier)
                        {
                            if (pos.ControlPoint1.Selected && !pos.ControlPoint1.Locked)
                            {
                                pos.ControlPoint1.X += xdelta;
                                pos.ControlPoint1.Y += ydelta;
                            }
                            if (pos.ControlPoint2.Selected && !pos.ControlPoint2.Locked)
                            {
                                pos.ControlPoint2.X += xdelta;
                                pos.ControlPoint2.Y += ydelta;
                            }
                        }
                    }
                    
                }
                Refresh();
            }
        }
        
		void RadShapeEditorControl_MouseUp(object sender, MouseEventArgs e)
        {
			this.Cursor = Cursors.Arrow;
            if (mouseDown)
            {
                mouseDown = false;
                //constructs the rectangle which the user has drawn for multiple point selection
                Rectangle r = new Rectangle(
                    downPoint.X < curPoint.X ? downPoint.X : curPoint.X,
                    downPoint.Y < curPoint.Y ? downPoint.Y : curPoint.Y,
                    Math.Abs(downPoint.X - curPoint.X),
                    Math.Abs(downPoint.Y - curPoint.Y));
				//converts the location and dimensions of the rectangle real coordinates
				r.X = (int)Math.Round((r.X) / Math.Pow(zoomFactor,2)) + xOverflowOffset;
				r.Y = (int)Math.Round((r.Y) / Math.Pow(zoomFactor,2)) + yOverflowOffset;
				r.Width = (int)Math.Round(r.Width / zoomFactor);
				r.Height = (int)Math.Round(r.Height / zoomFactor);
				
				//looks for and selects points in the selected rectangle
                foreach (ShapePoint pos in points)
                {
                    pos.Selected = point == null && r.Contains((pos.GetPoint()));
                    if (pos.Bezier)
                    {
						//select the control points if the point they belong to is not null and they're in the selected area
                        pos.ControlPoint1.Selected = point == null && r.Contains((pos.ControlPoint1.GetPoint()));
                        pos.ControlPoint2.Selected = point == null && r.Contains((pos.ControlPoint2.GetPoint()));
                    }
                    else
                    {
                        pos.ControlPoint1.Selected = false;
                        pos.ControlPoint2.Selected = false;
                    }
                }

                Refresh();
            }
        }
        
		void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (propertyGrid.SelectedObject is ShapePoint)
            {
                int index = this.points.IndexOf(this.point as ShapePoint);
                this.points[index] = propertyGrid.SelectedObject as ShapePoint;
                point = this.points[index];
            }
			Refresh();
		}
        
		void menuItemAnchorLeft_Click(object sender, EventArgs e)
        {
            this.point.Anchor ^= AnchorStyles.Left;
            propertyGrid.Refresh();
        }
        
		void menuItemAnchorRight_Click(object sender, EventArgs e)
        {
            this.point.Anchor ^= AnchorStyles.Right;
            propertyGrid.Refresh();
        }
        
		void menuItemAnchorTop_Click(object sender, EventArgs e)
        {
            this.point.Anchor ^= AnchorStyles.Top;
            propertyGrid.Refresh();
        }
        
		void menuItemAnchorBottom_Click(object sender, EventArgs e)
        {
            point.Anchor ^= AnchorStyles.Bottom;
            propertyGrid.Refresh();
        }
        
		void menuItemRemoveLine_Click(object sender, EventArgs e)
        {
            if (points.Count > 2)
            {
                points.Remove(point as ShapePoint);
                point = null;
                propertyGrid.SelectedObject = null;
                Refresh();
            }
        }
        
		void menuItemRemovePoint_Click(object sender, EventArgs e)
        {
            if (points.Count > 2)
            {
                points.Remove(point as ShapePoint);
                point = null;
                propertyGrid.SelectedObject = null;
                Refresh();
            }
        }
        
		void menuItemConvert_Click(object sender, EventArgs e)
        {
            (point as ShapePoint).Bezier = !(point as ShapePoint).Bezier;
            if ((point as ShapePoint).Bezier)
            {
                int index = points.IndexOf(point as ShapePoint) + 1;
                if (index >= points.Count)
                    index = 0;
                ShapePoint nextPoint = points[index];
                (point as ShapePoint).CreateBezier(nextPoint);
            }
            propertyGrid.Refresh();
            Refresh();
        }
        
		void menuItemAddPoint_Click(object sender, EventArgs e)
        {
            int index = points.IndexOf(point as ShapePoint) + 1;
            if (index >= points.Count)
                index = 0;
            points.Insert(index, new ShapePoint(downPoint.X, downPoint.Y));
            point = points[index];
            propertyGrid.SelectedObject = point;
            Refresh();
        }
        
		void menuItemLeftTopCorner_Click(object sender, EventArgs e)
        {
            point.X = dimension.X;
            point.Y = dimension.Y;
            point.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            point.Locked = true;
            Refresh();
        }
        
		void menuItemRightTopCorner_Click(object sender, EventArgs e)
        {
            point.X = dimension.Right;
            point.Y = dimension.Y;
            point.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            point.Locked = true;
            Refresh();
        }
        
		void menuItemLeftBottomCorner_Click(object sender, EventArgs e)
        {
            point.X = dimension.X;
            point.Y = dimension.Bottom;
            point.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
            point.Locked = true;
            Refresh();
        }
        
		void menuItemRightBottomCorner_Click(object sender, EventArgs e)
        {
            point.X = dimension.Right;
            point.Y = dimension.Bottom;
            point.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            point.Locked = true;
            Refresh();
        }
     
		void menuItemLocked_Click(object sender, EventArgs e)
        {
            point.Locked = !point.Locked;
        }

		public CustomShape GetShape()
		{
			CustomShape shape = new CustomShape();
			shape.Dimension = this.Dimension;
			foreach (ShapePoint point in this.Points)
				shape.Points.Add(point);
			return shape;
		}

		private void horizontallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//creates a new point, symmetric to the given one on the other side of a vertical symmetry line
			int index = points.IndexOf(point as ShapePoint) + 1;
			if (index >= points.Count)
				index = 0;
			
			//get the size of the drawable are and compute the coordinates of the new point
			Rectangle r = dimension;
			int midx = r.Left + r.Width / 2;
			ShapePoint newPoint = new ShapePoint();

			newPoint.X = point.X + 2 * (midx - point.X);
			newPoint.Y = point.Y;
			
			//insert the point in the chain of points
			points.Insert(index, newPoint);
			point = points[index];
			propertyGrid.SelectedObject = point;
			Refresh();
		}

		private void verticallyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			//creates a new point, symmetric to the given one on the other side of a horizontal symmetry line
			int index = points.IndexOf(point as ShapePoint) + 1;
			if (index >= points.Count)
				index = 0;
			//get the size of the drawable area and compute the coordinates of the new point
			Rectangle r = dimension;
			int midy = r.Top + r.Height / 2;
			ShapePoint newPoint = new ShapePoint();
			newPoint.X = point.X;
			newPoint.Y = point.Y + 2 * (midy - point.Y);

			//insert the point in the chain of points
			points.Insert(index, newPoint);
			point = points[index];
			propertyGrid.SelectedObject = point;
			Refresh();
		}

		private void horizontallyToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Please select the point to be used as a reference for the symmmetry", "Shape Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
			//set symmetric mode to true to trap the next mouse down on a point to use as a reference
			isSymmetricPointMode = true;
			//save the current point to be made symmetric horizontally
			newSymmetricPoint = point;
			//set horizontal symmetry flag
			isVerticalSymmetry = false;
		}

		private void verticallyToolStripMenuItem1_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Please select the point to be used as a reference for the symmmetry", "Shape Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
			//set symmetric mode to true to trap the next mouse down on a point to use as a reference
			isSymmetricPointMode = true;
			//set vertical symmetry flag
			isVerticalSymmetry = true;
			//save the current point to be made symmetric vertically
			newSymmetricPoint = point;
		}

		private void RadShapeEditorControl_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == Convert.ToChar("+"))
			{
				if (snapToGrid.FieldWidth < 100)
					snapToGrid.FieldWidth *= 2;
				this.Refresh();
			}
			else if (e.KeyChar == Convert.ToChar("-"))
			{
				if (snapToGrid.FieldWidth > 8)
					snapToGrid.FieldWidth /= 2;
				this.Refresh();
			}
		}
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if (e.Delta > 0)
			{
				if( zoomFactor < MAX_ZOOM_FACTOR)
				{
					zoomFactor += ZOOM_INCREMENT;
					DRAWABLE_GRID_LINE_WIDTH = (int)Math.Round(FIELD_WIDTH * zoomFactor);
					this.Refresh();
				}
			}
			else if (e.Delta < 0)
			{
				if (zoomFactor > MIN_ZOOM_FACTOR)
				{
					zoomFactor -= ZOOM_INCREMENT;
					DRAWABLE_GRID_LINE_WIDTH = (int)Math.Round(FIELD_WIDTH * zoomFactor);
					this.Refresh();
				}
			}
			base.OnMouseWheel(e);
		}

		private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
		{
			yOverflowOffset = vScrollBar1.Value = e.NewValue;
			this.Refresh();
		}

		private void hScrollBar1_Scroll(object sender, ScrollEventArgs e)
		{
			xOverflowOffset = hScrollBar1.Value = e.NewValue;
			this.Refresh();
		}

		private void SizeScrollBars()
		{
			hScrollBar1.Minimum = minWidth;
			vScrollBar1.Minimum = minHeight;

			//fix invalid values of offsets due to scrolling
			if(xOverflowOffset < hScrollBar1.Minimum)
				xOverflowOffset = hScrollBar1.Minimum;
			if(yOverflowOffset < vScrollBar1.Minimum)
				yOverflowOffset = vScrollBar1.Minimum;
				
			hScrollBar1.SetBounds(0, ClientRectangle.Height - hScrollBar1.Height, ClientRectangle.Width, hScrollBar1.Height);
			vScrollBar1.SetBounds(ClientRectangle.Right - vScrollBar1.Width, 0, vScrollBar1.Width, ClientRectangle.Height - hScrollBar1.Height);

			hScrollBar1.Maximum = (int)Math.Round((maxWidth * zoomFactor * zoomFactor - ClientRectangle.Width)) + vScrollBar1.Width * 2;
			vScrollBar1.Maximum = (int)Math.Round((maxHeight * zoomFactor * zoomFactor - ClientRectangle.Height)) + hScrollBar1.Height * 2;

			//fix invalid values of offsets due to scrolling
			if (xOverflowOffset > hScrollBar1.Maximum)
				xOverflowOffset = hScrollBar1.Maximum;
			if (yOverflowOffset > vScrollBar1.Maximum)
				yOverflowOffset = vScrollBar1.Maximum;

			vScrollBar1.Refresh();
			hScrollBar1.Refresh();
		}

		private void vScrollBar1_ValueChanged(object sender, EventArgs e)
		{
			yOverflowOffset = vScrollBar1.Value;
			this.Refresh();
		}

		private void hScrollBar1_ValueChanged(object sender, EventArgs e)
		{
			xOverflowOffset = hScrollBar1.Value;
			this.Refresh();
		}
	}
}
