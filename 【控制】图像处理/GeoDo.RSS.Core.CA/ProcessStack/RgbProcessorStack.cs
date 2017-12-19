using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Xml;
using System.Reflection;

namespace GeoDo.RSS.Core.CA
{
    delegate IRgbProcessor ProcessDelegate(XmlElement elem);

    public class RgbProcessorStack : IRgbProcessorStack
    {
        protected Stack<IRgbProcessor> _processStack = new Stack<IRgbProcessor>();
        protected Stack<IRgbProcessor> _unProcessStack = new Stack<IRgbProcessor>();
        private OnStackChangedHandler _stackChanged = null;
        private XmlDocument xmldoc = new XmlDocument();

        public RgbProcessorStack()
        {
        }

        public OnStackChangedHandler OnStackChanged
        {
            get { return _stackChanged; }
            set { _stackChanged = value; }
        }

        public int Count
        {
            get { return _processStack.Count; }
        }

        public IEnumerable<IRgbProcessor> Processors
        {
            get { return _processStack; }
        }

        public bool ProcessStackIsEmpty
        {
            get { return _processStack.Count == 0; }
        }

        public bool UnProcessStackIsEmpty
        {
            get { return _unProcessStack.Count == 0; }
        }

        public void Process(IRgbProcessor processor)
        {
            _processStack.Push(processor);
            _unProcessStack.Clear();
            CheckStackIsEmpty();
        }

        public void RemoveLast()
        {
            if (_processStack.Count > 0)
                _processStack.Pop();
            _unProcessStack.Clear();
            CheckStackIsEmpty();
        }

        public void ReProcess()
        {
            if (_unProcessStack.Count == 0)
                return;
            IRgbProcessor p = _unProcessStack.Pop();
            if (p != null)
            {
                _processStack.Push(p);
                CheckStackIsEmpty();
            }
        }

        public void UnProcess()
        {
            if (_processStack.Count == 0)
                return;
            IRgbProcessor p = _processStack.Pop();
            if (p != null)
            {
                _unProcessStack.Push(p);
                CheckStackIsEmpty();
            }
        }

        private void CheckStackIsEmpty()
        {
            if (_stackChanged != null)
                _stackChanged(this, ProcessStackIsEmpty, UnProcessStackIsEmpty);
        }

        public void Apply(int[][] indexesOfAOIs, Bitmap bitmap)
        {
            if (bitmap == null)
                return;
            BitmapData pdata = null;
            try
            {
                pdata = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.WriteOnly, bitmap.PixelFormat);
                //foreach (IRgbProcessor p in _processStack)
                //    p.Process(indexesOfAOIs, pdata);
                int count = Count;
                IRgbProcessor[] ps = _processStack.ToArray();
                for (int i = count - 1; i >= 0; i--)//由于是按照先进后出的Stat形式保存的，故应反序以此处理
                {
                    IRgbProcessor p = ps[i];
                    p.Process(indexesOfAOIs, pdata);
                }
            }
            finally
            {
                if (pdata != null)
                    bitmap.UnlockBits(pdata);
            }
        }


        public XmlElement ToXML()  //如果其他文档接收，可以用xmlDoc.ImportNode()方法
        {
            XmlElement root = xmldoc.CreateElement("ImageEnhance");
            XmlElement xmlQue = xmldoc.CreateElement("ProcessQueue");
            root.AppendChild(xmlQue);
            XmlElement elemProsess;
            foreach (IRgbProcessor p in _processStack)
            {
                elemProsess = xmldoc.CreateElement("Process");
                elemProsess.SetAttribute("Name", p.Name);
                elemProsess.SetAttribute("Identity", p.GetType().ToString());
                xmlQue.AppendChild(elemProsess);
                elemProsess.AppendChild(p.ToXML(xmldoc));
            }
            return root;
        }

        public void SaveTo(string filename)
        {
            if (filename == "" || filename == null)
            {
                return;
            }
            xmldoc.RemoveAll();
            xmldoc.AppendChild(ToXML());
            xmldoc.Save(filename);
        }

        public void ReadXmlElement(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }
            string sidentity;
            xmldoc.Load(filename);
            XmlNode xmlElem = xmldoc.ChildNodes[0].ChildNodes[0];
            XmlElement subElem;

            Assembly assem = Assembly.LoadFrom(AppDomain.CurrentDomain.BaseDirectory + "GeoDo.RSS.CA.dll");
            for (int i = (xmlElem.ChildNodes.Count - 1); i >= 0; i--)
            {
                subElem = (XmlElement)xmlElem.ChildNodes[i].ChildNodes[0];
                if (subElem.ChildNodes.Count == 0)
                {
                    return;
                }
                sidentity = xmlElem.ChildNodes[i].Attributes[1].Value.ToString();
                MethodInfo method = null;
                {
                    Type type = assem.GetType(sidentity);
                    if (type != null)
                    {
                        method = type.GetMethod("FromXML");
                        BindingFlags flag = BindingFlags.Public | BindingFlags.Static;
                        object[] parameters = new object[] { subElem };
                        IRgbProcessor process = (IRgbProcessor)method.Invoke(null, flag, Type.DefaultBinder, parameters, null);
                        _processStack.Push(process);
                    }
                }
            }
        }

        public void Clear()
        {
            _processStack.Clear();
            _unProcessStack.Clear();
        }
    }
}
