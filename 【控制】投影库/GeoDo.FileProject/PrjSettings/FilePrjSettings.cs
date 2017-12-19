using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.Project;
using GeoDo.RasterProject;
using GeoDo.RSS.Core.DF;
using System.Drawing;

namespace GeoDo.FileProject
{
    public class FilePrjSettings
    {
        protected int[] _outBandNos = null;
        //单位和OutEnvelope的单位一致
        protected float _outResolutionX = 0;
        protected float _outResolutionY = 0;
        //DriverName,如果为内存则为"MEMORY"
        protected string _outFormat = "";
        protected PrjEnvelope _outEnvelope = null;
        //如果_outFormat为非内存，则需要指定该参数
        private string _outPathAndFileName = null;
        /// <summary>
        /// 投影结束后是否清理缓存文件
        /// </summary>
        private bool _isClearPrjCache = false;
        //
        private object[] _extArgs = null;

        public FilePrjSettings()
        {
        }

        /// <summary>
        /// 从1开始的波段号
        /// </summary>
        public int[] OutBandNos
        {
            get { return _outBandNos; }
            set { _outBandNos = value; }
        }

        public float OutResolutionX
        {
            get { return _outResolutionX; }
            set { _outResolutionX = value; }
        }

        public float OutResolutionY
        {
            get { return _outResolutionY; }
            set { _outResolutionY = value; }
        }

        public string OutFormat
        {
            get { return _outFormat; }
            set { _outFormat = value; }
        }

        public PrjEnvelope OutEnvelope
        {
            get { return _outEnvelope; }
            set { _outEnvelope = value; }
        }

        public string OutPathAndFileName
        {
            get { return _outPathAndFileName; }
            set { _outPathAndFileName = value; }
        }

        public Size OutSize
        {
            get { return _outEnvelope == null ? Size.Empty : _outEnvelope.GetSize(_outResolutionX, _outResolutionY); }
        }

        public object[] ExtArgs
        {
            get { return _extArgs; }
            set { _extArgs = value; }
        }

        /// <summary>
        /// 投影结束后是否清理缓存文件
        /// </summary>
        public bool IsClearPrjCache
        {
            get { return _isClearPrjCache; }
            set { _isClearPrjCache = value; }
        }
    }

    public struct OutArgs
    {
        public PrjEnvelope PrjEnvelope;
        public string OutPathAndFileName;
    }
}
