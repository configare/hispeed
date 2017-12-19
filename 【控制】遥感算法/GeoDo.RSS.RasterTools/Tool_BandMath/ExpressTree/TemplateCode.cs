using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.RasterTools
{
    class TemplateCode
    {
        public const string FuncTemplate = @"
using System;
using System.Collections.Generic;

namespace GeoDo.RSS.RasterTools
{
    public class OperatorBuilder : IPixelValuesOperator<%DataType%>
    {
%Func%		
    }
}";

        public const string BandMathExecutorTemplate = @"
using System;
using System.Collections.Generic;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.RasterTools
{
    public class BandMathExecutor:IBandMathExecutor
    {
        public void Compute(IRasterDataProvider dataProvider, string expression, IRasterBand dstRasterBand, Action<int, string> progressTracker)
        {
            int[] bandNos = null;
            IPixelValuesOperator<%SrcDataType%> prd = ClassRuntimeGenerator.GeneratePixelValuesOperator<%SrcDataType%>(expression, out bandNos);
            if (bandNos == null || bandNos.Length == 0)
                return;
            int maxBandNo = int.MinValue;
            int minBandNo = int.MaxValue;
            for (int i = 0; i < bandNos.Length; i++)
            {
                if (bandNos[i] < minBandNo)
                    minBandNo = bandNos[i];
                if (bandNos[i] > maxBandNo)
                    maxBandNo = bandNos[i];
            }
            if(minBandNo<1)
                throw new ArgumentOutOfRangeException();
            if(maxBandNo > dataProvider.BandCount)
                throw new ArgumentOutOfRangeException();
            IRasterBand[] srcBands = new IRasterBand[bandNos.Length];
            for (int i = 0; i < bandNos.Length; i++)
                srcBands[i] = dataProvider.GetRasterBand(bandNos[i]);
            Func<%SrcDataType%[], %DstDataType%> opr = prd.GetOperatorFunc();
            IBandMathVisitor visitor = new BandMathVisitor();
            visitor.Visit<%SrcDataType%, %DstDataType%>(srcBands, dstRasterBand, prd.GetOperatorFunc(), progressTracker);
        }
    }
}
";
    }
}
