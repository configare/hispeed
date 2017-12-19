using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using CodeCell.Bricks.AppFramework;


namespace CodeCell.AgileMap.Components
{
    public class CatalogNetServerFolder : CatalogItem
    {
        public CatalogNetServerFolder()
            :base()
        { 
        }

        public CatalogNetServerFolder(string name, object tag)
            :base(name,tag)
        {
        }

        public CatalogNetServerFolder(string name, object tag, string description)
            :base(name,tag,description)
        {
        }

        public CatalogNetServerFolder(string name, object tag, string description, Image image)
            : base(name, tag, description,image)
        {
        }

        protected override void Init()
        {
            _name = "网络空间数据服务";
            _description = "网络空间数据服务";
            _image = ResourceLoader.GetBitmap("CatalogNetServerFolder.gif");
            //
            _oprItems.Add(new ContextOprItem("新建网络数据服务源", ResourceLoader.GetBitmap("CatalogNetServer.png"), enumContextKeys.AddServerSource));
        }

        internal override void LoadChildren()
        {
            if (!_isLoaded)
            {
                LoadNetServers();
                Refresh();
                _isLoaded = true;
            }
        }

        private void LoadNetServers()
        {
            CatalogNetServer[] servers = CatalogPersist.GetCatalogNetServers();
            if (servers == null || servers.Length == 0)
                return;
            foreach (CatalogNetServer srv in servers)
            {
                AddChild(srv);
            }
        }
    }
}
