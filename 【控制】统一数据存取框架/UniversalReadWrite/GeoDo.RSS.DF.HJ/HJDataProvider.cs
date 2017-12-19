using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using GeoDo.RSS.DF.GDAL;
using GeoDo.RSS.Core.DF;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GeoDo.RSS.DF.HJ
{
    public class HJDataProvider : GDALRasterDataProvider, IHJDataProvider
    {
        private Dictionary<string, string> _infoDic = null;
        private HdrFile _hdr = null;
        private int _lines = 0;
        private int _sample = 0;
        private Dictionary<int, int> _existBands = null;

        public HJDataProvider(string fileName, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, null, driver, access)
        {
            InitDataIdentify();
        }

        public HJDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, header1024, driver, access)
        {
            InitDataIdentify();
        }


        public void ReadVisiCoefficient(ref double[,] operCoef, ref double[,] testCoef, ref double[,] beforeSendCoef)
        {
            throw new NotImplementedException();
        }

        public void ReadIRCoefficient(ref double[,] operCoef, ref double[,] beforeSendCoef)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 调用GDAL前生成.hdr文件
        /// </summary>
        protected override void CallGDALBefore()
        {
            base.CallGDALBefore();
            string[] filenames = null;
            _infoDic = HJXML.ReadXMLFile(fileName, out filenames, out _existBands, out _lines, out _sample);
            //generate hdr file
            this._hdr = new HdrFile();
            _hdr.Lines = _lines;
            _hdr.BandNames = TryGetBandNames();
            _hdr.Bands = _existBands.Count;
            _hdr.Samples = _sample;
            _hdr.HeaderOffset = 0;
            string fname = HdrFile.GetHdrFileName(this._fileName);
            _hdr.SaveTo(fname);
        }

        private void InitDataIdentify()
        {
            _dataIdentify.Satellite = _infoDic["satelliteId"];
            _dataIdentify.Sensor = _infoDic["sensorId"];
            _dataIdentify.OrbitDateTime = DateTime.Parse(_infoDic["imagingStartTime"]);
            _dataIdentify.IsOrbit = string.IsNullOrEmpty(_infoDic["mapProjection"]);
        }

        /// <summary>
        /// 获取波段名列表
        /// </summary>
        /// <returns>波段名</returns>
        private string[] TryGetBandNames()
        {
            //string configfile = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.Location), "GeoDo.RSS.DF.NOAA.BandNames.xml");
            //string[] bandNames = null;
            //using (Stream stream = this.GetType().Assembly.GetManifestResourceStream("GeoDo.RSS.DF.NOAA.GeoDo.Rss.DF.NOAA.BandNames.xml"))
            //{
            //    XDocument doc = XDocument.Load(stream);
            //    doc.Save(configfile);
            //}

            //using (NOAABandNamesXmlParser xml = new NOAABandNamesXmlParser(configfile))
            //{
            //    bandNames = xml.GetBandNames(this._d1bdHeader.CommonInfoFor1BD.SatelliteName, this._d1bdHeader.CommonInfoFor1BD.SensorName);
            //}
            //return bandNames;
            List<string> bandname = new List<string>();
            foreach (int item in _existBands.Keys)
                bandname.Add(item.ToString());
            return bandname.ToArray();
        }

        private void ExtractVisiCoefficient(byte[] coefInfo, ref double[,] coefficient, int lineIndex, int channelIndex)
        {
            //byte[] coef = new byte[4];
            //for (int j = 0; j < 5; j++)
            //{
            //    for (int i = 0; i < 4; i++)
            //    {
            //        coef[i] = coefInfo[i + j * 4];
            //    }
            //    if (_d1bdHeader.IsBigEndian)
            //    {
            //        switch (j)
            //        {
            //            case 0:
            //            case 2:
            //                coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromBig(coef) / Math.Pow(10, 10);
            //                break;
            //            case 1:
            //            case 3:
            //                coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromBig(coef) / Math.Pow(10, 7);
            //                break;
            //            case 4:
            //                coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromBig(coef);
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        switch (j)
            //        {
            //            case 0:
            //            case 2:
            //                coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromLittle(coef) / Math.Pow(10, 10);
            //                break;
            //            case 1:
            //            case 3:
            //                coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromLittle(coef) / Math.Pow(10, 7);
            //                break;
            //            case 4:
            //                coefficient[lineIndex, j + channelIndex * 5] = (float)ToLocalEndian_Core.ToInt32FromLittle(coef);
            //                break;
            //        }
            //    }
            //}

        }

        private void ExtractIRCoefficient(byte[] coefInfo, ref double[,] coefficient, int lineIndex, int channelIndex)
        {
            //byte[] coef = new byte[4];
            //for (int j = 0; j < 3; j++)
            //{
            //    for (int i = 0; i < 4; i++)
            //    {
            //        coef[i] = coefInfo[i + j * 4];
            //    }
            //    if (_d1bdHeader.IsBigEndian)
            //        coefficient[lineIndex, j + channelIndex * 3] = (float)ToLocalEndian_Core.ToInt32FromBig(coef) / Math.Pow(10, 6);
            //    else
            //        coefficient[lineIndex, j + channelIndex * 3] = (float)ToLocalEndian_Core.ToInt32FromLittle(coef) / Math.Pow(10, 6);
            //}
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
