using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    internal abstract class ColorTable2RgbStretcher<T>
    {
        public object[] GetStretcher(ProductColorTable colorTable)
        {
            if (colorTable == null)
                return null;
            Dictionary<T, byte>[] map = new Dictionary<T, byte>[3]
            {
                new Dictionary<T,byte>(),
                new Dictionary<T,byte>(),
                new Dictionary<T,byte>()
            };
            GetMap(map, colorTable);
            Func<T, byte>[] sts = new Func<T, byte>[] 
            {
                GetStretcherFunc(map[0],colorTable),
                GetStretcherFunc(map[1],colorTable),
                GetStretcherFunc(map[2],colorTable)
            };
            return sts.ToArray().Cast<object>().ToArray();
        }

        private Func<T, byte> GetStretcherFunc(Dictionary<T, byte> map, ProductColorTable colorTable)
        {
            if (colorTable.ProductColors == null || colorTable.ProductColors.Length == 0)
                return null;
            return (v) =>
            {
                int count = colorTable.ProductColors.Length;
                ProductColor[] pc = colorTable.ProductColors;
                byte[] values = map.Values.ToArray();
                for (int i = 0; i < count; i++)
                {
                    if (pc[i].IsContains(T2Float(v)))
                        return values[i];
                }
                return 0;
            };
        }

        protected abstract float T2Float(T v);

        private void GetMap(Dictionary<T, byte>[] maps, ProductColorTable colorTable)
        {
            foreach (ProductColor c in colorTable.ProductColors)
            {
                maps[0].Add(Float2T(c.MinValue), c.Color.R);
                maps[1].Add(Float2T(c.MinValue), c.Color.G);
                maps[2].Add(Float2T(c.MinValue), c.Color.B);
            }
        }

        protected abstract T Float2T(float value);
    }

    internal class ColorTable2RgbStretcherByte : ColorTable2RgbStretcher<byte>
    {
        protected override byte Float2T(float value)
        {
            return (byte)value;
        }

        protected override float T2Float(byte v)
        {
            return (float)v;
        }
    }

    internal class ColorTable2RgbStretcherInt16 : ColorTable2RgbStretcher<Int16>
    {
        protected override short Float2T(float value)
        {
            return (Int16)value;
        }

        protected override float T2Float(short v)
        {
            return (float)v;
        }
    }

    internal class ColorTable2RgbStretcherUInt16 : ColorTable2RgbStretcher<UInt16>
    {
        protected override ushort Float2T(float value)
        {
            return (UInt16)value;
        }

        protected override float T2Float(ushort v)
        {
            return (float)v;
        }
    }

    internal class ColorTable2RgbStretcherInt32 : ColorTable2RgbStretcher<Int32>
    {
        protected override int Float2T(float value)
        {
            return (Int32)value;
        }

        protected override float T2Float(int v)
        {
            return (float)v;
        }
    }

    internal class ColorTable2RgbStretcherUInt32 : ColorTable2RgbStretcher<UInt32>
    {
        protected override uint Float2T(float value)
        {
            return (UInt32)value;
        }

        protected override float T2Float(uint v)
        {
            return (float)v;
        }
    }

    internal class ColorTable2RgbStretcherInt64 : ColorTable2RgbStretcher<Int64>
    {
        protected override long Float2T(float value)
        {
            return (Int64)value;
        }

        protected override float T2Float(long v)
        {
            return (float)v;
        }
    }

    internal class ColorTable2RgbStretcherUInt64 : ColorTable2RgbStretcher<UInt64>
    {
        protected override ulong Float2T(float value)
        {
            return (UInt64)value;
        }

        protected override float T2Float(ulong v)
        {
            return (float)v;
        }
    }

    internal class ColorTable2RgbStretcherDouble : ColorTable2RgbStretcher<double>
    {
        protected override double Float2T(float value)
        {
            return (double)value;
        }

        protected override float T2Float(double v)
        {
            return (float)v;
        }
    }

    internal class ColorTable2RgbStretcherFloat : ColorTable2RgbStretcher<float>
    {
        protected override float Float2T(float value)
        {
            return value;
        }

        protected override float T2Float(float v)
        {
            return v;
        }

    }
}
