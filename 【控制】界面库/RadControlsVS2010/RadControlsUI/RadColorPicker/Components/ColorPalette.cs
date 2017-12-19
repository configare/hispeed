using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Telerik.WinControls.UI
{
	/// <summary>
	/// Represents a color palette
	/// </summary>
	[ToolboxItem(false), ComVisible(false)]
	public partial class ColorPalette : UserControl
	{
		public ColorPalette()
		{
			InitializeComponent();
			this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
		}

        private int columns;
        private int selectedColorIndex;
		/// <summary>
		/// Gets or sets the number of columns in the palette
		/// </summary>
        public int Columns
		{
			get { return columns; }
			set { columns = value; }
		}

        private int colorMargin;
		/// <summary>
		/// Gets or sets the margin of the palette
		/// </summary>
		public int ColorMargin
		{
			get { return colorMargin; }
			set { colorMargin = value; }
		}

        private Color[] colors;
		/// <summary>
		/// Gets or sets the color in the palette
		/// </summary>
		public Color[] Colors
		{
			get { return colors; }
			set { colors = value; }
		}

        private Color selectedColor;
		/// <summary>
		/// Gets or sets the selected color
		/// </summary>
		public Color SelectedColor
		{
			get { return selectedColor; }
			set { selectedColor = value; Refresh();  }
		}

		/// <summary>
		/// Fires when the selected color has changed
		/// </summary>
		public event ColorChangedEventHandler ColorChanged;

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			if (colors != null && colors.Length > 0)
			{
				int rows = Colors.Length / columns;
				int width = (this.Width - 4 + colorMargin) / columns;
				int height = (this.Height - 4 + colorMargin) / rows;

				int i = 0;
				for (int y = 0; y < rows; y++)
					for (int x = 0; x < columns; x++)
					{
						Rectangle rect = new Rectangle(2 + (width  *  x) , 2 + height * y, width - colorMargin, height - colorMargin);
						using (Brush brush = new SolidBrush(colors[i]))
							e.Graphics.FillRectangle(brush, rect);

						if (colors[i] == SelectedColor)
						{
							rect.Width--;
							rect.Height--;
							using(Pen pen = new Pen(SystemColors.Highlight, 2))
								e.Graphics.DrawRectangle(pen, rect);
						}
						else
							ControlPaint.DrawBorder3D(e.Graphics, rect, Border3DStyle.RaisedInner);
						i++;
						if (i >= colors.Length)
							return;
					}
			}
		}

        private void SelectionChange(bool left, bool right, bool up, bool down)
        {
            if (colors != null && colors.Length > 0)
            {
                int i = selectedColorIndex;
                if (i-1 >= 0)
                    if (left) { SelectedColor = colors[i - 1]; i--; };
                if (i + 1 < colors.Length)
                    if (right){ SelectedColor = colors[i + 1]; i++; };
                if (i-8 >= 0)
                    if (up) { SelectedColor = colors[i - 8]; i -= 8; };
                if (i + 8 <= colors.Length)
                    if (down) { SelectedColor = colors[i + 8]; i+=8;}
                //     SelectedColor = colors[i];
                selectedColorIndex = i;
                if (ColorChanged != null)
                    ColorChanged(this, new ColorChangedEventArgs(SelectedColor));
                return;
            }
        }
            
        
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            switch (keyData)
            {
                case Keys.Left: { SelectionChange(true, false, false, false); break; }
                case Keys.Right: { SelectionChange(false, true, false, false); break; }
                case Keys.Up: { SelectionChange(false, false, true, false); break; }
                case Keys.Down: { SelectionChange(false, false, false, true); break; }
            }
       //     MessageBox.Show(keyData.ToString());
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			if (colors != null && colors.Length > 0)
			{
				int rows = Colors.Length / columns;
				int width = (this.Width + colorMargin) / columns;
				int height = (this.Height + colorMargin) / rows;

				int i = 0;
				for (int y = 0; y < rows; y++)
					for (int x = 0; x < columns; x++)
					{
						Rectangle rect = new Rectangle(width * x, height * y, width - colorMargin, height - colorMargin);
						if (rect.Contains(e.X, e.Y))
						{
							SelectedColor = colors[i];
                            selectedColorIndex = i;  
                            if (ColorChanged != null)
								ColorChanged(this, new ColorChangedEventArgs(SelectedColor));
							return;
						}
						i ++;
						if (i >= colors.Length)
							return;
					}
			}
		}
	}
}
