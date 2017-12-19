using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RasterProject;
using GeoDo.Project;
using GeoDo.RSS.DF.LDF;
using GeoDo.RSS.BlockOper;

namespace GeoDo.Tools.Projection
{
    public class Mosaic
    {
        private InputArg inArg;
        private RSS.Core.DF.IRasterDataProvider fileRaster = null;
        private Action<int, string> action;

        public Mosaic(InputArg inArg, RSS.Core.DF.IRasterDataProvider infileRaster,Action<int, string> action)
        {
            this.inArg = inArg;
            this.fileRaster = infileRaster;
            this.action = action;
        }

        internal IRasterDataProvider MosaicToFile(string mosaicFilename)
        {
            IRasterDataProvider mosaicFileRaster = null;
            try
            {
                if (File.Exists(mosaicFilename))
                {
                    mosaicFileRaster = RasterDataDriver.Open(mosaicFilename, enumDataProviderAccess.Update, null) as IRasterDataProvider;
                    if (mosaicFileRaster.BandCount != fileRaster.BandCount)
                    {
                        mosaicFileRaster.Dispose();
                        mosaicFileRaster = CreateMosaicFile(mosaicFilename, inArg);
                    }
                }
                else
                {
                    mosaicFileRaster = CreateMosaicFile(mosaicFilename, inArg);
                }
                if (mosaicFileRaster == null)
                    return null;
                RasterMoasicProcesser mo = new RasterMoasicProcesser();
                //mo.Moasic(new IRasterDataProvider[] { fileRaster }, mosaicFileRaster, true, new string[] { "0" }, action);
                mo.MoasicSimple(fileRaster, mosaicFileRaster, true, new string[] { "0" }, action);
                return mosaicFileRaster;
            }
            finally
            {
            }
        }

        private IRasterDataProvider CreateMosaicFile(string mosaicFilename, InputArg inArg)
        {
            ISpatialReference spatialRef = fileRaster.SpatialRef;
            PrjEnvelope env = inArg.MosaicInputArg.Envelope.PrjEnvelope;
            string bandNames = BandNameString(fileRaster as ILdfDataProvider);
            Size outSize = env.GetSize(fileRaster.ResolutionX, fileRaster.ResolutionY);
            string[] options = new string[]{
                            "INTERLEAVE=BSQ",
                            "VERSION=LDF",
                            "WITHHDR=TRUE",
                            "SPATIALREF=" + spatialRef.ToProj4String(),
                            "MAPINFO={" + 1 + "," + 1 + "}:{" + env.MinX + "," + env.MaxY + "}:{" + fileRaster.ResolutionX + "," + fileRaster.ResolutionY + "}",
                            "BANDNAMES="+ bandNames
                        };
            return CreateOutFile(mosaicFilename, fileRaster.BandCount, outSize, enumDataType.UInt16, options);
        }

        private string BandNameString(ILdfDataProvider fileRaster)
        {
            if (fileRaster == null)
                return null;
            string[] bandNames = (fileRaster as ILdfDataProvider).Hdr.BandNames;
            if (bandNames == null || bandNames.Length == 0)
                return null;
            string bandNameString = "";
            foreach (string b in bandNames)
            {
                bandNameString = bandNameString + b + ",";
            }
            return bandNameString.TrimEnd(',');
        }

        internal IRasterDataProvider CreateOutFile(string outfilename, int dstBandCount, Size outSize, enumDataType dataType, string[] options)
        {
            string dir = Path.GetDirectoryName(outfilename);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            IRasterDataDriver outdrv = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
            return outdrv.Create(outfilename, outSize.Width, outSize.Height, dstBandCount,dataType, options) as IRasterDataProvider;
        }
    }
}
