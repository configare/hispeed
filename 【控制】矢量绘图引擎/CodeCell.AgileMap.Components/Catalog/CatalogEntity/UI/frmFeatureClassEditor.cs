using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.UIs;

namespace CodeCell.AgileMap.Components
{
    public partial class frmFeatureClassEditor : frmEntityEditor
    {
        private SpatialFeatureDataset _dataset = null;

        public frmFeatureClassEditor()
        {
            InitializeComponent();
        }

        internal void SetSpatialDataset(SpatialFeatureDataset ds)
        {
            _dataset = ds;
        }

        protected override void FillEntityToCotrols()
        {
            if (_isNew)
            {
                Text = "新建空间要素类...";
                if (_dataset != null)
                    FillDataset(_dataset);
                return;
            }
            else
                Text = "编辑空间要素类属性...";
            SpatialFeatureClass ds = _entity as SpatialFeatureClass;
            if (ds.SpatialFeatureDataset != null)
            {
                FillDataset(ds.SpatialFeatureDataset);
            }
            txtName.Text = _entity.Name;
            txtDecription.Text = _entity.Description;
            txtMapScale.Value = ds.MapScale;
            txtSource.Text = ds.Source;
            if (ds.SpatialRef != null)
            {
                ISpatialReference sref = SpatialReferenceFactory.GetSpatialReferenceByWKT(ds.SpatialRef, enumWKTSource.EsriPrjFile);
                ucSpatialRef1.SpatialReference = sref;
            }
        }

        private void FillDataset(SpatialFeatureDataset ds)
        {
            txtDataset.Text = ds.Name;
            txtDataset.Tag = ds;
        }

        protected override bool IsCanSave()
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MsgBox.ShowInfo("空间要素类名称必须输入。");
                return false;
            }
            return true;
        }

        protected override void CollectValuesToEntity()
        {
            _entity.Name = txtName.Text;
            _entity.Description = txtDecription.Text;
            (_entity as SpatialFeatureClass).DataTable = _entity.Name;
            (_entity as SpatialFeatureClass).Source = txtSource.Text;
            (_entity as SpatialFeatureClass).MapScale = (int)txtMapScale.Value;
            if(txtDataset.Tag != null)
                (_entity as SpatialFeatureClass).DatasetId = (txtDataset.Tag as SpatialFeatureDataset).Id;
            ISpatialReference sref = ucSpatialRef1.SpatialReference;
            if (sref != null)
                (_entity as SpatialFeatureClass).SpatialRef = sref.ToString();
        }

        private void btnDataset_Click(object sender, EventArgs e)
        {
            ICatalogEntity c = txtDataset.Tag as ICatalogEntity;
            if (c == null)
                return;
            using (frmFeatureDatasetsEditor frm = new frmFeatureDatasetsEditor())
            {
                frm.ShowDialog(ref c, false,true);
            }
        }
    }
}
