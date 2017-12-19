using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GeoDo.ProjectDefine;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.FileProject;
using GeoDo.Project;
using GeoDo.RasterProject;

namespace TestForm
{
    public partial class FormSetParas : Form
    {
        private Point _mainFrmPoint;
        private Size _mainFrmSize;
        private ProjectParas _proParas;

        public ProjectParas ProParas
        {
            get { return _proParas; }
            set { _proParas = value; }
        }

        public Size MainFrmSize
        {
            get { return _mainFrmSize; }
            set { _mainFrmSize = value; }
        }

        public Point MainFrmPoint
        {
            get { return _mainFrmPoint; }
            set { _mainFrmPoint = value; }
        }

        public FormSetParas()
        {
            InitializeComponent();
            Init();
            InitBands();
        }

        private void InitBands()
        {
            tvBands.Nodes.Clear();
            if (ProParas == null)
                return;
            TreeNode subNode;
            for (int i = 0; i < ProParas.PrjSetting.BandMapTable.Count; i++)
            {
                subNode=new TreeNode("band_" + (i+1));
                tvBands.Nodes.Add(subNode);
            }
        }

        private void Init()
        {
            cmbOutRange.SelectedIndex = 0;
            cmbProType.SelectedIndex = 0;
            groupBox4.Visible = true;
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in tvBands.Nodes)
            {
                node.Checked = true;
            }
        }

        private void btnNone_Click(object sender, EventArgs e)
        {
            foreach (TreeNode node in tvBands.Nodes)
            {
                node.Checked = false;
            }
        }

        private void cmbOutRange_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox4.Visible = false;
            groupBox5.Visible = false;
            groupBox6.Visible = false;
            groupBox7.Visible = false;
            txtCenterX.ReadOnly = false;
            txtCenterY.ReadOnly = false;
            switch (cmbOutRange.SelectedItem.ToString())
            {
                case "整轨投影":
                    groupBox4.Visible = true;
                    txtCenterX.ReadOnly = true;
                    txtCenterY.ReadOnly = true;
                    break;
                case "按中心点":
                    groupBox4.Visible = true;
                    groupBox5.Visible = true;
                    break;
                case "按四角经纬度":
                    groupBox6.Visible = true;
                    break;
                case "按已定义的输出区域":
                    groupBox7.Visible = true;
                    break;
            }
        }

        private void FormSetParas_Move(object sender, EventArgs e)
        {
            if (this.DesktopLocation.X >= (MainFrmPoint.X + MainFrmSize.Width) && this.DesktopLocation.X <= (MainFrmPoint.X + MainFrmSize.Width + 20)&&this.DesktopLocation.Y>=MainFrmPoint.Y&&this.DesktopLocation.Y<=(MainFrmPoint.Y+MainFrmSize.Height))
            {
                this.SetDesktopLocation(MainFrmPoint.X + MainFrmSize.Width,this.DesktopLocation.Y);
            }
        }

        private void btnMoreType_Click(object sender, EventArgs e)
        {
            SpatialReferenceSelection spatialReferenceSelUI = new SpatialReferenceSelection();
            if (spatialReferenceSelUI.ShowDialog() == System.Windows.Forms.DialogResult.Yes)
            {
                MessageBox.Show(spatialReferenceSelUI.SpatialReference != null ?
                    spatialReferenceSelUI.SpatialReference.ToString() : string.Empty);
            }
        }

        private void cbIsRadiation_CheckedChanged(object sender, EventArgs e)
        {
            ProParas.PrjSetting.IsRadiation = cbIsRadiation.Checked;
        }

        private void cbIsOutMapTable_CheckedChanged(object sender, EventArgs e)
        {
            //ProParas.PrjSetting.IsOutMapTable = cbIsOutMapTable.Checked;
        }

        private void cbIsSolarZenith_CheckedChanged(object sender, EventArgs e)
        {
            ProParas.PrjSetting.IsSolarZenith = cbIsSolarZenith.Checked;
        }

        private void txtZXFBLNB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProParas.PrjSetting.OutResolutionY = (float)Convert.ToDouble(txtZXFBLNB.Text);
            }
        }

        private void txtZXFBLDX_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProParas.PrjSetting.OutResolutionX = (float)Convert.ToDouble(txtZXFBLDX.Text);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void btnSure_Click(object sender, EventArgs e)
        {
            switch (cmbOutRange.SelectedItem.ToString())
            {
                case "整轨投影":
                    ProParas.PrjSetting.OutEnvelope = null;
                    break;
                case "按中心点":
                    ProParas.PrjSetting.OutEnvelope = PrjEnvelope.CreateByCenter(Convert.ToDouble(txtCenterX.Text), Convert.ToDouble(txtCenterY.Text), Convert.ToDouble(numerWidth.Value), Convert.ToDouble(numerHeight.Value));
                    break;
                case "按四角经纬度":
                    ProParas.PrjSetting.OutEnvelope.MinX = Convert.ToDouble(txtMinX.Text);
                    ProParas.PrjSetting.OutEnvelope.MaxX = Convert.ToDouble(txtMaxX.Text);
                    ProParas.PrjSetting.OutEnvelope.MinY = Convert.ToDouble(txtMinY.Text);
                    ProParas.PrjSetting.OutEnvelope.MaxY = Convert.ToDouble(txtMaxY.Text);
                    break;
                case "按已定义的输出区域":
                    ProParas.PrjSetting.OutEnvelope = null;
                    break;
            }

            switch (cmbProType.SelectedItem.ToString())
            {

                case "等经纬度投影":
                    ProParas.DstSpatialRef = new SpatialReference(new GeographicCoordSystem());
                    break;
                case "麦卡托投影":
                    ProParas.DstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("MASMercator.prj");
                    break;
                case "兰勃托投影":
                    ProParas.DstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("MASLambert.prj");
                    break;
                case "极射赤面投影":
                    ProParas.DstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("MasPolarStere.prj");
                    break;
                case "等面积投影":
                    //ProParas.DstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByPrjFile("");
                    break;
            }
        }

    }
}
