using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using System.Collections;
using System.Threading;
using System.IO;
using System.Timers;
using System.Diagnostics;

namespace Telerik.WinControls.UI
{ 


	/// <summary>
	/// Creates a TextBox that can have a Transparent Background.
	/// </summary>
	internal class RadTransparentTextBox : System.Windows.Forms.TextBox
	{
		public RadTransparentTextBox()
		{
			this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			this.internalAlphaBackColor = this.BackColor;
			this.BackColor = Color.Transparent;
			
			OwnerDefWndProcHandler = new DefWndProcEventHandler(this.DefWndProc);

			internalBGSet = false;
			drawCaret = false;
			selectingText = false;

			blinkCaretTimer = new System.Timers.Timer(500);
			blinkCaretTimer.Elapsed += new ElapsedEventHandler(BlinkCaretTimer_Elapsed);
			blinkCaretTimer.AutoReset = true;

			backPanel = new BackLightPanel(this);
			this.Controls.Add(backPanel);
			internalOpacity = 255;
		}

		// Fields
		private delegate void DefWndProcEventHandler(ref Message M);
		//private Utilities TBUtils;

		private ColorMap[] CMap;
		private ImageAttributes IAttribs;
		private BackLightPanel backPanel;
		private Bitmap clientRegionBitmap;
		private PointF caretPosition;
		private System.Timers.Timer blinkCaretTimer;
		private bool drawCaret;
		private bool selectingText;
		private Color internalAlphaBackColor;
		private bool internalBGSet;
		private int internalOpacity;
		//This is a delegate so the AlphaPanel can pass mouse messages to the AlphaTextBox
		protected internal Delegate OwnerDefWndProcHandler;

		#region Private Methods

		///// <summary>
		///// Captures the ClientRegion ONLY of this AlphaTextBox. (Text Area)
		///// </summary>
		///// <returns>Returns a Bitmap containing the ClientRegion.</returns>
		//private Bitmap CaptureClientRegion()
		//{
		//    if (this.Parent == null)
		//    {
		//        return null;
		//    }
		//    internalBGSet=true;

		//    this.BackColor=AlphaBackColor;
		//    Bitmap client = TelerikPaintHelper.GetControlBmp(this.Parent, this.Bounds, NativeMethods.PRF_CHILDREN | NativeMethods.PRF_OWNED |
		//        NativeMethods.PRF_CLIENT | NativeMethods.PRF_ERASEBKGND);
		//    internalBGSet=false;
		//    this.BackColor=Color.Transparent;
		//    return client;
		//}

		///// <summary>
		///// Captures the NonClient area of the AlphaTextBox (Border and ScrollBars)
		///// </summary>
		///// <returns>Returns a Bitmap containing the NonClientRegion</returns>
		//private Bitmap CaptureNonClientRegion()
		//{
		//    if (this.Parent == null)
		//    {
		//        return null;
		//    }
		//    Bitmap nonClient = TelerikPaintHelper.GetControlBmp(this.Parent, this.Bounds, NativeMethods.PRF_NONCLIENT | NativeMethods.PRF_ERASEBKGND);
		//    return nonClient;
		//}

		/// <summary>
		/// Captures the ClientRegion ONLY of this AlphaTextBox. (Text Area)
		/// </summary>
		/// <returns>Returns a Bitmap containing the ClientRegion.</returns>
		private Bitmap CaptureClientRegion()
		{
			internalBGSet = true;
			Bitmap client = new Bitmap(this.Width, this.Height);
			this.BackColor = AlphaBackColor;
			Graphics g = Graphics.FromImage(client);
			IntPtr intp = g.GetHdc();

			//get the client region
			this.SendMessage(NativeMethods.WM_PRINT, intp, (IntPtr)(NativeMethods.PRF_CLIENT | NativeMethods.PRF_ERASEBKGND));
			g.ReleaseHdc(intp);
			g.Dispose();

			internalBGSet = false;
			this.BackColor = Color.Transparent;
			return client;
		}//CaptureClientRegion

