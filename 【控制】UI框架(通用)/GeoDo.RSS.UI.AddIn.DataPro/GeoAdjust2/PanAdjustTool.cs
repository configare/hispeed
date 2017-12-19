using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using GeoDo.RSS.BlockOper;
using System.IO;

namespace GeoDo.RSS.UI.AddIn.DataPro
{
    public class PanAdjustTool
    {
        public string SaveGeoAdjust(CoordEnvelope envelopeBeforeAdjust, IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                return null;
            Size oSize = new Size(srcRaster.Width, srcRaster.Height);
            //位置映射参数
            int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
            int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
            bool isInternal = new RasterMoasicClipHelper().ComputeBeginEndRowCol(srcRaster.CoordEnvelope, oSize, envelopeBeforeAdjust, oSize, ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
                                                                           ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);

            string drive = "LDF";
            CoordEnvelope dstEnvelope = envelopeBeforeAdjust;
            float resx = srcRaster.ResolutionX;
            float resy = srcRaster.ResolutionY;
            string ext = Path.GetExtension(srcRaster.fileName);
            if(string.IsNullOrWhiteSpace(ext))
                ext = ".LDF";
            else
                ext =ext.ToUpper();
            List<string> opts = new List<string>();
            if (ext == ".DAT")
            {
                drive = "MEM";
                opts.AddRange(new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=MEM",
                "WITHHDR=TRUE",
                "SPATIALREF=" + srcRaster.SpatialRef.ToProj4String(),
                "MAPINFO={" + 1 + "," + 1 + "}:{" + envelopeBeforeAdjust.MinX + "," + envelopeBeforeAdjust.MaxY + "}:{" + srcRaster.ResolutionX + "," + srcRaster.ResolutionY + "}"
                });
            }
            else
            {
                drive = "LDF";
                opts.AddRange(new string[]{
                "INTERLEAVE=BSQ",
                "VERSION=LDF",
                "WITHHDR=TRUE",
                "SPATIALREF=" + srcRaster.SpatialRef.ToProj4String(),
                "MAPINFO={" + 1 + "," + 1 + "}:{" + dstEnvelope.MinX + "," + dstEnvelope.MaxY + "}:{" +resx + "," + resy + "}",
                //"BANDNAMES="+ BandNameString(prjSettings.OutBandNos),
                "SENSOR=AVHRR"
                });
                if (srcRaster.DataIdentify != null)
                {
                    string satellite = srcRaster.DataIdentify.Satellite;
                    DateTime dt = srcRaster.DataIdentify.OrbitDateTime;
                    bool asc = srcRaster.DataIdentify.IsAscOrbitDirection;
                    if (!string.IsNullOrWhiteSpace(satellite))
                    {
                        opts.Add("SATELLITE=" + satellite);
                    }
                    if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                        opts.Add("DATETIME=" + dt.ToString("yyyy/MM/dd HH:mm"));
                    opts.Add("ORBITDIRECTION=" + (asc ? "ASC" : "DESC"));
                }
            }
            string fileName = GreatFileName(srcRaster.fileName,ext);

            IRasterDataDriver rasterDriver = GeoDataDriver.GetDriverByName(drive) as IRasterDataDriver;
            if (rasterDriver == null)
                throw new Exception("数据驱动获取失败："+ drive);
            using (IRasterDataProvider tProviders = rasterDriver.Create(fileName, srcRaster.Width, srcRaster.Height, srcRaster.BandCount, srcRaster.DataType, opts.ToArray()))
            {
                if (isInternal)
                {
                    int rowStep = ClipCutHelper.ComputeRowStep(srcRaster, oBeginRow, oEndRow);
                    int stepCount = (int)((oEndRow - oBeginRow) / rowStep + 0.5) * srcRaster.BandCount;
                    int step = 0;
                    int percent = 0;
                    int sample = (oEndCol - oBeginCol);
                    int typeSize = ClipCutHelper.GetSize(srcRaster.DataType);
                    int bufferSize = sample * rowStep * typeSize;
                    for (int oRow = oBeginRow; oRow < oEndRow; oRow += rowStep)
                    {
                        if (oRow + rowStep > oEndRow)
                            rowStep = oEndRow - oRow;
                        for (int bandIndex = 1; bandIndex <= srcRaster.BandCount; bandIndex++)
                        {
                            step++;
                            percent = (int)(step * 1.0f / stepCount * 100);
                            //if (progressCallback != null)
                            //    progressCallback(percent, "完成数据裁切" + percent + "%");
                            byte[] databuffer = new byte[bufferSize];
                            unsafe
                            {
                                fixed (byte* ptr = databuffer)
                                {
                                    IntPtr buffer = new IntPtr(ptr);
                                    srcRaster.GetRasterBand(bandIndex).Read(oBeginCol, oRow, sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                                    tProviders.GetRasterBand(bandIndex).Write(tBeginCol, tBeginRow + (oRow - oBeginRow), sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                                }
                            }
                        }
                    }
                }
                return tProviders.fileName;
            }
        }

        private string GreatFileName(string sourceFileName,string ext)
        {
            string targetFileName = Path.Combine(Path.GetDirectoryName(sourceFileName), Path.GetFileNameWithoutExtension(sourceFileName) + "_G" + ext);
            return targetFileName;
        }
    }
}
