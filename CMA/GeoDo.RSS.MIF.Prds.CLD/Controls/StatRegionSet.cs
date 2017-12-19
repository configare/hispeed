using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.HDF4;
using GeoDo.FileProject;
using CodeCell.AgileMap.Core;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public partial class StatRegionSet : Form
    {
        private static string _path = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\ProductArgs\CLD\StatRegion.xml";
        private List<PrjEnvelopeItem> _envList = new List<PrjEnvelopeItem>();
        public static bool UseRegion = false;
        public static bool UseRecgRegion = false;
        public static bool UseVectorAOIRegion = false;
        private static string _VectorAOIName = null;
        private static GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer = null;
        private static List<string> fieldValues = null;
        private static GeoDo.RSS.Core.DrawEngine.CoordEnvelope _aoiEnv = null;
        public static CoordEnvelope SubRegionEnv = null;
        public static Size SubRegionOutSize;
        public static int[] SubRegionOutIndex = null;
        public static CoordEnvelope SubRegionEnvLeft = null;
        public static Size SubRegionOutSizeLeft;
        public static int[] SubRegionOutIndexLeft = null;
        public static CoordEnvelope SubRegionEnvRight = null;
        public static Size SubRegionOutSizeRight;
        public static int[] SubRegionOutIndexRight = null;

        static InputArg _arg;

        public StatRegionSet()
        {
            InitializeComponent();
            Load += new EventHandler(frm_Load);
            lstRegions.SelectedIndexChanged += new EventHandler(lstRegionsSelectedIndexChanged);
        }

        #region 矩形区域
        public static PrjEnvelopeItem SelectedRegionEnvelope
        {
            get
            {
                if (UseRegion || UseRecgRegion)
                    return _arg.SelectedRegionEnvelope;
                return null;
            }
        }

        public static Envelope Envelope
        {
            get
            {
                if (UseRegion || UseRecgRegion)
                {
                    PrjEnvelope env = _arg.SelectedRegionEnvelope.PrjEnvelope;
                    return new Envelope(env.MinX,env.MinY,env.MaxX,env.MaxY);
                }
                return null;
            }
        }

        public static string RegionName
        {
            get
            {
                if (UseRegion || UseRecgRegion)
                    return _arg.SelectedRegionEnvelope.Name;
                return null;
            }
        }
        #endregion
        void frm_Load(object sender, EventArgs e)
        {
            _envList = new List<PrjEnvelopeItem>();
            InputArg arg=null;
            if (File.Exists(_path))
            {
                arg = InputArg.ParseStatRegions(_path);
                if (arg != null)
                {
                    InitSetting(arg);
                }

            }
            if (arg != null&&(UseRegion || UseRecgRegion))
            {
                string itemname = arg.SelectedRegionEnvelope.Name;
                foreach (PrjEnvelopeItem item in _envList)
                {
                    if (itemname == item.Name)
                    {
                        lstRegions.SelectedItem = itemname;
                        cbxUseRegion.Checked = true;
                        radiRecg.Checked = true;
                        break;
                    }
                }
            }
            else if (aoiContainer != null && aoiContainer.AOIs.Count()>0)
            {
                cbxUseRegion.Checked = true;
                radiVectorAOI.Checked = true;
                string regionName = "未命名";
                if (fieldValues != null )
                {
                    if (fieldValues.Count()>0)
                    {
                        regionName = "";
                        foreach (string region in fieldValues)
                        {
                            regionName += region;
                        }
                    } 
                    else
                        fieldValues.Clear();
                }
                txtAOIName.Text = regionName;
                _VectorAOIName = regionName;
            }
        }

        private void InitSetting(InputArg arg)
        {
            //SetSelectedNode(arg.Bands);
            if (arg.ValidEnvelopes != null && arg.ValidEnvelopes.Length>0)
            {
                _envList = arg.ValidEnvelopes.ToList();
                foreach (PrjEnvelopeItem item in _envList)
                {
                    lstRegions.Items.Add(item.Name);
                }
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            //根据界面设置进行投影、拼接、分幅、入库
            ////检查参数是否设置完全
            btnOk.Enabled = false;
            //UseRegion = cbxUseRegion.Checked;
            if (cbxUseRegion.Checked)
            {
                try
                {
                    if (radiRecg.Checked)
                    {
                        if (!CheckArgsIsOk())
                            return;
                        _arg = new InputArg(_path);
                        _arg.ValidEnvelopes = _envList.ToArray();
                        string name = lstRegions.SelectedItem.ToString();
                        foreach (PrjEnvelopeItem item in _envList)
                        {
                            if (name == item.Name)
                            {
                                _arg.SelectedRegionEnvelope = item;
                                break;
                            }
                        }
                        _arg.RegionToXml(_path);
                        UseRecgRegion = true;
                        UseRegion = true;
                        UseVectorAOIRegion = false;
                        this.Close();
                    }
                    else if (radiVectorAOI.Checked)
                    {
                        if (aoiContainer == null || aoiContainer.AOIs.Count() == 0)
                            throw new ArgumentException("未选择任何有效的AOI区域！");
                        if (string.IsNullOrWhiteSpace(txtAOIName.Text))
                            throw new ArgumentException("请输入有效的AOI区域名称！");
                        UseRecgRegion = false;
                        UseRegion = false;
                        UseVectorAOIRegion = true;
                        _VectorAOIName = txtAOIName.Text;
                        _aoiEnv = GetGeoRect(aoiContainer);
                        this.Close();
                    }
                    else
                    {
                        UseRegion = false;
                        UseRecgRegion = false;
                        UseVectorAOIRegion = false;
                        return;
                    }
                }
                catch(System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    btnOk.Enabled = true;
                }
            }
            else
            {
                UseRegion = false;
                UseRecgRegion = false;
                UseVectorAOIRegion = false;
                this.Close();
                return;
            }
        }

       private bool CheckArgsIsOk()
        {
            if (lstRegions.Items.Count<1)
               throw new ArgumentException("请设置统计区域！");
            if (lstRegions.Items.Count>0&&lstRegions.SelectedItems.Count != 1)
                throw new ArgumentException("请选择统计区域！", "提示信息");
            return true;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void lstRegionsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRegions.SelectedItem != null)
            {
                string name = lstRegions.SelectedItem.ToString();
                foreach (PrjEnvelopeItem item in _envList)
                {
                    if (name == item.Name)
                    {
                        txtRegionName.Text = name;
                        this.ucGeoRangeControl1.SetGeoEnvelope(item.PrjEnvelope);
                    }
                }
            }
        }

        private void btnAddEvp_Click(object sender, EventArgs e)
        {
            string evpName = txtRegionName.Text.Trim();
            if (string.IsNullOrWhiteSpace(evpName))
            {
                MessageBox.Show("请输入范围标识");
                return;
            }
            foreach (PrjEnvelopeItem item in _envList)
            {
                if (item.Name == evpName)
                {
                    MessageBox.Show("已存在名为" + evpName + "的区域范围名称，请重新输入！");
                    return;
                }
            }
            RasterProject.PrjEnvelope envelope = GetEnvelopeFromUI();
            if (envelope.Width == 0 || envelope.Height == 0
                || !CheckRegion(envelope.MinX, envelope.MaxX, -180, 180)
                || !CheckRegion(envelope.MinY, envelope.MaxY, -90, 90))
            {
                MessageBox.Show("请输入正确的地理坐标范围值！");
                return;
            }
            PrjEnvelopeItem env = new PrjEnvelopeItem(txtRegionName.Text, envelope);
            lstRegions.Items.Add(env.Name);
            _envList.Add(env);
        }

        private RasterProject.PrjEnvelope GetEnvelopeFromUI()
        {
                return new RasterProject.PrjEnvelope(ucGeoRangeControl1.MinX, ucGeoRangeControl1.MaxX,
                        ucGeoRangeControl1.MinY, ucGeoRangeControl1.MaxY);
        }

        private bool CheckRegion(double min, double max, double minLimit, double maxLimit)
        {
            if (min < max)
            {
                if (max <= maxLimit && min >= minLimit)
                {
                    return true;
                }
            }
            return false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (lstRegions.SelectedItem != null)
            {
                foreach (PrjEnvelopeItem item in _envList)
                {
                    if (item.Name == lstRegions.SelectedItem.ToString())
                    {
                        _envList.Remove(item);
                        break;
                    }
                }
                lstRegions.Items.Remove(lstRegions.SelectedItem);
            }
        }

        #region
        //获取多个aoi的外接矩形
        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope GetGeoRect(GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer)
        {
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope retRect = null;
            foreach (object obj in aoiContainer.AOIs)
            {
                string name;
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope rect = null;
                rect = GetGeoRect(obj as Feature, out name);
                if (rect == null)
                    continue;
                if (retRect == null)
                    retRect = rect;
                else
                    retRect = retRect.Union(rect);
            }
            return retRect;
        }

        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope GetGeoRect(Feature feature, out string name)
        {
            name = string.Empty;
            if (feature.Geometry == null)
                return null;
            Envelope evp = feature.Geometry.Envelope.Clone() as Envelope;
            if (evp == null)
                return null;
            return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
        }

        private void btnAOIStatRegion_Click(object sender, EventArgs e)
        {
            aoiContainer = new GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer();
            fieldValues = new List<string>();
            int fieldIndex = -1; string fieldName; string shapeFilename; string regionName = "";
            try
            {
                if (fieldValues != null)
                    fieldValues.Clear();
                if (aoiContainer != null)
                    aoiContainer.Dispose();
                using (frmStatRegionTemplates frm = new frmStatRegionTemplates())
                {
                    frm.listView1.MultiSelect = true;
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        Feature[] fets = null;//frm.GetSelectedFeatures();
                        fets = frm.GetStatFeatures(out fieldName, out shapeFilename, out fieldIndex);
                        if (fets == null)
                            throw new ArgumentException("未选定目标区域，请选择区域!");
                        else
                        {
                            {
                                foreach (Feature fet in fets)
                                {
                                    fieldValues.Add(fet.GetFieldValue(fieldIndex)); //获得选择区域名称
                                    aoiContainer.AddAOI(fet);
                                }
                                regionName = "";
                                foreach (string region in fieldValues)
                                {
                                    regionName += region;
                                }
                            }
                        }
                        txtAOIName.Text = regionName;
                        if (aoiContainer == null)
                            throw new ArgumentException("未选定目标区域，请选择区域!");
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }
        private void cbxUseRegion_CheckedChanged(object sender, EventArgs e)
        {
            if (cbxUseRegion.Checked)
            {
                radiRecg.Enabled = true;
                radiVectorAOI.Enabled = true;
            }
        }

        private void radiRecg_CheckedChanged(object sender, EventArgs e)
        {
            if (radiVectorAOI.Checked)
            {
                btnAOIStatRegion.Enabled = true;
            }
            else
            {
                btnAOIStatRegion.Enabled = false;
            }
        }

        #region 矢量AOI区域
        public static string AOIName
        {
            get
            {
                if (UseVectorAOIRegion)
                    return _VectorAOIName;
                return null;
            }
        }

        public static Envelope AOIEnvelope
        {
            get
            {
                if (UseVectorAOIRegion)
                {
                    return new Envelope(_aoiEnv.MinX, _aoiEnv.MinY, _aoiEnv.MaxX, _aoiEnv.MaxY);
                }
                return null;
            }
        }

        public static PrjEnvelope AOIPrjEnvelope
        {
            get
            {
                if (UseVectorAOIRegion)
                {
                    return new PrjEnvelope(_aoiEnv.MinX, _aoiEnv.MaxX, _aoiEnv.MinY, _aoiEnv.MaxY);
                }
                return null;
            }
        }


        public static GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer AoiContainer
        {
            get
            {
                if (UseVectorAOIRegion)
                    return aoiContainer;
                return null;
            }
        }
        #endregion

        #endregion

    }
}
