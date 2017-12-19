using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IMonitoringSubProduct:IDisposable
    {
        string Name { get; }
        string Identify { get; }
        bool IsBinary { get; }
        SubProductDef Definition { get; }
        List<AlgorithmDef> AlgorithmDefs { get; }
        IArgumentProvider ArgumentProvider { get; }
        void ResetArgumentProvider(string algIdentify);
        void ResetArgumentProvider(string satellite, string sensor, params string[] args);
        void ResetArgumentProvider(string algIdentify, string satellite, string sensor, params string[] args);
        IExtractResult Make(Action<int,string> progressTracker);
        IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage);
        /// <summary>
        /// 后续产品制作
        /// </summary>
        /// <param name="piexd">更新后的判识结果</param>
        /// <param name="progressTracker"></param>
        /// <returns></returns>
        IExtractResult MakeExtProduct(IPixelIndexMapper piexd, Action<int, string> progressTracker);
        bool ArgumentsIsOK();
        void SetExtHeader(IExtHeaderSetter setter, object header);
        AlgorithmDef UseDefaultAlgorithm(string productIdentify);
        bool CanDo { get; }
        void Reset();
    }
}
