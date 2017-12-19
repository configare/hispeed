using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.MapService
{
    public interface IMapImageGeneratorPool
    {
        int FreeInstanceCount { get; }
        int InstanceCount { get; }
        IMapImageGeneratorInstance GetMapImageGeneratorInstance();
    }
}