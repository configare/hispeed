using System;
using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.DF.GDAL;
using System.Drawing;
using GeoDo.Project;


namespace GeoDo.RSS.DF.AWX
{
	public class AWXDataProvider : GDALRasterDataProvider
	{
		public AWXDataProvider(string fileName, IGeoDataDriver driver, enumDataProviderAccess access)
			: base(fileName, null, driver, access)
		{
		}

		public AWXDataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access)
			: base(fileName, header1024, driver, access)
		{
		}

		protected override void CallGDALBefore()
		{
			base.CallGDALBefore();
            AWXFile awxFile = new AWXFile();
            awxFile.Read(fileName);
            if (awxFile.L1Header.ProductType ==1||awxFile.L1Header.ProductType ==2)
            {
                AWXLevel2HeaderImage l2HI =awxFile.L2Header as AWXLevel2HeaderImage;
                HdrFile hdr = new HdrFile();
                hdr.DataType=0;
                if (l2HI.Reserve!=0)
                    hdr.DataType = l2HI.Reserve;
                if (awxFile.L1Header.ProductType ==2)
                {
                    AWXLevel2HeaderImagePolarOrbit l2HIP =awxFile.L2Header as AWXLevel2HeaderImagePolarOrbit;
                    if(hdr.DataType==0)
                    {
                        if(l2HIP.BytesPerPixel==2)
                            hdr.DataType =2;
                        else
                            hdr.DataType = 1;
                    }
                }
                else
                {
                    l2HI =awxFile.L2Header as AWXLevel2HeaderImageGeostationary;
                    //if(hdr.DataType==0)
                        hdr.DataType =1;
                }
                hdr.Lines = l2HI.ImageHeight;
                string[] bandNames;
                hdr.Bands = CheckBands(awxFile.L1Header, l2HI,out bandNames);//需根据静止或极轨数据进行区分；
                hdr.BandNames = bandNames;
                hdr.Samples = l2HI.ImageWidth;
                hdr.HeaderOffset = awxFile.L1Header.RecordLength;                
                hdr.Intertleave = enumInterleave.BSQ;
                hdr.ByteOrder = (awxFile.L1Header.Endian == 0) ? enumHdrByteOder.Host_intel : enumHdrByteOder.Network_IEEE;
                //TryCreateSpatialRefByHeader(l2HI);
                //hdr.HdrProjectionInfo = GetHdrProjectionInfo();
                hdr.MapInfo = GetHdrMapInfo(l2HI);
                //hdr.GeoPoints = GetHdrGeoPoints(l2HI);
                hdr.SaveTo(Path.ChangeExtension(fileName, "hdr"));
            }
		}

        private int CheckBands(AWXLevel1Header l1Hdr, AWXLevel2HeaderImage l2HeaderImage,out string [] bandNames)
        {
            int bandNO = 1;
            bandNames = null;
            int ChN=l2HeaderImage.ChannelNumber ;
            if (l1Hdr.ProductType==1)
            {
                bandNO = 1;
                bandNames = new string[1];
                switch (ChN)
                {
                    case 1:
                        bandNames[0] = "红外通道（10.3-11.3）";
                        break;
                    case 2:
                        bandNames[0] = "水汽通道 (6.3-7.6)";
                        break;
                    case 3:
                        bandNames[0] = "红外分裂窗通道 (11.5-12.5)";
                        break;
                    case 4:
                        bandNames[0] = "可见光通道 (0.5-0.9)";
                        break;
                    case 5:
                        bandNames[0] = "中红外通道（3.5-4.0）";
                        break;
                    default:
                        bandNames[0] = "备用";
                        break;
                }
            }
            else if (l1Hdr.ProductType==2)
            {
                if (ChN==0)
                {
                    AWXLevel2HeaderImagePolarOrbit l2HIP = l2HeaderImage as AWXLevel2HeaderImagePolarOrbit;
                    bandNO = 3;
                    bandNames = new string[3] { "band" + l2HIP.RChannelNumber, "band" + l2HIP.GChannelNumber, "band" + l2HIP.BChannelNumber };
                } 
                else
                {
                    bandNames = new string[1];
                    if (ChN > 0 && ChN <= 5)
                    {
                        bandNO = 1;
                        bandNames[0] = string.Format("band {0}", ChN);
                    }
                    else if (ChN >= 101 && ChN <= 119)
                    {
                        bandNO = 1;
                        bandNames[0] = "TOVS HIRS通道";
                    }
                    else if (ChN >= 201 && ChN <= 204)
                    {
                        bandNO = 1;
                        bandNames[0] = "TOVS MSU通道";
                    }
                    else
                    {
                        bandNO = 1;
                        bandNames[0] = "备用";
                    }
                }
            }
            return bandNO;
        }

