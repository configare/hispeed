using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;
using System.Drawing;
using System.IO;


namespace GeoDo.RSS.MIF.Prds.FIR
{
    class Save2DBLV
    {
        public static  void SaveToDBLVDat(string hdffname,string rasterFileName, Feature[] features, double resolution, Action<int, string> processPolback)
        {

            CoordEnvelope envelope;
            HDF5Filter.GetDataCoordEnvelope(hdffname, out envelope);
            int height = (int)Math.Ceiling((envelope.MaxY - envelope.MinY) / resolution);
            int width = (int)Math.Ceiling((envelope.MaxX - envelope.MinX) / resolution);
            Int16[] dblv = new Int16[height * width];
            if (processPolback != null)
                processPolback(15, "开始计算火点判识结果...");
            ProcessFeaturesDBLV(features, resolution, width,envelope.MaxY,envelope.MinX, ref dblv);
            if (processPolback != null)
                processPolback(50, "开始输出火点判识结果...");
            IRasterDataProvider dataPrd = null;
            try
            {
                IRasterDataDriver driver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                string mapInfo = envelope.ToMapInfoString(new Size(width, height));
                //string mapInfo = new CoordEnvelope(envelope.MinX - 0.01, envelope.MaxX - 0.01, envelope.MinY - 0.1, envelope.MaxY - 0.1).ToMapInfoString(new Size(width, height));
                dataPrd = driver.Create(rasterFileName, width, height, 1, enumDataType.Int16, mapInfo);
                unsafe
                {
                    fixed (Int16* ptr = dblv)
                    {
                        IntPtr bufferPtr = new IntPtr(ptr);
                        dataPrd.GetRasterBand(1).Write(0, 0, width, height, bufferPtr, enumDataType.Int16, width, height);
                    }
                }
                if (processPolback != null)
                    processPolback(100, "输出火点判识结果完成！");
            }
            finally
            {
                if (dataPrd != null)
                    dataPrd.Dispose();
            }
        }

        private static void ProcessFeaturesDBLV(Feature[] features, double resolution, int width,double lulat,double lulon, ref Int16[] dblv)
        {
            foreach (Feature fea in features)
            {
                if (fea.FieldValues == null)
                    continue;
                ShapePoint pt = fea.Geometry as ShapePoint;
                int col = (int)Math.Ceiling((pt.X - lulon) / resolution);
                int row = (int)Math.Ceiling((lulat - pt.Y) / resolution);
                dblv[row * width + col] = 1;
            }
        }

        public  static void OutputFireList(Feature[] features,string txtfilename, Action<int, string> processPolback)
        {
            string[] fieldValues;
            int index = 0;
            DateTime orbitDateTime;
            using (StreamWriter sw = new StreamWriter(txtfilename, false, Encoding.Default))
            {
                foreach (Feature fet in features)
                {
                    if (processPolback != null)
                        processPolback(index, "输出火点信息列表...");
                    fieldValues = fet.FieldValues;
                    orbitDateTime = GetOrbitTime(fieldValues[0], fieldValues[1], fieldValues[2]);
                    //0，序号
                    sw.Write((index++).ToString().PadLeft(7, ' '));
                    sw.Write("\t");
                    //1，纬度
                    sw.Write(fieldValues[3].PadLeft(6, ' '));
                    sw.Write("\t");
                    //2，经度
                    sw.Write(fieldValues[4].PadLeft(7, ' '));
                    sw.Write("\t");
                    //3，火区号，默认-9999
                    int fireAreaNum = -9999;
                    sw.Write(fireAreaNum.ToString().PadLeft(7, ' '));
                    sw.Write("\t");
                    //4，地区码，默认0
                    int xianJie = 0;
                    sw.Write(xianJie.ToString().PadLeft(6, ' '));
                    sw.Write("\t");
                    //5，土地利用类型，默认0
                    byte landUse = 0; 
                    sw.Write(landUse.ToString().PadLeft(12, ' '));
                    sw.Write("\t");
                    //6，火点像元面积，默认1
                    int firearea = 1;
                    sw.Write(firearea.ToString("#0.000000").PadLeft(10, ' '));
                    sw.Write("\t");
                    //7，亚像元面积
                    double subfirearea = double.Parse(fieldValues[5]);
                    if (subfirearea == 0)
                        sw.Write("0.0000".PadLeft(6, ' '));
                    else
                        sw.Write(subfirearea.ToString("#0.0000").PadLeft(6, ' '));
                    sw.Write("\t");
                    //8，火点温度
                    sw.Write(fieldValues[6].PadLeft(8, ' '));
                    sw.Write("\t");
                    //9，时间，
                    sw.Write(orbitDateTime.ToString("yyyy-MM-dd HH:mm").PadLeft(16, ' '));
                    sw.Write("\t");
                    //10，火点强度等级
                    sw.Write(fieldValues[7].PadLeft(8, ' '));
                    sw.Write("\t");
                    //11，火点强度，与等级相同
                    //sw.Write(features[fet].FirIntensity.ToString("#.########").PadLeft(16, ' '));
                    sw.Write(fieldValues[7].PadLeft(16, ' '));
                    sw.WriteLine();

                }
            }

        }

        public static DateTime GetOrbitTime(string year, string monthday, string hhmm)
        {
            DateTime orbitDateTime = DateTime.MinValue;
            if (year.Length==4)
            {
                monthday = monthday.PadLeft(4, '0');
                hhmm = hhmm.PadLeft(4, '0');
                string datetime =year+monthday+hhmm;
                orbitDateTime = DateTime.ParseExact(datetime, "yyyyMMddHHmm", System.Globalization.CultureInfo.CurrentCulture);
            }
            return orbitDateTime;
        }
    }
}
