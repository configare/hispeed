#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：Administrator     时间：2014-1-15 14:43:50
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
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.HDF5;
//using GeoDo.RSS.DF.GDAL.HDF5Universal;
using GeoDo.RSS.DF.GDAL;
using OSGeo.GDAL;
using GeoDo.Project;
using System.Drawing;
using System.IO;


namespace GeoDo.RSS.DF.GDAL.HDF5GEO
{
    /// <summary>
    /// 类名：FYSnowPrdDataProvider
    /// 属性描述：FY3 MWRI 积雪产品读取
    /// 创建者：LiXJ   创建日期：2014-1-15 14:43:50
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
   public class FYSnowPrdDataProvider : RasterDataProvider
    {
       private object[] _args = null;
       private string[] _threebnds = null;
       protected string[] _allGdalSubDatasets = null;

       public FYSnowPrdDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, params object[] args)
           : base(fileName, driver)
       {
           _args = args;
           TryParseArgs();
           using (Dataset dataset = Gdal.Open(fileName, Access.GA_ReadOnly))
           {
               GDALHelper.GetDatasetAttributes(dataset, _attributes);
           }
           TryCreateBandProvider();
           _bandCount = _rasterBands.Count;
           TryGetDatTypeOfProvider();
           TryGetSizeOfProvider();
           //TryCreateSpatialRef();
           _coordType = enumCoordType.PrjCoord;
           _resolutionX = (float)(_coordEnvelope.Width / (_width));
           _resolutionY = (float)(_coordEnvelope.Height / (_height));
           if (_dataIdentify != null)
           {
               _dataIdentify.Sensor = "MWRI";
               _dataIdentify.OrbitDateTime = IceConDataProvider.TryGetFileDate(Path.GetFileName(fileName));
           }
       }
       private void TryParseArgs()
       {
           if (_args == null || _args.Length == 0)
           {
               _threebnds = new string[] { "SD_NorthernDaily_A" };
               _spatialRef = SpatialReference.FromProj4("+proj=laea +lat_0=90 +lon_0=0 +x_0=0 +y_0=0 +a=6371228 +b=6371228 +units=m +no_defs");
               _coordEnvelope = new CoordEnvelope(-9036842.762, 9036842.762, -9036842.762, 9036842.762);
           }
           else
           {
               string[] alldatasets = new string[16]{ "SD_Flags_NorthernDaily_A", "SD_Flags_NorthernDaily_D", "SD_Flags_SouthernDaily_A", "SD_Flags_SouthernDaily_D", 
                          "SD_NorthernDaily_A", "SD_NorthernDaily_D" ,"SD_SouthernDaily_A","SD_SouthernDaily_D",
                           "SWE_Flags_NorthernDaily_A", "SWE_Flags_NorthernDaily_D", "SWE_Flags_SouthernDaily_A", "SWE_Flags_SouthernDaily_D", 
                          "SWE_NorthernDaily_A", "SWE_NorthernDaily_D" ,"SWE_SouthernDaily_A","SWE_SouthernDaily_D"};
               string[] arguments = _args as string[];
               List<string> datas = new List<string>();
               string[] parts;
               foreach (string set in arguments)
               {
                   if (set.Contains("NorthernDaily") || set.Contains("SouthernDaily"))
                   {
                       parts = set.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                       foreach (string part in parts)
                       {
                           foreach (string data in alldatasets)
                           {
                               if (data == part)
                               {
                                   datas.Add(part);
                                   break;
                               }
                           }
                       }
                   }
               }
               if (datas.Count == 0)
               {
                   _threebnds = new string[] { "SD_NorthernDaily_A" };
                   _spatialRef = SpatialReference.FromProj4("+proj=laea +lat_0=90 +lon_0=0 +x_0=0 +y_0=0 +a=6371228 +b=6371228 +units=m +no_defs");
                   _coordEnvelope = new CoordEnvelope(-9036842.762, 9036842.762, -9036842.762, 9036842.762);
                   return;
               }
               _threebnds = datas.ToArray();

               if (_threebnds[0].Contains("NorthernDaily"))
               {
                   _spatialRef = SpatialReference.FromProj4("+proj=laea +lat_0=90 +lon_0=0 +x_0=0 +y_0=0 +a=6371228 +b=6371228 +units=m +no_defs");
                   _coordEnvelope = new CoordEnvelope(-9036842.762, 9036842.762, -9036842.762, 9036842.762);
               }

               if (_threebnds[0].Contains("SouthernDaily"))
               {
                   _spatialRef = SpatialReference.FromProj4("+proj=laea +lat_0=-90 +lon_0=0 +x_0=0 +y_0=0 +a=6371228 +b=6371228 +units=m +no_defs ");
                   _coordEnvelope = new CoordEnvelope(-9036842.762, 9036842.762, -9036842.762, 9036842.762);
               }
           }
       }
       #region 设置波段
       private void TryCreateBandProvider()
       {
           Dictionary<string, string> allGdalSubDatasets = this.Attributes.GetAttributeDomain("SUBDATASETS");
           _allGdalSubDatasets = RecordAllSubDatasetNames(allGdalSubDatasets);
           foreach (string dsName in _threebnds)
           {
               string dsPath = GetDatasetFullPath(dsName);
               Dataset dataset = Gdal.Open(dsPath, Access.GA_ReadOnly);
               IRasterBand[] gdalDatasets = ReadBandsFromDataset(dsName, dataset, this);
               _rasterBands.AddRange(gdalDatasets);
           }
       }

