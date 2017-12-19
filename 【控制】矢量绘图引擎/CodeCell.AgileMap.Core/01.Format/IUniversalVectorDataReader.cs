using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    /// <summary>
    /// 通用数据读取接口
    /// </summary>
    public interface IUniversalVectorDataReader : IDisposable
    {
        /// <summary>
        /// 是否为正常可读状态
        /// </summary>
        bool IsOK { get; }
        /// <summary>
        /// 尝试打开文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="args">预留参数</param>
        /// <returns></returns>
        bool TryOpen(string filename,byte[] bytes, params object[] args);
    }
}
