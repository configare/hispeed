using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GeoDo.RSS.MIF.Core;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.DRT
{
    public partial class UCExpCoefficientBase : UserControl, IArgumentEditorUI
    {
        private string _expType;
        private string _expFile;
        private Action<object> _handler;
        private object _cur = null;
        public delegate void BtnEditClickHandle(object CurArg);
        public BtnEditClickHandle BtnEdit_Click = null;

        public UCExpCoefficientBase()
        {
            InitializeComponent();
            Load += new EventHandler(UCExpCoefficientBase_Load);
        }

        private void InitControls()
        {
            lbFile.Location = new System.Drawing.Point(3, 6);
            cmbFile.Location = new System.Drawing.Point(lbFile.Right + 6, 6);
            cmbFile.Size = new System.Drawing.Size(this.Width - lbFile.Width - 36, 20);
            btEdit.Location = new System.Drawing.Point(this.Width - 24, 3);
            btEdit.Size = new System.Drawing.Size(24, 24);
            btEdit.UseVisualStyleBackColor = true;
        }

        public void SetExpType(string exptype)
        {
            _expType = exptype;
            InitCmb();
        }

        private void InitCmb()
        {
            switch (_expType.ToUpper())
            {
                case "SWI":
                    cmbFile.Text = Path.GetFileName(ArgsFileProvider.SWIExpsFile);
                    _expFile = ArgsFileProvider.SWIExpsFile;
                    break;
                case "VTI":
                    cmbFile.Text = Path.GetFileName(ArgsFileProvider.VTIExpsFile);
                    _expFile = ArgsFileProvider.VTIExpsFile;
                    break;
                case "TVDI":
                    lbFile.Text = "参数设置";
                    cmbFile.Text = Path.GetFileName(ArgsFileProvider.TVDIArgsFile);
                    _expFile = ArgsFileProvider.TVDIArgsFile;
                    break;
            }
        }

        void UCExpCoefficientBase_Load(object sender, EventArgs e)
        {
            InitControls();
            if (string.IsNullOrEmpty(_expFile))
                return;
            Reload();
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void Reload()
        {
            switch (_expType.ToUpper())
            {
                case "SWI":
                case "VTI":
                    _cur = ReadExpCofficientFile.LoadExpCoefficientFile(_expFile);
                    if (_cur == null)
                        _cur = new DRTExpCoefficientCollection();
                    break;
                case "TVDI":
                    _cur = ReadExpCofficientFile.LoadTVDIParasFile(_expFile);
                    if (_cur == null)
                        _cur = new TVDIParaClass();
                    break;
            }
        }


        public object GetArgumentValue()
        {
            return _cur;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void btEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_expFile) || !File.Exists(_expFile))
                return;
            using (frmDroughtSetting frm = new frmDroughtSetting(_cur, _expFile))
            {
                if (frm.ShowDialog() != DialogResult.OK)
                {
                    Reload();
                    //_cur = ReadExpCofficientFile.LoadExpCoefficientFile(_expFile);
                }
            }
            if (_handler != null)
                _handler(GetArgumentValue());
            if (BtnEdit_Click != null)
                BtnEdit_Click(_cur);

        }


        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }
    }
}
