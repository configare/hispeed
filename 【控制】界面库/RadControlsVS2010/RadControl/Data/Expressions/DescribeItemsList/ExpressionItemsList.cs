using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml;

namespace Telerik.Data.Expressions
{
    public class ExpressionItemsList : List<ExpressionItem>
    {
        /// <summary>
        /// Load expression items list from embedded in Telerik assembly xml source
        /// </summary>
        public void LoadFromXML()
        {
            string path = "Telerik.WinControls.Data.Expressions.DescribeItemsList.ExpressionItemsListData.xml";
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            LoadFromXML(stream);
        }

        /// <summary>
        /// Load expression items list
        /// </summary>
        /// <param name="path">Xml file path</param>
        public void LoadFromXML(string path)
        {
            FileStream stream = File.OpenRead(path);
            LoadFromXML(stream);
        }

        /// <summary>
        /// Load expression items list
        /// </summary>
        /// <param name="stream"></param>
        public void LoadFromXML(Stream stream)
        {
            XmlDocument xml;
            if (stream != null)
            {
                this.Clear();

                xml = new XmlDocument();
                xml.Load(stream);
                XmlNodeList nodes = xml.GetElementsByTagName("ExpressionItem");
                foreach (XmlNode node in nodes)
                {
                    ExpressionItem expressionItem = new ExpressionItem();
                    expressionItem.Name = node.Attributes["Name"].Value;
                    expressionItem.Syntax = node.Attributes["Syntax"].Value;
                    expressionItem.Value = node.Attributes["Value"].Value;
                    expressionItem.Type = (ExpressionItemType)Enum.Parse(typeof(ExpressionItemType), node.Attributes["Type"].Value, true);
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        if (childNode.Name == "Description")
                        {
                            expressionItem.Description = childNode.InnerText;
                            break;
                        }
                    }

                    this.Add(expressionItem);
                }
            }
        }
    }
}
