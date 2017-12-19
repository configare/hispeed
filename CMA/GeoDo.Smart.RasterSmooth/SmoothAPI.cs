using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace GeoDo.Smart.RasterSmooth
{
    public class SmoothAPI
    {
        /// <summary>
        /// 平滑inData为outData
        /// 都是像元优先级
        /// </summary>
        /// <param name="inData">输入数据序列(数据组织为:像元1文件1,像元1文件2,...)</param>
        /// <param name="outData">输出数据序列</param>
        /// <param name="iNum">数据序列个数</param>
        /// <param name="iCol">像元个数(iNum*iCol = inData.length)</param>
        /// <param name="iMaxValid">输入数据的最大有效值</param>
        /// <param name="iMinValid">输入数据的最小有效值</param>
        /// <param name="iCoe">输入数据的放大倍数</param>
        /// <param name="iInvalid">连续异常值个数下限</param>
        /// <param name="inInvalid">输入数据的无效数据(背景数据)值</param>
        /// <param name="outInvalid">输出数据的无效数据(背景数据)值</param>
        /// <param name="absValue">前后两次差值</param>
        /// <returns></returns>
        [DllImport(@"Smooth.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static unsafe int SmoothAPIInt16(short* inData, short* outData, int iNum, int iCol, int iMaxValid, int iMinValid, int iCoe, int iInvalid, int inInvalid, int outInvalid, double absValue);

        [DllImport(@"Smooth.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public extern static unsafe int SmoothAPIUInt16(ushort* inData, ushort* outData, int iNum, int iCol, int iMaxValid, int iMinValid, int iCoe, int iInvalid, int inInvalid, int outInvalid, double absValue);
    }
}
