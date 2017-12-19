using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.Bricks.AppFramework;


namespace CodeCell.AgileMap.Components
{
    public class CatalogRasterFile:CatalogFile
    {
        public CatalogRasterFile()
            :base()
        { 
        }

        public CatalogRasterFile(string name, object tag)
            :base(name,tag)
        {
        }

        public CatalogRasterFile(string name, object tag, string description)
            :base(name,tag,description)
        {
        }

        public CatalogRasterFile(string name, object tag, string description, Image image)
            : base(name, tag, description,image)
        {
        }

        protected override void Init()
        {
            _image = ResourceLoader.GetBitmap("cmdRaster.png");
        }

    }
}
