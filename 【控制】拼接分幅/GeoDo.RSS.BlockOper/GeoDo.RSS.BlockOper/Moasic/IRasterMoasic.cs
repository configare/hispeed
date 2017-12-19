using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.BlockOper
{
    public interface IRasterMoasic
    {
        /*DataType==,BandCount==,SpatialRef==,dstEnvelope = MaxEnvelope(srcRasters)*/
        IRasterDataProvider Moasic(IRasterDataProvider[] srcRasters, string driver, string outDir, bool isProcessInvalid, string[] invalidValues, Action<int, string> progressCallback, params object[] options);
        /*DataType==,BandCount==,SpatialRef==*/
        void Moasic(IRasterDataProvider[] srcRasters, IRasterDataProvider dstRaster,bool isProcessInvalid, string[] invalidValues,Action<int, string> progressCallback, params object[] options);
        /*DataType==,SpatialRef==*/
        void Moasic(IRasterBand[] srcBands, IRasterBand dstBand, Action<int, string> progressCallback, params object[] options);       
    }
}
