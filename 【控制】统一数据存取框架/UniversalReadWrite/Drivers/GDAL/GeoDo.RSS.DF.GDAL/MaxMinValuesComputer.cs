using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OSGeo.GDAL;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.GDAL
{
    internal class MaxMinValuesComputer
    {
        public static void ComputeMinMax(Band band,int interleave, double begin, double end, out double min, out double max, bool isCanApprox, Action<int, string> progressCallback)
        {
            min = max = 0;
            switch (band.DataType)
            {
                case DataType.GDT_Byte:
                    Byte minByte = Byte.MaxValue;
                    Byte maxByte = Byte.MinValue;
                    using (BandPixelsVisitor<Byte> v = new BandPixelsVisitor<Byte>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            if (pixel < minByte)
                                minByte = pixel;
                            if (pixel > maxByte)
                                maxByte = pixel;
                        },progressCallback);
                    }
                    min = minByte;
                    max = maxByte;
                    break;
                case DataType.GDT_UInt16:
                    UInt16 minUInt16 = UInt16.MaxValue;
                    UInt16 maxUInt16 = UInt16.MinValue;
                    using (BandPixelsVisitor<UInt16> v = new BandPixelsVisitor<UInt16>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            if (pixel < minUInt16)
                                minUInt16 = pixel;
                            if (pixel > maxUInt16)
                                maxUInt16 = pixel;
                        }, progressCallback);
                    }
                    min = minUInt16;
                    max = maxUInt16;
                    break;
                case DataType.GDT_Int16:
                    Int16 minInt16 = Int16.MaxValue;
                    Int16 maxInt16 = Int16.MinValue;
                    using (BandPixelsVisitor<Int16> v = new BandPixelsVisitor<Int16>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            if (pixel < minInt16)
                                minInt16 = pixel;
                            if (pixel > maxInt16)
                                maxInt16 = pixel;
                        }, progressCallback);
                    }
                    min = minInt16;
                    max = maxInt16;
                    break;
                case DataType.GDT_UInt32:
                    UInt32 minUInt32 = UInt32.MaxValue;
                    UInt32 maxUInt32 = UInt32.MinValue;
                    using (BandPixelsVisitor<UInt32> v = new BandPixelsVisitor<UInt32>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            if (pixel < minUInt32)
                                minUInt32 = pixel;
                            if (pixel > maxUInt32)
                                maxUInt32 = pixel;
                        }, progressCallback);
                    }
                    min = minUInt32;
                    max = maxUInt32;
                    break;
                case DataType.GDT_Int32:
                    Int32 minInt32 = Int32.MaxValue;
                    Int32 maxInt32 = Int32.MinValue;
                    using (BandPixelsVisitor<Int32> v = new BandPixelsVisitor<Int32>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            if (pixel < minInt32)
                                minInt32 = pixel;
                            if (pixel > maxInt32)
                                maxInt32 = pixel;
                        }, progressCallback);
                    }
                    min = minInt32;
                    max = maxInt32;
                    break;
                case DataType.GDT_Float32:
                    float minFloat = float.MaxValue;
                    float maxFloat = float.MinValue;
                    using (BandPixelsVisitor<float> v = new BandPixelsVisitor<float>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            if (pixel < minFloat)
                                minFloat = pixel;
                            if (pixel > maxFloat)
                                maxFloat = pixel;
                        }, progressCallback);
                    }
                    min = minFloat;
                    max = maxFloat;
                    break;
                case DataType.GDT_Float64:
                    double minDouble = double.MaxValue;
                    double maxDouble = double.MinValue;
                    using (BandPixelsVisitor<double> v = new BandPixelsVisitor<double>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            if (pixel < minDouble)
                                minDouble = pixel;
                            if (pixel > maxDouble)
                                maxDouble = pixel;
                        }, progressCallback);
                    }
                    min = minDouble;
                    max = maxDouble;
                    break;
                default:
                    throw new DataTypeIsNotSupportException(band.DataType.ToString());
            }
        }
    }
}
