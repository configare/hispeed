using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Core
{
    public partial class frmMaskTemplate : Form
    {
        private Size _size = Size.Empty;
        private ShapePoint _leftUpCoord = null;
        private float _resolution = 0;
        //private List<MaskTemplateObj> _maskObjs = null;

        private string _filename = null;

        private void SetMVGSelect()
        {

        }

        public frmMaskTemplate()
        {
            InitializeComponent();
        }

        public frmMaskTemplate(Size size, 
            //ShapePoint leftUpCoord, 
            float resolution)
        {
            _size = size;
            //_leftUpCoord = leftUpCoord;
            _resolution = resolution;
            InitializeComponent();
        }

        //public MaskTemplateObj[] MaskObject
        //{
        //    get { return _maskObjs != null && _maskObjs.Count > 0 ? _maskObjs.ToArray() : null; }
        //}

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            //if (_maskObjs != null)
            //    _maskObjs.Clear();
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "矢量数据(*.shp)|*.shp";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string filename = dlg.FileName;
                    string extName = Path.GetExtension(filename).ToUpper();
                    switch (extName)
                    {
                        case ".SHP":
                            LoadMaskTemplateFromShapeFile(filename);
                            //if (_maskObjs != null && _maskObjs.Count > 0)
                            //{
                            //    foreach (MaskTemplateObj obj in _maskObjs)
                            //        txtFileName.Text += obj.Name + ",";
                            //    if (txtFileName.Text.EndsWith(","))
                            //        txtFileName.Text = txtFileName.Text.Substring(0, txtFileName.Text.Length - 1);
                            //}
                            _filename = filename;
                            return;
                        default:
                            return;
                    }
                }
            }
        }

        public bool OpenFileWithoutForm(string filename)
        {
            if (File.Exists(filename))
            {
                string extName = Path.GetExtension(filename).ToUpper();
                switch (extName)
                {
                    case ".SHP":
                        LoadMaskTemplateFromShapeFile(filename);
                        //if (_maskObjs != null && _maskObjs.Count > 0)
                        //{
                        //    foreach (MaskTemplateObj obj in _maskObjs)
                        //        txtFileName.Text += obj.Name + ",";
                        //    if (txtFileName.Text.EndsWith(","))
                        //        txtFileName.Text = txtFileName.Text.Substring(0, txtFileName.Text.Length - 1);
                        //}
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        private void LoadMaskTemplateFromShapeFile(string filename)
        {
            IVectorFeatureDataReader vdr = VectorDataReaderFactory.GetUniversalDataReader(filename) as IVectorFeatureDataReader;
            if (vdr == null)
            {
                MessageBox.Show("输入的矢量文件\"" + filename + "\"格式错误,无法作为模板装入!");
                return;
            }
            if (vdr.ShapeType != enumShapeType.Polygon)
            {
                MessageBox.Show("不支持非多边形矢量文件作为模板装入!");
                return;
            }
            try
            {
                int fetCount = vdr.FeatureCount;
                if (fetCount == 1)
                {
                    SetMaskTemplateMatrixFromFeature(filename, vdr.Features[0]);
                }
                else
                {
                    using (frmFeatureSelector frm = new frmFeatureSelector(vdr.Features))
                    {
                        //frm.TopMost = true;
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            txtFileName.Text = filename;
                            Feature[] fets = frm.GetSelectedVectorFeatures();
                            if (fets == null || fets.Length == 0)
                                return;
                            for (int i = 0; i < fets.Length; i++)
                            {
                                SetMaskTemplateMatrixFromFeature(filename, fets[i]);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (vdr != null)
                    vdr.Dispose();
            }
        }

        private void SetMaskTemplateMatrixFromFeature(string name, Feature vectorFeature)
        {
            if (vectorFeature == null)
                return;
            name = Path.GetFileName(name);
            if (vectorFeature.FieldNames != null && vectorFeature.FieldNames.Length > 0)
            {
                if (Array.IndexOf(vectorFeature.FieldNames, "NAME") >= 0)
                    name += (":" + vectorFeature.GetFieldValue("NAME"));
                else if (Array.IndexOf(vectorFeature.FieldNames, "名称") >= 0)
                    name += (":" + vectorFeature.GetFieldValue("名称"));
                else
                    name += (":" + vectorFeature.FieldValues[0]);
            }
            //if (_maskObjs == null)
            //    _maskObjs = new List<MaskTemplateObj>();
            //MaskTemplateObj obj = new MaskTemplateObj(name, vectorFeature, _size, _leftUpCoord, _resolution);
            //_maskObjs.Add(obj);
        }

        public string Filename
        {
            get { return _filename; }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _filename = null;
            Close();
        }

        private void txtFileName_DoubleClick(object sender, EventArgs e)
        {
            btnOpenFile_Click(null, null);
        }

        private void frmMaskTemplate_Load(object sender, EventArgs e)
        {

        }
    }
}
