using System.Windows.Forms;
using System.ComponentModel;
using System;
using Telerik.WinControls;
using System.Runtime.InteropServices;

namespace Telerik.WinControls
{

    [ToolboxItem(false)]
    public class RadShimControl : Control
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams params1 = base.CreateParams;
                //params1.ClassStyle |= NativeMethods.CS_SAVEBITS;
                params1.Style &= -79691777;
                params1.ExStyle &= -262145;

                params1.Style |= NativeMethods.WS_POPUP;
                params1.Style |= NativeMethods.WS_CLIPSIBLINGS;
                params1.Style |= NativeMethods.WS_DISABLED;

                params1.ExStyle |= NativeMethods.WS_EX_LAYERED;
                params1.ExStyle |= NativeMethods.WS_EX_TRANSPARENT;

                return params1;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            this.UpdateLayered();
        }

        private byte OpacityAsByte
        {
            get
            {
                return (byte)(this.Opacity * 255);
            }
        }

        private double opacity = 1.0;
        [Category("CatWindowStyle"),
        TypeConverter(typeof(OpacityConverter)),
        Description("FormOpacityDescr"),
        DefaultValue((double)1)]
        public double Opacity
        {
            get
            {
                return this.opacity;
            }
            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < 0)
                {
                    value = 0;
                }
                this.opacity = value;
                //if ((this.OpacityAsByte < 0xff) &&
                //    OSFeature.Feature.IsPresent(OSFeature.LayeredWindows))
                //{
                //    if (formStateLayered != 1)
                //    {
                //        formStateLayered = 1;
                //        if (!flag1)
                //        {
                //            base.UpdateStyles();
                //        }
                //    }
                //}
                //else
                //{
                //    formStateLayered = (this.TransparencyKey != Color.Empty) ? 1 : 0;
                //    if (flag1 != (formStateLayered != 0))
                //    {
                //        int num1 = (int)((long)NativeMethods.GetWindowLong(new HandleRef(this, base.Handle), NativeMethods.GWL_EXSTYLE));
                //        System.Windows.Forms.CreateParams params1 = this.CreateParams;
                //        if (num1 != params1.ExStyle)
                //        {
                //            NativeMethods.SetWindowLong(new HandleRef(this, base.Handle), NativeMethods.GWL_EXSTYLE, new HandleRef(null, (IntPtr)params1.ExStyle));
                //        }
                //    }
                //}
                this.UpdateLayered();
            }
        }


        private void UpdateLayered()
        {
            if (base.IsHandleCreated && OSFeature.Feature.IsPresent(OSFeature.LayeredWindows))//this.TopLevel && 
            {
                bool flag1;
                flag1 = NativeMethods.SetLayeredWindowAttributes(new HandleRef(this, base.Handle), 0, this.OpacityAsByte, 2);
                //if (!flag1)
                //{
                //    throw new Win32Exception();
                //}
            }
        }

        public bool Redraw
        {
            set
            {
                if (this.Visible)
                {
                    if (value)
                    {
                        NativeMethods.SendMessage(new HandleRef(null, this.Handle), NativeMethods.WM_SETREDRAW, (IntPtr)1, (IntPtr)0);
                    }
                    else
                    {
                        NativeMethods.SendMessage(new HandleRef(null, this.Handle), NativeMethods.WM_SETREDRAW, (IntPtr)0, (IntPtr)0);
                    }
                }
            }
        }
    }

}