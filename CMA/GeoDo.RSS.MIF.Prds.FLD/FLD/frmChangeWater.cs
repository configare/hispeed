using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public partial class frmChangeWater : Form
    {
        public frmChangeWater()
        {
            InitializeComponent();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (vectorFeatureListView.SelectedItems == null || vectorFeatureListView.SelectedItems.Count == 0)
                return;
            foreach (ListViewItem lvItem in vectorFeatureListView.SelectedItems)
            {
                //VectorFeature vectorFeature = lvItem.Tag as VectorFeature;
                //DeleteVectorFeature(vectorFeature);
                //vectorFeatureListView.Items.Remove(lvItem);
            }
        }

        private void btnChooseRegion_Click(object sender, EventArgs e)
        {
            //using (frmStatSubRegionTemplates frm = new frmStatSubRegionTemplates(new Size(_session.AIOAgent.RasterDataReader.Samples, _session.AIOAgent.RasterDataReader.LineCount),
            //   _session.AIOAgent.RasterDataReader.LeftUpCoord, (float)_session.AIOAgent.RasterDataReader.LongitudeResolution))
            //{
            //    if (frm.ShowDialog() == DialogResult.OK)
            //    {
            //        if (frm.MaskObjects != null && frm.MaskObjects.Length != 0)
            //        {
            //            MaskTemplateObj obj = frm.MaskObjects[0];
            //            if (obj != null)
            //            {
            //                if (AddShapeAIO != null)
            //                    AddShapeAIO(sender, obj.Feature);
            //            }
            //        }
            //    }
            //}
        }
    }
}
