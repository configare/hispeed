using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.UI;
using Telerik.WinControls.UI;
using System.IO;
using System.Drawing;

namespace GeoDo.RSS.UI.WinForm
{
    internal class UCRecentFileList : IUIProvider, IRecentFileContainer
    {
        private ISmartSession _session;
        private RadListControl _fileRadListView;
        private RadListControl _dirRadListView;
        private RadRibbonBarBackstageView _view;
        private Dictionary<string, Image> _images = new Dictionary<string, Image>();

        public UCRecentFileList()
        {
            CreateFileListView();
            CreateDirListView();
        }

        private void CreateDirListView()
        {
            _dirRadListView = new RadListControl();
            _dirRadListView.Font = new Font("微软雅黑", 10.5f);
            _dirRadListView.ItemHeight = 60;
            _dirRadListView.BackColor = Color.FromArgb(250, 250, 250);
            _dirRadListView.Padding = new System.Windows.Forms.Padding(24, 38, 24, 6);
            _dirRadListView.Dock = System.Windows.Forms.DockStyle.Fill;
            _dirRadListView.VisualItemFormatting += new VisualListItemFormattingEventHandler(_dirRadListView_VisualItemFormatting);
            _dirRadListView.CreatingVisualListItem += new CreatingVisualListItemEventHandler(_dirRadListView_CreatingVisualListItem);
            _dirRadListView.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(_dirRadListView_SelectedIndexChanged);
        }

        private void CreateFileListView()
        {
            _fileRadListView = new RadListControl();
            _fileRadListView.Font = new Font("微软雅黑", 10.5f);
            _fileRadListView.ItemHeight = 60;
            _fileRadListView.BackColor = Color.FromArgb(250, 250, 250);
            _fileRadListView.Padding = new System.Windows.Forms.Padding(24, 38, 24, 6);
            _fileRadListView.Dock = System.Windows.Forms.DockStyle.Fill;
            _fileRadListView.VisualItemFormatting += new VisualListItemFormattingEventHandler(_radListView_VisualItemFormatting);
            _fileRadListView.CreatingVisualListItem += new CreatingVisualListItemEventHandler(_radListView_CreatingVisualListItem);
            _fileRadListView.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(_radListView_SelectedIndexChanged);
        }

        void _dirRadListView_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (_dirRadListView.SelectedItem == null)
                return;
            _view.HidePopup();
            object dir = _dirRadListView.SelectedItem.Tag.ToString();
            System.Diagnostics.Process.Start("explorer.exe",dir.ToString());
        }

        void _radListView_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
        {
            if (_fileRadListView.SelectedItem == null)
                return;
            _view.HidePopup();
            object v = _fileRadListView.SelectedItem.Tag;
            if (File.Exists(v.ToString()))
                OpenFileFactory.Open(v.ToString());
            else
            {
                MsgBox.ShowInfo("文件\"" + v.ToString() + "\"不存在。");
                _session.RecentFilesManager.Remove(v.ToString());
                LoadItemsByRecentUsedFiles();
            }
        }

        void _dirRadListView_CreatingVisualListItem(object sender, CreatingVisualListItemEventArgs args)
        {
            args.VisualItem = new PinnedListVisualItem();
        }

        void _radListView_CreatingVisualListItem(object sender, CreatingVisualListItemEventArgs args)
        {
            args.VisualItem = new PinnedListVisualItem();
        }

        void _dirRadListView_VisualItemFormatting(object sender, VisualItemFormattingEventArgs args)
        {
            string dirFlag = "DIR";
            if (_images.ContainsKey(dirFlag))
                args.VisualItem.Image = _images[dirFlag];
            else
            {
                Image img = GetImageByExt(dirFlag);
                if (img != null)
                {
                    _images.Add(dirFlag, img);
                    args.VisualItem.Image = _images[dirFlag];
                }
            }
        }

        void _radListView_VisualItemFormatting(object sender, VisualItemFormattingEventArgs args)
        {
            string file = args.VisualItem.Data.ToString();
            string[] name = file.Split('\n');
            if (name.Length == 1)
                return;
            string extName = "." + name[0].Split('.')[1].ToUpper();
            if (_images.ContainsKey(extName))
                args.VisualItem.Image = _images[extName];
            else
            {
                Image img = GetImageByExt(extName);
                if (img != null)
                {
                    _images.Add(extName, img);
                    args.VisualItem.Image = _images[extName];
                }
            }
        }

