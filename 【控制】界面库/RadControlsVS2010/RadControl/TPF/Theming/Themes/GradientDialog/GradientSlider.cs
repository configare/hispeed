using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{
    /// <exclude/>
	[ToolboxItem(false), ComVisible(false)]
	public partial class GradientSlider : UserControl
	{
		GradientColorValue marker;
        Point mousePosition;
        int mouseOffsetFromTheCenter;
		List<GradientColorValue> values = new List<GradientColorValue>();

		public GradientSlider()
		{
			InitializeComponent();
			SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
		}

		/// <summary>
		/// Gets or sets the values
		/// </summary>
		[DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden)]
		public List<GradientColorValue> Values
		{
			get { return values; }
			set { values = value; }
		}
		/// <summary>
		/// Fires when the color has changed
		/// </summary>
		public event ColorChangedEventHandler ColorChanged;

        private double ResizeCoefficient 
        {
            get 
            {
                return 1;
            }
        }

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			Rectangle rect = ClientRectangle;
			rect.Height -= 16;
			rect.Width -= 18;
			rect.X += 8;

			if (values.Count > 0)
			{
				ColorBlend blend = new ColorBlend();

				blend.Colors = new Color[values.Count];
				blend.Positions = new float[values.Count];
				for (int i = 0; i < values.Count; i++)
				{
					blend.Colors[i] = values[i].ColorValue;
					blend.Positions[i] = values[i].ColorPosition;
				}

				if (blend.Colors.Length < 2)
				{
					using (SolidBrush brush = new SolidBrush(blend.Colors[0]))
						e.Graphics.FillRectangle(brush, rect);
				}
				else
				{
					using (LinearGradientBrush brush = new LinearGradientBrush(rect, blend.Colors[0], blend.Colors[values.Count - 1], 0f))
					{
						brush.InterpolationColors = blend;
						e.Graphics.FillRectangle(brush, rect);
					}
                    for(int i=0; i<values.Count; i++){
                        GradientColorValue value = values[i];
					
						Rectangle pointRect = new Rectangle((int)(rect.Width * value.ColorPosition+4), rect.Height+2, 6, 12);
						using (Brush brush = new SolidBrush(value.ColorValue))
							e.Graphics.FillRectangle(brush, pointRect);
                        if (value == marker)
                            e.Graphics.DrawRectangle(new Pen(Brushes.OrangeRed, 1), pointRect);
                        else
                        {
                           e.Graphics.DrawRectangle(new Pen(Brushes.Black, 1), pointRect);
                        }
                    }

				}
			}
			else 
				e.Graphics.FillRectangle(Brushes.Blue, rect);
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
                      			
            Rectangle rect = ClientRectangle;
			rect.Height -= 16;
			rect.Width -= 18;
			rect.X += 8;

            if (e.Y <= rect.Height+1)
            {
                OnMouseDoubleClick(e);
            }

            marker = null;
			for (int i = 1; i < values.Count - 1; i++)
			{
				GradientColorValue value = values[i];

				Rectangle pointRect = new Rectangle((int)(rect.Width * value.ColorPosition + 4), rect.Height+2, 8, 12);
				if (pointRect.Contains(e.X, e.Y))
				{
					marker = value;
					this.Capture = true;
                    mouseOffsetFromTheCenter= (e.X - (int)(rect.Width * value.ColorPosition + 8));
                    mousePosition = new Point(e.X, e.Y);
					Refresh();
					return;
				}
			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if (e.Button == MouseButtons.Left && marker != null)
			{
				Rectangle rect = ClientRectangle;
				rect.Height -= 16;
				rect.Width -= 18;
				rect.X += 8;

				int index = values.IndexOf(marker);
			    
                  
                float position = ((float)e.X-mouseOffsetFromTheCenter - this.Parent.Location.X - this.Location.X) / (rect.Width);
                if (position > values[index - 1].ColorPosition + 0.00999 / ResizeCoefficient && position < values[index + 1].ColorPosition - 0.00999 / ResizeCoefficient)
                {
                    if (position > 0f && position < 1f)
                    {
                        marker.ColorPosition = position;
                        Refresh();

                        if (ColorChanged != null)
                            ColorChanged(this, new ColorChangedEventArgs(marker.ColorValue));
                    }
                }
			
			}
		}
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			Rectangle rect = new Rectangle(ClientRectangle.X - 10, ClientRectangle.Y - 8, ClientRectangle.Width + 500, ClientRectangle.Height + 10);
			if (!rect.Contains(e.X, e.Y) && marker != null && marker != values[0] && marker != values[values.Count - 1] &&Math.Abs(e.X-mousePosition.X)<70)
			{
				this.values.Remove(marker);
				if (ColorChanged != null)
					ColorChanged(this, new ColorChangedEventArgs(marker.ColorValue));
			}
			marker = null;
			Refresh();
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e)
		{
			base.OnMouseDoubleClick(e);

			Rectangle rect = ClientRectangle;
			rect.Height -= 16;
			rect.Width -= 18;
			rect.X += 8;

			GradientColorValue selected = null;

			for (int i = 0; i < values.Count; i++)
			{
				GradientColorValue value = values[i];

				Rectangle pointRect = new Rectangle((int)(rect.Width * value.ColorPosition + 4), rect.Height, 8, 14);
				if (pointRect.Contains(e.X, e.Y))
				{
					selected = value;
					break;
				}
				else if (pointRect.X > e.X)
				{
					if (values.Count >= 4)
					{
						MessageBox.Show("Only four color gradient styles are supported");
						return;
					}

                    Values.Insert(i, new GradientColorValue(Color.White, ((float)e.X - this.Parent.Location.X - this.Location.X) / (rect.Width + 4)));
					selected = Values[i];
					break;
				}
			}

			if (selected != null)
			{
				IRadColorDialog dialogForm = RadColorEditor.CreateColorDialogInstance();

				UserControl colorSelector = RadColorEditor.CreateColorSelectorInstance() as UserControl;
				((IColorSelector)dialogForm.RadColorSelector).SelectedColor = (Color)selected.ColorValue;
                ((IColorSelector)dialogForm.RadColorSelector).OldColor = (Color)selected.ColorValue;

				colorSelector.Dock = DockStyle.Fill;

				if (((Form)dialogForm).ShowDialog() == DialogResult.OK)
					selected.ColorValue = ((IColorSelector)dialogForm.RadColorSelector).SelectedColor;
				Refresh();

				if (ColorChanged != null)
					ColorChanged(this, new ColorChangedEventArgs(selected.ColorValue));
			}
		}
	}
}
