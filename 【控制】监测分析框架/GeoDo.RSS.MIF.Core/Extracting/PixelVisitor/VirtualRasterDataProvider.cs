using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.MIF.Core
{
    public class VirtualRasterDataProvider : RasterDataProvider, IVirtualRasterDataProvider
    {
        public VirtualRasterDataProvider(IRasterDataProvider[] dataProviders)
            : base(null, null)
        {
            if (dataProviders == null || dataProviders.Length == 0)
                throw new ArgumentNullException("dataProviders");
            if (IsMatched(dataProviders))
                LoadBands(dataProviders);
            SetFields(dataProviders[0]);
        }

        private void SetFields(IRasterDataProvider prd)
        {
            _width = prd.Width;
            _height = prd.Height;
            _dataType = prd.DataType;
            if (prd.CoordEnvelope != null)
                _coordEnvelope = prd.CoordEnvelope.Clone();
        }

        private bool IsMatched(IRasterDataProvider[] dataProviders)
        {
            if (dataProviders[0] is IVirtualScan0)
            {
                return IsMatch(dataProviders, (p1, p2) => { return p1.DataType == p2.DataType; });
            }
            if (IsMatch(dataProviders, (p1, p2) => { return p1.Width == p2.Width && p1.Height == p2.Height; }))
                if (IsMatch(dataProviders, (p1, p2) => { return p1.DataType == p2.DataType; }))
                    return true;
            return false;
        }

        private bool IsMatch(IRasterDataProvider[] dataProviders, Func<IRasterDataProvider, IRasterDataProvider, bool> checker)
        {
            for (int i = 0; i < dataProviders.Length - 1; i++)
            {
                if (!checker(dataProviders[i], dataProviders[i + 1]))
                    throw new ArgumentException("两个栅格数据提供者数据类型或大小不一致,无法构建虚拟数据提供者！");
            }
            return true;
        }

        private void LoadBands(IRasterDataProvider[] dataProviders)
        {
            foreach (IRasterDataProvider prd in dataProviders)
                for (int b = 0; b < prd.BandCount; b++)
                    _rasterBands.Add(prd.GetRasterBand(b + 1));
            _bandCount = _rasterBands.Count;
        }

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            _rasterBands.Clear();
            base.Dispose();
        }
    }
}
