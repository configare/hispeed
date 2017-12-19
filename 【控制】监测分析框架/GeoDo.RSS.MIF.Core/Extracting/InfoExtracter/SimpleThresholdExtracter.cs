using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class SimpleThresholdExtracter<T>:IThresholdExtracter<T>,IDisposable
    {
        private RasterPixelsVisitor<T> _infoExtracter;
        private IArgumentProvider _argProvider;
        private Func<int, T[], bool> _boolFunc;
        private int[] _visitBandNos;

        public SimpleThresholdExtracter()
        {
        }

        public SimpleThresholdExtracter(IArgumentProvider argProvider,int[] visitBandNos, string express)
        {
            Reset(argProvider, visitBandNos, express);
        }

        public void Reset(IArgumentProvider argProvider,int[] visitBandNos, string express)
        {
            if (_infoExtracter != null)
                _infoExtracter.Dispose();
            _infoExtracter = new RasterPixelsVisitor<T>(argProvider);
            _argProvider = argProvider;
            _visitBandNos = visitBandNos;
            IExtractFuncProvider<T> prd = ExtractFuncProviderFactory.CreateExtractFuncProvider<T>(visitBandNos, express, _argProvider);
            _boolFunc = prd.GetBoolFunc();
        }

        public void Extract(IPixelIndexMapper extractedPixels)
        {
            if (_infoExtracter == null || _boolFunc == null)
                return;
            _infoExtracter.VisitPixel(_visitBandNos, (idx, values) =>
                {
                    if (_boolFunc(idx, values))
                        extractedPixels.Put(idx);
                });
        }

        public void Dispose()
        {
            _argProvider = null;
            _boolFunc = null;
            if (_infoExtracter != null)
            {
                _infoExtracter.Dispose();
                _infoExtracter = null;
            }
        }
    }
}
