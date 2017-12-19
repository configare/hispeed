#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-3-26 14:32:25
* ------------------------------------------------------------------------
* 变更记录：
* 时间：                 修改者：                
* 修改说明：
* 
* ------------------------------------------------------------------------
* ========================================================================
*/
#endregion

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

namespace GeoDo.RSS.MIF.Prds.MWS
{
    /// <summary>
    /// 类名：Mul_RegionsClip
    /// 属性描述：支持多区域裁切
    /// 创建者：lxj   创建日期：2014-3-26 14:32:25
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
   public class MulRegionsClip
    {   
        private IAOIContainerLayer _aoiContainer = null;
        Envelope evp = new Envelope();
        private GeoDo.RSS.Core.DF.CoordEnvelope _fileEnvelope = null;
        private Size _fileSize = Size.Empty;
        private double _resolutionX = 0;
        private double _resolutionY = 0;
        private Project.ISpatialReference _activeSpatialRef;
        private enumCoordType _activeCoordType;
        private string _blockName = "DXX";
        private string OutDir;
        private BlockDefWithAOI[] envelopes;
        public string MutiRegionsClip(string file, GeoDo.RSS.Core.VectorDrawing.AOIContainerLayer aoiContainer, string savePath )
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
                evp.MaxX = corEnvelope.MaxX;
                evp.MaxY = corEnvelope.MaxY;
                evp.MinX = corEnvelope.MinX;
                evp.MinY = corEnvelope.MinY;
                BlockDefWithAOI outEnvelope;
                Size size;
                List<BlockDefWithAOI> blockList = new List<BlockDefWithAOI>();
                GetArgs(out outEnvelope, out size);
                envelopes = new BlockDefWithAOI[] { outEnvelope };
                blockList.AddRange(envelopes);
                blockList[0].AOIIndexes = GetIndexes(); //获得多个aoi的Index
                OutDir = savePath;
                string[] put = RasterClipT(file, blockList.ToArray(), OutDir, "Cut");
                return put[0];
        }
       
        public string[] RasterClipT(string infName, BlockDefWithAOI[] blocks, string outDir, string type)
        {
            IRasterDataProvider prd = GeoDataDriver.Open(infName) as IRasterDataProvider;
            try
            {
                if (type.Equals("Cut"))
                {
                     GeoDo.RSS.BlockOper.RasterCutProcesser cut = new  GeoDo.RSS.BlockOper.RasterCutProcesser();
                    IRasterDataProvider result = null;
                    if (prd is MemoryRasterDataProvider)
                        result = cut.Cut(prd, blocks[0], 100, "MEM", outDir,
                        new Action<int, string>((int progerss, string text) =>
                        {
                        }));
                    else
                        result = cut.Cut(prd, blocks[0], 100, "LDF", outDir,
                       new Action<int, string>((int progerss, string text) =>
                       {
                       }));
                    List<string> files = new List<string>();
                    if (result != null)
                    {
                        files.Add(result.fileName);
                        result.Dispose();
                    }
                    if (prd != null)
                        prd.Dispose();
                    return files.ToArray();
                }
                else
                    return null;
            }
            finally
            {

            }
        }
        private int[] GetIndexes()
        {
            int[] retAOI = null;
            foreach (object obj in _aoiContainer.AOIs)
            {
                int[] aoi = null;
                    aoi = GetIndexes(obj as Feature);
                if (aoi == null)
                    continue;
                if (retAOI == null)
                    retAOI = aoi;
                else
                    retAOI = AOIHelper.Merge(new int[][] { retAOI, aoi });
            }
            return retAOI;
        }
        private int[] GetIndexes(Feature feature)
        {
            VectorAOIGenerator vg = new VectorAOIGenerator();
            int[] aoi = null;
            aoi = vg.GetAOI(new ShapePolygon[] { feature.Geometry as ShapePolygon },
                            new Envelope(_fileEnvelope.MinX, _fileEnvelope.MinY, _fileEnvelope.MaxX, _fileEnvelope.MaxY),
                            _fileSize);
            return aoi;
        }
        private void GetArgs(out BlockDefWithAOI envelope, out Size size)//, out string dir, out string inputFilename)
        {
            envelope = new BlockDefWithAOI(_blockName, evp.MinX, evp.MinY, evp.MaxX, evp.MaxY);
            Size aoiSize = new Size((int)GetInteger(((evp.MaxX - evp.MinX) / _resolutionX), _resolutionX),
                 (int)GetInteger(((evp.MaxY - evp.MinY) / _resolutionY), _resolutionY));
            size = new Size((int)aoiSize.Width, aoiSize.Height);
        }
        protected int GetInteger(double fWidth, double res)
        {
            int v = (int)Math.Round(fWidth);
            if (fWidth - v > res)
                v++;
            return v;
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


        //创建输出删格文件
        protected IRasterDataProvider CreateOutRaster(string outFileName, enumDataType dataType, RasterMaper[] inrasterMaper, float resolution)
        {
            IRasterDataDriver raster = RasterDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
            GeoDo.RSS.Core.DF.CoordEnvelope outEnv = null;
            foreach (RasterMaper inRaster in inrasterMaper)
            {
                if (outEnv == null)
                    outEnv = inRaster.Raster.CoordEnvelope;
                else
                    outEnv = outEnv.Intersect(inRaster.Raster.CoordEnvelope);
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
            if (File.Exists(outFileName))
                File.Delete(outFileName);
            RasterDataProvider outRaster = raster.Create(outFileName, width, height, 1, dataType, mapInfo) as RasterDataProvider;
            return outRaster;
        }
    }
}
