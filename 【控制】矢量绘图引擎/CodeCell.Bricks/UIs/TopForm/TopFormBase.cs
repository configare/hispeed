using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CodeCell.Bricks.UIs
{
    public partial class TopFormBase : Form
    {
        private StickyWindow stickyWindow;

        public TopFormBase()
        {
            stickyWindow = new StickyWindow(this);
            stickyWindow.StickOnMove = true;
            stickyWindow.StickOnResize = true;
            stickyWindow.StickToOther = true;
            stickyWindow.StickToScreen = true;
            stickyWindow.StickGap = 20;

            InitializeComponent();
        }

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
    }
}
