using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public partial class frmEditTimeStatArg : Form
    {
        private string _argFilename;
        public Dictionary<string, string> ResultDic = new Dictionary<string, string>();

        public frmEditTimeStatArg()
        {
            InitializeComponent();
        }

        public frmEditTimeStatArg(string filename)
        {
            InitializeComponent();
            _argFilename = filename;
            InitForm();
        }

        private void InitForm()
        {
            if (!File.Exists(_argFilename))
                return;
            string[] contents = File.ReadAllLines(_argFilename, Encoding.Default);
            if (contents == null || contents.Length == 0)
                return;
            string[] stringsplit = null;
            foreach (string content in contents)
            {
                if (string.IsNullOrEmpty(content))
                    continue;
                stringsplit = content.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (stringsplit == null || stringsplit.Length < 2)
                    continue;
                AddLstItem(stringsplit);
            }
        }

        private void AddLstItem(string[] stringsplit)
        {
            ListViewItem lvi = new ListViewItem(stringsplit[0]);
            lvi.SubItems.Add(stringsplit[1]);
            lstNameRegion.Items.Add(lvi);
            if (!ResultDic.ContainsKey(stringsplit[0]))
                ResultDic.Add(stringsplit[0], stringsplit[1]);
        }

        private void btAdd_Click(object sender, EventArgs e)
        {
            if (!CheckEnv())
                return;
            AddLstItem(new string[] { txtName.Text, txtIdentify.Text });
        }

        private bool CheckEnv()
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MessageBox.Show("名称尚未填写!");
                return false;
            }
            if (string.IsNullOrEmpty(txtIdentify.Text))
            {
                MessageBox.Show("标识尚未填写!");
                return false;
            }
            if (ResultDic.ContainsKey(txtName.Text))
            {
                MessageBox.Show("已包含当前信息!");
                return false;
            }
            return true;
        }

        private void btDel_Click(object sender, EventArgs e)
        {
            if (lstNameRegion.SelectedItems == null || lstNameRegion.SelectedItems.Count == 0)
                return;
            string messageStr = "确定删除以下信息：\n\t";
            List<ListViewItem> removeItem = new List<ListViewItem>();
            foreach (ListViewItem item in lstNameRegion.SelectedItems)
            {
                messageStr += item.Text + " [" + item.SubItems[1].Text + "] " + "\n\t";
                removeItem.Add(item);
            }
            if (MessageBox.Show(messageStr, "确认删除", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (ListViewItem item in removeItem)
                {
                    lstNameRegion.Items.Remove(item);
                    if (ResultDic.ContainsKey(item.Text))
                        ResultDic.Remove(item.Text);
                }
            }
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            if (lstNameRegion.Items.Count == 0)
                return;
            ResultDic.Clear();
            List<string> fileContent = new List<string>();
            foreach (ListViewItem item in lstNameRegion.Items)
            {
                ResultDic.Add(item.Text, item.SubItems[1].Text);
                fileContent.Add(item.Text + " " + item.SubItems[1].Text);
            }
            string dir = Path.GetDirectoryName(_argFilename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllLines(_argFilename, fileContent.ToArray(), Encoding.Default);
            DialogResult = DialogResult.OK;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            DialogResult = DialogResult.Cancel;
        }
    }
}
