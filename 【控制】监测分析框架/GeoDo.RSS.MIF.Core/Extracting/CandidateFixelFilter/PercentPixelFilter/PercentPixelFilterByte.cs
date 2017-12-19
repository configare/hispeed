using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.MIF.Core
{
    public class PercentPixelFilterByte : CandidatePixelFilter, IBandOperator
    {
        protected float _percent;
        protected int _bandNo;
        protected IRasterBand _band;
        protected enumDataType _dataType;
        protected int _height, _width;
        protected bool _is_ASC_Order;

        public PercentPixelFilterByte(int bandNo, float percent, bool is_ASC_Order)
        {
            _percent = percent;
            _bandNo = bandNo;
            _is_ASC_Order = is_ASC_Order;
        }

        protected override int[] DoFilter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi)
        {
            return DoFilter(dataProvider, aoiRect, aoi, null);
        }

        enumDataType IBandOperator.DataType
        {
            get { return _dataType; }
        }

        int IBandOperator.Height
        {
            get { return _height; }
        }

        void IBandOperator.Read(int xOffset, int yOffset, int xSize, int ySize, IntPtr buffer, enumDataType dataType, int xBufferSize, int yBufferSize)
        {
            _band.Read(xOffset, yOffset, xSize, ySize, buffer, dataType, xBufferSize, yBufferSize);
        }

        int IBandOperator.Width
        {
            get { return _width; }
        }

        protected override int[] DoFilter(IRasterDataProvider dataProvider, Rectangle aoiRect, int[] aoi, byte[] assistInfo)
        {
            _band = dataProvider.GetRasterBand(_bandNo);
            _dataType = _band.DataType;
            _height = _band.Height;
            _width = _band.Width;
            int maxPixelsCount = (int)(_width * _height * _percent);
            int[] map = new int[byte.MaxValue + 1];
            using (BandPixelsVisitor<byte> visitor = new BandPixelsVisitor<byte>())
            {
                visitor.Visit(this, (idx, t) => { map[t]++; }, null);
                int count = 0;
                byte beginValue = 0;
                if (_is_ASC_Order)
                {
                    for (byte i = 0; i <= byte.MaxValue; i++)
                    {
                        count += map[i];
                        if (count > maxPixelsCount)
                        {
                            beginValue = i;
                            break;
                        }
                    }
                }
                else
                {
                    for (byte i = byte.MaxValue; i >= 0; i--)
                    {
                        count += map[i];
                        if (count > maxPixelsCount)
                        {
                            beginValue = i;
                            break;
                        }
                    }
                }
                List<int> idxes = new List<int>();
                if (_is_ASC_Order)
                {
                    visitor.Visit(this,
                        (idx, t) =>
                        {
                            if (t <= beginValue)
                            {
                                idxes.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        }, null);
                }
                else
                {
                    visitor.Visit(this,
                        (idx, t) =>
                        {
                            if (t >= beginValue)
                            {
                                idxes.Add(idx);
                                if (assistInfo != null)
                                    assistInfo[idx] = 1;
                            }
                        }, null);
                }
                return idxes.Count > 0 ? idxes.ToArray() : null;
            }
        }
    }
}
