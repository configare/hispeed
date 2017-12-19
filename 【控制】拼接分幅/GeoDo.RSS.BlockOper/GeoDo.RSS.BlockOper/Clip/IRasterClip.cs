using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.BlockOper
{
    public interface IRasterClip
    {
        /*samplePercent=[1~100]*/
        IRasterDataProvider[] Clip(IRasterDataProvider srcRaster, BlockDef[] blockDefs, int samplePercent, string driver, string outdir, Action<int, string> progressCallback, params object[] options);
        IRasterBand[] Clip(IRasterBand srcBand, BlockDef[] blockDefs, int samplePercent, string driver, Action<int, string> progressCallback, params object[] options);
    }
}
