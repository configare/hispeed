using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Drawing;

namespace GeoDo.RSS.Core.UI
{
    public class UIFrameworkDefinition : IDisposable
    {
        private Dictionary<string, Font> _fonts = new Dictionary<string, Font>();
        private Dictionary<string, IUIResourceProvider> _resProviders = new Dictionary<string, IUIResourceProvider>();
        private UITab[] _uiTabs;
        private UITheme _uitheme;
        private UIFileMenuView _uiFileMenu;
        private UIAppInfo _appInfo;
        private UIQuickAccessBar _quickAccessBar;

        public UIFrameworkDefinition()
        {
            //string resname = "GeoDo.RSS.Core.UI.Framework.UI.DefaultUI.xml";
            //using (Stream stream = this.GetType().Assembly.GetManifestResourceStream(resname))
            {
                string uiConfig = System.Configuration.ConfigurationManager.AppSettings["UIConfig"];
                //XDocument doc = XDocument.Load(stream);
                XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory +uiConfig);// "DefaultUI.xml");
                ParseAppInfo(doc);
                ParseTheme(doc);
                ParseFonts(doc);
                ParseResProviders(doc);
                ParseQuickAccessBar(doc);
                ParserTabs(doc);
            }
        }

        private void ParseQuickAccessBar(XDocument doc)
        {
            XElement root = doc.Element("UIFrameworkDef").Element("UITabs").Element("QuickAccessBar");
            if (root == null)
                return;
            var eles = root.Elements("UIButton");
            if (eles == null || eles.Count() == 0)
                return;
            List<UIButton> btns = new List<UIButton>();
            foreach (XElement ele in eles)
            {
                btns.Add(ParseUIButton(ele));
            }
            _quickAccessBar = new UIQuickAccessBar();
            _quickAccessBar.Buttons = btns.Count > 0 ? btns.ToArray() : null;
        }

        private void ParseAppInfo(XDocument doc)
        {
            XElement root = doc.Element("UIFrameworkDef").Element("AppInfo");
            if (root == null)
                return;
            root = root.Element("TileInfo");
            if (root == null)
                return;
            string tile = GetStringAttribute(root, "tile");
            UITileInfo tileInfo = new UITileInfo() { Tile = tile };
            _appInfo = new UIAppInfo() { TileInfo = tileInfo };
        }

        private void ParseTheme(XDocument doc)
        {
            XElement root = doc.Element("UIFrameworkDef").Element("Theme");
            if (root == null)
                return;
            _uitheme = new UITheme(root.Attribute("name").Value, root.Attribute("assembly").Value, root.Attribute("class").Value);
        }

        /*
         * <ResourceLocations>
              <DirResourceProvider name="" dir=""/>
              <EmmbedResourceProvider name="system" assembly="" namespace=""/>
           </ResourceLocations>
         */
        private void ParseResProviders(XDocument doc)
        {
            XElement root = doc.Element("UIFrameworkDef").Element("ResourceLocations");
            if (root == null)
                return;
            foreach (XElement ele in root.Elements())
            {
                switch (ele.Name.LocalName)
                {
                    case "DirResourceProvider":
                        string dir = ele.Attribute("dir").Value;
                        if (dir.Contains("$systemroot$"))
                            dir = dir.Replace("$systemroot$", System.AppDomain.CurrentDomain.BaseDirectory);
                        _resProviders.Add(ele.Attribute("name").Value, new DirUIResourceProvider(dir));
                        break;
                    case "EmmbedResourceProvider":
                        string fname = AppDomain.CurrentDomain.BaseDirectory + ele.Attribute("assembly").Value;
                        if (File.Exists(fname))
                            _resProviders.Add(ele.Attribute("name").Value, new EmmbedUIResourceProvider(ele.Attribute("assembly").Value, ele.Attribute("namespace").Value));
                        break;
                }
            }
        }

        private void ParseFonts(XDocument doc)
        {
            XElement fontsElement = doc.Element("UIFrameworkDef").Element("Fonts");
            foreach (XElement fElement in fontsElement.Elements("Font"))
            {
                Font font = new Font(fElement.Attribute("family").Value, float.Parse(fElement.Attribute("size").Value));
                _fonts.Add(fElement.Attribute("name").Value, font);
            }
        }

