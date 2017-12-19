using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public static class HttpCommands
    {
        public const string cstCatalogPage = "VectorMapCatalog.ashx";
        public const string cstServerInstanceCnfgFile = "VectorServerInstance.cnfg";
        public const string cstArgNameCommand ="cmd";
        public const string cstArgCmdGetInstanceList = "GetInstanceList";
        public const string cstArgCmdGetCatalogList = "GetCatalogList";
        public const string cstArgCmdGetFetClassProperty = "GetFetClassProperty";
        public const string cstArgCmdReadFeatures = "ReadFeatures";
        public const string cstArgCmdBeginRead = "BeginRead";
        public const string cstArgCmdEndRead = "EndRead";
        public const string cstArgMinX = "minx";
        public const string cstArgMaxX = "maxx";
        public const string cstArgMinY = "miny";
        public const string cstArgMaxY = "maxy";
        public const string cstArgInstanceId = "instance";
        public const string cstArgFetClassId = "fetclassid";
        public const string cstErrorCommandInvalid = "不支持指定的命令{0}。";
        public const string cstErrorCommandIsEmpty = "命令为空。";
        public const string cstErrorInstanceIsNotExisted = "请求访问的实例{0}不存在。";
        public const string cstErrorCatalogProviderNotExistdOfInstance = "请求的实例{0}不存在目录提供者,或构造目录提供者失败。";
        public const string cstErrorArgInstanceIdIsMissing = "参数InstanceId丢失。";

        public static string GetCatalogUrlPage(string serverUrl, string pageUrl)
        {
            if (!serverUrl.EndsWith("/"))
                return serverUrl + "/" + pageUrl;
            return serverUrl + pageUrl;
        }

        public static string GetCatalogUrl(string serverUrl,Dictionary<string, string> argNameAndValues)
        {
            if (serverUrl == null)
                throw new ArgumentNullException("服务器URL为空。");
            if (argNameAndValues == null || argNameAndValues.Count == 0)
                return cstCatalogPage;
            StringBuilder sb = new StringBuilder(serverUrl);
            int i =0;
            foreach (string key in argNameAndValues.Keys)
            {
                if (i == 0)
                    sb.Append("?");
                sb.Append(key);
                sb.Append("=");
                sb.Append(argNameAndValues[key]);
                if (i != argNameAndValues.Count - 1)
                    sb.Append("&");
                i++;
            }
            return sb.ToString();
        }
    }
}
