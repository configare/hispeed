using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace CodeCell.Bricks.ModelFabric
{
    public static class ArgBindingHelper
    {
        static ArgBindingHelper()
        { 
        }

        public static BindingAttribute GetBingdingAttribute(string propertyName, IAction action)
        {
            Dictionary<PropertyInfo, BindingAttribute> propertyInfos = GetBindingProperties(action.GetType());
            foreach (PropertyInfo pinfo in propertyInfos.Keys)
            {
                if (pinfo.Name.ToUpper() == propertyName.ToUpper())
                {
                    return propertyInfos[pinfo];
                }
            }
            return null;
        }

        public static Dictionary<PropertyInfo, BindingAttribute> GetBindingProperties(Type type)
        {
            PropertyInfo[] infos = type.GetProperties();
            if (infos == null || infos.Length == 0)
                return null;
            Dictionary<PropertyInfo, BindingAttribute> attinfos = new Dictionary<PropertyInfo, BindingAttribute>();
            foreach (PropertyInfo info in infos)
            {
                object[] atts = info.GetCustomAttributes(typeof(BindingAttribute), true);
                if (atts != null && atts.Length > 0)
                {
                    attinfos.Add(info, atts[0] as BindingAttribute);
                }
            }
            return attinfos;
        }
    }
}


