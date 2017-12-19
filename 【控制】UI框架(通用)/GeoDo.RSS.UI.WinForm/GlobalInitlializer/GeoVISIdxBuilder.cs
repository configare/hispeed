using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Linq;
using System.Reflection;
using System.Configuration;
using CodeCell.Bricks.Runtime;

namespace GeoDo.RSS.UI.WinForm
{
    public static class GeoVISIdxBuilder
    {
        public static void BuildGeoVISIdx()
        {
            string builderNameArray = ConfigurationManager.AppSettings["GeoVISIdxBuilder"];
            if (string.IsNullOrEmpty(builderNameArray))
                return;
            try
            {
                string[] parts = builderNameArray.Split(':');
                if (parts == null || parts.Length < 2)
                    return;
                if (!parts[0].ToUpper().EndsWith(".DLL"))
                    return;
                string dllPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, parts[0]);
                if (!File.Exists(dllPath))
                    return;
                Assembly assem = Assembly.LoadFrom(parts[0]);
                if (assem == null)
                    return;
                if (string.IsNullOrEmpty(parts[1]))
                    return;
                assem.CreateInstance(parts[1]);
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
            }
        }
    }
}