        private void ParserTabs(XDocument doc)
        {
            List<UITab> tabs = new List<UITab>();
            XElement root = doc.Element("UIFrameworkDef").Element("UITabs");
            var tabEles = root.Elements("UITab");
            foreach (XElement uiTab in tabEles)
            {
                if (uiTab.Attribute("name") == null || uiTab.Attribute("text") == null || uiTab.Attribute("font") == null)
                    continue;
                UITab tab = new UITab(uiTab.Attribute("name").Value, uiTab.Attribute("text").Value, uiTab.Attribute("font").Value);
                TryLoadContentFromUIProvider(uiTab, tab);
                tab.Children = ParseItemsOfTab(uiTab);
                tabs.Add(tab);
            }
            XElement uiMenu = root.Element("FileMenuView");
            ParserFileMenu(uiMenu);
            _uiTabs = tabs.Count > 0 ? tabs.ToArray() : null;
        }

        private void ParserFileMenu(XElement uiMenu)
        {
            XElement uiItems = uiMenu.Element("Items");
            _uiFileMenu = new UIFileMenuView();
            ParserFileMenuItems(uiItems);
        }

        private void ParserFileMenuItems(XElement uiItems)
        {
            List<UIMenuItem> coms = new List<UIMenuItem>();
            foreach (XElement item in uiItems.Elements())
            {
                string type = item.Name.LocalName;
                switch (type)
                {
                    case "MenuItem":
                        coms.Add(ParseMenuItem(item));
                        break;
                }
            }
            _uiFileMenu.MenuItems = coms.Count > 0 ? coms.ToArray() : null;
        }

        private UIMenuItem ParseMenuItem(XElement e)
        {
            UIMenuItem it = new UIMenuItem();
            it.Text = e.Attribute("text").Value;
            if (e.Attribute("identify") != null)
            {
                int id = 0;
                int.TryParse(e.Attribute("identify").Value, out id);
                it.Identify = id;
            }
            it.Image = e.Attribute("image").Value;
            if (e.Attribute("argument") != null)
                it.Argument = e.Attribute("argument").Value;
            if (e.Attribute("page") != null)
                it.Provider = e.Attribute("page").Value;
            return it;
        }

        private void TryLoadContentFromUIProvider(XElement uiTab, UITab tab)
        {
            string provider = null;
            if (uiTab.Attribute("provider") == null)
                return;
            provider = uiTab.Attribute("provider").Value;
            if (provider == "assembly:class")
                return;
            tab.Provider = provider;
        }

        private UIItem[] ParseItemsOfTab(XElement uiTab)
        {
            List<UIItem> items = new List<UIItem>();
            XElement root = uiTab.Element("UIItems");
            if (root == null)
                return null;
            foreach (XElement itElement in root.Elements())
            {
                string type = itElement.Name.LocalName;
                switch (type)
                {
                    case "UICommandGroup":
                        UICommandGroup uig = ParseUICommandGroup(itElement);
                        if (uig != null)
                            items.Add(uig);
                        break;
                }
            }
            return items.Count > 0 ? items.ToArray() : null;
        }

        private UICommandGroup ParseUICommandGroup(XElement itElement)
        {
            UICommand[] cmds = ParseUICommands(itElement);
            UICommandGroup uig = new UICommandGroup(itElement.Attribute("name").Value, itElement.Attribute("text").Value, cmds);
            string provider = null;
            if (itElement.Attribute("provider") != null)
                provider = itElement.Attribute("provider").Value;
            if (provider != null && provider != "assembly:class")
                uig.Provider = provider;
            uig.Visible = GetBoolAttribute(itElement, "visible");
            uig.AllowCollapsed = GetBoolAttribute(itElement, "allowcollapsed");//AllowCollapsed
            return uig;
        }

        private bool GetBoolAttribute(XElement itElement, string p)
        {
            if (itElement.Attribute("visible") == null)
                return true;
            string bstr = itElement.Attribute("visible").Value;
            bool v = true;
            if (bool.TryParse(bstr, out v))
                return v;
            return true;
        }

        private string GetStringAttribute(XElement ele, string attName)
        {
            if (ele == null)
                return string.Empty;
            if (ele.Attribute(attName) == null)
                return string.Empty;
            return ele.Attribute(attName).Value;
        }

