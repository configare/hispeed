using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.RemoveLines
{
    public interface IHDF5RemoveLines
    {
        /// <summary>
        /// HDF5数据去条带
        /// </summary>
        /// <param name="bandNo">指定波段号的数据集</param>
        /// <param name="inputFileName">需要操作的文件</param>
        /// <param name="writeMode">操作模式</param>
        /// <param name="outputFileName">输出文件</param>
        void RemoveLines(int[] bandNo, string inputFileName, HDFWriteMode writeMode, string outputFileName, Action<int, string> progressCallback);
        /// <summary>
        /// HDF5数据去条带
        /// </summary>
        /// <param name="mappingDataSet">数据集和波段号的映射关系</param>
        /// <param name="inputFileName">需要操作的文件</param>
        /// <param name="writeMode">操作模式</param>
        /// <param name="outputFileName">输出文件</param>
        void RemoveLines(Dictionary<string, int> mappingDataSet, string inputFileName, HDFWriteMode writeMode, string outputFileName, Action<int, string> progressCallback);
    }
}
