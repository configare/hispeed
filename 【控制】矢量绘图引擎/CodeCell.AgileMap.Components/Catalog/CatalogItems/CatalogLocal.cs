using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using System.IO;
using CodeCell.Bricks.UIs;
using CodeCell.Bricks.AppFramework;


namespace CodeCell.AgileMap.Components
{
    public class CatalogLocal : CatalogItem
    {
        private string[] _supportedFileExts = new string[] { "*.shp", "*.rst" };
        private string _mapFileExts = "*.mcd";

        public CatalogLocal()
            : base()
        {
        }

        public CatalogLocal(string name, object tag)
            : base(name, tag)
        {
        }

        public CatalogLocal(string name, object tag, string description)
            : base(name, tag, description)
        {
        }

        public CatalogLocal(string name, object tag, string description, Image image)
            : base(name, tag, description, image)
        {
        }

        protected override void Init()
        {
            _image = ResourceLoader.GetBitmap("CatalogLocalFolder.png");
            _oprItems.Add(new ContextOprItem("刷新", ResourceLoader.GetBitmap("cmdRefresh.gif"), enumContextKeys.Refresh));
        }

        internal override void LoadChildren()
        {
            if (!_isLoaded)
            {
                string[] dirs = null;
                try
                {
                    dirs = Directory.GetDirectories(_tag.ToString());
                }
                catch (Exception ex)
                {
                    MsgBox.ShowError(ex.Message);
                    return;
                }
                //load dirs
                if (dirs != null && dirs.Length > 0)
                {
                    foreach (string d in dirs)
                    {
                        string[] dparts = d.Split(Path.DirectorySeparatorChar);
                        CatalogLocal loc = new CatalogLocal(dparts[dparts.Length - 1], d, d);
                        AddChild(loc);
                    }
                }
                //load files
                foreach (string ext in _supportedFileExts)
                {
                    string[] files = Directory.GetFiles(_tag.ToString(), ext);
                    if (files != null && files.Length > 0)
                    {
                        foreach (string f in files)
                        {
                            CatalogFile cf = null;
                            if(ext == "*.shp")
                                cf = new CatalogFile(Path.GetFileNameWithoutExtension(f), f, f);
                            else if (ext == "*.rst")
                                cf = new CatalogRasterFile(Path.GetFileNameWithoutExtension(f), f, f);
                            AddChild(cf);
                        }
                    }
                }
                //
                Refresh();
                _isLoaded = true;
            }
        }
    }
}
