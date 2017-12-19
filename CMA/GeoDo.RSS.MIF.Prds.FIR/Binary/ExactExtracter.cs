using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    internal class ExactExtracter
    {
        protected IContextMessage _contextMessage;
        //候选火点像元
        protected IPixelIndexMapper _candidateFirPixels;
        //待判识火点像元
        protected IPixelIndexMapper _waitingFirPixels;

        public ExactExtracter()
        {
            CreateFilterPipe();
        }

        private void CreateFilterPipe()
        {
        }

        public IExtractResult Extracting(IArgumentProvider argProvider, out Dictionary<int, PixelFeature> features, IContextMessage contextMessage, Action<int, string> progressTracker)
        {
            _contextMessage = contextMessage;
            CreateMapper(argProvider);
            features = null;
            DoFilters(argProvider, out features, progressTracker);
            return _candidateFirPixels;
            //return GetOtherExtractResult.GetExtractResult(argProvider, features, _candidateFirPixels, contextMessage, progressTracker);
        }

        private void CreateMapper(IArgumentProvider argProvider)
        {
            if (_candidateFirPixels != null)
                _candidateFirPixels.Dispose();
            if (_waitingFirPixels != null)
                _waitingFirPixels.Dispose();
            _candidateFirPixels = PixelIndexMapperFactory.CreatePixelIndexMapper("FIR", argProvider.DataProvider.Width, argProvider.DataProvider.Height, argProvider.DataProvider.CoordEnvelope, argProvider.DataProvider.SpatialRef);
            _waitingFirPixels = PixelIndexMapperFactory.CreatePixelIndexMapper("FIR", argProvider.DataProvider.Width, argProvider.DataProvider.Height, argProvider.DataProvider.CoordEnvelope, argProvider.DataProvider.SpatialRef);
        }

        private void DoFilters(IArgumentProvider argProvider, out Dictionary<int, PixelFeature> features, Action<int, string> progressTracker)
        {
            features = null;
            ICandidatePixelFilter filter = null;
            int[] aoi = argProvider.AOI;
            Size size = new Size(argProvider.DataProvider.Width, argProvider.DataProvider.Height);
            Rectangle aoiRect = AOIHelper.ComputeAOIRect(aoi, size);
            int[] filteredAOI = null;
            if (progressTracker != null)
                progressTracker.Invoke(5, "正在过滤高温象元,请稍候...");
            //4% 高温像元(中红外通道)
            CreateArgument.PrintInfo(_contextMessage, "[开始]x%高温像元过滤...");
            filter = CreateArgument.CreateHighTmpXPercentFilter(argProvider, _contextMessage);
            if (filter == null)
                return;
            filteredAOI = filter.Filter(argProvider.DataProvider, aoiRect, aoi);
            //by chennan 20130402 感兴趣内计算
            UpdateFilteredAOIByAOI(ref filteredAOI, argProvider.AOI);
            if (filteredAOI != null)
            {
                _waitingFirPixels.Put(filteredAOI);
                CreateArgument.PrintInfo(_contextMessage, "     x%高温点个数:" + filteredAOI.Length.ToString());
            }
            CreateArgument.PrintInfo(_contextMessage, "[完成]x%高温像元过滤。");
            if (progressTracker != null)
                progressTracker.Invoke(10, "正在提取陆地高温点,请稍候...");
            //陆地高温点判识
            CreateArgument.PrintInfo(_contextMessage, "[开始]陆地高温点过滤...");
            filter = CreateArgument.CreateLandHighTmpFilter(argProvider, _contextMessage);
            //by chennan 20130402 感兴趣内计算
            UpdateFilteredAOIByAOI(ref filteredAOI, argProvider.AOI);
            if (filter != null)
            {
                aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
                filteredAOI = filter.Filter(argProvider.DataProvider, aoiRect, filteredAOI);
                if (filteredAOI != null)
                {
                    _candidateFirPixels.Put(filteredAOI);
                    _waitingFirPixels.Remove(filteredAOI);
                    CreateArgument.PrintInfo(_contextMessage, "     陆地高温点个数:" + filteredAOI.Length.ToString() + ",候选像元个数:" + _candidateFirPixels.Indexes.Count().ToString());
                }
            }
            CreateArgument.PrintInfo(_contextMessage, "[完成]陆地高温点过滤。");
            if (progressTracker != null)
                progressTracker.Invoke(20, "正在提取异常高温点,请稍候...");
            //异常高温点判识
            CreateArgument.PrintInfo(_contextMessage, "[开始]邻域内异常高温点过滤...");
            filter = CreateArgument.CreateAbnormalHighTmpFile(argProvider, _contextMessage);
            //by chennan 20130402 感兴趣内计算
            UpdateFilteredAOIByAOI(ref filteredAOI, argProvider.AOI);
            if (filter != null)
            {
                filteredAOI = _waitingFirPixels.Indexes.ToArray();
                aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
                filteredAOI = filter.Filter(argProvider.DataProvider, aoiRect, filteredAOI);
                //by chennan 20130402 感兴趣内计算
                UpdateFilteredAOIByAOI(ref filteredAOI, argProvider.AOI);
                if (filteredAOI != null)
                {
                    _candidateFirPixels.Put(filteredAOI);
                    _waitingFirPixels.Remove(filteredAOI);
                    CreateArgument.PrintInfo(_contextMessage, "     异常高温点个数:" + filteredAOI.Length.ToString() + ",候选像元个数:" + _candidateFirPixels.Indexes.Count().ToString());
                }
            }
            CreateArgument.PrintInfo(_contextMessage, "[完成]邻域内异常高温点过滤。");
            if (progressTracker != null)
                progressTracker.Invoke(30, "正在提取疑似高温点,请稍候...");
            //疑似火点判识
            CreateArgument.PrintInfo(_contextMessage, "[开始]疑似火点判识...");
            DoubtFirPixelFilter doubtFilter = CreateArgument.CreateDoubtFilter(argProvider, _contextMessage);
            if (doubtFilter == null)
                return;
            filteredAOI = _candidateFirPixels.Indexes.ToArray();
            aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
            filteredAOI = doubtFilter.Filter(argProvider, aoiRect, filteredAOI);
            //by chennan 20130402 感兴趣内计算
            UpdateFilteredAOIByAOI(ref filteredAOI, argProvider.AOI);
            if (filteredAOI == null)
            {
                CreateArgument.PrintInfo(_contextMessage, "     没有监测出疑似火点");
                _candidateFirPixels.Reset();
                return;
            }
            _candidateFirPixels.Reset();
            _candidateFirPixels.Put(filteredAOI);
            CreateArgument.PrintInfo(_contextMessage, "[完成]疑似火点判识。");
            CreateArgument.PrintInfo(_contextMessage, "     监测出疑似火点:" + filteredAOI.Length.ToString());
            if (progressTracker != null)
                progressTracker.Invoke(50, "正在计算背景温度,请稍候...");
            //背景温度计算
            CreateArgument.PrintInfo(_contextMessage, "[开始]背景亮温计算...");
            BackTmpComputer backTmpComputer = CreateArgument.CreateBackTmpComputer(argProvider, doubtFilter as IBackTmpComputerHelper, _contextMessage);
            if (backTmpComputer == null)
                return;
            aoiRect = AOIHelper.ComputeAOIRect(filteredAOI, size);
            features = backTmpComputer.Compute(argProvider, aoiRect, filteredAOI);
            //记录火点特征集合
            _candidateFirPixels.Tag = new FirFeatureCollection("火点像元特征", features);
            CreateArgument.PrintInfo(_contextMessage, "     为" + features.Count.ToString() + "个疑似火点计算了背景亮温");
            CreateArgument.PrintInfo(_contextMessage, "[完成]背景亮温计算。");
            if (progressTracker != null)
                progressTracker.Invoke(70, "正在进行火点确认,请稍候...");
            //火点确认
            CreateArgument.PrintInfo(_contextMessage, "[开始]火点确认...");
            VertifyFirPixelFiter vertifyFilter = CreateArgument.CreateVertifyFilter(argProvider, _contextMessage);
            if (vertifyFilter == null)
                return;
            filteredAOI = vertifyFilter.Filter(argProvider, features);
            _candidateFirPixels.Reset();
            if (filteredAOI != null && filteredAOI.Length > 0)
            {
                _candidateFirPixels.Put(filteredAOI);
                CreateArgument.PrintInfo(_contextMessage, "     确认火点数:" + filteredAOI.Length.ToString());
                GetFirePointIndeiex.WriteFireIndexiexFilename(argProvider, features.Keys.ToArray());
            }
            CreateArgument.PrintInfo(_contextMessage, "[完成]火点确认。");
        }

        /// <summary>
        /// 依据感兴趣区域修正过滤AOI
        /// </summary>
        /// <param name="filteredAOI"></param>
        /// <param name="aoiIndex"></param>
        private void UpdateFilteredAOIByAOI(ref int[] filteredAOI, int[] aoiIndex)
        {
            if (aoiIndex == null || filteredAOI == null)
                return;
            IEnumerable<int> result = filteredAOI.Intersect(aoiIndex);
            if (result == null || result.Count() == 0)
            {
                filteredAOI = null;
                return;
            }
            filteredAOI = result.ToArray();
        }
    }
}
