using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing.Imaging;

namespace GeoDo.RSS.Core.RasterDrawing
{
    public static class BitmapBuilderFactory
    {
        public static ColorPalette GetDefaultGrayColorPalette()
        {
            return BitmapBuilder<byte>.GetDefaultGrayColorPalette();
        }

        public static IBitmapBuilder<byte> CreateBitmapBuilderByte()
        {
            BitmapBuilder<byte> builder = new BitmapBuilder<byte>();
            Func<byte, byte> defaultStretcher = (v) => { return v; };
            builder.SetDefaultStretcher(defaultStretcher, defaultStretcher, defaultStretcher, defaultStretcher);
            return builder;
        }

        public static IBitmapBuilder<UInt16> CreateBitmapBuilderUInt16()
        {
            BitmapBuilder<UInt16> builder = new BitmapBuilder<UInt16>();
            Func<UInt16, byte> defaultStretcher = (v) => { return (byte)v; };
            builder.SetDefaultStretcher(defaultStretcher, defaultStretcher, defaultStretcher, defaultStretcher);
            return builder;
        }

        public static IBitmapBuilder<Int16> CreateBitmapBuilderInt16()
        {
            BitmapBuilder<Int16> builder = new BitmapBuilder<Int16>();
            Func<Int16, byte> defaultStretcher = (v) => { return (byte)v; };
            builder.SetDefaultStretcher(defaultStretcher, defaultStretcher, defaultStretcher, defaultStretcher);
            return builder;
        }

        public static IBitmapBuilder<Int32> CreateBitmapBuilderInt32()
        {
            BitmapBuilder<Int32> builder = new BitmapBuilder<Int32>();
            Func<Int32, byte> defaultStretcher = (v) => { return (byte)v; };
            builder.SetDefaultStretcher(defaultStretcher, defaultStretcher, defaultStretcher, defaultStretcher);
            return builder;
        }

        public static IBitmapBuilder<UInt32> CreateBitmapBuilderUInt32()
        {
            BitmapBuilder<UInt32> builder = new BitmapBuilder<UInt32>();
            Func<UInt32, byte> defaultStretcher = (v) => { return (byte)v; };
            builder.SetDefaultStretcher(defaultStretcher, defaultStretcher, defaultStretcher, defaultStretcher);
            return builder;
        }

        public static IBitmapBuilder<Int64> CreateBitmapBuilderInt64()
        {
            BitmapBuilder<Int64> builder = new BitmapBuilder<Int64>();
            Func<Int64, byte> defaultStretcher = (v) => { return (byte)v; };
            builder.SetDefaultStretcher(defaultStretcher, defaultStretcher, defaultStretcher, defaultStretcher);
            return builder;
        }

        public static IBitmapBuilder<UInt64> CreateBitmapBuilderUInt64()
        {
            BitmapBuilder<UInt64> builder = new BitmapBuilder<UInt64>();
            Func<UInt64, byte> defaultStretcher = (v) => { return (byte)v; };
            builder.SetDefaultStretcher(defaultStretcher, defaultStretcher, defaultStretcher, defaultStretcher);
            return builder;
        }

        public static IBitmapBuilder<float> CreateBitmapBuilderFloat()
        {
            BitmapBuilder<float> builder = new BitmapBuilder<float>();
            Func<float, byte> defaultStretcher = (v) => { return (byte)v; };
            builder.SetDefaultStretcher(defaultStretcher, defaultStretcher, defaultStretcher, defaultStretcher);
            return builder;
        }

        public static IBitmapBuilder<double> CreateBitmapBuilderDouble()
        {
            BitmapBuilder<double> builder = new BitmapBuilder<double>();
            Func<double, byte> defaultStretcher = (v) => { return (byte)v; };
            builder.SetDefaultStretcher(defaultStretcher, defaultStretcher, defaultStretcher, defaultStretcher);
            return builder;
        }
    }
}
