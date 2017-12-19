using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.RSS.Layout.GDIPlus
{
    public class GxdRasterItem:GxdItem,IGxdRasterItem
    {
        protected string _fileName;
        protected object _arguments;
        protected string[] _fileOpenArgs;
        protected string _colorTableName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="arguments">arguments的值为一定格式的字符串，用于记录该栅格文件打开需要的一些参数</param>
        public GxdRasterItem(string fileName, object arguments)
        {
            _fileName = fileName;
            _arguments = arguments;
        }

        public GxdRasterItem(string fileName, object arguments, string[] fileopenArgs, string colorTableName)
        {
            _fileName = fileName;
            _arguments = arguments;
            _fileOpenArgs = fileopenArgs;
            _colorTableName = colorTableName;
        }

        public string FileName
        {
            get { return _fileName; }
        }

        public object Arguments
        {
            get { return _arguments; }
        }

        public string[] FileOpenArgs
        {
            get { return _fileOpenArgs; }
        }

        public string ColorTableName
        {
            get { return _colorTableName; }
        }

        //GxdRasterItem
        public override System.Xml.Linq.XElement ToXml()
        {
            XElement ele = new XElement("GxdRasterItem");
            ele.SetAttributeValue("filename", _fileName ?? string.Empty);
            ele.SetElementValue("RasterItemArgument", GetArgXElement());
            ele.SetAttributeValue("filename", _fileName ?? string.Empty);
            if (_fileOpenArgs != null && !string.IsNullOrWhiteSpace(_colorTableName) && _colorTableName.Length != 0)
                ele.SetAttributeValue("fileargs", GetFileArgs());
            if(!string.IsNullOrWhiteSpace(_colorTableName))
                ele.SetElementValue("ColorTableName",_colorTableName);
            return ele;
        }

        private object GetFileArgs()
        {
            if (_fileOpenArgs == null || _fileOpenArgs.Length == 0)
                return null;
            string fileargs = string.Join(";", _fileOpenArgs);
            return fileargs;
        }

        private string GetArgXElement()
        {
            return _arguments != null ? _arguments.ToString() : string.Empty;
        }
    }
}
