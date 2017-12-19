using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.DF.MicapsData;
using CodeCell.AgileMap.Core;
using System.Xml.Linq;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RasterProject;

namespace GeoDo.RSS.MIF.Prds.CLD
{
    public class MicapsDataProcess
    {
        private static Dictionary<string, string> _fields2names = new Dictionary<string, string>();//name,identity
        private static Dictionary<string, int> _fields2Indexs = new Dictionary<string, int>();//name,index
        private static string _datatype = "CLDGroundObserveData";
        private static string DATA_CONFIG_DIR = AppDomain.CurrentDomain.BaseDirectory + @"\SystemData\MicapsDataConfig\MicapsDataDefine.xml";

        public MicapsDataProcess()
        {
            InitialArgs();
        }

        private void InitialArgs()
        {
            GetFieldsNames(GetDataTypeSetting(_datatype));
        }

        private static void GetFieldsNames(XElement dataSetting)
        {
            if (dataSetting == null)
                return;
            int fieldCount;
            if (Int32.TryParse(dataSetting.Attribute("fieldcount").Value, out fieldCount))
            {
                IEnumerable<XElement> items = dataSetting.Elements("Field");
                if (items == null || items.Count() < 1)
                    return;
                int index;
                foreach (XElement item in items)
                {
                    if (Int32.TryParse(item.Attribute("index").Value, out index))
                    {
                        if (index < fieldCount)
                        {
                            string identity = item.Attribute("identify").Value;
                            string name = item.Attribute("name").Value;
                            if (!string.IsNullOrEmpty(name) && !_fields2names.ContainsKey(name))
                            {
                                _fields2names.Add(name, identity);
                                _fields2Indexs.Add(name, index);
                            }
                        }
                    }
                }
                return;
            }
            return;
        }

        private static XElement GetDataTypeSetting(string dataTypeId)
        {
            if (!File.Exists(DATA_CONFIG_DIR))
                return null;
            if (string.IsNullOrEmpty(dataTypeId))
                return null;
            XElement root = XElement.Load(DATA_CONFIG_DIR);
            IEnumerable<XElement> items = root.Elements("DataDefine");
            if (items == null || items.Count() == 0)
                return null;
            foreach (XElement item in items)
            {
                if (item.Attribute("identify").Value == dataTypeId)
                    return item;
            }
            return null;
        }

        public string[] DisplayNames
        {
            get{return _fields2names.Keys.ToArray();}
        }

        public static string Identity(string name)
        {
            return _fields2names[name];
        }

        public static int Index(string name)
        {
            return _fields2Indexs[name];
        }

        public static long [] Index(string [] names)
        {
            List<long> indexs = new List<long>();
            foreach (string name in names)
            {
                indexs.Add(_fields2Indexs[name]);
            }
            return indexs.ToArray();
        }

        #region 读取Micaps数据
        public static double[][] GetMicapsData(string[] files, string[] fieldnames, Envelope envelope = null)
        {
            int height = fieldnames.Length;
            double[][] MicapsData = new double[height][];
            for (int i = 0; i < height; i++)
            {
                MicapsData[i] = GetMicapsData(files, fieldnames[i], envelope);
            }
            return MicapsData;
        }

        public static double[] GetMicapsData(string[] files, string fieldname, Envelope envelope = null)
        {
            int index = _fields2Indexs[fieldname];
            return GetMicapsData(files, index, null);
        }

        public static double[] GetMicapsData(string[] files, int index, string[] fillValuesStr, Envelope envelope = null)
        {
            List<double> valuelist = new List<double>();
            double value;
            foreach (string file in files)
            {
                Feature[] returnFeatures = GetMicapsFeatures(file, envelope);
                foreach (Feature fea in returnFeatures)
                {
                    string valuestr =fea.FieldValues[index];
                    if (fillValuesStr!=null&&fillValuesStr.Contains(valuestr))
                        continue;
                    if (Double.TryParse(valuestr, out value))
                    {
                        valuelist.Add(value);
                    }
                }
            }
            return valuelist.ToArray();
        }

