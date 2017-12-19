using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.FileProject;
using System.Drawing;
using System.Windows.Forms;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RasterProject;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class ThemeGraphRegionFacctory
    {
        public static void RegistToButton(string productIdentify, RadDropDownButtonElement btnRegion, RibbonTab tab, ISmartSession session)
        {
            ThemeGraphRegionFacctory rad = new ThemeGraphRegionFacctory(btnRegion, tab, productIdentify, session);
            rad.AddThemeGraphRegionSettingButton();
        }

        private RadDropDownButtonElement _btnSetThemeGraphRegion;
        private RibbonTab _tab;
        private string _productIdentify;
        private ISmartSession _session;

        private ThemeGraphRegionFacctory(RadDropDownButtonElement btn, RibbonTab tab, string productIdentify, ISmartSession session)
        {
            _btnSetThemeGraphRegion = btn;
            _tab = tab;
            _productIdentify = productIdentify;
            _session = session;
        }

        private void AddThemeGraphRegionSettingButton()
        {
            _btnSetThemeGraphRegion.MinSize = new Size(80, 20);
            _btnSetThemeGraphRegion.MaxSize = new Size(120, 100);
            _btnSetThemeGraphRegion.Text = "无范围应用";
            _btnSetThemeGraphRegion.TextAlignment = ContentAlignment.BottomCenter;
            _btnSetThemeGraphRegion.ImageAlignment = ContentAlignment.TopCenter;
            _btnSetThemeGraphRegion.Click += new EventHandler(_btnSetThemeGraphRegion_Click);
            InitThemeGraphRegion();
            UpdateBtn();
            RadRibbonBarGroup gpSetThemeGraphRegion = new RadRibbonBarGroup();
            gpSetThemeGraphRegion.Text = "专题图输出范围";
            gpSetThemeGraphRegion.Items.Add(_btnSetThemeGraphRegion);
            _tab.Items.Add(gpSetThemeGraphRegion);
        }

        private void InitThemeGraphRegion()
        {   //初始化当前区域设置为“无范围应用”
            ThemeGraphRegion newRegion = ThemeGraphRegionSetting.GetThemeGraphRegion(_productIdentify);
            newRegion.Enable = false;
            ThemeGraphRegionSetting.SaveThemeGraphRegion(newRegion);
        }

        //专题图输出范围设置
        void _btnSetThemeGraphRegion_Click(object sender, EventArgs e)
        {
            LoadThemeGraphRegion();
            _btnSetThemeGraphRegion.ShowDropDown();
        }

        private void LoadThemeGraphRegion()
        {
            _btnSetThemeGraphRegion.Items.Clear();
            ThemeGraphRegion region = ThemeGraphRegionSetting.GetThemeGraphRegion(_productIdentify);
            _btnSetThemeGraphRegion.Tag = region;
            if (region != null)
            {
                bool enable = region.Enable;
                int selectedIndex = region.SelectedIndex;
                PrjEnvelopeItem[] items = region.PrjEnvelopeItems;
                if (items != null)
                {
                    for (int i = 0; i < items.Length; i++)
                    {
                        PrjEnvelopeItem item = items[i];
                        RadMenuItem menu = new RadMenuItem();
                        menu.Text = item.Name;
                        menu.Tag = i;
                        menu.Click += new EventHandler(menu_Click);
                        if (enable && selectedIndex == i)
                        {
                            _btnSetThemeGraphRegion.Text = item.Name;
                            menu.IsChecked = true;
                        }
                        _btnSetThemeGraphRegion.Items.Add(menu);
                    }
                    _btnSetThemeGraphRegion.Items.Add(new RadMenuSeparatorItem());
                    RadMenuItem cancelRegion = new RadMenuItem("取消范围应用");
                    cancelRegion.Tag = region;
                    cancelRegion.Click += new EventHandler(cancelRegion_Click);
                    _btnSetThemeGraphRegion.Items.Add(cancelRegion);
                }
            }
            _btnSetThemeGraphRegion.Items.Add(new RadMenuSeparatorItem());
            RadMenuItem editRegion = new RadMenuItem("编辑范围");
            editRegion.Click += new EventHandler(editRegion_Click);
            editRegion.Tag = region;
            _btnSetThemeGraphRegion.Items.Add(editRegion);
        }

        void regionFromAoi_Click(object sender, EventArgs e)
        {
            if (_session.SmartWindowManager.ActiveCanvasViewer == null)
                return;
            AOIItem[] aoiItems = _session.SmartWindowManager.ActiveCanvasViewer.AOIProvider.GetAOIItems();
            if (aoiItems == null || aoiItems.Length == 0)
                return;
            CoordEnvelope env = aoiItems[0].GeoEnvelope;
            ThemeGraphRegion region = (sender as RadMenuItem).Tag as ThemeGraphRegion;

            region.Add(new PrjEnvelopeItem("AOI",new PrjEnvelope(env.MinX,env.MaxX,env.MinY,env.MaxY)));
            ThemeGraphRegionSetting.SaveThemeGraphRegion(region);
            UpdateBtn();
        }

        void cancelRegion_Click(object sender, EventArgs e)
        {
            _btnSetThemeGraphRegion.Text = "无范围应用";
            ThemeGraphRegion region = (sender as RadMenuItem).Tag as ThemeGraphRegion;
            region.Enable = false;
            ThemeGraphRegionSetting.SaveThemeGraphRegion(region);
        }

        void editRegion_Click(object sender, EventArgs e)
        {
            frmThemeGraphRegionManager frm = new frmThemeGraphRegionManager();
            {
                frm.Init(_session);
                frm.TopLevel = true;
                frm.TopMost = true;
                Form mainFrm = _session.SmartWindowManager.MainForm as Form;
                frm.Location = new Point(mainFrm.Right - frm.Width - 8, mainFrm.Top + 150);
                frm.StartPosition = FormStartPosition.Manual;
                frm.FormClosed += new FormClosedEventHandler(frm_FormClosed);
                frm.Show(mainFrm);
            }
        }

        void frm_FormClosed(object sender, FormClosedEventArgs e)
        {
            UpdateBtn();
        }

        private void UpdateBtn()
        {
            ThemeGraphRegion newRegion = ThemeGraphRegionSetting.GetThemeGraphRegion(_productIdentify);
            if (newRegion == null || !newRegion.Enable || newRegion.PrjEnvelopeItems == null || newRegion.SelectedIndex >= newRegion.PrjEnvelopeItems.Length)
                _btnSetThemeGraphRegion.Text = "无范围应用";
            else
                _btnSetThemeGraphRegion.Text = newRegion.PrjEnvelopeItems[newRegion.SelectedIndex].Name;
        }

        void menu_Click(object sender, EventArgs e)
        {
            int? index = (sender as RadMenuItem).Tag as int?;
            ThemeGraphRegion region = ((sender as RadMenuItem).Owner as RadDropDownButtonElement).Tag as ThemeGraphRegion;
            region.Enable = true;
            region.SelectedIndex = index.Value;
            _btnSetThemeGraphRegion.Text = region.PrjEnvelopeItems[index.Value].Name;
            ThemeGraphRegionSetting.SaveThemeGraphRegion(region);
        }
    }
}
