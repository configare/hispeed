using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.Drawing;
using System.IO;
using GeoDo.RSS.BlockOper;
using GeoDo.RSS.DF.LDF;

namespace GeoDo.RSS.UI.AddIn.GeoAdjust
{
    public class GeoAdjustHelper
    {
        public string SaveGeoAdjust(CoordEnvelope envelopeBeforeAdjust, IRasterDataProvider rasterRaster,IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                return null;
            Size oSize = new Size(rasterRaster.Width, rasterRaster.Height);
            //位置映射参数
            int tBeginRow = -1, tEndRow = -1, tBeginCol = -1, tEndCol = -1;
            int oBeginRow = -1, oEndRow = -1, oBeginCol = -1, oEndCol = -1;
            bool isInternal = new RasterMoasicClipHelper().ComputeBeginEndRowCol(rasterRaster.CoordEnvelope, oSize, envelopeBeforeAdjust, oSize, ref oBeginRow, ref oBeginCol, ref oEndRow, ref oEndCol,
                                                                           ref tBeginRow, ref tBeginCol, ref tEndRow, ref tEndCol);

            string drive = "LDF";
            CoordEnvelope dstEnvelope = envelopeBeforeAdjust;
            float resx = srcRaster.ResolutionX;
            float resy = srcRaster.ResolutionY;
            string ext = Path.GetExtension(srcRaster.fileName);
            if (string.IsNullOrWhiteSpace(ext))
                ext = ".LDF";
            else
                ext = ext.ToUpper();
            List<string> opts = new List<string>();
            if(ext==".LDF"||ext==".DAT")
            {
                if (ext == ".LDF")
                {
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
                else 
                {
                    drive = "MEM";
                    opts.AddRange(new string[]{
                    "INTERLEAVE=BSQ",
                    "VERSION=MEM",
                    "WITHHDR=TRUE",
                    "SPATIALREF=" + (srcRaster.SpatialRef==null?"":srcRaster.SpatialRef.ToProj4String()),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + envelopeBeforeAdjust.MinX + "," + envelopeBeforeAdjust.MaxY + "}:{" + srcRaster.ResolutionX + "," + srcRaster.ResolutionY + "}"
                    });
                }
                string fileName = GreatFileName(srcRaster.fileName, ext);
                IRasterDataDriver rasterDriver = GeoDataDriver.GetDriverByName(drive) as IRasterDataDriver;
                if (rasterDriver == null)
                    throw new Exception("数据驱动获取失败：" + drive);
                using (IRasterDataProvider tProviders = rasterDriver.Create(fileName, rasterRaster.Width, rasterRaster.Height, srcRaster.BandCount, srcRaster.DataType, opts.ToArray()))
                {
                    if (isInternal)
                    {
                        int rowStep = ClipCutHelper.ComputeRowStep(srcRaster, oBeginRow, oEndRow);
                        int stepCount = (int)((oEndRow - oBeginRow) / rowStep + 0.5) * srcRaster.BandCount;
                        int step = 0;
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
            //仅用于水情MVG
            else if (ext == ".MVG")
            {
                drive = "MEM";
                ext = ".DAT";
                opts.AddRange(new string[]{
                    "INTERLEAVE=BSQ",
                    "VERSION=MEM",
                    "WITHHDR=TRUE",
                    "SPATIALREF=" + (rasterRaster.SpatialRef==null?"":rasterRaster.SpatialRef.ToProj4String()),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + envelopeBeforeAdjust.MinX + "," + envelopeBeforeAdjust.MaxY + "}:{" + rasterRaster.ResolutionX + "," + rasterRaster.ResolutionY + "}"
                    });
                string fileName = GreatFileName(srcRaster.fileName, ext);
                IRasterDataDriver rasterDriver = GeoDataDriver.GetDriverByName(drive) as IRasterDataDriver;
                if (rasterDriver == null)
                    throw new Exception("数据驱动获取失败：" + drive);
                using (IRasterDataProvider tProviders = rasterDriver.Create(fileName, rasterRaster.Width, rasterRaster.Height, srcRaster.BandCount, srcRaster.DataType, opts.ToArray()))
                {
                    if (isInternal)
                    {
                        int rowStep = ClipCutHelper.ComputeRowStep(srcRaster, oBeginRow, oEndRow);
                        int stepCount = (int)((oEndRow - oBeginRow) / rowStep + 0.5) * srcRaster.BandCount;
                        int step = 0;
                        int sample = (oEndCol - oBeginCol);
                        int bufferSize = sample * rowStep;
                        for (int oRow = oBeginRow; oRow < oEndRow; oRow += rowStep)
                        {
                            if (oRow + rowStep > oEndRow)
                                rowStep = oEndRow - oRow;
                            for (int bandIndex = 1; bandIndex <= srcRaster.BandCount; bandIndex++)
                            {
                                step++;
                                switch(srcRaster.DataType)
                                {
                                    case enumDataType.Int16:
                                        {
                                            Int16[] databuffer = new Int16[bufferSize];
                                            unsafe
                                            {
                                                fixed (Int16* ptr = databuffer)
                                                {
                                                    IntPtr buffer = new IntPtr(ptr);
                                                    srcRaster.GetRasterBand(bandIndex).Read(oBeginCol, oRow, sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                                                    //修改值
                                                    for (int i = 0; i < bufferSize; i++)
                                                    {
                                                        if (databuffer[i] > 1)
                                                            databuffer[i] = 0;
                                                    }
                                                    tProviders.GetRasterBand(bandIndex).Write(tBeginCol, tBeginRow + (oRow - oBeginRow), sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                                                }
                                            }
                                            break;
                                        }
                                }
                            }
                        }
                    }
                    return tProviders.fileName;
                }
            }
            return null;
           
        }

        public string SaveGeoAdjustByChangeCoordEnvelope(CoordEnvelope envelopeAfterAdjust, IRasterDataProvider srcRaster)
        {
            if (srcRaster == null)
                return null;
            string drive = "LDF";
            CoordEnvelope dstEnvelope = envelopeAfterAdjust;
            float resx = srcRaster.ResolutionX;
            float resy = srcRaster.ResolutionY;
            string extension = Path.GetExtension(srcRaster.fileName).ToUpper();
            List<string> opts = new List<string>();
            if (extension == ".LDF" ||extension==".LD3"|| extension==".LD2"||extension == ".DAT")
            {
                if (extension == ".DAT")
                {
                    drive = "MEM";
                    opts.AddRange(new string[]{
                    "INTERLEAVE=BSQ",
                    "VERSION=MEM",
                    "WITHHDR=TRUE",
                    "SPATIALREF=" + (srcRaster.SpatialRef==null?"":srcRaster.SpatialRef.ToProj4String()),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + envelopeAfterAdjust.MinX + "," + envelopeAfterAdjust.MaxY + "}:{" + srcRaster.ResolutionX + "," + srcRaster.ResolutionY + "}"
                    });
                    string fileName = GreatFileName(srcRaster.fileName, extension);
                    IRasterDataDriver rasterDriver = GeoDataDriver.GetDriverByName(drive) as IRasterDataDriver;
                    if (rasterDriver == null)
                        throw new Exception("数据驱动获取失败：" + drive);
                    using (IRasterDataProvider tProviders = rasterDriver.Create(fileName, srcRaster.Width, srcRaster.Height, srcRaster.BandCount, srcRaster.DataType, opts.ToArray()))
                    {
                        int rowStep = ClipCutHelper.ComputeRowStep(srcRaster, 0, srcRaster.Height);
                        int sample = srcRaster.Width;
                        int typeSize = ClipCutHelper.GetSize(srcRaster.DataType);
                        int bufferSize = sample * rowStep * typeSize;
                        for (int oRow = 0; oRow < srcRaster.Height; oRow += rowStep)
                        {
                            if (oRow + rowStep > srcRaster.Height)
                                rowStep = srcRaster.Height - oRow;
                            for (int bandIndex = 1; bandIndex <= srcRaster.BandCount; bandIndex++)
                            {
                                byte[] databuffer = new byte[bufferSize];
                                unsafe
                                {
                                    fixed (byte* ptr = databuffer)
                                    {
                                        IntPtr buffer = new IntPtr(ptr);
                                        srcRaster.GetRasterBand(bandIndex).Read(0, oRow, sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                                        tProviders.GetRasterBand(bandIndex).Write(0, oRow, sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                                    }
                                }
                            }
                        }
                        return tProviders.fileName;
                    }
                }
                else
                {
                //    opts.AddRange(new string[]{
                //"INTERLEAVE=BSQ",
                //"VERSION=LDF",
                //"WITHHDR=TRUE",
                //"SPATIALREF=" + srcRaster.SpatialRef.ToProj4String(),
                //"MAPINFO={" + 1 + "," + 1 + "}:{" + dstEnvelope.MinX + "," + dstEnvelope.MaxY + "}:{" +resx + "," + resy + "}",
                ////"BANDNAMES="+ BandNameString(prjSettings.OutBandNos),
                //});
                //    if (srcRaster.DataIdentify != null)
                //    {
                //        string satellite = srcRaster.DataIdentify.Satellite;
                //        string sensor = srcRaster.DataIdentify.Sensor;
                //        DateTime dt = srcRaster.DataIdentify.OrbitDateTime;
                //        bool asc = srcRaster.DataIdentify.IsAscOrbitDirection;
                //        if (!string.IsNullOrWhiteSpace(satellite))
                //        {
                //            opts.Add("SATELLITE=" + satellite);
                //        }
                //        if (!string.IsNullOrWhiteSpace(sensor))
                //        {
                //            opts.Add("SENSOR=" + sensor);
                //        }
                //        if (dt != DateTime.MinValue && dt != DateTime.MaxValue)
                //            opts.Add("DATETIME=" + dt.ToString("yyyy/MM/dd HH:mm"));
                //        opts.Add("ORBITDIRECTION=" + (asc ? "ASC" : "DESC"));
                //    }
                    //内存拷贝
                    string srcfileName = srcRaster.fileName;
                    string desfileName = GreatFileName(srcRaster.fileName, extension);
                    File.Copy(srcfileName, desfileName);
                    using (IRasterDataProvider dataPrd = GeoDataDriver.Open(desfileName) as IRasterDataProvider)
                    {
                        (dataPrd as ILdfDataProvider).Update(envelopeAfterAdjust);
                    }
                    string hdrSrcName = Path.ChangeExtension(srcfileName, ".hdr");
                    string hdrDesName = Path.ChangeExtension(desfileName, ".hdr");
                    HdrFile hdr = HdrFile.LoadFrom(hdrSrcName);
                    if (hdr.MapInfo != null)
                    {
                        hdr.MapInfo.BaseMapCoordinateXY.Latitude = envelopeAfterAdjust.Center.Y;
                        hdr.MapInfo.BaseMapCoordinateXY.Longitude = envelopeAfterAdjust.Center.X;
                    }
                    if (hdr.GeoPoints != null)
                    {
                        hdr.GeoPoints = null;
                    }
                    HdrFile.SaveTo(hdrDesName, hdr);
                    return desfileName;
                }
                
            }
            //仅用于水情MVG
            else if (extension == ".MVG")
            {
                drive = "MEM";
                string ext = ".DAT";
                opts.AddRange(new string[]{
                    "INTERLEAVE=BSQ",
                    "VERSION=MEM",
                    "WITHHDR=TRUE",
                    "SPATIALREF=" + (srcRaster.SpatialRef==null?"":srcRaster.SpatialRef.ToProj4String()),
                    "MAPINFO={" + 1 + "," + 1 + "}:{" + envelopeAfterAdjust.MinX + "," + envelopeAfterAdjust.MaxY + "}:{" + srcRaster.ResolutionX + "," + srcRaster.ResolutionY + "}"
                    });
                string fileName = GreatFileName(srcRaster.fileName, ext);
                IRasterDataDriver rasterDriver = GeoDataDriver.GetDriverByName(drive) as IRasterDataDriver;
                if (rasterDriver == null)
                    throw new Exception("数据驱动获取失败：" + drive);
                using (IRasterDataProvider tProviders = rasterDriver.Create(fileName, srcRaster.Width, srcRaster.Height, srcRaster.BandCount, srcRaster.DataType, opts.ToArray()))
                {
                    int rowStep = ClipCutHelper.ComputeRowStep(srcRaster, 0, srcRaster.Height);
                    int sample = srcRaster.Width;
                    int bufferSize = sample * rowStep;
                    for (int oRow = 0; oRow < srcRaster.Height; oRow += rowStep)
                    {
                        if (oRow + rowStep > srcRaster.Height)
                            rowStep = srcRaster.Height - oRow;
                        for (int bandIndex = 1; bandIndex <= srcRaster.BandCount; bandIndex++)
                        {
                            switch (srcRaster.DataType)
                            {
                                case enumDataType.Int16:
                                    {
                                        Int16[] databuffer = new Int16[bufferSize];
                                        unsafe
                                        {
                                            fixed (Int16* ptr = databuffer)
                                            {
                                                IntPtr buffer = new IntPtr(ptr);
                                                srcRaster.GetRasterBand(bandIndex).Read(0, oRow, sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                                                //修改值
                                                for (int i = 0; i < bufferSize; i++)
                                                {
                                                    if (databuffer[i] > 1)
                                                        databuffer[i] = 0;
                                                }
                                                tProviders.GetRasterBand(bandIndex).Write(0, oRow, sample, rowStep, buffer, srcRaster.DataType, sample, rowStep);
                                            }
                                        }
                                        break;
                                    }
                            }
                        }
                    }
                    return tProviders.fileName;
                }
            }
            return null;
        }

        private string GreatFileName(string sourceFileName, string ext)
        {
            string targetFileName = Path.Combine(Path.GetDirectoryName(sourceFileName), Path.GetFileNameWithoutExtension(sourceFileName) + "_G" + ext);
            return targetFileName;
        }
    }
}
