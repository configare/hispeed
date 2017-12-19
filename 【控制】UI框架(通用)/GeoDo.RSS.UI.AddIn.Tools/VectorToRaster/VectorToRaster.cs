using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using GeoDo.RSS.Core.DF;
using CodeCell.AgileMap.Core;
using GeoDo.RSS.MIF.Core;
using System.IO;
using System.Drawing.Imaging;

namespace GeoDo.RSS.UI.AddIn.Tools
{
    public class VectorToRaster:IVectorToRaster
    {
        private Action<int, string> _progressCallback;

        public VectorToRaster(Action<int, string> progressCallback)
        {
            _progressCallback = progressCallback;
        }

        public void ProcessVectorToRaster(string shpFileName,string shpPrimaryField ,enumDataType dataType,double resolution,string rasterFileName)
        {
            //创建目标文件
            if(string.IsNullOrEmpty(rasterFileName))
                return;
            if (string.IsNullOrEmpty(shpFileName)||!File.Exists(shpFileName)||
                Path.GetExtension(shpFileName).ToUpper()!=".SHP")
                return ;
            using (IVectorFeatureDataReader dr = VectorDataReaderFactory.GetUniversalDataReader(shpFileName) 
                as IVectorFeatureDataReader)
            {
                if (dr == null)
                    return;
                Envelope env = dr.Envelope;
                if (env == null)
                    return;
                CoordEnvelope envelope = new CoordEnvelope(env.MinX, env.MaxX, env.MinY, env.MaxY);
                int height=(int)Math.Ceiling((envelope.MaxY-envelope.MinY)/resolution);
                int width=(int)Math.Ceiling((envelope.MaxX-envelope.MinX)/resolution);
                IRasterDataProvider dataPrd = null;
                try
                {
                    string extension = Path.GetExtension(rasterFileName).ToUpper();
                    switch (extension)
                    {
                        case ".LDF":
                            {
                                IRasterDataDriver driver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
                                string mapInfo = envelope.ToMapInfoString(new Size(width, height));
                                dataPrd = driver.Create(rasterFileName, width, height, 1, dataType, mapInfo);
                                break;
                            }
                        case ".DAT":
                            {
                                IRasterDataDriver driver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                                string mapInfo = envelope.ToMapInfoString(new Size(width, height));
                                dataPrd = driver.Create(rasterFileName, width, height, 1, dataType, mapInfo);
                                break;
                            }
                        default:
                            return;
                    }
                    Feature[] features = dr.FetchFeatures();
                    if (features == null || features.Length < 1)
                        return;
                    ProcessVectorToRaster(features, shpPrimaryField, dataPrd);
                }
                finally
                {
                    if (dataPrd != null)
                        dataPrd.Dispose();
                }
            }
        }

        public void ProcessVectorToRaster(Feature[] features, string shpPrimaryField, IRasterDataProvider rasterProvider)
        {
            if (features == null || features.Length < 1)
                return;
            for(int i=0;i<features.Length;i++)
            {
                if(features[i].Geometry is ShapePolygon)
                    continue;
                else
                    return;
            }
            Dictionary<string, Color> nameColors = new Dictionary<string, Color>();
            int[] aoi;
            if(_progressCallback!=null)
                _progressCallback(0, "开始矢量栅格化");
            using (Bitmap bitmap = VectorsToBitmap(rasterProvider, features, shpPrimaryField, out nameColors))
            {
                Size size = new Size(rasterProvider.Width, rasterProvider.Height);
                string fieldValue;
                Color color;
                float persent = -1f;
                foreach (Feature fea in features)
                {
                    persent++;
                    if (shpPrimaryField == "OID")
                        fieldValue = fea.OID.ToString();
                    else
                        fieldValue = fea.GetFieldValue(shpPrimaryField);
                    if (String.IsNullOrEmpty(fieldValue))
                        continue;
                    color = nameColors[fieldValue];
                    aoi = GetAOIByFeature(bitmap, color);
                    //修改IRasterDataProvider值
                    if (aoi != null && aoi.Length > 0)
                    {
                        switch (rasterProvider.DataType)
                        {
                            case enumDataType.Float:
                                {
                                    float value;
                                    if (float.TryParse(fieldValue, out value))
                                    {
                                        RasterWriterFloat(rasterProvider, aoi, rasterProvider.Width, value,_progressCallback,new float[]{persent,features.Length});
                                        continue;
                                    }
                                    else
                                    {
                                        throw new Exception("设置的主属性无法进行栅格化转换！");
                                    }
                                }
                            case enumDataType.Int16:
                                {
                                    Int16 value;
                                    if (Int16.TryParse(fieldValue, out value))
                                    {
                                        RasterWriterInt16(rasterProvider, aoi, rasterProvider.Width, value,_progressCallback, new float[] { persent, features.Length });
                                        continue;
                                    }
                                    else
                                    {
                                        throw new Exception("设置的主属性无法进行栅格化转换！");
                                    }
                                }
                            case enumDataType.Int32:
                                {
                                    Int32 value;
                                    if (Int32.TryParse(fieldValue, out value))
                                    {
                                        RasterWriterInt32(rasterProvider, aoi, rasterProvider.Width, value, _progressCallback, new float[] { persent, features.Length });
                                        continue;
                                    }
                                    else
                                    {
                                        throw new Exception("设置的主属性无法进行栅格化转换！");
                                    }
                                }
                            case enumDataType.UInt16:
                                {
                                    UInt16 value;
                                    if (UInt16.TryParse(fieldValue, out value))
                                    {
                                        RasterWriterUInt16(rasterProvider, aoi, rasterProvider.Width, value, _progressCallback, new float[] { persent, features.Length });
                                        continue;
                                    }
                                    else
                                    {
                                        throw new Exception("设置的主属性无法进行栅格化转换！");
                                    }
                                }
                            case enumDataType.Byte:
                                {
                                    Byte value;
                                    if (Byte.TryParse(fieldValue, out value))
                                    {
                                        RasterWriterByte(rasterProvider, aoi, rasterProvider.Width, value, _progressCallback, new float[] { persent, features.Length });
                                        continue;
                                    }
                                    else
                                    {
                                        throw new Exception("设置的主属性无法进行栅格化转换！");
                                    }
                                }
                            default:
                                return;
                        }
                    }
                }
                if (_progressCallback != null)
                    _progressCallback(100, "矢量栅格化完成！");
            }
        }

