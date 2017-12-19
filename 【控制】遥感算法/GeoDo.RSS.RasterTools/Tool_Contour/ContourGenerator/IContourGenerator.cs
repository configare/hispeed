using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using System.Runtime.InteropServices;

namespace GeoDo.RSS.RasterTools
{
    internal static class ContourGeneratorFactory
    {
        public static IContourGenerator GetContourGenerator(enumDataType dataType)
        {
            switch(dataType)
            {
                case enumDataType.Byte:
                    return new ContourGeneratorByte();
                case enumDataType.UInt16:
                    return new ContourGeneratorUInt16();
                case enumDataType.Int16:
                    return new ContourGeneratorInt16();
                case enumDataType.UInt32:
                    return new ContourGeneratorUInt32();
                case enumDataType.Int32:
                    return new ContourGeneratorInt32();
                case enumDataType.Int64:
                    return new ContourGeneratorInt64();
                case enumDataType.UInt64:
                    return new ContourGeneratorInt64();
                case enumDataType.Float:
                    return new ContourGeneratorFloat();
                case enumDataType.Double:
                    return new ContourGeneratorDouble();
                default:
                    return null;
            }
        }
    }

    internal interface IContourGenerator:IDisposable
    {
        bool IsOutputUncompleted { get; set; }
        double NoDataForOutsideAOI{get;set;}
        int Sample { get; set; }
        double[] ContourValues { get; }
        ContourLine[] Generate(IRasterBand raster, double[] contourValues, int[] aoi, Action<int, string> tracker);
        //Begin 空间插值添加
        ContourLine[] Generate(int width, int height, double[] contourValues, Action<int, string> tracker);
        void SetDataValue(double[] pointValue, int width, int height);
        //End
    }
}
