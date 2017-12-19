using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.WinControls.UI;
using GeoDo.RSS.Core.UI;
using System.Drawing;
using System.IO;
using System.Xml;
using GeoDo.RSS.UI.AddIn.Theme;
using GeoDo.RSS.MIF.Core;
using System.Xml.Linq;
using GeoDo.RSS.Core.DF;
using System.Windows.Forms;

namespace GeoDo.RSS.MIF.Prds.BAG
{
    public class AvgNDVISetter
    {
        public static void RegistButton(RadDropDownButtonElement btnRegion, RibbonTab tab, ISmartSession session)
        {
            AvgNDVISetter rad = new AvgNDVISetter(btnRegion,tab,session);
            rad.AddAvgNDVISettingItem();
        }

        private static string ThemeXMLPath = AppDomain.CurrentDomain.BaseDirectory + @"Themes\CMAThemes.xml";
        private RadDropDownButtonElement _btnAvgNDVISetting;
        private RibbonTab _tab;
        private XElement _themeElement = null;
        private ISmartSession _session = null;
        private List<NDVISettingItem> _settingItemList = new List<NDVISettingItem>();

        private AvgNDVISetter(RadDropDownButtonElement btn, RibbonTab tab,ISmartSession session)
        {
            _btnAvgNDVISetting = btn;
            _tab = tab;
            _session = session;
        }

        private void AddAvgNDVISettingItem()
        {
            _btnAvgNDVISetting.MinSize = new Size(80, 20);
            _btnAvgNDVISetting.MaxSize = new Size(120, 100);
            _btnAvgNDVISetting.Text = "未使用端元值";
            _btnAvgNDVISetting.TextAlignment = ContentAlignment.BottomCenter;
            _btnAvgNDVISetting.ImageAlignment = ContentAlignment.TopCenter;
            _btnAvgNDVISetting.Click += new EventHandler(_btnAvgNDVISetting_Click);
            InitAvgNDVISetting();
            RadRibbonBarGroup gpAvgNDVISetting = new RadRibbonBarGroup();
            gpAvgNDVISetting.Text = "端元值设置";
            gpAvgNDVISetting.Items.Add(_btnAvgNDVISetting);
            _tab.Items.Add(gpAvgNDVISetting);
        }


        private void InitAvgNDVISetting()
        {
            //查询配置文件，是否使用端元值 
            _themeElement = XElement.Load(ThemeXMLPath);
            if (_themeElement==null)
                return;
            SetDefaultNode();
            CreateDropDownButton();
        }

        private void CreateDropDownButton()
        {
            RadMenuItem miEdit = new RadMenuItem("编辑端元值");
            _btnAvgNDVISetting.Items.Add(miEdit);
            miEdit.Click += new EventHandler(miEdit_Click);
            if (_settingItemList == null || _settingItemList.Count < 1)
                return;
            RadMenuItem miApply = new RadMenuItem("应用端元值");
            miApply.Click += new EventHandler(miApply_Click);
            _btnAvgNDVISetting.Items.Add(miApply);
            RadMenuItem miCancel = new RadMenuItem("取消应用");
            miCancel.Click += new EventHandler(miCancel_Click);
            _btnAvgNDVISetting.Items.Add(miCancel);
            foreach (NDVISettingItem item in _settingItemList)
            {
                if (item.IsUse)
                {
                    _btnAvgNDVISetting.Text = "应用端元值";
                }
            }
        }

        void miEdit_Click(object sender, EventArgs e)
        {
            frmArgNDVIManager frm = new frmArgNDVIManager();
            {
                frm.Init(_settingItemList);
                frm.TopLevel = true;
                frm.TopMost = true;
                Form mainFrm = _session.SmartWindowManager.MainForm as Form;
                frm.Location = new Point(mainFrm.Right - frm.Width - 8, mainFrm.Top + 150);
                frm.StartPosition = FormStartPosition.Manual;
                DialogResult result = frm.ShowDialog();
                if (result == DialogResult.OK)
                {
                    _settingItemList = frm.SettingList;
                    SaveSettingItems();
                    UpdateBtn();
                } 
            }
        }

        private void UpdateBtn()
        {
            foreach (NDVISettingItem item in _settingItemList)
            {
                if (item.IsUse)
                    _btnAvgNDVISetting.Text = "应用端元值";
            }
            _btnAvgNDVISetting.Text = "未使用端元值";
        }

        void miCancel_Click(object sender, EventArgs e)
        {
            foreach (NDVISettingItem item in _settingItemList)
            {
                item.IsUse = false;
            }
            _btnAvgNDVISetting.Text = "未使用端元值";
            SaveSettingItems();
        }

        void miApply_Click(object sender, EventArgs e)
        {
            foreach (NDVISettingItem item in _settingItemList)
            {
                item.IsUse = true;
            }
            _btnAvgNDVISetting.Text = "应用端元值";
            SaveSettingItems();
        }

