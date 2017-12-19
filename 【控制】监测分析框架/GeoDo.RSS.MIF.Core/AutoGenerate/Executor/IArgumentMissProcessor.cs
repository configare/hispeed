using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public interface IArgumentMissProcessor
    {
        string DoGettingArgument(SubProductDef subProduct, AlgorithmDef algorithm, string argument);
    }
}
