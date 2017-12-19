#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/25 9:02:13
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
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace GeoDo.RSS.DF.SeaWave
{
    public class SeaWaveDataProvider : RasterDataProvider
    {
        protected FileStream _fs;
        protected StreamReader _stReader;

        public SeaWaveDataProvider(string fileName, IGeoDataDriver driver)
            : base(fileName, driver)
        {
            _filelist = new string[] { fileName };
            _fileName = fileName;
            ReadData();
        }

        private void ReadData()
        {
            _dataType = enumDataType.Float;
            _coordEnvelope = new CoordEnvelope(-180, 180, -75, 75);
            _spatialRef = SpatialReference.GetDefault();
            int res = 3;
            _width = (int)_coordEnvelope.Width / res + 1;
            _height = (int)_coordEnvelope.Height / res + 1;
            _dataTypeSize = DataTypeHelper.SizeOf(_dataType);
            _resolutionY = res;
            _resolutionX = res;
            _bandCount = 1;
            _coordType = enumCoordType.GeoCoord;
            FillBand();
        }

        private unsafe void FillBand()
        {
            float[][] bandValues = new float[1][];
            bandValues[0] = new float[_width * _height];
            IArrayRasterDataProvider dataProvider = new ArrayRasterDataProvider<float>(
                _fileName, bandValues, _width, _height, _coordEnvelope, _spatialRef);
            float lon, lat, val;
            int col = 0, row = 0;
            float res = _resolutionX;//or _resolutionY
            float minX = (float)_coordEnvelope.MinX;
            float maxY = (float)_coordEnvelope.MaxY;
            _fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
            _stReader = new StreamReader(_fs, Encoding.ASCII);
            fixed (float* ptr0 = bandValues[0])
            {
                float* ptr = ptr0;
                while (!_stReader.EndOfStream)
                {
                    string sLine = _stReader.ReadLine();
                    string[] parts = sLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    float.TryParse(parts[0], out lon);
                    float.TryParse(parts[1], out lat);
                    float.TryParse(parts[2], out val);
                    lon -= 180;
                    col = (int)((lon - minX) / _resolutionX);
                    row = (int)((maxY - lat) / _resolutionY);
                    ptr = ptr0 + row * _width + col;
                    *ptr = val;
                }
            }
            _rasterBands.Add(dataProvider.GetRasterBand(1));
        }

        public override void AddBand(enumDataType dataType)
        {
        }

        public override void Dispose()
        {
            _stReader.Dispose();
            _fs.Dispose();
            base.Dispose();
        }
    }
}
