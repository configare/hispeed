using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using Telerik.WinControls.UI.Data;
using System.IO;
using GeoDo.RSS.Layout.GDIPlus;
using System.Drawing.Imaging;

namespace GeoDo.RSS.Layout.Elements
{
    public delegate void TemplateClickedHandler(object sender, ILayoutTemplate template);

    public partial class UCTemplateSelectWindow : UserControl, IDisposable
    {
        private RadDropDownList _ddbTempClass = null;
        private RadButton _btnRefresh = null;
        private RadListView _listOverview = null;
        private TemplateClickedHandler _templateClicked = null;
        private string[] _templatesFoldersNames = null;
        private Font _textFont = new Font("微软雅黑", 10, FontStyle.Bold);

        public UCTemplateSelectWindow()
        {
            InitializeComponent();
            Init();
            Disposed += new EventHandler(UCTemplateSelectWindow_Disposed);
        }

        void UCTemplateSelectWindow_Disposed(object sender, EventArgs e)
        {
            if (_textFont != null)
            {
                _textFont.Dispose();
                _textFont = null;
            }
        }

        public TemplateClickedHandler TemplateClicked
        {
            get { return _templateClicked; }
            set { _templateClicked = value; }
        }

        private void Init()
        {
            this.Controls.Clear();

            _ddbTempClass = new RadDropDownList();
            _btnRefresh = new RadButton();
            _listOverview = new RadListView();
            LoadLayoutTemplateName();
            CreatDropControls();
            InitRefreshBtn();
            CreatListView();
            this.Controls.Add(_ddbTempClass);
            this.Controls.Add(_btnRefresh);
            this.Controls.Add(_listOverview);
            _listOverview.BringToFront();
        }

        private void LoadLayoutTemplateName()
        {
            _templatesFoldersNames = new string[] {"自定义","火情","水情","沙尘","大雾","积雪",
                                                   "植被","干旱","地表高温","海冰","蓝藻水华",
                                                   "热带气旋","城市热岛","暴雨及中尺度对流","通用专题图"};
            string path = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate";
            List<string> allname = new List<string>();
            allname.AddRange(_templatesFoldersNames);
            if (Directory.Exists(path))
            {
                int rootLength = path.Length;
                string[] names = Directory.GetDirectories(path);
                foreach (string dir in names)
                {
                    string dirName = dir.Remove(0, rootLength + 1);
                    if (!allname.Contains(dirName))
                    {
                        allname.Add(dirName);
                    }
                }
            }
            _templatesFoldersNames = allname.ToArray();
        }

        private void InitRefreshBtn()
        {
            _btnRefresh.Click += new EventHandler(_btnRefresh_Click);
            _btnRefresh.Dock = DockStyle.Bottom;
            _btnRefresh.Image = ImageGetter.GetImageByName("");
            _btnRefresh.Text = "刷新";
            _btnRefresh.TextAlignment = ContentAlignment.MiddleCenter;
            _btnRefresh.ImageAlignment = ContentAlignment.MiddleCenter;
        }

        void _btnRefresh_Click(object sender, EventArgs e)
        {
            _listOverview.Items.Clear();
            //LoadClassFromArray();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate";
            path = Path.Combine(path, _ddbTempClass.SelectedItem.Text.Trim());
            if (Directory.Exists(path))
                ParseTemplate(path);
        }

        private void CreatDropControls()
        {
            bool isOk = LoadClassFromArray();
            if (!isOk)
                return;
            _ddbTempClass.DropDownStyle = RadDropDownStyle.DropDownList;
            _ddbTempClass.DropDownHeight = 150;
            _ddbTempClass.AutoSizeItems = true;
            _ddbTempClass.Dock = DockStyle.Top;
            _ddbTempClass.Font = new Font("微软雅黑", 10);
            _ddbTempClass.Location = new Point(20, 20);
            _ddbTempClass.SelectedIndex = 0;
            string path = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate";
            if (_ddbTempClass.SelectedItem != null)
            {
                path = Path.Combine(path, _ddbTempClass.SelectedItem.Text.Trim());
                if (Directory.Exists(path))
                    ParseTemplate(path);
                _ddbTempClass.SelectedIndexChanged += new PositionChangedEventHandler(_ddbTempClass_SelectedIndexChanged);
            }
        }

