using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Telerik.WinControls
{
    //public delegate void TemplateEventHandler(object sender, TemplateEventArgs e);

    internal partial class ScreenTipPresenter : Form
    {
        private const int DefaultOffset = 25;

        #region Fields

        private int showDelay = 900;
        private Point pivotPoint = Point.Empty;
        private Timer timer = new Timer();
        private TipStates taskbarState = TipStates.Hidden;
        private RadItem activeItem;
        private Control ownerControl;

        #endregion

        #region Constructors

        public ScreenTipPresenter()
            : this(null)
        {
        }

        public ScreenTipPresenter(Control ownerControl)
        {
            InitializeComponent();
            this.ownerControl = ownerControl;
            this.timer.Tick += new EventHandler(OnTimer);
            timer.Enabled = true;
        }

        #endregion

        #region Methods

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = new CreateParams();
                cp.Parent = IntPtr.Zero;

                cp.Style = NativeMethods.WS_POPUP;
                cp.ExStyle = NativeMethods.WS_EX_TOOLWINDOW | NativeMethods.WS_EX_LAYERED | NativeMethods.WS_EX_TOPMOST;
                return cp;
            }
        }

        public void Show(RadItem item, Point pivotPoint)
        {
            this.activeItem = item;
            this.Show(item.ScreenTip, pivotPoint, -1);
        }

        public void Show(IScreenTipContent content, Point pivotPoint, int delay)
        {
            if (pivotPoint.IsEmpty || content == null)
            {
                return;
            }

            if (delay >= 0)
            {
                this.showDelay = delay;
            }
            this.pivotPoint = pivotPoint;

            UpdateScreenTipSizeAndShape(content);
            UpdateScreenTipState();
        }

        public new void Hide()
        {
            if (taskbarState != TipStates.Hidden)
            {
                timer.Stop();
                taskbarState = TipStates.Hidden;
                base.Hide();
                radScreenTipControl1.SetScreenTipElement(null);
            }
        }

        #endregion

        #region Event handlers

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case NativeMethods.WM_NCHITTEST:
                    message.Result = (IntPtr)NativeMethods.HTTRANSPARENT;
                    break;
                default:
                    base.WndProc(ref message);
                    break;
            }
        }

        protected void OnTimer(object obj, EventArgs e)
        {
            if (taskbarState == TipStates.Appearing)
            {
                this.pivotPoint = GetCorrectedLocation(this.pivotPoint);

                NativeMethods.SetWindowPos(new HandleRef(this.activeItem, this.Handle),
                    NativeMethods.HWND_TOPMOST, this.pivotPoint.X, this.pivotPoint.Y, 0, 0,
                    NativeMethods.SWP_SHOWWINDOW | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOSIZE);

                if (this.Visible == false)
                {
                    this.Visible = true;
                }

                timer.Stop();
                this.taskbarState = TipStates.Visible;
            }
        }

        #endregion

        private void UpdateScreenTipSizeAndShape(IScreenTipContent content)
        {
            RadScreenTipElement screenTipElement = (RadScreenTipElement)content;

            if (screenTipElement.TipSize != Size.Empty)
            {
                screenTipElement.MinSize = screenTipElement.TipSize;
                screenTipElement.MaxSize = new Size(screenTipElement.TipSize.Width, screenTipElement.MaxSize.Height);
            }

            Point currentMousePosition = Control.MousePosition;
            Screen currentScreen = Screen.FromPoint(currentMousePosition);

            this.radScreenTipControl1.Size = currentScreen.WorkingArea.Size;
            this.radScreenTipControl1.SetScreenTipElement(screenTipElement);
            this.radScreenTipControl1.LoadElementTree();
            this.radScreenTipControl1.RootElement.InvalidateMeasure(true);
            this.radScreenTipControl1.RootElement.UpdateLayout();
            this.radScreenTipControl1.Size = radScreenTipControl1.RootElement.DesiredSize.ToSize();
            this.Size = this.radScreenTipControl1.Size;

            if (this.radScreenTipControl1.RootElement.Shape != null)
            {
                if (this.Region != null)
                {
                    this.Region.Dispose();
                }
                using (GraphicsPath path = this.radScreenTipControl1.RootElement.Shape.CreatePath(this.DisplayRectangle))
                {
                    this.Region = new Region(path);
                }
            }
        }

        private void UpdateScreenTipState()
        {
            switch (taskbarState)
            {
                case TipStates.Hidden:
                    this.taskbarState = TipStates.Appearing;
                    timer.Interval = this.showDelay;
                    timer.Start();
                    break;

                case TipStates.Appearing:
                    Refresh();
                    break;

                case TipStates.Visible:
                    timer.Stop();
                    Refresh();
                    break;

                case TipStates.Disappearing:
                    timer.Stop();
                    this.taskbarState = TipStates.Visible;
                    Refresh();
                    break;
            }
        }

        private Point GetCorrectedLocation(Point current)
        {
            Point corrected = current;

            if (this.ownerControl != null)
            {
                //Get the current mouse position in screen coordinates.
                Point currentMousePosition = Control.MousePosition;
                //Find the screen which contains the cursor.
                Screen currentScreen = Screen.FromPoint(currentMousePosition);

                int correctedLocationX = current.X;
                int correctedLocationY = current.Y;

                //If the x coordinate of the screentip will cause it to appear outside
                //of the right edge of the current screen, apply an offset to the x coordinate of the
                //screen tip.
                if (corrected.X + this.Size.Width > currentScreen.WorkingArea.Right)
                {
                    correctedLocationX = currentScreen.WorkingArea.Right - this.Size.Width - DefaultOffset;
                }

                //If the y coordinate of the screentip will cause it to appear outside
                //of the bottom edge of the current screen, apply an offset to the y coordinate of the
                //screen tip.
                if (corrected.Y + this.Size.Height > currentScreen.WorkingArea.Bottom)
                {
                    correctedLocationY = currentScreen.WorkingArea.Bottom - this.Size.Height - DefaultOffset;
                }

                //If the x coordinate of the screentip will cause it to appear outside
                //of the left edge of the current screen, apply an offset to the x coordinate of the
                //screen tip.
                if (corrected.X - this.Size.Width < currentScreen.WorkingArea.Left)
                {
                    correctedLocationX = currentScreen.WorkingArea.Left + DefaultOffset;
                }

                //If the y coordinate of the screentip will cause it to appear outside
                //of the top edge of the current screen, apply an offset to the y coordinate of the
                //screen tip.
                if (corrected.Y - this.Size.Height < currentScreen.WorkingArea.Top)
                {
                    correctedLocationY = currentScreen.WorkingArea.Top + DefaultOffset;
                }


                Rectangle screenTipBounds = new Rectangle(new Point(correctedLocationX, correctedLocationY), this.Size);

                //If the corrected bounds rectangle contains the mouse location, apply another offset
                //so that the screen tip is not immediatelly closed because of the mouse cursor being on it.
                if (screenTipBounds.Contains(currentMousePosition))
                {
                    int mousePositionInScreenTipX = currentMousePosition.X - screenTipBounds.X;
                    int mousePositionInScreenTipY = currentMousePosition.Y - screenTipBounds.Y;

                    //If the horizontal offset of the cursor within the screen tip bounds rectangle is
                    //not bigger than the vertical offset, move the screentip horizontally.
                    //Otherwise move it vertically.
                    if (!(mousePositionInScreenTipX > mousePositionInScreenTipY))
                    {
                        //If we can apply a positive horizontal offset to the screen tip so that
                        //we escape from the mouse, do it. Otherwise: negative offset.
                        if (currentMousePosition.X + this.Width + DefaultOffset < currentScreen.WorkingArea.Right)
                        {
                            correctedLocationX = currentMousePosition.X + DefaultOffset;
                        }
                        else
                        {
                            correctedLocationX = currentMousePosition.X - this.Width - DefaultOffset;
                        }
                    }
                    else
                    {
                        //If we can apply a positive vertical offset to the screen tip so that
                        //we escape from the mouse, do it. Otherwise: negative offset.
                        if (currentMousePosition.Y + this.Height + DefaultOffset < currentScreen.WorkingArea.Bottom)
                        {
                            correctedLocationY = currentMousePosition.Y + DefaultOffset;
                        }
                        else
                        {
                            correctedLocationY = currentMousePosition.Y - this.Height - DefaultOffset;
                        }
                    }
                }

                corrected = new Point(correctedLocationX, correctedLocationY);


            }

            //Rectangle desk = Screen.GetBounds(this);
            //int xOffset = this.Size.Width + DefaultOffset;
            //int yOffset = this.Size.Height + DefaultOffset;
            //Point lpt = new Point(desk.Right, desk.Bottom);
            //if (this.ownerControl != null)
            //{
            //    xOffset += this.ownerControl.Size.Width;
            //    yOffset += this.ownerControl.Size.Height;
            //    lpt = current;
            //}

            //if (current.X + this.Size.Width > desk.Right)
            //{
            //    corrected.X = lpt.X - xOffset;
            //}
            //if (this.pivotPoint.Y + this.Size.Height > desk.Bottom)
            //{
            //    corrected.Y = lpt.Y - yOffset;
            //}

            return corrected;
        }
    }
}