using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using Telerik.WinControls.Design;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// The TextBox control that is hosted by default by RadTextBoxItem.
    /// Children of this calss can be passed to RadTextBoxItem in order to customize the hosted text box.
    /// </summary>
	[ToolboxItem(false)]
	public class HostedTextBoxBase : System.Windows.Forms.TextBox
	{
		// Fields
        private bool allowPrompt = true;
        private string nullText = String.Empty;
        private Color nullColor = SystemColors.GrayText;
        private bool useGenericBorderPaint = false;

        /// <summary>
        /// Gets or sets a color of the null text
        /// </summary>
        [Description("Gets or sets a color of the null text")]
        [Browsable(false)]
        [Category("Appearance")]
        public Color NullTextColor
        {
            get
            {
                return this.nullColor;
            }
            set
            {
                this.nullColor = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show the bottom part of characters, clipped 
        /// due to font name or size particularities
        /// </summary>
        [Description("Gets or sets a value indicating whether to show the bottom part of characters, clipped due to font name or size particularities")]
        [DefaultValue(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public bool UseGenericBorderPaint
        {
            get
            {
                return useGenericBorderPaint;
            }
            set
            {
                useGenericBorderPaint = value;
            }
        }

        [Browsable(true)]
        [Category("Appearance")]
		[Description("Gets or sets the prompt text that is displayed when the TextBox  contains no text.")]
        [Localizable(true)]
		public string NullText
        {
            get 
			{ 
				return nullText;  
			}
            set 
			{
                if (value == null)
                {
                    value = string.Empty;
                }

				nullText = value.Trim(); 
				this.Invalidate(); 
			}
        }

		public bool ShouldSerializeNullText()
		{
			if (String.IsNullOrEmpty(nullText))
				return false;

			return true;
		}

		public void ResetNullText()
		{
			nullText = "";
		}

        [Browsable(true)]
        [Category("Appearance")]
		[Description("Gets or sets whether to use different than SystemColors.GrayText color for the prompt text.")]
        public Color PromptForeColor
        {
            get 
			{ 
				return nullColor; 
			}
            set 
			{ 
				nullColor = value; 
				this.Invalidate(); 
			}
        }

        protected override void OnTextAlignChanged(EventArgs e)
        {
            base.OnTextAlignChanged(e);
            this.Invalidate();
        }

		[Category(RadDesignCategory.BehaviorCategory), 
		Description("Controls whether the text of the edit control can span more than one line"), 
		RefreshProperties(RefreshProperties.All), 
		DefaultValue(false), Localizable(true)]
		public override bool Multiline
		{
			get
			{
				return base.Multiline;
			}
			set
			{
				base.Multiline = value;
			}
		}

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
			if (allowPrompt && this.Text.Length == 0)
			{
				DrawNullText(e.Graphics);
			}
        }

        protected override void WndProc(ref System.Windows.Forms.Message message)
        {
            switch (message.Msg)
            {
                case NativeMethods.WM_SETFOCUS:
                    allowPrompt = false;
                    break;

				case NativeMethods.WM_KILLFOCUS:
                    allowPrompt = true;
                    break;

                case NativeMethods.WM_NCPAINT:
                    {
                        if (this.useGenericBorderPaint && !this.Multiline)
                        {
                            HandleRef hWnd = new HandleRef(this, message.HWnd);
							HandleRef hdc = new HandleRef(this, Telerik.WinControls.NativeMethods.GetWindowDC(hWnd));
                            HandleRef clippingRgn = new HandleRef(this, new IntPtr(Telerik.WinControls.NativeMethods.DCX_WINDOW | Telerik.WinControls.NativeMethods.DCX_INTERSECTRGN));
                            try
                            {
                                if (this.PaintBorders(hWnd.Handle, hdc.Handle, clippingRgn.Handle))
                                {
                                    message.Result = IntPtr.Zero;
                                }
                            }
                            finally
                            {
                                if (hdc.Handle != IntPtr.Zero)
                                {
                                    Telerik.WinControls.NativeMethods.ReleaseDC(hWnd, hdc);
                                }
                            }
                            return;
                        }
                        break;
                    }
                case NativeMethods.WM_PRINT://0x317:
                    {
                        if (this.useGenericBorderPaint && !this.Multiline)
                        {
                            base.WndProc(ref message);
                            if ((((int)message.LParam) & 2) != 0)
                            {
                                HandleRef ref5 = new HandleRef(this, message.HWnd);
                                HandleRef ref6 = new HandleRef(this, message.WParam);
                                this.PaintBordersCore(ref5.Handle, ref6.Handle, IntPtr.Zero);
                            }
                            return;
                        }
                        break;
                    }
            }
            base.WndProc(ref message);
			if (message.Msg == NativeMethods.WM_PAINT && allowPrompt &&
				this.Text.Length == 0 && !this.GetStyle(ControlStyles.UserPaint))
			{
				DrawTextPrompt();
			}
        }

        private bool PaintBorders(IntPtr hWnd, IntPtr hdc, IntPtr hRgn)
        {
            if (hdc == IntPtr.Zero)
            {
                return false;
            }
            using (Graphics graphics = Graphics.FromHdc(hdc))
            {
                Telerik.WinControls.NativeMethods.RECT lpRect = new NativeMethods.RECT();
                if (!Telerik.WinControls.NativeMethods.GetWindowRect(new HandleRef(hWnd, this.Handle), ref lpRect))
                {
                    return false;
                }
                Rectangle windowRectangle = new Rectangle(0, 0, lpRect.right - lpRect.left, lpRect.bottom - lpRect.top);
                Rectangle rectangle2 = windowRectangle;
                rectangle2.Offset(2, 2);
                rectangle2.Width -= 4;
                rectangle2.Height -= 4;
                graphics.SetClip(rectangle2, CombineMode.Exclude);
                this.PaintBordersCore(graphics, windowRectangle);
            }
            return true;
        }

        protected void PaintBorders(PaintEventArgs e)
        {
            Telerik.WinControls.NativeMethods.RECT lpRect = new Telerik.WinControls.NativeMethods.RECT();
            if (Telerik.WinControls.NativeMethods.GetWindowRect(new HandleRef(base.Handle, this.Handle), ref lpRect))
            {
                Rectangle rectangle = new Rectangle(0, 0, lpRect.right - lpRect.left, lpRect.bottom - lpRect.top);

                Rectangle rectangle2 = rectangle;
                rectangle2.Offset(2, 2);
                rectangle2.Width -= 4;
                rectangle2.Height -= 4;
                e.Graphics.SetClip(rectangle2, CombineMode.Exclude);
                this.PaintBordersCore(e.Graphics, rectangle);
            }
        }

        protected void PaintBordersCore(Graphics graphics, Rectangle windowRectangle)
        {
            using (Brush brush = new SolidBrush(this.BackColor))
            {
                graphics.FillRectangle(brush, windowRectangle);
            }
        }

        private void PaintBordersCore(IntPtr hWnd, IntPtr hdc, IntPtr hRgn)
        {
            if (hdc != IntPtr.Zero)
            {
                using (Graphics graphics = Graphics.FromHdc(hdc))
                {
                    Telerik.WinControls.NativeMethods.RECT lpRect = new Telerik.WinControls.NativeMethods.RECT();
                    if (!Telerik.WinControls.NativeMethods.GetWindowRect(new HandleRef(hWnd, this.Handle), ref lpRect))
                    {
                        return;
                    }
                    Rectangle rectangle = new Rectangle(0, 0, lpRect.right - lpRect.left, lpRect.bottom - lpRect.top);
                    graphics.SetClip(Rectangle.Inflate(rectangle, -1, -1), CombineMode.Exclude);
                    this.PaintBorders(new PaintEventArgs(graphics, rectangle));
                }
            }
        }

        /// <summary>
        /// Overload to automatically create the Graphics region before drawing the text prompt
        /// </summary>
        /// <remarks>The Graphics region is disposed after drawing the prompt.</remarks>
        protected virtual void DrawTextPrompt()
        {
            using (Graphics g = this.CreateGraphics())
            {
                DrawNullText(g);
            }
        }

        /// <summary>
        /// Draws the NullText in the client area of the TextBox using the default font and color.
        /// </summary>
		protected virtual void DrawNullText(Graphics graphics)
        {
            TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.Top | 
									TextFormatFlags.EndEllipsis;
            Rectangle rectangle = this.ClientRectangle;

            // Offset the rectangle based on the HorizontalAlignment, 
            // otherwise the display looks a little strange
            
            if (this.RightToLeft == RightToLeft.Yes)
            {
                flags |= TextFormatFlags.RightToLeft;
                switch (this.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        flags = flags | TextFormatFlags.HorizontalCenter;
                        rectangle.Offset(0, 1);
                        break;
                    case HorizontalAlignment.Left:
                        flags = flags | TextFormatFlags.Right;
                        rectangle.Offset(1, 1);
                        break;
                    case HorizontalAlignment.Right:
                        flags = flags | TextFormatFlags.Left;
                        rectangle.Offset(0, 1);
                        break;
                }
            }
            else
            {
                flags &= ~TextFormatFlags.RightToLeft;
                switch (this.TextAlign)
                {
                    case HorizontalAlignment.Center:
                        flags = flags | TextFormatFlags.HorizontalCenter;
                        rectangle.Offset(0, 1);
                        break;
                    case HorizontalAlignment.Left:
                        flags = flags | TextFormatFlags.Left;
                        rectangle.Offset(1, 1);
                        break;
                    case HorizontalAlignment.Right:
                        flags = flags | TextFormatFlags.Right;
                        rectangle.Offset(0, 1);
                        break;
                }                
            }
			TextRenderer.DrawText(graphics, nullText, this.Font, rectangle, nullColor, this.BackColor, flags);
        }
	}
}