        public static double[] GetMicapsData(string[] files, int index, string[] fillValuesStr, string min , string max , Envelope envelope = null)
        {
            List<double> valuelist = new List<double>();
            Double value;
            Double minval = Double.Parse(min);
            Double maxval = Double.Parse(max);
            foreach (string file in files)
            {
                Feature[] returnFeatures = GetMicapsFeatures(file, envelope);
                foreach (Feature fea in returnFeatures)
                {
                    string valuestr = fea.FieldValues[index];
                    if (fillValuesStr != null&&fillValuesStr.Contains(valuestr))
                        continue;
                    if (Double.TryParse(valuestr, out value))
                    {
                        if (value >= minval && value <= maxval)
                            valuelist.Add(value);
                    }
                }
            }
            return valuelist.ToArray();
        }

        private static Feature[] GetMicapsFeatures(string file, Envelope envelope = null)
        {
            MicapsDataReader dr = MicapsDataReaderFactory.GetVectorFeatureDataReader(file, new object[1] { _datatype }) as MicapsDataReader;
            if (envelope != null)
                return dr.GetFeatures(envelope);
            else
                return dr.Features;        
        }

        /// <summary>
        /// 两组micaps数据间进行相关系数计算时，返回两个匹配的站点数据的平均
        /// </summary>
        /// <param name="filesl"></param>
        /// <param name="indexl"></param>
        /// <param name="fillValuesStrL"></param>
        /// <param name="filesr"></param>
        /// <param name="indexr"></param>
        /// <param name="fillValuesStrr"></param>
        /// <param name="micDL"></param>
        /// <param name="micDR"></param>
        /// <param name="srL"></param>
        /// <param name="srR"></param>
        /// <param name="envelope"></param>
        public static void GetMicapsDataAvg(string[] filesl, int indexl, string[] fillValuesStrL, string[] filesr, int indexr, string[] fillValuesStrr, out double[] micDL, out double[] micDR, out long srL, out long srR, Envelope envelope = null)
        {
            micDL = null; micDR = null;
            srL = 0; srR = 0;
            Dictionary<string, ShapePoint> unionStationPosL;
            Dictionary<string, List<string>> unionStationValuesL;//站点编号，数据索引号，数据值
            GetUnionStationValues(filesl, indexl, fillValuesStrL, envelope, out unionStationPosL, out unionStationValuesL);
            if (unionStationPosL.Count < 1)
                throw new ArgumentException("指定的Micaps数据不包含站点数据！");
            Dictionary<string, ShapePoint> unionStationPosR;//站点编号, 位置
            Dictionary<string, List<string>> unionStationValuesR;//站点编号，数据索引号，数据值
            GetUnionStationValues(filesr, indexr, fillValuesStrr, envelope, out unionStationPosR, out unionStationValuesR);
            if (unionStationPosR.Count < 1)
                throw new ArgumentException("指定的Micaps数据不包含站点数据！");
            //判断两组站点的匹配
            List<string> matchedStationNO=new List<string>();
            foreach (string stationNO in unionStationPosL.Keys)
            {
                if (unionStationValuesR.ContainsKey(stationNO))
                    matchedStationNO.Add(stationNO);
            }
            List<double> micapsdatalistL = new List<double>();
            List<double> micapsdatalistR = new List<double>();
            foreach (string sta in matchedStationNO)
            {
                List<string> str = unionStationValuesL[sta];
                srL += str.Count;
                micapsdatalistL.Add(CalAvgbyStr(str));
                str = unionStationValuesR[sta];
                srR += str.Count;
                micapsdatalistR.Add(CalAvgbyStr(str));
            }
            micDL = micapsdatalistL.ToArray();
            micDR = micapsdatalistR.ToArray();
        }

        private static  double CalAvgbyStr(List<string> liststr)
        {
            double sum = 0;
            int count = 0;
            double staValue;
            foreach (string str in liststr)//list为所有站点非填充值的值的集合，
            {
                if (double.TryParse(str, out staValue))
                {
                    sum += staValue;
                    count++;
                }
            }
            if (count != 0)
                return (sum / count);//有效平均
            else
                return 0;//无效填0

        }
        #endregion