        private Image GetImageByExt(string extName)
        {
            switch (extName)
            {
                case ".HDF":
                    return _session.UIFrameworkHelper.GetImage("system:FileType_HDF.png");
                case ".LDF":
                    return _session.UIFrameworkHelper.GetImage("system:FileType_LDF.png");
                case ".BMP":
                case ".JPG":
                case ".JPEG":
                case ".IMG":
                case ".TIF":
                case ".TIFF":
                case ".PNG":
                    return _session.UIFrameworkHelper.GetImage("system:FileType_Image.png");
                case ".XLS":
                case ".XLSX":
                    return _session.UIFrameworkHelper.GetImage("system:FileType_Excel.png");
                case ".DOC":
                case ".DOCX":
                    return _session.UIFrameworkHelper.GetImage("system:FileType_Word.png");
                case ".GXD":
                    return _session.UIFrameworkHelper.GetImage("system:FileType_Map.png");
                case ".GXT":
                    return _session.UIFrameworkHelper.GetImage("system:FileType_Template.png");
                case ".SHP":
                    return _session.UIFrameworkHelper.GetImage("system:FileType_Vector.png");
                case "DIR":
                    return _session.UIFrameworkHelper.GetImage("system:FileType_Dir.png");
                default:
                    return _session.UIFrameworkHelper.GetImage("system:FileType_Normal.png");
            }
        }

        public object Content
        {
            get
            {
                RadSplitContainer panel = new RadSplitContainer();
                panel.Dock = System.Windows.Forms.DockStyle.Fill;
                //
                RadSplitContainer leftPanelcontainer = new RadSplitContainer();
                leftPanelcontainer.Dock = System.Windows.Forms.DockStyle.Left;
                SplitPanel leftPanel = new SplitPanel();
                RadLabel lb = new RadLabel();
                lb.Text = "最近访问过的文件";
                lb.BackColor = Color.White;
                lb.ForeColor = Color.Gray;
                lb.Font = new Font("微软雅黑",18);
                lb.Left = 20;
                lb.Top = 2;
                leftPanel.Controls.Add(lb);
                leftPanelcontainer.SplitPanels.Add(leftPanel);
                leftPanel.Controls.Add(_fileRadListView);
                //
                RadSplitContainer rightPanelContainer = new RadSplitContainer();
                SplitPanel rightPanel = new SplitPanel();
                lb = new RadLabel();
                lb.Text = "最近访问过的路径";
                lb.BackColor = Color.White;
                lb.ForeColor = Color.Gray;
                lb.Font = new Font("微软雅黑", 18);
                lb.Left = 20;
                lb.Top = 2;
                rightPanel.Controls.Add(lb);
                rightPanelContainer.SplitPanels.Add(rightPanel);
                rightPanelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
                rightPanel.Controls.Add(_dirRadListView);
                //
                panel.SplitPanels.Add(leftPanelcontainer);
                panel.SplitPanels.Add(rightPanelContainer);
                return panel;
            }
        }

        public void LoadItemsByRecentUsedFiles()
        {
            _fileRadListView.Items.Clear();
            List<string> dirs = new List<string>();
            string[] fnames = _session.RecentFilesManager.GetRecentUsedFiles();
            if (fnames != null && fnames.Length > 0)
            {
                foreach (string f in fnames)
                {
                    string dir = Path.GetDirectoryName(f);
                    if (!dirs.Contains(dir))
                        dirs.Add(dir);
                    RadListDataItem it = new RadListDataItem(Path.GetFileName(f) + "\n" + dir);
                    it.Tag = f;
                    _fileRadListView.Items.Add(it);
                }
            }
            //
            foreach (string dir in dirs)
            {
                string[] parts = dir.Split('\\');
                RadListDataItem it = new RadListDataItem(parts[parts.Length-1] + "\n" + dir);
                it.Tag = dir;
                _dirRadListView.Items.Add(it);
            }
        }

        public void Init(ISmartSession session, params object[] arguments)
        {
            _session = session;
            LoadItemsByRecentUsedFiles();
        }

        public void UpdateStatus()
        {
        }


        public void SetFileMenView(RadRibbonBarBackstageView view)
        {
            _view = view;
        }
    }
}
