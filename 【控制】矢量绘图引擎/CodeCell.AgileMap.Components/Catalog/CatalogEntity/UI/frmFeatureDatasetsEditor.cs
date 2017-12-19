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
    public partial class frmFeatureDatasetsEditor : frmEntityEditor
    {
        public frmFeatureDatasetsEditor()
        {
            InitializeComponent();
        }

        protected override void FillEntityToCotrols()
        {
            if (_isNew)
                Text = "新建空间数据集...";
            else
                Text = "编辑空间数据集属性...";
            SpatialFeatureDataset ds = _entity as SpatialFeatureDataset;
            if (_isNew)
                return;
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

        protected override bool IsCanSave()
        {
            if (string.IsNullOrEmpty(txtName.Text))
            {
                MsgBox.ShowInfo("空间数据集名称必须输入。");
                return false;
            }
            return true;
        }

        protected override void CollectValuesToEntity()
        {
            _entity.Name = txtName.Text;
            _entity.Description = txtDecription.Text;
            (_entity as SpatialFeatureDataset).Source = txtSource.Text;
            (_entity as SpatialFeatureDataset).MapScale = (int)txtMapScale.Value;
            ISpatialReference sref = ucSpatialRef1.SpatialReference;
            if (sref != null)
                (_entity as SpatialFeatureDataset).SpatialRef = sref.ToString();
        }
    }
}