        #region 相关系数计算对应micaps点抽取栅格数据
        /// <summary>
        /// 相关系数计算对应micaps点抽取栅格数据,存在多个数据则该点平均
        /// </summary>
        /// <param name="rasterfiles"></param>
        /// <param name="bandNum"></param>
        /// <param name="fillValueStrRst"></param>
        /// <param name="micapsFiles"></param>
        /// <param name="mbandNo"></param>
        /// <param name="fillValuestr"></param>
        /// <param name="envelope"></param>
        /// <param name="rasterdata"></param>
        /// <param name="micapsdata"></param>
        /// <param name="scL"></param>
        /// <param name="scR"></param>
        public static void GetMatchedValues(string[] rasterfiles, int bandNum,string [] fillValueStrRst, string[] micapsFiles, int mbandNo,string [] fillValuestr, PrjEnvelope envelope, out double[] rasterdata, out double[] micapsdata,out long scL,out long scR)
        {
            rasterdata = null;
            micapsdata = null;
            scL = 0; scR = 0;
            Envelope dstEnv = GetInterectEnv(rasterfiles, envelope);
            if (dstEnv == null || dstEnv.Width == 0 || dstEnv.Height == 0)
                throw new ArgumentNullException("待分析区域为空,请确认自定义区域范围或数据文件范围！");
            #region 获取micaps数据
            Dictionary<string, ShapePoint> unionStationPos;
            Dictionary<string, List<string>> unionStationValues;//站点编号，数据索引号，数据值
            GetUnionStationValues(micapsFiles, mbandNo, fillValuestr, dstEnv, out unionStationPos, out unionStationValues);
            if (unionStationPos.Count < 1)
                throw new ArgumentException("指定的Micaps数据不包含站点数据！");
            double staValue;
            List<double> micapsdatalist = new List<double>();
            foreach (List<string> list in unionStationValues.Values)
            {
                double sum=0;
                int count=0;
                foreach (string str in list)//list为所有站点非填充值的值的集合，
                {
                    if (double.TryParse(str, out staValue) )
                    {
                        sum += staValue;
                        count++;
                        scR++;
                    }
                }
                if (count != 0)
                    micapsdatalist.Add(sum / count);//有效平均
                else
                    micapsdatalist.Add(0);//无效填0
            }
            micapsdata = micapsdatalist.ToArray();
            #endregion 
            #region 获取栅格对应点的数据
            int matchedcount =micapsdatalist.Count;
            Double[] fillValuesRst = null;
            fillValuesRst = CloudParaFileStatics.GetFillValues<Double>(fillValueStrRst, enumDataType.Double);
            rasterdata = new double[matchedcount];
            double[] summatchedRasterValues = new double[matchedcount];
            int[] sumRasterCounts = new int[matchedcount];
            foreach (string rasterf in rasterfiles)
            {
                rasterdata = GetMatchedRasterValues(rasterf, bandNum, unionStationPos.Values.ToArray());
                double alue;
                for (int i = 0; i < matchedcount; i++)
                {
                    alue = rasterdata[i];
                    if (fillValuesRst==null||!fillValuesRst.Contains(alue))
                    {
                        summatchedRasterValues[i] += alue;
                        sumRasterCounts[i]++;
                        scL++;
                    }
                }

            }
            for (int i = 0; i < matchedcount; i++)
            {
                if (sumRasterCounts[i] != 0)
                    rasterdata[i] = summatchedRasterValues[i] / sumRasterCounts[i];
                else
                    rasterdata[i] = 0;
            }
            #endregion
        }

