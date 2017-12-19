using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RSS.Core.UI;
//using GeoDo.RSS.UI.AddIn.L2ColorTable;
//using GeoDo.RSS.UI.AddIn.L2ColorTable;
//using AIO.Engine.Engine.IMG;

namespace GeoDo.RSS.UI.AddIn.L2ColorTable
{
    public partial class frmL2ColorTable : Form
    {
        private Dictionary<TreeNode, string> _nodeTips = new Dictionary<TreeNode, string>();
        private ISmartSession _session = null;
        private BandValueColorPair[] _colorTables;

        public frmL2ColorTable()
        {
            InitializeComponent();
            txtTips.Text = string.Empty;
        }

        public BandValueColorPair[] ColorTable
        {
            get { return _colorTables; }
        }

        internal void SetSession(ISmartSession session)
        {
            _session = session;
        }

        private void frmL2ColorTable_Load(object sender, EventArgs e)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\L2ColorTableFiles";
            string[] files = Directory.GetFiles(dir, "*.xml");
            if (files == null || files.Length == 0)
                return;
            foreach (string f in files)
            {
                string name = Path.GetFileNameWithoutExtension(f);
                TreeNode tn = new TreeNode(name);
                L2ColorTableParser p = new L2ColorTableParser(f);
                tn.Text = tn.Text + "(" + p.Description + ")";
                tn.Tag = p;
                string tips = GetTipsByFile(p);
                _nodeTips.Add(tn, tips);
                TreeNode tnIndex = new TreeNode("索引空间");
                tnIndex.Tag = "AllIndex";
                LoadBandValueNodes(tnIndex, p);
                tn.Nodes.Add(tnIndex);
                TreeNode tnColorTable = new TreeNode("颜色表");
                tnColorTable.Tag = "AllColorTable";
                LoadColorTableNodes(tnColorTable, p);
                tn.Nodes.Add(tnColorTable);
                tvColotTables.Nodes.Add(tn);
                //tn.Expand();
                //tnIndex.Expand();
            }
            //
            if (tvColotTables.Nodes.Count > 0)
            {
                FillAttsOfTreeNode(tvColotTables.Nodes[0]);
            }
        }

        private void LoadColorTableNodes(TreeNode tn, L2ColorTableParser p)
        {
            string[] tips = null;
            string[] names = p.GetColorTables(out tips);
            if (names == null || names.Length == 0)
                return;
            foreach (string n in names)
            {
                TreeNode t = new TreeNode(n);
                t.Tag = "ColorTable";
                tn.Nodes.Add(t);
                _nodeTips.Add(t, p.GetColorTableTips(n));
            }
            string strTips = null;
            foreach (string s in tips)
                strTips += (s + "\n");
            _nodeTips.Add(tn, strTips.Substring(0, strTips.Length - 1));
        }

        private void LoadBandValueNodes(TreeNode tn, L2ColorTableParser p)
        {
            string[] tips = null;
            string[] names = p.GetBandValueRanges(out tips);
            if (names == null || names.Length == 0)
                return;
            foreach (string n in names)
            {
                TreeNode t = new TreeNode(n);
                t.Tag = "BandValue";
                tn.Nodes.Add(t);
                _nodeTips.Add(t, p.GetBandValueTips(n));
            }
            string strTips = null;
            foreach (string s in tips)
                strTips += (s + "\n");
            _nodeTips.Add(tn, strTips.Substring(0, strTips.Length - 1));
        }

