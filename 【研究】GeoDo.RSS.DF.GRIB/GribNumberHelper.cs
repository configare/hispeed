using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    public static class GribNumberHelper
    {
        /// <summary>
        /// 将两个字节转换为无符号整型
        /// </summary>
        /// <param name="raf">文件流读取参数</param>
        public static int Uint2(Stream raf)
        {
            //byte[] bytes = ReadBytes(raf, 2);
            //return (uint)BitConverter.ToUInt16(bytes, 0); 

            int a = raf.ReadByte();
            int b = raf.ReadByte();
            return a << 8 | b;
        }

        public static int Uint2(int a, int b)
        {
            return a << 8 | b;
        }

        /// <summary>
        /// 将两个字节转换为有符号整型
        /// </summary>
        /// <param name="raf">文件流读取参数</param>
        public static int Int2(Stream raf)
        {
            //byte[] bytes = ReadBytes(raf, 2);
            //return (int)BitConverter.ToInt16(bytes, 0);

            int a = raf.ReadByte();
            int b = raf.ReadByte();
            return Int2(a, b);
        }

        public static int Int2(int a, int b)
        {
            return (1 - ((a & 128) >> 6)) * ((a & 127) << 8 | b);
        }

        /// <summary>
        /// 将三个字节转换为有符号整型
        /// </summary>
        /// <param name="raf">文件流读取参数</param>
        public static int Int3(Stream raf)
        {
            //byte[] bytes = ReadBytes(raf, 3);
            //byte[] fourBytes = new byte[4];
            //bytes.CopyTo(fourBytes, 0);
            //fourBytes[3] = 0;
            //return BitConverter.ToInt32(fourBytes, 0);

            int a = raf.ReadByte();
            int b = raf.ReadByte();
            int c = raf.ReadByte();
            return (1 - ((a & 128) >> 6)) * ((a & 127) << 16 | b << 8 | c);
        }

        /// <summary>
        /// 将三个字节转换为无符号整型
        /// </summary>
        /// <param name="raf"></param>
        public static int Uint3(Stream raf)
        {
            //byte[] bytes = ReadBytes(raf, 3);
            //byte[] fourBytes = new byte[4];
            //bytes.CopyTo(fourBytes, 0);
            //fourBytes[3] = 0;
            //return (int)BitConverter.ToUInt32(fourBytes, 0);

            int a = raf.ReadByte();
            int b = raf.ReadByte();
            int c = raf.ReadByte();
            int ret = a << 16 | b << 8 | c;

            //raf.Seek(-3, SeekOrigin.Current);
            //byte[] bt = new byte[3];
            //raf.Read(bt, 0, 3);
            //if (ret != (bt[0] << 16 | bt[1] << 8 | bt[2]))
            //    throw new Exception();

            return ret;
        }

        /// <summary>
        /// 将四个字节转换为有符号整型
        /// </summary>
        /// <param name="raf">文件流读取参数</param>
        public static int Int4(Stream raf)
        {
            int a = raf.ReadByte();
            int b = raf.ReadByte();
            int c = raf.ReadByte();
            int d = raf.ReadByte();
            // all bits set to ones
            if (a == 0xff && b == 0xff && c == 0xff && d == 0xff)
                return -9999;
            int i2 = (1 - ((a & 128) >> 6)) * ((a & 127) << 24 | b << 16 | c << 8 | d);
            return i2;
        }

        public static float Float4(Stream raf)
        {
            int a = raf.ReadByte();
            int b = raf.ReadByte();
            int c = raf.ReadByte();
            int d = raf.ReadByte();

            int sgn, mant, exp;

            mant = b << 16 | c << 8 | d;
            if (mant == 0) return 0.0f;

            sgn = -(((a & 128) >> 6) - 1);
            exp = (a & 127) - 64;

            return (float)(sgn * Math.Pow(16.0, exp - 6) * mant);
        }

        /// <summary>
        /// 将八字节转换为有符号长整型
        /// </summary>
        /// <param name="raf">RandomAccessFile</param>
        public static long Int8(Stream raf)
        {
            int a = raf.ReadByte();
            int b = raf.ReadByte();
            int c = raf.ReadByte();
            int d = raf.ReadByte();
            int e = raf.ReadByte();
            int f = raf.ReadByte();
            int g = raf.ReadByte();
            int h = raf.ReadByte();
            return (1 - ((a & 128) >> 6)) * ((a & 127) << 56 | b << 48 | c << 40 | d << 32 | e << 24 | f << 16 | g << 8 | h);
        }

        public static float IEEEfloat4(System.IO.Stream raf)
        {
            byte[] bytes = ReadBytes(raf, 4);
            return BitConverter.ToSingle(bytes, 0);
        }

        private static byte[] ReadBytes(Stream raf, int count)
        {
            byte[] bytes = new byte[count];
            raf.Read(bytes, 0, count);
            if (BitConverter.IsLittleEndian)
            {
                bytes = Swap(bytes);
            }
            return bytes;
        }

        public static byte[] Swap(byte[] bytes)
        {
            int nb = bytes.Length;
            byte[] swappedBytes = new byte[nb];
            for (int i = 0; i < nb; i++)
            {
                swappedBytes[i] = bytes[nb - 1 - i];
            }
            return swappedBytes;
        }
    }
}
