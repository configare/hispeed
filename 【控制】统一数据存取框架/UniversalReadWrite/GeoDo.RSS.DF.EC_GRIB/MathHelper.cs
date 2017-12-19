using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.EC_GRIB
{
    public static class MathHelper
    {
        public static int Bytes2Int(byte[] bytes)
        {
            //int value = 0;
            //int count = bytes.Length;
            //for (int i = 0; i < count; i++)
            //{
            //    value += bytes[i] * (int)Math.Pow(16, 2 * (count - 1 - i));
            //}
            bytes = bytes.Reverse().ToArray();
            return BitConverter.ToInt16(bytes, 0);
        }

        public static float Bytes2Float(byte[] bytes)
        {
            bytes = bytes.Reverse().ToArray();
            return BitConverter.ToSingle(bytes, 0);
        }
    }
}
