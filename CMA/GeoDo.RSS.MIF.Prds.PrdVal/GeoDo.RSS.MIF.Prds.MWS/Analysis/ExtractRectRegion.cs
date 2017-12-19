using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.MIF.Prds.Comm;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.Core.DF;
using System.IO;
using System.Drawing;
using GeoDo.RSS.BlockOper;
using CodeCell.AgileMap.Core;
using System.Windows.Forms;
using GeoDo.RSS.DF.MEM;
using GeoDo.RSS.Core.UI;
using GeoDo.RSS.Core.DrawEngine;
using GeoDo.RasterProject;

namespace GeoDo.RSS.MIF.Prds.MWS
{
  public  class ExtractRectRegion
    {
        private IAOIContainerLayer _aoiContainer = null;
        GeoDo.RasterProject.PrjEnvelope evp = new GeoDo.RasterProject.PrjEnvelope();
        private GeoDo.RSS.Core.DF.CoordEnvelope _fileEnvelope = null;
        private Size _fileSize = Size.Empty;
        private double _resolutionX = 0;
        private double _resolutionY = 0;
        private Project.ISpatialReference _activeSpatialRef;
        private enumCoordType _activeCoordType;
        private string _blockName = "DXX";
        private string OutDir;
        private BlockDefWithAOI[] envelopes;
        public string ExractRect(string file, GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer, string savePath, string regionName)
        {
            _aoiContainer = aoiContainer;
            using (IRasterDataProvider raster = GeoDataDriver.Open(file) as IRasterDataProvider)
            {
                _resolutionX = raster.ResolutionX;
                _resolutionY = raster.ResolutionY;
                _fileSize = new Size(raster.Width, raster.Height);
                _fileEnvelope = raster.CoordEnvelope;
                _activeSpatialRef = raster.SpatialRef;
                _activeCoordType = raster.CoordType;
                raster.Dispose();
            }
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope corEnvelope = null;
            corEnvelope = GetGeoRect();  //获取多个aoi的外接矩形
            evp.MaxX = corEnvelope.MaxX + 2.0f;
            evp.MaxY = corEnvelope.MaxY + 2.0f;
            evp.MinX = corEnvelope.MinX - 2.0f;
            evp.MinY = corEnvelope.MinY - 2.0f;
            string filename = ExtractFile(file,0, evp, regionName, savePath);
            return filename;
        }
        //获取多个aoi的外接矩形
        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope GetGeoRect()
        {
            GeoDo.RSS.Core.DrawEngine.CoordEnvelope retRect = null;
            foreach (object obj in _aoiContainer.AOIs)
            {
                string name;
                GeoDo.RSS.Core.DrawEngine.CoordEnvelope rect = null;
                rect = GetGeoRect(obj as Feature, out name);
                if (rect == null)
                    continue;
                if (retRect == null)
                    retRect = rect;
                else
                    retRect = retRect.Union(rect);
            }
            return retRect;
        }
        private GeoDo.RSS.Core.DrawEngine.CoordEnvelope GetGeoRect(Feature feature, out string name)
        {
            name = string.Empty;
            if (feature.Geometry == null)
                return null;
            Envelope evp = feature.Geometry.Envelope.Clone() as Envelope;
            if (evp == null)
                return null;
            return new GeoDo.RSS.Core.DrawEngine.CoordEnvelope(evp.MinX, evp.MaxX, evp.MinY, evp.MaxY);
        }
        private string ExtractFile(string filename, int bandIndex, PrjEnvelope env, string regionNam, string outdir)
        {
            string projectionType = null;
            string dstFileName = null;
            IRasterDataProvider prd = GeoDataDriver.Open(filename) as IRasterDataProvider;
            double dminx = env.MinX, dmax = env.MaxX, dminy = env.MinY, dmaxy = env.MaxY;
            int xoffset = (int)(Math.Round((dminx - prd.CoordEnvelope.MinX) / prd.ResolutionX));
            int yoffset = (int)(Math.Round((prd.CoordEnvelope.MaxY - dmaxy) / prd.ResolutionY));
            if (xoffset < 0)
                xoffset = 0;
            if (yoffset < 0)
                yoffset = 0;
            //int yoffset = (int)(Math.Round(dminy - prd.CoordEnvelope.MinY) / prd.ResolutionY);
            int width = (int)(Math.Round((env.MaxX - env.MinX) / prd.ResolutionX));
            int height = (int)(Math.Round((env.MaxY - env.MinY) / prd.ResolutionY));
            if (xoffset < 0 || yoffset < 0 || width <= 0 || height <= 0)
            {
                MessageBox.Show("文件：" + filename + "小于指定的提取范围");
                return null;
            }
            float tResolutionX;
            float tResolutionY;
            tResolutionX = Convert.ToSingle(prd.ResolutionX);
            tResolutionY = Convert.ToSingle(prd.ResolutionY);
            string[] optionString = new string[]{
            "INTERLEAVE=BSQ",
            "VERSION=LDF",
            "WITHHDR=TRUE",
            "SPATIALREF="+ projectionType,
            "MAPINFO={" + 1 + "," + 1 + "}:{" + dminx + "," + dmaxy+ "}:{" + tResolutionX + "," + tResolutionY + "}"
            };
            enumDataType dataType = prd.DataType;
            if (!Directory.Exists(outdir))
                Directory.CreateDirectory(outdir);
            dstFileName = Path.Combine(outdir, Path.GetFileName(filename).Replace("China", regionNam));// +"\\" + Path.GetFileNameWithoutExtension(filename) + "_" + regionNam + ".dat";
            if (File.Exists(dstFileName))
            {
                return dstFileName;
            }
            IRasterDataDriver dataDriver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            IRasterDataProvider dstRaster = dataDriver.Create(dstFileName, width, height, 1, dataType, optionString);
            float[] buffer = new float[width * height];
            unsafe
            {
                fixed (float* ptr = buffer)
                {
                    IntPtr bufferPtr = new IntPtr(ptr);
                    prd.GetRasterBand(bandIndex + 1).Read(xoffset, yoffset, width, height, bufferPtr, dataType, width, height);
                    fixed (float* wptr = buffer)
                    {
                        IntPtr newBuffer = new IntPtr(wptr);
                        dstRaster.GetRasterBand(1).Write(0, 0, width, height, newBuffer, dataType, width, height);
                    }
                }
            }
            if (prd != null)
                prd.Dispose();
            if (dstRaster != null)
                dstRaster.Dispose();
            dataDriver.Dispose();
            return dstFileName;
        }
    }
}
