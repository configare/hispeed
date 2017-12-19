#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/27 13:43:40
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.FY1D
{
    /// <summary>
    /// 类名：ToLocalEndian
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/27 13:43:40
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public static class ToLocalEndian
    {
        public static int ToInt32FromBig(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 4)
                return 0;
            byte[] bs = new byte[4];
            bs[3] = bytes[0];
            bs[2] = bytes[1];
            bs[1] = bytes[2];
            bs[0] = bytes[3];
            return BitConverter.ToInt32(bs, 0);
        }

        public static int ToInt32FromLittle(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 4)
                return 0;
            return BitConverter.ToInt32(bytes, 0);
        }

        public static double ToDouble64FromBig(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 8)
                return 0;
            byte[] bs = new byte[8];
            bs[7] = bytes[0];
            bs[6] = bytes[1];
            bs[5] = bytes[2];
            bs[4] = bytes[3];
            bs[3] = bytes[4];
            bs[2] = bytes[5];
            bs[1] = bytes[6];
            bs[0] = bytes[7];
            return BitConverter.ToDouble(bs, 0);
        }

        public static Int64 ToInt64FromBig(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 8)
                return 0;
            byte[] bs = new byte[8];
            bs[7] = bytes[0];
            bs[6] = bytes[1];
            bs[5] = bytes[2];
            bs[4] = bytes[3];
            bs[3] = bytes[4];
            bs[2] = bytes[5];
            bs[1] = bytes[6];
            bs[0] = bytes[7];
            return BitConverter.ToInt64(bs, 0);
        }

        public static Int64 ToInt64FromLittle(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 8)
                return 0;
            return BitConverter.ToInt64(bytes, 0);
        }

        public static UInt64 ToUInt64FromBig(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 8)
                return 0;
            byte[] bs = new byte[8];
            bs[7] = bytes[0];
            bs[6] = bytes[1];
            bs[5] = bytes[2];
            bs[4] = bytes[3];
            bs[3] = bytes[4];
            bs[2] = bytes[5];
            bs[1] = bytes[6];
            bs[0] = bytes[7];
            return BitConverter.ToUInt64(bs, 0);
        }

        public static UInt64 ToUInt64FromLittle(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 8)
                return 0;
            return BitConverter.ToUInt64(bytes, 0);
        }

        public static double ToDouble64FromLittle(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 8)
                return 0;
            return BitConverter.ToDouble(bytes, 0);
        }

        public static string ReadString(char[] chars)
        {
            if (chars == null)
                return "";
            string retString = null;
            foreach (char c in chars)
                retString += c.ToString();
            return retString.Trim();
        }

        public static string ReadString2(char[] chars)
        {
            if (chars == null)
                return "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in chars)
                sb.Append(c);
            return sb.ToString().Trim();
        }

        public static Int16 ToInt16FromBig(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 2)
                return 0;
            byte[] bs = new byte[2];
            bs[1] = bytes[0];
            bs[0] = bytes[1];
            return BitConverter.ToInt16(bs, 0);
        }

        public static Int16 ToInt16FromLittle(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 2)
                return 0;
            return BitConverter.ToInt16(bytes, 0);
        }

        public static UInt16 ToUInt16FromBig(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 2)
                return 0;
            byte[] bs = new byte[2];
            bs[1] = bytes[0];
            bs[0] = bytes[1];
            return BitConverter.ToUInt16(bs, 0);
        }

        public static UInt16 ToUInt16FromLittle(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 2)
                return 0;
            return BitConverter.ToUInt16(bytes, 0);
        }


        public static float ToFloatFromBig(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 4)
                return 0;
            byte[] bs = new byte[4];
            bs[3] = bytes[0];
            bs[2] = bytes[1];
            bs[1] = bytes[2];
            bs[0] = bytes[3];
            return BitConverter.ToSingle(bs, 0);
        }

        public static float ToFloatFromLittle(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 4)
                return 0;
            return BitConverter.ToSingle(bytes, 0);
        }

        public static UInt32 ToUInt32FromBig(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 4)
                return 0;
            byte[] bs = new byte[4];
            bs[3] = bytes[0];
            bs[2] = bytes[1];
            bs[1] = bytes[2];
            bs[0] = bytes[3];
            return BitConverter.ToUInt32(bs, 0);
        }

        public static UInt32 ToUInt32FromLittle(byte[] bytes)
        {
            if (bytes == null || bytes.Length != 4)
                return 0;
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}
