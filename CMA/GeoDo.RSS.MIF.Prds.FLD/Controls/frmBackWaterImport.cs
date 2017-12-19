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
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public partial class frmBackWaterImport : Form
    {
        private Size _size = Size.Empty;
        private ShapePoint _leftUpCoord = null;
        private float _resolution = 0;
        private string _filename = null;
        private Feature _maskObj = null;
        private IRasterDataProvider _datValue = null;

        public frmBackWaterImport()
        {
            InitializeComponent();
        }

        public string FileName
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

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (_maskObj != null)
                _maskObj.Dispose();
            _datValue = null;
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Filter = "SMART Raster(*.dat)|*.dat|矢量数据(*.shp)|*.shp";
                dlg.InitialDirectory = Path.Combine(MifEnvironment.GetWorkspaceDir(), "\\FLD");
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string filename = dlg.FileName;
                    string extName = Path.GetExtension(filename).ToUpper();
                    switch (extName)
                    {
                        case ".SHP":
                            LoadMaskTemplateFromShapeFile(filename);
                            if (_maskObj != null)
                            {
                                txtFileName.Text = _maskObj.FieldNames[0] ;
                            }
                            _filename = filename;
                            return;
                        case ".DAT":
                            using (IRasterDataProvider udr = RasterDataDriver.Open(dlg.FileName) as IRasterDataProvider)
                            {
                                //if (udr == null || ((Math.Abs(Math.Round(udr.CoordEnvelope.MinX, 4) - Math.Round(_leftUpCoord.X, 4)) > udr.ResolutionX
                                //         || Math.Abs(Math.Round(udr.CoordEnvelope.MaxY, 4) - Math.Round(_leftUpCoord.Y, 4)) > udr.ResolutionY)
                                //     || udr.Width != _size.Width
                                //     || udr.Height != _size.Height
                                //     || udr.ResolutionX - _resolution > 1.0E-15
                                //     || udr.ResolutionY - _resolution > 1.0E-15))
                                //{
                                //    //MsgBox.ShowInfo("选择的背景水体与当前水体位置信息不一致,请重新选择!");
                                //    return;
                                //}
                                txtFileName.Text = Path.GetFileNameWithoutExtension(filename);
                                _filename = filename;
                            }
                            break;
                        default:
                            return;
                    }
                }
            }
        }

        private void txtFileName_DoubleClick(object sender, EventArgs e)
        {
            btnOpenFile_Click(null, null);
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
                        if (_maskObj != null )
                        {
                            txtFileName.Text = filename;
                        }
                        return true;
                    default:
                        return false;
                }
            }
            return false;
        }

        private void LoadMaskTemplateFromShapeFile(string filename)
        {
            IVectorFeatureDataReader vdr =  VectorDataReaderFactory.GetUniversalDataReader(filename) as IVectorFeatureDataReader;
            if (vdr == null)
            {
                //MsgBox.ShowInfo("输入的矢量文件\"" + filename + "\"格式错误,无法作为模板装入!");
                return;
            }
            if (vdr.ShapeType != enumShapeType.Polygon)
            {
                //MsgBox.ShowInfo("不支持非多边形矢量文件作为模板装入!");
                return;
            }
            try
            {
                _maskObj = vdr.Features[0];
                //int fetCount = vdr.FeatureCount;
                //if (fetCount == 1)
                //{
                //    SetMaskTemplateMatrixFromFeature(filename, vdr.FetchFirstFeature());
                //}
                //else
                //{
                //    using (frmFeatureSelector frm = new frmFeatureSelector(vdr.Features))
                //    {
                //        if (frm.ShowDialog() == DialogResult.OK)
                //        {
                //            Feature[] fets = frm.GetSelectedVectorFeatures();
                //            if (fets == null || fets.Length == 0)
                //                return;
                //            for (int i = 0; i < fets.Length; i++)
                //            {
                //                SetMaskTemplateMatrixFromFeature(filename, fets[i]);
                //            }
                //        }
                //    }
                //}
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
            _maskObj = vectorFeature;
        }
    }
}
