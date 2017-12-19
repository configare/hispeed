using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.DF.LDF;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace GeoDo.RSS.DF.MVG
{
    public static class MvgHeaderConvertor
    {
        #region mvgHeader To ldfHeader
        public static Ldf1Header MvgHeaderToLdfHeader(MvgHeader mvgHeader)
        {
            Ldf1Header ldfHeader = new Ldf1Header();
            FillLdfHeaderAttributes(mvgHeader, ldfHeader);
            return ldfHeader;
        }

        private static void FillLdfHeaderAttributes(MvgHeader mvgHeader, Ldf1Header ldfHeader)
        {
            //ldfHeaderBase 成员
            ldfHeader._channelCount = 1;
            ldfHeader._dataType = (Int16)LdfHeaderBase.DataTypeToIntValue(typeof(Int16));
            ldfHeader._width = (UInt16)mvgHeader.Width;
            ldfHeader._height = (UInt16)mvgHeader.Height;
            ldfHeader._headerSize = 128;
            ldfHeader._interleave = (Int16)LdfHeaderBase.InterleaveToInt(enumInterleave.BSQ);
            //ldf1Header成员
            ldfHeader.FileIdentify = "LA";
            ldfHeader.YMDHM = mvgHeader.CreatedDateTime;
            ldfHeader.ProjectType = mvgHeader.ProjectType;
            ldfHeader.LonSolution = mvgHeader.LongitudeResolution;
            ldfHeader.LatSolution = mvgHeader.LatitudeResolution;
            ldfHeader.StandardLat1 = mvgHeader.StandardLatitude1;
            ldfHeader.StandardLat2 = mvgHeader.StandardLatitude2;
            ldfHeader.EarthRadius = mvgHeader.EarthRadius;
            ldfHeader.MinLat = mvgHeader.MinLat;
            ldfHeader.MaxLat = mvgHeader.MaxLat;
            ldfHeader.MinLon = mvgHeader.MinLon;
            ldfHeader.MaxLon = mvgHeader.MaxLon;
            GetLdfHeaderSpatialRef(mvgHeader, ldfHeader);
        }

        private static void GetLdfHeaderSpatialRef(MvgHeader mvgHeader, Ldf1Header ldfHeader)
        {
            if (mvgHeader.Version == 3)
                GetLdfHeaderSpatialRefByExtend(mvgHeader, ldfHeader);
        }

        private static void GetLdfHeaderSpatialRefByExtend(MvgHeader mvgHeader, Ldf1Header ldfHeader)
        {
            ldfHeader.GeoDoIdentify = mvgHeader.GeoDoIdentify;
            ldfHeader.PrjId = mvgHeader.PrjId;
            ldfHeader.A = mvgHeader.A;
            ldfHeader.B = mvgHeader.B;
            ldfHeader.Sp1 = mvgHeader.Sp1;
            ldfHeader.Sp2 = mvgHeader.Sp2;
            ldfHeader.Lat0 = mvgHeader.Lat0;
            ldfHeader.Lon0 = mvgHeader.Lon0;
            ldfHeader.X0 = mvgHeader.X0;
            ldfHeader.Y0 = mvgHeader.Y0;
            ldfHeader.K0 = mvgHeader.K0;
        }
        #endregion

        public static void FillMvgHeader(Ldf1Header ldfHeader,MvgHeader mvgHeader)
        {
            mvgHeader.CreatedDateTime = ldfHeader.YMDHM;
            mvgHeader.ProjectType = ldfHeader.ProjectType;
            mvgHeader.Width = (Int16)ldfHeader.Width;
            mvgHeader.Height = (Int16)ldfHeader.Height;
            mvgHeader.LongitudeResolution = ldfHeader.LonSolution;
            mvgHeader.LatitudeResolution = ldfHeader.LatSolution;
            mvgHeader.StandardLatitude1 = ldfHeader.StandardLat1;
            mvgHeader.StandardLatitude2 = ldfHeader.StandardLat2;
            mvgHeader.EarthRadius = ldfHeader.EarthRadius;
            mvgHeader.MinLat = ldfHeader.MinLat;
            mvgHeader.MaxLat = ldfHeader.MaxLat;
            mvgHeader.MinLon = ldfHeader.MinLon;
            mvgHeader.MaxLon = ldfHeader.MaxLon;
            mvgHeader.IsExtend = ldfHeader.IsExtended;
            mvgHeader.SpatialRef = ldfHeader.SpatialRef;
            if (ldfHeader.IsExtended)
            {
                mvgHeader.GeoDoIdentify = ldfHeader.GeoDoIdentify;
                mvgHeader.PrjId = ldfHeader.PrjId;
                mvgHeader.A = ldfHeader.A;
                mvgHeader.B = ldfHeader.B;
                mvgHeader.Sp1 = ldfHeader.Sp1;
                mvgHeader.Sp2 = ldfHeader.Sp2;
                mvgHeader.Lat0 = ldfHeader.Lat0;
                mvgHeader.Lon0 = ldfHeader.Lon0;
                mvgHeader.X0 = ldfHeader.X0;
                mvgHeader.Y0 = ldfHeader.Y0;
                mvgHeader.K0 = ldfHeader.K0;
            }
        }
    }
}
