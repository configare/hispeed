using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.Bricks.AppFramework;

namespace CodeCell.AgileMap.Components
{
    public class CatalogRoot:CatalogItem
    {
        public CatalogRoot()
            :base()
        { 
        }

        public CatalogRoot(string name, object tag)
            :base(name,tag)
        {
        }

        public CatalogRoot(string name, object tag, string description)
            :base(name,tag,description)
        {
        }

        public CatalogRoot(string name, object tag, string description, Image image)
            : base(name, tag, description,image)
        {
        }

        protected override void Init()
        {
            _name = "空间数据目录";
            _description = "所有空间数据的根目录";
            //
            AddChild(new CatalogLocalFolder());
            AddChild(new CatalogDatabaseConnFolder());
            AddChild(new CatalogNetServerFolder());
            //
            _image = ResourceLoader.GetBitmap("CatalogRoot.png");
        }
    }
}
