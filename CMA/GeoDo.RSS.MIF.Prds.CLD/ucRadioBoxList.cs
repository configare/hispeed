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
    public partial class ucRadioBoxList : UserControl, IEnumerable
    {
        private List<RadioButton> _chks = new List<RadioButton>();
        public event EventHandler CheckedChanged = null;

        public ucRadioBoxList()
        {
            InitializeComponent();
            this.Disposed += new EventHandler(ucRadioBoxList_Disposed);
        }

        void ucRadioBoxList_Disposed(object sender, EventArgs e)
        {
            if (CheckedChanged != null)
                CheckedChanged = null;
        }

        public void ResetContent(long[] id, string[] names)
        {
            //获取数据集信息
            _chks.Clear();
            flowLayoutPanel1.Controls.Clear();
            if (id == null || id.Length == 0)
                return;
            int length = id.Length;
            for (int i = 0; i < length; i++)
            {
                RadioButton chk = new RadioButton();
                chk.AutoSize = true;
                chk.Text = names[i];
                chk.Tag = id[i];
                chk.CheckedChanged += new EventHandler(chk_CheckedChanged);
                _chks.Add(chk);
                flowLayoutPanel1.Controls.Add(chk);
            }
        }

        void chk_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckedChanged != null)
                CheckedChanged(this, e);
        }

        /// <summary>
        /// 如果返回为空，则代表该产品没有子数据集或者所选子数据集为空。
        /// </summary>
        public long Checked
        {
            get
            {
                return GetChecked();
            }
        }

        /// <summary>
        /// 如果返回为空，则代表该产品没有子数据集或者所选子数据集为空。
        /// </summary>
        public string  CheckedName
        {
            get
            {
                return GetCheckedNames();
            }
        }


        private long GetChecked()
        {
            foreach (RadioButton chk in _chks)
            {
                if (chk.Checked)
                {
                    return (long)chk.Tag;
                }
            }
            return -1;
        }

        private string  GetCheckedNames()
        {
            foreach (RadioButton chk in _chks)
            {
                if (chk.Checked)
                {
                    return chk.Text;
                }
            }
            return null;
        }

        //为这个自定义控件增加了foreach遍历方法
        public IEnumerator GetEnumerator()
        {
            return _chks.GetEnumerator();
        }
    }
}
