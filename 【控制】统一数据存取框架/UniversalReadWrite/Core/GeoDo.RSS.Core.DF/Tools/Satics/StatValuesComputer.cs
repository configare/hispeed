using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public class StatValuesComputer
    {
        public static void Stat(IBandOperator band, int interleave, out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            double begin = 0;
            double end = 0;
            MaxMinValuesComputer.GetMinMaxByDataType(band, out begin, out end);
            Stat(band, interleave, out min, out max, out mean, out stddev, isCanApprox, progressCallback);
        }

        public static void Stat(IBandOperator band, int interleave, double begin, double end, out double min, out double max, out double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            min = max = mean = stddev = 0;
            double sum = 0;
            int count = 0;
            switch (band.DataType)
            {
                case enumDataType.Byte:
                    Byte minByte = Byte.MaxValue;
                    Byte maxByte = Byte.MinValue;
                    using (BandPixelsVisitor<Byte> v = new BandPixelsVisitor<Byte>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            sum += pixel;
                            count++;
                            if (pixel < minByte)
                                minByte = pixel;
                            if (pixel > maxByte)
                                maxByte = pixel;
                        }, progressCallback);
                    }
                    min = minByte;
                    max = maxByte;
                    break;
                case enumDataType.UInt16:
                    UInt16 minUInt16 = UInt16.MaxValue;
                    UInt16 maxUInt16 = UInt16.MinValue;
                    using (BandPixelsVisitor<UInt16> v = new BandPixelsVisitor<UInt16>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            sum += pixel;
                            count++;
                            if (pixel < minUInt16)
                                minUInt16 = pixel;
                            if (pixel > maxUInt16)
                                maxUInt16 = pixel;
                        }, progressCallback);
                    }
                    min = minUInt16;
                    max = maxUInt16;
                    break;
                case enumDataType.Int16:
                    Int16 minInt16 = Int16.MaxValue;
                    Int16 maxInt16 = Int16.MinValue;
                    using (BandPixelsVisitor<Int16> v = new BandPixelsVisitor<Int16>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            sum += pixel;
                            count++;
                            if (pixel < minInt16)
                                minInt16 = pixel;
                            if (pixel > maxInt16)
                                maxInt16 = pixel;
                        }, progressCallback);
                    }
                    min = minInt16;
                    max = maxInt16;
                    break;
                case enumDataType.UInt32:
                    UInt32 minUInt32 = UInt32.MaxValue;
                    UInt32 maxUInt32 = UInt32.MinValue;
                    using (BandPixelsVisitor<UInt32> v = new BandPixelsVisitor<UInt32>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            sum += pixel;
                            count++;
                            if (pixel < minUInt32)
                                minUInt32 = pixel;
                            if (pixel > maxUInt32)
                                maxUInt32 = pixel;
                        }, progressCallback);
                    }
                    min = minUInt32;
                    max = maxUInt32;
                    break;
                case enumDataType.Int32:
                    Int32 minInt32 = Int32.MaxValue;
                    Int32 maxInt32 = Int32.MinValue;
                    using (BandPixelsVisitor<Int32> v = new BandPixelsVisitor<Int32>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            sum += pixel;
                            count++;
                            if (pixel < minInt32)
                                minInt32 = pixel;
                            if (pixel > maxInt32)
                                maxInt32 = pixel;
                        }, progressCallback);
                    }
                    min = minInt32;
                    max = maxInt32;
                    break;
                case enumDataType.Float:
                    float minFloat = float.MaxValue;
                    float maxFloat = float.MinValue;
                    using (BandPixelsVisitor<float> v = new BandPixelsVisitor<float>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            sum += pixel;
                            count++;
                            if (pixel < minFloat)
                                minFloat = pixel;
                            if (pixel > maxFloat)
                                maxFloat = pixel;
                        }, progressCallback);
                    }
                    min = minFloat;
                    max = maxFloat;
                    break;
                case enumDataType.Double:
                    double minDouble = double.MaxValue;
                    double maxDouble = double.MinValue;
                    using (BandPixelsVisitor<double> v = new BandPixelsVisitor<double>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            sum += pixel;
                            count++;
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
            mean = sum / count;
            ComputeStddev(band, interleave, begin, end, count, mean, out stddev, isCanApprox, progressCallback);
        }

        public static void ComputeStddev(IBandOperator band, int interleave, double begin, double end, int count, double mean, out double stddev, bool isCanApprox, Action<int, string> progressCallback)
        {
            stddev = 0;
            double devSum = 0;
            switch (band.DataType)
            {
                case  enumDataType.Byte:
                    using (BandPixelsVisitor<Byte> v = new BandPixelsVisitor<Byte>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            devSum += (pixel - mean) * (pixel - mean);
                        }, progressCallback);
                    }
                    break;
                case enumDataType.UInt16:
                    using (BandPixelsVisitor<UInt16> v = new BandPixelsVisitor<UInt16>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            devSum += (pixel - mean) * (pixel - mean);
                        }, progressCallback);
                    }
                    break;
                case enumDataType.Int16:
                    using (BandPixelsVisitor<Int16> v = new BandPixelsVisitor<Int16>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            devSum += (pixel - mean) * (pixel - mean);
                        }, progressCallback);
                    }
                    break;
                case enumDataType.UInt32:
                    using (BandPixelsVisitor<UInt32> v = new BandPixelsVisitor<UInt32>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            devSum += (pixel - mean) * (pixel - mean);
                        }, progressCallback);
                    }
                    break;
                case enumDataType.Int32:
                    using (BandPixelsVisitor<Int32> v = new BandPixelsVisitor<Int32>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            devSum += (pixel - mean) * (pixel - mean);
                        }, progressCallback);
                    }
                    break;
                case enumDataType.Float:
                    using (BandPixelsVisitor<float> v = new BandPixelsVisitor<float>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            devSum += (pixel - mean) * (pixel - mean);
                        }, progressCallback);
                    }
                    break;
                case enumDataType.Double:
                    using (BandPixelsVisitor<double> v = new BandPixelsVisitor<double>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel > end)
                                return;
                            devSum += (pixel - mean) * (pixel - mean);
                        }, progressCallback);
                    }
                    break;
                default:
                    throw new DataTypeIsNotSupportException(band.DataType.ToString());
            }
            stddev = Math.Sqrt(devSum / count);
        }

        public static void ComputeHistogram(IBandOperator band, int buckets, int interleave, int[] histogram, bool isCanApprox, Action<int, string> progressCallback)
        { 
            double begin = 0;
            double end = 0;
            MaxMinValuesComputer.GetMinMaxByDataType(band, out begin, out end);
            ComputeHistogram(band, buckets, interleave,begin,end, histogram, isCanApprox, progressCallback);
        }

        public static void ComputeHistogram(IBandOperator band, int buckets,int interleave, double begin, double end, int[] histogram, bool isCanApprox, Action<int, string> progressCallback)
        {            
            switch (band.DataType)
            {
                case enumDataType.Byte:
                    using (BandPixelsVisitor<Byte> v = new BandPixelsVisitor<Byte>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel >= end)
                                return;
                            histogram[pixel]++;
                        }, progressCallback);
                    }
                    break;
                case enumDataType.UInt16:
                    using (BandPixelsVisitor<UInt16> v = new BandPixelsVisitor<UInt16>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel >= end)
                                return;
                            histogram[pixel]++;
                        }, progressCallback);
                    }
                    break;
                case enumDataType.Int16:
                    using (BandPixelsVisitor<Int16> v = new BandPixelsVisitor<Int16>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel >= end)
                                return;
                            histogram[pixel - (int)begin]++;
                        }, progressCallback);
                    }
                    break;
                case enumDataType.UInt32:
                    using (BandPixelsVisitor<UInt32> v = new BandPixelsVisitor<UInt32>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel >= end)
                                return;
                            histogram[pixel]++;
                        }, progressCallback);
                    }
                    break;
                case enumDataType.Int32:
                    using (BandPixelsVisitor<Int32> v = new BandPixelsVisitor<Int32>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel >= end)
                                return;
                            histogram[pixel -(Int32)begin]++;
                        }, progressCallback);
                    }
                    break;
                case enumDataType.Float:
                    using (BandPixelsVisitor<float> v = new BandPixelsVisitor<float>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel >= end)
                                return;
                            histogram[(int)pixel - (int)begin]++;
                        }, progressCallback);
                    }
                    break;
                case enumDataType.Double:
                    using (BandPixelsVisitor<double> v = new BandPixelsVisitor<double>())
                    {
                        v.Visit(band, interleave, pixel =>
                        {
                            if (pixel < begin || pixel >= end)
                                return;
                            histogram[(int)pixel - (int)begin]++;
                        }, progressCallback);
                    }
                    break;
                default:
                    throw new DataTypeIsNotSupportException(band.DataType.ToString());
            }
        }
    }
}
