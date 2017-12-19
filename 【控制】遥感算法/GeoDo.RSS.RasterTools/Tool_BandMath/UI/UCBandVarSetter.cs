using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GeoDo.RSS.RasterTools
{
    public partial class UCBandVarSetter : UserControl
    {
        class BandVarItem
        {
            public string BandVar;
            public string BandVarRef = "未映射";
            public int BandNo;

            public BandVarItem(string bandVar)
            {
                BandVar = bandVar;
            }

            public override string ToString()
            {
                return BandVar + " - [" + BandVarRef+"]";
            }
        }

        public class BandName
        {
            public int BandNo;

            public BandName(int bandNo)
            {
                BandNo = bandNo;
            }

            public override string ToString()
            {
                return "Band "+BandNo.ToString();
            }
        }

        public class FileBandNames
        {
            public string FileName;
            public BandName[] BandNames;
        }

        public event EventHandler ApplyClicked;
        public event EventHandler CancelClicked;

        public UCBandVarSetter()
        {
            InitializeComponent();
        }

        public void SetFileBandNames(FileBandNames[] fileBandNames)
        {
            if (fileBandNames == null || fileBandNames.Length == 0)
                return;
            foreach (FileBandNames file in fileBandNames)
            {
                TreeNode root = new TreeNode(file.FileName);
                lvBandNos.Nodes.Add(root);
                if (file.BandNames != null)
                {
                    foreach (BandName bandName in file.BandNames)
                    {
                        TreeNode trNode = new TreeNode(bandName.ToString());
                        trNode.Tag = bandName;
                        root.Nodes.Add(trNode);
                    }
                }
            }
            lvBandNos.ExpandAll();
        }

        public void SetExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return;
            lvBandVars.Tag = expression;
            txtExpression.Text = expression;
            int[] bandNos;
            ClassRuntimeGenerator.GetOperatorString(expression, out bandNos);
            if (bandNos != null && bandNos.Length > 0)
            {
                foreach (int bNo in bandNos)
                {
                    lvBandVars.Items.Add(new BandVarItem("B"+bNo.ToString()));
                }
            }
        }

        public void SetSaveAsFileName(string fname)
        {
            txtSaveAs.Text = fname;
        }

        public string SaveAsFileName
        {
            get { return txtSaveAs.Text; }
        }

        public Dictionary<string, int> MappedBandNos
        {
            get 
            {
                Dictionary<string, int> mappedBandNos = new Dictionary<string, int>();
                foreach (BandVarItem item in lvBandVars.Items)
                {
                    mappedBandNos.Add(item.BandVar.ToLower(), item.BandNo);
                }
                return mappedBandNos;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (IsMapped())
            {
                if (ApplyClicked != null)
                    ApplyClicked(null, null);
            }
        }

        private bool IsMapped()
        {
            foreach (BandVarItem item in lvBandVars.Items)
            {
                if (item.BandVarRef == "未映射")
                    return false;
            }
            if (string.IsNullOrWhiteSpace(txtSaveAs.Text))
                return false;
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (CancelClicked != null)
                CancelClicked(null, null);
        }

        private void lvBandNos_MouseClick(object sender, MouseEventArgs e)
        {
            if (lvBandVars.SelectedItem == null)
                return;
            TreeNode trNode = lvBandNos.GetNodeAt(e.Location);
            if (trNode == null || trNode.Parent == null)
                return;
            BandName bandName = trNode.Tag as BandName;
            BandVarItem item = lvBandVars.SelectedItem as BandVarItem;
            item.BandVarRef = bandName.ToString();
            item.BandNo = bandName.BandNo;
            //
            int idx = lvBandVars.SelectedIndex;
            BandVarItem[] vars = lvBandVars.Items.Cast<BandVarItem>().ToArray();
            lvBandVars.Items.Clear();
            foreach (BandVarItem it in vars)
                lvBandVars.Items.Add(it);
            lvBandVars.SelectedIndex = idx;
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = "NSMC Ldf File(*.ldf)|*.ldf|NSMC MEM File(*.dat)|*.dat";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtSaveAs.Text = dlg.FileName;
                }
            }
        }
    }
}
