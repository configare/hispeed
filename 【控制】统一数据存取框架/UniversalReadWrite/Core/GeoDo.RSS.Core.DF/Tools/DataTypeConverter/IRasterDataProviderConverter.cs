using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.Core.DF
{
    public interface IRasterDataProviderConverter
    {
        IRasterDataProvider ConvertDataType<TSrc,TDst>(string srcFileName, enumDataType dstDataType, string dstFileName,Func<TSrc,TDst> converter);
        IRasterDataProvider ConvertDataType<TSrc, TDst>(IRasterDataProvider srcDataProvider, enumDataType dstDataType, string dstFileName, Func<TSrc, TDst> converter);
    }
}
