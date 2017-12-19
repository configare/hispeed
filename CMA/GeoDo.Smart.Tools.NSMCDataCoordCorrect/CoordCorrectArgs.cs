#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：admin     时间：2013-09-06 8:35:45
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace GeoDo.Smart.Tools.NSMCDataCoordCorrect
{
    /// <summary>
    /// 类名：CoordCorrectArgs
    /// 属性描述：保存记录数据修正设置的参数
    /// 创建者：罗战克   创建日期：2013-09-06 8:35:45
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class CoordCorrectArgs
    {
        private string _identify;
        private string _name;
        private string _indir;
        private string _filter;
        private string _datasets;
        private string _outdir;
        private string _resolution;

        public CoordCorrectArgs()
        { }

        public CoordCorrectArgs(string identify, string name, string indir, string filter, string datasets, string outdir)
        {
            _identify = identify;
            _name = name;
            _indir = indir;
            _filter = filter;
            _datasets = datasets;
            _outdir = outdir;
        }

        public string Identify
        {
            get { return _identify; }
            set { _identify = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Indir
        {
            get { return _indir; }
            set { _indir = value; }
        }

        public string Filter
        {
            get { return _filter; }
            set { _filter = value; }
        }

        public string Datasets
        {
            get { return _datasets; }
            set { _datasets = value; }
        }

        public string Outdir
        {
            get { return _outdir; }
            set { _outdir = value; }
        }

        public string Resolution
        {
            get { return _resolution; }
            set { _resolution = value; }
        }
    }

    public class CoordCorrectArgParse
    {
        private string _argFile = System.AppDomain.CurrentDomain.BaseDirectory + @"SystemData\NSMCDataCoordCorrectArgs.xml";

        public CoordCorrectArgParse()
        { }

        public CoordCorrectArgs[] LoadArgs()
        {
            XElement root = XElement.Load(_argFile);
            IEnumerable<XElement> eles = root.Elements("Data");
            List<CoordCorrectArgs> args = new List<CoordCorrectArgs>();
            foreach (XElement ele in eles)
            {
                string identify = ele.Attribute("identify").Value;
                string name = ele.Attribute("name").Value;
                XElement argele = ele.Element("Arg");
                string indir = argele.Attribute("indir").Value;
                string filter = argele.Attribute("filter").Value;
                string datasets = argele.Attribute("datasets").Value;
                string outdir = argele.Attribute("outdir").Value;
                string resolution = argele.Attribute("resolution").Value;
                CoordCorrectArgs arg = new CoordCorrectArgs(identify, name, indir, filter, datasets, outdir);
                arg.Resolution = resolution;
                args.Add(arg);
            }
            return args.ToArray();
        }

        public void WriteToXml(CoordCorrectArgs[] args)
        {
            XElement root = new XElement("DataCoorrect");
            foreach (CoordCorrectArgs arg in args)
            {
                XElement argele = new XElement("Arg",
                    new XAttribute("indir", arg.Indir),
                    new XAttribute("filter", arg.Filter),
                    new XAttribute("datasets", arg.Datasets),
                    new XAttribute("resolution", arg.Resolution),
                    new XAttribute("outdir", arg.Outdir));
                root.Add(new XElement("Data",
                    new XAttribute("identify", arg.Identify), 
                    new XAttribute("name", arg.Name),
                    argele));
            }
            root.Save(_argFile);
        }
    }
}
