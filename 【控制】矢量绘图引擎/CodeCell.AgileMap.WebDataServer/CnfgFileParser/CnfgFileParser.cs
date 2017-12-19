using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    internal static class CnfgFileParser
    {
        /*
         * <VectorServerInstances>
               <Instance name="北京1:5000万基础矢量服务" descrption="" args =""/>
               <Instance name="世界1:5000万基础矢量服务" descrption="" args =""/>
           </VectorServerInstances>
         */
        public static IServerInstance[] Parse(string cnfgfile)
        {
            XDocument doc = XDocument.Load(cnfgfile);
            XElement root = doc.Element("VectorServerInstances");
            var eles = root.Elements("Instance");
            if (eles == null)
                return null;
            List<IServerInstance> defs = new List<IServerInstance>();
            foreach (XElement ele in eles)
            {
                int id = int.Parse(ele.Attribute("id").Value);
                string name = ele.Attribute("name").Value;
                string desc = ele.Attribute("description").Value;
                string args = ele.Attribute("args").Value;
                InstanceIdentify idobj = new InstanceIdentify(id, name, desc);
                IServerInstance def = ServerInstanceFactory.GetServerInstance(idobj, args);
                defs.Add(def);
            }
            return defs.Count > 0 ? defs.ToArray() : null;
        }
    }
}
