using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;

namespace GeoDo.RSS.DI.DAT
{
    public class DATXMLParser
    {
        private string _xmlFile;

        public DATXMLParser()
        {
            string[] transXmls = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "DATTransDef.xml", SearchOption.AllDirectories);
            if (transXmls == null || transXmls.Length != 1)
                return;
            _xmlFile = transXmls[0];
        }

        public TransDef GetTransDef()
        {
            if (string.IsNullOrEmpty(_xmlFile) || !File.Exists(_xmlFile))
                return null;
            XElement root = XElement.Load(_xmlFile);
            if (root == null)
                return null;
            ProductDef[] products = ParseProduct(root);
            if (products == null)
                return null;
            return new TransDef(products);
        }

        private ProductDef[] ParseProduct(XElement root)
        {
            if (!root.HasElements)
                return null;
             string name, smartIdentify, fileIdentify;
            XElement[] elements = root.Elements().ToArray();
            SubProductDef[] subProducts = null;
            List<ProductDef> productDefs = new List<ProductDef>();
            foreach (XElement productElement in elements)
            {
                if (!productElement.HasElements)
                    continue;
                subProducts = ParseSubProduct(productElement);
                if (subProducts == null)
                    continue;
                  name = GetStringAttr(productElement, "name");
                smartIdentify = GetStringAttr(productElement, "smartidentify");
                fileIdentify = GetStringAttr(productElement, "fileidentify");
                productDefs.Add(new ProductDef(name, smartIdentify, fileIdentify, subProducts));
            }
            return productDefs.Count==0?null:productDefs.ToArray();
        }

        private SubProductDef[] ParseSubProduct(XElement productElement)
        {
            XElement[] elements = productElement.Elements().ToArray();
            TabelDef[] tableDefs = null;
            List<SubProductDef> subProductDefs = new List<SubProductDef>();
            SubProductDef subProductDef = null;
            string name, smartIdentify, fileIdentify;
            foreach (XElement subProductElement in elements)
            {
                name = GetStringAttr(subProductElement, "name");
                smartIdentify = GetStringAttr(subProductElement, "smartidentify");
                fileIdentify = GetStringAttr(subProductElement, "fileidentify");
                if (!subProductElement.HasElements)
                {
                    subProductDef = new SubProductDef(name, smartIdentify, fileIdentify, null);
                }
                else
                {
                    tableDefs = ParseTables(subProductElement);
                    subProductDef = new SubProductDef(name, smartIdentify, fileIdentify, tableDefs);
                }
                subProductDefs.Add(subProductDef);
            }
            return subProductDefs.Count == 0 ? null : subProductDefs.ToArray();
        }

        private TabelDef[] ParseTables(XElement subProductElement)
        {
            XElement[] elements = subProductElement.Elements().ToArray();
            List<TabelDef> tableDefs = new List<TabelDef>();
            TabelDef tableDef = null;
            float smartValue;
            float fileValue;
            foreach (XElement tableDefE in elements)
            {
                smartValue = GetFloatAttr(tableDefE, "smartvalue");
                fileValue = GetFloatAttr(tableDefE, "filevalue");
                if (smartValue == float.MinValue || fileValue == float.MinValue)
                    continue;
                tableDef = new TabelDef(smartValue, fileValue);
                tableDefs.Add(tableDef);
            }
            return tableDefs.Count == 0 ? null : tableDefs.ToArray();
        }

        private float GetFloatAttr(XElement element, string attributeName)
        {
            float result = float.MinValue;
            XAttribute attri = element.Attribute(attributeName);
            if (attri == null)
                return result;
            if (float.TryParse(attri.Value, out result))
                return result;
            return float.MinValue;
        }

        private string GetStringAttr(XElement element, string attributeName)
        {
            XAttribute attri = element.Attribute(attributeName);
            if (attri == null)
                return string.Empty;
            return attri.Value;
        }
    }
}
