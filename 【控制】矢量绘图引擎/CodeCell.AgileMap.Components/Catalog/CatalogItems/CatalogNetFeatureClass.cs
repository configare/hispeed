using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using System.Data;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.Components
{
    public class CatalogNetFeatureClass : CatalogItem
    {
        public CatalogNetFeatureClass()
            : base()
        {
        }

        public CatalogNetFeatureClass(string name, object tag)
            : base(name, tag)
        {
        }

        public CatalogNetFeatureClass(string name, object tag, string description)
            : base(name, tag, description)
        {
        }

        public CatalogNetFeatureClass(string name, object tag, string description, Image image)
            : base(name, tag, description, image)
        {
        }

        protected override void Init()
        {
            _image = ResourceLoader.GetBitmap("CatalogFeatureClass.png");
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
