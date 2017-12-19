using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace CodeCell.Bricks.UIs
{
    public class ToolStripEx:ToolStrip
    {
        public static Form MainForm = null;
        protected Form _defaultOwner = null;
        protected Form _newOwner = null;
        protected DockStyle _originalDock = DockStyle.Left;
        protected ToolStripLayoutStyle _originalLayout = ToolStripLayoutStyle.Flow;

        public ToolStripEx()
        {
            using (Stream st = this.GetType().Assembly.GetManifestResourceStream("AgileMap.Bricks.More.png"))
            {
                Bitmap bm = new Bitmap(st);
                ToolStripButton defaultButton = new ToolStripButton(bm);
                defaultButton.Click += new EventHandler(defaultButton_Click);
                Items.Add(defaultButton);
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {

            if (Name == "左侧工具条")
            {
                TryApplyBackColor();
            }
        }


        private void TryApplyBackColor()
        {
            try
            {
                string fname = AppDomain.CurrentDomain.BaseDirectory + "ToolStrip.BackColor";
                if (File.Exists(fname))
                {
                    string[] lines = File.ReadAllLines(fname,Encoding.Default);
                    if (lines == null || lines.Length == 0)
                        return;
                    Color color = Color.FromArgb(int.Parse(lines[0]));
                    BackColor = color;
                }
            }
            catch 
            {
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                dlg.Color = BackColor;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    BackColor = dlg.Color;
                    Invalidate();
                    TrySaveBackColor();
                }
            }
            //
           
        }

        private void TrySaveBackColor()
        {
            if (Name == "左侧工具条")
            {
                try
                {
                    string fname = AppDomain.CurrentDomain.BaseDirectory + "ToolStrip.BackColor";
                    File.WriteAllLines(fname, new string[] { BackColor.ToArgb().ToString() }, Encoding.Default);
                }
                catch
                {
                }
            }
        }

        public Form DefaultOwner
        {
            set { _defaultOwner = value; }
        }

        void defaultButton_Click(object sender, EventArgs e)
        {
            ToolStrip ts = (sender as ToolStripButton).Owner as ToolStrip;
            if (ts.FindForm().Equals(_defaultOwner))
            {
                FreeToolStrip(ts);
            }
            else
            {
                FixToolStrip(ts, true);
            }
        }

        private void FixToolStrip(ToolStrip ts, bool isCloseForm)
        {
            Form frm = ts.FindForm();
           _defaultOwner.Controls.Add(ts);
            foreach (ToolStripItem it in ts.Items)
                if (it is ToolStripSeparator)
                    it.Visible = true;
            ts.Dock = _originalDock;
            ts.LayoutStyle = _originalLayout;
            ts.BringToFront();
            if (isCloseForm)
                frm.Close();
        }

        private void FreeToolStrip(ToolStrip ts)
        {
            _originalDock = ts.Dock;
            _originalLayout = ts.LayoutStyle;
            TopFormBase frm = new TopFormBase();
            frm.Text = "工具栏...";
            frm.Tag = ts;
            frm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            frm.BackColor = Color.Gray;
            switch (ts.Dock)
            {
                case DockStyle.Left:
                case DockStyle.Right:
                    frm.Width = ts.Width * 2 + 8;
                    ts.Dock = DockStyle.Fill;
                    ts.LayoutStyle = ToolStripLayoutStyle.Flow;
                    foreach (ToolStripItem it in ts.Items)
                        if (it is ToolStripSeparator)
                            it.Visible = false;
                    break;
                case DockStyle.Top :
                case DockStyle.Bottom:
                    frm.ClientSize = new Size(frm.Width, ts.Height);
                    ts.Dock = DockStyle.Top;
                    break;
            }
            frm.Controls.Add(ts);
            frm.Show(MainForm);
            frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
        }

        void frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            FixToolStrip((sender as Form).Tag as ToolStrip, false);
        }
    }
}