        private UICommand[] ParseUICommands(XElement itElement)
        {
            List<UICommand> uicommands = new List<UICommand>();
            foreach (XElement ele in itElement.Elements())
            {
                string name = ele.Name.LocalName;
                switch (name)
                {
                    case "UIButton":
                        UIButton btn = ParseUIButton(ele);
                        if (btn != null)
                            uicommands.Add(btn);
                        break;
                    case "UIDropDownButton":
                        UIDropDownButton dbtn = ParseUIDropDownButton(ele);
                        if (dbtn != null)
                            uicommands.Add(dbtn);
                        break;
                }
            }
            return uicommands.Count > 0 ? uicommands.ToArray() : null;
        }

        private UIDropDownButton ParseUIDropDownButton(XElement ele)
        {
            UIDropDownButton btn = new UIDropDownButton();
            btn.Text = ele.Attribute("text").Value;
            btn.Image = ele.Attribute("image").Value;
            if (ele.Attribute("font") != null)
                btn.Font = ele.Attribute("font").Value;
            btn.ExpandArrowButton = bool.Parse(ele.Attribute("expandarrowbutton").Value);
            btn.ArrowPosition = ele.Attribute("arrowposition").Value;
            if (ele.Attribute("textaligment") != null)
                btn.TextAligment = ele.Attribute("textaligment").Value;
            if (ele.Attribute("imagealigment") != null)
                btn.ImageAligment = ele.Attribute("imagealigment").Value;
            List<UIItem> items = new List<UIItem>();
            foreach (XElement e in ele.Elements())
            {
                switch (e.Name.LocalName)
                {
                    case "MenuHeaderItem":
                        items.Add(new UIMenuHeader(e.Attribute("text").Value));
                        break;
                    case "MenuItem":
                        UIMenuItem it = new UIMenuItem();
                        it.Text = e.Attribute("text").Value;
                        int id = 0;
                        int.TryParse(e.Attribute("identify").Value, out id);
                        it.Identify = id;
                        it.Image = e.Attribute("image").Value;
                        it.Argument = e.Attribute("argument").Value;
                        items.Add(it);
                        break;
                }
            }
            btn.MenuItems = items.Count > 0 ? items.ToArray() : null;
            return btn;
        }

        private UIButton ParseUIButton(XElement ele)
        {
            int id = 0;
            if (ele.Attribute("identify") != null)
                int.TryParse(ele.Attribute("identify").Value, out id);
            UIButton btn = new UIButton(ele.Attribute("name").Value, ele.Attribute("text").Value, id, ele.Attribute("image").Value);
            if (ele.Attribute("textalignment") != null)
                btn.TextAligment = ele.Attribute("textalignment").Value;
            if (ele.Attribute("imagealignment") != null)
                btn.TextAligment = ele.Attribute("imagealignment").Value;
            if (ele.Attribute("argument") != null)
                btn.Argument = ele.Attribute("argument").Value;
            if (ele.Attribute("textimagerelation") != null)
                btn.TextImageRelation = ele.Attribute("textimagerelation").Value;
            return btn;
        }


        public UIQuickAccessBar QuickAccessBar
        {
            get { return _quickAccessBar; }
        }

        public Font GetFont(string name)
        {
            if (name != null && _fonts.ContainsKey(name))
                return _fonts[name];
            return null;
        }

        public Image GetImage(string resIdentify)
        {
            IUIResourceProvider prd = null;
            if (resIdentify == null)
                return null;
            string[] parts = resIdentify.Split(':');
            if (parts.Length == 1)
                return null;
            try
            {
                prd = _resProviders[parts[0]];
            }
            catch (Exception e)
            {

            }
            if (prd == null)
                return null;
            return prd.GetImage(parts[1]);
        }

        public UIAppInfo AppInfo
        {
            get { return _appInfo; }
        }

        public UITab[] UITabs
        {
            get { return _uiTabs; }
        }

        public UITheme UITheme
        {
            get { return _uitheme; }
        }

        public UIFileMenuView UIFileMenu
        {
            get { return _uiFileMenu; }
        }

        public void Dispose()
        {
            _uiTabs = null;
            _uiFileMenu = null;
            if (_fonts != null && _fonts.Count > 0)
            {
                foreach (Font fnt in _fonts.Values)
                    fnt.Dispose();
                _fonts.Clear();
                _fonts = null;
            }
        }
    }
}
