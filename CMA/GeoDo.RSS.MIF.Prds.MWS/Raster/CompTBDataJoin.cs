using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.IO;
using GeoDo.Project;
using GeoDo.FileProject;
using GeoDo.RasterProject;
using System.Text.RegularExpressions;
using GeoDo.RSS.Core.DF;
using GeoDo.MicroWaveSnow.FYJoin;

namespace GeoDo.RSS.MIF.Prds.MWS
{
    public class CompTBDataJoin
    {

        private static Regex DataReg = new Regex(@"(?<year>\d{4})(?<month>\d{2})(?<day>\d{2})", RegexOptions.Compiled);
        public CompTBDataJoin()
        { 

        }
        public string[] CompJoinData(string[] filenames, string chosefileInfo)
        {
            //infos[0]: orbit
            //infos[1]:areaNams
            //infos[2]:radio 拼或者不拼
            //infos[3] + infos[4]: 起止时间
            string[] infos = chosefileInfo.Split(new char[] { ':' });
            List<string> joinfiles = new List<string>();
            string[] files = filenames;
            //在这里开始循环调用出数据，拆开区域名称
            infos[1] = infos[1].Substring( 0,infos[1].Length - 1);
            string[] areas = infos[1].Split(new char[] { ',' });
            foreach (string area in areas)
            {
                string refile = Processfiles(files, area);
                if(File.Exists(refile))
                    joinfiles.Add(refile);
            }
            return joinfiles.ToArray();
        }
        /// <summary>
        /// 投影和拼接
        /// </summary>
        /// <param name="files"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        private string Processfiles(string[] files, string area)
        {
            float minX = 0.00f;
            float maxX = 0.00f;
            float minY = 0.00f;
            float maxY = 0.00f;
            ClipSNWParaData getEvp = new ClipSNWParaData();
            float[] evp = getEvp.GetEvelope( files[0],area);
            minX = evp[0] - 2.0f;// 72.0f;
            maxX = evp[1] + 2.0f;// 142.0f;
            minY = evp[2] - 2.0f;// 16.0f;
            maxY = evp[3] + 2.0f;// 56.0f;
            FY3MWRIJoin Join = new FY3MWRIJoin();
            string outJoinfilename = Path.GetDirectoryName(files[0]) + "\\Prj\\" + Path.GetFileName(files[0]).Remove(Path.GetFileName(files[0]).LastIndexOf(".")) + "_区域" + area + "_" + "moscia" + ".ldf";
            FY3L2L3FileProjector projector = new FY3L2L3FileProjector();
            FY3L2L3FilePrjSettings prjSetting = new FY3L2L3FilePrjSettings();
            prjSetting.ExtArgs = new object[] { "360" };
            prjSetting.OutFormat = "LDF";
            prjSetting.OutResolutionX = 0.1f;
            prjSetting.OutResolutionY = 0.1f;
            prjSetting.OutEnvelope = new PrjEnvelope(minX, maxX, minY, maxY);

            foreach (string file in files)
            {
                try
                {
                    string outPrjfile = Path.GetDirectoryName(file) + "\\Prj\\" + Path.GetFileName(file).Remove(Path.GetFileName(file).LastIndexOf(".")) + "_区域" + area + ".LDF";
                    prjSetting.OutPathAndFileName = outPrjfile;
                    IRasterDataProvider mainRaster = null;
                    IRasterDataProvider locationRaster = null;
                    if (file.Contains("FY3C"))
                    {
                        string[] openArgs = new string[] { "datasets=" + "Data/EARTH_OBSERVE_BT_10_to_89GHz" };
                        mainRaster = RasterDataDriver.Open(file, openArgs) as IRasterDataProvider;
                        string[] locationArgs = new string[] { "datasets=" + "Data/Longitude,Data/Latitude", "geodatasets=" + "Data/Longitude,Data/Latitude" };//可设成配置参数
                        locationRaster = RasterDataDriver.Open(file, locationArgs) as IRasterDataProvider;
                    }
                    else
                    {
                        string[] openArgs = new string[] { "datasets=" + "EARTH_OBSERVE_BT_10_to_89GHz" };
                        mainRaster = RasterDataDriver.Open(file, openArgs) as IRasterDataProvider;
                        string[] locationArgs = new string[] { "datasets=" + "Longitude,Latitude", "geodatasets=" + "Longitude,Latitude" };//可设成配置参数
                        locationRaster = RasterDataDriver.Open(file, locationArgs) as IRasterDataProvider;
                    }
                    prjSetting.LocationFile = locationRaster;
                    projector.Project(mainRaster, prjSetting, SpatialReference.GetDefault(), null);
                    string[] joinfile = new string[] { outPrjfile };
                    if (!File.Exists(outJoinfilename))
                    {
                        string access = "create";
                        Join.MicroWaveDataJoint(joinfile, 0.1f, outJoinfilename, access);
                    }
                    else
                    {
                        string access = "update";
                        Join.MicroWaveDataJoint(joinfile, 0.1f, outJoinfilename, access);
                    }
                }
                catch (Exception ex)
                {
                    //throw new FileNotFoundException(ex.Message);
                    Console.WriteLine(ex.Message);
                }
                finally
                {

                }
            }

            return outJoinfilename;
        }
     
    }
}