        private unsafe void RasterWriterByte(IRasterDataProvider rasterProvider, int[] aois, int width, byte value, Action<int, string> progressCallback, float[] processPercent)
        {
            byte[] buffer = new byte[] { value };
            int curStep = -1;
            fixed (byte* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                for (int i = 0; i < aois.Length; i++)
                {
                    curStep++;
                    int curp = (int)(((processPercent[0] + curStep * 1.0f / aois.Length) / processPercent[1]) * 100);
                    if (progressCallback != null)
                        progressCallback(curp, "栅格化完成" + curp + "%");
                    int row = aois[i] / width;
                    int col = aois[i] % width;
                    rasterProvider.GetRasterBand(1).Write(col - 1, row - 1, 1, 1, bufferPtr, enumDataType.Byte, 1, 1);
                }
            }
        }

        private unsafe void RasterWriterUInt16(IRasterDataProvider rasterProvider, int[] aois, int width, ushort value, Action<int, string> progressCallback, float[] processPercent)
        {
            UInt16[] buffer = new UInt16[] { value };
            int curStep = -1;
            fixed (UInt16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                for (int i = 0; i < aois.Length; i++)
                {
                    curStep++;
                    int curp = (int)(((processPercent[0] + curStep * 1.0f / aois.Length) / processPercent[1]) * 100);
                    if (progressCallback != null)
                        progressCallback(curp, "栅格化完成" + curp + "%");
                    int row = aois[i] / width;
                    int col = aois[i] % width;
                    rasterProvider.GetRasterBand(1).Write(col - 1, row - 1, 1, 1, bufferPtr, enumDataType.UInt16, 1, 1);
                }
            }
        }

        private unsafe void RasterWriterInt32(IRasterDataProvider rasterProvider, int[] aois, int width, Int32 value, Action<int, string> progressCallback, float[] processPercent)
        {
            Int32[] buffer = new Int32[] { value };
            int curStep = -1;
            fixed (Int32* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                for (int i = 0; i < aois.Length; i++)
                {
                    curStep++;
                    int curp = (int)(((processPercent[0] + curStep * 1.0f / aois.Length) / processPercent[1]) * 100);
                    if (progressCallback != null)
                        progressCallback(curp, "栅格化完成" + curp + "%");
                    int row = aois[i] / width;
                    int col = aois[i] % width;
                    rasterProvider.GetRasterBand(1).Write(col - 1, row - 1, 1, 1, bufferPtr, enumDataType.Int32, 1, 1);
                }
            }
        }

        private unsafe void RasterWriterInt16(IRasterDataProvider rasterProvider, int[] aois, int width, Int16 value, Action<int, string> progressCallback, float[] processPercent)
        {
            Int16[] buffer = new Int16[] { value };
            int curStep = -1;
            fixed (Int16* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);
                for (int i = 0; i < aois.Length; i++)
                {
                    curStep++;
                    int curp = (int)(((processPercent[0] + curStep * 1.0f / aois.Length) / processPercent[1]) * 100);
                    if (progressCallback != null)
                        progressCallback(curp, "栅格化完成" + curp + "%");
                    int row = aois[i] / width;
                    int col = aois[i] % width;
                    rasterProvider.GetRasterBand(1).Write(col - 1, row - 1, 1, 1, bufferPtr, enumDataType.Int16, 1, 1);
                }
            }
        }

