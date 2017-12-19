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
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.PrdVal
{
    public partial class UCAeronetSiteArgs : UserControl, IArgumentEditorUI2
    {
        private Action<object> _handler;
        private bool _IsExcuteArgumentValueChangedEvent = false;
        private CoordEnvelope _envelope = null;

        public UCAeronetSiteArgs()
        {
            InitializeComponent();
        }

        public object GetArgumentValue()
        {
            ValArguments args = new ValArguments();
            if (!string.IsNullOrEmpty(txtToValDir.Text))
                args.FileNamesToVal = new string[] { txtToValDir.Text };
            if (lstForValFiles.Items.Count > 1)
            {
                List<string> fileList=new List<string>();
                foreach (string item in lstForValFiles.Items)
                {
                    fileList.Add(item);
                }
                args.FileNamesForVal = fileList.ToArray();
            }
            args.CreatScatter = chbScatter.Checked;
            args.CreatTimeSeq = chbTimeSeq.Checked;
            args.CreatHistogram = chbHistogram.Checked;
            args.CreatRMSE = chbRmse.Checked;
            return args;
        }

        public void InitControl(IExtractPanel panel, ArgumentBase arg)
        {
            
        }

        public bool IsExcuteArgumentValueChangedEvent
        {
            get
            {
                return _IsExcuteArgumentValueChangedEvent;
            }
            set
            {
                
            }
        }

        public object ParseArgumentValue(System.Xml.Linq.XElement ele)
        {
            return null;
        }

        public void SetChangeHandler(Action<object> handler)
        {
            _handler = handler;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(txtToValDir.Text)||(lstForValFiles.Items==null||lstForValFiles.Items.Count<1))
                return ;
            if (!chbHistogram.Checked && !chbRmse.Checked && !chbScatter.Checked && !chbTimeSeq.Checked)
            {
                MessageBox.Show("未选择验证方式！");
                return;
            }
            if (_handler != null)
                _handler(GetArgumentValue());
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog frm = new OpenFileDialog())
            {
                frm.Filter = "待验证卫星数据文件(*.hdf)|*.hdf";
                frm.Multiselect = true;
                frm.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in frm.FileNames)
                    {
                        if (CheckValDataFile(file))
                            lstForValFiles.Items.Add(file);
                        else
                        {
                            MessageBox.Show("所选数据地理范围不一致，可选择范围多时次数据！");
                            break;
                        }
                    }
                }
            }
        }

        private bool CheckValDataFile(string file)
        {
            if (!File.Exists(file))
                return false;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(file) as IRasterDataProvider)
            {
                if (_envelope == null)
                    _envelope = dataPrd.CoordEnvelope;
                else
                {
                    if (_envelope.MinX != dataPrd.CoordEnvelope.MinX|| _envelope.MaxX != dataPrd.CoordEnvelope.MaxX ||
                        _envelope.MinY != dataPrd.CoordEnvelope.MinY||_envelope.MaxY!=dataPrd.CoordEnvelope.MaxY)
                        return false;
                }
            }
            return true;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstForValFiles.SelectedItem != null)
            {
                foreach (string item in lstForValFiles.SelectedItems)
                    lstForValFiles.Items.Remove(item);
            }
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                //dialog.Filter = "Aeronet验证数据文件(*.lev20,*.lev15,*lev10)|*.lev20;*.lev15;*.lev10";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    txtToValDir.Text = dialog.SelectedPath;
                }
            }
            
        }
    }
}
