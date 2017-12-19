using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public partial class PrjArgsSelectBand : UserControl
    {
        public PrjArgsSelectBand()
        {
            InitializeComponent();
        }

        public void SetArgs(RSS.Core.DF.IRasterDataProvider rasterDataProvider)
        {
            if (rasterDataProvider == null)
                return;
            DataIdentify dataIdentify = rasterDataProvider.DataIdentify;
            if (dataIdentify.IsOrbit)
            {
                AddBandsWithOrbit(rasterDataProvider);
            }
            else
            {
                AddBandsWithProjected(rasterDataProvider);
            }
        }

        private void AddBandsWithOrbit(IRasterDataProvider rasterDataProvider)
        {
            PrjBand[] prjBands = PrjBandTable.GetPrjBands(rasterDataProvider);
            if (prjBands == null || prjBands.Length == 0)
                return;
            tvBandList.Nodes.Clear();
            for (int i = 0; i < prjBands.Length; i++)
            {
                TreeNode node = new TreeNode(prjBands[i].ToString());
                node.Checked = true;
                node.Tag = prjBands[i];
                tvBandList.Nodes.Add(node);
            }
        }

        private void AddBandsWithProjected(IRasterDataProvider rasterDataProvider)
        {
            DataIdentify dataIdentify = rasterDataProvider.DataIdentify;
            int bandCount = rasterDataProvider.BandCount;
            tvBandList.Nodes.Clear();
            for (int i = 1; i <= bandCount; i++)
            {
                TreeNode node = new TreeNode("band" + i);
                node.Checked = true;
                node.Tag = new PrjBand(dataIdentify.Sensor, rasterDataProvider.ResolutionX, "1", i, "band" + "", "", "");
                tvBandList.Nodes.Add(node);
            }
        }

        /// <summary>
        /// 从1开始的编号
        /// </summary>
        /// <returns></returns>
        public int[] SelectedBandNumbers()
        {
            List<int> selectedBands = new List<int>();
            for (int i = 0; i < tvBandList.Nodes.Count; i++)
            {
                if (tvBandList.Nodes[i].Checked)
                {
                    selectedBands.Add(i + 1);
                }
            }
            return selectedBands.ToArray();
        }

        public PrjBand[] SelectedBands
        {
            get
            {
                return GetSelectedBands();
            }
        }

        private PrjBand[] GetSelectedBands()
        {
            List<PrjBand> selectedBands = new List<PrjBand>();
            for (int i = 0; i < tvBandList.Nodes.Count; i++)
            {
                if (tvBandList.Nodes[i].Checked)
                {
                    selectedBands.Add(tvBandList.Nodes[i].Tag as PrjBand);
                }
            }
            return selectedBands.ToArray();
        }

        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tvBandList.Nodes.Count; i++)
            {
                tvBandList.Nodes[i].Checked = true;
            }
        }

        private void btnSelectNone_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < tvBandList.Nodes.Count; i++)
            {
                tvBandList.Nodes[i].Checked = false;
            }
        }
    }
}
