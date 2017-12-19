using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace GeoDo.Smart.ValueComposite
{
    /// <summary>
    /// 参数解析类
    /// </summary>
    public class ArgumentParser
    {
        string argumentFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ArgConfig.xml");

        public ArgumentParser()
        {
        }

        public string ParseArgumentValue(string argumentName)
        {
            XElement rootElement = XElement.Load(argumentFile);
            if (rootElement == null)
                return null;
            IEnumerable<XElement> arguments = rootElement.Elements("Argument");
            if (arguments == null)
                return null;
            foreach (XElement arg in arguments)
            {
                if (arg.Attribute("name").Value == argumentName)
                    return arg.Attribute("value").Value;
            }
            return null;
        }
    }
}