		/// <summary>
		/// Captures the NonClient area of the AlphaTextBox (Border and ScrollBars)
		/// </summary>
		/// <returns>Returns a Bitmap containing the NonClientRegion</returns>
		private Bitmap CaptureNonClientRegion()
		{
			Bitmap nonClient = new Bitmap(this.Width, this.Height);
			IntPtr intp;

			Graphics g = Graphics.FromImage(nonClient);
			intp = g.GetHdc();

			//get the non client region
			this.SendMessage(NativeMethods.WM_PRINT, intp, (IntPtr)(NativeMethods.PRF_NONCLIENT | NativeMethods.PRF_ERASEBKGND));
			g.ReleaseHdc(intp);
			g.Dispose();

			return nonClient;
		}//CaptureNonClientRegion

		/// <summary>
		/// Updates the ClientRegion of the AlphaTextBox and
		/// draws it to the screen.
		/// </summary>
		private void UpdateRegion()
		{
			if (clientRegionBitmap != null)
			{
				clientRegionBitmap.Dispose();
			}
			clientRegionBitmap= MapColors(AlphaBackColor, Color.FromArgb(Opacity, AlphaBackColor), CaptureClientRegion(), true);
			if (clientRegionBitmap != null)
			{
				backPanel.BackgroundImage=(Bitmap)clientRegionBitmap.Clone();
			}
			//get the caret position
			SetCaret();

			GC.Collect();
		}

		/// <summary>
		/// Determines the CaretPosition in this AlphaTextBox.
		/// </summary>
		private void SetCaret()
		{
			//if the text is blank, set the cursor to the default top left corner and return.
			if(this.Text==string.Empty)
			{
				caretPosition=new PointF(2F, 2F);
				DrawCaretToBitmap();
				return;
			}

			int selectionIndex=this.SelectionStart+this.SelectionLength;
			bool lastIsNLine=false;
			float fntWidth=0;
			string textToMeasure = string.Empty;
			int position=0;

			//the cursor should be at the end of the text, but no new line is there
			if(selectionIndex==this.TextLength && this.Text[this.Text.Length-1]!='\n')
				selectionIndex--;
			else
				if(selectionIndex==this.TextLength && this.Text[this.Text.Length-1]=='\n')
			{
				//the cursor is at the end of the text and a new line is there.
				//Decrement selection index to get the YCoordinate.
				lastIsNLine=true;
				selectionIndex--;
			}

			//The xCoordinate is stored in the low order word and the yCoordinate is 
			//stored in the high order word of the return value.  
			position = ((IntPtr)this.SendMessage(NativeMethods.EM_POSFROMCHAR, (IntPtr)selectionIndex, IntPtr.Zero)).ToInt32();

			if(this.SelectionStart!=this.TextLength)
			{
				caretPosition=new PointF((position & 0xFF),(position >> 16) & 0xFF);
				DrawCaretToBitmap();
				return;
			}
			else
			{
				textToMeasure = this.Text[selectionIndex].ToString();
				using (Graphics g = this.CreateGraphics())
				{
					fntWidth = g.MeasureString(textToMeasure, this.Font).Width;
				}
				caretPosition=new PointF((position & 0xFF) + (fntWidth/2),(position >> 16) & 0xFF);
			}
			if (lastIsNLine)
			{
				caretPosition = new PointF(2F, caretPosition.Y + this.FontHeight);
			}
			DrawCaretToBitmap();
		}

		protected internal object SendMessage(int Msg, IntPtr WParams, IntPtr LParams)
		{
			Message m = Message.Create(this.Handle, Msg, WParams, LParams);
			object[] vals ={ m };
			//this.OwnerDefWndProcHandler.DynamicInvoke(vals);
			//this.DefWndProc(ref m);
			base.WndProc(ref m);
			Debug.WriteLine("invoke:" + m.ToString());
			return ((Message)vals[0]).Result;
			//return m.Result;
		}

		/// <summary>
		/// Draws the Caret to the bitmap of the AlphaPanel object
		/// for this AlphaTextBox
		/// </summary>
		private void DrawCaretToBitmap()
		{
			if (backPanel.BackgroundImage == null)
			{
				return;
			}
			using (Graphics g = Graphics.FromImage(backPanel.BackgroundImage))
			{
				if (drawCaret)
				{
					g.FillRectangle(new SolidBrush(this.ForeColor), caretPosition.X, caretPosition.Y,
						this.Font.SizeInPoints / 5, this.FontHeight);
				}
				else
				{
					backPanel.BackgroundImage = (Bitmap)clientRegionBitmap.Clone();
				}
			}
			backPanel.Refresh();
		}

