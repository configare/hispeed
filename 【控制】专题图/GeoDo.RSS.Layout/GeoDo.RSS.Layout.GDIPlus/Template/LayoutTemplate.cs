using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class LayoutTemplate : ILayoutTemplate, IDisposable
    {
        private string _name = null;
        private string _text = null;
        private ILayout _layout;
        private string _fullPath = null;

        /// <summary>
        /// 通过文件地址构造模板
        /// </summary>
        /// <param name="fname">文件的完整路径</param>
        public LayoutTemplate(string fname)
        {
            if (!File.Exists(fname))
                return;
            if (Path.GetExtension(fname).ToLower() != ".gxt")
                return;
            _name = _text = Path.GetFileNameWithoutExtension(fname);
            _fullPath = fname;
            //load layout from .gxt file
            _layout = LayoutFromFile.LoadFromFile(fname);
        }

        /// <summary>
        /// 构造当前的专题图模板
        /// </summary>
        /// <param name="host"></param>
        /// <param name="name"></param>
        /// <param name="text"></param>
        public LayoutTemplate(ILayoutHost host)
        {
            _layout = host.LayoutRuntime.Layout;
        }

        public LayoutTemplate(ILayout layout)
        {
            _layout = layout;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Text
        {
            get { return _text; }
            set { _text = value; }
        }

        public ILayout Layout
        {
            get { return _layout; }
        }

        public string FullPath
        {
            get { return _fullPath; }
            set { _fullPath = value; }
        }

        public Bitmap GetOverview(Size size)
        {
            using (LayoutControl lc = new LayoutControl())
            {
                lc.Width = size.Width;
                lc.Height = size.Height;
                ILayoutHost host = new LayoutHost(lc);
                (host as LayoutHost)._template = this;
                host.LayoutRuntime.ChangeLayout(_layout);
                (host as LayoutHost)._layout = _layout;
                host.ToSuitedSize(_layout);
                return host.SaveToBitmap();
            }
        }

        public void SaveTo(string fname)
        {
            //save to .gxt file
            if (Path.GetExtension(fname).ToLower() != ".gxt")
                return;
            LayoutToFile.SaveToFile(fname, _layout);
        }

        public void ApplyVars(Dictionary<string, string> vars)
        {
            foreach (string varName in vars.Keys)
                TryApplyVars(_layout, varName, vars[varName]);
        }

        private void TryApplyVars(IElement ele, string varName, string varValue)
        {
            if (ele is TextElement)
            {
                TextElement txt = ele as TextElement;
                if (txt.Text.Contains(varName))
                    txt.Text = txt.Text.Replace(varName, varValue);
            }
            else if (ele is MultlineTextElement)
            {
                MultlineTextElement txt = ele as MultlineTextElement;
                if (txt.Text.Contains(varName))
                    txt.Text = txt.Text.Replace(varName, varValue);
            }
            else if (ele is IElementGroup)
            {
                foreach (IElement subEle in (ele as IElementGroup).Elements)
                    TryApplyVars(subEle, varName, varValue);
            }
        }

        public void Dispose()
        {
            
        }


        public static ILayoutTemplate FindTemplate(string templateName)
        {
            if (string.IsNullOrEmpty(templateName))
                return null;
            string path = AppDomain.CurrentDomain.BaseDirectory + "LayoutTemplate";
            if (!Directory.Exists(path))
                return null;
            string[] files = Directory.GetFiles(path, "*" + templateName + ".gxt", SearchOption.AllDirectories);
            if (files == null || files.Length == 0)
                return null;
            //如果有多个文件匹配，则应用搜索到的第一个文件
            string full = Path.GetFullPath(files[0]);
            ILayoutTemplate template = new LayoutTemplate(full);
            return template;
        }

        public static ILayoutTemplate LoadTemplateFrom(string fname)
        {
            ILayoutTemplate template = new LayoutTemplate(fname);
            return template;
        }

        public static ILayoutTemplate CreateFrom(string xmlContent)
        {
            ILayout layout = LayoutFromFile.LoadFromXml(xmlContent);
            return new LayoutTemplate(layout);
        }
    }
}
