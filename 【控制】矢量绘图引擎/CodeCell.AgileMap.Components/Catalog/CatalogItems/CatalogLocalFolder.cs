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
    public class CatalogLocalFolder : CatalogItem
    {
        public CatalogLocalFolder()
            :base()
        { 
        }

        public CatalogLocalFolder(string name, object tag)
            :base(name,tag)
        {
        }

        public CatalogLocalFolder(string name, object tag, string description)
            :base(name,tag,description)
        {
        }

        public CatalogLocalFolder(string name, object tag, string description, Image image)
            : base(name, tag, description,image)
        {
        }

        protected override void Init()
        {
            _name = "本地空间数据目录";
            _description = "所有包含空间数据的文件夹或者共享文件夹";
            _image = ResourceLoader.GetBitmap("CatalogLocalFolder.png");
            //
            _oprItems.Add(new ContextOprItem("新建本地数据源", ResourceLoader.GetBitmap("CatalogAddLocal.png"), enumContextKeys.AddLocalSource));
        }

        public override void Click(enumContextKeys key)
        {
            switch (key)
            {
                case enumContextKeys.AddLocalSource:
                    AddLocaDataSource();
                    break;
                default:
                    base.Click(key);
                    break;
            }
        }

        internal override void LoadChildren()
        {
            if (!_isLoaded)
            {
                CatalogLocal[] locs = CatalogPersist.GetCatalogLocalFolders();
                if (locs != null && locs.Length > 0)
                {
                    foreach (CatalogLocal l in locs)
                        AddChild(l);
                    Refresh();
                }
                _isLoaded = true;
            }
        }

        private void AddLocaDataSource()
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    string dir = dlg.SelectedPath;
                    CatalogLocal loc = new CatalogLocal(dir, dir, dir);
                    AddChild(loc);
                    Refresh();
                }
            }
        }
    }
}
