using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.Bricks.AppFramework;


namespace CodeCell.AgileMap.Components
{
    public class CatalogDatabaseConnFolder : CatalogItem
    {
        public CatalogDatabaseConnFolder()
            :base()
        { 
        }

        public CatalogDatabaseConnFolder(string name, object tag)
            :base(name,tag)
        {
        }

        public CatalogDatabaseConnFolder(string name, object tag, string description)
            :base(name,tag,description)
        {
        }

        public CatalogDatabaseConnFolder(string name, object tag, string description, Image image)
            : base(name, tag, description,image)
        {
        }

        protected override void Init()
        {
            _name = "空间数据库连接";
            _description = "所有空间数据库连接的根目录,包括Oracle Spatial,MySql Spatial,MsSql Spatial ...";
            _image = ResourceLoader.GetBitmap("CatalogServerFolder.png");
            //
            _oprItems.Add(new ContextOprItem("新建数据库数据源", ResourceLoader.GetBitmap("CatalogServer.png"), enumContextKeys.AddServerSource));
        }

        internal override void LoadChildren()
        {
            if (!_isLoaded)
            {
                LoadDatabaseConns();
                Refresh();
                _isLoaded = true;
            }
        }

        private void LoadDatabaseConns()
        {
            CatalogDatabaseConn[] items = CatalogPersist.GetCatalogDatabaseConns();
            if (items != null && items.Length > 0)
            {
                foreach (CatalogDatabaseConn it in items)
                    AddChild(it);
            }
        }

        public override void Click(enumContextKeys key)
        {
            switch(key)
            {
                case enumContextKeys.AddServerSource:
                    AddChild(new CatalogDatabaseConn());
                    Refresh();
                    break;
                default:
                    base.Click(key);
                    break;
            }
        }
    }
}
