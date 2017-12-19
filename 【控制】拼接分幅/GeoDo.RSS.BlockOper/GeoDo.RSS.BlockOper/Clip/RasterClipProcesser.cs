using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;

namespace GeoDo.RSS.BlockOper
{
    public class RasterClipProcesser:IRasterClip
    {
        public IRasterBand[] Clip(IRasterBand srcBand, BlockDef[] blockDefs, int samplePercent, string driver, Action<int, string> progressCallback, params object[] options)
        {
            if (blockDefs.Length == 0 || blockDefs == null || srcBand == null)
                return null;
            IRasterBand[] targetBands = new IRasterBand[blockDefs.Length];
            int blockNums = 0;
            foreach (BlockDef it in blockDefs)
            {
                int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
                int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
                CoordEnvelope oEnvelope = srcBand.CoordEnvelope;
                CoordEnvelope tEnvelope = it.ToEnvelope();
                Size oSize = new Size(srcBand.Width, srcBand.Height);
                Size tSize = ClipCutHelper.GetTargetSize(it, srcBand.ResolutionX, srcBand.ResolutionY);
                bool isInternal =new RasterMoasicClipHelper().ComputeBeginEndRowCol(oEnvelope, oSize, tEnvelope, tSize, ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
                                                                               ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);
                IRasterDataDriver rasterDriver = GeoDataDriver.GetDriverByName(driver) as IRasterDataDriver;
                //targetBands[blockNums] = ;
                blockNums++;
            }
            return targetBands;
        }

