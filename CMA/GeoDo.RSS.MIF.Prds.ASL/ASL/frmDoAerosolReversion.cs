using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Prds.ASL
{
    public enum SatelliteID
    {
        Null,
        Terra,
        Aqua
    }

    public partial class frmDoAerosolReversion : Form
    {
        List<string> _modisDataFnames;
        string _keywords = string.Empty;
        SatelliteID _satelliteID =SatelliteID.Null;
        DateTime _datatime = DateTime.MinValue;
        string _MODISdataDir = null;
        string _MODISAncidataDir = null;
        string _ProductOutDir = null;
        const float SolZen_Threshold = 80.0f;//太阳天顶角阈值，大于该值属于太阳耀斑SunGlint
        const double Rel_Equality_Eps = 0.000001, Zero_Eps = 0.000001;
        const float FV_GEO = -999.0f;//默认填充值
        const float FV_L1B = -999.0f;//默认填充值
        const double DTR = Math.PI / 180;//角度到弧度转换
        const Double RTD = 180 / Math.PI;
        const double GLINT_THRESHOLD = 40.0;
      
        public frmDoAerosolReversion()
        {
            InitializeComponent();
            _modisDataFnames = new List<string>();
        }

        private void btnMODISDataDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dia = new FolderBrowserDialog())
            {
                if (DialogResult.OK == dia.ShowDialog())
                {
                    tbMODISDataDir.Text = dia.SelectedPath;
                    _MODISdataDir = tbMODISDataDir.Text;
                }
            }
        }

        private void btnAncillaryDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dia = new FolderBrowserDialog())
            {
                if (DialogResult.OK == dia.ShowDialog())
                {
                    tbMODISAncillaryDir.Text = dia.SelectedPath;
                    _MODISAncidataDir = tbMODISAncillaryDir.Text;
                }
            }
        }

        private void btnOutputDir_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dia = new FolderBrowserDialog())
            {
                if (DialogResult.OK == dia.ShowDialog())
                {
                    tbProductOutDir.Text = dia.SelectedPath;
                    _ProductOutDir = tbProductOutDir.Text;
                }
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            _MODISdataDir = @"D:\Geodo\试验数据\MODIS";
            _MODISAncidataDir = @"D:\Geodo\试验数据\MODIS\2013_01_14_014";
            _ProductOutDir = @"D:\Geodo\试验数据\MODIS\testoutput";
            if (!CheckEnvironment())//判断各个路径是否输入正确
                return;
            if (!CheckModisDataFile())//判断气溶胶繁衍需要的4个HDF文件是否齐全
                return;
            string img1KMFilename = string.Empty;
            string hdr1KMFilename = string.Empty;
            string imgHKMFilename = string.Empty;
            string hdrHKMFilename = string.Empty;
            string imgQKMFilename = string.Empty;
            string hdrQKMFilename = string.Empty;
            string imgGEOFilename = string.Empty;
            string hdrGEOFilename = string.Empty;
            string imgMETFilename = string.Empty;
            string hdrMETFilename = string.Empty;
            string niseNorthFilename = string.Empty;
            string niseSouthFilename = string.Empty;
            string engBINFilename = string.Empty;
            string gadsBINFilename = string.Empty;
            string tovsBINFilename = string.Empty;
            string appname = string.Empty;
            int index =0;
            try
            {
                btnStart.Enabled = false;
                progressBar1.Visible = true;
                progressBar1.Maximum = 15;
                progressBar1.Value = 1;
                #region 获取辅助文件名
                string niseFilename = GetInputFilename("*.HDFEOS");
                if (niseFilename == string.Empty)
                {
                    MessageBox.Show("nise辅助文件名不正确或未找到", "信息提示", MessageBoxButtons.OK);
                    return;
                }
                string engFilename = GetInputFilename("eng.*");
                if (engFilename == string.Empty)
                {
                    MessageBox.Show("eng辅助文件名不正确或未找到", "信息提示", MessageBoxButtons.OK);
                    return;
                }
                string gadsFilename = GetInputFilename("gdas1.*");
                if (gadsFilename == string.Empty)
                {
                    MessageBox.Show("gads辅助文件名不正确或未找到", "信息提示", MessageBoxButtons.OK);
                    return;
                }
                string oisstFname = GetInputFilename("oisst.*");
                if (oisstFname == string.Empty)
                {
                    MessageBox.Show("oisst辅助文件名不正确或未找到", "信息提示", MessageBoxButtons.OK);
                    return;
                }
                string tovsFilename = GetInputFilename("TOAST16*.*");
                if (tovsFilename == string.Empty)
                {
                    MessageBox.Show("tovs辅助文件名不正确或未找到", "信息提示", MessageBoxButtons.OK);
                    return;
                }
                #endregion
                string appDir = @"D:\Geodo\smart\TFS\气象MAS二期\SMART\bin\Release\SystemData\AerosolWinExe";//
                string Dir = AppDomain.CurrentDomain.BaseDirectory;//\TEMP
                string productTempDir = @"D:\Geodo\试验数据\MODIS\temp\";//Path.Combine(appDir,"Temp\\");// tbProductOutDir.Text.Trim() + "\\";
                if (!Directory.Exists(productTempDir))
                {
                    Directory.CreateDirectory(productTempDir);
                } 
                else
                {                
                    //DelProductData(productTempDir);
                }

                #region 提取数据为binary文件
                appname =Path.Combine(appDir, "modis_extract_1km.exe");
                index = GetKeywordIndex("1KM");
                img1KMFilename = GetFileFlag(Path.GetFileNameWithoutExtension(_modisDataFnames[index])) + "." + _keywords + ".1000m.img";
                img1KMFilename = Path.Combine(productTempDir, img1KMFilename);
                hdr1KMFilename = Path.ChangeExtension(img1KMFilename,".hdr");// productTempDir + Path.GetFileNameWithoutExtension(img1KMFilename) + ".hdr";
                //RunMyProcess(appname, index, img1KMFilename, hdr1KMFilename);

                appname = Path.Combine(appDir, "modis_extract_hkm.exe");
                index = GetKeywordIndex("HKM");
                imgHKMFilename =GetFileFlag(Path.GetFileNameWithoutExtension(_modisDataFnames[index])) + "." + _keywords + ".500m.img";
                imgHKMFilename = Path.Combine(productTempDir, imgHKMFilename);
                hdrHKMFilename = Path.ChangeExtension(imgHKMFilename, ".hdr");// productTempDir + Path.GetFileNameWithoutExtension(imgHKMFilename) + ".hdr";
                //RunMyProcess(appname, index, imgHKMFilename, hdrHKMFilename);

                appname = Path.Combine(appDir, "modis_extract_qkm.exe");
                index = GetKeywordIndex("QKM");
                imgQKMFilename = GetFileFlag(Path.GetFileNameWithoutExtension(_modisDataFnames[index])) + "." + _keywords + ".250m.img";
                imgQKMFilename = Path.Combine(productTempDir, imgQKMFilename);
                hdrQKMFilename = Path.ChangeExtension(imgQKMFilename, ".hdr");// productTempDir + Path.GetFileNameWithoutExtension(imgQKMFilename) + ".hdr";
                //RunMyProcess(appname, index, imgQKMFilename, hdrQKMFilename);

                appname = Path.Combine(appDir, "modis_extract_geo.exe");
                index = GetKeywordIndex("MOD03");
                imgGEOFilename =  GetFileFlag(Path.GetFileNameWithoutExtension(_modisDataFnames[index])) + "." + _keywords + ".geo.img";
                imgGEOFilename = Path.Combine(productTempDir, imgGEOFilename);
                hdrGEOFilename = Path.ChangeExtension(imgGEOFilename, ".hdr");// productTempDir + Path.GetFileNameWithoutExtension(imgGEOFilename) + ".hdr";
                //RunMyProcess(appname, index, imgGEOFilename, hdrGEOFilename);

                appname = Path.Combine(appDir, "modis_extract_met.exe");
                index = GetKeywordIndex("1KM");
                imgMETFilename = Path.Combine(productTempDir, GetFileFlag(Path.GetFileNameWithoutExtension(_modisDataFnames[index])) + "." + _keywords + ".met.img");
                hdrMETFilename = Path.ChangeExtension(imgMETFilename, ".hdr");// productTempDir + Path.GetFileNameWithoutExtension(imgMETFilename) + ".hdr";
                //RunMyProcess(appname, index, imgMETFilename, hdrGEOFilename);

                appname = Path.Combine(appDir, "extract_nsidc_nise.bat");
                niseFilename = GetInputFilename("*.HDFEOS");
                niseNorthFilename = Path.Combine(productTempDir, Path.GetFileNameWithoutExtension(niseFilename) + "_NORTH.bin");
                niseSouthFilename = Path.Combine(productTempDir, Path.GetFileNameWithoutExtension(niseFilename) + "_SOUTH.bin");
                //RunBATProcess(new string[] { appname, niseFilename, niseNorthFilename, niseSouthFilename });
                WriteniseNorthBinHdr(niseNorthFilename);
                WriteniseNorthBinHdr(niseSouthFilename);

                appname = Path.Combine(appDir, "extract_ncep_ice.bat");
                engFilename = GetInputFilename("eng.*");
                engBINFilename = Path.Combine(productTempDir,Path.GetFileName(engFilename) + ".bin");
                //RunBATProcess(new string[] { appname, engFilename, engBINFilename });
                WriteEngBinHdr(engBINFilename);

                appname = Path.Combine(appDir, "extract_ncep_gdas1.bat");
                gadsFilename = GetInputFilename("gdas1.*");
                gadsBINFilename = Path.Combine(productTempDir , Path.GetFileNameWithoutExtension(gadsFilename) + ".bin");
                //RunBATProcess(new string[] { appname, gadsFilename, gadsBINFilename });
                //WriteGDASBinHdr(gadsBINFilename);

                appname = Path.Combine(appDir, "extract_tovs_ozone.bat");
                tovsFilename = GetInputFilename("TOAST16*.*");
                tovsBINFilename = Path.Combine(productTempDir,Path.GetFileNameWithoutExtension(tovsFilename) + ".bin");
                //RunBATProcess(new string[] { appname, tovsFilename, tovsBINFilename });  
                WriteTOVSBinHdr(tovsBINFilename);

                //string tempoisst = Path.Combine(productTempDir,Path.GetFileName(oisstFname));
                //File.Copy(oisstFname, tempoisst, true);
                //WriteOISSTBinHdr(tempoisst);
                #endregion           

                #region 将参数写入到配置文件
                //appname = Path.Combine(appDir, "SedArg.bat");
                //RunBATProcess(new string[] { appname, _keywords, 
                //                                Path.GetFileName(niseNorthFilename),
                //                                Path.GetFileName(niseSouthFilename),
                //                                Path.GetFileName(engBINFilename),
                //                                Path.GetFileName(gadsBINFilename),
                //                                Path.GetFileName(tempoisst),
                //                                Path.GetFileName(tovsBINFilename),
                //                                _satelliteID.ToString()
                //                                 });

                #endregion
                
                //云检测
                //appname = Path.Combine(appDir, "cloudmask.exe");
                //string cloudcfg = Path.Combine(productTempDir, "cloudmask.cfg");
                //RunBATProcess(new string[] { appname, cloudcfg, _satelliteID.ToString() });

                //气溶胶反演算法,输入"config,_satelliteID.ToString(),_month
                //string aslconfig = Path.Combine(productTempDir, "aerosol.cfg");
                //string MOD35_MASK_HDR = productTempDir + "\\" + _keywords + ".mod35.hdr";
                //string MOD35_QA_HDR = productTempDir + "\\" + _keywords + ".mod35qa.hdr";

                #region 读入L1B、GEO、cloudmask、cloudqa波段数据
                string fname = AppDomain.CurrentDomain.BaseDirectory + @"SystemData\MefConfig.xml";
                IRasterDataProvider dataPrd = GeoDataDriver.Open(imgQKMFilename) as IRasterDataProvider;
                float[,] Refl_1 = ReadFloatBand(dataPrd, 1);//.66um
                float[,] Refl_2 = ReadFloatBand(dataPrd, 2);//.87um
                dataPrd = GeoDataDriver.Open(imgHKMFilename) as IRasterDataProvider;
                float[,] Refl_3 = ReadFloatBand(dataPrd, 3);//.46um
                float[,] Refl_4 = ReadFloatBand(dataPrd, 4);//.55um
                float[,] Refl_5 = ReadFloatBand(dataPrd, 5);//1.24um
                float[,] Refl_6 = ReadFloatBand(dataPrd, 6);//1.64um
                float[,] Refl_7 = ReadFloatBand(dataPrd, 7);//2.13um
                dataPrd = GeoDataDriver.Open(img1KMFilename) as IRasterDataProvider;
                float[,] Refl_9 = ReadFloatBand(dataPrd, 9);//.44um
                float[,] Refl_12 = ReadFloatBand(dataPrd, 12);//.55um
                float[,] Refl_13 = ReadFloatBand(dataPrd, 13);//.66um
                float[,] Refl_16 = ReadFloatBand(dataPrd, 16);//.86um
                float[,] Refl_26 = ReadFloatBand(dataPrd, 26);//1.38um
                float[,] Rad_20 = ReadFloatBand(dataPrd, 20);//3.660～3.840（μm)
                float[,] Rad_31 = ReadFloatBand(dataPrd, 31);//11um,10.780～11.280（μm)
                float[,] Rad_32 = ReadFloatBand(dataPrd, 32);//11.770～12.270（μm)
                List<float[,]> L1BDataList = new List<float[,]>();
                L1BDataList.Add(Refl_1);
                L1BDataList.Add(Refl_2);
                L1BDataList.Add(Refl_3);
                L1BDataList.Add(Refl_4);
                L1BDataList.Add(Refl_5);
                L1BDataList.Add(Refl_6);
                L1BDataList.Add(Refl_7);
                L1BDataList.Add(Refl_9);
                L1BDataList.Add(Refl_12);
                L1BDataList.Add(Refl_13);
                L1BDataList.Add(Refl_16);
                L1BDataList.Add(Refl_26);
                L1BDataList.Add(Rad_20);
                L1BDataList.Add(Rad_31);
                L1BDataList.Add(Rad_32);
                dataPrd = GeoDataDriver.Open(imgGEOFilename) as IRasterDataProvider;
                //band names = {Latitude, Longitude, SensorZenith, SensorAzimuth, SolarZenith, SolarAzimuth, Elevation, LandSea}
                float[,] Lat = ReadFloatBand(dataPrd, 1);
                float[,] Lon = ReadFloatBand(dataPrd, 2);
                float[,] SatZen = ReadFloatBand(dataPrd, 3);
                float[,] SatAz = ReadFloatBand(dataPrd, 4);
                float[,] SolZen = ReadFloatBand(dataPrd, 5);
                float[,] SolAz = ReadFloatBand(dataPrd, 6);
                float[,] Height = ReadFloatBand(dataPrd, 7);
                float[,] LandSea = ReadFloatBand(dataPrd, 8);//0--浅海，1--陆地，2--海/湖岸线，3--内陆水体，6--中等深海，7--深海
                //相对方位角
                float[,] RelAz = new float[dataPrd.Height,dataPrd.Width];
                int i, j; 
                for (j = 0; j < dataPrd.Width;j++ )
                {
                    for (i = 0; i < dataPrd.Height; i++)
                    {
                        RelAz[i, j] = Math.Abs(SolAz[i, j] - SatAz[i, j]);
                    }
                }
                List<float[,]> geolist = new List<float[,]>();
                geolist.Add(Lat);
                geolist.Add(Lon);
                geolist.Add(SatZen);
                geolist.Add(SatAz);
                geolist.Add(SolZen);
                geolist.Add(SolAz);
                geolist.Add(Height);
                geolist.Add(LandSea);
                geolist.Add(RelAz);
                dataPrd = GeoDataDriver.Open(imgMETFilename) as IRasterDataProvider;
                //band names = {scan type, mirror side},203行×1列
                byte[, ,] METdata = ReadByteBand(dataPrd, dataPrd.BandCount);

                string mod35name = _keywords + ".mod35.img";
                string mod35Fname = Path.Combine(productTempDir, mod35name);
                dataPrd = GeoDataDriver.Open(mod35Fname) as IRasterDataProvider;
                int Buf_cldmsk = 6;//云掩膜文件有6个bytes,文件中为6个波段
                byte[, ,] Cloud = ReadByteBand(dataPrd, Buf_cldmsk);//云监测6个波段的byte数据

                string mod35qaname = _keywords + ".mod35qa.img";
                string mod35qaFname = Path.Combine(productTempDir, mod35qaname);
                dataPrd = GeoDataDriver.Open(mod35qaFname) as IRasterDataProvider;
                int Buf_cldmsk_QA=10;
                byte[, ,] QA_Cloud = ReadByteBand(dataPrd, Buf_cldmsk_QA);//云检测质量QA文件10个波段的byte数据
                #endregion                

                #region 读入NCEP的gdas和TOVST辅助数据,用于数据的大气校正【gdas未实现】
                //int RTN_NCEP = -1;
                //db_read_anc_data.f，读取NCEP辅助数据
                //gadsBINFilename, 
                //tovsBINFilename,
                dataPrd = GeoDataDriver.Open(tovsBINFilename) as IRasterDataProvider;
                float[,] TOVST = ReadFloatBand(dataPrd, 1);
                //DB_READ_ANC_DATA(Lon_center,Lat_center, anc_met_lun, ozone_lun, sfctmp, ugrd, vgrd, pwat, ozone);
                //读取成功返回RTN_NCEP=1
                #endregion                

                # region aerosol.f 气溶胶反演主程序
                int month = _datatime.Month;
                string platform_name = _satelliteID.ToString();
                //从数据中读取每个5分钟段中的总扫描行number of scans
                int nscans = 203;//一般为203
                int ILINE = 10;//行
                int ISWATH = 1354;//列ISWATH=1500，应从L1B_1KM中得到Max Earth View Frames或EV_Frames
                //db_mod04_chk_input.f 判断输入
                    //从L1B_1KM数据读取行列数计算输出MOD04的out_lines_10km，out_elements_10km
                #region 每个Scans的10行×1354列，共NUMSQ个10km * 10km反演单元的结果
                int NUMSQ = (ILINE / 10) * (ISWATH / 10); //NUMSQ为每个Iscan所有列包含的bin个数
                Int16[] SDS_Tau_Land_Ocean = new Int16[NUMSQ];//默认值NUMCELLS=150;
                Int16[] SDS_Tau_Land_Ocean_img = new Int16[NUMSQ];
                int NWAV_S = 7;//_S表示for Sea
                Int16[,] SDSTAU_average = new Int16[NUMSQ, NWAV_S];
                Int16[,] SDSTAUS_average = new Int16[NUMSQ, NWAV_S];
                Int16[,] SDS_RefF_average = new Int16[NUMSQ, NWAV_S];
                Int16[] SDS_ratio_small_Land_Ocean = new Int16[NUMSQ];
                Int16[] SDS_Reflected_flux_Land_Ocean = new Int16[NUMSQ];
                Int16[] SDS_CLDFRC_land = new Int16[NUMSQ];
                Int16[] SDS_dust_weighting = new Int16[NUMSQ];
                Int16[] SDS_CLDFRC_OCEAN = new Int16[NUMSQ];

                int Land_Sol2=3;//陆地气溶胶反演中用到的波段数
                Int16[,] SDS_RefF_land = new Int16[NUMSQ, Land_Sol2];
                #endregion

                #region 10km * 10km bin 的L1B数据反射率及发射率
                float[,] Refl_1_cube = new float[4 * ILINE, 4 * ISWATH];
                float[,] Refl_2_cube = new float[4 * ILINE, 4 * ISWATH];
                float[,] Refl_3_cube = new float[2 * ILINE, 2 * ISWATH];
                float[,] Refl_4_cube = new float[2 * ILINE, 2 * ISWATH];
                float[,] Refl_5_cube = new float[2 * ILINE, 2 * ISWATH];
                float[,] Refl_6_cube = new float[2 * ILINE, 2 * ISWATH];
                float[,] Refl_7_cube = new float[2 * ILINE, 2 * ISWATH];
                float[,] Refl_9_cube = new float[ILINE, ISWATH];
                float[,] Refl_26_cube = new float[ILINE, ISWATH];
                float[,] Refl_12_cube = new float[ILINE, ISWATH];
                float[,] Refl_13_cube = new float[ILINE, ISWATH];
                float[,] Refl_16_cube = new float[ILINE, ISWATH];
                float[,] Rad_20_cube = new float[ILINE, ISWATH];
                float[,] Rad_31_cube = new float[ILINE, ISWATH];
                float[,] Rad_32_cube = new float[ILINE, ISWATH];
                #endregion                

                #region  NUMSQ个10km * 10km bin的云检测文件中各个flag,
                bool[,] DET_Flag = new bool[ILINE, ISWATH];
                int[,] UFQ_Flag = new int[ILINE, ISWATH];
                bool[,] DayNight_Flag = new bool[ILINE, ISWATH];
                bool[,] SunGlint_Flag = new bool[ILINE, ISWATH];
                bool[,] SnowIce_Flag = new bool[ILINE, ISWATH];
                int[,] LandSea_Flag = new int[ILINE, ISWATH];
                bool[,] Non_CloudOb_Flag = new bool[ILINE, ISWATH];
                bool[,] Thin_CirNIR_Flag = new bool[ILINE, ISWATH];
                bool[,] Shadow_Flag = new bool[ILINE, ISWATH];
                bool[,] Thin_CirIR_Flag = new bool[ILINE, ISWATH];
                bool[,] Cloud_SimpIR_Flag = new bool[ILINE, ISWATH];
                bool[,] High_Cloud_Flag = new bool[ILINE, ISWATH];
                bool[,] Cloud_IRTemp_Flag = new bool[ILINE, ISWATH];
                bool[,] Cloud_3p75_11_Flag = new bool[ILINE, ISWATH];
                bool[,] Cloud_VisRat_Flag = new bool[ILINE, ISWATH];
                bool[,] Cloud_SpatVar_Flag = new bool[ILINE, ISWATH];
                #endregion
                #region 从MOD35各个flag中导出的云掩膜
                bool[,] CldMsk_250 = new bool[4 * ILINE, 4 * ISWATH];//由db_CldMsk_Info_MOD04&CldMsk_Land返回,Cloud Mask (250m resolution from 1km resolution)
                bool[,] CldMsk_500 = new bool[2 * ILINE, 2 * ISWATH];//由db_CldMsk_Info_MOD04&CldMsk_Land返回
                bool[,] CldMsk_1km = new bool[ILINE, ISWATH];//由db_CldMsk_Info_MOD04&CldMsk_Land返回
                int[,] Land_CLDMSK_forfraction = new int[ILINE, ISWATH];//由CldMsk_Land返回
                //* SnowMsk_Ratio(ISWATH,ILINE),
                //* SnowMsk_500m(2*ISWATH,2*ILINE),
                //* High_Cloud_Flag_500(2*ISWATH,2*ILINE),
                #endregion

                int scan_flag=-1, mirror_side=-1;//met文件中的白天/黑夜、扫描方向标识
                int Set_Counter_Ocean = 0, Set_Counter_Land = 0;
                int Set_Counter_Ocean_cloud = 0;
                int Quality_flag_forJoint = 0, NO_Ret_Land = 0, index_wave, QCONTROL_land_wav1, QCONTROL_land_wav2;
                double aa, SCALE1 = 1.0, SCALE2 = 100.0, SCALE3 = 1000.0, SCALE4 = 10000.0, OFFSET1 = 0.0, OFFSET2 = 0.0, OFFSET3 = 0.0, OFFSET4 = 0.0;
                
                int IDATA, il1, ip1, il2, ip2, il4, ip4;//循环控制参数
                int ilstart, ilend;//bin在段数据中的起始、终止行号
                float Lon_center, Lat_center, MTHET0, MTHET, MPHI0, MPHI;//, MSCATT;//各个bin的角度信息
                float met_sfctmp, met_ugrd, met_vgrd, met_pwat, ozone;//各个bin的辅助数据文件GDAS和TOVST中的地表温度、风场速度的U/V分量，大气可降水浓度，臭氧浓度
                double cossza;
                float x_cossza;
                for (int Iscan = 0; Iscan < nscans; Iscan++)
                {
                    ilstart = Iscan * 10;//第Iscan个扫描阵列的起始行号，包括，=
                    ilend = (Iscan + 1) * 10;//第Iscan个扫描阵列的终止行号，不包括，<                    
                    scan_flag = METdata[0, Iscan,0];//db_get_swath_metadata.f,从met数据(1列×nscans行)中读取scan_flag、mirror_side
                    mirror_side = METdata[1, Iscan, 0];
                    if (scan_flag ==1)//仅对白天数据进行反演
                    {
                        #region get_mod04_data.f 提取气溶胶反演需要的LIB、角度、云掩膜数据   
                        # region (1)mod04_get_angles.f 不改变值，在循环外读取，line272-288
                        /*定位及角度数据集 float 初始值=FV_GEO,读取8个，
                         * Lat[2030*1354],Lon[2030*1354],SatZen[2030*1354],SolZen[2030*1354],SatAz[2030*1354],SolAz[2030*1354],Height[2030*1354],LandSea[2030*1354]
                         *计算1个；
                         *RelAz[2030*1354],
                         */
                        #endregion

                        # region (2)mod04_get_L1b.f  声明放在循环外
                        /*L1B数据反射率及发射率 float 初始值=FV_L1B*/
                        //太阳天顶角订正Normalize sensor radiance data to reflectance units
                        #region  L1B数据太阳天顶角订正
                        //Loop over 1-km lines and pixels in scan cube
                        for (il1 = ilstart; il1 < ilend; il1++)//1KM的 lines,行,应考虑与Iscan的关系
                        {
                            for (ip1 = 0; ip1 < ISWATH;ip1++ )//1KM的samples,列
                            {
                                cossza = Math.Cos(DTR * SolZen[il1, ip1]);
                                if (Math.Abs(cossza) < Zero_Eps)
                                    x_cossza = 0.0f;
                                else
                                    x_cossza = (float)(1.0 / cossza);

                                //Normalize 1-km bands to reflectance units 
                                if (Math.Abs((SolZen[il1, ip1] - FV_GEO) / FV_GEO) < Rel_Equality_Eps)
                                {
                                    Refl_9_cube[il1, ip1] = FV_L1B;
                                    Refl_12_cube[il1, ip1] = FV_L1B;
                                    Refl_13_cube[il1, ip1] = FV_L1B;
                                    Refl_16_cube[il1, ip1] = FV_L1B;
                                    Refl_26_cube[il1, ip1] = FV_L1B;
                                    #region  500m太阳天顶角订正
                                    for (il2 = 2 * il1; il2 < 2 * il1+2; il2++)
                                    {
                                        for (ip2 = 2 * ip1; ip2 <2*ip1+ 2; ip2++)
                                        {
                                            Refl_3_cube[il2, ip2] = FV_L1B;
                                            Refl_4_cube[il2, ip2] = FV_L1B;
                                            Refl_5_cube[il2, ip2] = FV_L1B;
                                            Refl_6_cube[il2, ip2] = FV_L1B;
                                            Refl_7_cube[il2, ip2] = FV_L1B;
                                        }
                                    }
                                    #endregion

                                    #region  250m太阳天顶角订正
                                    for (il4 = il1 * 4; il4 < il1 * 4 + 4; il4++)
                                    {
                                        for (ip4 = ip1 * 4; ip4 < ip4 * 4 + 4; ip4++)
                                        {
                                            Refl_1_cube[il4, ip4] = FV_L1B;
                                            Refl_2_cube[il4, ip4] = FV_L1B;
                                        }
                                    }
                                    #endregion
                                }
                                else if (Math.Abs(cossza) < Zero_Eps)
                                {
                                    Refl_9_cube[il1, ip1] = FV_L1B;
                                    Refl_12_cube[il1, ip1] = FV_L1B;
                                    Refl_13_cube[il1, ip1] = FV_L1B;
                                    Refl_16_cube[il1, ip1] = FV_L1B;
                                    Refl_26_cube[il1, ip1] = FV_L1B;
                                    #region  500m太阳天顶角订正
                                    for (il2 = 2 * il1; il2 < 2 * il1 + 2; il2++)
                                    {
                                        for (ip2 = 2 * ip1; ip2 < 2 * ip1 + 2; ip2++)
                                        {
                                            Refl_3_cube[il2, ip2] = FV_L1B;
                                            Refl_4_cube[il2, ip2] = FV_L1B;
                                            Refl_5_cube[il2, ip2] = FV_L1B;
                                            Refl_6_cube[il2, ip2] = FV_L1B;
                                            Refl_7_cube[il2, ip2] = FV_L1B;
                                        }
                                    }
                                    #endregion

                                    #region  250m太阳天顶角订正
                                    for (il4 = il1 * 4; il4 < il1 * 4 + 4; il4++)
                                    {
                                        for (ip4 = ip1 * 4; ip4 < ip4 * 4 + 4; ip4++)
                                        {
                                            Refl_1_cube[il4, ip4] = FV_L1B;
                                            Refl_2_cube[il4, ip4] = FV_L1B;
                                        }
                                    }
                                    #endregion
                                }
                                else if (SolZen[il1, ip1] < SolZen_Threshold)
                                {
                                    if (Math.Abs((Refl_9[il1, ip1] - FV_L1B) / FV_L1B) >Rel_Equality_Eps)
                                        Refl_9_cube[il1, ip1] = x_cossza * Refl_9[il1, ip1];
                                    if (Math.Abs((Refl_12[il1, ip1] - FV_L1B) / FV_L1B) >Rel_Equality_Eps)
                                        Refl_12_cube[il1, ip1] = x_cossza * Refl_12[il1, ip1];
                                    if (Math.Abs((Refl_13[il1, ip1] - FV_L1B) / FV_L1B) >Rel_Equality_Eps)
                                        Refl_13_cube[il1, ip1] = x_cossza * Refl_13[il1, ip1];
                                    if (Math.Abs((Refl_16[il1, ip1] - FV_L1B) / FV_L1B) >Rel_Equality_Eps)
                                        Refl_16_cube[il1, ip1] = x_cossza * Refl_16[il1, ip1];
                                    if (Math.Abs((Refl_26[il1, ip1] - FV_L1B) / FV_L1B) >Rel_Equality_Eps)
                                        Refl_26_cube[il1, ip1] = x_cossza * Refl_26[il1, ip1];
                                    #region  500m太阳天顶角订正
                                    for (il2 = 2 * il1; il2 < 2 * il1 + 2; il2++)
                                    {
                                        for (ip2 = 2 * ip1; ip2 < 2 * ip1 + 2; ip2++)
                                        {
                                            if (Math.Abs((Refl_3[il2, ip2] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                                                Refl_3_cube[il2, ip2] = x_cossza * Refl_3[il2, ip2];
                                            if (Math.Abs((Refl_4[il2, ip2] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                                                Refl_4_cube[il2, ip2] = x_cossza * Refl_4[il2, ip2];
                                            if (Math.Abs((Refl_5[il2, ip2] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                                                Refl_5_cube[il2, ip2] = x_cossza * Refl_5[il2, ip2];
                                            if (Math.Abs((Refl_6[il2, ip2] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                                                Refl_6_cube[il2, ip2] = x_cossza * Refl_6[il2, ip2];
                                            if (Math.Abs((Refl_7[il2, ip2] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                                                Refl_7_cube[il2, ip2] = x_cossza * Refl_7[il2, ip2];
                                        }
                                    }
                                    #endregion

                                    #region  250m太阳天顶角订正
                                    for (il4 = il1 * 4; il4 < il1 * 4 + 4; il4++)
                                    {
                                        for (ip4 = ip1 * 4; ip4 < ip4 * 4 + 4; ip4++)
                                        {
                                            if (Math.Abs((Refl_1[il4, ip4] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                                                Refl_1_cube[il4, ip4] = x_cossza * Refl_1[il4, ip4];
                                            if (Math.Abs((Refl_2[il4, ip4] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                                                Refl_2_cube[il4, ip4] = x_cossza * Refl_2[il4, ip4];
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    Refl_9_cube[il1, ip1] = FV_L1B;
                                    Refl_12_cube[il1, ip1] = FV_L1B;
                                    Refl_13_cube[il1, ip1] = FV_L1B;
                                    Refl_16_cube[il1, ip1] = FV_L1B;
                                    Refl_26_cube[il1, ip1] = FV_L1B;
                                    #region  500m太阳天顶角订正
                                    for (il2 = 2 * il1; il2 < 2 * il1 + 2; il2++)
                                    {
                                        for (ip2 = 2 * ip1; ip2 < 2 * ip1 + 2; ip2++)
                                        {
                                            Refl_3_cube[il2, ip2] = FV_L1B;
                                            Refl_4_cube[il2, ip2] = FV_L1B;
                                            Refl_5_cube[il2, ip2] = FV_L1B;
                                            Refl_6_cube[il2, ip2] = FV_L1B;
                                            Refl_7_cube[il2, ip2] = FV_L1B;
                                        }
                                    }
                                    #endregion

                                    #region  250m太阳天顶角订正
                                    for (il4 = il1 * 4; il4 < il1 * 4 + 4; il4++)
                                    {
                                        for (ip4 = ip1 * 4; ip4 < ip4 * 4 + 4; ip4++)
                                        {
                                            Refl_1_cube[il4, ip4] = FV_L1B;
                                            Refl_2_cube[il4, ip4] = FV_L1B;
                                        }
                                    }
                                    #endregion
                                }                                
                            }
                        }
                        #endregion

                        //Loop over 500-m lines and pixels between 1-km footprints.
                        #region  500m太阳天顶角订正
                        //int il2t1, ip2t1;
                        //for (il2 = ilstart*2; il2 <ilend*2; il2++)
                        //{
                        //    il2t1 = (int)Math.Floor(il2 / 2.0);//其在经纬度数据集中的行位置                            
                        //    for (ip2 = 0; ip2 < ISWATH*2; ip2++)
                        //    {
                        //        ip2t1 = (int)Math.Floor(ip2 / 2.0);//其在经纬度数据集中的列位置
                        //        cossza = Math.Cos(DTR * SolZen[il2t1, ip2t1]);
                        //        if (Math.Abs(cossza) < Zero_Eps)
                        //            x_cossza = 0.0f;
                        //        else
                        //            x_cossza = (float)(1.0 / cossza);
                        //        //Normalize 1-km bands to reflectance units
                        //        if (Math.Abs((SolZen[il2t1, ip2t1] - FV_GEO) / FV_GEO) < Rel_Equality_Eps)
                        //        {
                        //            Refl_3_cube[il2, ip2] = FV_L1B;
                        //            Refl_4_cube[il2, ip2] = FV_L1B;
                        //            Refl_5_cube[il2, ip2] = FV_L1B;
                        //            Refl_6_cube[il2, ip2] = FV_L1B;
                        //            Refl_7_cube[il2, ip2] = FV_L1B;
                        //        }
                        //        else if (Math.Abs(cossza) < Zero_Eps)
                        //        {
                        //            Refl_3_cube[il2, ip2] = FV_L1B;
                        //            Refl_4_cube[il2, ip2] = FV_L1B;
                        //            Refl_5_cube[il2, ip2] = FV_L1B;
                        //            Refl_6_cube[il2, ip2] = FV_L1B;
                        //            Refl_7_cube[il2, ip2] = FV_L1B;
                        //        }
                        //        else if (SolZen[il2t1, ip2t1] < SolZen_Threshold)
                        //        {
                        //            if (Math.Abs((Refl_3[il2, ip2] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                        //                Refl_3_cube[il2, ip2] = x_cossza * Refl_3[il2, ip2];
                        //            if (Math.Abs((Refl_4[il2, ip2] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                        //                Refl_4_cube[il2, ip2] = x_cossza * Refl_4[il2, ip2];
                        //            if (Math.Abs((Refl_5[il2, ip2] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                        //                Refl_5_cube[il2, ip2] = x_cossza * Refl_5[il2, ip2];
                        //            if (Math.Abs((Refl_6[il2, ip2] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                        //                Refl_6_cube[il2, ip2] = x_cossza * Refl_6[il2, ip2];
                        //            if (Math.Abs((Refl_7[il2, ip2] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                        //                Refl_7_cube[il2, ip2] = x_cossza * Refl_7[il2, ip2];
                        //        }
                        //        else
                        //        {
                        //            Refl_3_cube[il2, ip2] = FV_L1B;
                        //            Refl_4_cube[il2, ip2] = FV_L1B;
                        //            Refl_5_cube[il2, ip2] = FV_L1B;
                        //            Refl_6_cube[il2, ip2] = FV_L1B;
                        //            Refl_7_cube[il2, ip2] = FV_L1B;
                        //        }
                        //    }
                        //}
                        #endregion
                        //Loop over 250-m lines and pixels between 1-km footprints
                        #region  250m太阳天顶角订正
                        //int il4t1, ip4t1;
                        //for (il4 = ilstart*4; il4 < ilend*4; il4++)
                        //{
                        //    il4t1 = (int)Math.Floor(il4 / 4.0);//其在经纬度数据集中的行位置
                        //    for (ip4 = 0; ip4 < ISWATH*4; ip4++)
                        //    {
                        //        ip4t1 = (int)Math.Floor(ip4 / 4.0);//其在经纬度数据集中的列位置
                        //        cossza = Math.Cos(DTR * SolZen[il4t1, ip4t1]);
                        //        if (Math.Abs(cossza) < Zero_Eps)
                        //            x_cossza = 0.0f;
                        //        else
                        //            x_cossza = (float)(1.0 / cossza);

                        //        if (Math.Abs((SolZen[il4t1, ip4t1] - FV_GEO) / FV_GEO) < Rel_Equality_Eps)
                        //        {
                        //            Refl_1_cube[il4, ip4] = FV_L1B;
                        //            Refl_2_cube[il4, ip4] = FV_L1B;
                        //        }
                        //        else if (Math.Abs(cossza) < Zero_Eps)
                        //        {
                        //            Refl_1_cube[il4, ip4] = FV_L1B;
                        //            Refl_2_cube[il4, ip4] = FV_L1B;
                        //        }
                        //        else if (SolZen[il4t1, ip4t1] < SolZen_Threshold)
                        //        {
                        //            if (Math.Abs((Refl_1[il4, ip4] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                        //                Refl_1_cube[il4, ip4] = x_cossza * Refl_1[il4, ip4];
                        //            if (Math.Abs((Refl_2[il4, ip4] - FV_L1B) / FV_L1B) > Rel_Equality_Eps)
                        //                Refl_2_cube[il4, ip4] = x_cossza * Refl_2[il4, ip4];
                        //        }
                        //        else
                        //        {
                        //            Refl_3_cube[il4, ip4] = FV_L1B;
                        //            Refl_4_cube[il4, ip4] = FV_L1B;
                        //        }
                        //    }
                        //}
                        #endregion
                        #endregion
                        
                        #region (3)mod04_get_mask.f 云掩膜及QA文件的整波段读取放在循环外
                        #region bin的Cloud及QA_Cloud,注释掉
                        //byte[, ,] Cloud_cube = new byte[Buf_cldmsk, ILINE, ISWATH];//初始值为0，从Cloud的ilstart行到ilend-1行
                        //byte[, ,] QA_Cloud_cube = new byte[Buf_cldmsk_QA, ILINE, ISWATH];//初始值为0，从Cloud_QA的ilstart行到ilend-1行
                        //int no,ilc,ipc;
                        //for (no = 0; no < Buf_cldmsk;no++ )
                        //{
                        //    int i = 0;
                        //    for (ilc = ilstart; ilc<ilend;ilc++ )
                        //    {
                        //        for (ipc = 0; ipc < ISWATH;ipc++ )
                        //        {
                        //            Cloud_cube[no, i, ipc] = Cloud[no, ilc, ipc];                                    
                        //        }
                        //        i++;
                        //    }
                        //}
                        //for (no = 0; no < Buf_cldmsk_QA; no++)
                        //{
                        //    int i = 0;
                        //    for (ilc = ilstart; ilc < ilend; ilc++)
                        //    {
                        //        for (ipc = 0; ipc < ISWATH; ipc++)
                        //        {
                        //            QA_Cloud_cube[no, i, ipc] = QA_Cloud[no, ilc, ipc];
                        //        }
                        //        i++;
                        //    }
                        //}

                        #endregion                        
                         #endregion

                        # region(4)db_CldMsk_Info_MOD04.f,按位读取云监测结果，并矩阵返回， 没有用到Cloud mask QA。
                         #region 云掩膜数据集mod35中的各个flag 
                        /*
                         * QA_Cloud(Buf_cldmsk_QA,2030,1354) !Buf_cldmsk_QA=10
                         * Cloud(Buf_cldmsk,2030,1354), !Buf_cldmsk=6
                        * DET_Flag(ISWATH,ILINE),
                        * UFQ_Flag(ISWATH,ILINE),
                        * DayNight_Flag(ISWATH,ILINE),
                        * SunGlint_Flag(ISWATH,ILINE),
                        * SnowIce_Flag(ISWATH,ILINE),
                        * LandSea_Flag(ISWATH,ILINE),
                        * Non_CloudOb_Flag(ISWATH,ILINE),
                        * Thin_CirNIR_Flag(ISWATH,ILINE),
                        * Shadow_Flag(ISWATH,ILINE),
                        * Thin_CirIR_Flag(ISWATH,ILINE),
                        * Cloud_SimpIR_Flag(ISWATH,ILINE),
                        * High_Cloud_Flag(ISWATH,ILINE),
                        * Cloud_IRTemp_Flag(ISWATH,ILINE),
                        * Cloud_3p75_11_Flag(ISWATH,ILINE),
                        * Cloud_VisRat_Flag(ISWATH,ILINE),
                        * Cloud_SpatVar_Flag(ISWATH,ILINE),
                         */
                         #endregion
                        //Get_CldMsk_bitInfo()函数;
                        byte[] myBytes = new byte[Buf_cldmsk];//Buf_cldmsk=6
                        int no, l1_cube, p1, l, p, l1 = ilstart;//ilstart为Iscans的起始行
                        # region  【80】CldMsk_1km,500m,250m的生成
                        for (l1_cube = 0; l1_cube < ILINE; l1_cube++, l1++)
                        {
                            for (p1 = 0; p1 < ISWATH; p1++)
                            {
                                for (no = 0; no < Buf_cldmsk; no++)
                                {
                                    myBytes[no] = Cloud[no, l1, p1];//各个波段的6个byte，
                                }
                                BitArray bits = new BitArray(myBytes);
                                DET_Flag[l1_cube,p1] =bits[0];//云判识算法是否运行,0=未运行，1=运行
                                UFQ_Flag[l1_cube, p1] = Bits2Int(bits, 1, 2);//Unobstructed FOV Quality Flag，像元清晰置信标识，0=有云，1=不确定，2=很可能清晰，3=清晰
                                DayNight_Flag[l1_cube, p1] = bits[3];//Day/Night Flag，0 = Night / 1 = Day 
                                SunGlint_Flag[l1_cube, p1] = bits[4];//SunGlint Flag，0=耀斑，1=非耀斑
                                SnowIce_Flag[l1_cube, p1] = bits[5];//Snow/Ice Flag，0=云雪，1=非云雪
                                LandSea_Flag[l1_cube,p1] = Bits2Int(bits, 6, 7);//Land/Sea Flag，[0 (water); 1 (coastal), 2 (desert), 3 (land)]
                                Non_CloudOb_Flag[l1_cube, p1] = true;// bits[8];//Non-Cloud obstruction (dust) Flag，像元是否有气溶胶，0为有气溶胶
                                Thin_CirNIR_Flag[l1_cube, p1] = bits[9];//Thin Cirrus detected Flag (Solar)，0=有云，1=无云
                                Shadow_Flag[l1_cube, p1] = true;// bits[10];//Shadow Flag，0=阴影，1=无阴影
                                Thin_CirIR_Flag[l1_cube, p1] = bits[11];//Thin Cirrus detected Flag (IR)，0=有云，1=无云
                                Cloud_SimpIR_Flag[l1_cube, p1] = bits[13];//Cloud Flag-Simple Threshold Test，0=有云，1=无云
                                //Cloud_CO2_Flag，bits[14],co2 threshold test,??，0=有云，1=无云
                                //Cloud_6p7_Flag,bits[15],6.7 um test？？，0=有云，1=无云
                                //High_Cloud_Flag[l1_cube, p1] = false;// bits[16];//High Cloud Flag - 1.38 Micron Test，0=有云，1=无云
                                Cloud_IRTemp_Flag[l1_cube, p1] = bits[18];//Cloud Flag - IR Temperature Difference test，0=有云，1=无云
                                Cloud_3p75_11_Flag[l1_cube, p1] = bits[19];//Cloud Flag - 3.75-11 Micron Test，0=有云，1=无云
                                Cloud_VisRat_Flag[l1_cube, p1] = bits[21];//Cloud Flag - Visible Ratio Test，0=有云，1=无云
                                Cloud_SpatVar_Flag[l1_cube, p1] = bits[27];//Cloud Flag - Spatial Variability？？25?or27?，0=有云，1=无云
                                //Cloud_VisRat_Flag[l1_cube, p1] = true;//0=有云，1=无云

                                CldMsk_1km[l1_cube, p1] = true;//0=有云，1=无云，默认无云

                                #region 250m及500m的Iscans云掩膜赋初值
                                for (l = 0; l < 4; l++)
                                {
                                    for (p = 0; p < 4; p++)
                                    {
                                        CldMsk_250[l1_cube * 4 + l, p1 * 4 + p] = true;//0=有云，1=无云，默认无云
                                    }
                                }
                                for (l = 0; l < 2; l++)
                                {
                                    for (p = 0; p < 2; p++)
                                    {
                                        CldMsk_500[l1_cube * 2 + l, p1 * 2 + p] = true;//0=有云，1=无云，默认无云
                                    }
                                }
                                #endregion

                                #region High cloud flag for ocean;
                                //default set to cloudy (0: cloudy; 1: clear)if cloud mask is not determined (DET_Flag=0,false)
                                int IR_cloud = 0;//近红外通道的卷云像素个数
                                High_Cloud_Flag[l1_cube, p1] = false;
                                if (DET_Flag[l1_cube, p1]==true)//=1,determined
                                {                                    
                                    High_Cloud_Flag[l1_cube, p1] = true;//1
                                    if (Thin_CirIR_Flag[l1_cube, p1] == false)//bits 9
                                        IR_cloud++;
                                    if (Cloud_3p75_11_Flag[l1_cube, p1] == false)//bits 19
                                        IR_cloud++;
                                    if (Cloud_IRTemp_Flag[l1_cube, p1] == false)//bits 18
                                        IR_cloud++;
                                    if (IR_cloud>=1)
                                        High_Cloud_Flag[l1_cube, p1] = false;//只要有多于1个flag标识为0，有云，则高云标识为有云。
                                }
                                #endregion                                 

                                if (DET_Flag[l1_cube, p1] == false)//If not determined(DET_Flag=0,NOT processed), set cloud mask to be cloudy (=0)
                                {
                                    LandSea_Flag[l1_cube, p1] = -1;
                                    CldMsk_1km[l1_cube, p1] = false;
                                    Cloud_VisRat_Flag[l1_cube, p1] = false;
                                    #region 250m及500m的bin云掩膜赋值
                                    //当云检测未运行时，1km、500m、250米的云掩膜文件中的相应像素为false，有云
                                    for (l = 0; l < 4; l++)
                                    {
                                        for (p = 0; p < 4; p++)
                                        {
                                            CldMsk_250[l1_cube * 4 + l, p1 * 4 + p] = false;
                                        }
                                    }
                                    for (l = 0; l < 2; l++)
                                    {
                                        for (p = 0; p < 2; p++)
                                        {
                                            CldMsk_500[l1_cube * 2 + l, p1 * 2 + p] = false;
                                        }
                                    }
                                    #endregion
                                }
                                else
                                {
                                    IR_cloud = 0;
                                    if (UFQ_Flag[l1_cube, p1] == 1 || UFQ_Flag[l1_cube, p1] == 0)// indicating cloudy pixel，0 = cloudy ，01 = uncertain 
                                    {
                                        CldMsk_1km[l1_cube, p1] = false;
                                        Cloud_VisRat_Flag[l1_cube, p1] = false;
                                        #region 250m及500m的bin云掩膜赋值
                                        for (l = 0; l < 4; l++)
                                        {
                                            for (p = 0; p < 4; p++)
                                            {
                                                CldMsk_250[l1_cube * 4 + l, p1 * 4 + p] = false;
                                            }
                                        }
                                        for (l = 0; l < 2; l++)
                                        {
                                            for (p = 0; p < 2; p++)
                                            {
                                                CldMsk_500[l1_cube * 2 + l, p1 * 2 + p] = false;
                                            }
                                        }
                                        #endregion
                                    }
                                    else//UFQ_Flag=2,= probably clear;UFQ_Flag=3, clear;
                                    {
                                        if (Thin_CirIR_Flag[l1_cube, p1] == false)//bits 9
                                            IR_cloud++;
                                        if (Cloud_SimpIR_Flag[l1_cube, p1] == false)//bits 13
                                            IR_cloud++;
                                        if (Cloud_3p75_11_Flag[l1_cube, p1] == false)//bits 19
                                            IR_cloud++;
                                        if (Cloud_IRTemp_Flag[l1_cube, p1] == false)//bits 18
                                            IR_cloud++;
                                        if (IR_cloud >= 2)//只要有多于2个flag标识为0，有云，则云掩膜标识为有云。
                                        {
                                            CldMsk_1km[l1_cube, p1] = false;
                                            #region 250m及500m的bin云掩膜赋值
                                            for (l = 0; l < 4; l++)
                                            {
                                                for (p = 0; p < 4; p++)
                                                {
                                                    CldMsk_250[l1_cube * 4 + l, p1 * 4 + p] = false;
                                                }
                                            }
                                            for (l = 0; l < 2; l++)
                                            {
                                                for (p = 0; p < 2; p++)
                                                {
                                                    CldMsk_500[l1_cube * 2 + l, p1 * 2 + p] = false;
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                    if (High_Cloud_Flag[l1_cube, p1] == true)
                                    {
                                        if (LandSea_Flag[l1_cube, p1] != 0 && Non_CloudOb_Flag[l1_cube, p1] == false)//LandSea_Flag=0,water;Non_CloudOb_Flag=0,表示有气溶胶存在
                                        {
                                            CldMsk_1km[l1_cube, p1] = true;
                                            #region 250m及500m的bin云掩膜赋值
                                            for (l = 0; l < 4; l++)
                                            {
                                                for (p = 0; p < 4; p++)
                                                {
                                                    CldMsk_250[l1_cube * 4 + l, p1 * 4 + p] = true;
                                                }
                                            }
                                            for (l = 0; l < 2; l++)
                                            {
                                                for (p = 0; p < 2; p++)
                                                {
                                                    CldMsk_500[l1_cube * 2 + l, p1 * 2 + p] = true;
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        #region 【888】3 x 3 windhowing
                        int l11,p11;
                        for (l1_cube = 0; l1_cube < ILINE; l1_cube++)
                        {
                            for (p1 = 0; p1 < ISWATH; p1++)
                            {
                                if (Cloud_VisRat_Flag[l1_cube,p1]==false)//bits 21,
                                {//只要该点有云，其云掩膜中的3×3邻域全置为0
                                    if (l1_cube > 0 && l1_cube < ILINE-1)//不在边缘
                                    {
                                        if (p1 > 0 && p1 < ISWATH - 1)//不在边缘
                                        {
                                            for (l11 = l1_cube - 1; l11 <= l1_cube + 1;l11++ )
                                            {
                                                for (p11 = p1 - 1; p11 <= p1 + 1;p11++ )
                                                {
                                                    CldMsk_1km[l11, p11] = false;
                                                    CldMsk_1km[l1, p1] = false;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        for (l1_cube = 0; l1_cube < ILINE; l1_cube++)
                        {
                            for (p1 = 0; p1 < ISWATH; p1++)
                            {
                                if (CldMsk_1km[l1, p1] == false)
                                {
                                    #region 250m及500m的bin云掩膜赋值
                                    for (l = 0; l < 4; l++)
                                    {
                                        for (p = 0; p < 4; p++)
                                        {
                                            CldMsk_250[l1_cube * 4 + l, p1 * 4 + p] = false;
                                        }
                                    }
                                    for (l = 0; l < 2; l++)
                                    {
                                        for (p = 0; p < 2; p++)
                                        {
                                            CldMsk_500[l1_cube * 2 + l, p1 * 2 + p] = false;
                                        }
                                    }
                                    #endregion
                                }
                                if (Cloud_SpatVar_Flag[l1_cube, p1] == false)
                                {
                                    if (l1_cube > 0 && l1_cube < ILINE - 1)
                                    {
                                        if (p1 > 0 && p1 < ISWATH - 1)
                                        {
                                            for (l11 = l1_cube - 1; l11 <= l1_cube + 1; l11++)
                                            {
                                                for (p11 = p1 - 1; p11 <= p1 + 1; p11++)
                                                {
                                                    CldMsk_1km[l11, p11] = true;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        for (l1_cube = 0; l1_cube < ILINE; l1_cube++)
                        {
                            for (p1 = 0; p1 < ISWATH; p1++)
                            {
                                if (CldMsk_1km[l1, p1] == true)
                                {
                                    #region 250m及500m的bin云掩膜赋值
                                    for (l = 0; l < 4; l++)
                                    {
                                        for (p = 0; p < 4; p++)
                                        {
                                            CldMsk_250[l1_cube * 4 + l, p1 * 4 + p] = true;
                                        }
                                    }
                                    for (l = 0; l < 2; l++)
                                    {
                                        for (p = 0; p < 2; p++)
                                        {
                                            CldMsk_500[l1_cube * 2 + l, p1 * 2 + p] = true;
                                        }
                                    }
                                    #endregion
                                }
                            }
                        }
                        #endregion
                        #endregion

                        #endregion

                        # region //CldMsk_Land函数
                        /*Generate cloud mask over land using spatial variability of 0.47 (>0.01) and 1.38 um (> 0.007) reflectance as well 
                        as absolute  value of 1.38 um > 0.1
                         * 输入：ISWATH，ILINE，Refl_3，Refl_26
                         * 输出：CLDMSK_1KM，CldMsk_500，CldMsk_250
                         */
                        CldMsk_Land(ILINE, ISWATH, Refl_3, Refl_26, out CldMsk_1km, out CldMsk_500,out CldMsk_250,out Land_CLDMSK_forfraction);                                        
                        # endregion

                        # region 循环，【9000 IDATA = 1,NUMSQ】
                        for (IDATA = 0; IDATA < NUMSQ;IDATA++ )//对于每一个bin分别进行反演和计算
                        {
                            SDS_Tau_Land_Ocean[IDATA] = -9999;
                            SDS_Tau_Land_Ocean_img[IDATA] = -9999;
                            //SET_INDEX函数，Call to SET_INDEX sets the indexing for Modis Channels                            
                            int[] idex;//每个bin的起始列号
                            SET_INDEX( IDATA,out idex);
                            int START_1KM=idex[0], END_1KM=idex[1], START_500=idex[2], END_500=idex[3], START_250=idex[4], END_250=idex[5];
                            //GEOLOC_ANGLE函数，计算10* 10 bin中心的地理位置及角度
                            //Lon_center,Lat_center,MTHET0,MTHET,MPHI0,MPHI,MDPHI,MSCATT,MHGHT
                            float [] geoangle=GEOLOC_ANGLE(Iscan, START_1KM, Lat, Lon, SatZen, SatAz, SolZen, SolAz, Height);
                            Lon_center =geoangle[0];
                            Lat_center = geoangle[1];
                            MTHET0 = geoangle[2];
                            MTHET = geoangle[3];
                            MPHI0 = geoangle[4];
                            MPHI = geoangle[5];
                            //MSCATT = geoangle[6];
                            double total_o3 = 0, total_h2o = 0;
                            #region 采用辅助文件的O3进行大气矫正
                            int TOVSTline =(int)Math.Ceiling(90.0-Lat_center);
                            if (TOVSTline==0)
                            {
                                TOVSTline = 1;
                            }
                            int TOVSTcolumn = (Lon_center >= 0) ? (int)Math.Ceiling(Lon_center) : (int)Math.Ceiling(360 + Lon_center);
                            if (TOVSTcolumn==0)
                            {
                                TOVSTcolumn = 360;
                            }
                            //total_o3 =TOVST[TOVSTline,TOVSTcolumn];//需要好搞清楚TOAST16_130114文件的行列号与经纬度的对应关系，原点位于(0E,90N),向西，向南为正
                            //TRANS_2WAY_NECP(L1BDataList, LandSea_Flag, Iscan, idex, MTHET, MTHET0, total_h2o, total_o3);
                            #endregion

                            //采用固定参数,反射率数据的大气（水汽、臭氧、二氧化碳）校正                            
                            total_o3 = 0; total_h2o = 0;
                            TRANS_2WAY_NECP(L1BDataList, LandSea_Flag, Iscan, idex, MTHET, MTHET0, 0, 0);
                            #region  确定陆地、水体，云，太阳耀斑，雪/冰像素的个数
                            //LAND_WATER_CLOUD函数，
                            /*
                            This subroutine counts water, land, cloudy, glint and snow/ice pixels in a 10x10 km area
                            (1) If 100% water pixels go to water process,otherwise goes to land.
                            (2) If 100% cloudy pixels, no aerosol retrieval is done.
                            (3) Glint and snow/ice pixels will be set to as cloudy pixel to prohibit any retrievals.
                             * */
                            int[] QA_Flag_Ocean;
                            int waterCount = -1, landCount = -1;
                            int QA_Temp = LAND_WATER_CLOUD(idex, CldMsk_1km, Land_CLDMSK_forfraction, DET_Flag, SunGlint_Flag, SnowIce_Flag, LandSea_Flag, Shadow_Flag, High_Cloud_Flag, out QA_Flag_Ocean,out waterCount,out landCount);
                            #endregion

                            #region 设置输出文件中10*10 bin的地理位置及角度
                            //计算输出的HDF文件中的各个数据集的数值
                            float MSCATT=0;
                            POPULATE_COMMON_ARRAYS(IDATA,Lat_center,Lon_center,MTHET0,MTHET,MPHI0,MPHI,MSCATT,QA_Temp);
                            #endregion

                            #region  Restore cloud fraction in QA_Flag_Land to pass to Process_Land
                            //COMPUTE_GLINTANGLE函数
                            float MDPHI = CalMDPHI(MPHI0,MPHI);
                            double GLINTANGLE = COMPUTE_GLINTANGLE(MTHET0, MTHET, MDPHI, out QA_Flag_Ocean[8]);
                            #endregion                           

                            # region 进行反演
                            /*
                             * If all pixels of 10*10 bin are water, then processing ocean algorithm, 
                             * else  processing land algorithm; if the number
                             * of cloudy pixels >90 then reject any processing
                             */
                            int Qcontrol_special_land = 0;
                            int cloud_num = 0, Qcontrol = -1;
                            if (waterCount>=100 ||landCount >0)
                            {
                                if (waterCount >= 100)//processing ocean algorithm
                                {
                                    Set_Counter_Ocean=Set_Counter_Ocean+1;
                                    Set_Counter_Ocean_cloud = Set_Counter_Ocean_cloud + 1;
                                    //PROCESS_ocean函数

                                    //Total_retrieval_ocean函数，计算反演成功失败的像素数

                                    //Fill small weighting for ocean from average value
                                    if (SDSTAU_average[IDATA, 2]>0)
                                    {
                                        aa = ((SDSTAUS_average[IDATA, 2] / (SCALE3 + OFFSET3)) / (SDSTAU_average[IDATA, 2] / SCALE3 + OFFSET3));
                                        SDS_ratio_small_Land_Ocean[IDATA] = (Int16)(aa * SCALE3 + OFFSET3);
                                    } 
                                    else
                                    {
                                        SDS_ratio_small_Land_Ocean[IDATA] = SDSTAU_average[IDATA, 2];
                                    }
                                    SDS_Reflected_flux_Land_Ocean[IDATA] = SDS_RefF_average[IDATA, 2];
                                    //POPULATE_TAU_LAND_OCEAN函数，SDS_Tau_Land_Ocean_img

                                    //POPULATE_TAU_LAND_OCEAN函数，SDS_Tau_Land_Ocean

                                    //Filled with Fill_Value for land
                                    SDS_CLDFRC_land[IDATA] = -99;
                                    //FILLVALUE_LAND函数

                                    NO_Ret_Land = NO_Ret_Land + 1;
                                    index_wave = 3;
                                    QCONTROL_land_wav1 = 0;
                                    QCONTROL_land_wav2 = 0;
                                    //Fill_QAflag_CritRef_land函数

                                    //FILLVALUE_LAND_extra函数
                                    
                                }
                                else if (landCount > 0)//processing land algorithm
                                {
                                    int quality_land = 1;
                                    Set_Counter_Land = Set_Counter_Land + 1;
                                    if (waterCount > 0)
                                        quality_land = 0;
                                    //PROCESS_Land函数

                                    Quality_flag_forJoint = 1;//返回Quality_flag_forJoint,此处为假设值
                                    //Fill small weighting for land
                                    SDS_ratio_small_Land_Ocean[IDATA] = SDS_dust_weighting[IDATA];
                                    SDS_Reflected_flux_Land_Ocean[IDATA] = SDS_RefF_average[IDATA, 2];
                                    //POPULATE_TAU_LAND_OCEAN函数
                                    POPULATE_TAU_LAND_OCEAN(IDATA);
                                    if (Quality_flag_forJoint == 1 || Quality_flag_forJoint == 2 || Quality_flag_forJoint==3)
                                    {
                                        //POPULATE_TAU_LAND_OCEAN函数
                                    }
                                    //Filled with Fill_Value for land
                                    Qcontrol = -7;
                                    //Fill_QAflag_ocean函数

                                    SDS_CLDFRC_OCEAN[IDATA] = -99;
                                    //FILLVALUE_Ocean函数
                                    FILLVALUE_Ocean();
                                }
                            } 
                            else
                            {
                                NO_Ret_Land = NO_Ret_Land + 1;
                                SDS_CLDFRC_land[IDATA] = -99;
                                //FILLVALUE_LAND()函数
                                index_wave = 3;
                                QCONTROL_land_wav1 = 0;
                                QCONTROL_land_wav2 = 0;
                                //Fill_QAflag_CritRef_land函数

                                //FILLVALUE_LAND_extra函数

                                //Fill small weighting for land
                                SDS_ratio_small_Land_Ocean[IDATA] = SDS_dust_weighting[IDATA];
                                SDS_Reflected_flux_Land_Ocean[IDATA] = SDS_RefF_land[IDATA,2];
                                //POPULATE_TAU_LAND_OCEAN函数

                                if (Quality_flag_forJoint == 1 || Quality_flag_forJoint == 2 || Quality_flag_forJoint==3)
                                    ;//POPULATE_TAU_LAND_OCEAN函数
                                Qcontrol = -2;
                                //Fill_QAflag_ocean函数

                                SDS_CLDFRC_OCEAN[IDATA]=-99;
                                //FILLVALUE_Ocean函数

                                //Fill small weighting for ocean from average value
                                if (SDSTAU_average[IDATA, 2]>0)
                                {
                                    aa = ((SDSTAUS_average[IDATA, 2] / (SCALE3 + OFFSET3)) / (SDSTAU_average[IDATA, 2] / SCALE3 + OFFSET3));
                                    SDS_ratio_small_Land_Ocean[IDATA] = (Int16)(aa * SCALE3 + OFFSET3);
                                } 
                                else
                                {
                                    SDS_ratio_small_Land_Ocean[IDATA] = SDSTAU_average[IDATA, 2];
                                }
                                SDS_Reflected_flux_Land_Ocean[IDATA] = SDS_RefF_average[IDATA, 2]; 
                                //POPULATE_TAU_LAND_OCEAN函数，SDS_Tau_Land_Ocean_img
                                
                                //POPULATE_TAU_LAND_OCEAN函数，SDS_Tau_Land_Ocean
                                
                            }
                            #endregion

                        }
                        # endregion
                        //db_mod04_write_products.f， Call to write out MOD04 output file

                    }
                }
                //db_mod04_file_close.f 关闭文件

            #endregion 

                CopyProductDataToOutputDir(productTempDir, tbProductOutDir.Text.Trim());
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("启动应用程序时出错！原因：" + ex.Message, "信息提示", MessageBoxButtons.OK);
            }
            finally
            {
                btnStart.Enabled = true;
                progressBar1.Value = progressBar1.Maximum;
            }
        }

        #region 写入辅助文件BIN的头信息.hdr

        private void WriteniseNorthBinHdr(string Filename)
        {
            string hdrname = Path.ChangeExtension(Filename, ".hdr");
            using (StreamWriter sw = new StreamWriter(hdrname, false, Encoding.Default))
            {
                sw.WriteLine("ENVI");
                sw.WriteLine("description = {\r\nFile Imported into ENVI}");
                sw.WriteLine(string.Format("samples = {0}", 721));
                sw.WriteLine(string.Format("lines = {0}", 721));
                sw.WriteLine("bands = 1");
                sw.WriteLine("header offset = 0");
                sw.WriteLine("file type = ENVI Standard");
                sw.WriteLine(string.Format("data type = {0}", 1));
                sw.WriteLine("interleave = bsq");
                sw.WriteLine("sensor type = Unknown");
                sw.WriteLine("byte order = 0");
                sw.WriteLine("wavelength units = Unknown");
            }
        }

        private void WriteEngBinHdr(string engBINFilename)
        {
            string hdrname = Path.ChangeExtension(engBINFilename, ".hdr");
            using (StreamWriter sw = new StreamWriter(hdrname, false, Encoding.Default))
            {
                sw.WriteLine("ENVI");
                sw.WriteLine("description = {\r\nFile Imported into ENVI}");
                sw.WriteLine(string.Format("samples = {0}", 720));
                sw.WriteLine(string.Format("lines = {0}", 360));
                sw.WriteLine("bands = 1");
                sw.WriteLine("header offset = 0");
                sw.WriteLine("file type = ENVI Standard");
                sw.WriteLine(string.Format("data type = {0}", 4));
                sw.WriteLine("interleave = bsq");
                sw.WriteLine("sensor type = Unknown");
                sw.WriteLine("byte order = 0");
                sw.WriteLine("wavelength units = Unknown");
            }
        }

        private void WriteGDASBinHdr(string gadsBINFilename)
        {

        }

        private void WriteTOVSBinHdr(string tovsBINFilename)
        {
            string hdrname = Path.ChangeExtension(tovsBINFilename, ".hdr");
            using (StreamWriter sw = new StreamWriter(hdrname, false, Encoding.Default))
            {
                sw.WriteLine("ENVI");
                sw.WriteLine("description = {\r\nFile Imported into ENVI}");
                sw.WriteLine(string.Format("samples = {0}", 360));
                sw.WriteLine(string.Format("lines = {0}", 181));
                sw.WriteLine("bands = 1");
                sw.WriteLine("header offset = 0");
                sw.WriteLine("file type = ENVI Standard");
                sw.WriteLine(string.Format("data type = {0}", 4));
                sw.WriteLine("interleave = bsq");
                sw.WriteLine("sensor type = Unknown");
                sw.WriteLine("byte order = 0");
                sw.WriteLine("wavelength units = Unknown");
            }
        }

        private void WriteOISSTBinHdr(string OISSTBINFilename)
        {
        }
        #endregion


        //将没有反演值的区域填充为默认值
        private void FILLVALUE_Ocean()
        {

        }

        //populates optical depth at 0.5 micron for both land and ocean
        private void POPULATE_TAU_LAND_OCEAN(int IDATA)
        {

        }

        private double COMPUTE_GLINTANGLE(float MTHET0, float MTHET, float MDPHI, out int QA_Flag_Ocean8)
        {
            double GLINT_ANGLE = 0.0;
            QA_Flag_Ocean8 = -1;
            if (MTHET0 > 0 && MTHET > 0 && MDPHI>0)
            {
                GLINT_ANGLE = Math.Cos(MTHET0 * DTR) * Math.Cos(MTHET * DTR) + Math.Sin(MTHET0 * DTR) * Math.Sin(MTHET * DTR) * Math.Cos(MDPHI * DTR);
                GLINT_ANGLE = Math.Acos(GLINT_ANGLE) * RTD;
            }
            if (GLINT_ANGLE >= GLINT_THRESHOLD)
                QA_Flag_Ocean8 = 1;
            else
                QA_Flag_Ocean8 = 0;
            return GLINT_ANGLE;
        }

        /// <summary>
        /// 计算输出的HDF文件中的各个数据集的数值
        /// </summary>
        /// <param name="IDATA"></param>
        /// <param name="Lat_center"></param>
        /// <param name="Lon_center"></param>
        /// <param name="MTHET0"></param>
        /// <param name="MTHET"></param>
        /// <param name="MPHI0"></param>
        /// <param name="MPHI"></param>
        /// <param name="MSCATT"></param>
        /// <param name="QA_Temp"></param>
        private void POPULATE_COMMON_ARRAYS(int IDATA,float Lat_center,float Lon_center,float MTHET0,float MTHET,float MPHI0,float MPHI,float MSCATT,int QA_Temp)
        {

        }

        private int LAND_WATER_CLOUD(int[] idex, bool[,] CldMsk_1km, int[,] Land_CLDMSK_forfraction, bool[,] DET_Flag, bool[,] SunGlint_Flag, bool[,] SnowIce_Flag, int[,] LandSea_Flag, bool[,] Shadow_Flag, bool[,] High_Cloud_Flag, out int[] QA_Flag_Ocean, out int Water, out int Land)
        {
            int ILINE =10,ISWATH=1354;
            //Land_CLDMSK_forfraction = new float[ILINE, ISWATH];//由CldMsk_Land返回
            byte[,] CldMsk_250 = new byte[4 * ILINE,4 * ISWATH];
            byte[,] CldMsk_500 = new byte[2 * ILINE, 2 * ISWATH];

            byte[,] High_Cloud_Flag_500 = new byte[2 * ILINE, 2 * ISWATH];
            //byte[,] High_Cloud_Flag = new byte[ILINE, ISWATH];
            byte[,] Quality_cirrus = new byte[ILINE, ISWATH];
            //byte[,] Shadow_Flag = new byte[ILINE, ISWATH];
            byte[,] SnowMsk_Ratio = new byte[ILINE, ISWATH];
            int[] QA_Flag_Land = new int[19];
            QA_Flag_Ocean = new int[12];            
            int Ret_Quality_cirrus=1;
            int cloud_num = 0, cloud_num_land = 0;
            int Det_cldmsk = 0, Desert=0, Glint = 0, Snowice = 0;
            Water = 0; Land = 0;
            int START_1KM=idex[0];
            int END_1KM=idex[1];
            int START_500=idex[2]; 
            int END_500=idex[3]; 
            int START_250=idex[4]; 
            int END_250=idex[5];
            //cloud_num and cloud_num _land after cloud mask at 1km  before ice,snow flag
            int IXX, IYY;
            for (IYY = 0; IYY < 10;IYY++ )
            {
                for (IXX = START_1KM; IXX <= END_1KM;IXX++ )
                {
                    if (CldMsk_1km[IYY, IXX] ==false )
                        cloud_num++;
                    if (Land_CLDMSK_forfraction[IYY, IXX] == 0)
                        cloud_num_land++;
                }
            }
            //Calculating number of land, water, cloudy, sun-glint and snow/ice pixels
            for (IYY = 0; IYY < 10; IYY++)
            {
                for (IXX = START_1KM; IXX <= END_1KM; IXX++)
                {
                    if (DET_Flag[IYY, IXX] == true)// 1 = determined 
                        Det_cldmsk++;
                    if (SunGlint_Flag[IYY, IXX] == false)//0 = Yes,是耀斑
                        Glint++;
                    if (SnowIce_Flag[IYY, IXX] == true)//1=no,非云雪
                        Snowice++;
                    if (DET_Flag[IYY, IXX] == true && LandSea_Flag[IYY, IXX] == 0)//LandSea_Flag=0,water
                        Water++;
                    else if (DET_Flag[IYY, IXX] == true && LandSea_Flag[IYY, IXX] == 2 || DET_Flag[IYY, IXX] == true && LandSea_Flag[IYY, IXX] == 3)//LandSea_Flag=2, Desert ;=3,land
                        Land++;
                }
            }
            //Glint, snow/ice and shadow pixels are treated as cloudy pixels to exclude them in the process
            int l2, p2, l11,p11;
            //for 10 * 10   box.
            for (IYY = 0; IYY < 10; IYY++)
            {
                for (IXX = START_1KM; IXX <= END_1KM; IXX++)
                {
                    //High cloud flag for ocean in 500 m resolution
                    for (l2 = 0; l2 < 2;l2++ )
                    {
                        for (p2 = 0; p2 < 2;p2++ )
                        {
                            High_Cloud_Flag_500[2 * IYY + l2, 2 * IXX+p2] = 1;//初始为1，无云
                        }
                    }
                    if (High_Cloud_Flag[IYY,IXX] == false)
                    {
                        for (l2 = 0; l2 < 2; l2++)
                        {
                            for (p2 = 0; p2 < 2; p2++)
                            {
                                High_Cloud_Flag_500[2 * IYY + l2, 2 * IXX + p2] = 0;
                            }
                        }
                    }
                    if (SnowIce_Flag[IYY, IXX] == false || SnowMsk_Ratio[IYY, IXX] == 0 || Shadow_Flag[IYY, IXX] == false)
                    {
                        if (IYY>1&&IYY<8)
                        {
                            if (IXX>START_1KM+1&&IXX<END_1KM-1)
                            {
                                for (l11=IYY-2;l11<=IYY+2;l11++)
                                {
                                    for (p11 = IXX - 2; p11 <= IXX + 2; p11++)
                                    {
                                        CldMsk_1km[l11, p11] = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            int l4, p4;
            for (IYY = 0; IYY < 10; IYY++)
            {
                for (IXX = START_1KM; IXX <= END_1KM; IXX++)
                {
                    if (CldMsk_1km[IYY,IXX]==false)
                    {
                        for (l4 = 0; l4 < 4;l4++ )
                        {
                            for (p4 = 0; p4 < 4;p4++ )
                            {
                                CldMsk_250[4 * IYY + l4, 4 * IXX + p4] = 0;
                            }
                        }
                        for (l2 = 0; l2 < 2; l2++)
                        {
                            for (p2 = 0; p2 < 2; p2++)
                            {
                                CldMsk_500[2 * IYY + l2, 2 * IXX + p2] = 0;
                            }
                        }
                    }
                }
            }
            //Setting quality Flag to retrive with lower quality for cirrus or smoke?
            int N = 0;
            for (IYY = 0; IYY < 10; IYY++)
            {
                for (IXX = START_1KM; IXX <= END_1KM; IXX++)
                {
                    N = 0;
                    for (l4 = 0; l4 < 4; l4++)
                    {
                        for (p4 = 0; p4 < 4; p4++)
                        {
                            if (Quality_cirrus[IYY, IXX] == 1 && CldMsk_250[4 * IYY + l4, 4 * IXX + p4] == 1)
                                N++;
                        }
                    }
                    if (N == 16)
                        Ret_Quality_cirrus = 0;
                }
            }
            //Set QA array values; QA_Land(3)=1 set for daytime only, which is checked by the solar zenith angle in MOD_PR04_PR05_V2.f
            for (int i = 0; i < 19;i++ )
            {
                QA_Flag_Land[i] = 0;
            }
            if (cloud_num == 100)
                QA_Flag_Land[0] = 1;
            else
                QA_Flag_Land[0] = 0;
            //Setting 2nd bit of ist byte all to Zero. Since Wisconsin group cloud mask is not used this bit is not valid
            if (cloud_num > 0 && cloud_num <= 30)
                QA_Flag_Land[1] = 0;
            if (cloud_num > 31 && cloud_num <= 60)
                QA_Flag_Land[1] = 1;
            if (cloud_num > 61 && cloud_num <= 90)
                QA_Flag_Land[1] =2;
            if (cloud_num > 91)
                QA_Flag_Land[1] =3;
            QA_Flag_Ocean[6] = 0;
            if (Snowice >= 90)
                QA_Flag_Land[2] = 0;
            else
                QA_Flag_Land[2] = 1;
            if (Water >= 90)
                QA_Flag_Land[3] = 0;
            else if(Desert==100)
                QA_Flag_Land[3] = 2;
            else if (Desert != 100&&Land==100)
                QA_Flag_Land[3] = 3;
            else
                QA_Flag_Land[3] = 1;
            if (Water >= 100)
                QA_Flag_Ocean[7] = 1;
            else
                QA_Flag_Ocean[7] = 0;
            //Set Cloud Mask QA bit to QA_Temp array
            int QA_Temp = 0;
            QA_Temp=BYTE_SET(QA_Flag_Land[0], 0, QA_Temp);
              //CALL BYTE_SET(QA_Flag_Land(2),1,QA_Temp)
            QA_Temp = BYTE_SET(QA_Flag_Land[1], 1, QA_Temp);
            //CALL BYTE_SET(QA_Flag_Land(3),3,QA_Temp)
            QA_Temp = BYTE_SET(QA_Flag_Land[2], 3, QA_Temp);
              //CALL BYTE_SET(QA_Flag_Land(4),4,QA_Temp)
            QA_Temp = BYTE_SET(QA_Flag_Land[3], 4, QA_Temp);

            return QA_Temp;
        }

        private int  BYTE_SET(int QA_V,int Bit_SP, int QA_Byte)
        {
            int [] myints =new int [1]{QA_Byte};
            BitArray bits_Temp = new BitArray(myints);
            if (QA_V == 0)
                bits_Temp[Bit_SP] = false;
            else if(QA_V == 1)
                bits_Temp[Bit_SP] = true;
            else if (QA_V == 2)
            { 
                bits_Temp[Bit_SP] = false; 
                bits_Temp[Bit_SP + 1] = true;
            }
            else if (QA_V == 3)
            {
                bits_Temp[Bit_SP] = true;
                bits_Temp[Bit_SP + 1] = true;
            }
            else if (QA_V == 4)
            {
                bits_Temp[Bit_SP] = false;
                bits_Temp[Bit_SP + 1] = false;
                bits_Temp[Bit_SP + 2] = true;
            }
            else if (QA_V == 5)
            {
                bits_Temp[Bit_SP] = true;
                bits_Temp[Bit_SP + 1] = false;
                bits_Temp[Bit_SP + 2] = true;
            }
            else if (QA_V ==6)
            {
                bits_Temp[Bit_SP] = false;
                bits_Temp[Bit_SP + 1] = true;
                bits_Temp[Bit_SP + 2] = true;
            }
            else if (QA_V == 7)
            {
                bits_Temp[Bit_SP] = true;
                bits_Temp[Bit_SP + 1] = true;
                bits_Temp[Bit_SP + 2] = true;
            }
            else if (QA_V == 8)
            {
                bits_Temp[Bit_SP] = false;
                bits_Temp[Bit_SP + 1] = false;
                bits_Temp[Bit_SP + 2] = false;
                bits_Temp[Bit_SP + 3] = true;
            }
            else if (QA_V == 9)
            {
                bits_Temp[Bit_SP] = true;
                bits_Temp[Bit_SP + 1] = false;
                bits_Temp[Bit_SP + 2] = false;
                bits_Temp[Bit_SP + 3] = true;
            }
            else if (QA_V == 10)
            {
                bits_Temp[Bit_SP] = false;
                bits_Temp[Bit_SP + 1] = true;
                bits_Temp[Bit_SP + 2] = false;
                bits_Temp[Bit_SP + 3] = true;
            }
            else if (QA_V == 11)
            {
                bits_Temp[Bit_SP] = true;
                bits_Temp[Bit_SP + 1] = true;
                bits_Temp[Bit_SP + 2] = false;
                bits_Temp[Bit_SP + 3] = true;
            }
            else if (QA_V == 12)
            {
                bits_Temp[Bit_SP] = false;
                bits_Temp[Bit_SP + 1] = false;
                bits_Temp[Bit_SP + 2] = true;
                bits_Temp[Bit_SP + 3] = true;
            }
            else if (QA_V == 13)
            {
                bits_Temp[Bit_SP] = true;
                bits_Temp[Bit_SP + 1] = false;
                bits_Temp[Bit_SP + 2] = true;
                bits_Temp[Bit_SP + 3] = true;
            }
            else if (QA_V == 14)
            {
                bits_Temp[Bit_SP] = false;
                bits_Temp[Bit_SP + 1] = true;
                bits_Temp[Bit_SP + 2] = true;
                bits_Temp[Bit_SP + 3] = true;
            }
            else if (QA_V == 15)
            {
                bits_Temp[Bit_SP] = true;
                bits_Temp[Bit_SP + 1] = true;
                bits_Temp[Bit_SP + 2] = true;
                bits_Temp[Bit_SP + 3] = true;
            }
            bits_Temp.CopyTo(myints, 0);
            return myints[0];
        }

        private void TRANS_2WAY_NECP(List<float[,]> L1BDataList,int [,] LandSea_Flag,int Iscan,int[] idex,float SatZen, float SolZen,double  Total_H2O, double Total_O3)
        {
            float[,] Refl_1=L1BDataList[0];
            float[,] Refl_2 = L1BDataList[1];
            float[,] Refl_3 = L1BDataList[2];
            float[,] Refl_4 = L1BDataList[3];
            float[,] Refl_5 = L1BDataList[4];
            float[,] Refl_6 = L1BDataList[5];
            float[,] Refl_7 = L1BDataList[6];
            float[,] Refl_9 = L1BDataList[7];
            float[,] Refl_12 = L1BDataList[8];
            float[,] Refl_13 = L1BDataList[9];
            float[,] Refl_16 = L1BDataList[10];
            //Refl_26,Rad_20,Rad_31,Rad_32不需要进行大气校正
            float[,] Refl_26 = L1BDataList[11];
            float[,] Rad_20 = L1BDataList[12];
            float[,] Rad_31 = L1BDataList[13];
            float[,] Rad_32 = L1BDataList[14];
            int ILINE = 10, ISWATH = 1354;
            int scans = 203;

            float[,] Refl_1_Iscan_GC = new float[4 * scans * ILINE, 4 * ISWATH];//band1
            float[,] Refl_2_Iscan_GC = new float[4 * scans * ILINE, 4 * ISWATH];//band2
            float[,] Refl_3_Iscan_GC = new float[2 * scans * ILINE, 2 * ISWATH];//band3
            float[,] Refl_4_Iscan_GC = new float[2 * scans * ILINE, 2 * ISWATH];//band4
            float[,] Refl_5_Iscan_GC = new float[2 * scans * ILINE, 2 * ISWATH];//band5
            float[,] Refl_6_Iscan_GC = new float[2 * scans * ILINE, 2 * ISWATH];//band6
            float[,] Refl_7_Iscan_GC = new float[2 * scans * ILINE, 2 * ISWATH];//band7
            float[,] Refl_9_Iscan_GC = new float[scans * ILINE, ISWATH];//9
            float[,] Refl_12_Iscan_GC = new float[scans * ILINE, ISWATH];//12
            float[,] Refl_13_Iscan_GC = new float[scans * ILINE, ISWATH];//13
            float[,] Refl_16_Iscan_GC = new float[scans * ILINE, ISWATH];//16
            float[,] Rad_20_Iscan_GC = new float[scans * ILINE, ISWATH];//band20
            float[,] Refl_26_Iscan_GC = new float[scans * ILINE, ISWATH];//band26,//未用
            float[,] Rad_31_Iscan_GC = new float[scans * ILINE, ISWATH];//band31
            float[,] Refl_32_Iscan_GC = new float[scans * ILINE, ISWATH];//band32
            float[,] Refl_31_HKM_Iscan_GC = new float[2 * scans * ILINE, 2 * ISWATH];
            int START_1KM = idex[0];//IDATA * 10;
            int END_1KM = idex[1];//IDATA * 10+9
            int START_500 = idex[2];//IDATA * 20
            int END_500 = idex[3];//IDATA * 20+19
            int START_250 = idex[4];//IDATA * 40
            int END_250 = idex[5];//IDATA * 40+39
            //temperature conversion variables
            double Planck_constant = 6.6260755e-34, Speed_light = 2.9979246e+8, Boltz_cons = 1.380658e-23, wav1 = 3.75, wav2 = 11.0, wav3 = 12.0;
            //change total precipitable water unit from mm (kg/m^2) to cm
            double Corr_Cirrus_S = 0.0;
            Total_H2O = Total_H2O / 10.0;

            //Calculate gemoetric factor for 2-way transmission
            //the air mass factor (G)是太阳天顶角和卫星天顶角的函数
            double G_factor = -1.0;
            if (SatZen > 0 && SolZen > 0)
                G_factor = 1 / Math.Cos(SatZen * DTR) + 1 / Math.Cos(SolZen * DTR);

            #region Calculate 2-way H2O transmission
            //Bi_H2O_Coef表示利用NECP的H2O数据进行计算的系数K1,k2,k3
            double[] B1_H2O_Coef = new double[3] { -5.73888, 0.925534, -0.0188365 };//0.66
            double[] B2_H2O_Coef = new double[3] { 5.32960, 0.824260, -0.0277443 };//0.86
            double[] B5_H2O_Coef = new double[3] { -6.39296, 0.942186, -0.0131901 };//1.24
            double[] B6_H2O_Coef = new double[3] { -7.76288, 0.979707, 0.007784 };//1.64
            double[] B7_H2O_Coef = new double[3] { -4.05388, 0.872951, -0.0268464 };//2.12
            //Opt_H2O_Clim_Chi表示不利用NCEP数据时的H2O光学厚度
            double Opt_H2O_Clim_Ch1 = 1.543E-02;//0.66
            double Opt_H2O_Clim_Ch2 = 1.947E-02;//0.86
            double Opt_H2O_Clim_Ch5 = 1.184E-02;//1.24
            double Opt_H2O_Clim_Ch6 = 9.367E-03;//1.64
            double Opt_H2O_Clim_Ch7 = 5.705E-02;//2.12
            double LOGCON, LOGCON2, EXPONENT;
            //RTrans_H2O_Chi表示计算得到的包含水汽吸收的大气透过率
            double RTrans_H2O_Ch1, RTrans_H2O_Ch2, RTrans_H2O_Ch5, RTrans_H2O_Ch6, RTrans_H2O_Ch7;
            if (Total_H2O > 0 && G_factor > 0)//GDAS数据读取成功
            {
                LOGCON = Math.Log(Total_H2O * G_factor);
                LOGCON2 = LOGCON * LOGCON;
                EXPONENT = B1_H2O_Coef[0] + B1_H2O_Coef[1] * LOGCON + B1_H2O_Coef[2] * LOGCON2;
                RTrans_H2O_Ch1 = Math.Exp(Math.Exp(EXPONENT));
                EXPONENT = B2_H2O_Coef[0] + B2_H2O_Coef[1] * LOGCON + B2_H2O_Coef[2] * LOGCON2;
                RTrans_H2O_Ch2 = Math.Exp(Math.Exp(EXPONENT));
                EXPONENT = B5_H2O_Coef[0] + B5_H2O_Coef[1] * LOGCON + B5_H2O_Coef[2] * LOGCON2;
                RTrans_H2O_Ch5 = Math.Exp(Math.Exp(EXPONENT));
                EXPONENT = B6_H2O_Coef[0] + B6_H2O_Coef[1] * LOGCON + B6_H2O_Coef[2] * LOGCON2;
                RTrans_H2O_Ch6 = Math.Exp(Math.Exp(EXPONENT));
                EXPONENT = B7_H2O_Coef[0] + B7_H2O_Coef[1] * LOGCON + B7_H2O_Coef[2] * LOGCON2;
                RTrans_H2O_Ch7 = Math.Exp(Math.Exp(EXPONENT));
            }
            else//采用默认参数
            {
                RTrans_H2O_Ch1 = Math.Exp(Opt_H2O_Clim_Ch1 * G_factor);
                RTrans_H2O_Ch2 = Math.Exp(Opt_H2O_Clim_Ch2 * G_factor);
                RTrans_H2O_Ch5 = Math.Exp(Opt_H2O_Clim_Ch5 * G_factor);
                RTrans_H2O_Ch6 = Math.Exp(Opt_H2O_Clim_Ch6 * G_factor);
                RTrans_H2O_Ch7 = Math.Exp(Opt_H2O_Clim_Ch7 * G_factor);
            }
            #endregion
            
            # region Calculate 2-way O3 transmission 
            //Bi_O3_Coef表示利用NECP的O3数据进行计算的系数K,Opt_O3_Clim_Chi表示不利用NCEP数据时的O3光学厚度
            double B1_O3_Coef = 5.09E-5,Opt_O3_Clim_Ch1 = 2.478E-02;//0.66
            double B3_O3_Coef = 4.26E-6,Opt_O3_Clim_Ch3 = 2.432E-03;//0.47
            double B4_O3_Coef = 1.05E-4,Opt_O3_Clim_Ch4 = 2.957E-02;//0.55
            //RTrans_O3_Chi表示计算得到的包含臭氧吸收的大气透过率
            double RTrans_O3_Ch1, RTrans_O3_Ch3, RTrans_O3_Ch4;
            if (Total_O3 > 0 && G_factor > 0)//GDAS数据O3读取成功
            {
                RTrans_O3_Ch1 = Math.Exp(B1_O3_Coef * Total_O3 * G_factor);
                RTrans_O3_Ch3 = Math.Exp(B3_O3_Coef * Total_O3 * G_factor);
                RTrans_O3_Ch4 = Math.Exp(B4_O3_Coef * Total_O3 * G_factor);
            }
            else//采用默认参数
            {
                RTrans_O3_Ch1 = Math.Exp(Opt_O3_Clim_Ch1 * G_factor);
                RTrans_O3_Ch3 = Math.Exp(Opt_O3_Clim_Ch3 * G_factor);
                RTrans_O3_Ch4 = Math.Exp(Opt_O3_Clim_Ch4 * G_factor);
            }
            #endregion

            # region 计算CO2气体吸收的大气透过率transmission，全球一样，参数默认
            //Opt_CO2_Clim_Chi表示不利用NCEP数据时的CO2光学厚度
            double Opt_CO2_Clim_Ch5 = 4.196E-04;//1.24,
            double Opt_CO2_Clim_Ch6 = 8.260E-03;//1.64
            double Opt_CO2_Clim_Ch7 = 2.164E-02;//2.12
            double RTrans_CO2_Ch5, RTrans_CO2_Ch6, RTrans_CO2_Ch7;
            //RTrans_CO2_Chi表示不利用NCEP数据时，利用默认CO2光学厚度Opt_CO2_Clim_Chi计算得到的大气透过率
            RTrans_CO2_Ch5 = Math.Exp(Opt_CO2_Clim_Ch5 * G_factor);
            RTrans_CO2_Ch6 = Math.Exp(Opt_CO2_Clim_Ch6 * G_factor);
            RTrans_CO2_Ch7 = Math.Exp(Opt_CO2_Clim_Ch7 * G_factor);
            #endregion

            # region 计算由于多种大气吸收形成的大气透过率Multi_factor_Chi
            double Multi_factor_Ch1, Multi_factor_Ch2, Multi_factor_Ch3, Multi_factor_Ch4, Multi_factor_Ch5, Multi_factor_Ch6, Multi_factor_Ch7;
            Multi_factor_Ch1 = RTrans_H2O_Ch1 * RTrans_O3_Ch1;//进行H2O和O3校正
            Multi_factor_Ch2 = RTrans_H2O_Ch2;//进行H2O校正
            Multi_factor_Ch3 = RTrans_O3_Ch3;//进行O3校正
            Multi_factor_Ch4 = RTrans_O3_Ch4;//进行O3校正
            Multi_factor_Ch5 = RTrans_H2O_Ch5 * RTrans_CO2_Ch5;//进行H2O和CO2校正
            Multi_factor_Ch6 = RTrans_H2O_Ch6 * RTrans_CO2_Ch6;//进行H2O和CO2校正
            Multi_factor_Ch7 = RTrans_H2O_Ch7 * RTrans_CO2_Ch7;//进行H2O和CO2校正
            #endregion
            
            int [] QA_Flag_Land=new int [19];
            int [] QA_Flag_Ocean=new int [12];
            double [] Corr_Cirrus =new double [3];
            int il, ip, il2, ip2, il4, ip4;
            int N_Red;
            double Total_Red;
            double c1, c2, w_meter, wave=0;
            # region Loop (1x1 km reoslution) over along and across scan line within a granule;
              for ( il = 0; il < ILINE;il++ )
              {
                  for (ip=START_1KM;ip<=END_1KM;ip++)
                  {
                    //Perform cirrus correction (if Reflectance of 1.38 micron < 1 % and Reflectance of 0.66 micron > 4%)
                     N_Red=0;
                     Total_Red = 0.0;
                     for (il4 = 4 * il; il4 <= 4 * (il + 1) - 3;il4++ )
                     {
                         for (ip4 = 4 * ip; ip4 < 4 * (ip + 1) - 3;ip4++ )
                         {
                             if (Refl_1[Iscan*40+il4, ip4] > 0)
                             {
                                 Total_Red = Total_Red + Refl_1[Iscan * 40 + il4, ip4];
                                 N_Red = N_Red + 1;
                             }
                         }
                     }
                     if (N_Red > 0)
                         Total_Red = Total_Red / N_Red;
                     else
                         Total_Red = 0;
                      #region QA_Flag_Ocean(10) is used instead of QA_Flag_Ocean(9)
                     if (LandSea_Flag[il, ip] == 0)// water 
                     {
                         if (Refl_26[Iscan * 10 + il, ip] > 0)
                         {
                             if (Total_Red >= 0.04 && Refl_26[Iscan * 10 + il, ip] < 0.005)
                             {
                                 Corr_Cirrus_S = 2 * Refl_26[Iscan * 10 + il, ip];
                                 QA_Flag_Land[15] = 0;
                                 QA_Flag_Ocean[9] = 0;
                             }
                             if (Total_Red < 0.04)
                             {
                                 for (int i = 0; i < 3; i++)
                                 {
                                     Corr_Cirrus[i] = 9999.0;
                                 }
                                 Corr_Cirrus_S = 9999.0;
                                 QA_Flag_Land[15] = 2;
                                 QA_Flag_Ocean[9] = 2;
                             }
                             if (Refl_26[Iscan * 10 + il, ip] > 0.005)
                             {
                                 for (int i = 0; i < 3; i++)
                                 {
                                     Corr_Cirrus[i] = 9999.0;
                                 }
                                 Corr_Cirrus_S = 9999.0;
                                 QA_Flag_Land[15] = 3;
                                 QA_Flag_Ocean[9] = 3;
                             }
                         }
                         else
                         {
                             for (int i = 0; i < 3; i++)
                             {
                                 Corr_Cirrus[i] = 9999.0;
                             }
                             Corr_Cirrus_S = 9999.0;
                             QA_Flag_Land[15] = 1;
                             QA_Flag_Ocean[9] = 1;

                         }
                     }
                     else//for LandSea_Flag = 1 - COASTAL, LandSea_Flag = 2 - DESERT and LandSea_Flag = 3 - LAND
                     {
                         if (Refl_26[Iscan * 10 + il, ip] > 0)
                         {
                             if (Total_Red >= 0.04 && Refl_26[Iscan * 10 + il, ip] < 0.005)
                             {
                                 Corr_Cirrus_S = 2 * Refl_26[Iscan * 10 + il, ip];
                                 QA_Flag_Land[15] = 0;
                                 QA_Flag_Ocean[9] = 0;
                             }
                             if (Total_Red < 0.04)
                             {
                                 Corr_Cirrus[0] = 9999.0;
                                 Corr_Cirrus_S = 9999.0;
                                 QA_Flag_Land[15] = 2;
                                 QA_Flag_Ocean[9] = 2;
                             }
                             if (Refl_26[Iscan * 10 + il, ip] > 0.005)
                             {
                                 Corr_Cirrus[0] = 9999.0;
                                 Corr_Cirrus_S = 9999.0;
                                 QA_Flag_Land[15] = 3;
                                 QA_Flag_Ocean[9] = 3;
                             }
                         }
                         else
                         {
                             Corr_Cirrus[0] = 9999.0;
                             Corr_Cirrus_S = 9999.0;
                             QA_Flag_Land[15] = 1;
                             QA_Flag_Ocean[9] = 1;

                         }
                     }
                      #endregion                   

                      #region Correct radiance due to gaseous absorption and cirrus clouds contamination
                     if (LandSea_Flag[il, ip] == 0)//For Ocean
                     {
                         # region  For Ocean
                         for (il4 = 4 * il; il4 <= 4 * (il + 1) - 3; il4++)
                          {
                              for (ip4 = 4 * ip; ip4 < 4 * (ip + 1) - 3; ip4++)
                              {
                                  if (Refl_1[Iscan * 40 + il4, ip4] > 0)
                                      Refl_1_Iscan_GC[Iscan * 40 + il4, ip4] = Refl_1[Iscan * 40 + il4, ip4] * (float)Multi_factor_Ch1;
                                  else
                                      Refl_1_Iscan_GC[Iscan * 40 + il4, ip4] = Refl_1[Iscan * 40 + il4, ip4];
                                  if (Refl_2[Iscan * 40 + il4, ip4] > 0)
                                      Refl_2_Iscan_GC[Iscan * 40 + il4, ip4] = Refl_2[Iscan * 40 + il4, ip4] * (float)Multi_factor_Ch2;
                                  else
                                      Refl_2_Iscan_GC[Iscan * 40 + il4, ip4] = Refl_2[Iscan * 40 + il4, ip4];

                              }
                          }
                          for (il2 = il * 2; il2 < 2 * (il + 1) - 1; il2++)
                          {
                              for (ip2 = 2 * ip; ip2 < 2 * (ip + 1) - 1; ip2++)
                              {
                                  if (Refl_3[Iscan * 20 + il2, ip2] > 0)
                                      Refl_3_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_3[Iscan * 20 + il2, ip2] * (float)Multi_factor_Ch3;
                                  else
                                      Refl_3_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_3[Iscan * 20 + il2, ip2];
                                  if (Refl_4[Iscan * 20 + il2, ip2] > 0)
                                      Refl_4_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_4[Iscan * 20 + il2, ip2] * (float)Multi_factor_Ch4;
                                  else
                                      Refl_4_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_4[Iscan * 20 + il2, ip2];
                                  if (Refl_5[Iscan * 20 + il2, ip2] > 0)
                                      Refl_5_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_5[Iscan * 20 + il2, ip2] * (float)Multi_factor_Ch5;
                                  else
                                      Refl_5_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_5[Iscan * 20 + il2, ip2];
                                  if (Refl_6[Iscan * 20 + il2, ip2] > 0)
                                      Refl_6_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_6[Iscan * 20 + il2, ip2] * (float)Multi_factor_Ch6;
                                  else
                                      Refl_6_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_6[Iscan * 20 + il2, ip2];
                                  if (Refl_7[Iscan * 20 + il2, ip2] > 0)
                                      Refl_7_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_7[Iscan * 20 + il2, ip2] * (float)Multi_factor_Ch7;
                                  else
                                      Refl_7_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_7[Iscan * 20 + il2, ip2];
                              }
                          }
                          if (Refl_9[Iscan*10 + il, ip] > 0)
                              Refl_9_Iscan_GC[Iscan * 10 + il, ip] = Refl_9[Iscan * 10 + il, ip] * (float)Multi_factor_Ch3;
                          if (Refl_12[Iscan * 10 + il, ip] > 0)
                              Refl_12_Iscan_GC[Iscan * 10 + il, ip] = Refl_12[Iscan * 10 + il, ip] * (float)Multi_factor_Ch4;
                          if (Refl_13[Iscan * 10 + il, ip] > 0)
                              Refl_13_Iscan_GC[Iscan * 10 + il, ip] = Refl_13[Iscan * 10 + il, ip] * (float)Multi_factor_Ch1;
                          if (Refl_16[Iscan * 10 + il, ip] > 0)
                              Refl_16_Iscan_GC[Iscan * 10 + il, ip] = Refl_16[Iscan * 10 + il, ip] * (float)Multi_factor_Ch2;
                      #endregion
                     }
                     else
                     {
                         # region For Land
                         for (il4 = 4 * il; il4 <= 4 * (il + 1) - 3; il4++)
                          {
                              for (ip4 = 4 * ip; ip4 < 4 * (ip + 1) - 3; ip4++)
                              {
                                  if (Refl_1[Iscan * 40 + il4, ip4] > 0)
                                      Refl_1_Iscan_GC[Iscan * 40 + il4, ip4] = Refl_1[Iscan * 40 + il4, ip4] * (float)Multi_factor_Ch1;
                                  else
                                      Refl_1_Iscan_GC[Iscan * 40 + il4, ip4] = Refl_1[Iscan * 40 + il4, ip4];
                                  if (Refl_2[Iscan * 40 + il4, ip4] > 0)
                                      Refl_2_Iscan_GC[Iscan * 40 + il4, ip4] = Refl_2[Iscan * 40 + il4, ip4] * (float)Multi_factor_Ch2;
                                  else
                                      Refl_2_Iscan_GC[Iscan * 40 + il4, ip4] = Refl_2[Iscan * 40 + il4, ip4];

                              }
                          }
                          for (il2 = il * 2; il2 < 2 * (il + 1) - 1; il2++)
                          {
                              for (ip2 = 2 * ip; ip2 < 2 * (ip + 1) - 1; ip2++)
                              {
                                  if (Refl_3[Iscan * 20 + il2, ip2] > 0)
                                      Refl_3_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_3[Iscan * 20 + il2, ip2] * (float)Multi_factor_Ch3;
                                  else
                                      Refl_3_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_3[Iscan * 20 + il2, ip2];
                                  if (Refl_4[Iscan * 20 + il2, ip2] > 0)
                                      Refl_4_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_4[Iscan * 20 + il2, ip2] * (float)Multi_factor_Ch4;
                                  else
                                      Refl_4_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_4[Iscan * 20 + il2, ip2];
                                  if (Refl_5[Iscan * 20 + il2, ip2] > 0)
                                      Refl_5_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_5[Iscan * 20 + il2, ip2] * (float)Multi_factor_Ch5;
                                  else
                                      Refl_5_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_5[Iscan * 20 + il2, ip2];
                                  if (Refl_6[Iscan * 20 + il2, ip2] > 0)
                                      Refl_6_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_6[Iscan * 20 + il2, ip2] * (float)Multi_factor_Ch6;
                                  else
                                      Refl_6_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_6[Iscan * 20 + il2, ip2];
                                  if (Refl_7[Iscan * 20 + il2, ip2] > 0)
                                      Refl_7_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_7[Iscan * 20 + il2, ip2] * (float)Multi_factor_Ch7;
                                  else
                                      Refl_7_Iscan_GC[Iscan * 20 + il2, ip2] = Refl_7[Iscan * 20 + il2, ip2];
                              }
                          }
                         #endregion
                     }
                      //compute Temprature channels ( convert from radiance to temperature)
                     c1 = 2.0 * Planck_constant * (Speed_light * Speed_light);
                     c2 = (Planck_constant * Speed_light) / Boltz_cons;
                      //convert wavelength to meters
                     for (int ij = 1; ij <= 3;ij++ )
                     {
                         if (ij == 1)
                             wave = wav1;
                         if (ij == 2)
                             wave = wav2;
                         if (ij == 3)
                             wave = wav3;
                         w_meter = (1.0e-6 * wave);
                         if (ij == 1 && Rad_20[Iscan * 10 + il, ip]>0)
                             Rad_20_Iscan_GC[Iscan * 10 + il, ip] = (float)(c2 / (w_meter * Math.Log(c1 / (1.0e+6 * Rad_20[Iscan * 10 + il, ip] * Math.Pow(w_meter,5)) + 1.0)));
                         if (ij == 2 && Rad_31[Iscan * 10 + il, ip] > 0)
                             Rad_31_Iscan_GC[Iscan * 10 + il, ip] = (float)(c2 / (w_meter * Math.Log(c1 / (1.0e+6 * Rad_31[Iscan * 10 + il, ip] * Math.Pow(w_meter, 5)) + 1.0)));
                         if (ij == 3 && Rad_32[Iscan * 10 + il, ip] > 0)
                             Refl_32_Iscan_GC[Iscan * 10 + il, ip] = (float)(c2 / (w_meter * Math.Log(c1 / (1.0e+6 * Rad_32[Iscan * 10 + il, ip] * Math.Pow(w_meter, 5)) + 1.0)));
                     }
                      #endregion                      
                  }
              }
            #endregion

            #region convert to 500 meter resolution
            for (il = 0; il < ILINE; il++)
            {
                for (ip = START_1KM; ip <= END_1KM; ip++)
                {
                    if (Rad_31_Iscan_GC[Iscan * 10 + il, ip] > 0)
                    {
                        for (il2 = 0; il2 < 2; il2++)
                        {
                            for (ip2 = 0; ip2 < 2; ip2++)
                            {
                                Refl_31_HKM_Iscan_GC[Iscan * 20 + il * 2 + il2, ip * 2 + ip2] = Rad_31_Iscan_GC[Iscan * 10 + il, ip];
                            }
                        }
                    }
                }
            }                

            #endregion
              
            # region Snow-masking scheme based upon Ref(1.24 micron)/Ref(0.86 micron) < 1 at 500 m resolution
            int N_0p86 = 0;
            double Total_0p86 = 0.0, Ratio = 0;
            int[,] SnowMsk_500m = new int[2 * scans * ILINE, 2 * ISWATH];
            int[,] SnowMsk_Ratio = new int[scans * ILINE, ISWATH];
            for (il = 0; il < 2 * ILINE;il++ )
            {
                for (ip = START_500; ip <= END_500;ip++ )
                {
                    N_0p86 = 0;
                    Total_0p86 = 0.0;
                    for (il2=2*il;il2<=2*(il+1)-1;il2++)
                    {
                        for (ip2 = 2 * ip; ip2 <= 2 * (ip + 1) - 1; ip2++)
                        {
                            if (Refl_2[Iscan*40+il2,ip2]>0)
                            {
                                Total_0p86 = Total_0p86 + Refl_2[Iscan * 40 + il2, ip2];
                                N_0p86 = N_0p86 + 1;
                            }
                        }
                    }
                    Ratio = 0;
                    if (Total_0p86 > 0 && Refl_5[Iscan * 20 + il, ip]>0)
                    {
                        if (N_0p86 > 0)
                            Ratio = ((Total_0p86 / N_0p86) - Refl_5[Iscan * 20 + il, ip]) / ((Total_0p86 / N_0p86) + Refl_5[Iscan * 20 + il, ip]);
                        else
                            Total_0p86 = 0.0; Ratio = 0.0;
                    }
                    else
                        Total_0p86 = 0.0; Ratio = 0.0;
                    SnowMsk_500m[Iscan * 20 + il, ip] = 1;
                    //IF(ratio.gt.0.20 .and.(Total_0p86/N_0p86).gt.0.08) THEN
                    if (Ratio > 0.20 && Refl_31_HKM_Iscan_GC[Iscan * 20 + il, ip] < 285)
                        SnowMsk_500m[Iscan * 20 + il, ip] = 0;
                }
            }
            int ISNOW = 0;
            for (il = 0; il < ILINE;il++ )
            {
                for (ip = START_1KM; ip <= END_1KM;ip++ )
                {
                    ISNOW = 0;
                    for (il2 = 2 * il; il2 < 2 * (il + 1);il2++ )
                    {
                        for (ip2 = 2 * ip; ip2 < 2 * (ip + 1);ip2++ )
                        {
                            if (SnowMsk_500m[Iscan * 20 + il2, ip2] == 0)
                                ISNOW = ISNOW + 1;
                        }
                    }
                    SnowMsk_Ratio[Iscan * 10 + il, ip] = 1;
                    if(ISNOW>=1)
                        SnowMsk_Ratio[Iscan * 10 + il, ip] = 0;
                }
            }
            #endregion

        }

        private void get_ancillary_aer(float lon, float lat, string gadsBINFilename, string tovsBINFilename, out float met_sfctmp, out float met_ugrd, out float met_vgrd, out float met_pwat, out float ozone)
        {
            float met_land = -999.0f,met_prmsl = -999.0f;
            met_sfctmp = -999.0f;
            met_pwat = -999.0f;
            met_ugrd = -999.0f;
            met_vgrd = -999.0f;
            //从GDASFname读取met_grid

            #region GDAS data
            float[, ,] met_grid = new float[181, 360, 54];
            float[] p = new float[26] { 1000.0f, 975.0f, 950.0f, 925.0f, 900.0f, 850.0f, 800.0f, 750.0f, 700.0f, 650.0f, 600.0f, 550.0f, 500.0f, 450.0f, 400.0f, 350.0f, 300.0f, 250.0f, 200.0f, 150.0f, 100.0f, 70.0f, 50.0f, 30.0f, 20.0f, 10.0f };
            float missing = -999.0f;
            float[] met_pres = new float[26];//Array of pressure levels (hPa)
            float[] met_temp = new float[26];//Array of atmospheric temperatures (K) at PRES(0:25)
            float[] met_mixr = new float[26];//Array of water vapor mixing ratios (g/kg) at PRES(0:25)
            for (int i = 0; i < 26;i++ )
            {
                met_pres[i] = missing;
                met_temp[i] = missing;
                met_mixr[i] = missing;
            }
            //Compute cell coordinates in met and ozn grids
            float x, x0, dx, y, y0, dy;
            x = Math.Min(Math.Max(lon, -179.99f), 179.99f);//经度
            if (x < 0.0f) //将-180～180范围转换为0～360范围。原0～180的不变，-180～0的变为180到360
                x = x + 360.0f;
            x0 = 0.0f;
            dx = 1.0f;
            int samples = (int)((x - x0 + 0.5 * dx) / dx)-1;//列号,注意：从0开始
            if (samples == 360)
                samples = 0;
            y = Math.Min(Math.Max(lat, -89.99f), 89.99f);//纬度
            y0 = 90.0f;
            dy = -1.0f;
            int lines = (int)((y - y0 + 0.5 * dy) / dy)-1;//行号,注意：从0开始
            for (int i = 0; i < 26; i++)
            {
                met_pres[i] = p[i];
                met_temp[i] =met_grid[lines,samples,i];
            }
            for (int i = 0; i < 21;i++ )
            {
                met_mixr[i]=met_grid[lines,samples,i+26];
            }
            met_land = met_grid[lines, samples, 47];//Land mask (0=water, 1=land)
            //blint_met.f()函数，met_sfctmp,Surface temperature (K)
            //float [,] met_grid48 =new float [181,360];
            //int l ,s;
            //for (l = 0; l < 181;l++ )
            //{
            //    for (s = 0;s < 360; s++)
            //    {
            //        met_grid48[l, s] = met_grid[l, s, 48];
            //    }
            //}
            met_sfctmp = met_grid[lines, samples, 48];//blint_met(met_grid48, lines, samples, x, y);//双线性插值
            //blint_met.f()函数，met_prmsl,
            met_prmsl = met_grid[lines, samples, 49];//blint_met(met_grid49, lines, samples, x, y);//双线性插值
            met_prmsl = met_prmsl * 0.01f;//Pressure (hPa) at mean sea level
            met_pwat = met_grid[lines, samples, 50];//Precipitable water (g/cm**2)
            met_ugrd = met_grid[lines, samples, 51];//Surface wind u component (m/s)
            met_vgrd = met_grid[lines, samples, 52];//Surface wind v component (m/s)
            //Convert relative humidity profile (%) to mixing ratio (g/kg)
            float satmix;
            for (int i = 0; i < 21; i++)
            {
                satmix = (float)(622.0 * ppv(met_temp[i]) / met_pres[i]);//Compute mixing ratio at 100% relative humidity
                met_mixr[i] = satmix * 0.01f * met_mixr[i];//Convert relative humidity to mixing ratio
            }
            for (int i = 20; i < 26; i++)
            {
                met_mixr[i] = (float)(Math.Max(met_mixr[20], 0.003) * Math.Pow((met_mixr[i]/100.0f),3));
                met_mixr[i] = (float )(Math.Max(met_mixr[i], 0.003));
            }
            #endregion

            #region OZONE DATA
            //从tovsBINFilename中读取臭氧数据
            float [,] ozn_grid =new float [181,360];
            ozone = ozn_grid[lines, samples];
            #endregion

            #region ICE DATA
            #endregion

            #region SST DATA
            #endregion

            #region NISE DATA 全球冰雪范围
            #endregion

        }

        /// <summary>
        /// Bi-linear interpolation scheme used with GDAS gridded products.
        /// </summary>
        /// <param name="met_grid"></param>
        /// <param name="lines"></param>
        /// <param name="samples"></param>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        private float blint_met(float[,] met_grid,int lines, int samples, float lon, float lat)
        {
            //未实现，插值算法没看懂
            float met_sfctmp = -999.0f;
            int ret = 0,line1,sammple1;
            if (lines <0||lines>180)
                ret = -1;
            if (samples < 0 || lines > 359)
                ret = -1;
            if (ret ==0)
            {
                bool y_interp = true;
                float idi = lon - samples;
                if (idi>=0)
                {
                    line1 = lines + 1;
                } 
                else
                {
                }
            }

            return met_sfctmp;

        }

        /// <summary>
        /// Compute saturation vapor pressure(饱和水蒸气压) over water for a given temperature.
        /// </summary>
        /// <param name="Temp"></param>Temperature (K)
        /// <returns></returns>Saturation vapor pressure over water (mb),饱和水蒸气压
        private double  ppv(float Temp)
        {
            double e00 = 611.21;
            double t00 = 273.16;
            double ti = t00 - 23.0;
            //Saturation vapour pressure
            double esw = e00 * Math.Exp(17.502 * (Temp - t00) / (Temp - 32.19));
            double esi = e00 * Math.Exp(22.587 * (Temp - t00) / (Temp + 0.7));
            double ppv=0;
            if (Temp>t00)
            {
                ppv= esw/100;//Conversion from [Pascal] to [mb],/100
            }
            else if (Temp > ti && Temp<=t00)
            {
                double tt = esi + (esw - esi) * ((Temp - ti) / (t00 - ti)) * ((Temp - ti) / (t00 - ti));
                ppv= tt / 100;
            } 
            else if(Temp<=ti)
            {
                ppv= esi/100;
            }
            return ppv;
        }

        /// <summary>
        /// 读取NCEP辅助文件gdas1.PGrbF00.020430.00z,
        /// NCEP met grid and NCEP ozone
        /// </summary>
        /// <param name="Lon_center"></param>
        /// <param name="Lat_center"></param>
        /// <param name="anc_met_lun"></param>
        /// <param name="ozone_lun"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        /// <param name="?"></param>
        private void DB_READ_ANC_DATA(float Lon_center, float Lat_center, string anc_met_lun, string ozone_lun, double sfctmp, double ugrd, double vgrd, double pwat, double ozone)
        {

        }

        private float[] GEOLOC_ANGLE(int Iscan,int START_1KM,float[,]Lat,float[,]Lon,float[,]SatZen,float[,]SatAz,float[,]SolZen,float[,]SolAz,float[,]Height)
        {
            float Lat_center, Lon_center, Lon_Min, Lon_Max, sumdif,diff;
            float[] Lon_4 = new float[4];//中心四点经度
            float MTHET0, MTHET, MPHI0, MPHI, MDPHI=0, MSCATT, MHGHT=0;//
            int IX1, IX2, IY1, IY2;
            IY1 = START_1KM + 4;//中心左列
            IY2 = START_1KM + 5;//中心右列
            IX1 = Iscan * 10 + 4;//中心上行
            IX2 = Iscan * 10 + 5;//中心下行
            Lon_4[0] =Lon[IX1,IY1];
            Lon_4[1] = Lon[IX2, IY1];
            Lon_4[2] = Lon[IX1, IY2];
            Lon_4[3] = Lon[IX2, IY2];
            Lon_Min = Lon_4[0];
            Lon_Max = Lon_4[0];
            for (int i = 1; i < 4;i++ )
            {
                if (Lon_4[i] < Lon_Min)
                    Lon_Min = Lon_4[i];
                if (Lon_4[i] > Lon_Max)
                    Lon_Max = Lon_4[i];
            }
            if (Lon_Max < -180.0 || Lon_Min < -180.0)
            {
                Lon_center=-999.0f;
                Lat_center=-999.0f;
                MTHET0=-99.99f;
                MTHET=-99.99f;
                MPHI0=-99.99f;
                MPHI=-99.99f;
                MSCATT = -99.99f;
            }
            else//Otherwise, check for other condisitons that may contain fill value
            {
                sumdif = 0;
                for (int i = 1; i < 4;i++ )
                {
                    diff = Lon_4[i] - Lon_4[0];
                    if (diff > 180)
                        diff = diff - 360;
                    if (diff < -180)
                        diff = 360 + diff;
                    sumdif = sumdif + diff;
                }
                Lon_center = Lon_4[0] + sumdif / 4.0f;//相当于平均值
                if (Lon_center > 180)
                    Lon_center = Lon_center - 360;
                if(Lon_center<-180)
                    Lon_center = Lon_center + 360;
                Lat_center = (Lat[IX1, IY1] + Lat[IX2, IY1] + Lat[IX1, IY1] + Lat[IX2, IY2]) / 4.0f;
                MTHET0 = (SolZen[IX1, IY1] + SolZen[IX2, IY1] + SolZen[IX1, IY1] + SolZen[IX2, IY2]) / 4.0f;
                MTHET = (SatZen[IX1, IY1] + SatZen[IX2, IY1] + SatZen[IX1, IY1] + SatZen[IX2, IY2]) / 4.0f;
                MPHI0 = (SolAz[IX1, IY1] + SolAz[IX2, IY1] + SolAz[IX1, IY1] + SolAz[IX2, IY2]) / 4.0f;
                MHGHT = (Height[IX1, IY1] + Height[IX2, IY1] + Height[IX1, IY1] + Height[IX2, IY2]) / 4000.0f;
                MPHI = (SatAz[IX1, IY1] + SatAz[IX2, IY1] + SatAz[IX1, IY1] + SatAz[IX2, IY2]) / 4.0f;
                MDPHI = CalMDPHI(MPHI0,MPHI);
                if (MTHET0 > 0 && MTHET > 0 && MDPHI > 0)
                {
                    MSCATT = (float)(-Math.Cos(MTHET0 * DTR) * Math.Cos(MTHET * DTR) + Math.Sin(MTHET0 * DTR) * Math.Sin(MTHET * DTR) * Math.Cos(MDPHI * DTR));
                    MSCATT = (float)(Math.Acos(MSCATT) * RTD);
                }
                else
                    MSCATT = -99.99f;
            }
            List<float> angle = new List<float>();
            angle.Add(Lon_center);
            angle.Add(Lat_center);
            angle.Add(MTHET0);
            angle.Add(MTHET);
            angle.Add(MPHI0);
            angle.Add(MPHI);
            angle.Add(MDPHI);
            angle.Add(MSCATT);
            angle.Add(MHGHT);
            return angle.ToArray();
        }

        /// <summary>
        /// 计算相对方位角
        /// </summary>
        /// <param name="phi0"></param>
        /// <param name="phi"></param>
        /// <returns></returns>
        private float  CalMDPHI(float MPHI0,float MPHI)
        {
            float MDPHI = Math.Abs(MPHI0 - MPHI - 180.0f);
            if (MDPHI > 360)
            {
                return (MDPHI % 360.0f);
            }
            else if (MDPHI > 180)
            {
                return (360.0f - MDPHI);
            }
            return MDPHI;
        }


        /// <summary>
        /// 设置10×10km bin的3种分辨率的计算开始、终止的像元位置
        /// </summary>
        /// <param name="IDATA"></param>
        /// <param name="START_500"></param>
        /// <param name="END_500"></param>
        /// <param name="START_250"></param>
        /// <param name="END_250"></param>
        /// <param name="START_1KM"></param>
        /// <param name="END_1KM"></param>
        private void SET_INDEX(int IDATA,out int[] index)
        {
            index =new int[6];
            index[0] = IDATA * 10;//START_1KM
            index[1] = index[0] + 9;//END_1KM
            index[2] = IDATA * 20;//START_500
            index[3] = index[2] + 19;//END_500
            index[4] = IDATA * 40;//START_250
            index[5] = index[4] + 39;//END_250
        }

        private void CldMsk_Land(int ILINE, int ISWATH, float[,] Refl_3, float[,] Refl_26, out bool[,] CldMsk_1km, out bool[,] CldMsk_hkm, out bool[,] CldMsk_qkm, out int[,] Land_CLDMSK_forfraction)
        {
            double cloud_threhold_land47_1 = 0.0025;
            double cloud_threhold_land47_2 = 0.4;
            double THRHLD1380_1=0.003;
            double THRHLD1380_2 = 0.01;
            CldMsk_1km = new bool[ILINE, ISWATH];
            int[,] Quality_cirrus = new int[ILINE, ISWATH];
            Land_CLDMSK_forfraction = new int[ILINE, ISWATH];
            CldMsk_hkm = new bool[2 * ILINE, 2 * ISWATH];
            CldMsk_qkm = new bool[4 * ILINE, 4 * ISWATH];
            int l,p,l1, p1,l2,p2,l4,p4;
            int N = 0;
            #region 初始化,是否必要？初始值为0
            for (l1 = 0; l1 < ILINE; l1++)
            {
                for (p1 = 0; p1 < ISWATH; p1++)
                {
                    CldMsk_1km[l1, p1] = false;
                    Quality_cirrus[l1, p1] = 0;
                    Land_CLDMSK_forfraction[l1, p1] = 0;
                }
            }
            for (l2 = 0; l2 < 2 * ILINE; l2++)
            {
                for (p2 = 0; p2 < 2 * ISWATH; p2++)
                {
                    CldMsk_hkm[l2, p2] = false;
                }
            }
            for (l4 = 0; l4 < 4 * ILINE; l4++)
            {
                for (p4 = 0; p4 < 4 * ISWATH; p4++)
                {
                    CldMsk_qkm[l4, p4] = false;
                }
            }
            #endregion
            //Cloud mask based upon spatial variability of 0.47 micron 500 m resolution reflectance data
            ////调用CldMsk_3by3_HKM函数
            CldMsk_3by3_HKM(ILINE, ISWATH, Refl_3, cloud_threhold_land47_1, out CldMsk_hkm);
            //reflactance test for 0.47 um
            for (l2 = 0; l2 < 2 * ILINE; l2++)
            {
                for (p2 = 0; p2 < 2 * ISWATH; p2++)
                {
                    if (Refl_3[l2, p2] > cloud_threhold_land47_2)
                        CldMsk_hkm[l2, p2] = false;
                }
            }
            List<int[,]> ls =new List<int[,]>();
            ////调用CldMsk_3by3_1KM函数
            ls = CldMsk_3by3_1KM(ILINE, ISWATH, Refl_26, THRHLD1380_1, out CldMsk_1km);
            //CldMsk_1km =ls[0];
            Quality_cirrus = ls[1];
            Land_CLDMSK_forfraction = ls[2];
            for (l1 = 0; l1 < ILINE; l1++)
            { 
                for (p1 = 0; p1 <ISWATH; p1++)
                {
                    N = 0;
                    for (l = 0; l < 2;l++ )
                    {
                        for (p = 0; p < 2;p++ )
                        {
                            l2 = l1 * 2 + l;
                            p2 = p1 * 2 + p;
                            if (CldMsk_1km[l1, p1] == true && CldMsk_hkm[l2, p2] == true)
                                CldMsk_hkm[l2, p2] = true;
                            else
                                CldMsk_hkm[l2, p2] = false;
                            if (CldMsk_hkm[l2, p2] == true)
                                N++;
                        }
                    }
                    //N=4 to overwrite 1.38 micro channel cloud screening results
                    if (N==4)
                    {
                        for (l = 0; l < 4; l++)
                        {
                            for (p = 0; p < 4; p++)
                            {
                                l4 = l1 * 4 + l;
                                p4 = p1 * 4 + p;
                                CldMsk_qkm[l4, p4] = true;
                            }
                        }
                    }
                }
            }
        }

        private void CldMsk_3by3_HKM(int ILINE, int ISWATH, float[,] REFHKM, double THRHLD, out bool[,] CLDMSK)
        {
            CLDMSK = new bool[ILINE * 2, ISWATH * 2];//Cloud mask
            double[] RMED = new double[ISWATH * 2];//Mean
            double[] RMEDSQ = new double[ISWATH * 2];//Standard deviation
            int[] NROWS = new int[ISWATH * 2];
            int p;
            double std, var;
            # region 初始化
            for (p = 0; p < ISWATH;p++ )
            {
                NROWS[p] = 0;
                RMED[p] = 0;
                RMEDSQ[p] = 0;
            }
            #endregion
            //Checking 500 meter resolution
            int NX = 0,IX,JY,JX;
            for (IX = 1; IX < ILINE * 2 - 2; IX++)//从第2行到倒数第二行
            {
                for (JX = IX - 1; JX < IX + 1;JX++ )//3行
                {
                    NX = NX + 1;
                    for (JY = 1; JY < ISWATH * 2 - 2;JY++ )
                    {
                        NROWS[JY]++;
                        RMED[JY] += REFHKM[JX, JY] + REFHKM[JX, JY-1] + REFHKM[JX, JY+1];//3列
                        RMEDSQ[JY] += REFHKM[JX, JY] * REFHKM[JX, JY] + REFHKM[JX, JY - 1] * REFHKM[JX, JY - 1] + REFHKM[JX, JY + 1] * REFHKM[JX, JY + 1];//3列
                        #region 判断是否满足条件
                        if (NX == 3)//make clear determination where possible and re-initialize work array elements
                        {
                            if (NROWS[JY] == 3)//make clear/cloud determination only when all 9 pixels in 3x3 array are valid
                            {
                                var=9.0/8.0*(RMEDSQ[JY]/9.0-RMED[JY]*RMED[JY]/81.0);
                                if (var>0.0)
                                {
                                    std =Math.Sqrt(var);
                                    std = std * (RMED[JY] / 3.0);//New definitation
                                    if (std<THRHLD)
                                    {
                                        CLDMSK[IX,JY]=true;
                                    }
                                }
                            }
                            NROWS[JY] = 0;
                            RMED[JY] = 0.0;
                            RMEDSQ[JY] = 0.0;
                        }
                        #endregion
                    }
                }
                NX = 0;
            }
            for (JY = 0; JY < ISWATH * 2;JY++ )//边缘行
            {
                CLDMSK[0, JY] = CLDMSK[1, JY];//第一行等于第二行
                CLDMSK[ILINE * 2 - 1, JY] = CLDMSK[ILINE * 2 - 2, JY];//倒数第一行等于倒数第二行
            }
            for (JX = 0; JX < ILINE * 2;JX++ )//边缘列
            {
                CLDMSK[JX, 0] = CLDMSK[JX, 1];//第一列等于第二列
                CLDMSK[JX, ISWATH * 2 - 1] = CLDMSK[JX, ISWATH * 2 - 2];//倒数第一列等于倒数第二列
            }

        }

        private List<int[,]> CldMsk_3by3_1KM(int ILINE, int ISWATH, float[,] REF1KM, double THRHLD1, out bool[,] CLDMSK)
        {
            List<int[,]> ls =new List<int[,]>();
            CLDMSK = new bool [ILINE , ISWATH ];//Cloud mask
            int[,]Land_CLDMSK_forfraction = new int[ILINE, ISWATH];
            int[,]Quality_cirrus = new int[ILINE, ISWATH];
            double[] RMED = new double[ISWATH ];//Mean
            double[] RMEDSQ = new double[ISWATH];//Standard deviation
            double std, var;
            int NX = 0, IX, JY, JX;
            for (IX = 1; IX < ILINE  - 2; IX++)//从第2行到倒数第二行
            {
                for (JX = IX - 1; JX < IX + 1; JX++)//3行
                {
                    NX = NX + 1;
                    for (JY = 1; JY < ISWATH * 2 - 2; JY++)
                    {
                        RMED[JY] += REF1KM[JX, JY] + REF1KM[JX, JY - 1] + REF1KM[JX, JY + 1];//3列
                        RMEDSQ[JY] += REF1KM[JX, JY] * REF1KM[JX, JY] + REF1KM[JX, JY - 1] * REF1KM[JX, JY - 1] + REF1KM[JX, JY + 1] * REF1KM[JX, JY + 1];//3列
                        #region 判断是否满足条件
                        if (NX == 3)//make clear determination where possible and re-initialize work array elements
                        {
                                var = 9.0 / 8.0 * (RMEDSQ[JY] / 9.0 - RMED[JY] * RMED[JY] / 81.0);
                                if (var > 0.0)
                                {
                                    std = Math.Sqrt(var);
                                    if (std < THRHLD1)
                                    {
                                        CLDMSK[IX, JY] = true;
                                    }
                                }                           
                            RMED[JY] = 0.0;
                            RMEDSQ[JY] = 0.0;
                        }
                        #endregion
                    }
                }
                NX = 0;
            }
            for (JY = 0; JY < ISWATH ; JY++)//边缘行
            {
                CLDMSK[0, JY] = CLDMSK[1, JY];//第一行等于第二行
                CLDMSK[ILINE - 1, JY] = CLDMSK[ILINE - 2, JY];//倒数第一行等于倒数第二行
            }
            for (JX = 0; JX < ILINE ; JX++)//边缘列
            {
                CLDMSK[JX, 0] = CLDMSK[JX, 1];//第一列等于第二列
                CLDMSK[JX, ISWATH - 1] = CLDMSK[JX, ISWATH - 2];//倒数第一列等于倒数第二列
            }
            # region //Checking for 1.38 micron reflectance
            for (JX = 0; JX < ILINE;JX++ )
            {
                for (JY = 0; JY < ISWATH;JY++ )
                {
                    Land_CLDMSK_forfraction[JX, JY] = (CLDMSK[JX, JY]?1:0);
                    if (REF1KM[JX, JY] > 0.025)
                    {
                        CLDMSK[JX, JY] = false;
                        Land_CLDMSK_forfraction[JX, JY] = (CLDMSK[JX, JY] ? 1 : 0); ;//save for cloud fraction
                    }
                    //if Variabily cloud mask says cloud free and smoke or cirrus reduce quality
                    else if (CLDMSK[JX, JY] == true && REF1KM[JX, JY] > 0.01 && REF1KM[JX, JY] <= 0.025)
                    {
                        //cloud free  may be cirrus  or smoke , will be retrived but put a quality value 
                        Quality_cirrus[JX, JY] = 1;
                    }
                }
            }
            # endregion
            //ls.Add(CLDMSK);
            ls.Add(Land_CLDMSK_forfraction);
            ls.Add(Quality_cirrus);
            return ls;
        }


        private int Bits2Int(BitArray bits,int start,int end)
        {
            bool[] bit2 = new bool[2] { bits[start], bits[end] };//应注意顺序，大段序还是小段序
            //bit2[0] = bits[1];
            //bit2[1] = bits[2];
            BitArray te = new BitArray(bit2);
            var  result = new int[1];
            te.CopyTo(result, 0);
            return result[0];
        }

        private List<int[,]> Get_CldMsk_bitInfo(byte[, ,] Cloud, int bandCount, int ls, int le, int ILINE,int ISWATH)
        {
            List<int[,]> list = new List<int[,]>();
            int[,] flag = new int[ILINE, ISWATH];
            int bytei = 0;

            

            return list;
        }

        private unsafe float[,] ReadFloatBand(IRasterDataProvider dataPrd, int bandNO)
        {
            IRasterBand bands = dataPrd.GetRasterBand(bandNO);
            if (bands != null)
            {
                int height = dataPrd.Height;
                int width = dataPrd.Width;
                float[,] bandraster = new float[height,width];
                try
                {
                    float[] buffer = new float[width * height];
                    fixed (float* ptr = buffer)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        bands.Read(0, 0, width, height, bufferPtr, dataPrd.DataType, width, height);
                    }
                    int k = 0;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            bandraster[i, j] = buffer[k];
                            k++;
                        }
                    }
                    return bandraster;
                }
                catch (System.Exception ex)
                {

                }
            }
            return null;
        }

        private unsafe byte[,,] ReadByteBand(IRasterDataProvider dataPrd,int NO)
        {
            int height = dataPrd.Height;//行
            int width = dataPrd.Width;//列
            byte[, ,] data = new byte[NO, height, width];
            for (int bandNO = 0; bandNO < NO; bandNO++)
            {
                IRasterBand bands = dataPrd.GetRasterBand(bandNO+1);
                if (bands != null)
                {
                    try
                    {
                        byte[] buffer = new byte[width * height];
                        fixed (byte* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            bands.Read(0, 0, width, height, bufferPtr, dataPrd.DataType, width, height);
                        }
                        int k = 0,i,j;
                        for (i = 0; i < height; i++)
                        {
                            for (j = 0; j < width; j++)
                            {
                                data[bandNO,i, j] = buffer[k];//应注意二维行列与一维的对应关系
                                k++;
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        return null;
                    }
                }
            }
            return data;
        }

        private bool CheckEnvironment()
        {
            if (string.IsNullOrEmpty(_MODISdataDir) || !Directory.Exists(_MODISdataDir))
            {
                MessageBox.Show("MODIS数据路径设置不正确！", "信息提示", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(_MODISAncidataDir) || !Directory.Exists(_MODISAncidataDir))
            {
                MessageBox.Show("MODIS辅助产品路径设置不正确！", "信息提示", MessageBoxButtons.OK);
                return false;
            }
            if (string.IsNullOrEmpty(_ProductOutDir))
            {
                MessageBox.Show("MODIS气溶胶反演输出数据路径没有设置");
                return false;
            }
            if (!Directory.Exists(_ProductOutDir))
            {
                Directory.CreateDirectory(_ProductOutDir);
            }
            return true;
        }

        private bool CheckModisDataFile()
        {
            _keywords = string.Empty;
            _modisDataFnames.Clear();
            _satelliteID = SatelliteID.Null;
            int year = 0;
            int days = 0;
            foreach (var item in Directory.GetFiles(_MODISdataDir, "*.hdf", SearchOption.TopDirectoryOnly))
            {
                if (_satelliteID == SatelliteID.Null)
                {
                    if (item.Contains("MOD"))
                        _satelliteID = SatelliteID.Terra;
                    else if (item.Contains("MYD"))
                        _satelliteID = SatelliteID.Aqua;
                    else
                        _satelliteID =SatelliteID.Null;
                }
                if (_satelliteID != SatelliteID.Null&&string.IsNullOrEmpty(_keywords))
                {
                    //示例
                    //文件名：MOD03.A2013014.0300.005.2013014094104.hdf
                    //filename =MOD03.A2013014.0300.005.2013014094104
                    //keyWord =A2013014.0300.005
                    string filename = Path.GetFileNameWithoutExtension(item);
                    _keywords = filename.Substring(filename.IndexOf(".") + 1, filename.LastIndexOf(".") - filename.IndexOf(".") - 1);
                    days = int.Parse(_keywords.Substring(5, _keywords.IndexOf(".") - 5));
                    year = int.Parse(_keywords.Substring(1, 4));
                    DateTime dt = new DateTime(year, 1, 1);
                    _datatime = dt.AddDays(days - 1);
                }

                if ((_satelliteID == SatelliteID.Terra && item.Contains("MOD"))
                    || (_satelliteID == SatelliteID.Aqua && item.Contains("MYD")))
                {
                    if (!_modisDataFnames.Contains(item))
                        _modisDataFnames.Add(item);
                }
                else
                {
                    continue;
                }
            }
            if (_modisDataFnames.Count < 4)
            {
                MessageBox.Show("MODIS数据产品不完整，应该包括！", "信息提示", MessageBoxButtons.OK);
                return false;
            }
            return true;
        }

        private string GetFileFlag(string filename)
        {
            return filename.Substring(0, filename.IndexOf("."));
        }

        private void RunBATProcess(string[] items)
        {
            
            Process myprocess = new Process();
            string arg = string.Empty;

            for (int i = 1; i < items.Length; i++)
            {
                arg += "\"" + items[i] + "\" ";
            }
            ProcessStartInfo startInfo = new ProcessStartInfo(items[0], arg);
            startInfo.CreateNoWindow = true;
            myprocess.StartInfo = startInfo;
            myprocess.StartInfo.UseShellExecute = false;
            myprocess.StartInfo.WorkingDirectory = Path.GetDirectoryName(items[0]);
            myprocess.Start();
            myprocess.WaitForExit();
            progressBar1.Value++;
            this.Invalidate();
        }

        private void RunMyProcess(string filename, int index, string imgFilename, string hdrFilename)
        {
            if (!Directory.Exists(Path.GetDirectoryName(imgFilename)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(imgFilename));
            }
            Process myprocess = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo(filename, "\""
                    + _modisDataFnames[index] + "\" \""
                    + _satelliteID.ToString() + "\" \""
                    + imgFilename + "\" \"" + hdrFilename + "\"");
            startInfo.CreateNoWindow = true;
            myprocess.StartInfo = startInfo;
            myprocess.StartInfo.UseShellExecute = false;
            myprocess.Start();
            myprocess.WaitForExit();
            progressBar1.Value++;
            this.Invalidate();
        }

        private void CopyProductDataToOutputDir(string productTempDir, string productOutputDir)
        {
            foreach (var item in Directory.GetFiles(productTempDir))
            {
                if (item.Contains("MOD04"))
                    File.Copy(item, Path.Combine(productOutputDir, Path.GetFileName(item)));
            }
            progressBar1.Value++;
            this.Invalidate();
        }

        private string GetInputFilename(string arg)
        {
            string[] inputFilename = Directory.GetFiles(_MODISAncidataDir, arg);
            if (inputFilename == null || inputFilename.Length == 0)
                return string.Empty;
            else
                return inputFilename[0];
        }

        private int GetKeywordIndex(string key)
        {
            for (int i = 0; i < _modisDataFnames.Count; i++)
            {
                if (_modisDataFnames[i].Contains(key))
                {
                    return i;
                }
            }
            return 0;
        }
        
        private void DelProductData(string productTempDir)
        {
            if (!Directory.Exists(productTempDir))
                return;
            foreach (var item in Directory.GetFiles(productTempDir))
            {
                File.Delete(item);
            }
            //Directory.Delete(productTempDir);
            progressBar1.Value++;
            this.Invalidate();
        }

        /// <summary>
        /// 用于多幅影像时
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="n"></param>
        /// <param name="k"></param>
        private void GetKeywordIndex(string keywords, out int n,out int k)
        {
            List<string[]> filelist =new List<string[]>();
            n = -1; k = -1;
            for (int i = 0; i < filelist.Count; i++)
            {
                if (filelist[i].Contains(keywords))
                   n = i;break;
            }
            if (n==-1)
                return;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnOpenPrdDir_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_ProductOutDir))
            {
                System.Diagnostics.Process.Start("Explorer.exe");
            }
            else
            {
                MessageBox.Show("无法打开文件位置，请确定输出文件路径正确！");
            }
        }
    }
}
