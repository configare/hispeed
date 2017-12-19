using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using GeoDo.Project;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.UI.AddIn.TouMImportSmart;
using GeoDo.RSS.UI.AddIn.Theme;

namespace GeoDo.RSS.MIF.Prds.FOG
{
    public class SubProductTOUFOG : CmaMonitoringSubProduct
    {
        private Action<int, string> _progressTracker = null;
        private IContextMessage _contextMessage = null;

        public SubProductTOUFOG(SubProductDef subProductDef)
            : base(subProductDef)
        {

        }

        public override IExtractResult Make(Action<int, string> progressTracker)
        {
            return Make(progressTracker, null);
        }

        public override IExtractResult Make(Action<int, string> progressTracker, IContextMessage contextMessage)
        {
            _progressTracker = progressTracker;
            _contextMessage = contextMessage;
            if (_argumentProvider == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName") == null)
                return null;
            if (_argumentProvider.GetArg("AlgorithmName").ToString() == "0TOUAlgorithm")
            {
                return TOUAlgorithm();
            }
            return null;
        }

        private IExtractResult TOUAlgorithm()
        {
            float lonMin = float.Parse(_argumentProvider.GetArg("LonMin").ToString());
            float lonMax = float.Parse(_argumentProvider.GetArg("LonMax").ToString());
            float latMin = float.Parse(_argumentProvider.GetArg("LatMin").ToString());
            float latMax = float.Parse(_argumentProvider.GetArg("LatMax").ToString());
            float invaild = float.Parse(_argumentProvider.GetArg("Invaild").ToString());
            float zoom = (float)_argumentProvider.GetArg("Zoom");
            int width = (int)_argumentProvider.GetArg("Width");
            int height = (int)_argumentProvider.GetArg("Height");
            string touFile = Obj2String(_argumentProvider.GetArg("TOUTxt"));
            float outlonMin = float.Parse(_argumentProvider.GetArg("OutLonMin").ToString());
            float outlonMax = float.Parse(_argumentProvider.GetArg("OutLonMax").ToString());
            float outlatMin = float.Parse(_argumentProvider.GetArg("OutLatMin").ToString());
            float outlatMax = float.Parse(_argumentProvider.GetArg("OutLatMax").ToString());
            bool isChina = (bool)_argumentProvider.GetArg("isChina");
            string outFilename = MifEnvironment.GetFullFileName(Path.GetFileName(touFile));
            string chinaMask = string.Empty;
            if (isChina)
                chinaMask = AppDomain.CurrentDomain.BaseDirectory + "\\SystemData\\ProductArgs\\FOG\\TOUChinaMask\\china_mask.txt";
            TouProcessor.ProcessTouFile(touFile, ref outFilename, chinaMask);
            touFile = outFilename;
            if (string.IsNullOrEmpty(touFile))
                return null;
            FY3TouImportSMART import = new FY3TouImportSMART(new Size(width, height), new CoordEnvelope(lonMin, lonMax, latMin, latMax), zoom, invaild, _progressTracker);
            string error;
            string dstFilename = GetDstFilename(new RasterIdentify(touFile).OrbitDateTime, "FY3A", "VIRR", 0.5f);
            if (!import.ConvertTextToDat(touFile, dstFilename, new CoordEnvelope(outlonMin, outlonMax, outlatMin, outlatMax), out error))
            {
                if (_contextMessage != null && !string.IsNullOrEmpty(error))
                    _contextMessage.PrintMessage(_error);
                return null;
            }
            
            if (File.Exists(dstFilename))
            {
                return  new FileExtractResult("0TOU", dstFilename, true); 
            }

            else
                return null;

        }

        private string GetDstFilename(DateTime orbitTime, string satellite, string sensor, float resoultion)
        {
            RasterIdentify id = CreatRasterIndetifyId(orbitTime, satellite, sensor, resoultion);
            return id.ToWksFullFileName(".dat");
        }

        private RasterIdentify CreatRasterIndetifyId(DateTime orbitTime, string satellite, string sensor, float resoultion)
        {
            RasterIdentify id = new RasterIdentify();
            id.ThemeIdentify = "CMA";
            id.ProductIdentify = "FOG";
            id.SubProductIdentify = "0TOU";
            id.Satellite = satellite;
            id.Sensor = sensor;
            float dstResolution = resoultion * 100000;
            if (dstResolution > 9999)
                id.Resolution = (int)(dstResolution / 1000) + "KM";
            else
                id.Resolution = dstResolution + "M";
            id.OrbitDateTime = orbitTime;
            id.GenerateDateTime = DateTime.Now;
            return id;
        }

        private string Obj2String(object argument)
        {
            if (argument == null)
                return null;
            return argument.ToString();
        }
    }
}
