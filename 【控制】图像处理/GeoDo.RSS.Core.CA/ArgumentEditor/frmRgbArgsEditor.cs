using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace GeoDo.RSS.Core.CA
{
    public partial class frmRgbArgsEditor : Form,IRgbProcessorArgEditor
    {
        protected OnArgEditorApplyClick _argEditorApplyClick = null ;
        protected OnArgEditorOkClick _argEditorOkClick = null;
        protected OnArgEditorCancelClick _argEditorCancelClick = null;
        protected OnArgEditorPreviewing _argEditorPreviewing = null;
        protected RgbProcessorArg _arg = null;
        protected IRgbArgEditorEnvironmentSupport _env = null;
        protected IRgbProcessor _processor = null;

        public frmRgbArgsEditor()
        {
            InitializeComponent();
            Load += new EventHandler(frmRgbArgsEditor_Load);
            FormClosed += new FormClosedEventHandler(frmRgbArgsEditor_FormClosed);
        }

        void frmRgbArgsEditor_Load(object sender, EventArgs e)
        {
        }
        
        public OnArgEditorApplyClick OnApplyClicked
        {
            get { return _argEditorApplyClick; }
            set { _argEditorApplyClick = value; }
        }

        public OnArgEditorCancelClick OnCancelClicked
        {
            get { return _argEditorCancelClick; }
            set { _argEditorCancelClick = value; }
        }

        public OnArgEditorOkClick OnOkClicked
        {
            get { return _argEditorOkClick; }
            set { _argEditorOkClick = value; }
        }

        public OnArgEditorPreviewing OnPreviewing
        {
            get { return _argEditorPreviewing; }
            set { _argEditorPreviewing = value; }
        }

        public IRgbProcessor Processor
        {
            get { return _processor; }
            set { _processor = value; }
        }

        public void Show(RgbProcessorArg arg)
        {
            if (arg == null)
                return;
            _arg = arg;
            Show();
        }

        public DialogResult ShowDialog(RgbProcessorArg arg)
        {
            _arg = arg;
            return ShowDialog();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            CollectArguments();
            if (_argEditorApplyClick != null)
                _argEditorApplyClick(this, _arg);
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            CollectArguments();
            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            Close();
        }

        void frmRgbArgsEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (_argEditorOkClick != null && !Modal)
                    _argEditorOkClick(this, _arg);
            }
            else
            {
                if (_argEditorCancelClick != null && !Modal)
                    _argEditorCancelClick(this, _arg);
            }
            _argEditorApplyClick = null;
            _argEditorOkClick = null;
            _argEditorCancelClick = null;
            _argEditorPreviewing = null;
        }

        public virtual bool IsSupport(Type type)
        {
            return false;
        }

        protected virtual void TryApply()
        {
            CollectArguments();
            if (ckPreviewing.Checked)
            {
                TryRefreshOutside();
            }
        }

        protected virtual void CollectArguments()
        {
        }

        protected virtual void TryRefreshOutside()
        {
            if (_argEditorPreviewing != null)
                _argEditorPreviewing(this, _arg);
        }

        public void Init(IRgbArgEditorEnvironmentSupport env, IRgbProcessor processor)
        {
            _env = env;
            _processor = processor;
        }
    }
}
