#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：罗战克     时间：2013-09-04 15:55:47
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

namespace GeoDo.RSS.DF.GDAL.HDF5Universal
{
    /// <summary>
    /// 类名：FY3L2L3ProductDefParse
    /// 属性描述：风云三L2L3级产品数据定义。
    /// 通过约定固定的文件命名规范、固定的数据集，定义固定的L2、L3级产品，通过配置文件来描述，并且可以扩展。
    /// 创建者：罗战克   创建日期：2013-09-04 15:55:47
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class FY3L2L3ProductDefParse
    {
        private FY3L2L3HdfProductDef[] _allHdfDdf = null;

        public FY3L2L3ProductDefParse()
        {
            if(_allHdfDdf==null)
                _allHdfDdf = Parse();
        }

        private static FY3L2L3HdfProductDef[] Parse()
        {
            string filename = System.AppDomain.CurrentDomain.BaseDirectory + "FY3L2L3ProductDef.xml";
            XDocument doc = XDocument.Load(filename);
            XElement root = doc.Root;
            IEnumerable<XElement> eles = root.Elements("Product");
            List<FY3L2L3HdfProductDef> list = new List<FY3L2L3HdfProductDef>();
            foreach (XElement ele in eles)
            {
                FY3L2L3HdfProductDef hdfDef = FY3L2L3HdfProductDef.Parse(ele);
                if (hdfDef != null)
                    list.Add(hdfDef);
            }
            return list.ToArray();
        }

        public FY3L2L3HdfProductDef[] GetAll()
        {
            return _allHdfDdf;
        }
    }
}
