using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.MVG
{
    public interface IMvgDataProvider : IRasterDataProvider
    {
        MvgHeader Header { get; }

        /// <summary>
        /// mvg文件另存为ldf文件
        /// </summary>
        /// <param name="fileName">ldf文件名</param>
        void ToLdfFile(string ldfFileName);

        /// <summary>
        /// mvg文件另存为ldf文件，保存在mvg文件的当前目录下
        /// </summary>
        void ToLdfFile();
    }
}
