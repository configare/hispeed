using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class BandIndexOutOfRangeException:Exception
    {
        protected int _maxBandCount = 0;
        protected int _bandIndex = 0;

        public BandIndexOutOfRangeException(int bandCount, int bandIndex)
        {
            _maxBandCount = bandCount;
            _bandIndex = bandIndex;
        }

        public override string Message
        {
            get
            {
                return string.Format("波段下标\"{0}\"越界,应为\"{1}~{2}\"之间的数！", _bandIndex, 0, _maxBandCount);
            }
        }
    }

    public class RasterBandsIsEmptyException : Exception
    {
        public override string Message
        {
            get
            {
                return "波段集合为空！";
            }
        }
    }

    public class InterleaveIsNotSupportException : Exception
    {
        protected enumInterleave _interleave = enumInterleave.BSQ;

        public InterleaveIsNotSupportException(enumInterleave interleave)
        {
            _interleave = interleave;
        }

        public override string Message
        {
            get
            {
                return "不支持的像素布局方式\""+_interleave.ToString()+"\"！";
            }
        }
    }

    public class RequestBlockOutOfRasterException : Exception
    {
        protected int _offsetX = 0;
        protected int _offsetY = 0;
        protected int _xSize = 0;
        protected int _ySize = 0;

        public RequestBlockOutOfRasterException(int offsetX,int offsetY,int xSize,int ySize)
        {
            _offsetX = offsetX;
            _offsetY = offsetY;
            _xSize = xSize;
            _ySize = ySize;
        }

        public override string Message
        {
            get
            {
                return string.Format("请求的数据块(OffsetX = {0},OffsetY = {1},XSize = {2},YSize = {3})超过栅格范围！",
                    _offsetX, _offsetY, _xSize, _ySize);
            }
        }
    }

    public class BufferIsEmptyException : Exception
    {
        public override string Message
        {
            get
            {
                return "指定的缓存区为空！";
            }
        }
    }

    public class DataTypeIsNotSupportException : Exception
    {
        private string _dataType = string.Empty;

        public DataTypeIsNotSupportException(Type dataType)
        {
            _dataType = dataType == null ? dataType.ToString() : string.Empty;
        }

        public DataTypeIsNotSupportException(string dataType)
        {
            _dataType = dataType ?? string.Empty;
        }

        public DataTypeIsNotSupportException(enumDataType dataType)
        {
            _dataType = dataType.ToString();
        }

        public override string Message
        {
            get
            {
                return "不支持的数据类型\"" + _dataType + "\"!";
            }
        }
    }

    public class DriverListIsEmptyException : Exception
    {
        public override string Message
        {
            get { return "没有找到注册的驱动!"; }
        }
    }

    public class NoMatchedDirverException : Exception
    {
        private string _driverName = null;

        public NoMatchedDirverException()
            : base()
        { 
        }

        public NoMatchedDirverException(string driverName)
            :base()
        {
            _driverName = driverName;
        }

        public override string Message
        {
            get 
            {
                if (string.IsNullOrEmpty(_driverName))
                    return "没有找到匹配的驱动！";
                else
                    return string.Format("没有找到匹配的驱动{0}！", _driverName);
            }
        }
    }
}
