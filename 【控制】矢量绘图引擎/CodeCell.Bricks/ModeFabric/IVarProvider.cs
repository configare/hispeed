using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.Bricks.ModelFabric
{
    /// <summary>
    /// 变量提供者
    /// 每一次Task执行过程中都回调用VarPrivider.GetVarValue
    /// 内部记录哪些数据处理完了，哪些没有处理
    /// 例如：5分钟段积雪产品HDF文件扫描
    /// 扫描方式：1）.Timer扫描磁盘    List<ProcessedFilenanmes> .NextFiles()
    ///                  2）.数据中检索新文件 GetNewestFileFormDB()
    /// QualityChecker:var:env@InputFilename
    /// </summary>
    public interface IVarProvider
    {
        object GetVarValue(enumVarType varType, string varName);
    }
}
