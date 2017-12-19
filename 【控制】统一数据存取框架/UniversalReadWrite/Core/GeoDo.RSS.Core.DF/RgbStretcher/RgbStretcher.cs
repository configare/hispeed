using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class RgbStretcher<T> : IRgbStretcher<T>, IDisposable
    {
        protected Func<T, byte> _stretcher = null;
        protected byte _minRgb = 0;
        protected byte _maxRgb = 255;
        protected byte[] _map = null;
        protected bool _isUseMap = false;
        protected int[] _defaultBands = null;

        public RgbStretcher()
        {
        }

        public RgbStretcher(Func<T, byte> stretcher)
        {
            _stretcher = stretcher;
        }

        public bool IsUseMap
        {
            get { return _isUseMap; }
            set
            {
                if (_isUseMap != value)
                {
                    _isUseMap = value;
                    if (_isUseMap)
                    {
                        if (_map == null)
                            BuildMap();
                        SetStretcherUseMap();
                    }
                    else
                    {
                        SetStretcherUseAlgorithm();
                    }
                }
            }
        }

        public int[] DefaultBands
        {
            get { return _defaultBands; }
            set { _defaultBands = value; }
        }

        public void ResetMap()
        {
            _map = null;
        }

        public byte[] Map
        {
            get
            {
                if (_map == null)
                    BuildMap();
                return _map;
            }
        }

        protected virtual void BuildMap()
        {
            throw new NotImplementedException();
        }

        public unsafe Func<T, byte> Stretcher
        {
            get { return _stretcher; }
        }

        public unsafe void Stretch(T data, byte* rgb)
        {
            *rgb = _stretcher(data);
        }

        protected virtual void SetStretcherUseMap()
        {            
        }

        protected virtual void SetStretcherUseAlgorithm()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //
        }
    }
}
