using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.MIF.Prds.VGT
{
    public class CmaVgtMonitoringSubProduct : CmaMonitoringSubProduct
    {
        public CmaVgtMonitoringSubProduct()
            : base()
        { }

        public CmaVgtMonitoringSubProduct(SubProductDef subProductDef)
            : base(subProductDef)
        { }

        protected IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX = inrasterMaper[0].Raster.ResolutionX;
            float resY = inrasterMaper[0].Raster.ResolutionY;
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        protected IRasterDataProvider CreateOutRaster(string outFileName, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Union(inRaster.Raster.CoordEnvelope);
            }
            float resX, resY;
            if (resolution != 0f)
            {
                resX = resolution;
                resY = resolution;
            }
            else
            {
                resX = inrasterMaper[0].Raster.ResolutionX;
                resY = inrasterMaper[0].Raster.ResolutionY;
            }
            int width = (int)(Math.Round(outEnv.Width / resX));
            int height = (int)(Math.Round(outEnv.Height / resY));
            string mapInfo = outEnv.ToMapInfoString(new Size(width, height));
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, enumDataType.Int16, mapInfo) as RasterDataProvider;
            return outRaster;
        }

        protected short[] GetNanValues(string argumentName)
        {
            string nanValuestring = _argumentProvider.GetArg(argumentName) as string;
            if (!string.IsNullOrEmpty(nanValuestring))
            {
                string[] valueStrings = nanValuestring.Split(new char[] { ',', '，' });
                if (valueStrings != null && valueStrings.Length > 0)
                {
                    List<short> values = new List<short>();
                    short value;
                    for (int i = 0; i < valueStrings.Length; i++)
                    {
                        if (Int16.TryParse(valueStrings[i], out value))
                            values.Add(value);
                    }
                    if (values.Count > 0)
                    {
                        return values.ToArray();
                    }
                }
            }
            return null;
        }

        protected RasterIdentify GetRasterIdentifyID(string[] fileNames)
        {
            RasterIdentify rst = new RasterIdentify(fileNames);
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = _subProductDef.Identify;

            object obj = _argumentProvider.GetArg("OutFileIdentify");
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
                rst.SubProductIdentify = obj.ToString();

            rst.IsOutput2WorkspaceDir = true;
            return rst;
        }

        protected string FindCloudExtractResult(string inputFileName)
        {
            string dirName = Path.Combine(MifEnvironment.GetWorkspaceDir(), "VGT");
            RasterIdentify rstIdentify = GetRasterIdentify(inputFileName);
            string fname = InterestedRaster<Int16>.GetWorkspaceFileName(rstIdentify);
            string[] cloudFiles = Directory.GetFiles(dirName, Path.GetFileName(fname), SearchOption.AllDirectories);
            if (cloudFiles != null && cloudFiles.Length > 0)
                return cloudFiles[0];
            else
                return null;
        }

        protected RasterIdentify GetRasterIdentify(string inputFileName)
        {
            RasterIdentify rst = new RasterIdentify(inputFileName);
            rst.ThemeIdentify = "CMA";
            rst.ProductIdentify = _subProductDef.ProductDef.Identify;
            rst.SubProductIdentify = "0CLM";
            return rst;
        }

        protected Dictionary<string, string> Obj2Dic(object dicObj)
        {
            if (dicObj == null || string.IsNullOrEmpty(dicObj.ToString()))
                return null;
            return dicObj as Dictionary<string, string>;
        }

        protected int Obj2int(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return -1;
            int value = -1;
            if (int.TryParse(obj.ToString(), out value))
                return value;
            return -1;
        }
    }
}