       private string GetDatasetFullPath(string datasetName)
       {
           return FindGdalSubDataset(datasetName);
       }

       private string GetDatasetShortName(string datasetName)
       {
           string shortDatasetName = null;
           int groupIndex = datasetName.LastIndexOf("/");
           if (groupIndex == -1)
               shortDatasetName = datasetName;
           else
               shortDatasetName = datasetName.Substring(groupIndex + 1);
           return shortDatasetName;
       }

       private string FindGdalSubDataset(string datasetShortName)
       {
           for (int i = 0; i < _allGdalSubDatasets.Length; i++)
           {
               string shortGdalDatasetName = GetDatasetShortName(_allGdalSubDatasets[i]);
               if (shortGdalDatasetName == datasetShortName)
                   return _allGdalSubDatasets[i];
           }
           return null;
       }

       private string[] RecordAllSubDatasetNames(Dictionary<string, string> subdatasets)
       {
           List<string> dss = new List<string>();
           int idx = 0;
           foreach (string key in subdatasets.Keys)
               if (idx++ % 2 == 0)
                   dss.Add(subdatasets[key]);
           return dss.ToArray();
       }

       private IRasterBand[] ReadBandsFromDataset(string dsname, Dataset ds, IRasterDataProvider provider)
       {
           int bandNo = 1;
           IRasterBand[] bands = new IRasterBand[ds.RasterCount];
           for (int i = 1; i <= ds.RasterCount; i++)
           {
               bands[i - 1] = new GDALRasterBand(provider, ds.GetRasterBand(i), new GDALDataset(ds));
               bands[i - 1].BandNo = bandNo++;
               bands[i - 1].Description = dsname;
           }
           return bands;
       }

       #endregion

       private void TryGetDatTypeOfProvider()
       {
           if (_rasterBands != null && _rasterBands.Count > 0)
           {
               _dataType = _rasterBands[0].DataType;
           }
       }

       private void TryGetSizeOfProvider()
       {
           if (_rasterBands != null && _rasterBands.Count > 0)
           {
               _width = _rasterBands[0].Width;
               _height = _rasterBands[0].Height;
           }
       }

       //private void TryCreateSpatialRef()
       //{
       //    try
       //    {
       //        if (_spatialRef == null)
       //            _coordType = enumCoordType.Raster;
       //        else
       //            _coordType = _spatialRef.ProjectionCoordSystem != null ? enumCoordType.PrjCoord : enumCoordType.GeoCoord;
       //    }
       //    catch (Exception ex)
       //    {
       //        Console.WriteLine(ex.Message);
       //    }
       //}

       //private void TryCreateCoordTransform()
       //{
       //    if (_spatialRef == null)
       //        _coordTransform = CoordTransoformFactory.GetCoordTransform(null, null, _width, _height);
       //    else
       //    {
       //        _coordTransform = CoordTransoformFactory.GetCoordTransform(
       //            new Point(0, 0),
       //            new Point(_width, _height),
       //            new double[] { _coordEnvelope.MinX, _coordEnvelope.MaxY },
       //            new double[] { _coordEnvelope.MaxX, _coordEnvelope.MinY });
       //    }
       //}

       //protected void TrySetEnvelopeAndResolutions()
       //{
       //    double[] coord1 = new double[2];
       //    _coordTransform.Raster2DataCoord(0, 0, coord1);
       //    double[] coord2 = new double[2];
       //    _coordTransform.Raster2DataCoord(_height, _width, coord2);
       //    _resolutionX = (float)(_coordEnvelope.Width / (_width));
       //    _resolutionY = (float)(_coordEnvelope.Height / (_height));
       //}

       public override void AddBand(enumDataType dataType)
       {
           throw new NotImplementedException();
       }
       public override int[] GetDefaultBands()
       {
           if (_rasterBands.Count == 1)
               return new int[] { 1 };
           else
               return new int[] { 3 };
       }

       public override object GetStretcher(int bandNo)
       {
           object stratcher = RgbStretcherFactory.GetStretcher("MWRIX.SWE");
           if (stratcher == null)
               stratcher = RgbStretcherFactory.CreateStretcher(enumDataType.Byte, 0, 255);
           return stratcher;
       }
    }
}
