using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace CodeCell.Bricks.Runtime
{
    public static class TopmostFormsManager
    {
        [DllImport("user32")]
        public static extern bool IsWindowVisible(long wndHandle);
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern int GetClipBox(IntPtr hDC, ref System.Drawing.Rectangle lpRect);
        //
        private static Form _owner = null;
        private static System.Drawing.Rectangle _validBoundsAtScreen = System.Drawing.Rectangle.Empty;
        private static List<Form> _frms = new List<Form>();

        public static void Init(Form owner)
        {
            _owner = owner;
            _owner.FormClosed += new FormClosedEventHandler(Owner_FormClosed);
            _owner.Resize += new EventHandler(Owner_Resize);
        }

        public static bool IsWindowPhyVisible(IntPtr AHandle)
        {
            IntPtr vDC = GetWindowDC(AHandle);
            try
            {
                System.Drawing.Rectangle vRect = new System.Drawing.Rectangle();
                GetClipBox(vDC, ref vRect);
                return !(vRect.Width - vRect.Left <= 0 && vRect.Height - vRect.Top <= 0);
            }
            finally
            {
                ReleaseDC(AHandle, vDC);
            }
        }

        static void _owner_Deactivate(object sender, EventArgs e)
        {
            MinimizedForms();
        }

        static void _owner_Activated(object sender, EventArgs e)
        {
            SetNormalAdjustRelativePosition();
        }

        public static Form[] Forms
        {
            get { return _frms != null && _frms.Count > 0 ? _frms.ToArray() : null; }
        }

        public static Form QueryFormByName(string name)
        {
            if (_frms == null || _frms.Count == 0)
                return null;
            foreach (Form frm in _frms)
                if (frm.Name.ToUpper() == name.ToUpper())
                    return frm;
            return null;
        }

        public static Form QueryFormByName(Type type)
        {
            if (_frms == null || _frms.Count == 0)
                return null;
            foreach (Form frm in _frms)
                if (frm.GetType().Equals(type))
                    return frm;
            return null;
        }

        public static bool Exists(Form frm)
        {
            if ( frm == null || _frms == null || _frms.Count == 0)
                return false;
            return _frms.Contains(frm);
        }

        public static void RegTopmostForm(Form frm)
        {
            if (frm == null)
                return;
            if (!_frms.Contains(frm))
            {
                //frm.TopMost = true;
                //frm.ShowInTaskbar = false;
                _frms.Add(frm);
                frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
            }
        }

        static void frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(_frms != null && _frms.Contains(sender as Form))
                _frms.Remove(sender as Form);
        }

        static void Owner_Resize(object sender, EventArgs e)
        {
            //switch (_owner.WindowState)
            //{
            //    case FormWindowState.Normal:
            //        AdjustRelativePosition();
            //        break;
            //    case FormWindowState.Minimized:
            //        MinimizedForms();
            //        break;
            //    case FormWindowState.Maximized:
            //        SetNormalAdjustRelativePosition();
            //        break;
            //}
        }

        private static void MinimizedForms()
        {
            if(_frms == null || _frms.Count==0)
                return ;
            foreach (Form frm in _frms)
            {
                frm.Visible = false;
                frm.WindowState = FormWindowState.Minimized;
            }
        }

        private static void SetNormalAdjustRelativePosition()
        {
           if(_frms == null || _frms.Count==0)
            return ;
           foreach (Form frm in _frms)
           {
               if (frm.IsDisposed)
                   continue;
               frm.WindowState = FormWindowState.Normal;
               frm.Visible = true;
           }
           AdjustRelativePosition();
        }

        private static void AdjustRelativePosition()
        {
            //
        }

        static void Owner_FormClosed(object sender, FormClosedEventArgs e)
        {
            
        }
    }
}