        private unsafe void RasterWriterFloat(IRasterDataProvider rasterProvider, int[] aois, int width, float value, Action<int, string> progressCallback,float[] processPercent)
        {
            float[] buffer = new float[] { value };
            int curStep = -1;
            fixed (float* ptr = buffer)
            {
                IntPtr bufferPtr = new IntPtr(ptr);  
                for (int i = 0; i < aois.Length; i++)
                {
                    curStep++;
                    int curp = (int)(((processPercent[0] + curStep * 1.0f / aois.Length) / processPercent[1]) * 100);
                    if (progressCallback != null)
                        progressCallback(curp, "栅格化完成" + curp + "%");
                    int row = aois[i] / width;
                    int col = aois[i] % width;   
                    rasterProvider.GetRasterBand(1).Write(col - 1, row - 1, 1, 1, bufferPtr, enumDataType.Float, 1, 1);
                 }
            }
        }

        public Bitmap VectorsToBitmap(IRasterDataProvider prd, Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
        {
            using (IVector2BitmapConverter c = new Vector2BitmapConverter())
            {
                Dictionary<ShapePolygon, Color> vectors = GetVectorColors(features, shpPrimaryField, out nameColors);
                Bitmap bmp = new Bitmap(prd.Width, prd.Height, PixelFormat.Format24bppRgb);
                Envelope envelop = GetEnvelop(prd);
                c.ToBitmap(vectors, Color.Black, envelop, new Size(prd.Width, prd.Height), ref bmp);
                return bmp;
            }
        }

        private Envelope GetEnvelop(IRasterDataProvider prd)
        {
            return new Envelope(prd.CoordEnvelope.MinX, prd.CoordEnvelope.MinY, prd.CoordEnvelope.MaxX, prd.CoordEnvelope.MaxY);
        }

        private Dictionary<ShapePolygon, Color> GetVectorColors(Feature[] features, string shpPrimaryField, out Dictionary<string, Color> nameColors)
        {
            Dictionary<ShapePolygon, Color> vectorColors = new Dictionary<ShapePolygon, Color>();
            nameColors = new Dictionary<string, Color>();
            int count = features.Count();
            Random random = new Random(1);
            Color color;
            if (shpPrimaryField == "OID")
            {
                for (int i = 0; i < count; i++)
                {
                    color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                    if (features[i].OID == null)
                        continue;
                    if (!nameColors.Keys.Contains(features[i].OID.ToString())
                      && !String.IsNullOrEmpty(features[i].OID.ToString()))
                        nameColors.Add(features[i].OID.ToString(), color);
                    if (!vectorColors.Keys.Contains(features[i].Geometry as ShapePolygon))
                        vectorColors.Add(features[i].Geometry as ShapePolygon, color);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    color = Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
                    if (features[i].GetFieldValue(shpPrimaryField) == null)
                        continue;
                    if (!nameColors.Keys.Contains(features[i].GetFieldValue(shpPrimaryField))
                      && !String.IsNullOrEmpty(features[i].GetFieldValue(shpPrimaryField)))
                        nameColors.Add(features[i].GetFieldValue(shpPrimaryField), color);
                    if (!vectorColors.Keys.Contains(features[i].Geometry as ShapePolygon))
                        vectorColors.Add(features[i].Geometry as ShapePolygon, color);
                }
            }
            return vectorColors;
        }

        public int[] GetAOIByFeature(Bitmap bitmap, Color color)
        {
            using (IBitmap2RasterConverter c = new Bitmap2RasterConverter())
            {
                return c.ToRaster(bitmap, color);
            }
        }

        public void ProcessVectorToRaster(Feature[] features, string shpPrimaryField, enumDataType dataType, double resolution, CoordEnvelope envelope, string rasterFileName)
        {
            //创建目标文件
            if(string.IsNullOrEmpty(rasterFileName))
                return;
            if (envelope == null)
                return;
            int height=(int)Math.Ceiling((envelope.MaxY-envelope.MinY)/resolution);
            int width=(int)Math.Ceiling((envelope.MaxX-envelope.MinX)/resolution);
            IRasterDataProvider dataPrd = null;
            try
            {
                string extension = Path.GetExtension(rasterFileName).ToUpper();
                switch (extension)
                {
                    case ".LDF":
                        {
                            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("LDF") as IRasterDataDriver;
                            string mapInfo = envelope.ToMapInfoString(new Size(width, height));
                            dataPrd = driver.Create(rasterFileName, width, height, 1, dataType, mapInfo);
                            break;
                        }
                    case ".DAT":
                        {
                            IRasterDataDriver driver = GeoDataDriver.GetDriverByName("MEM") as IRasterDataDriver;
                            string mapInfo = envelope.ToMapInfoString(new Size(width, height));
                            dataPrd = driver.Create(rasterFileName, width, height, 1, dataType, mapInfo);
                            break;
                        }
                    default:
                        return;
                }
                if (features == null || features.Length < 1)
                    return;
                ProcessVectorToRaster(features, shpPrimaryField, dataPrd);
            }
            finally
            {
                if (dataPrd != null)
                    dataPrd.Dispose();
            }
        }

    }
}
