using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using System.IO;
using System.Net;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.AppFramework;
using CodeCell.Bricks.UIs;


namespace CodeCell.AgileMap.Components
{
    public class CatalogNetServer : CatalogItem
    {
        public CatalogNetServer()
            :base()
        { 
        }

        public CatalogNetServer(string name, object tag)
            :base(name,tag)
        {
        }

        public CatalogNetServer(string name, object tag, string description)
            :base(name,tag,description)
        {
        }

        public CatalogNetServer(string name, object tag, string description, Image image)
            : base(name, tag, description,image)
        {
        }

        protected override void Init()
        {
            _image = ResourceLoader.GetBitmap("CatalogNetServer.png");
        }

        internal override void LoadChildren()
        {
            if (!_isLoaded)
            {
                ReadAndLoadInstances();
                Refresh();
                _isLoaded = true;
            }
        }

        private void ReadAndLoadInstances()
        {
            CatalogNetInstance[] ists = GetInstances();
            if (ists == null || ists.Length == 0)
                return;
            foreach (CatalogNetInstance ist in ists)
                AddChild(ist);
        }

        private CatalogNetInstance[] GetInstances()
        {
            try
            {
                string url = _tag.ToString();
                if (!url.EndsWith("/"))
                    url += "/";
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add(HttpCommands.cstArgNameCommand, HttpCommands.cstArgCmdGetInstanceList);
                url = url + HttpCommands.GetCatalogUrl(HttpCommands.cstCatalogPage, args);
                object obj = GetObjectFromHttpStream.GetObject(url);
                InstanceIdentify[] ids = (InstanceIdentify[])obj;
                if (ids != null && ids.Length > 0)
                {
                    CatalogNetInstance[] instances = new CatalogNetInstance[ids.Length];
                    int i = 0;
                    foreach (InstanceIdentify id in ids)
                    {
                        instances[i] = new CatalogNetInstance(id.Name, id, id.Description);
                        i++;
                    }
                    return instances;
                }
                return null;
            }
            catch (Exception ex)
            {
                MsgBox.ShowError(ex.Message);
                return null;
            }
        }
    }
}
