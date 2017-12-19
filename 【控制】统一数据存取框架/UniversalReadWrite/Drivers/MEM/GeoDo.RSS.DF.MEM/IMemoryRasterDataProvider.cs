using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.MEM
{
    public interface IMemoryRasterDataProvider:IVirtualScan0
    {
        int HeaderSize { get; }
        string MmfName { get; }
        /// <summary>
        /// 设置扩展头
        /// </summary>
        /// <typeparam name="TExtHanderStruct"></typeparam>
        /// <param name="extHeader"></param>
        void SetExtHeader<TExtHanderStruct>(TExtHanderStruct extHeader);
        /// <summary>
        /// 获取扩展头
        /// </summary>
        /// <typeparam name="TExtHanderStruct"></typeparam>
        /// <returns></returns>
        TExtHanderStruct GetExtHeader<TExtHanderStruct>();
    }
}