        /// <summary>
        /// 获取多个micpas文件的站点并集，同时返回各个站点的某个类型的值
        /// </summary>
        /// <param name="mfiles"></param>
        /// <param name="mbandNo"></param>
        /// <param name="fillValuestr"></param>
        /// <param name="envelope"></param>
        /// <param name="unionStationPos"></param>
        /// <param name="unionStationValues"></param>
        private static void GetUnionStationValues(string[] mfiles, int mbandNo,string [] fillValuestr,Envelope envelope,  out Dictionary<string, ShapePoint> unionStationPos, out Dictionary<string, List<string>> unionStationValues)
        {
            int length = mfiles.Length;
            unionStationPos = new Dictionary<string, ShapePoint>();//站点编号，位置
            unionStationValues = new Dictionary<string, List<string>>();//站点编号，数据索引号位置处的多个数据值
            string stationValue;
            Feature[] singleFeatures;
            ShapePoint pt;
            string stationNo;
            for (int i = 0; i < mfiles.Length; i++)//foreach (string file in mfiles)
            {
                singleFeatures = GetMicapsFeatures(mfiles[i], envelope);
                foreach (Feature fea in singleFeatures)
                {
                    pt = fea.Geometry as ShapePoint;
                    stationNo = fea.FieldValues[0];//站点编号
                    stationValue = fea.FieldValues[mbandNo];
                    if (fillValuestr != null && fillValuestr.Contains(stationValue))
                        continue;
                    //判断条件：站名相同的条件下，判断pt的X/Y；站名不同，认为点不同
                    if (stationNo != "01009")
                    {
                        if (unionStationPos.ContainsKey(stationNo))
                        {
                            if (unionStationPos[stationNo].X == pt.X && unionStationPos[stationNo].Y == pt.Y)
                                unionStationValues[stationNo].Add(stationValue);
                        }
                        else
                        {
                            unionStationPos.Add(stationNo, pt);
                            unionStationValues.Add(stationNo, new List<string> { stationValue });
                        }
                    }
                }
            }
        }

