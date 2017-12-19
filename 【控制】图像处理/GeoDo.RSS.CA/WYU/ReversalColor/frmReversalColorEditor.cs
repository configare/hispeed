using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmReversalColorEditor : frmRgbArgsEditor
    {
        private ReversalColorArg _actualArg = null;
        private frm3BandViewer _3bandViewer = null;
        private EventHandler _refreshSubscriber = null;

        public frmReversalColorEditor()
        {
            InitializeComponent();
            Load += new EventHandler(frmLogEnhance_Load);
            _refreshSubscriber = new EventHandler(SubscribeRefresh);
            FormClosed += new FormClosedEventHandler(frmCurveProcessorArgEditor_FormClosed);
        }

        public Bitmap ActiveDrawing
        {
            get
            {
                return _env != null ? (_env.ActiveDrawing != null ? _env.ActiveDrawing : null) : null;
            }
        }

        void SubscribeRefresh(object sender, EventArgs e)
        {
            if (_3bandViewer != null)
            {
                _3bandViewer.UpdateView();
            }
            else
                ck3BandView.Checked = false;
        }

        void frmLogEnhance_Load(object sender, EventArgs e)
        {
            _actualArg = _arg as ReversalColorArg;
            _actualArg.ApplyR = true;
            _actualArg.ApplyG = true;
            _actualArg.ApplyB = true;
            _env.CanvasRefreshSubscribers.Add(_refreshSubscriber);
            TryApply();
        }

        protected override void CollectArguments()
        {
            ApplyArgument();
        }

        public override bool IsSupport(System.Type type)
        {
            return type.Equals(typeof(RgbProcessorReversalColor));
        }

        private void rdiRed_CheckedChanged(object sender, EventArgs e)
        {
            ApplyArgument();
            TryApply();
        }

        private void rdiGreen_CheckedChanged(object sender, EventArgs e)
        {
            ApplyArgument();
            TryApply();
        }

        private void rdiBlue_CheckedChanged(object sender, EventArgs e)
        {
            ApplyArgument();
            TryApply();
        }

        private void ApplyArgument()
        {
            _actualArg.ApplyR = rdiRed.Checked;
            _actualArg.ApplyG = rdiGreen.Checked;
            _actualArg.ApplyB = rdiBlue.Checked;
        }

        private void ck3BandView_CheckedChanged(object sender, EventArgs e)
        {
            if (ck3BandView.Checked)
            {
                if (_3bandViewer == null)
                    _3bandViewer = new frm3BandViewer();
                _3bandViewer.Owner = this;
                _3bandViewer.Location = new Point(this.Location.X + this.Width, this.Location.Y);
                _3bandViewer.Show();
                _3bandViewer.UpdateView();
                _3bandViewer.FormClosed += new FormClosedEventHandler(_3bandViewer_FormClosed);
            }
            else
            {
                (_3bandViewer as frm3BandViewer).Close();
                _3bandViewer = null;
            }
        }

        void _3bandViewer_FormClosed(object sender, FormClosedEventArgs e)
        {
            ck3BandView.Checked = false;
        }

        private void Update3BandViewer()
        {
            if (_3bandViewer != null)
                _3bandViewer.UpdateView();
        }

        private void frmCurveProcessorArgEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_3bandViewer != null)
            {
                (_3bandViewer as Form).Close();
                _3bandViewer = null;
            }
        }

        private void btnAll_Click(object sender, EventArgs e)
        {
            rdiRed.Checked = true;
            rdiGreen.Checked = true;
            rdiBlue.Checked = true;
            ApplyArgument();
        }
    }
}
