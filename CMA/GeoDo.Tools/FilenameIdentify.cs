using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.RasterProject;
using GeoDo.FileProject;
using System.IO;

namespace GeoDo.Tools
{
    public class FilenameIdentify
    {
        public DataIdentify DataIdentify;
        public string Level;
        public string ProjectionIdentify;
        public string Station;
        public string DayOrNight;
        public string OrbitIdentify;

        public void GetDataIdentify(IRasterDataProvider raster)
        {
            DataIdentify = raster.DataIdentify;
            Level = "L1";
            //ProjectionIdentify = "GLL";
            Station = ParseStation(raster.fileName);
            //DayOrNight = "D";
            //OrbitIdentify = "L1";
        }

        private bool ValidEnvelope(IRasterDataProvider inputRaster, PrjEnvelopeItem[] validEnvelopes, out string msg)
        {
            bool hasValid = false;
            StringBuilder str = new StringBuilder();
            PrjEnvelope fileEnv = ProjectionFactory.GetEnvelope(inputRaster);
            foreach (PrjEnvelopeItem validEnvelope in validEnvelopes)
            {
                hasValid = ProjectionFactory.HasInvildEnvelope(inputRaster, validEnvelope.PrjEnvelope);
                if (!hasValid)
                    str.AppendLine("数据不在范围内：" + validEnvelope.Name + validEnvelope.PrjEnvelope.ToString());
            }
            msg = str.ToString();
            return hasValid;
        }

        private string ParseStation(string filename)
        {
            string[] stations = new string[] { "BJ", "GZ", "XJ", "XZ", "JM", "KS", "SW", "MS" };
            foreach (string station in stations)
            {
                if (filename.Contains(station))
                    return station;
            }
            return "XX";
        }

        public static string OverviewFileName(string filename)
        {
            return Path.ChangeExtension(filename, "overview.png");
        }

        public static string HdrFileName(string filename)
        {
            return Path.ChangeExtension(filename, ".hdr");
        }

        public static string OutPutXmlFilename(string outputDir, string inputFilename)
        {
            return Path.Combine(outputDir, Path.GetFileName(inputFilename) + ".xml");
        }
    }
}
