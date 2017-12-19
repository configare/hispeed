using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using System.Collections;
using Telerik.WinControls.Layouts;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
	public partial class FloatingForm : Form
	{
		private ResizeDirection directionToResize;
		

		private Point initialMovingPoint;
		private Point oldMousePos;
		private Point lastResizePoint;
		private Point offsetPoint;
		private Point oldLocation;

		private Timer fadeTimer;
		private Timer fadeStartTimer;
		private Timer backFadeTimer;
	
		private int fadeDelay;

		private bool isWindowMoving;
	
		private int maxChildWidth;
		
		private Stack<int> shrinkWidths;

		public int CaptionHeight = 19;
	
		private Size initialSize;

		private Color closeButtonForeColor = Color.FromArgb(255, 255, 255);
		private Color arrowButtonForeColor = Color.FromArgb(255, 255, 255);

		private const int offsetStep = 10;
		private readonly int resizeConst = 3;

		private RadToolStripManager toolStripManager;
	
		public enum ResizeDirection
		{
			Top,
			Bottom,
			Left,
			Right,
			None,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public int left;
			public int top;
			public int right;
			public int bottom;
			public RECT(int left, int top, int right, int bottom)
			{
				this.left = left;
				this.top = top;
				this.right = right;
				this.bottom = bottom;
			}
			public RECT(Rectangle r)
			{
				this.left = r.Left;
				this.top = r.Top;
				this.right = r.Right;
				this.bottom = r.Bottom;
			}
			
			public System.Drawing.Size Size
			{
				get
				{
					return new System.Drawing.Size(this.right - this.left, this.bottom - this.top);
				}
			}
		}

        public FloatingForm(RadToolStripManager toolStripManager)
		{
			InitializeComponent();

			this.VisibleChanged += new EventHandler(FloatingForm_VisibleChanged);
			this.FormBorderStyle = FormBorderStyle.None;
			this.ShowInTaskbar = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.Opaque, true);
			this.StartPosition = FormStartPosition.Manual;
			this.MinimumSize = new Size(20, 19);
			this.shrinkWidths = new Stack<int>();
			this.toolStripManager = toolStripManager;
			this.directionToResize = ResizeDirection.None;
			this.TopMost = true;
			this.ForeColor = Color.White;
			this.Font = new Font(this.Font, FontStyle.Bold);
		
			this.fadeTimer = new Timer();		
			this.fadeStartTimer = new Timer();
			this.backFadeTimer = new Timer();

			this.fadeTimer.Tick += new EventHandler(fadeTimer_Tick);
			this.fadeTimer.Interval = 100;
			this.fadeStartTimer.Tick += new EventHandler(fadeStartTimer_Tick);
			this.backFadeTimer.Tick += new EventHandler(backFadeTimer_Tick);

		}

		private string key = "";
		/// <summary>
		/// Gets or sets the key value associated to this object
		/// </summary>
		[Browsable(false)]
		[DefaultValue("")]
		[Description("Gets or sets the key value associated to this object")]
		public string Key
		{
			get
			{
				
				return this.key;
			}
			set
			{
				this.key = value;
			}
		}

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.components != null)
                {
                    components.Dispose();
                }

                if (this.fadeTimer != null)
                {
                    this.fadeTimer.Dispose();
                    this.fadeTimer = null;
                }

                if (this.fadeStartTimer != null)
                {
                    this.fadeStartTimer.Dispose();
                    this.fadeStartTimer = null;
                }

                if (this.backFadeTimer != null)
                {
                    this.backFadeTimer.Dispose();
                    this.backFadeTimer = null;
                }
            }
            base.Dispose(disposing);
        }

		private void backFadeTimer_Tick(object sender, EventArgs e)
		{
			if (this.Opacity < 1)
				this.Opacity += 0.06;
			else
				this.backFadeTimer.Stop();
		}

		private void fadeStartTimer_Tick(object sender, EventArgs e)
		{
			this.fadeStartTimer.Stop();
			this.fadeTimer.Start();
		}

		private void fadeTimer_Tick(object sender, EventArgs e)
		{
			if (this.Opacity > 0.5)
				this.Opacity -= 0.03;
			else
				this.fadeTimer.Stop();
		}

		public int FadeDelay
		{
			get
			{
				return this.fadeDelay;
			}

			set
			{
				if (value > 0)
				{
					this.fadeStartTimer.Interval = value;
			
					this.fadeStartTimer.Start();
				
				}

				this.fadeDelay = value;
			}
		}

		protected override void OnMouseEnter(EventArgs e)
		{

			if (this.FadeDelay > 0)
			{
				if (this.fadeStartTimer != null)
					this.fadeStartTimer.Stop();

				if (this.fadeTimer != null)
					this.fadeTimer.Stop();

				if (this.backFadeTimer != null)
					this.backFadeTimer.Start();
			}
			base.OnMouseEnter(e);
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			if (this.FadeDelay > 0)
			{
				if (this.fadeTimer != null)
					this.fadeTimer.Stop();

				if (this.fadeStartTimer != null)
					this.fadeStartTimer.Start();

				if (this.backFadeTimer != null)
					this.backFadeTimer.Stop();
			}
			base.OnMouseLeave(e);
		}

		public RadToolStripManager ToolStripManager
		{
			get
			{
				return this.toolStripManager;
			}
		}

		public Stack<int> ShrinkWidths
		{
			get
			{
				return this.shrinkWidths;
			}
			set
			{
				this.shrinkWidths = value;
			}
		}
		private void PerformDocking()
		{
			RadToolStrip toolStripUnderMouseCursor = null;
			RadToolStripItem item = new RadToolStripItem();
			
			if (this.directionToResize == ResizeDirection.None)
			{
				for (int i = 0; i < toolStripManager.DockingSites.Count; i++)
				{
					toolStripUnderMouseCursor = toolStripManager.DockingSites[i] as RadToolStrip;
					toolStripUnderMouseCursor.Focus();
					if (toolStripUnderMouseCursor.ToolStripManager.Orientation == Orientation.Horizontal)
					{
						toolStripUnderMouseCursor.ToolStripManager.resizeTimer.Interval = 10;
					}
					else
						toolStripUnderMouseCursor.ToolStripManager.resizeTimer.Interval = 10;

					Rectangle rangeRect = toolStripUnderMouseCursor.RectangleToScreen(toolStripUnderMouseCursor.ClientRectangle);

                    int edgeOffset = 20;
					if (toolStripUnderMouseCursor.ToolStripManager.Orientation == Orientation.Horizontal )
						rangeRect = new Rectangle(rangeRect.X, rangeRect.Y - edgeOffset, rangeRect.Width, rangeRect.Height + 2 * edgeOffset);		
					else
						rangeRect = new Rectangle(rangeRect.X - edgeOffset, rangeRect.Y, rangeRect.Width + edgeOffset, rangeRect.Height);		
				

					if ( rangeRect.Height == 0 )
						rangeRect = new Rectangle(rangeRect.X, rangeRect.Y, rangeRect.Width, rangeRect.Height + edgeOffset);
					else
						if (rangeRect.Width == 0)
						{
                            rangeRect = new Rectangle(rangeRect.X - edgeOffset, rangeRect.Y, rangeRect.Width + 2 * edgeOffset, rangeRect.Height);
			
						}
					if (this.Controls.Count > 0)
						if ((this.Controls[0] as RadToolStripItemControl).Items.Count > 0)
						{
									
							if (rangeRect.Contains(Cursor.Position))
							{
								item.FloatingFormPreferredSize = this.Size;
								item.ShrinkWidths = this.shrinkWidths;
								item.FloatingCloseButton.Visible = this.Controls[1].Visible;
								item.FloatingOverFlowButton.Visible = this.Controls[2].Visible;

								InsertToAppropriatePosition(toolStripUnderMouseCursor.ToolStripManager, item, rangeRect);
								item.Key = this.Key;

								this.InitializeToolStripItem(item, toolStripUnderMouseCursor);
								this.Capture = false;
								this.Controls.Clear();

								this.Close();
							}
						}
				}
			}
		}

        private void InsertToAppropriatePosition(RadToolStripManager toolStripManager, RadToolStripItem item, Rectangle rangeRect)
		{
			toolStripManager.SuspendLayout();
			if (toolStripManager.Items.Count > 0 )
			{
				
				if (toolStripManager.Orientation == Orientation.Horizontal)
				{
					RadToolStripElement element = new RadToolStripElement();
					Rectangle topRect = new Rectangle(rangeRect.X, rangeRect.Y, rangeRect.Width, 25);
					Rectangle bottomRect = new Rectangle(rangeRect.X, rangeRect.Bottom - 25, rangeRect.Width, 25);

					if ((topRect.Contains(this.Location)) && (toolStripManager.parentAutoSize ))
					{

						toolStripManager.Items.Insert(0, element);
						element.Items.Add(item);
						item.InvalidateLayout();

					}
					else
					{
						if (bottomRect.Contains(this.Location) && toolStripManager.parentAutoSize )
						{

							toolStripManager.Items.Add(element);
							element.Items.Add(item);
							item.InvalidateLayout();

						}
						else
						{
							(toolStripManager.Items[0] as RadToolStripElement).Items.Add(item);
							item.InvalidateLayout();

						}
					}
				}
				else
				{
					RadToolStripElement element = new RadToolStripElement();
                    element.Orientation = Orientation.Vertical;
					Rectangle leftRect = new Rectangle(rangeRect.X - 25, rangeRect.Y, 25, rangeRect.Height);
					Rectangle rightRect = new Rectangle(rangeRect.Right - 25, rangeRect.Y, 25 , rangeRect.Height);

					if (leftRect.Contains(this.Location) && toolStripManager.parentAutoSize )
					{

						toolStripManager.Items.Insert(0, element);
						element.Items.Add(item);
						item.InvalidateLayout();


					}
					else
					{
						if (rightRect.Contains(this.Location) && toolStripManager.parentAutoSize)
						{

							toolStripManager.Items.Add(element);
							element.Items.Add(item);
							item.InvalidateLayout();


						}
						else
						{
							(toolStripManager.Items[0] as RadToolStripElement).Items.Add(item);
							item.InvalidateLayout();

						}
					}
			
				}
			}
			else
			{
				RadToolStripElement element = new RadToolStripElement();
				toolStripManager.Items.Add( element );
				(toolStripManager.Items[0] as RadToolStripElement).Items.Add(item);
				item.InvalidateLayout();

			}
			if (this.fadeTimer != null)
				this.fadeTimer.Stop();

			if (this.fadeStartTimer != null)
				this.fadeStartTimer.Stop();

			if (this.backFadeTimer != null)
				this.backFadeTimer.Stop();

			toolStripManager.ResumeLayout(true);
		}

		private void NotifyToolStripItemsChanged(RadToolStripManager manager)
		{
			foreach (RadToolStripElement element in manager.Items)
			{
				foreach (RadToolStripItem item in element.Items)
				{	
					item.OverflowManager.ManagerChanged = true;
				}
			}
		}

		private void InitializeToolStripItem(RadToolStripItem item, RadToolStrip toolStripUnderMouseCursor)		
		{
			item.Grip.ParentToolStripItem = item;	
			item.Grip.Capture = true;
			item.Grip.FromFloating = true;
			item.itemsLayout.ParentToolStripItem = item;
			item.Text = this.Text;
			NotifyToolStripItemsChanged(toolStripUnderMouseCursor.ToolStripManager);
			this.toolStripManager.formList.Remove(this);

            Orientation orientation = toolStripUnderMouseCursor.ToolStripManager.Orientation;
            item.Orientation = orientation;
            RadToolStripItemControl itemControl = this.Controls[0] as RadToolStripItemControl;

			for (int j = 0; j < itemControl.Items.Count; j++)
			{
                RadItem childItem = itemControl.Items[j];

                if (orientation == Orientation.Vertical)
                {
                    if (childItem is RadComboBoxElement || childItem is RadTextBoxElement)
                    {
                        childItem.Visibility = ElementVisibility.Collapsed;
                    }
                }

				item.Items.Add(childItem);
			}
		}
	
		private void FloatingForm_VisibleChanged(object sender, EventArgs e)
		{
			if (this.Visible)
			{
				if (this.Controls.Count > 0)
				{
					foreach (RadItem item in (this.Controls[0] as RadToolStripItemControl).Items)
					{

						if (this.maxChildWidth < item.Bounds.Width)
							this.maxChildWidth = item.Bounds.Width;


						item.Visibility = ElementVisibility.Visible;
					}
					this.Controls[0].Location = new Point(3, this.CaptionHeight);
					this.MinimumSize = new Size(this.maxChildWidth, this.MinimumSize.Height);
					this.initialSize = this.Size;
					this.toolStripManager.LayoutEngine.SetLayoutInvalidated(true);
					this.toolStripManager.LayoutEngine.PerformParentLayout();
					this.NotifyToolStripItemsChanged(this.toolStripManager);
				}
			}

		}

		private Rectangle arrowRect;
		private Pen borderPen = new Pen(Color.Black);

		public Pen BorderPen
		{
			get
			{
				return this.borderPen;
			}
			set
			{
				this.borderPen = value;
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			SolidBrush brush = new SolidBrush(this.BackColor);
			SolidBrush stringBrush = new SolidBrush(this.ForeColor);

			e.Graphics.FillRectangle(brush, this.ClientRectangle);
	
			TextRenderer.DrawText(e.Graphics, this.Text, this.Font,
			  new Rectangle(2, 5, this.ClientRectangle.Width-50, this.CaptionHeight), this.ForeColor, TextFormatFlags.EndEllipsis);
		

			e.Graphics.DrawLine(this.borderPen, 2, 3, 2, this.ClientRectangle.Bottom - 5);
			e.Graphics.DrawLine(this.borderPen, 3, 2, this.ClientRectangle.Right - 4, 2 );
			e.Graphics.DrawLine(this.borderPen, 3, this.ClientRectangle.Bottom - 4, this.ClientRectangle.Right - 4, this.ClientRectangle.Bottom - 4);
			e.Graphics.DrawLine(this.borderPen, this.ClientRectangle.Right - 3, this.ClientRectangle.Bottom - 5, this.ClientRectangle.Right - 3, this.ClientRectangle.Top + 3);

			brush.Dispose();
			stringBrush.Dispose();
			base.OnPaint(e);
		}


		private void DrawDownArrow(Graphics g, Rectangle arrow, Pen pen)
		{
			SolidBrush brush = new SolidBrush(pen.Color);

			g.FillPolygon(brush,
						 new Point[] {
                            new Point(arrow.Left, arrowRect.Top),
                            new Point(arrow.Left + arrow.Width / 2, arrow.Height),
                            new Point(arrow.Left + arrow.Width, arrowRect.Top)
                        });
		
			brush.Dispose();
		}

		private void DrawCloseButton(Graphics g, Rectangle cross, Pen pen)
		{
			cross.Inflate(-2, -2);

			g.DrawLine(pen, cross.X, cross.Y, cross.Right, cross.Bottom);
			g.DrawLine(pen, cross.X + 1, cross.Y, cross.Right, cross.Bottom - 1);
			g.DrawLine(pen, cross.X, cross.Y + 1, cross.Right - 1, cross.Bottom);
			g.DrawLine(pen, cross.Right, cross.Y, cross.Left, cross.Bottom);
			g.DrawLine(pen, cross.Right - 1, cross.Y, cross.Left, cross.Bottom - 1);
			g.DrawLine(pen, cross.Right, cross.Y + 1, cross.Left + 1, cross.Bottom);
		}

        internal void InitializeMove(Point pt)
        {
            this.isWindowMoving = true;
            this.initialMovingPoint = pt;
            this.oldMousePos = this.initialMovingPoint;
            this.oldLocation = this.Location;
            this.Capture = true;
            Cursor.Current = Cursors.SizeAll;
        }

		protected override void OnMouseDown(MouseEventArgs e)
		{
			for (int i = 0; i < toolStripManager.DockingSites.Count; i++)
			{
				RadToolStrip toolStripUnderMouseCursor = toolStripManager.DockingSites[i] as RadToolStrip;
				toolStripUnderMouseCursor.Focus();
			}

			if (e.Button == MouseButtons.Left)
			{
				switch (this.directionToResize)
				{
					case ResizeDirection.None:
						{
                            this.InitializeMove(this.PointToScreen(e.Location));
							break;
						}

					case ResizeDirection.Right:
					case ResizeDirection.Left:
						Cursor.Current = Cursors.SizeWE;
						break;

					case ResizeDirection.Top:
					case ResizeDirection.Bottom:
						Cursor.Current = Cursors.SizeNS;
						break;

				}
			}
				
			
			base.OnMouseDown(e);			
		}
	
		protected override void OnMouseUp(MouseEventArgs e)
		{
			this.Capture = false;
			this.isWindowMoving = false;

			base.OnMouseUp(e);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (this.Controls.Count > 2)
			{
				this.Controls[1].Bounds = new Rectangle(this.Bounds.Width - 20, 3, 12, 12);
				this.Controls[2].Bounds = new Rectangle(this.Bounds.Width - 38, 3, 10, 10);
				if (this.Controls[0] as RadToolStripItemControl != null)
				{
					(this.Controls[0] as RadToolStripItemControl).ContainerElement.ToolStripContainterElementFill.Size = this.Size;
				}

			}

		
			
		}
		
		private void SetResizeDirection(MouseEventArgs e)
		{
			if (!this.isWindowMoving)
			{
				if (e.Location.X >= 0 && e.Location.X <= this.ClientRectangle.Right)
				{
					if (e.Location.Y >= 0 && e.Location.Y <= this.resizeConst)
					{
						this.directionToResize = ResizeDirection.Top;
						Cursor.Current = Cursors.SizeNS;
					}
				}

				if (e.Location.X >= 0 && e.Location.X <= this.ClientRectangle.Right)
				{
					if (e.Location.Y >= this.ClientRectangle.Bottom - this.resizeConst && e.Location.Y <= this.ClientRectangle.Bottom)
					{
						{
							this.directionToResize = ResizeDirection.Bottom;
							Cursor.Current = Cursors.SizeNS;
						}
					}
				}


				if (e.Location.X >= 0 && e.Location.X <= this.resizeConst)
				{
					if (e.Location.Y > this.resizeConst && e.Location.Y <= this.ClientRectangle.Bottom - this.resizeConst)
					{
						this.directionToResize = ResizeDirection.Left;
						Cursor.Current = Cursors.SizeWE;
					}
				}

				if (e.Location.X >= this.ClientRectangle.Right - this.resizeConst && e.Location.X <= this.ClientRectangle.Right)
				{
					if (e.Location.Y > this.resizeConst && e.Location.Y <= this.ClientRectangle.Bottom - this.resizeConst)
					{
						this.directionToResize = ResizeDirection.Right;
						Cursor.Current = Cursors.SizeWE;

					}
				}

				this.offsetPoint = Point.Empty;
				this.oldMousePos = Point.Empty;
				this.lastResizePoint = e.Location;
			}

		}

		private RadItem GetBottomMostItem()
		{
			ToolStripFlowLayout layout = (this.Controls[0] as RadToolStripItemControl).ContainerElement.StackLayoutPanel;
			RadItem bottomMoseItem = null;

			foreach (RadItem myItem in layout.Children)
			{
				if (bottomMoseItem == null)
					bottomMoseItem = myItem;
				else
				{
					if (bottomMoseItem.Bounds.Top < myItem.Bounds.Top)
						bottomMoseItem = myItem;
				}
			}

			return bottomMoseItem;
		}

		private void LeftResize(MouseEventArgs e)
		{
			if ((e.Location.X > this.ClientRectangle.Left + offsetStep)
				 && (e.Location.X >= this.oldMousePos.X))
			{
				SuspendLayout();
	
				int width = this.GetPreferredWidthToShrink();

				if (width > 0)
				{
					if (this.Size.Width - width > this.MinimumSize.Width)
					{
						ToolStripFlowLayout layout = (this.Controls[0] as RadToolStripItemControl).ContainerElement.StackLayoutPanel;

						this.Size = new Size(this.Size.Width - width, this.Size.Height);
						this.Location = new Point(this.Location.X + width, this.Location.Y);

						RadItem bottomMoseItem = this.GetBottomMostItem();

						this.Size = new Size(this.Size.Width, this.CaptionHeight + 4 + bottomMoseItem.Bounds.Bottom);
						offsetPoint = e.Location;
					}
				}
				ResumeLayout(true);
			}
			else
			{
				
					this.SuspendLayout();
					if (this.shrinkWidths.Count > 0)
					{
						// extend
						int width = this.shrinkWidths.Pop();

						if (width > 0)
						{
							if ((-width - offsetStep > e.Location.X) && (this.PointToScreen(e.Location).X < this.PointToScreen(this.oldMousePos).X))
							{

								this.Size = new Size(this.Size.Width + width, this.Size.Height);
								this.Location = new Point(this.Location.X - width, this.Location.Y);

								ToolStripFlowLayout layout = (this.Controls[0] as RadToolStripItemControl).ContainerElement.StackLayoutPanel;

								RadItem bottomMoseItem = this.GetBottomMostItem();

								this.Size = new Size(this.Size.Width, this.CaptionHeight + 4 + bottomMoseItem.Bounds.Bottom);
								offsetPoint = e.Location;
							}
							else
								this.shrinkWidths.Push(width);
						}

						this.ResumeLayout(true);
					}
				
			}

			this.oldMousePos = new Point(e.X, e.Y);		
		}

		private void TopResize(MouseEventArgs e)
		{
				
			if ((this.PointToScreen(e.Location).Y < this.Bounds.Top - offsetStep)
				 && ( Cursor.Position.Y < this.oldMousePos.Y))
			{
				int width = this.GetPreferredWidthToShrink();

				if (width > 0)
				{
					if (this.Size.Width - width > this.MinimumSize.Width)
					{

						this.Size = new Size(this.Size.Width - width, this.Size.Height);
						RadItem bottomMoseItem = this.GetBottomMostItem();
						int difference = this.Size.Height;

						this.Size = new Size(this.Size.Width, this.CaptionHeight + 4 + bottomMoseItem.Bounds.Bottom);
						this.Location = new Point(this.Location.X, this.Location.Y - (this.Size.Height - difference));		
						offsetPoint = e.Location;

					}
				}
			}
			else
			{
				
				if ((e.Location.Y > this.ClientRectangle.Top + offsetStep) && (Cursor.Position.Y > this.oldMousePos.Y)
						&& e.Location.Y > offsetPoint.Y )
				{
					this.SuspendLayout();
					if (this.shrinkWidths.Count > 0)
					{
						// extend
						int width = this.shrinkWidths.Pop();


						if ((width > 0))
						{
							this.Size = new Size(this.Size.Width + width, this.Size.Height);


							RadItem bottomMoseItem = this.GetBottomMostItem();
							
							int difference = this.Size.Height;

							this.Size = new Size(this.Size.Width, this.CaptionHeight + 4 + bottomMoseItem.Bounds.Bottom);

							int z = difference - this.Size.Height;
							if (z < e.Location.Y)
							{
								this.Location = new Point(this.Location.X, this.Location.Y - (this.Size.Height - difference));

								offsetPoint = e.Location;
							}
							else
							{
								this.Size = new Size(this.Size.Width - width, difference);
								this.shrinkWidths.Push(width);
							}
						}
					}

					this.ResumeLayout(true);
				

				}
				
			}
			
	
			this.oldMousePos = this.PointToScreen(e.Location);
		}

		private void BottomResize(MouseEventArgs e)
		{
			if ((e.Location.Y > this.ClientRectangle.Bottom + 2*offsetStep)
				 && (e.Location.Y > this.oldMousePos.Y))
			{
				SuspendLayout();
				int width = this.GetPreferredWidthToShrink();

				if (width > 0)
				{
					if (this.Size.Width - width > this.MinimumSize.Width)
					{

						this.Size = new Size(this.Size.Width - width, this.Size.Height);

						RadItem bottomMoseItem = this.GetBottomMostItem();

						this.Size = new Size(this.Size.Width, this.CaptionHeight + 4 + bottomMoseItem.Bounds.Bottom);
						offsetPoint = e.Location;

					}
				}
				ResumeLayout(true);
			}
			else
			{
				if (offsetPoint.Y == 0) offsetPoint = new Point(offsetPoint.X, e.Location.Y + 1);
				if (oldMousePos.Y == 0) oldMousePos = new Point(oldMousePos.X, e.Location.Y + 1);
				if ((e.Location.Y < this.ClientRectangle.Bottom - offsetStep) && (e.Location.Y <= oldMousePos.Y)
					&& (e.Location.Y < offsetPoint.Y))
				{

					this.SuspendLayout();
					if (this.shrinkWidths.Count > 0)
					{
						// extend
						int width = this.shrinkWidths.Pop();


						if (width > 0)
						{
							this.Size = new Size(this.Size.Width + width, this.Size.Height);


							RadItem bottomMoseItem = this.GetBottomMostItem();


							this.Size = new Size(this.Size.Width, this.CaptionHeight + 4 + bottomMoseItem.Bounds.Bottom);
							offsetPoint = e.Location;

						}


					}
					this.ResumeLayout(true);
				
				}

			}

			this.oldMousePos = new Point(e.X, e.Y);		
		}

		private void RightResize(MouseEventArgs e)
		{
			if ((e.Location.X < this.ClientRectangle.Right - offsetStep)
				 && (e.Location.X < this.oldMousePos.X) )					
					{
						SuspendLayout();
						int width = this.GetPreferredWidthToShrink();

						if (width > 0)
						{
							if (this.Size.Width - width > this.MinimumSize.Width)
							{
							
								this.Size = new Size(this.Size.Width - width, this.Size.Height);

								RadItem bottomMoseItem = this.GetBottomMostItem();

								this.Size = new Size(this.Size.Width, this.CaptionHeight + 4 + bottomMoseItem.Bounds.Bottom);
								offsetPoint = e.Location;
						
							}
						}
				
						ResumeLayout(true);
					}
					else
					{

						if (( e.Location.X > this.ClientRectangle.Right + 5*offsetStep ) && ( e.Location.X >= oldMousePos.X )
							&& ( e.Location.X > offsetPoint.X ))
						{

							this.SuspendLayout();
							if (this.shrinkWidths.Count > 0)
							{
								// extend
								int width = this.shrinkWidths.Pop();

									if (width > 0)
									{
										this.Size = new Size(this.Size.Width + width, this.Size.Height);
									
	
										RadItem bottomMoseItem = this.GetBottomMostItem();


										this.Size = new Size(this.Size.Width, this.CaptionHeight + 4 + bottomMoseItem.Bounds.Bottom);
										offsetPoint = e.Location;
					
									}		
							}
							this.ResumeLayout(true);
						}
						
					}

			this.oldMousePos = new Point(e.X, e.Y);			
		}


		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

            if (e.Button == MouseButtons.None)
            {
                this.directionToResize = ResizeDirection.None;
                this.SetResizeDirection(e);
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (this.isWindowMoving)
            {
                Point location = this.PointToScreen(e.Location);

                int XDistance = location.X - this.initialMovingPoint.X;
                int YDistance = location.Y - this.initialMovingPoint.Y;

                this.Location = new Point(this.oldLocation.X + XDistance, this.oldLocation.Y + YDistance);
                //attempt to dock the floating form
                this.PerformDocking();
                return;
            }

            switch(this.directionToResize)
            {
                case ResizeDirection.Bottom:
                    this.BottomResize(e);
                    break;
                case ResizeDirection.Left:
                    this.LeftResize(e);
                    break;
                case ResizeDirection.Right:
                    this.RightResize(e);
                    break;
                case ResizeDirection.Top:
                    this.TopResize(e);
                    break;
            }

            this.Invalidate(true);
		}

		private int GetPreferredWidthToShrink()
		{
			if (this.Controls.Count > 0)
			{
				int countOfItems = (this.Controls[0] as RadToolStripItemControl).Items.Count;
				int preferredShrinkWidth = 0;

				Stack<RadItem> stack = new Stack<RadItem>();

				foreach (RadItem item in (this.Controls[0] as RadToolStripItemControl).Items)
				{
					if (item.Bounds.Top == 0)
					{
						stack.Push(item);
					}
				}

				int count = stack.Count / 2;
	
				for (int i = 0; i < count; i++)
				{
					preferredShrinkWidth += stack.Pop().FullBoundingRectangle.Width;
				}
	
				if (this.Size.Width - preferredShrinkWidth >= this.MinimumSize.Width)
				{
					this.shrinkWidths.Push(preferredShrinkWidth);
				}
				return preferredShrinkWidth;

			}
			
			return 0;	
		}


		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams params1 = base.CreateParams;
				
				if (Environment.OSVersion.Version.Major > 5 ||
					(Environment.OSVersion.Version.Major == 5 && Environment.OSVersion.Version.Minor >= 1))
					params1.ClassStyle |= 0x20000;

				params1.Style &= -79691777;
				params1.ExStyle &= -262145;
				params1.ExStyle |= 0x10000;
				params1.Style |= -2147483648;
				params1.Style |= 0x800;
				
				return params1;
			}
		}

        protected virtual void WmNCActivate(ref Message m)
		{
			if (m.WParam != IntPtr.Zero)
			{
                this.DefWndProc(ref m);
			}
			else
			{
				base.WndProc(ref m);
			}
		}

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == NativeMethods.WM_NCACTIVATE)
			{
				this.WmNCActivate(ref m);
				return;
			}
			base.WndProc(ref m);
		}

		private void FloatingForm_Load(object sender, EventArgs e)
		{

		}

		private void FloatingForm_Load_1(object sender, EventArgs e)
		{

		}
	
	}
	
}