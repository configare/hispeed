using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.WebDataServer
{
    internal static class ServerInstanceFactory
    {
        public static IServerInstance GetServerInstance(InstanceIdentify instanceIdentify, string args)
        {
            if (args == null)
                return null;
            string argc = args.ToUpper();
            ICatalogProvider provider = null;
            if (argc.Contains("$") && argc.Contains("DATA SOURCE") && argc.Contains("USER ID") && !argc.Contains(":"))
            {
                provider = new CatalogProviderSpatialDb(args);
                return new ServerInstanceForDb(instanceIdentify, provider,args);
            }
            else if (argc.Contains(":"))
            {
                provider = new CatalogProviderFile(args);
                return new ServerInstanceForFile(instanceIdentify, provider,args);
            }
            else
            {
                throw new Exception("指定的实例构造参数\""+args+"\"非支持的有效格式。");
            }
        }
    }
}
