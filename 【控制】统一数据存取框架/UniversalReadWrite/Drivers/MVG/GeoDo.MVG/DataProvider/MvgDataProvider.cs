using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;
using GeoDo.Project;
using GeoDo.RSS.DF.LDF;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GeoDo.RSS.DF.MVG
{
    public class MvgDataProvider : RasterDataProvider, IMvgDataProvider
    {
        protected FileStream _fsStream = null;
        protected BinaryWriter _binaryWriter = null;
        protected BinaryReader _binaryReader = null;
        protected MvgHeader _header = null;
        protected string[] _valueNames = null;

        //适用于打开文件的情况
        public MvgDataProvider(string fileName, IGeoDataDriver dataDriver, bool isReadOnly)
            : base(fileName, dataDriver)
        {
            _driver = dataDriver;
            _fileName = fileName;
            string hdrfile = HdrFile.GetHdrFileName(_fileName);
            if (File.Exists(hdrfile))
                _filelist = new string[] { _fileName, hdrfile };
            else
                _filelist = new string[] { _fileName };
            _header = new MvgHeader(fileName);
            FillProviderAttributes();
            _fsStream = new FileStream(_fileName, FileMode.Open, isReadOnly ? FileAccess.Read : FileAccess.ReadWrite);
            if (!isReadOnly)
            {
                _binaryWriter = new BinaryWriter(_fsStream);
            }
            _binaryReader = new BinaryReader(_fsStream);
            LoadBands();
        }

        public MvgDataProvider(string fileName, IGeoDataDriver dataDriver, MvgHeader mvgHeader, bool isReadOnly)
            : base(fileName, dataDriver)
        {
            _driver = dataDriver;
            _fileName = fileName;
            _driver = dataDriver;
            _header = mvgHeader;
            string hdrfile = HdrFile.GetHdrFileName(_fileName);
            if (File.Exists(hdrfile))
                _filelist = new string[] { _fileName, hdrfile };
            else
                _filelist = new string[] { _fileName };
            FillProviderAttributes();
            _fsStream = new FileStream(fileName, FileMode.Open, isReadOnly ? FileAccess.Read : FileAccess.ReadWrite);
            _binaryWriter = new BinaryWriter(_fsStream);
            _binaryReader = new BinaryReader(_fsStream);
            LoadBands();
        }

        public MvgHeader Header
        {
            get { return _header; }
        }

        private void FillProviderAttributes()
        {
            _bandCount = 1;
            _dataType = enumDataType.Int16;
            if (_header != null)
            {
                _width = _header.Width;
                _height = _header.Height;
                _resolutionX = _header.LongitudeResolution;
                _resolutionY = _header.LatitudeResolution;
                _spatialRef = _header.SpatialRef;
                _coordEnvelope = new Core.DF.CoordEnvelope(_header.MinLon, _header.MaxLon, _header.MinLat, _header.MaxLat);
                TryCreateCoordTransform();
            }
        }

        private void TryCreateCoordTransform()
        {
            if (_spatialRef == null)
                _coordTransform = CoordTransoformFactory.GetCoordTransform(null, null, _width, _height);
            else
            {
                double minLon, minLat, maxLon, maxLat;
                _header.GetCoordEnvelope(out minLon, out minLat, out maxLon, out maxLat);
                _coordTransform = CoordTransoformFactory.GetCoordTransform(
                    new Point(0, 0),
                    new Point(_width, _height),
                    new double[] { minLon, maxLat },
                    new double[] { maxLon, minLat });
            }
        }

        private void CreatHdrFile(string fileName)
        {
            HdrFile hdr;
            string fname = HdrFile.GetHdrFileName(fileName);
            if (File.Exists(fname))
                return;
            else
            {
                hdr = _header.ToHdrFile();
                if (hdr != null)
                    hdr.SaveTo(fname);
            }
        }

        private void LoadBands()
        {
            _rasterBands.Add(new MvgRasterBand(this, _binaryReader, _binaryWriter, _fsStream));
        }

        public override void AddBand(enumDataType dataType)
        {
            throw new NotSupportedException("MVG文件为单波段栅格文件，不支持新增波段！");
        }

        #region Mvg To Ldf File
        public void ToLdfFile()
        {
            string ldfFileName = Path.Combine(Path.GetDirectoryName(_fileName), Path.GetFileNameWithoutExtension(_fileName) + ".ldf");
            ToLdfFile(ldfFileName);
        }

        public void ToLdfFile(string ldfFileName)
        {
            ILdfDriver ldfDrv = null;
            IRasterDataProvider ldfPrd = null;
            IRasterBand ldfBand = null;
            IRasterBand mvgBand = null;
            Ldf1Header ldfHeader = null;
            try
            {
                ldfDrv = GeoDataDriver.GetDriverByName("LDF") as ILdfDriver;
                if (ldfDrv == null)
                    return;
                ldfPrd = ldfDrv.Create(ldfFileName, _width, _height, 1, enumDataType.Int16);
                if (ldfPrd == null)
                    return;
                ldfBand = ldfPrd.GetRasterBand(1);
                if (ldfBand == null)
                    return;

                ldfHeader = MvgHeaderConvertor.MvgHeaderToLdfHeader(_header);
                if (ldfHeader == null)
                    return;

                //read data by block ,write ldf provider
                mvgBand = GetRasterBand(1);
                if (mvgBand == null)
                    return;
                WriteDataFromMvgToLdf(ldfBand, mvgBand);
            }
            finally
            {
                Disposed(ldfDrv, ldfPrd, ldfBand, null, null, mvgBand);
            }
        }

        private void WriteDataFromMvgToLdf(IRasterBand ldfBand, IRasterBand mvgBand)
        {
            int rowsOfBlock = (int)Math.Floor((float)GeoDo.RSS.Core.DF.Constants.MAX_PIXELS_BLOCK / (_width * DataTypeHelper.SizeOf(enumDataType.Int16)));//每块的行数
            if (rowsOfBlock > _height)
                rowsOfBlock = _height;
            int countBlocks = (int)Math.Floor((float)_height / rowsOfBlock); //总块数
            Int16[] buffer = new Int16[rowsOfBlock * _width];
            IntPtr bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);

            int bRow = 0;
            int eRow = 0;
            int bufferRowCount = 0;
            for (int i = 0; i < countBlocks; i++, bRow += rowsOfBlock)
            {
                eRow = Math.Min(_height, bRow + rowsOfBlock);
                bufferRowCount = eRow - bRow;
                mvgBand.Read(0, bRow, _width, bufferRowCount, bufferPtr, enumDataType.Int16, _width, bufferRowCount);
                ldfBand.Write(0, bRow, _width, bufferRowCount, bufferPtr, enumDataType.Int16, _width, bufferRowCount);
            }
        }

        private void CreatHdrFileByLdfFile(string ldfFileName)
        {
            string hdrFileName = HdrFile.GetHdrFileName(ldfFileName);
            if (!File.Exists(hdrFileName))
                CreatHdrFile(HdrFile.GetHdrFileName(ldfFileName));
        }
        #endregion

        #region ldf to mvg file
        /// <summary>
        /// ldf文件另存为mvg文件
        /// </summary>
        /// <param name="ldfName">ldf文件名</param>
        /// <param name="bandNo">需要获取的波段号</param>
        /// <param name="options">      
        /*
         *  VALUECOUNT = 2         //default:0
         *  VALUES = "{0,1}"       //
         *  VALUENAMES             //
            SPATIALREF=Proj4                  //default:wgs-84
            MAPINFO={X,Y}:{Col,Row}:{ResolutionX,ResolutionY} //default:null
            WITHHDR=[TRUE|FALSE]      //default:true
        */
        /// </param>
        public static void FromLDF(string ldfFileName, int bandNo, params object[] options)
        {
            string mvgFileName = Path.Combine(Path.GetDirectoryName(ldfFileName), Path.GetFileNameWithoutExtension(ldfFileName) + ".MVG");
            FromLDF(ldfFileName, mvgFileName, bandNo, options);
        }

        /// <summary>
        /// ldf文件另存为mvg文件
        /// </summary>
        /// <param name="ldfName">ldf文件名</param>
        /// <param name="mvgName">mvg文件名</param>
        /// <param name="bandNo">需要获取的波段号</param>
        /// <param name="options">      
        /*
         *  VALUECOUNT = 2         //default:0
         *  VALUES = "{0,1}"       //
         *  VALUENAMES             //
            SPATIALREF=Proj4                  //default:wgs-84
            MAPINFO={X,Y}:{Col,Row}:{ResolutionX,ResolutionY} //default:null
            WITHHDR=[TRUE|FALSE]      //default:true
        */
        /// </param>
        public static void FromLDF(string ldfFileName, string mvgFileName, int bandNo, params object[] options)
        {
            IGeoDataDriver ldfDrv = null;
            IRasterDataProvider ldfPrd = null;
            IRasterBand ldfBand = null;
            IMvgDriver mvgDrv = null;
            IMvgDataProvider mvgPrd = null;
            IRasterBand mvgBand = null;
            try
            {
                ldfDrv = GeoDataDriver.GetDriverByName("LDF");
                if (ldfDrv == null)
                    return;
                ldfPrd = ldfDrv.Open(ldfFileName, enumDataProviderAccess.ReadOnly) as IRasterDataProvider;
                if (ldfPrd == null)
                    return;
                Ldf1Header ldfHeader = (ldfPrd as ILdfDataProvider).Header as Ldf1Header;
                if (ldfHeader == null || (ldfHeader.DataType != enumDataType.Int16 && ldfHeader.DataType != enumDataType.UInt16))
                    throw new NotSupportedException("只支持Int16和UInt16数据格式的LDF文件转换！");
                ldfBand = ldfPrd.GetRasterBand(bandNo);
                if (ldfBand == null)
                    return;

                //创建MVG文件
                mvgDrv = GeoDataDriver.GetDriverByName("MVG") as IMvgDriver;
                if (mvgDrv == null)
                    return;
                mvgPrd = mvgDrv.Create(mvgFileName, ldfBand.Width, ldfBand.Height, 1, enumDataType.Int16, options) as IMvgDataProvider;
                if (mvgPrd == null)
                    return;
                MvgHeaderConvertor.FillMvgHeader(ldfHeader, mvgPrd.Header);
                //重新生成hdr头文件
                string hdrFileName = HdrFile.GetHdrFileName(mvgFileName);
                HdrFile.SaveTo(hdrFileName, mvgPrd.Header.ToHdrFile());

                //读数据并写入MVG文件中
                mvgBand = mvgPrd.GetRasterBand(1);
                if (mvgBand == null)
                    return;
                WriteDataFromLdfToMvg(ldfHeader, ldfBand, mvgBand);
            }
            finally
            {
                if (mvgPrd != null)
                    (mvgPrd as MvgDataProvider).Disposed(ldfDrv, ldfPrd, ldfBand, mvgDrv, mvgPrd, mvgBand);
            }
        }

        private static void WriteDataFromLdfToMvg(Ldf1Header ldfHeader, IRasterBand ldfBand, IRasterBand mvgBand)
        {
            int width = ldfBand.Width;
            int height = ldfBand.Height;
            int rowsOfBlock = (int)Math.Floor((float)GeoDo.RSS.Core.DF.Constants.MAX_PIXELS_BLOCK / (width * DataTypeHelper.SizeOf(enumDataType.Int16)));//每块的行数
            if (rowsOfBlock > height)
                rowsOfBlock = height;
            int countBlocks = (int)Math.Floor((float)height / rowsOfBlock); //总块数
            Int16[] buffer = new Int16[rowsOfBlock * width];
            IntPtr bufferPtr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);

            int bRow = 0;
            int eRow = 0;
            int bufferRowCount = 0;
            for (int i = 0; i < countBlocks; i++, bRow += rowsOfBlock)
            {
                eRow = Math.Min(height, bRow + rowsOfBlock);
                bufferRowCount = eRow - bRow;
                ldfBand.Read(0, bRow, width, bufferRowCount, bufferPtr, ldfHeader.DataType, width, bufferRowCount);
                mvgBand.Write(0, bRow, width, bufferRowCount, bufferPtr, enumDataType.Int16, width, bufferRowCount);
            }
        }

        private void Disposed(IGeoDataDriver ldfDrv, IRasterDataProvider ldfPrd, IRasterBand ldfBand, IMvgDriver mvgDrv, IMvgDataProvider mvgPrd, IRasterBand mvgBand)
        {
            if (mvgBand != null)
                mvgBand.Dispose();
            if (mvgPrd != null)
                mvgPrd.Dispose();
            if (mvgDrv != null)
                mvgDrv.Dispose();
            if (ldfBand != null)
                ldfBand.Dispose();
            if (ldfPrd != null)
                ldfPrd.Dispose();
            if (ldfDrv != null)
                ldfDrv.Dispose();
        }
        #endregion

        public override void Dispose()
        {
            base.Dispose();
            if (_fsStream != null)
            {
                _fsStream.Dispose();
                _fsStream = null;
            }
            if (_binaryReader != null)
            {
                _binaryReader.Dispose();
                _binaryReader = null;
            }
            if (_binaryWriter != null)
            {
                _binaryWriter.Dispose();
                _binaryWriter = null;
            }
        }
    }
}
