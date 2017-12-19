using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class ucMonths : UserControl, IEnumerable
    {
        private CheckBox _all = new CheckBox();
        private CheckBox[] _chks = new CheckBox[12];

        public ucMonths()
        {
            InitializeComponent();
            this.Load += new EventHandler(ucMonths_Load);
            _all.AutoSize = true;
            _all.Text = "全部";
            _all.CheckedChanged += new EventHandler(_all_CheckedChanged);
        }

        void _all_CheckedChanged(object sender, EventArgs e)
        {
            foreach (CheckBox chk in _chks)
            {
                chk.Checked = _all.Checked;
            }
        }

        void ucMonths_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 12; i++)
            {
                CheckBox chk = new CheckBox();
                chk.AutoSize = true;
                chk.Text = i + 1 + "月";
                chk.Tag = i + 1;
                _chks[i] = chk;
                flowLayoutPanel1.Controls.Add(chk);
            }
            flowLayoutPanel1.Controls.Add(_all);
        }

        public bool IsAllChecked()
        {
            return _all.Checked;
        }

        public int[] Checkeds
        {
            get
            {
                return GetChecked();
            }
        }

        private int[] GetChecked()
        {
            List<int> sels = new List<int>();
            foreach (CheckBox chk in _chks)
            {
                if (chk.Checked)
                    sels.Add((int)chk.Tag);
            }
            return sels.ToArray();
        }

        //为这个自定义控件增加了foreach遍历方法
        public IEnumerator GetEnumerator()
        {
            return _chks.GetEnumerator();
        }
    }
}
