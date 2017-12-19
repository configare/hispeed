using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Paint;
using System.ComponentModel;
using System.Reflection;

namespace Telerik.WinControls.UI
{
    /// <summary>
    /// This class represents the container which holds
    /// all controls put in a <see cref="Telerik.WinControls.UI.RadScrollablePanel"/>
    /// control. The scrolling support comes from this container.
    /// </summary>
    [ToolboxItem(false)]
    public class RadScrollablePanelContainer : System.Windows.Forms.Panel
    {
        #region Fields

        private RadScrollablePanel parentPanel;

        #endregion

        #region Events

        public event ScrollbarSynchronizationNeededEventHandler ScrollBarSynchronizationNeeded;

        #endregion

        #region Ctor

        /// <summary>
        /// Creates an instance of the <see cref="Telerik.WinControls.UI.RadScrollablePanelContainer"/>
        /// class. This constructor is used by the Visual Studio Designer.
        /// </summary>
        public RadScrollablePanelContainer() : this(null)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="Telerik.WinControls.UI.RadScrollablePanelContainer"/>
        /// class.
        /// </summary>
        /// <param name="parentPanel">An instance of the <see cref="Telerik.WinControls.UI.RadScrollablePanel"/>
        /// class which represents the owner of this container.</param>
        public RadScrollablePanelContainer(RadScrollablePanel parentPanel)
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.Selectable, true);
            this.parentPanel = parentPanel;
        }

        #endregion

        #region Service methods

        protected virtual object GetFieldValue(object instance, string fieldName)
        {
            FieldInfo fieldInfo = instance.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            return fieldInfo.GetValue(instance);
        }

        #endregion

        #region Overrides

        protected override Point ScrollToControl(Control activeControl)
        {
            Point result = base.ScrollToControl(activeControl);
            if (this.ScrollBarSynchronizationNeeded != null)
            {
                ScrollbarSynchronizationNeededEventArgs args = new ScrollbarSynchronizationNeededEventArgs();
                args.ActiveControl = activeControl;
                args.ScrolledLocation = result;
                this.ScrollBarSynchronizationNeeded(this, args);
            }
            return result;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            Padding ncMargin = this.parentPanel.NCMargin;
            RadGdiGraphics radGraphics = new RadGdiGraphics(g);
            radGraphics.TranslateTransform(-ncMargin.Left, -ncMargin.Top);
            this.parentPanel.RootElement.Paint(radGraphics, e.ClipRectangle, 0, new SizeF(1, 1), true);
            radGraphics.TranslateTransform(ncMargin.Left, ncMargin.Top);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_NCCALCSIZE:
                    {
                        if (this.AutoScroll)
                        {
                            this.OnWMNCCalcSize(ref m);
                        }
                        else
                        {
                            base.WndProc(ref m);
                        }
                        break;
                    }
                default:
                    {
                        base.WndProc(ref m);
                        break;
                    }
            }
        }

        #endregion

        #region WndProc handlers

        private void OnWMNCCalcSize(ref System.Windows.Forms.Message m)
        {
            if (m.WParam == new IntPtr(1))
            {
                NativeMethods.NCCALCSIZE_PARAMS ncCalcSizeParams = new NativeMethods.NCCALCSIZE_PARAMS();
                ncCalcSizeParams = (NativeMethods.NCCALCSIZE_PARAMS)
                 Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.NCCALCSIZE_PARAMS));


                int horizontalScrollHeight = this.HorizontalScroll.Visible ? SystemInformation.HorizontalScrollBarHeight : 0;
                int verticalScrollWidth = this.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;

                ncCalcSizeParams.rgrc[0].top += 0;
                ncCalcSizeParams.rgrc[0].left += this.parentPanel.RootElement.RightToLeft ? - verticalScrollWidth : 0;
                ncCalcSizeParams.rgrc[0].right += this.parentPanel.RootElement.RightToLeft ? 0 : verticalScrollWidth;
                ncCalcSizeParams.rgrc[0].bottom += horizontalScrollHeight;

                Marshal.StructureToPtr(ncCalcSizeParams, m.LParam, true);

                m.Result = IntPtr.Zero;
            }
            else
            {

                NativeMethods.RECT ncCalcSizeParams = new NativeMethods.RECT();
                ncCalcSizeParams = (NativeMethods.RECT)
                 Marshal.PtrToStructure(m.LParam, typeof(NativeMethods.RECT));

                int horizontalScrollHeight = this.HorizontalScroll.Visible ? SystemInformation.HorizontalScrollBarHeight : 0;
                int verticalScrollWidth = this.VerticalScroll.Visible ? SystemInformation.VerticalScrollBarWidth : 0;

                ncCalcSizeParams.top += 0;
                ncCalcSizeParams.left += this.parentPanel.RootElement.RightToLeft ? -verticalScrollWidth : 0;
                ncCalcSizeParams.right += this.parentPanel.RootElement.RightToLeft ? 0 : verticalScrollWidth;
                ncCalcSizeParams.bottom += horizontalScrollHeight;

                Marshal.StructureToPtr(ncCalcSizeParams, m.LParam, true);
                m.Result = IntPtr.Zero;
            }

            base.WndProc(ref m);
        }

        #endregion
    }

    /// <summary>
    /// Instance of this class contain information about the control to which
    /// a container of the RadScrollablePanel is scrolled.
    /// </summary>
    public class ScrollbarSynchronizationNeededEventArgs : EventArgs
    {
        #region Fields

        public Point ScrolledLocation = Point.Empty;
        public Control ActiveControl = null;

        #endregion
    }

    public delegate void ScrollbarSynchronizationNeededEventHandler(object sender, ScrollbarSynchronizationNeededEventArgs args);
}