		/// <summary>
		/// Draws the Color specified by New at every instance of the Color specified by Old
		/// according to the Bitmap specified by Bmp.
		/// </summary>
		/// <param name="oldColor">The Color to be replace.</param>
		/// <param name="newColor">The Color with which to replace Old.</param>
		/// <param name="bmp">The Bitmap that needs to be mapped.</param>
		/// <param name="shouldDispose">true if the bitmap is to be disposed, false otherwise.</param>
		/// <returns>Returns a copy of Bmp with the Color New mapped to Color Old.</returns>
		protected internal Bitmap MapColors(Color oldColor, Color newColor, Bitmap bmp, bool shouldDispose)
		{
			if (bmp == null)
			{
				return null;
			}
			Bitmap tmpBmp = new Bitmap(bmp.Width, bmp.Height);
			CMap = new ColorMap[1];
			CMap[0] = new ColorMap();
			CMap[0].OldColor = oldColor;
			CMap[0].NewColor = newColor;
			IAttribs = new ImageAttributes();
			IAttribs.SetRemapTable(CMap, ColorAdjustType.Bitmap);
			using (Graphics g = Graphics.FromImage(tmpBmp))
			{
				g.DrawImage(bmp, new Rectangle(new Point(0, 0), new Size(bmp.Width, bmp.Height)), 0, 0,
					bmp.Width, bmp.Height, GraphicsUnit.Pixel, IAttribs);
			}
			if (shouldDispose)
			{
				bmp.Dispose();
			}
			return tmpBmp;
		}

		private void BlinkCaretTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			//if selecting text, we dont want the cursor to display. (From SHIFT+<-OR->)
			if(!selectingText && this.SelectionLength>0)
			{
				selectingText=true;
				drawCaret=false;
				UpdateRegion();
			}
			else if(!selectingText)
			{
				drawCaret=!drawCaret;
				DrawCaretToBitmap();
			}
		}

		#endregion

		#region Overrides
		/// <summary>
		/// This will always return Color.Transparent.
		/// Use AlphaBackColor to return the true BackColor.
		/// </summary>
		public override Color BackColor
		{
			//This override always sets the BackColor property to
			//transparent unless an internal edit is happening.

			get
			{
				return base.BackColor;
			}
			set
			{
				if(!internalBGSet)
					base.BackColor = Color.Transparent;
				else
				{
					base.BackColor=value;
				}
			}
		}

