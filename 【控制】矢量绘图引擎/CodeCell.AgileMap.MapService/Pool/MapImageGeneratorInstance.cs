using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.MapService
{
    public class MapImageGeneratorInstance:IMapImageGeneratorInstance
    {
        private IMapImageGenerator _mapImageGenerator = null;
        private bool _isfree = true;

        public MapImageGeneratorInstance(IMapImageGenerator mapImageGenerator)
        {
            _mapImageGenerator = mapImageGenerator;
        }

        public IMapImageGenerator MapImageGenerator
        {
            get { return _mapImageGenerator; }
        }

        public bool IsFree
        {
            get { return _isfree; }
        }

        public bool Hold()
        {
            _isfree = true;
            return true;
        }

        public void Free()
        {
            _isfree = false;
        }
    }
}