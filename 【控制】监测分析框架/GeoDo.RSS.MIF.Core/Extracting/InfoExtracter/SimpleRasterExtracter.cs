using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public class SimpleRasterExtracter<TDataType, TFeature> : IRasterExtracter<TDataType, TFeature>
    {
        private RasterPixelsVisitor<TDataType> _infoExtracter;
        protected IArgumentProvider _argProvider;
        private int[] _visitBandNos;
        protected Func<int, TDataType[], TFeature> _featureComputer;

        public SimpleRasterExtracter()
        {
        }

        public SimpleRasterExtracter(IArgumentProvider argProvider, int[] visitBandNos, string express)
        {
            Reset(argProvider, visitBandNos, express);
        }

        public void Reset(IArgumentProvider argProvider, int[] visitBandNos, string express)
        {
            if (_infoExtracter != null)
                _infoExtracter.Dispose();
            _infoExtracter = new RasterPixelsVisitor<TDataType>(argProvider);
            _argProvider = argProvider;
            _visitBandNos = visitBandNos;
            IFeatureComputeFuncProvider<TDataType,TFeature> prd = ExtractFuncProviderFactory.CreateFeatureComputeFuncProvider<TDataType,TFeature>(visitBandNos, express, _argProvider);
            _featureComputer = prd.GetComputeFunc();                
        }

        public void Extract(IPixelFeatureMapper<TFeature> extractedPixels)
        {
            if (_infoExtracter == null || _featureComputer == null)
                return;
            _infoExtracter.VisitPixel(_visitBandNos, (idx, values) =>
            {
                extractedPixels.Put(idx, _featureComputer(idx, values));
            });
        }
    }
}