        public IRasterDataProvider[] Clip(IRasterDataProvider srcRaster, BlockDef[] blockDefs, int samplePercent, string driver, string outdir, Action<int, string> progressCallback, params object[] options)
        {
        IRasterDataProvider[] tProviders = new IRasterDataProvider[blockDefs.Length];
            if (progressCallback != null)
                progressCallback(0, "开始数据分幅");
            for(int blockNums=0;blockNums < blockDefs.Length;blockNums++)
            {
                if (progressCallback != null)
                    progressCallback((int)((blockNums + 1.0) / blockDefs.Length * 100), string.Format("正在分幅第{0}/{1}个文件...", (blockNums + 1).ToString(), blockDefs.Length));
                //位置映射参数
                int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
                int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
                CoordEnvelope oEnvelope = srcRaster.CoordEnvelope;
                CoordEnvelope tEnvelope = blockDefs[blockNums].ToEnvelope();
                Size oSize = new Size(srcRaster.Width, srcRaster.Height);
                Size tSize = ClipCutHelper.GetTargetSize(blockDefs[blockNums], srcRaster.ResolutionX, srcRaster.ResolutionY);
                bool isInternal = new RasterMoasicClipHelper().ComputeBeginEndRowCol(oEnvelope, oSize, tEnvelope, tSize, ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
                                                                               ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);
                string blockFilename = ClipCutHelper.GetBlockFilename(blockDefs[blockNums], srcRaster.fileName, outdir,driver);
                IRasterDataDriver rasterDriver = GeoDataDriver.GetDriverByName(driver) as IRasterDataDriver;
                int oWidth = 0;
                int oHeight = 0;
                float tResolutionX;
                float tResolutionY;
                if (samplePercent > 0 && samplePercent < 100)
                {
                    oHeight = (int)(tSize.Width * samplePercent * 1f / 100 + 0.5);
                    oWidth = (int)(tSize.Width * samplePercent * 1f / 100 + 0.5);
                    tResolutionX = srcRaster.ResolutionX * samplePercent * 1f / 100;
                    tResolutionY = srcRaster.ResolutionY * samplePercent * 1f / 100;
                }
                else
                {
                    oHeight = tSize.Height;
                    oWidth = tSize.Width;
                    tResolutionX = srcRaster.ResolutionX;
                    tResolutionY = srcRaster.ResolutionY;
                }
                string[] optionString = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
                "SPATIALREF=" + srcRaster.SpatialRef.ToProj4String(),
                "MAPINFO={" + 1 + "," + 1 + "}:{" + tEnvelope.MinX + "," + tEnvelope.MaxY + "}:{" + tResolutionX + "," + tResolutionY + "}"
                };
                tProviders[blockNums] = rasterDriver.Create(blockFilename, oWidth, oHeight, srcRaster.BandCount, srcRaster.DataType, optionString);
                int rowStep = ClipCutHelper.ComputeRowStep(srcRaster, oBeginRow, oEndRow);
                for (int oRow = oBeginRow; oRow < oEndRow; oRow += rowStep)
                {
                    if (oRow + rowStep > oEndRow)
                        rowStep = oEndRow - oRow;
                    for (int bandIndex = 1; bandIndex <= srcRaster.BandCount; bandIndex++)
                    {
                        int sample = (oEndCol - oBeginCol);
                        int typeSize = ClipCutHelper.GetSize(srcRaster.DataType);
                        int bufferSize = sample * rowStep * typeSize;
                        byte[] databuffer = new byte[bufferSize];
                        unsafe
                        {
                            fixed (byte* ptr = databuffer)
                            {
                                IntPtr buffer = new IntPtr(ptr);
                                srcRaster.GetRasterBand(bandIndex).Read(oBeginCol, oRow, sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                                
                                if (samplePercent > 0 && samplePercent < 100)
                                {
                                    tProviders[blockNums].GetRasterBand(bandIndex).Write((int)(tBeginCol * samplePercent * 1f / 100 + 0.5), (int)((tBeginRow + (oRow - oBeginRow)) * samplePercent * 1f / 100 + 0.5), (int)(sample * samplePercent * 1f / 100 + 0.5), (int)(rowStep * samplePercent * 1f / 100 + 0.5), buffer, srcRaster.DataType, (int)(sample * samplePercent * 1f / 100 + 0.5), (int)(rowStep * samplePercent * 1f / 100 + 0.5));
                                }
                                else
                                {
                                    tProviders[blockNums].GetRasterBand(bandIndex).Write(tBeginCol, tBeginRow + (oRow - oBeginRow), sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                                }
                            }
                        }
                    }
                }
                
            }
            return tProviders;
        }

        public IRasterDataProvider Clip(IRasterDataProvider srcRaster, BlockDef blockDefs, int samplePercent, string driver, string outdir, Action<int, string> progressCallback ,out double validPercent, params object[] options)
        {
            IRasterDataProvider tProviders = null;
            if (progressCallback != null)
                progressCallback(0, "开始数据分幅");
            //位置映射参数
            int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
            int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
            CoordEnvelope oEnvelope = srcRaster.CoordEnvelope;
            CoordEnvelope tEnvelope = blockDefs.ToEnvelope();
            Size oSize = new Size(srcRaster.Width, srcRaster.Height);
            Size tSize = ClipCutHelper.GetTargetSize(blockDefs, srcRaster.ResolutionX, srcRaster.ResolutionY);
            bool isInternal = new RasterMoasicClipHelper().ComputeBeginEndRowCol(oEnvelope, oSize, tEnvelope, tSize, ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
                                                                           ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);
            string blockFilename = ClipCutHelper.GetBlockFilename(blockDefs, srcRaster.fileName, outdir,driver);
            IRasterDataDriver rasterDriver = GeoDataDriver.GetDriverByName(driver) as IRasterDataDriver;
            int oWidth = 0;
            int oHeight = 0;
            float tResolutionX;
            float tResolutionY;
            if (samplePercent > 0 && samplePercent < 100)
            {
                oHeight = (int)(tSize.Width * samplePercent * 1f / 100 + 0.5);
                oWidth = (int)(tSize.Width * samplePercent * 1f / 100 + 0.5);
                tResolutionX = srcRaster.ResolutionX * samplePercent * 1f / 100;
                tResolutionY = srcRaster.ResolutionY * samplePercent * 1f / 100;
            }
            else
            {
                oHeight = tSize.Height;
                oWidth = tSize.Width;
                tResolutionX = srcRaster.ResolutionX;
                tResolutionY = srcRaster.ResolutionY;
            }
            string[] optionString = new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
                "SPATIALREF=" + srcRaster.SpatialRef.ToProj4String(),
                "MAPINFO={" + 1 + "," + 1 + "}:{" + tEnvelope.MinX + "," + tEnvelope.MaxY + "}:{" + tResolutionX + "," + tResolutionY + "}"
                };
            tProviders = rasterDriver.Create(blockFilename, oWidth, oHeight, srcRaster.BandCount, srcRaster.DataType, optionString);
            int rowStep = ClipCutHelper.ComputeRowStep(srcRaster, oBeginRow, oEndRow);
            int sample = (oEndCol - oBeginCol);
            int typeSize = ClipCutHelper.GetSize(srcRaster.DataType);
            
            long allPixelByte = sample * (oEndRow - oBeginRow) * typeSize * srcRaster.BandCount;
            long validPixelByte = 0;
            long validPer = (int)(allPixelByte * 0.1f);
            int stepCount = (int)((oEndRow - oBeginRow) / rowStep + 0.5) * srcRaster.BandCount;
            int step = 0;
            int percent=0;
            for (int oRow = oBeginRow; oRow < oEndRow; oRow += rowStep)
            {
                if (oRow + rowStep > oEndRow)
                    rowStep = oEndRow - oRow;
                for (int bandIndex = 1; bandIndex <= srcRaster.BandCount; bandIndex++)
                {
                    step++;
                    percent = (int)(step * 1.0f / stepCount*100);
                    if (progressCallback != null)
                        progressCallback(percent, "完成数据分幅" + percent + "%");
                    int bufferSize = sample * rowStep * typeSize;
                    byte[] databuffer = new byte[bufferSize];
                    unsafe
                    {
                        fixed (byte* ptr = databuffer)
                        {
                            IntPtr buffer = new IntPtr(ptr);
                            srcRaster.GetRasterBand(bandIndex).Read(oBeginCol, oRow, sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                            if (validPixelByte < validPer)
                            {
                                foreach (byte b in databuffer)
                                {
                                    if (b != 0)
                                        validPixelByte++;
                                }
                            }
                            if (validPixelByte == 0)
                                continue;
                            if (samplePercent > 0 && samplePercent < 100)
                            {
                                tProviders.GetRasterBand(bandIndex).Write((int)(tBeginCol * samplePercent * 1f / 100 + 0.5), (int)((tBeginRow + (oRow - oBeginRow)) * samplePercent * 1f / 100 + 0.5), (int)(sample * samplePercent * 1f / 100 + 0.5), (int)(rowStep * samplePercent * 1f / 100 + 0.5), buffer, srcRaster.DataType, (int)(sample * samplePercent * 1f / 100 + 0.5), (int)(rowStep * samplePercent * 1f / 100 + 0.5));
                            }
                            else
                            {
                                tProviders.GetRasterBand(bandIndex).Write(tBeginCol, tBeginRow + (oRow - oBeginRow), sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                            }
                        }
                    }
                }
            }
            validPercent = validPixelByte * 1.0d / allPixelByte;
            if (progressCallback != null)
                progressCallback(100, "完成数据分幅");
            return tProviders;
        }
    }
}