        private string GetTipsByFile(L2ColorTableParser p)
        {
            string sApplyFor = string.Empty;
            if (p.ApplyFor != null)
            {
                foreach (string s in p.ApplyFor)
                    sApplyFor += (s + ",");
                sApplyFor.Substring(0, sApplyFor.Length - 1);
            }
            return p.Name + "\n" +
                      sApplyFor + "\n" +
                      p.Description;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void GetSelectedColorTable(out L2ColorTableParser p, out string indexSpacename)
        {
            p = null;
            indexSpacename = null;
            TreeNode tn = tvColotTables.SelectedNode;
            if (tn == null || tn.Tag == null || tn.Tag.ToString() != "BandValue")
            {
                MsgBox.ShowInfo("请从左边填色方案树中索引空间节点下选择要应用的索引空间。\n\n系统会根据索引空间查找对应的颜色表。");
                return;
            }
            indexSpacename = tn.Text;
            p = tn.Parent.Parent.Tag as L2ColorTableParser;
        }

        private BandValueColorPair[] GetBandValueColorPairs()
        {
            L2ColorTableParser p = null;
            string indexSpacename = null;
            GetSelectedColorTable(out p, out indexSpacename);
            if (p != null && indexSpacename != null)
            {
                return p.GetBandValueColorPairByIndexSpacename(indexSpacename);
            }
            return null;
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            BandValueColorPair[] values = GetBandValueColorPairs();
            //if (RangeIsOK(values))
                ApplyBandValueColorPairsToAIOAgent(values);
        }

        private bool RangeIsOK(BandValueColorPair[] values)
        {
            if (values == null)
            {
                MsgBox.ShowInfo("选择索引空间不能应用与当前显示影像,请重新选择或者选择\"调色板填色\"功能进行填色。");
                return false;
            }
            //
            int minValue = int.MaxValue;
            int maxValue = int.MinValue;
            foreach (BandValueColorPair v in values)
            {
                if (v.MinValue < minValue)
                    minValue = v.MinValue;
                if (v.MaxValue > maxValue)
                    maxValue = v.MaxValue;
            }
            //
            int limitSmallValue = -1;
            int limitLargeValue = -1;
            //_session.AIOAgent.GetLimitValueOfEditingImage(out limitSmallValue, out limitLargeValue);
            //
            if (maxValue < limitSmallValue || minValue > limitLargeValue)
            {
                MsgBox.ShowInfo("选择的\"索引空间\"不能应用与当前显示影像,请重新选择或者选择\"调色板填色\"功能进行填色。");
                return false;
            }
            return true;
        }

        private void ApplyBandValueColorPairsToAIOAgent(BandValueColorPair[] bandValueColorPairs)
        {
            if (bandValueColorPairs == null)
                return;
            List<DensityRange> ranges = ToDensityRange(bandValueColorPairs);
            //_session.Application.AIOAgentMgr.ApplyLinearGradientColorTable(1, ranges);
        }

        public static List<DensityRange> ToDensityRange(BandValueColorPair[] bandValueColorPairs)
        {
            if (bandValueColorPairs == null)
                return null;
            List<DensityRange> ranges = new List<DensityRange>();
            foreach (BandValueColorPair v in bandValueColorPairs)
            {
                //[min,max)
                ranges.Add(new DensityRange(v.MinValue, v.MinValue == v.MaxValue ? v.MaxValue : v.MaxValue + 1, v.Color.R, v.Color.G, v.Color.B));
            }
            return ranges;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            BandValueColorPair[] values = GetBandValueColorPairs();
            //if (RangeIsOK(values))
            //{
                DialogResult = DialogResult.OK;
                _colorTables = values;
                //ApplyBandValueColorPairsToAIOAgent(values);
                Close();
            //}
        }

        private void tvColotTables_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode tn = tvColotTables.GetNodeAt(e.Location);
            if (tn == null)
                return;
            if (tvColotTables.SelectedNode != null && tvColotTables.SelectedNode.Equals(tn))
                return;
            FillAttsOfTreeNode(tn);
        }

        private void FillAttsOfTreeNode(TreeNode tn)
        {
            ucColorRampUseBlocks1.Apply(null);
            tvColotTables.SelectedNode = tn;
            if (_nodeTips.ContainsKey(tn))
                txtTips.Text = _nodeTips[tn];
            else
                txtTips.Text = string.Empty;
            string sContent = string.Empty;
            if (tn.Tag is L2ColorTableParser)
            {
                sContent = (tn.Tag as L2ColorTableParser).GetFileInnerText();
            }
            else if (tn.Tag.ToString() == "AllIndex")
            {
                sContent = (tn.Parent.Tag as L2ColorTableParser).GetAllIndexXml();
            }
            else if (tn.Tag.ToString() == "AllColorTable")
            {
                sContent = (tn.Parent.Tag as L2ColorTableParser).GetAllColorTable();
            }
            else if (tn.Tag.ToString() == "BandValue")
            {
                string s = (tn.Parent.Parent.Tag as L2ColorTableParser).GetBandValueRangeInnerText(tn.Text);
                if (s != null)
                    sContent = s;
                else
                    sContent = string.Empty;
                ucColorRampUseBlocks1.Apply(GetBandValueColorPairs());
            }
            else if (tn.Tag.ToString() == "ColorTable")
            {
                string ss = (tn.Parent.Parent.Tag as L2ColorTableParser).GetColorTableInnerText(tn.Text);
                if (ss != null)
                    sContent = ss;
                else
                    sContent = string.Empty;
                BandValueColorPair[] colors = (tn.Parent.Parent.Tag as L2ColorTableParser).GetColorsByColorTableName(tn.Text);
                ucColorRampUseBlocks1.Apply(colors);
            }
            sContent = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><Content> " + sContent + "</Content>";
            //string url = _session.Application.TemporalFileManager.NextTemporalFilename(".xml", null);
            string url = _session.TemporalFileManager.NextTemporalFilename(".xml", null);
            File.WriteAllText(url, sContent);
            txtContent.Url = new Uri(url);
        }
    }
}
