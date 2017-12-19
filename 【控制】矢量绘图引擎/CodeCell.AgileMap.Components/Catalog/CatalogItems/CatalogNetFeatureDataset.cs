using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.Components
{
    public class CatalogNetFeatureDataset : CatalogItem
    {
        public CatalogNetFeatureDataset()
            : base()
        {
        }

        public CatalogNetFeatureDataset(string name, object tag)
            : base(name, tag)
        {
        }

        public CatalogNetFeatureDataset(string name, object tag, string description)
            : base(name, tag, description)
        {
        }

        public CatalogNetFeatureDataset(string name, object tag, string description, Image image)
            : base(name, tag, description, image)
        {
        }

        protected override void Init()
        {
            _image = ResourceLoader.GetBitmap("CatalogDataset.png");
            _oprItems.Add(new ContextOprItem("属性", ResourceLoader.GetBitmap("cmdProperty.png"), enumContextKeys.Property));
        }

        public override void Click(enumContextKeys key)
        {
            switch (key)
            {
                case enumContextKeys.Property:
                    break;
                default:
                    base.Click(key);
                    break;
            }
        }
    }
}