        void _btnAvgNDVISetting_Click(object sender, EventArgs e)
        {
            _btnAvgNDVISetting.ShowDropDown();
        }

        private void SetDefaultNode()
        {
            //解析DefaultValue节点
            XElement cmaTheme = _themeElement.Element("Theme");
            if (cmaTheme == null)
                return;
            IEnumerable<XElement> bagProduct = from item in cmaTheme.Element("Products").Elements("Product")
                                               where item.Attribute("identify").Value.Equals("BAG")
                                               select item;
            if (bagProduct == null || bagProduct.Count() < 1)
                return;
            IEnumerable<XElement> extractElement = from item in bagProduct.First().Element("SubProducts").Elements("SubProduct")
                                                   where item.Attribute("identify").Value.Equals("DBLV")
                                                   select item;
            IEnumerable<XElement> algorithmElement = from item in extractElement.First().Element("Algorithms").Elements("Algorithm")
                                                    where item.Attribute("identify").Value.Equals("BAGExtract")
                                                    select item;
            IEnumerable<XElement> argumentElement = from item in algorithmElement.First().Element("Arguments").Elements("Argument")
                                                    where item.Attribute("name").Value.Equals("NDVISetting")
                                                    select item;
            XElement valueList = argumentElement.First().Element("DefaultValue");
            //解析为NDVISettingItem[]
            ParseDefaultValue(valueList);
        }

        private void ParseDefaultValue(XElement valueElement)
        {
            if (_settingItemList != null)
                _settingItemList.Clear();
            IEnumerable<XElement> items = valueElement.Elements("ValueItem");
            if (items == null || items.Count() < 1)
                return;
            XElement ele;
            for (int i = 0; i < items.Count(); i++)
            {
                ele = items.ElementAt(i);
                if (ele == null)
                    continue;
                NDVISettingItem item = new NDVISettingItem();
                item.Name = ele.Attribute("name").Value;
                float value;
                if (float.TryParse(ele.Attribute("minvalue").Value, out value))
                    item.MinValue = value;
                if (float.TryParse(ele.Attribute("maxvalue").Value, out value))
                    item.MaxValue = value;
                bool isUse;
                if (bool.TryParse(ele.Attribute("isuse").Value, out isUse))
                    item.IsUse = isUse;
                string envelope = ele.Attribute("envelope").Value;
                item.Envelope = ParseEnvelope(envelope);
                _settingItemList.Add(item);
            }
        }

        private CoordEnvelope ParseEnvelope(string envelope)
        {
            if (string.IsNullOrEmpty(envelope))
                return null;
            string[] strArray = envelope.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
            if (strArray == null || strArray.Length != 4)
                return null;
            CoordEnvelope env = new CoordEnvelope(double.Parse(strArray[0]), double.Parse(strArray[1]),
                                                 double.Parse(strArray[2]), double.Parse(strArray[3]));
            return env;
        }

        private void SaveSettingItems()
        {
            if (_themeElement == null)
                return;
            XElement cmaTheme = _themeElement.Element("Theme");
            if (cmaTheme == null)
                return;
            IEnumerable<XElement> bagProduct = from item in cmaTheme.Element("Products").Elements("Product")
                                               where item.Attribute("identify").Value.Equals("BAG")
                                               select item;
            if (bagProduct == null || bagProduct.Count() < 1)
                return;
            IEnumerable<XElement> extractElement = from item in bagProduct.First().Element("SubProducts").Elements("SubProduct")
                                                   where item.Attribute("identify").Value.Equals("DBLV")
                                                   select item;
            IEnumerable<XElement> algorithmElement = from item in extractElement.First().Element("Algorithms").Elements("Algorithm")
                                                     where item.Attribute("identify").Value.Equals("BAGExtract")
                                                     select item;
            IEnumerable<XElement> argumentElement = from item in algorithmElement.First().Element("Arguments").Elements("Argument")
                                                    where item.Attribute("name").Value.Equals("NDVISetting")
                                                    select item;
            XElement valueList = argumentElement.First().Element("DefaultValue");
            valueList.Elements("ValueItem").Remove();
            if (_settingItemList == null || _settingItemList.Count < 1)
            {
                _themeElement.Save(ThemeXMLPath);
                return;
            }
            foreach (NDVISettingItem item in _settingItemList)
            {
                valueList.Add(new XElement("ValueItem",new XAttribute("name",item.Name),
                    new XAttribute("envelope",EnvelopeToString(item.Envelope)),
                    new XAttribute("minvalue",item.MinValue),
                    new XAttribute("maxvalue",item.MaxValue),
                    new XAttribute("isuse",item.IsUse)));
            }
            _themeElement.Save(ThemeXMLPath);
        }

        private string EnvelopeToString(CoordEnvelope envelope)
        {
            if (envelope == null)
                return "";
            return envelope.MinX.ToString() + "," + envelope.MaxX.ToString() + "," + envelope.MinY.ToString() + "," + envelope.MaxY;
        }
    }
}
