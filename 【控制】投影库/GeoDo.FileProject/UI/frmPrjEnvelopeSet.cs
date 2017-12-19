using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public partial class frmPrjEnvelopeSet : Form
    {
        private ISpatialReference _spatialRef;

        public frmPrjEnvelopeSet()
        {
            InitializeComponent();
        }

        public void SetArgs(ISpatialReference spatialRef, PrjEnvelope prjEnvelope, float resolutionX, float resolutionY, string outDir)
        {
            _spatialRef = spatialRef == null ? SpatialReference.GetDefault() : spatialRef;
            prjEnvelopeSet1.SetArgs(spatialRef, prjEnvelope, resolutionX, resolutionY);
            txtOutDirOrFile.Text = outDir;
            this.Controls.Add(prjEnvelopeSet1);
        }

        public void SetDefault(IRasterDataProvider rasterDataProvider)
        {
            prjArgsSelectBand1.SetArgs(rasterDataProvider);
        }

        public PrjOutArg PrjOutArg
        {
            get
            {
                PrjOutArg prjOutArg = new PrjOutArg(_spatialRef, new PrjEnvelopeItem[] {new PrjEnvelopeItem("DXX", prjEnvelopeSet1.PrjEnvelope) }, 
                    prjEnvelopeSet1.ResolutionX, prjEnvelopeSet1.ResolutionY, txtOutDirOrFile.Text);
                prjOutArg.SelectedBands = prjArgsSelectBand1.SelectedBandNumbers();
                return prjOutArg;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (prjEnvelopeSet1.PrjEnvelope == PrjEnvelope.Empty)
                return;
            if (prjEnvelopeSet1.ResolutionX <= 0f || prjEnvelopeSet1.ResolutionY <= 0f)
                return;
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOutDirOrFile_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog diag = new FolderBrowserDialog())
            {
                if (diag.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    txtOutDirOrFile.Text = diag.SelectedPath;
                }
            }
        }
    }
}
