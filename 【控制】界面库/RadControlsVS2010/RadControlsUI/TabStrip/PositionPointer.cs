using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace Telerik.WinControls.UI
{
    /// <summary>Defines the directions of the hint pointers in RadTabstrip. Hint pointers 
    /// facilitate the drag and drop functionality by notifying the user that an item 
    /// can be dropped at the location chosen by the user. 
    /// </summary>
   public enum Direction    
   {
		/// <summary>
	    /// Indicates a left hint pointer direction.
		/// </summary>
		Left,
		/// <summary>
		/// Indicates a right hint pointer direction.
		/// </summary>
		Right,
        /// <summary>
        /// Indicates an up hint pointer direction.
        /// </summary>
        Up,
        /// <summary>
        /// Indicates a down hint point direction.
        /// </summary>
        Down,
    }

    public class PositionPointer : System.Windows.Forms.Form
    {
        public PositionPointer()
        {
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "PositionPoiner";
            this.Text = "PositionPoiner";
            this.TransparencyKey = Color.Sienna;
            this.BackColor = Color.Sienna;
            this.MinimumSize = new Size(1, 1);
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(8, 8);
            this.ShowInTaskbar = false;
            arrowColor = Color.Black;
        }

        public PositionPointer(Direction pointerDirection)
            : this()
        {
            this.dir = pointerDirection;
        }

        private Direction dir = Direction.Down;
        private Color arrowColor;

        public Color ArrowColor
        {
            get
            {
                return arrowColor;
            }
            set
            {
                arrowColor = value;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics graphics = e.Graphics;

            SolidBrush sbrush = new SolidBrush(ArrowColor);

            switch (dir)
            {
				case Direction.Up:
					{
						graphics.FillPolygon(sbrush,
						   new Point[] {
                            new Point(0, ClientRectangle.Height),
                            new Point(ClientRectangle.Width / 2, 0),
                            new Point(ClientRectangle.Width, ClientRectangle.Height)
                        });
						break;
					}
				case Direction.Down:
					{
						graphics.FillPolygon(sbrush,
						new Point[] {
                            new Point(0, 0),
                            new Point(ClientRectangle.Width / 2, ClientRectangle.Height),
                            new Point(ClientRectangle.Width, 0)
                        });
						break;
					}
				case Direction.Left:
					{
						graphics.FillPolygon(sbrush,
						new Point[] {
                            new Point(0, ClientRectangle.Height / 2),
                            new Point(ClientRectangle.Width, ClientRectangle.Height),
                            new Point(ClientRectangle.Width, 0)
                        });
						break;
					}
				case Direction.Right:
					{
						graphics.FillPolygon(sbrush,
						   new Point[] {
						    new Point(0, 0),
                            new Point(0, ClientRectangle.Height),
                            new Point(ClientRectangle.Width, ClientRectangle.Height / 2)
                        });
						break;
					}				
            }

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // PositionPoiner
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "PositionPoiner";
            this.ResumeLayout(false);

        }
    }
}
