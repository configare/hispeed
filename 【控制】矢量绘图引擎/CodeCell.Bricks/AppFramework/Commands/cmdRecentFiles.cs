using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace CodeCell.Bricks.AppFramework
{
    public class cmdRecentFiles : BaseCommand
    {
        private bool _isAttached = false;
        private ToolStripMenuItem _menuItem = null;

        public cmdRecentFiles()
        {
            Init();
        }

        public cmdRecentFiles(bool beginGroup)
            : base(beginGroup)
        {
            Init();
        }

        private void Init()
        {
            _name = "RecentFiles";
            _text = "最近打开过的文件";
            _tooltips = "最近打开过的文件";
            //_image = ResourceLoader.GetBitmap("cmdOpen.png");
        }

        public override bool Enabled
        {
            get
            {
                if (!_isAttached)
                {
                    _menuItem = _hook.CommandHelper.GetControlByCommandType(this.GetType()) as ToolStripMenuItem;
                    _hook.Application.RecentUsedFilesMgr.OnAddOneFile += new CodeCell.Bricks.Runtime.RecentUsedFiles.OnAddOneFileHandler(RecentUsedFilesMgr_OnAddOneFile);
                    _hook.Application.RecentUsedFilesMgr.OnRemoveOneFile += new CodeCell.Bricks.Runtime.RecentUsedFiles.OnRemoveOneFileHandler(RecentUsedFilesMgr_OnRemoveOneFile);
                    _isAttached = true;
                }
                return true;
            }
        }

        void RecentUsedFilesMgr_OnRemoveOneFile(object sender, string filename)
        {
            if (_menuItem.DropDownItems != null && _menuItem.DropDownItems.Count > 0)
            {
                for (int i = _menuItem.DropDownItems.Count - 1; i >= 0; i--)
                {
                    if (_menuItem.DropDownItems[i].Tag.ToString().ToUpper() == filename.ToUpper())
                    {
                        _menuItem.DropDownItems.Remove(_menuItem.DropDownItems[i]);
                        break;
                    }
                }
            }
            //ReNameAllMenuItems();
        }

        private void ReNameAllMenuItems()
        {
            if (_menuItem.DropDownItems != null && _menuItem.DropDownItems.Count > 0)
            {
                for (int i = 1; i <= _menuItem.DropDownItems.Count; i++)
                {
                    _menuItem.DropDownItems[i-1].Text = i.ToString()+" "+ _menuItem.DropDownItems[i-1].Tag.ToString();
                }
            }
        }

        void RecentUsedFilesMgr_OnAddOneFile(object sender, string filename)
        {
            ToolStripMenuItem it = new ToolStripMenuItem(string.Empty);
            it.Tag = filename;
            it.Click += new EventHandler(it_Click);
            _menuItem.DropDownItems.Insert(0,it);
            ReNameAllMenuItems();
        }

        void it_Click(object sender, EventArgs e)
        {
            string file = (sender as ToolStripMenuItem).Tag.ToString();
            //OpenFileFactory.OpenFile(file, false);
        }
    }
}
