using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.HDF4.Cloudsat;
using GeoDo.FileProject;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class ConnectCloudSAT2Region
    {
        private Dictionary<string, long> _file2ID;
        private Dictionary<long, PrjEnvelopeItem> _allRegions;
        private ConnectMySqlCloud _con = new ConnectMySqlCloud();

        public ConnectCloudSAT2Region(Dictionary<string, long> file2ID, Dictionary<long, PrjEnvelopeItem> allRegions)
        {
            _file2ID = file2ID;
            _allRegions = allRegions;
        }

        public void Compute(Action<string> ProBack)
        {
            float[] lats = null, lons = null;
            double minX, maxX, minY, maxY;
            foreach (long regionid in _allRegions.Keys)
            {

                PrjEnvelopeItem item =_allRegions[regionid];
                minX=item.PrjEnvelope.MinX;//10
                maxX=item.PrjEnvelope.MaxX;//60
                minY=item.PrjEnvelope.MinY;//65
                maxY = item.PrjEnvelope.MaxY;//145
                ProBack("\t开始进行CloudSAT数据与" + item.Name+"区域的关联计算...");
                Dictionary<Int32, double> granuleNOPct = new Dictionary<Int32,double >();
                Dictionary<long,Double> filePct =new Dictionary<long,Double>();
                Dictionary<Int32, Int32> granuleNOAllCount = new Dictionary<Int32, Int32>();
                string[] fNameParts;
                //string fName;
                int granuleNO;
                float uplat, uplon, downlat, downlon;
                foreach (string file in _file2ID.Keys)
                {
                    try
                    {
                        //2007101052404_05066_CS_MODIS-AUX_GRANULE_P_R04_E02.hdf
                        string fname = Path.GetFileName(file);
                        ProBack("\t\t开始进行" + file + "与" + item.Name + "区域的关联计算...");
                        fNameParts = Path.GetFileNameWithoutExtension(file).Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
                        if (fNameParts.Length != 8)
                            continue;
                        if (fNameParts[2] != "CS")
                            continue;
                        granuleNO = int.Parse(fNameParts[1]);
                        if (!granuleNOPct.ContainsKey(granuleNO) && fNameParts[3].ToUpper() != "MODIS-AUX")//每个轨道号数据具有同一个覆盖度
                        {
                            int inCount = 0;
                            CloudsatDataProvider locRst = null;
                            ProBack("\t\t\t读取" + fname + "的经纬度数据集...");
                            using (locRst = GeoDataDriver.Open(file) as CloudsatDataProvider)
                            {
                                lats = locRst.ReadVdata("Latitude", null, 0, locRst.Height) as float[];
                                lons = locRst.ReadVdata("Longitude", null, 0, locRst.Height) as float[];
                            }
                            ProBack("\t\t\t开始计算" + fname + "与区域的相交...");
                            int firstpt = -1, lastpt = -1;
                            for (int i = 0; i < lats.Length; i++)
                            {
                                if (lats[i] >= minY && lats[i] <= maxY && lons[i] >= minX && lons[i] <= maxX)
                                {
                                    if (firstpt == -1)
                                        firstpt = i;
                                    lastpt = i;
                                    inCount++;
                                }
                            }
                            if (firstpt != -1)//存在覆盖该区域的点
                            {
                                #region 计算南点和北点的经纬度
                                if (firstpt != lastpt)//多于一个点
                                {
                                    if (lats[firstpt] > lats[lastpt])
                                    {
                                        uplat = lats[firstpt];
                                        uplon = lons[firstpt];
                                        downlat = lats[lastpt];
                                        downlon = lons[lastpt];
                                    }
                                    else
                                    {
                                        uplat = lats[lastpt];
                                        uplon = lons[lastpt];
                                        downlat = lats[firstpt];
                                        downlon = lons[firstpt];
                                    }
                                }
                                else
                                {
                                    uplat = lats[firstpt];
                                    uplon = lons[firstpt];
                                    downlat = lats[firstpt];
                                    downlon = lons[firstpt];
                                }
                                #endregion
                                #region 入库
                                //granuleNO,regionid,uplat,uplon,downlat,downlon,firstpt,lastpt
                                if (!_con.IshasRecord("cp_cloudsatinregionlatlon_tb", "GranuleNumber", granuleNO))
                                    _con.InsertCloudsatGranule2Region(granuleNO, regionid, uplat, uplon, downlat, downlon, firstpt, lastpt);
                                ProBack("\t\t\t将" + granuleNO + "起始范围计算结果入库成功！");
                                #endregion
                            }
                            granuleNOPct.Add(granuleNO, inCount / (lats.Length * 1.0) * 100.0);
                            granuleNOAllCount.Add(granuleNO, lats.Length);
                        }
                        if (granuleNOAllCount.ContainsKey(granuleNO))
                        {
                            if (!_con.IshasRecord("cp_cloudsat2region_tb", "CloudSatID", _file2ID[file]))
                            {
                                _con.InsertCloudsat2RegionTable(_file2ID[file], granuleNOAllCount[granuleNO], regionid, granuleNOPct[granuleNO]);
                            }
                            ProBack("\t\t将" + fname + "计算结果入库成功！");
                        }
                    }
                    catch (System.Exception ex)
                    {
                        ProBack("\t"+file+"处理出错！"+ex.Message);
                    }
                }
            }
        }

         class cloudsatF
        {
            public string filename;
            public double minX;
            public double maxX; 
            public double minY;
            public double maxY;
             public cloudsatF(string file,double minx,double maxx,double miny,double maxy)
            {
                filename = file;
                minX = minx;
                maxX = maxx;
                minY = miny;
                maxY = maxy;
            }
        }

        public static void GetCoverPtCount(object paras)
        {
            cloudsatF cloudsatFg =paras as cloudsatF;
            string file = cloudsatFg.filename;
            double minX= cloudsatFg.minX;
            double maxX= cloudsatFg.maxX;
            double minY= cloudsatFg.minY;
            double maxY = cloudsatFg.maxY;            
            CloudsatDataProvider locRst = null;
            int inCount = 0;
            float[] lats, lons;
            using (locRst = GeoDataDriver.Open(file) as CloudsatDataProvider)
            {
                lats = locRst.ReadVdata("Latitude", null, 0, locRst.Height) as float[];
                lons = locRst.ReadVdata("Longitude", null, 0, locRst.Height) as float[];
            }
            for (int i = 0; i < lats.Length; i++)
            {
                if (lats[i] >= (double)minY && lats[i] <= (double)maxY && lons[i] >= (double)minY && lons[i] <= (double)maxY)
                {
                    inCount++;
                }
            }
            //return new int[] { lats.Length,inCount };
        }
    }
}