        private HdrProjectionInfo GetHdrProjectionInfo()
        {
            if (_spatialRef == null || _spatialRef.ProjectionCoordSystem == null)
                return null;
            HdrProjectionInfo prjInfo = new HdrProjectionInfo();
            prjInfo.Datum = WktToHdrDatum(_spatialRef.GeographicsCoordSystem.Datum.Name);
            prjInfo.Name = _spatialRef.Name ?? string.Empty;
            prjInfo.ProjectionID = int.Parse(_spatialRef.ProjectionCoordSystem.Name.ENVIName);
            float[] args = null;
            string units = null;
            _spatialRef.ToEnviProjectionInfoString(out args, out units);
            prjInfo.PrjArguments = args;
            prjInfo.Units = units;
            return prjInfo;
        }

        private string WktToHdrDatum(string wktName)//D_WGS_1984
        {
            switch (wktName)
            {
                case "D_WGS_1984":
                    return "WGS-84";
                default:
                    return "WGS-84";
            }
        }

        protected HdrMapInfo GetHdrMapInfo(AWXLevel2HeaderImage l2H)
        {
            HdrMapInfo mapInfo = new HdrMapInfo();
            mapInfo.BaseMapCoordinateXY = new HdrGeoPointCoord(l2H.GeographicalScopeWestLongitude / 100.0f, l2H.GeographicalScopeNorthLatitude / 100.0f);//左上角经纬度
            mapInfo.BaseRowColNumber = new Point(1, 1);
            mapInfo.CoordinateType = GetCoordinateSystemName();
            float XResolution = Math.Abs(l2H.GeographicalScopeEastLongitude - l2H.GeographicalScopeWestLongitude) * 1.0f / 100.0f / l2H.ImageWidth;
            float YResolution = Math.Abs(l2H.GeographicalScopeNorthLatitude - l2H.GeographicalScopeSouthLatitude) * 1.0f / 100.0f / l2H.ImageHeight;
            mapInfo.XYResolution = new HdrGeoPointCoord(XResolution,YResolution);            
            return mapInfo;
        }

        protected string GetCoordinateSystemName()
        {
            if (_spatialRef == null || _spatialRef.GeographicsCoordSystem == null || _spatialRef.GeographicsCoordSystem.Name == null)
                return "WGS-84";
            //return _spatialRef.GeographicsCoordSystem.Name;//这里的name是wkt的，需要转换为hdr的name
            if (_spatialRef.GeographicsCoordSystem.Name == "GCS_WGS_1984")
                return "WGS-84";
            else
                return "WGS-84";
        }

        protected HdrGeoPoint[] GetHdrGeoPoints(AWXLevel2HeaderImage l2H)
        {
            Point cornerPUL = new Point(1,1);
            Point cornerPDR = new Point(l2H.ImageHeight, l2H.ImageWidth);
            HdrGeoPointCoord cornerGeoPUL = new HdrGeoPointCoord(l2H.GeographicalScopeWestLongitude / 100.0f, l2H.GeographicalScopeNorthLatitude / 100.0f);
            HdrGeoPointCoord cornerGeoPDR = new HdrGeoPointCoord(l2H.GeographicalScopeEastLongitude / 100.0f, l2H.GeographicalScopeSouthLatitude / 100.0f);
            HdrGeoPoint UL = new HdrGeoPoint(cornerPUL, cornerGeoPUL);
            HdrGeoPoint DR = new HdrGeoPoint(cornerPDR, cornerGeoPDR);
            return new HdrGeoPoint[2] { UL ,DR};
        }


        private void TryCreateSpatialRefByHeader(AWXLevel2HeaderImage l2H)
        {
            GetSpatialRefFromHeaderDefault(l2H.ProjectMode, l2H.ProjectStandardLatitude1, l2H.ProjectStandardLatitude2, l2H.ProjectCenterLongitude);
        }

        protected void GetSpatialRefFromHeaderDefault(int prjType, float sp1, float sp2, float standarLon)
        {
            using (SpatialReferenceBuilder builder = new SpatialReferenceBuilder())
            {
                _spatialRef = builder.GetSpatialRef(prjType, sp1, sp2, standarLon);
            }
        }

        protected void TrySetEnvelopeAndResolutions()
        {
            double[] coord1 = new double[2];
            _coordTransform.Raster2DataCoord(0, 0, coord1);
            double[] coord2 = new double[2];
            _coordTransform.Raster2DataCoord(_height, _width, coord2);
            _coordEnvelope = new CoordEnvelope(coord1[0], coord2[0], coord2[1], coord1[1]);

            _resolutionX = (float)(_coordEnvelope.Width / (_width));
            _resolutionY = (float)(_coordEnvelope.Height / (_height));
        }

        private void TryCreateSpatialRef()
        {
            //_spatialRef = _ldfHeader.SpatialRef;
            if (_spatialRef == null)
                _coordType = enumCoordType.Raster;
            else if (_spatialRef.ProjectionCoordSystem != null)
                _coordType = enumCoordType.PrjCoord;
            else
                _coordType = enumCoordType.GeoCoord;
        }


    }
}
