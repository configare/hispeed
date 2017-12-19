#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/22 10:31:39
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
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.Project;

namespace GeoDo.RSS.DF.SeaTransparency
{
    /// <summary>
    /// 类名：SeaTransparencyDataProvider
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/22 10:31:39
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class SeaTransparencyDataProvider:RasterDataProvider,ISeaTransparencyDataProvider
    {
        private enumSeaTransparencyType _type;
        private FileStream _fs;
        private StreamReader _stReader;

        public SeaTransparencyDataProvider(string fileName, IGeoDataDriver driver)
            : base(fileName, driver)
        {
            _fileName = fileName;
            InitDataDefinition();
        }

        private void InitDataDefinition()
        {
           //通过文件行数设置海水透明度类型（中国/全球）
            string[] valueLines = File.ReadAllLines(_fileName);
            if (valueLines.Length == 1200)
                _type = enumSeaTransparencyType.China;
            else if (valueLines.Length == 4320)
                _type = enumSeaTransparencyType.Global;
            else
                return;
            //根据类型初始化相关参数
            _dataType = enumDataType.Float;
            _dataTypeSize = DataTypeHelper.SizeOf(_dataType);
            _bandCount = 1;
            _coordType = enumCoordType.GeoCoord;
            switch (_type)
            {
                case enumSeaTransparencyType.China:
                    {
                        _coordEnvelope = new CoordEnvelope(0, 50, 100, 150);
                        _spatialRef = SpatialReference.GetDefault();
                        _width = 1200;
                        _height = 1200;
                        _resolutionY = (float)(_coordEnvelope.Height / _height);
                        _resolutionX = (float)(_coordEnvelope.Width / _width);
                        break;
                    }
                case enumSeaTransparencyType.Global:
                    {
                        _coordEnvelope = new CoordEnvelope(-180, 180, -90, 90);
                        _spatialRef = SpatialReference.GetDefault();
                        _width = 8640;
                        _height = 4320;
                        _resolutionY = (float)(_coordEnvelope.Height / _height);
                        _resolutionX = (float)(_coordEnvelope.Width / _width);
                        break;
                    }
            }
            FillBand();
        }

        public enumSeaTransparencyType Type
        {
            get { return _type; }
        }

        public override void AddBand(enumDataType dataType)
        {

        }

        private unsafe void FillBand()
        {
            float[][] bandValues = new float[1][];
            bandValues[0] = new float[_width * _height];
            IArrayRasterDataProvider dataProvider = new ArrayRasterDataProvider<float>(
                _fileName, bandValues, _width, _height, _coordEnvelope, _spatialRef);
            float val;
            int row = _height - 1;
            float res = _resolutionX;//or _resolutionY
            float minX = (float)_coordEnvelope.MinX;
            float maxY = (float)_coordEnvelope.MaxY;
            _fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
            _stReader = new StreamReader(_fs, Encoding.Default);
            fixed (float* ptr0 = bandValues[0])
            {
                float* ptr = ptr0;
                while (!_stReader.EndOfStream)
                {
                    string sLine = _stReader.ReadLine();
                    string[] parts = sLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    for (int i = 0; i < parts.Length; i++)
                    {
                        float.TryParse(parts[i], out val);
                        ptr = ptr0 + row * _width + i;
                        *ptr = val;
                    }
                    row--;
                }
            }
            _rasterBands.Add(dataProvider.GetRasterBand(1));
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
