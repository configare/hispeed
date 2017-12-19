using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.AppFramework;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Catalog
{
    public class ApplicationCatalog:ApplicationDefault
    {
        protected IMapControl _mapControl = null;

        public ApplicationCatalog(IMapControl mapControl, string tempDir)
            :base(tempDir)
        {
            _mapControl = mapControl;
        }

        public IMapControl MapControl
        {
            get { return _mapControl; }
        }
    }
}
