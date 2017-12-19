using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.MapService
{
    public interface IMapImageGeneratorInstance
    {
        bool IsFree { get; }
        bool Hold();
        void Free();
        IMapImageGenerator MapImageGenerator { get; }
    }
}