using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;

namespace GeoDo.RSS.MIF.Core
{
    public interface IArgumentProvider : IDisposable
    {
        bool BandArgsIsSetted { get; }
        AlgorithmDef CurrentAlgorithmDef { get; set; }
        IRasterDataProvider DataProvider { get; set; }
        int[] AOI { get; set; }
        Feature[] AOIs { get; set; }
        string[] ArgNames { get; }
        ArgumentDef GetArgDef(string argName);
        object GetArg(string argName);
        void SetArg(string argName, object argValue);
        void Reset();
        ICurrentRasterInteractiver CurrentRasterInteractiver { get; }
        IEnvironmentVarProvider EnvironmentVarProvider { get; }
    }
}
