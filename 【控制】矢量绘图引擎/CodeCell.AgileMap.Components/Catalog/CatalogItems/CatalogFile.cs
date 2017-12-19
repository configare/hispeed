using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.Bricks.AppFramework;


namespace CodeCell.AgileMap.Components
{
    public class CatalogFile:CatalogItem
    {
        public CatalogFile()
            :base()
        { 
        }

        public CatalogFile(string name, object tag)
            :base(name,tag)
        {
        }

        public CatalogFile(string name, object tag, string description)
            :base(name,tag,description)
        {
        }

        public CatalogFile(string name, object tag, string description, Image image)
            : base(name, tag, description,image)
        {
        }

        protected override void Init()
        {
            _image = ResourceLoader.GetBitmap("CatalogFeatureClass.png");
        }

    }
}