		public override bool Multiline
		{
			get
			{
				return base.Multiline;
			}
			set
			{
				base.Multiline = value;
				UpdateRegion();
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus (e);
			blinkCaretTimer.Start();
			UpdateRegion();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus (e);
			drawCaret=false;
			UpdateRegion();
			blinkCaretTimer.Stop();
		}

		protected override void OnForeColorChanged(EventArgs e)
		{
			if(!internalBGSet)
			{
				base.OnForeColorChanged (e);
				UpdateRegion();
			}
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged (e);
			UpdateRegion();
		}

		protected override void OnBorderStyleChanged(EventArgs e)
		{
			base.OnBorderStyleChanged (e);
			UpdateRegion();
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged (e);
			backPanel.Size=this.ClientSize;
			UpdateRegion();
		}

		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged (e);
			backPanel.Visible=this.Visible;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged (e);
			drawCaret=true;
			if (!blinkCaretTimer.Enabled)
			{
				blinkCaretTimer.Start();
			}
			UpdateRegion();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp (e);
			if(this.SelectionLength==0 && selectingText)
			{
				selectingText=false;
				drawCaret=true;
			}
			if (!blinkCaretTimer.Enabled)
			{
				blinkCaretTimer.Start();
			}
			UpdateRegion();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown (e);

			//We need this on KeyDown so if a key is held down, the
			//screen will scroll accordingly
			if(e.KeyCode==Keys.Left || e.KeyCode==Keys.Up || e.KeyCode==Keys.Down || 
				e.KeyCode==Keys.Right || e.KeyCode==Keys.PageDown || e.KeyCode==Keys.PageUp || 
				e.KeyCode==Keys.Home || e.KeyCode==Keys.End)
			{
				drawCaret=true;
				if (!blinkCaretTimer.Enabled)
				{
					blinkCaretTimer.Start();
				}
				UpdateRegion();
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp (e);
			//only UpdateRegion under these conditions.
			//Keys Changing text will UpdateRegion in OnTextChanged
			if(e.KeyCode==Keys.Left || e.KeyCode==Keys.Up || e.KeyCode==Keys.Down || 
				e.KeyCode==Keys.Right || e.KeyCode==Keys.PageDown || e.KeyCode==Keys.PageUp || 
				e.KeyCode==Keys.Home || e.KeyCode==Keys.End)
			{
				UpdateRegion();
			}
			else if(this.SelectionLength==0 && selectingText)
			{
				selectingText=false;
				drawCaret=true;

				if (!blinkCaretTimer.Enabled)
				{
					blinkCaretTimer.Start();
				}
				UpdateRegion();
			}
		}

		protected override void WndProc(ref Message m)
		{
			base.WndProc (ref m);

			//Debug.WriteLine(m.ToString());
			if (m.Msg == NativeMethods.WM_HSCROLL || m.Msg == NativeMethods.WM_VSCROLL ||
				m.Msg == NativeMethods.WM_MOUSEWHEEL)
			{
				//stop the caret from being drawn when scrolling or it will end up
				//in strange places.
				drawCaret=false;
				blinkCaretTimer.Stop();
				UpdateRegion();
			}
			else if(m.Msg==NativeMethods.WM_MOUSEMOVE && (int)m.WParam!=0)
			{
				//WParam holds key information, like mouse button click, etc.
				//if it is not 0, then update.
				drawCaret=false;
				selectingText=true;
				UpdateRegion();
			}
		}

		#endregion

		#region Public Methods and Variables

		//This is the amount of transparency applied to the background.
		//0=Totally transparent; 255=Totally Opaque
		[Category("Appearance"),Description("The Alpha Amount, or transparency amount, applied to the background."), Browsable(true),
		 DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]

		public int Opacity
		{
			get
			{
				return internalOpacity;
			}
			set
			{
				if (value > 255 || value < 0)
				{
					throw (new Exception("AlphaAmount must be between 0 and 255"));
				}
				else
				{
					internalOpacity = value;
				}
				UpdateRegion();
			}
		}

		//setting the BackColor property will not do anything for the AlphaTextBox because it is always
		//set to be transparent.  You must set AlphaBackColor instead.
		[Category("Appearance"),Description("The visible background color for the AlphaTextBox."), Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public Color AlphaBackColor
		{
			get
			{
				return internalAlphaBackColor;
			}
			set
			{
				internalAlphaBackColor=value;
				UpdateRegion();
			}
		}

		///// <summary>
		///// Returns a "screen shot" of the AlphaTextBox
		///// </summary>
		///// <returns>Returns a Bitmap containing the "screen shot"</returns>
		//public Bitmap GetScreenShot()
		//{
		//    Bitmap tmpB=CaptureNonClientRegion();
		//    Bitmap tmpB2=TBUtils.MapColors(AlphaBackColor, Color.FromArgb(AlphaAmount, AlphaBackColor), CaptureClientRegion(), true);
		//    Graphics g=Graphics.FromImage(tmpB);
		//    g.DrawImage(tmpB2, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
		//    g.Dispose();
		//    tmpB2.Dispose();

		//    return tmpB;
		//}//GetScreenShot
		#endregion

		#region Nested Types

		protected class BackLightPanel : Panel
		{
			protected internal BackLightPanel(RadTransparentTextBox owner)
			{
				this.Size=owner.Size;
				this.Location=new Point(0, 0);
				this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
				this.SetStyle(ControlStyles.UserPaint, true);
				this.SetStyle(ControlStyles.DoubleBuffer,true);
				this.SetStyle(ControlStyles.Selectable, false);
				this.owner = owner;
			}

			// Fields
			private RadTransparentTextBox owner;

			protected override void WndProc(ref Message message)
			{
				base.WndProc (ref message);

				switch (message.Msg)
				{
					case NativeMethods.WM_MOUSEMOVE:
					case NativeMethods.WM_LBUTTONDOWN:
					case NativeMethods.WM_LBUTTONUP:
					case NativeMethods.WM_LBUTTONDBLCLK:
					case NativeMethods.WM_MOUSELEAVE:
					case NativeMethods.WM_RBUTTONDOWN:
					case NativeMethods.WM_MOUSEACTIVATE:
						this.SendMessage(message.Msg, message.WParam, message.LParam);
						break;
				}
			}

			protected internal void SendMessage(int msg, IntPtr wParams, IntPtr lParams)
			{
				Message m = Message.Create(this.owner.Handle, msg, wParams, lParams);
				object[] vals ={ m };
				this.owner.SendMessage(msg, wParams, lParams);// OwnerDefWndProcHandler.DynamicInvoke(vals);
			}
		}

		#endregion
	}

	//#region Utilities Class
	//public class Utilities
	//{
	//    /// <summary>
	//    /// Constructor for Utilities
	//    /// </summary>
	//    /// <param name="DMaster">The delegate to the Master Control's DefWndPrc function.</param>
	//    /// <param name="Master">The Master Control.</param>
	//    protected internal Utilities(Delegate DMaster, Control Master)
	//    {
	//        MasterDelegate=DMaster;
	//        MasterControl=Master;
	//    }//Constructor

	//    #region Win32 Structs and methods
	//    [ StructLayout( LayoutKind.Sequential)]
	//    private struct STRUCT_RECT 
	//    {
	//        public int left;
	//        public int top;
	//        public int right;
	//        public int bottom;
	//    }//STRUCT_RECT

	//    [ StructLayout( LayoutKind.Sequential)]
	//    private struct STRUCT_CHARRANGE
	//    {
	//        public int cpMin;
	//        public int cpMax;
	//    }//STRUCT_CHARRANGE

	//    [ StructLayout( LayoutKind.Sequential)]
	//    private struct STRUCT_FORMATRANGE
	//    {
	//        public IntPtr hdc; 
	//        public IntPtr hdcTarget; 
	//        public STRUCT_RECT rc; 
	//        public STRUCT_RECT rcPage; 
	//        public STRUCT_CHARRANGE chrg; 
	//    }//STRUCT FORMATRANGE

	//    [StructLayout(LayoutKind.Sequential)]
	//    private struct CHARFORMAT2 
	//    {
	//        public int cbSize;
	//        public int dwMask;
	//        public int dwEffects;
	//        public int yHeight;
	//        public int yOffset;
	//        public int crTextColor;
	//        public byte bCharSet;
	//        public byte bPitchAndFamily;
	//        [MarshalAs(UnmanagedType.ByValTStr, SizeConst=32)] 
	//        public string szFaceName;
	//        public UInt16 wWeight;
	//        public UInt16 sSpacing;
	//        public int crBackColor;
	//        public int lcid;
	//        public int dwReserved;
	//        public UInt16 sStyle;
	//        public UInt16 wKerning;
	//        public byte bUnderlineType;
	//        public byte bAnimation;
	//        public byte bRevAuthor;
	//        public byte bReserved1;
	//    }//struct charformat2 

	//    private int ToTwips(float amt)
	//    {
	//        return (int)(amt*14.4F);
	//    }//ToTwips
	//    #endregion

	//    #region Private Globals

	//    private Control MasterControl;
	//    private Delegate MasterDelegate;


	//    #endregion

	//    #region Windows Message Constants (Thank You Google)

	//    protected internal readonly int EM_GETLINECOUNT=0xBA;
	//    protected internal readonly int EM_LINEINDEX=0xBB;
	//    protected internal readonly int EM_LINELENGTH=0xC1;
	//    protected internal readonly int EM_LINEFROMCHAR=0xC9;
	//    protected internal readonly int EM_GETSEL=0xB0;
	//    protected internal readonly int EM_GETFIRSTVISIBLELINE=0xCE;
	//    protected internal readonly int EM_SETEVENTMASK=0x431;
	//    protected internal readonly int EM_POSFROMCHAR=0xD6;
	//    protected internal readonly int EN_UPDATE=0x0400;
	//    protected internal readonly int EM_MOUSESELECT=0xFF;
	//    protected internal readonly int WM_PRINT=0x317;
	//    protected internal readonly int PRF_ERASEBKGND=0x8;
	//    protected internal readonly int PRF_CLIENT=0x4;
	//    protected internal readonly int PRF_NONCLIENT=0x2;
	//    protected internal readonly int WM_MOUSEMOVE=0x200;
	//    protected internal readonly int WM_LBUTTONDOWN=0x201;
	//    protected internal readonly int WM_LBUTTONUP=0x202;
	//    protected internal readonly int WM_RBUTTONDOWN=0x204;
	//    protected internal readonly int WM_LBUTTONDBLCLK=0x203;
	//    protected internal readonly int WM_MOUSELEAVE=0x2A3;
	//    protected internal readonly int WM_MOUSEACTIVATE=0x21;
	//    protected internal readonly int WM_HSCROLL=0x114;
	//    protected internal readonly int	WM_VSCROLL=0x115;
	//    protected internal readonly int WM_MOUSEWHEEL=0x20A;
	//    protected internal readonly int WM_SETREDRAW=0xB;
	//    protected internal readonly int WM_KEYDOWN=0x100;
	//    protected internal readonly int EM_FORMATRANGE=0x439;
	//    protected internal readonly int EM_SETCHARFORMAT=0x400+68;
	//    protected internal readonly int EM_GETCHARFORMAT=0x400+58;
	//    protected internal readonly int CFM_BACKCOLOR=0x4000000;
	//    protected internal readonly int CFM_COLOR=0x40000000;
	//    protected internal readonly int CFE_AUTOBACKCOLOR=0x4000000;
	//    protected internal readonly int SCF_SELECTION=0x1;
	//    protected internal readonly int MK_LBUTTON=0x1;
	//    protected internal readonly int SB_LINEUP=0x0;
	//    protected internal readonly int SB_LINEDOWN=0x1;
	//    #endregion

	//    #region Protected Internal Functions

	//    /// <summary>
	//    /// Draws the contents of the parent RichTextBox into the specified
	//    /// device context.
	//    /// </summary>
	//    /// <param name="g">The graphics object that will draw to the device context.</param>
	//    /// <param name="startChar">The first charachter index to capture.</param>
	//    /// <param name="endChar">The last charachter index to capture.</param>
	//    protected internal void FormatRange(Graphics g, int startChar, int endChar)
	//    {
	//        STRUCT_CHARRANGE charRange;
	//        charRange.cpMin=startChar;
	//        charRange.cpMax=endChar;

	//        STRUCT_RECT rc;
	//        rc.top=0;
	//        rc.bottom=ToTwips(MasterControl.ClientSize.Height+40);
	//        rc.left=0;

	//        if(MasterControl.Size.Width-MasterControl.ClientSize.Width==20)//VScrollbar present
	//            rc.right=ToTwips(MasterControl.ClientSize.Width+(MasterControl.ClientSize.Width/80F)+4);
	//        else
	//            //VScrollbar not present
	//            rc.right=ToTwips(MasterControl.ClientSize.Width+(MasterControl.ClientSize.Width/100F)+5);
	
	//        STRUCT_RECT rcPage;
	//        rcPage.top=0;
	//        rcPage.bottom=ToTwips(MasterControl.Size.Height);
	//        rcPage.left=0;
	//        rcPage.right=ToTwips(MasterControl.Size.Width);
	//        IntPtr hdc = g.GetHdc();

	//        //This is what specifies all the information
	//        //for drawing to the bitmap
	//        STRUCT_FORMATRANGE forRange;
	//        forRange.chrg=charRange;
	//        forRange.hdc=hdc;
	//        forRange.hdcTarget=hdc;
	//        forRange.rc=rc;
	//        forRange.rcPage=rcPage;

	//        //We have to send and IntPtr as the lParam.  You cant simply 
	//        //make a pointer to forRange, so allocate memory and Marshal
	//        //it to an IntPtr
	//        IntPtr lParam=Marshal.AllocCoTaskMem(Marshal.SizeOf(forRange)); 
	//        Marshal.StructureToPtr(forRange, lParam, false);

	//        SendMessageToMaster(EM_FORMATRANGE, (IntPtr)1, lParam, 1);
	//        SendMessageToMaster(EM_FORMATRANGE, IntPtr.Zero, IntPtr.Zero, -1);

	//        //release resources
	//        Marshal.FreeCoTaskMem(lParam);
	//        g.ReleaseHdc(hdc);
	//    }//FormatRange

	//    /// <summary>
	//    /// Sets the highlight of the selected RichText.
	//    /// Should only be used for 1 charachter.
	//    /// </summary>
	//    /// <param name="back">The bacground color to use.</param>
	//    /// <param name="fore">The text color to use.</param>
	//    protected internal void SetRTHighlight(Color back, Color fore)
	//    {
	//        CHARFORMAT2 cf2=new CHARFORMAT2();
	//        cf2.dwMask=(CFM_BACKCOLOR|CFM_COLOR);
	//        cf2.cbSize=Marshal.SizeOf(cf2);
	//        cf2.crBackColor=ColorTranslator.ToWin32(back);
	//        cf2.crTextColor=ColorTranslator.ToWin32(fore);

	//        IntPtr lParam=Marshal.AllocCoTaskMem(cf2.cbSize); 
	//        Marshal.StructureToPtr(cf2, lParam, false);

	//        SendMessageToMaster(EM_SETCHARFORMAT, (IntPtr)SCF_SELECTION, lParam, 1);

	//        Marshal.FreeCoTaskMem(lParam);
	//    }//SetRTHighlight

	//    /// <summary>
	//    /// Gets the background and foreground color of selected RichText.
	//    /// Should only be used for 1 charachter.
	//    /// </summary>
	//    /// <param name="back">Will recieve the background color.</param>
	//    /// <param name="fore">Will recieve the Text Color.</param>
	//    /// <param name="AlphaBackColor">The color to return if the back color is transparent.</param>
	//    protected internal void GetRTHighlight(ref Color back, ref Color fore, Color AlphaBackColor)
	//    {
	//        CHARFORMAT2 cf2=new CHARFORMAT2();
	//        cf2.cbSize=Marshal.SizeOf(cf2);

	//        IntPtr lParam=Marshal.AllocCoTaskMem(cf2.cbSize);
	//        Marshal.StructureToPtr(cf2, lParam, false);

	//        //return lParam so you can get the modified version
	//        lParam=(IntPtr)SendMessageToMaster(EM_GETCHARFORMAT, (IntPtr)SCF_SELECTION, lParam, 3);

	//        //assign the modified memory contents to cf2 so the
	//        //properties will be available
	//        cf2=(CHARFORMAT2)Marshal.PtrToStructure(lParam, typeof(CHARFORMAT2));

	//        //if crBackColor is 0, it is probably picking up transparency, so 
	//        //set crBackColor to AlphaBackColor so it can be blended.
	//        back=cf2.crBackColor==0 ? AlphaBackColor : ColorTranslator.FromWin32(cf2.crBackColor);
	//        fore=ColorTranslator.FromWin32(cf2.crTextColor);

	//        Marshal.FreeCoTaskMem(lParam);
	//    }//GetRTHighlight

	//    /// <summary>
	//    /// This sends a message to the Master AlphaControl (The control
	//    /// this panel is owned by) via the MasterDelegate function.
	//    /// </summary>
	//    /// <param name="Msg">An integer specifying the Windows Message to send.</param>
	//    /// <param name="WParams">An IntPtr specifying the WParameters.</param>
	//    /// <param name="LParams">An IntPtr specifying the LParameters</param>
	//    /// <param name="ReturnInstance">Returns: 0: The Message (Message)
	//    /// 1: Message Return Value (IntPtr) 2: WParams (IntPtr) 3: LParams (IntPtr)</param>
	//    /// <returns>An object specified by ReturnInstance.  If ReturnInstance is not acceptable, -1 is returned.</returns>
	//    protected internal object SendMessageToMaster(int Msg, IntPtr WParams, IntPtr LParams, int ReturnInstance)
	//    {
	//        Message m=Message.Create(MasterControl.Handle, Msg, WParams, LParams);
	//        object[] vals={m};
	//        MasterDelegate.DynamicInvoke(vals);

	//        switch(ReturnInstance)
	//        {
	//            case 0:
	//                return vals[0];
				
	//            case 1:
	//                return ((Message)vals[0]).Result;

	//            case 2:
	//                return ((Message)vals[0]).WParam;

	//            case 3:
	//                return ((Message)vals[0]).LParam;

	//            default:
	//                return -1;
	//        }//switch
	//    }


	//    #endregion
	//}//Class Utilities

	//#endregion
}