        public static unsafe T[] GetDataValue<T>(IRasterBand band,int width, int height, int xoffset = 0, int yoffset = 0)
        {
            int length = width * height;
            enumDataType dataType = band.DataType;            ;
            switch (dataType)
            {
                case enumDataType.Float:
                    {
                        float[] buffer = new float[length];
                        fixed (float* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Float, width, height);
                        }
                        return buffer as T[];
                    }
                case enumDataType.Int16:
                    {
                        short[] buffer = new short[length];
                        fixed (short* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Int16, width, height);
                        }
                        return buffer as T[];
                    }
                case enumDataType.Byte:
                    {
                        Byte[] buffer = new Byte[width * height];
                        fixed (Byte* ptr = buffer)
                        {
                            IntPtr bufferPtr = new IntPtr(ptr);
                            band.Read(xoffset, yoffset, width, height, bufferPtr, enumDataType.Byte, width, height);
                        }
                        return buffer as T[];
                    }

            }
            return null;
        }

        /// <summary>
        /// 获取栅格文件对应波段中对应点集处的数据值
        /// </summary>
        /// <param name="rasterFile"></param>
        /// <param name="bandNum"></param>
        /// <param name="unionStationPos"></param>
        /// <returns></returns>
        private static Double[] GetMatchedRasterValues(string rasterFile, int bandNum, ShapePoint []unionStationPos)
        {
            List<double> rasterdatalist = new List<double>();
            int xoffset, yoffset;
            enumDataType datatype = enumDataType.Unknow;
            using (IRasterDataProvider dataPrd = GeoDataDriver.Open(rasterFile) as IRasterDataProvider)
            {
                datatype = dataPrd.DataType;
                if (datatype != enumDataType.Float && datatype != enumDataType.Int16 && datatype != enumDataType.Byte)
                    return null;
                CoordEnvelope env = dataPrd.CoordEnvelope;
                float reslX = dataPrd.ResolutionX;
                float reslY = dataPrd.ResolutionY;
                IRasterBand band = dataPrd.GetRasterBand(bandNum);
                double value = 0;
                foreach (ShapePoint spt in unionStationPos)
                {
                    xoffset = (int)((spt.X - env.MinX) / reslX);
                    yoffset = (int)((env.MaxY - spt.Y) / reslY);
                    if (xoffset < 0 || yoffset < 0)
                        continue;
                    switch (datatype)
                    {
                        case enumDataType.Float:
                            value = GetDataValue<float>(band, 1, 1, xoffset, yoffset)[0];
                            break;
                        case enumDataType.Int16:
                            value = GetDataValue<short>(band, 1, 1, xoffset, yoffset)[0];
                            break;
                        case enumDataType.Byte:
                            value = GetDataValue<byte>(band, 1, 1, xoffset, yoffset)[0];
                            break;
                    }
                    rasterdatalist.Add(value);
                }
            }
            return rasterdatalist.ToArray();
        }

        #endregion

        /// <summary>
        /// 获取栅格文件与自定义区域的交集区域
        /// </summary>
        /// <param name="rasterfiles"></param>
        /// <param name="regionEnv"></param>
        /// <returns></returns>
        public static Envelope GetInterectEnv(string[] rasterfiles, PrjEnvelope regionEnv)
        {
            return CloudParaFileStatics.GetIntersectEnvolop(rasterfiles, regionEnv);
        }

        #region SVD分解抽取点

        /// <summary>
        /// 获取指定范围内的栅格及micpas数据的某一波段/类型的数据值，同时返回范围内点集的位置
        /// 若未指定区域，则以栅格的范围为主
        /// </summary>
        /// <param name="rasterfiles"></param>
        /// <param name="bandNum"></param>
        /// <param name="fillValueStrRst"></param>
        /// <param name="micapsFiles"></param>
        /// <param name="mbandNo"></param>
        /// <param name="fillValuestr"></param>
        /// <param name="envelope"></param>
        /// <param name="rasterdata"></param>
        /// <param name="micapsdata"></param>
        /// <param name="matchedpos"></param>
        public static void GetMatchedMatrixs(string[] rasterfiles, int bandNum, string[] fillValueStrRst, string[] micapsFiles, int mbandNo, string[] fillValuestr, PrjEnvelope envelope, out double[][] rasterdata, out double[][] micapsdata, out List<ShapePoint> matchedpos)
        {
            rasterdata = null;
            micapsdata = null;
            matchedpos = null;
            Envelope dstEnv = GetInterectEnv(rasterfiles, envelope);
            if (dstEnv == null || dstEnv.Width == 0 || dstEnv.Height == 0)
                throw new ArgumentNullException("待分析区域为空,请确认自定义区域范围或数据文件范围！");
            #region 获取micaps数据
            int length = micapsFiles.Length;
            GetMatchedPosAndValues(micapsFiles, mbandNo, fillValuestr,dstEnv,  out micapsdata, out matchedpos);
            int width = matchedpos.Count;
            #endregion
            #region 获取栅格对应点的数据
            GetMatchedRasterValues(rasterfiles, bandNum, fillValueStrRst, matchedpos,out rasterdata);
            //Double[] fillValuesRst = null;
            //fillValuesRst = CloudParaFileStatics.GetFillValues<Double>(fillValueStrRst, enumDataType.Double);
            //rasterdata = new double[length][];
            //double[] matchedRasterValues = new double[width];
            //for (int lr = 0; lr < length;lr++ )
            //{
            //    matchedRasterValues = GetMatchedRasterValues(rasterfiles[lr], bandNum, matchedpos.ToArray());
            //    double alue;
            //    for (int i = 0; i < width; i++)
            //    {
            //        alue = matchedRasterValues[i];
            //        if (fillValuesRst != null && fillValuesRst.Contains(alue))
            //            matchedRasterValues[i] = 0;
            //    }
            //    rasterdata[lr] = matchedRasterValues;
            //}
            #endregion
        }

        public static void GetMatchedRasterValues(string[] rasterfiles, int bandNum, string[] fillValueStrRst,List<ShapePoint> matchedpos,out double[][] rasterdata)
        {
            rasterdata = null;
            int width = matchedpos.Count;
            int length = rasterfiles.Length;
            Double[] fillValuesRst = null;
            fillValuesRst = CloudParaFileStatics.GetFillValues<Double>(fillValueStrRst, enumDataType.Double);
            rasterdata = new double[length][];
            double[] matchedRasterValues = new double[width];
            for (int lr = 0; lr < length; lr++)
            {
                matchedRasterValues = GetMatchedRasterValues(rasterfiles[lr], bandNum, matchedpos.ToArray());
                double alue;
                for (int i = 0; i < width; i++)
                {
                    alue = matchedRasterValues[i];
                    if (fillValuesRst != null && fillValuesRst.Contains(alue))
                        matchedRasterValues[i] = 0;
                }
                rasterdata[lr] = matchedRasterValues;
            }
        }

        public static void GetMatchedPosAndValues(string[] micapsFiles, int mbandNo, string[] fillValuestr, Envelope dstEnv,out double[][] micapsdata, out List<ShapePoint> matchedpos)
        {
            micapsdata = null;
            matchedpos = null;
            #region 获取micaps数据
            int length = micapsFiles.Length;
            Dictionary<string, ShapePoint> unionStationPos = new Dictionary<string, ShapePoint>();
            Dictionary<int, Dictionary<string, string>> allstationValue = new Dictionary<int, Dictionary<string, string>>();
            GetUnionStationPos(micapsFiles, mbandNo, fillValuestr, dstEnv, out unionStationPos, out allstationValue);
            //各个文件的站点号取并集后取交集,求得最终需要处理的站点集合
            List<string> matchedsn;
            GetMatchedStation(length, unionStationPos, allstationValue, out matchedsn, out matchedpos);
            //获取各个micaps文件的站点值
            double staValue;
            int width = matchedsn.Count;
            string[] matchedsnstr = matchedsn.ToArray();
            micapsdata = new double[length][];
            int w, l;
            for (l = 0; l < length; l++)
            {
                micapsdata[l] = new double[width];
                for (w = 0; w < width; w++)
                {
                    if (double.TryParse(allstationValue[l][matchedsnstr[w]], out staValue))
                        micapsdata[l][w] = staValue;
                }
            }
            #endregion
        }

        /// <summary>
        /// 两组micaps数据间进行SVD分解时，返回两个匹配的站点数据数组
        /// </summary>
        /// <param name="filesl"></param>
        /// <param name="indexl"></param>
        /// <param name="fillValuesStrL"></param>
        /// <param name="filesr"></param>
        /// <param name="indexr"></param>
        /// <param name="fillValuesStrr"></param>
        /// <param name="envelope"></param>
        /// <param name="micDL"></param>
        /// <param name="micDR"></param>
        public static void GetMicapsDataMatrixs(string[] filesl, int indexl, string[] fillValuesStrL, string[] filesr, int indexr, string[] fillValuesStrr, Envelope envelope, out double[][] micDL, out double[][] micDR, out List<ShapePoint> matchedpos)
        {
            micDL = null; micDR = null;
            matchedpos = null;
            #region 获取左场micaps数据
            int length = filesl.Length;
            Dictionary<string, ShapePoint> unionStationPosL,unionStationPosR;
            Dictionary<int, Dictionary<string, string>> allstationValueL, allstationValueR;
            GetUnionStationPos(filesl, indexl, fillValuesStrL, envelope, out unionStationPosL, out allstationValueL);
            List<string> matchedsnl, matchedsnr;
            List<ShapePoint> matchedposl, matchedposr;
            GetMatchedStation(length,unionStationPosL, allstationValueL, out matchedsnl, out matchedposl);
            #endregion
            #region 获取右场micaps数据
            GetUnionStationPos(filesr, indexr, fillValuesStrr, envelope, out unionStationPosR, out allstationValueR);
            GetMatchedStation(length, unionStationPosR, allstationValueR, out matchedsnr, out matchedposr);
            #endregion
            #region 匹配左右场micaps数据
            List<string> matchedsn = new List<string>();
            matchedpos = new List<ShapePoint>();
            foreach (string sn in matchedsnl)
            {
                if (matchedsnr.Contains(sn))
                {
                    matchedsn.Add(sn);
                    matchedpos.Add(unionStationPosL[sn]);
                }
            }
            #endregion
            if (matchedsn.Count<1)
                throw new ArgumentException("左右场没有匹配的站点数据！");
            //获取各个micaps文件的站点值
            double staValue;
            int width = matchedsn.Count;
            string[] matchedsnstr = matchedsn.ToArray();
            micDL = new double[length][];
            micDR = new double[length][];
            int w, l;
            for (l = 0; l < length; l++)
            {
                micDL[l] = new double[width];
                micDR[l] = new double[width];
                for (w = 0; w < width; w++)
                {
                    if (double.TryParse(allstationValueL[l][matchedsnstr[w]], out staValue))
                        micDL[l][w] = staValue;
                    if (double.TryParse(allstationValueR[l][matchedsnstr[w]], out staValue))
                        micDR[l][w] = staValue;
                }
            }
        }

        private static void GetUnionStationPos(string[] files, int index, string[] fillValuesStr, Envelope envelope, out Dictionary<string, ShapePoint> unionStationPos, out Dictionary<int, Dictionary<string, string>> allstationValue)
        {
            int length = files.Length;
            unionStationPos = new Dictionary<string, ShapePoint>();
            allstationValue = new Dictionary<int, Dictionary<string, string>>();
            Feature[] singleFeatures;
            for (int i = 0; i < length; i++)//foreach (string file in mfiles)
            {
                singleFeatures = GetMicapsFeatures(files[i], envelope);
                if (singleFeatures == null || singleFeatures.Length==0)
                {
                    throw new Exception(files[i]+"与待分析区域没有焦点请重试！");
                }
                foreach (Feature fea in singleFeatures)
                {
                    ShapePoint pt = fea.Geometry as ShapePoint;
                    string stationNo = fea.FieldValues[0];//站点编号
                    string stationValue = fea.FieldValues[index];
                    if (fillValuesStr != null && fillValuesStr.Contains(stationValue))
                        continue;
                    if (stationNo != "01009")
                    {
                        if (!unionStationPos.ContainsKey(stationNo))
                            unionStationPos.Add(stationNo, pt);
                        if (!allstationValue.ContainsKey(i))
                        {
                            Dictionary<string, string> stationv = new Dictionary<string, string>();
                            stationv.Add(stationNo, stationValue);
                            allstationValue.Add(i, stationv);
                        }
                        else
                            allstationValue[i].Add(stationNo, stationValue);
                    }
                }
            }
            if (unionStationPos.Count < 1)
                throw new ArgumentException("指定的Micaps数据不包含站点数据！");
        }

        public static List<ShapePoint> GetUnionStationPos(string [] files, CodeCell.AgileMap.Core.Envelope envelope)
        {
            int length = files.Length;
            Feature[] singleFeatures;
            List<ShapePoint> shapiontsList = new List<ShapePoint>();
            for (int i = 0; i < length; i++)
            {
                singleFeatures = GetMicapsFeatures(files[i], envelope);
                foreach (Feature fea in singleFeatures)
                {
                    ShapePoint pt = fea.Geometry as ShapePoint;
                    string stationNo = fea.FieldValues[0];//站点编号
                    if (stationNo == "01009")
                        continue;
                    if (!shapiontsList.Contains(pt))
                        shapiontsList.Add(pt);
                }
            }
            return shapiontsList;
        }

        private static void GetMatchedStation(int length,Dictionary<string, ShapePoint> unionStationPos,Dictionary<int, Dictionary<string, string>> allstationValue,out List<string> matchedSN,out List<ShapePoint> matchedPos)
        {
            matchedSN= new List<string>();
            matchedPos = new List<ShapePoint>();
            foreach (string sn in unionStationPos.Keys)
            {
                bool matched = true;
                for (int i = 0; i < length; i++)
                {
                    if (!allstationValue[i].ContainsKey(sn))
                    {
                        matched = false;
                        break;
                    }
                }
                if (matched)
                {
                    matchedSN.Add(sn);
                    matchedPos.Add(unionStationPos[sn]);
                }
            }
            if (matchedSN.Count < 1)
                throw new ArgumentException("选取的micaps文件无匹配的站点数据！");
        }

        #endregion

    }
}
