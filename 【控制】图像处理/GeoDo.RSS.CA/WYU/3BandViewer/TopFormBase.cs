using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.CA
{
    public partial class TopFormBase : Form
    {
        private StickyWindow stickyWindow;
        //private ControlExpand _ce;

        public TopFormBase()
        {
            stickyWindow = new StickyWindow(this);
            stickyWindow.StickOnMove = true;
            stickyWindow.StickOnResize = true;
            stickyWindow.StickToOther = true;
            stickyWindow.StickToScreen = true;
            stickyWindow.StickGap = 20;

            InitializeComponent();
            //Load += new EventHandler(TopFormBase_Load);
        }

        //void TopFormBase_Load(object sender, EventArgs e)
        //{
        //    this.Resize += new EventHandler(Form1_Resize);
        //    X = this.Width;
        //    Y = this.Height;
        //    setTag(this);
        //}

        public new void Show(IWin32Window owner)
        {
            Form frm = FindForm(this);
            if (frm != null)
            {
                if (frm.WindowState == FormWindowState.Minimized)
                    frm.WindowState = FormWindowState.Normal;
                frm.Visible = true;
                frm.Activate();
            }
            else
            {
                base.Show(owner);
            }
        }

        private Form FindForm(Form frm)
        {
            if (Application.OpenForms != null && Application.OpenForms.Count > 0)
            {
                foreach (Form f in Application.OpenForms)
                    if (f.Equals(frm))
                        return f;
            }
            return null;
        }

        /// <summary>
        /// 设置与获取是否在移动时可自动停靠
        /// </summary>
        public bool StickOnMove
        {
            get { return stickyWindow.StickOnMove; }
            set { stickyWindow.StickOnMove = value; }
        }

        /// <summary>
        /// 设置与获取是否在Resize时可自动停靠
        /// </summary>
        public bool StickOnResize
        {
            get { return stickyWindow.StickOnResize; }
            set { stickyWindow.StickOnResize = value; }
        }

        /// <summary>
        /// 设置与获取是否在从此类派生的所有窗口之间皆可自动停靠
        /// </summary>
        public bool StickToOther
        {
            get { return stickyWindow.StickToOther; }
            set { stickyWindow.StickToOther = value; }
        }


        /// <summary>
        /// 设置与获取是否在可自动停靠在屏幕边缘
        /// </summary>
        public bool StickToScreen
        {
            get { return stickyWindow.StickToScreen; }
            set { stickyWindow.StickToScreen = value; }
        }

        /// <summary>
        /// 设置与获取停靠间距
        /// </summary>
        public int StickGap
        {
            get { return stickyWindow.StickGap; }
            set
            {
                if (value <= 0 | value > 100)
                    throw new Exception("间距必须在0-100个象素之间");
                else
                    stickyWindow.StickGap = value;
            }
        }

        /*
        private float X;
        private float Y;

        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                _ce.SizeTag = con.Width + ":" + con.Height + ":" + con.Left + ":" + con.Top + ":" + con.Font.Size;
                if (con.Controls.Count > 0)
                    setTag(con);
            }
        }

        private void setControls(float newx, float newy, Control cons)
        {
            foreach (Control con in cons.Controls)
            {

                string[] mytag = _ce.SizeTag.ToString().Split(new char[] { ':' });
                float a = Convert.ToSingle(mytag[0]) * newx;
                con.Width = (int)a;
                a = Convert.ToSingle(mytag[1]) * newy;
                con.Height = (int)(a);
                a = Convert.ToSingle(mytag[2]) * newx;
                con.Left = (int)(a);
                a = Convert.ToSingle(mytag[3]) * newy;
                con.Top = (int)(a);
                Single currentSize = Convert.ToSingle(mytag[4]) * Math.Min(newx, newy);
                con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                if (con.Controls.Count > 0)
                {
                    setControls(newx, newy, con);
                }
            }

        }

        void Form1_Resize(object sender, EventArgs e)
        {
            float newx = (this.Width) / X;
            float newy = this.Height / Y;
            setControls(newx, newy, this);
        }
        */
    }
}
