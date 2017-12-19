using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.Bricks.UIs;

namespace GeoDo.RSS.MIF.Core
{
    public partial class frmRename : Form
    {
        public delegate bool IsRepeatNameChecker(string name);
        internal event IsRepeatNameChecker CheckerHandler = null;

        public frmRename()
        {
            InitializeComponent();
            Load += new EventHandler(frmRename_Load);
        }

        void frmRename_Load(object sender, EventArgs e)
        {
            textBox1.Focus();
            textBox1.SelectAll();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!NameIsOK(textBox1.Text))
            {
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        private bool NameIsOK(string name)
        {
            if (name.Trim() == string.Empty)
            {
                MsgBox.ShowInfo("名称必须输入,请重新输入。");
                textBox1.Focus();
                textBox1.SelectAll();
                return false;
            }
            if (CheckerHandler != null)
            {
                if (CheckerHandler(name))
                {
                    MsgBox.ShowInfo("名称重复,请重新输入。");
                    textBox1.Focus();
                    textBox1.SelectAll();
                    return false;
                }
            }
            return true;
        }

        public static string ReName(string name, IsRepeatNameChecker checker)
        {
            using (frmRename frm = new frmRename())
            {
                frm.textBox1.Text = name;
                frm.StartPosition = FormStartPosition.CenterParent;
                frm.CheckerHandler = checker;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    return frm.textBox1.Text;
                }
            }
            return name;
        }
    }
}
