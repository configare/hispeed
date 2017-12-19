#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：罗战克     时间：2014-2-18 17:25:19
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion
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
    /// <summary>
    /// 动态创建产品对应数据集控件
    /// </summary>
    public partial class ucCheckBoxList : UserControl, IEnumerable
    {
        private List<CheckBox> _chks = new List<CheckBox>();
        private CheckBox _all = new CheckBox();

        public ucCheckBoxList()
        {
            InitializeComponent();
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
                CheckBox chk = new CheckBox();
                chk.AutoSize = true;
                chk.Text = names[i];
                chk.Tag = id[i];
                _chks.Add(chk);
                flowLayoutPanel1.Controls.Add(chk);
            }
            flowLayoutPanel1.Controls.Add(_all);
        }

        /// <summary>
        /// 如果返回为空，则代表该产品没有子数据集或者所选子数据集为空。
        /// </summary>
        public long[] CheckedIds
        {
            get 
            { return GetSelectedDataset();
            }
        }

        private long[] GetSelectedDataset()
        {
            List<long> selected = new List<long>();
            foreach (CheckBox chk in _chks)
            {
                if (chk.Checked)
                {
                    selected.Add((long)chk.Tag);
                }
            }
            return selected.ToArray();
        }

        //为这个自定义控件增加了foreach遍历方法
        public IEnumerator GetEnumerator()
        {
            return _chks.GetEnumerator();
        }

        /// <summary>
        /// 如果返回为空，则代表该产品没有子数据集或者所选子数据集为空。
        /// </summary>
        public string [] CheckedNames
        {
            get
            {
                return GetSelectedDatasetNames();
            }
        }

        private string[] GetSelectedDatasetNames()
        {
            List<string> selected = new List<string>();
            foreach (CheckBox chk in _chks)
            {
                if (chk.Checked)
                {
                    selected.Add(chk.Text);
                }
            }
            return selected.ToArray();
        }

    }
}
