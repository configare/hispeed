using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using GeoDo.RSS.Core.CA;

namespace GeoDo.RSS.CA
{
    [Export(typeof(IRgbProcessorArgEditor))]
    public partial class frmMiddleFilterArgEditor : frmRgbArgsEditor
    {
        private RgbWndProcessorArg _actualArg = null;
       
        public frmMiddleFilterArgEditor()
        {
            InitializeComponent();
            Load += new EventHandler(frmMiddleFilterArgEditor_Load);
        }

        void frmMiddleFilterArgEditor_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            _actualArg = _arg as RgbWndProcessorArg;
            InitCollectWindow();
        }

        private void InitCollectWindow()
        {
            cbxWindowRadius.Items.Add(3);
            cbxWindowRadius.Items.Add(5);
            cbxWindowRadius.Items.Add(7);
            cbxWindowRadius.Items.Add(9);
            cbxWindowRadius.SelectedIndexChanged += new EventHandler(collectedWindowRadius_SelectedIndexChanged);
            cbxWindowRadius.SelectedIndex = 0;
        }

        private void collectedWindowRadius_SelectedIndexChanged(object sender, EventArgs e)
        {
            TryApply();
        }

        /// <summary>
        /// 将控件中的参数值赋值到参数对象中
        /// </summary>
        protected override void CollectArguments()
        {
            _actualArg.WndHeight = _actualArg.WndWidth = int.Parse(cbxWindowRadius.SelectedItem.ToString());
        }

        public override bool IsSupport(System.Type type)
        {
            return type.Equals(typeof(RgbProcessorMiddleFilter)) || type.Equals(typeof(RgbProcessorAverageFilter));
        }
    }
}