        private bool LoadClassFromArray()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate";
            //if (!Directory.Exists(path))
            //    return false;
            if (_templatesFoldersNames == null || _templatesFoldersNames.Length == 0)
                return false;
            string folderPath;
            RadListDataItem item;
            foreach (string name in _templatesFoldersNames)
            {
                folderPath = Path.Combine(path, name);
                if (!Directory.Exists(folderPath))
                    continue;
                item = new RadListDataItem(name);
                item.Font = new Font("微软雅黑", 10);
                _ddbTempClass.Items.Add(item);
            }
            return true;
        }

        private void CreatListView()
        {
            _listOverview.AllowEdit = false;
            _listOverview.AllowDrop = false;
            _listOverview.AllowArbitraryItemHeight = true;//使用平均高度
            _listOverview.EnableColumnSort = false;
            _listOverview.Location = new Point(5, 30);
            _listOverview.Dock = DockStyle.Fill;
            _listOverview.ViewType = ListViewType.ListView;
            _listOverview.AutoSize = true;
            _listOverview.ShowCheckBoxes = false;
            _listOverview.ShowItemToolTips = true;
            _listOverview.ShowGroups = false;
            _listOverview.FullRowSelect = true;
            _listOverview.ItemSpacing = 8;
            _listOverview.ItemMouseClick += new ListViewItemEventHandler(SelectedListViewItemsChanged);
        }

        void _ddbTempClass_SelectedIndexChanged(object sender, PositionChangedEventArgs e)
        {
            if (_ddbTempClass.SelectedIndex == -1)
                return;
            _listOverview.Items.Clear();
            string path = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate";
            if (!Directory.Exists(path))
                return;
            path = Path.Combine(path, _ddbTempClass.SelectedItem.Text.Trim());
            if (!Directory.Exists(path))
                return;
            ParseTemplate(path);
        }

        private void ParseTemplate(string path)
        {
            string[] fnames = Directory.GetFiles(path, "*.gxt");
            if (fnames == null || fnames.Length == 0)
                return;
            foreach (string fname in fnames)
            {
                ListViewDataItem item = new ListViewDataItem();
                item.Tag = fname;
                item.Image = GetOverviewImage(fname);
                item.Text = Path.GetFileNameWithoutExtension(fname);
                item.ImageAlignment = ContentAlignment.MiddleCenter;
                item.TextAlignment = ContentAlignment.TopLeft;
                item.TextImageRelation = TextImageRelation.TextAboveImage;
                item.Font = _textFont;
                item.ForeColor = Color.FromArgb(102, 112, 131);
                _listOverview.Items.Add(item);
            }
        }

        private Image GetOverviewImage(string fname)
        {
            string ovFileName = fname.ToLower().Replace(".gxt", ".png");
            if (File.Exists(ovFileName))
            {
                using (MemoryStream ms = new MemoryStream(File.ReadAllBytes(ovFileName)))
                {
                    return new Bitmap(ms);
                }
            }
            else
            {
                try
                {
                    using (ILayoutTemplate template = LayoutTemplate.LoadTemplateFrom(fname))
                    {
                        if (template != null)
                        {
                            Bitmap bitmap = template.GetOverview(new Size(165, 165));
                            if (bitmap != null)
                                bitmap.Save(ovFileName, ImageFormat.Png);
                            return bitmap;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            return null;
        }

        void SelectedListViewItemsChanged(object sender, EventArgs e)
        {
            if (_listOverview.SelectedIndex == -1)
                return;
            if (_templateClicked != null)
                _templateClicked(sender, LayoutTemplate.LoadTemplateFrom(_listOverview.SelectedItem.Tag.ToString()));
        }
    }
}
