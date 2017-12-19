using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.BlockOper
{
    public class RasterCutProcesser : IRasterCut
    {
        public RasterCutProcesser()
        {

        }

        public IRasterDataProvider Cut(IRasterDataProvider srcRaster, BlockDefWithAOI blockDefsAoi, int samplePercent, string driver, string outdir, Action<int, string> progressCallback, params object[] options)
        {
            IRasterDataProvider tProviders;
            if (progressCallback != null)
                progressCallback(0, "开始数据裁切");
            if (blockDefsAoi == null)
                return null;
            //位置映射参数
            int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
            int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
            CoordEnvelope oEnvelope = srcRaster.CoordEnvelope;
            CoordEnvelope tEnvelope = blockDefsAoi.ToEnvelope();
            Size oSize = new Size(srcRaster.Width, srcRaster.Height);
            Size tSize = ClipCutHelper.GetTargetSize(blockDefsAoi, srcRaster.ResolutionX, srcRaster.ResolutionY);
            bool isInternal = new RasterMoasicClipHelper().ComputeBeginEndRowCol(oEnvelope, oSize, tEnvelope, tSize, ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
                                                                           ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);
            string blockFilename = ClipCutHelper.GetBlockFilename(blockDefsAoi, srcRaster.fileName, outdir,driver);
            tSize = new Size(oEndCol - oBeginCol, oEndRow - oBeginRow);
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
            int[] oAoi = ClipCutHelper.GetOffsetIndex(blockDefsAoi.AOIIndexes, oBeginRow, oBeginCol, srcRaster.Width, oWidth);
            int rowStep = ClipCutHelper.ComputeRowStep(srcRaster, oBeginRow, oEndRow);

            int stepCount = (int)((oEndRow - oBeginRow) / rowStep + 0.5) * srcRaster.BandCount;
            int step = 0;
            int percent = 0;

            for (int oRow = oBeginRow; oRow < oEndRow; oRow += rowStep)
            {
                if (oRow + rowStep > oEndRow)
                    rowStep = oEndRow - oRow;
                for (int bandIndex = 1; bandIndex <= srcRaster.BandCount; bandIndex++)
                {
                    step++;
                    percent = (int)(step * 1.0f / stepCount * 100);
                    if (progressCallback != null)
                        progressCallback(percent, "完成数据裁切" + percent + "%");

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
                            byte[] dataNewBf = ClipCutHelper.WriteValueToBuffer(databuffer, oAoi, typeSize);

                            fixed (byte* p = dataNewBf)
                            {
                                IntPtr newBuffer = new IntPtr(p);
                                if (samplePercent > 0 && samplePercent < 100)
                                {
                                    tProviders.GetRasterBand(bandIndex).Write((int)(tBeginCol * samplePercent * 1f / 100 + 0.5), (int)((tBeginRow + (oRow - oBeginRow)) * samplePercent * 1f / 100 + 0.5), (int)(sample * samplePercent * 1f / 100 + 0.5), (int)(rowStep * samplePercent * 1f / 100 + 0.5), newBuffer, srcRaster.DataType, (int)(sample * samplePercent * 1f / 100 + 0.5), (int)(rowStep * samplePercent * 1f / 100 + 0.5));
                                }
                                else
                                {
                                    tProviders.GetRasterBand(bandIndex).Write(tBeginCol, tBeginRow + (oRow - oBeginRow), sample, rowStep, newBuffer, srcRaster.DataType, sample, rowStep);
                                }
                            }
                        }
                    }
                }
            }
            return tProviders;
        }
    }
}
