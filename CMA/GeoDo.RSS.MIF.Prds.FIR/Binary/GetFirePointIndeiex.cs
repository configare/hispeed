using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.FIR
{
    public class GetFirePointIndeiex
    {
        private static string GetFireIndexiexFilename(IArgumentProvider argProvider)
        {
            RasterIdentify id = new RasterIdentify(argProvider.DataProvider.fileName);
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FIR";
            id.SubProductIdentify = "DBLV";
            id.IsOutput2WorkspaceDir = false;
            DataIdentify dataId = argProvider.DataProvider.DataIdentify;
            if (dataId == null)
                dataId = new DataIdentify();
            id.Satellite = dataId.Satellite ?? "Unknow";
            id.Sensor = dataId.Sensor ?? "Unknow";
            id.OrbitDateTime = dataId.OrbitDateTime;
            string result = id.ToWksFullFileName(".dat");
            return Path.Combine(Path.GetDirectoryName(result), Path.GetFileNameWithoutExtension(result) + ".fiif");
        }

        public static string GetFireIndexiexFilename(string dblvFilename)
        {
            string result = Path.Combine(Path.GetDirectoryName(dblvFilename), Path.GetFileNameWithoutExtension(dblvFilename) + ".fiif");
            if (File.Exists(result))
                return result;
            return string.Empty;
        }

        public static bool WriteFireIndexiexFilename(IArgumentProvider argProvider, int[] indexiex)
        {
            string filenanme = GetFireIndexiexFilename(argProvider);
            int length = indexiex.Length;
            List<string> fileContent = new List<string>();
            for (int i = 0; i < length; i++)
                fileContent.Add(indexiex[i].ToString());
            File.WriteAllLines(filenanme, fileContent.ToArray(), Encoding.Default);
            return true;
        }

        public static bool ReadFireIndexiexFilename(IArgumentProvider argProvider, out int[] indexiex)
        {
            indexiex = null;
            string filenanme = GetFireIndexiexFilename(argProvider);
            if (!File.Exists(filenanme))
                return false;
            string[] contents = File.ReadAllLines(filenanme, Encoding.Default);
            if (contents == null || contents.Length == 0)
                return false;
            int length = contents.Length;
            List<int> fileContent = new List<int>();
            int indextemp = -1;
            for (int i = 0; i < length; i++)
            {
                if (int.TryParse(contents[i], out indextemp))
                    fileContent.Add(indextemp);
            }
            indexiex = fileContent.Count == 0 ? null : fileContent.ToArray();
            return true;
        }
    }
}
