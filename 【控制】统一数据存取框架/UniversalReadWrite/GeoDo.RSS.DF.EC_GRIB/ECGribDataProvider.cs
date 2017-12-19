using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.DF.EC_GRIB
{
    public class ECGribDataProvider : RasterDataProvider, IECGribDataProvider
    {
        private FileStream _gribFs = null;
        private BinaryReader _gribBr = null;
        private PDSClass _pds = null;
        private IThirdSection _thirdSec = null;
        private BDSClass _bds = null;
        private int _dataLength = 0;
        private float[] _values = null;

        public ECGribDataProvider(string fileName, IGeoDataDriver driver)
            : base(fileName, driver)
        {
            ReadToDataProvider();
        }

        public string Parameter
        {
            get 
            {
                if (_pds != null)
                    return _pds.Parameter;
                else
                    return null;
            }
        }

        public int DataLength
        {
            get { return _dataLength; }
        }

        public float[] Values
        {
            get { return _values; }
        }

        private void ReadToDataProvider()
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            if (!File.Exists(fileName))
                return;
            _gribFs = new FileStream(fileName, FileMode.Open);
            _gribBr = new BinaryReader(_gribFs);
            char[] fileFg = _gribBr.ReadChars(4);
            string fileFgStr = string.Join("", fileFg);
            if (!string.Equals(fileFgStr, "GRIB"))   //01-04:GRIB的文件标识
                throw new Exception("文件错误");
            byte[] bytes = _gribBr.ReadBytes(3);   //05-07:读取3字节的整数
            _dataLength = MathHelper.Bytes2Int(bytes); //整个文件的数据长度，bytes[0]*16*16*16+bytes[1]*16*16+bytes[2];
            if (_dataLength == 0)
                return;
            Byte version = _gribBr.ReadByte();  //08:版本，为1
            _pds = new PDSClass(_gribFs, _gribBr);

            if (_pds.ThreeSection == enumThreeSection.BMS)
                _thirdSec = new BMSClass(_gribFs, _gribBr);
            else
            {
                _thirdSec = new GDSClass(_gribFs, _gribBr);
                _width = (_thirdSec as GDSClass).LatPointsNum;
                _height = (_thirdSec as GDSClass).LonPointsNum;
                _resolutionX = (_thirdSec as GDSClass).LatResolution;
                _resolutionY = (_thirdSec as GDSClass).LonResolution;
            }
            _bds = new BDSClass(_gribFs, _gribBr, (_thirdSec as GDSClass).LonPointsNum, (_thirdSec as GDSClass).LatPointsNum);

            float[] xDatas = _bds.OriginData as float[];
            int length = xDatas.Length;
            float[] values = new float[length];
            for (int i = 0; i < length; i++)
            {
                values[i] = GribHelper.ComputeActualValue(xDatas[i], _pds.DecimalFactor, _bds.BinaryScale, _bds.ReferenceValue);
            }
            _values = values;
            _coordEnvelope = new CoordEnvelope(-180, 180, -90, 90);
        }

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            if (_gribBr != null)
            {
                _gribBr.Dispose();
                _gribBr = null;
            }
            if (_gribFs != null)
            {
                _gribFs.Dispose();
                _gribFs = null;
            }
        }
    }
}
