#region Version Info
/*========================================================================
* 功能概述：根据设置的自定义区域提取GPC数据中的单波段数据，并存储为LDF文件
* 
* 创建者：$zhangyanbing$     时间：2014-2-19 09:10:15
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using GeoDo.RasterProject;
using System.Collections;
using System.Collections.Generic;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;
using System.Runtime.InteropServices;
using GeoDo.RSS.DF.GPC;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    /// <summary>
    /// 类名：GPCPreProcess
    /// 属性描述：搜寻指定文件夹下的GPC格式的D2数据，并根据设置的自定义区域提取GPC数据中的单波段数据，存储为LDF文件
    /// 创建者：zhangyanbing   创建日期：2014-2-19 09:10:15
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public partial class GPCPreProcess : Form
    {
        private  float _resX;
        private float _resY;
        CoordEnvelope _env = null;
        ISpatialReference _spatialRef = SpatialReference.GetDefault();
        private Dictionary<string,Int16> _needDatasets=new Dictionary<string,Int16>();
        private Dictionary<String, String> _allFiles = new Dictionary<String, String>();
        private Dictionary<string, Dictionary<String, Int16>> _allDatasets = new Dictionary<string, Dictionary<String, Int16>>();
        private bool _fillcolor = true;

        public GPCPreProcess()
        {
            InitializeComponent();
            this.treeviewdataset.Dock = DockStyle.Fill;
            treeviewdataset.CheckBoxes = true;
            treeviewdataset.AfterCheck += new TreeViewEventHandler(treeviewdataset_AfterCheck);
            //treeviewdataset.AfterSelect  +=new TreeViewEventHandler(treeviewdataset_AfterSelect);
        }

        #region 文件选择及树填充
        private void btnOpen_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtInDir.Text = dialog.SelectedPath;
                _allFiles.Clear();
                TryGetGPCfile(dialog.SelectedPath);
                if (_allFiles == null)
                {
                    errorProvider1.SetError(txtInDir, "当前路径不存在可处理的GPC数据");
                    return;
                }
                //AddD2DataSets();
                AddAllD2DataSets();
                //FillMyTreeView();
                FillAllTreeView();
            }

        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtOutDir.Text = dialog.SelectedPath;
            }
        }

        private void TryGetGPCfile(string dir)
        {
            string[] files = Directory.GetFiles(dir, "*.GPC", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                _allFiles= null;
            FileInfo finfo;
            long size;
            foreach (string file in files)
            {
                finfo = new FileInfo(file);
                size = finfo.Length;
                if (size == 871000)
                    _allFiles.Add(Path.GetFileName(file), file);
            }
        }

        private void AddD2DataSets()
        {
            _needDatasets.Add("Latitude", 1);
            //_needDatasets.Add("Longitude index (equal-area)", 2);
            //_needDatasets.Add("Western-most longitude index (equal-angle)", 3);
            //_needDatasets.Add("Eastern-most longitude index (equal-angle)", 4);
            _needDatasets.Add("Land/water/coast code", 5);//5
            _needDatasets.Add("Number of observations", 6);//6
            _needDatasets.Add("Number of daytime observations", 7);
            _needDatasets.Add("Mean cloud amount", 8);//8
            _needDatasets.Add("Mean IR-marginal cloud amount", 9);
            _needDatasets.Add("Frequency of mean cloud amount = 0-10%", 10);
            _needDatasets.Add("Frequency of mean cloud amount = 10-20%", 11);
            _needDatasets.Add("Frequency of mean cloud amount = 20-30%", 12);
            _needDatasets.Add("Frequency of mean cloud amount = 30-40%", 13);
            _needDatasets.Add("Frequency of mean cloud amount = 40-50%", 14);
            _needDatasets.Add("Frequency of mean cloud amount = 50-60%", 15);
            _needDatasets.Add("Frequency of mean cloud amount = 60-70%", 16);
            _needDatasets.Add("Frequency of mean cloud amount = 70-80%", 17);
            _needDatasets.Add("Frequency of mean cloud amount = 80-90%", 18);
            _needDatasets.Add("Frequency of mean cloud amount = 90-100%", 19);
            _needDatasets.Add("Mean cloud top pressure(PC)", 20);
            _needDatasets.Add("Standard deviation of spatial mean over time(PC)", 21);
            _needDatasets.Add("Time mean of standard deviation over space(PC)", 22);//23
            _needDatasets.Add("Cloud temperature(TC)", 23);
            _needDatasets.Add("Standard deviation of spatial mean over time(TC)", 24);
            _needDatasets.Add("Time mean of standard deviation over space(TC)", 25);
            _needDatasets.Add("Mean cloud optical thickness(TAU)", 26);//26
            _needDatasets.Add("Standard deviation of spatial mean over time(TAU)", 27);
            _needDatasets.Add("Time mean of standard deviation over space(TAU)", 28);
            _needDatasets.Add("Mean cloud water path(WP)", 29);//29
            _needDatasets.Add("Standard deviation of spatial mean over time(WP)", 30);//20
            _needDatasets.Add("Time mean of standard deviation over space(WP)", 31);
            _needDatasets.Add("Mean CA for low-level clouds", 32);
            _needDatasets.Add("Mean PC for low-level clouds", 33);
            _needDatasets.Add("Mean TC for low-level clouds", 34);
            _needDatasets.Add("Mean CA for middle-level clouds", 35);
            _needDatasets.Add("Mean PC for middle-level clouds", 36);
            _needDatasets.Add("Mean TC for middle-level clouds", 37);
            _needDatasets.Add("Mean CA for high-level clouds", 38);
            _needDatasets.Add("Mean PC for high-level clouds", 39);
            _needDatasets.Add("Mean TC for high-level clouds", 40);
            _needDatasets.Add("Mean CA for cloud type 1 = Cumulus, liquid", 41);
            _needDatasets.Add("Mean PC for cloud type 1 = Cumulus, liquid", 42);
            _needDatasets.Add("Mean TC for cloud type 1 = Cumulus, liquid", 43);
            _needDatasets.Add("Mean TAU for cloud type 1 = Cumulus, liquid", 44);
            _needDatasets.Add("Mean WP for cloud type 1 = Cumulus, liquid", 45);
            _needDatasets.Add("Mean CA for cloud type 2 = Stratocumulus, liquid", 46);
            _needDatasets.Add("Mean PC for cloud type 2 = Stratocumulus, liquid", 47);
            _needDatasets.Add("Mean TC for cloud type 2 = Stratocumulus, liquid", 48);
            _needDatasets.Add("Mean TAU for cloud type 2 = Stratocumulus, liquid", 49);
            _needDatasets.Add("Mean WP for cloud type 2 = Stratocumulus, liquid", 50);
            _needDatasets.Add("Mean CA for cloud type 3 = Stratus, liquid", 51);
            _needDatasets.Add("Mean PC for cloud type 3 = Stratus, liquid", 52);
            _needDatasets.Add("Mean TC for cloud type 3 = Stratus, liquid", 53);
            _needDatasets.Add("Mean TAU for cloud type 3 = Stratus, liquid", 54);
            _needDatasets.Add("Mean WP for cloud type 3 = Stratus, liquid", 55);
            _needDatasets.Add("Mean CA for cloud type 4 = Cumulus, ice", 56);
            _needDatasets.Add("Mean PC for cloud type 4 = Cumulus, ice", 57);
            _needDatasets.Add("Mean TC for cloud type 4 = Cumulus, ice", 58);
            _needDatasets.Add("Mean TAU for cloud type 4 = Cumulus, ice", 59);
            _needDatasets.Add("Mean WP for cloud type 4 = Cumulus, ice", 60);
            _needDatasets.Add("Mean CA for cloud type 5 = Stratocumulus, ice", 61);
            _needDatasets.Add("Mean PC for cloud type 5 = Stratocumulus, ice", 62);
            _needDatasets.Add("Mean TC for cloud type 5 = Stratocumulus, ice", 63);
            _needDatasets.Add("Mean TAU for cloud type 5 = Stratocumulus, ice", 64);
            _needDatasets.Add("Mean WP for cloud type 5 = Stratocumulus, ice", 65);
            _needDatasets.Add("Mean CA for cloud type 6 = Stratus, ice", 66);
            _needDatasets.Add("Mean PC for cloud type 6 = Stratus, ice", 67);
            _needDatasets.Add("Mean TC for cloud type 6 = Stratus, ice", 68);
            _needDatasets.Add("Mean TAU for cloud type 6 = Stratus, ice", 69);
            _needDatasets.Add("Mean WP for cloud type 6 = Stratus, ice", 70);
            _needDatasets.Add("Mean CA for cloud type 7 = Altocumulus, liquid", 71);
            _needDatasets.Add("Mean PC for cloud type 7 = Altocumulus, liquid", 72);
            _needDatasets.Add("Mean TC for cloud type 7 = Altocumulus, liquid", 73);
            _needDatasets.Add("Mean TAU for cloud type 7 = Altocumulus, liquid", 74);
            _needDatasets.Add("Mean WP for cloud type 7 = Altocumulus, liquid", 75);
            _needDatasets.Add("Mean CA for cloud type 8 = Altostratus, liquid", 76);
            _needDatasets.Add("Mean PC for cloud type 8 = Altostratus, liquid", 77);
            _needDatasets.Add("Mean TC for cloud type 8 = Altostratus, liquid", 78);
            _needDatasets.Add("Mean TAU for cloud type 8 = Altostratus, liquid", 79);
            _needDatasets.Add("Mean WP for cloud type 8 = Altostratus, liquid", 80);
            _needDatasets.Add("Mean CA for cloud type 9 = Nimbostratus, liquid", 81);
            _needDatasets.Add("Mean PC for cloud type 9 = Nimbostratus, liquid", 82);
            _needDatasets.Add("Mean TC for cloud type 9 = Nimbostratus, liquid", 83);
            _needDatasets.Add("Mean TAU for cloud type 9 = Nimbostratus, liquid", 84);
            _needDatasets.Add("Mean WP for cloud type 9 = Nimbostratus, liquid", 85);
            _needDatasets.Add("Mean CA for cloud type 10 = Altocumulus, ice", 86);
            _needDatasets.Add("Mean PC for cloud type 10 = Altocumulus, ice", 87);
            _needDatasets.Add("Mean TC for cloud type 10 = Altocumulus, ice", 88);
            _needDatasets.Add("Mean TAU for cloud type 10 = Altocumulus, ice", 89);
            _needDatasets.Add("Mean WP for cloud type 10 = Altocumulus, ice", 90);
            _needDatasets.Add("Mean CA for cloud type 11 = Altostratus, ice", 91);
            _needDatasets.Add("Mean PC for cloud type 11 = Altostratus, ice", 92);
            _needDatasets.Add("Mean TC for cloud type 11 = Altostratus, ice", 93);
            _needDatasets.Add("Mean TAU for cloud type 11 = Altostratus, ice", 94);
            _needDatasets.Add("Mean WP for cloud type 11 = Altostratus, ice", 95);
            _needDatasets.Add("Mean CA for cloud type 12 = Nimbostratus, ice", 96);
            _needDatasets.Add("Mean PC for cloud type 12 = Nimbostratus, ice", 97);
            _needDatasets.Add("Mean TC for cloud type 12 = Nimbostratus, ice", 98);
            _needDatasets.Add("Mean TAU for cloud type 12 = Nimbostratus, ice", 99);
            _needDatasets.Add("Mean WP for cloud type 12 = Nimbostratus, ice", 100);
            _needDatasets.Add("Mean CA for cloud type 13 = Cirrus", 101);
            _needDatasets.Add("Mean PC for cloud type 13 = Cirrus", 102);
            _needDatasets.Add("Mean TC for cloud type 13 = Cirrus", 103);
            _needDatasets.Add("Mean TAU for cloud type 13 = Cirrus", 104);
            _needDatasets.Add("Mean WP for cloud type 13 = Cirrus", 105);
            _needDatasets.Add("Mean CA for cloud type 14 = Cirrostratus", 106);
            _needDatasets.Add("Mean PC for cloud type 14 = Cirrostratus", 107);
            _needDatasets.Add("Mean TC for cloud type 14 = Cirrostratus", 108);
            _needDatasets.Add("Mean TAU for cloud type 14 = Cirrostratus", 109);
            _needDatasets.Add("Mean WP for cloud type 14 = Cirrostratus", 110);
            _needDatasets.Add("Mean CA for cloud type 15 = Deep convective", 111);
            _needDatasets.Add("Mean PC for cloud type 15 = Deep convective", 112);
            _needDatasets.Add("Mean TC for cloud type 15 = Deep convective", 113);
            _needDatasets.Add("Mean TAU for cloud type 15 = Deep convective", 114);
            _needDatasets.Add("Mean WP for cloud type 15 = Deep convective", 115);
            _needDatasets.Add("Mean TS from clear sky composite(TS)", 116);//116
            _needDatasets.Add("Time mean of standard deviation over space(TS)", 117);
            _needDatasets.Add("Mean RS from clear sky composite(RS)", 118);//118
            _needDatasets.Add("Mean ice/snow cover", 119);//119
            _needDatasets.Add("Mean Surface pressure (PS)", 120);
            _needDatasets.Add("Mean Near-surface air temperature (TSA)", 121);
            _needDatasets.Add("Mean Temperature at 740 mb (T)", 122);
            _needDatasets.Add("Mean Temperature at 500 mb (T)", 123);
            _needDatasets.Add("Mean Temperature at 375 mb (T)", 124);
            _needDatasets.Add("Mean Tropopause pressure (PT)", 125);
            _needDatasets.Add("Mean Tropopause temperature (TT)", 126);
            _needDatasets.Add("Mean Stratosphere temperature at 50 mb (T)", 127);
            _needDatasets.Add("Mean Precipitable water for 1000-680 mb (PW)", 128);
            _needDatasets.Add("Mean Precipitable water for 680-310 mb (PW)", 129);
            _needDatasets.Add("Mean Ozone column abundance (O3)", 130);
        }

        private void AddAllD2DataSets()
        {
            Dictionary<String, Int16> boxIdenti = new Dictionary<String, Int16>();
            boxIdenti.Add("Latitude", 1);
            //boxIdenti.Add("Longitude index (equal-area)", 2);
            //boxIdenti.Add("Western-most longitude index (equal-angle)", 3);
            //boxIdenti.Add("Eastern-most longitude index (equal-angle)", 4);
            boxIdenti.Add("Land water coast code", 5);//5
            boxIdenti.Add("Number of observations", 6);//6
            boxIdenti.Add("Number of daytime observations", 7);
            _allDatasets.Add("Box identification", boxIdenti);
            Dictionary<String, Int16> cloudAmount = new Dictionary<String, Int16>();
            cloudAmount.Add("Mean cloud amount", 8);//8
            cloudAmount.Add("Mean IR-marginal cloud amount", 9);
            cloudAmount.Add("Frequency of mean cloud amount = 0-10%", 10);
            cloudAmount.Add("Frequency of mean cloud amount = 10-20%", 11);
            cloudAmount.Add("Frequency of mean cloud amount = 20-30%", 12);
            cloudAmount.Add("Frequency of mean cloud amount = 30-40%", 13);
            cloudAmount.Add("Frequency of mean cloud amount = 40-50%", 14);
            cloudAmount.Add("Frequency of mean cloud amount = 50-60%", 15);
            cloudAmount.Add("Frequency of mean cloud amount = 60-70%", 16);
            cloudAmount.Add("Frequency of mean cloud amount = 70-80%", 17);
            cloudAmount.Add("Frequency of mean cloud amount = 80-90%", 18);
            cloudAmount.Add("Frequency of mean cloud amount = 90-100%", 19);
            _allDatasets.Add("Cloud amount (CA) ", cloudAmount);
            Dictionary<String, Int16> meanPC = new Dictionary<String, Int16>();
            meanPC.Add("Mean cloud top pressure(PC)", 20);
            meanPC.Add("Standard deviation of spatial mean over time(PC)", 21);
            meanPC.Add("Time mean of standard deviation over space(PC)", 22);
            _allDatasets.Add("Mean cloud top pressure (PC)", meanPC);
            Dictionary<String, Int16> meanTC = new Dictionary<String, Int16>();
            meanTC.Add("Cloud temperature(TC)", 23);
            meanTC.Add("Standard deviation of spatial mean over time(TC)", 24);
            meanTC.Add("Time mean of standard deviation over space(TC)", 25);
            _allDatasets.Add("Mean cloud top temperature (TC)", meanTC);
            Dictionary<String, Int16> meanCloudTAU = new Dictionary<String, Int16>();
            meanCloudTAU.Add("Mean cloud optical thickness(TAU)", 26);//26
            meanCloudTAU.Add("Standard deviation of spatial mean over time(TAU)", 27);
            meanCloudTAU.Add("Time mean of standard deviation over space(TAU)", 28);
            _allDatasets.Add("Mean cloud optical thickness (TAU)", meanCloudTAU);
            Dictionary<String, Int16> meanCloudWP = new Dictionary<String, Int16>();
            meanCloudWP.Add("Mean cloud water path(WP)", 29);//29
            meanCloudWP.Add("Standard deviation of spatial mean over time(WP)", 30);//20
            meanCloudWP.Add("Time mean of standard deviation over space(WP)", 31);
            _allDatasets.Add("Mean cloud water path (WP)", meanCloudWP);
            Dictionary<String, Int16> IRCloudTypes = new Dictionary<String, Int16>();
            IRCloudTypes.Add("Mean CA for low-level clouds", 32);
            IRCloudTypes.Add("Mean PC for low-level clouds", 33);
            IRCloudTypes.Add("Mean TC for low-level clouds", 34);
            IRCloudTypes.Add("Mean CA for middle-level clouds", 35);
            IRCloudTypes.Add("Mean PC for middle-level clouds", 36);
            IRCloudTypes.Add("Mean TC for middle-level clouds", 37);
            IRCloudTypes.Add("Mean CA for high-level clouds", 38);
            IRCloudTypes.Add("Mean PC for high-level clouds", 39);
            IRCloudTypes.Add("Mean TC for high-level clouds", 40);
            _allDatasets.Add("IR cloud types", IRCloudTypes);
            Dictionary<String, Int16> lowCloudTypes = new Dictionary<String, Int16>();
            lowCloudTypes.Add("Mean CA for cloud type 1 = Cumulus, liquid", 41);
            lowCloudTypes.Add("Mean PC for cloud type 1 = Cumulus, liquid", 42);
            lowCloudTypes.Add("Mean TC for cloud type 1 = Cumulus, liquid", 43);
            lowCloudTypes.Add("Mean TAU for cloud type 1 = Cumulus, liquid", 44);
            lowCloudTypes.Add("Mean WP for cloud type 1 = Cumulus, liquid", 45);
            lowCloudTypes.Add("Mean CA for cloud type 2 = Stratocumulus, liquid", 46);
            lowCloudTypes.Add("Mean PC for cloud type 2 = Stratocumulus, liquid", 47);
            lowCloudTypes.Add("Mean TC for cloud type 2 = Stratocumulus, liquid", 48);
            lowCloudTypes.Add("Mean TAU for cloud type 2 = Stratocumulus, liquid", 49);
            lowCloudTypes.Add("Mean WP for cloud type 2 = Stratocumulus, liquid", 50);
            lowCloudTypes.Add("Mean CA for cloud type 3 = Stratus, liquid", 51);
            lowCloudTypes.Add("Mean PC for cloud type 3 = Stratus, liquid", 52);
            lowCloudTypes.Add("Mean TC for cloud type 3 = Stratus, liquid", 53);
            lowCloudTypes.Add("Mean TAU for cloud type 3 = Stratus, liquid", 54);
            lowCloudTypes.Add("Mean WP for cloud type 3 = Stratus, liquid", 55);
            lowCloudTypes.Add("Mean CA for cloud type 4 = Cumulus, ice", 56);
            lowCloudTypes.Add("Mean PC for cloud type 4 = Cumulus, ice", 57);
            lowCloudTypes.Add("Mean TC for cloud type 4 = Cumulus, ice", 58);
            lowCloudTypes.Add("Mean TAU for cloud type 4 = Cumulus, ice", 59);
            lowCloudTypes.Add("Mean WP for cloud type 4 = Cumulus, ice", 60);
            lowCloudTypes.Add("Mean CA for cloud type 5 = Stratocumulus, ice", 61);
            lowCloudTypes.Add("Mean PC for cloud type 5 = Stratocumulus, ice", 62);
            lowCloudTypes.Add("Mean TC for cloud type 5 = Stratocumulus, ice", 63);
            lowCloudTypes.Add("Mean TAU for cloud type 5 = Stratocumulus, ice", 64);
            lowCloudTypes.Add("Mean WP for cloud type 5 = Stratocumulus, ice", 65);
            lowCloudTypes.Add("Mean CA for cloud type 6 = Stratus, ice", 66);
            lowCloudTypes.Add("Mean PC for cloud type 6 = Stratus, ice", 67);
            lowCloudTypes.Add("Mean TC for cloud type 6 = Stratus, ice", 68);
            lowCloudTypes.Add("Mean TAU for cloud type 6 = Stratus, ice", 69);
            lowCloudTypes.Add("Mean WP for cloud type 6 = Stratus, ice", 70);
            lowCloudTypes.Add("Mean CA for cloud type 7 = Altocumulus, liquid", 71);
            lowCloudTypes.Add("Mean PC for cloud type 7 = Altocumulus, liquid", 72);
            lowCloudTypes.Add("Mean TC for cloud type 7 = Altocumulus, liquid", 73);
            lowCloudTypes.Add("Mean TAU for cloud type 7 = Altocumulus, liquid", 74);
            lowCloudTypes.Add("Mean WP for cloud type 7 = Altocumulus, liquid", 75);
            _allDatasets.Add("LOW cloud types (vis-adjusted TC)", lowCloudTypes);
            Dictionary<String, Int16> midCloudTypes = new Dictionary<String, Int16>();
            midCloudTypes.Add("Mean CA for cloud type 8 = Altostratus, liquid", 76);
            midCloudTypes.Add("Mean PC for cloud type 8 = Altostratus, liquid", 77);
            midCloudTypes.Add("Mean TC for cloud type 8 = Altostratus, liquid", 78);
            midCloudTypes.Add("Mean TAU for cloud type 8 = Altostratus, liquid", 79);
            midCloudTypes.Add("Mean WP for cloud type 8 = Altostratus, liquid", 80);
            midCloudTypes.Add("Mean CA for cloud type 9 = Nimbostratus, liquid", 81);
            midCloudTypes.Add("Mean PC for cloud type 9 = Nimbostratus, liquid", 82);
            midCloudTypes.Add("Mean TC for cloud type 9 = Nimbostratus, liquid", 83);
            midCloudTypes.Add("Mean TAU for cloud type 9 = Nimbostratus, liquid", 84);
            midCloudTypes.Add("Mean WP for cloud type 9 = Nimbostratus, liquid", 85);
            midCloudTypes.Add("Mean CA for cloud type 10 = Altocumulus, ice", 86);
            midCloudTypes.Add("Mean PC for cloud type 10 = Altocumulus, ice", 87);
            midCloudTypes.Add("Mean TC for cloud type 10 = Altocumulus, ice", 88);
            midCloudTypes.Add("Mean TAU for cloud type 10 = Altocumulus, ice", 89);
            midCloudTypes.Add("Mean WP for cloud type 10 = Altocumulus, ice", 90);
            midCloudTypes.Add("Mean CA for cloud type 11 = Altostratus, ice", 91);
            midCloudTypes.Add("Mean PC for cloud type 11 = Altostratus, ice", 92);
            midCloudTypes.Add("Mean TC for cloud type 11 = Altostratus, ice", 93);
            midCloudTypes.Add("Mean TAU for cloud type 11 = Altostratus, ice", 94);
            midCloudTypes.Add("Mean WP for cloud type 11 = Altostratus, ice", 95);
            midCloudTypes.Add("Mean CA for cloud type 12 = Nimbostratus, ice", 96);
            midCloudTypes.Add("Mean PC for cloud type 12 = Nimbostratus, ice", 97);
            midCloudTypes.Add("Mean TC for cloud type 12 = Nimbostratus, ice", 98);
            midCloudTypes.Add("Mean TAU for cloud type 12 = Nimbostratus, ice", 99);
            midCloudTypes.Add("Mean WP for cloud type 12 = Nimbostratus, ice", 100);
            _allDatasets.Add("MIDDLE cloud types (VIS-adjusted TC)", midCloudTypes);
            Dictionary<String, Int16> highCloudTypes = new Dictionary<String, Int16>();
            highCloudTypes.Add("Mean CA for cloud type 13 = Cirrus", 101);
            highCloudTypes.Add("Mean PC for cloud type 13 = Cirrus", 102);
            highCloudTypes.Add("Mean TC for cloud type 13 = Cirrus", 103);
            highCloudTypes.Add("Mean TAU for cloud type 13 = Cirrus", 104);
            highCloudTypes.Add("Mean WP for cloud type 13 = Cirrus", 105);
            highCloudTypes.Add("Mean CA for cloud type 14 = Cirrostratus", 106);
            highCloudTypes.Add("Mean PC for cloud type 14 = Cirrostratus", 107);
            highCloudTypes.Add("Mean TC for cloud type 14 = Cirrostratus", 108);
            highCloudTypes.Add("Mean TAU for cloud type 14 = Cirrostratus", 109);
            highCloudTypes.Add("Mean WP for cloud type 14 = Cirrostratus", 110);
            highCloudTypes.Add("Mean CA for cloud type 15 = Deep convective", 111);
            highCloudTypes.Add("Mean PC for cloud type 15 = Deep convective", 112);
            highCloudTypes.Add("Mean TC for cloud type 15 = Deep convective", 113);
            highCloudTypes.Add("Mean TAU for cloud type 15 = Deep convective", 114);
            highCloudTypes.Add("Mean WP for cloud type 15 = Deep convective", 115);
            _allDatasets.Add("HIGH cloud types (VIS-adjusted TC)", highCloudTypes);
            Dictionary<String, Int16> meanTS = new Dictionary<String, Int16>();
            meanTS.Add("Mean TS from clear sky composite(TS)", 116);//116
            meanTS.Add("Time mean of standard deviation over space(TS)", 117);
            _allDatasets.Add("Mean surface temperature (TS)", meanTS);
            Dictionary<String, Int16> meanRS = new Dictionary<String, Int16>();
            meanRS.Add("Mean RS from clear sky composite(RS)", 118);//118
            _allDatasets.Add("Mean surface reflectance (RS)", meanRS);
            Dictionary<String, Int16> Snow_Icecover = new Dictionary<String, Int16>();
            Snow_Icecover.Add("Mean ice snow cover", 119);//119
            _allDatasets.Add("Snow Ice cover", Snow_Icecover);
            Dictionary<String, Int16> TOVSatmosInfo = new Dictionary<String, Int16>();
            TOVSatmosInfo.Add("Mean Surface pressure (PS)", 120);
            TOVSatmosInfo.Add("Mean Near-surface air temperature (TSA)", 121);
            TOVSatmosInfo.Add("Mean Temperature at 740 mb (T)", 122);
            TOVSatmosInfo.Add("Mean Temperature at 500 mb (T)", 123);
            TOVSatmosInfo.Add("Mean Temperature at 375 mb (T)", 124);
            TOVSatmosInfo.Add("Mean Tropopause pressure (PT)", 125);
            TOVSatmosInfo.Add("Mean Tropopause temperature (TT)", 126);
            TOVSatmosInfo.Add("Mean Stratosphere temperature at 50 mb (T)", 127);
            TOVSatmosInfo.Add("Mean Precipitable water for 1000-680 mb (PW)", 128);
            TOVSatmosInfo.Add("Mean Precipitable water for 680-310 mb (PW)", 129);
            TOVSatmosInfo.Add("Mean Ozone column abundance (O3)", 130);
            _allDatasets.Add("TOVS atmospheric information", TOVSatmosInfo);
        }
        
        private void FillMyTreeView()
        {
            this.treeviewdataset.Nodes.Clear();
            this.treeviewdataset.BeginUpdate();
            foreach (string  file in _allFiles.Keys)
            {
                this.treeviewdataset.Nodes.Add(new TreeNode(Path.GetFileName(file)));
            }
            foreach (TreeNode node in treeviewdataset.Nodes)
            {
                foreach(var item  in _needDatasets.Keys)
                {
                    node.Nodes.Add(item);
                }
            }
            this.treeviewdataset.EndUpdate();
            this.treeviewdataset.CollapseAll();
        }

        private void FillAllTreeView()
        {
            this.treeviewdataset.Nodes.Clear();
            this.treeviewdataset.BeginUpdate();
            foreach (string  file in _allFiles.Keys)
            {
                this.treeviewdataset.Nodes.Add(new TreeNode(Path.GetFileName(file)));
            }
            Dictionary<string,Int16> DatasetsG=new Dictionary<string,Int16>();
            foreach (TreeNode node in treeviewdataset.Nodes)
            {
                foreach(var group  in _allDatasets.Keys)
                {
                    node.Nodes.Add(group);
                }
                foreach(TreeNode groupnode  in node.Nodes)
                {
                    foreach (var  Dataset in _allDatasets[groupnode.Text].Keys)
                    {
                        groupnode.Nodes.Add(Dataset);
                    }
                }
            }
            this.treeviewdataset.EndUpdate();
            this.treeviewdataset.CollapseAll();
        }

        #endregion

        #region 数据集选择响应

        void treeviewdataset_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;
            if (e.Node.Level== 0)
            {
                _fillcolor = false;
                CheckChangeALL(e.Node);
                return;
            }else if (e.Node.Level ==1)
            {
                _fillcolor = false;
                CheckChangeALL(e.Node);
                return;
            }
            else if (e.Node.Level == 2 )
            {
                if (e.Node.Checked && _fillcolor)
                    e.Node.Parent.BackColor = Color.Green;
                else
                    e.Node.Parent.BackColor = Color.White;
            }            
        }

        private void CheckChangeALL(TreeNode root)
        {
            bool check = root.Checked;
            foreach (TreeNode node in root.Nodes)
            {
                if (node.Level == 1)
                {
                    node.Checked = check;
                    foreach (TreeNode subnode in node.Nodes)
                    {
                        subnode.Checked = check;
                    }
                }
                if (node.Level==2)
                {
                    node.Checked = check;
                }                    
            }
            treeviewdataset.Refresh();
        }

        void treeviewdataset_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        #endregion

        #region 范围设置
        private bool AddRegion()
        {
            double minlon = doubleTextBox1.Value;
            double maxlon = doubleTextBox2.Value;
            double minlat = doubleTextBox3.Value;
            double maxlat = doubleTextBox4.Value;
            CoordEnvelope env = new CoordEnvelope(minlon, maxlon, minlat, maxlat);
            if (env.Width <= 0 || env.Height <= 0
                || env.MinX < -180 || env.MaxX > 180
                || env.MinY < -90 || env.MaxY > 90)
            {
                errorProvider1.SetError(panel2, "经纬度值超出有效值");
                return false;
            }
            if (env.MinX == env.MaxX || env.MinY == env.MaxY)
            {
                errorProvider1.SetError(panel2, "经纬度范围不合法");
                return false;
            }
            _env = env;
            return true;
        }

        #endregion

        #region 数据集输出LDF
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtInDir.Text==null)
            {
                errorProvider1.SetError(txtInDir, "请选择输入文件");
                return;
            }
            if (txtOutDir.Text == null)
            {
                errorProvider1.SetError(txtOutDir, "请设置输出目录");
                return;
            }
            if (!AddRegion() || _env == null)

            if (!AddRegion() || _env == null)
            {
                errorProvider1.SetError(panel2, "请设置经纬度范围");
                return;
            }
            if (!TryProcessfiles())//选择数据集判断
            {
                errorProvider1.SetError(panel1, "请选择数据集");
                return;
            }
            this.Activate();
            MessageBox.Show("数据处理完成！");
        }

        private bool TryProcessfiles()
        {
            bool processd = false;
            Dictionary<string, Int16> checkbands = new Dictionary<string, Int16>();
            foreach (TreeNode node in treeviewdataset.Nodes)
            {
                foreach (TreeNode groupnode in node.Nodes)
                {
                    checkbands.Clear();
                    foreach (TreeNode subnode in groupnode.Nodes)
                    {
                        if (subnode.Checked)//选中
                        {
                            checkbands.Add(subnode.Text, _allDatasets[groupnode.Text][subnode.Text]);
                        }
                    }
                    if (checkbands.Count != 0)
                    {
                        processd = true;
                        ProcessGPC2ldf(node.Text, checkbands);
                    }
                }
            }
            return processd;
        }

        private unsafe void ProcessGPC2ldf(string filename,Dictionary <string,Int16> checkbands)
        {
            string orgName = "ISCCP", prdslevel = "D2", halfYearCode, regionCode = "GLOBAL", year="9999", month="99", day = "99", UTCtime="XXXX";
            string[] args = filename.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length==9)
            {
                orgName = args[0];
                prdslevel = args[1];
                halfYearCode = args[2];
                regionCode = args[3];
                year = args[4];
                month = args[5];
                day = args[6];
                UTCtime = args[7]; 
            }
            //TryParseFnames(filename,out args );
            float[] buffer = null;
            string bandfname;
            int width, height, xOffset, yOffset;
            using (IRasterDataProvider dataprd = GeoDataDriver.Open(_allFiles[filename]) as IRasterDataProvider)
            {
                _resX = dataprd.ResolutionX;
                _resY = dataprd.ResolutionY;
                width = (int)(_env.Width / _resX);
                height = (int)(_env.Height / _resY);
                xOffset = (int)((_env.MinX + 180) / _resX);
                yOffset = (int)((90 - _env.MaxY) / _resY);
                enumDataType dataType = dataprd.DataType;
                _spatialRef = dataprd.SpatialRef;
                string[] options = CreateLDFOptions(_spatialRef);
                string outdir = txtOutDir.Text;//_allFiles[filename].Substring(0, _allFiles[filename].LastIndexOf("."));
                foreach (string bname in checkbands.Keys)
                {
                    bandfname = Path.Combine(outdir, "L2ExtrData" + "\\" + orgName + "_" + prdslevel + "\\" + year + "\\" + month + "\\" + UTCtime, "band" +"_"+ checkbands[bname] + "_" + bname + ".LDF");
                    IRasterBand band = dataprd.GetRasterBand(checkbands[bname]);
                    buffer = new float[width * height];
                    fixed (float* ptr = buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        band.Read(xOffset, yOffset, width, height, bufferPtr, dataType, width, height);
                    }
                    TryCreateLDFFile(bandfname, 1, width, height, buffer, options, dataType);
                }
            }
        }

        private void TryParseFnames(string fname, out string[] args)
        {
            args = fname.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

        }
        private string[] CreateLDFOptions(ISpatialReference spatialRef)
        {
            string bandNames = "";
            string[] options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + spatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + _env.MinX + "," + _env.MaxY + "}:{" + _resX + "," + _resY + "}",
                            "BANDNAMES="+ bandNames
                        };
            return options;
        }

        private void TryCreateLDFFile(string bandfname, int bandNO, int xSize, int ySize, float[] buffer, string[] options, enumDataType dataType)
        {
            string dstDir  =Path.GetDirectoryName(bandfname);
            if (!Directory.Exists(dstDir))
            {
                Directory.CreateDirectory(dstDir);
            }
            if (File.Exists(bandfname))
            {
                File.Delete(bandfname);
            }
            IRasterDataDriver driver = RasterDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            using (IRasterDataProvider bandRaster = driver.Create(bandfname, xSize, ySize, 1, dataType, options) as IRasterDataProvider)
            {
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                try
                {
                    bandRaster.GetRasterBand(1).Write(0, 0, xSize, ySize, handle.AddrOfPinnedObject(), enumDataType.Float, xSize, ySize);
                }
                finally
                {
                    handle.Free();
                }
            }
        }
        #endregion


    }
}
