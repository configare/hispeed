using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GPC
{
    partial class Callatlon
    {
        private float _dlat = 2.5f;

       public static double [] CalCenterLatLon(int latIndex,int lonIndex)
       {

           return new double[2] { 0.0, 0.0 };
       }

        /// <summary>
        /// 将等面积的gridcell转换为经纬度
        /// </summary>
       public static float[,] EQ2SQ( byte[] lonIndexBegin, byte[] lonIndexEnd,out int [] latIdx,out int [] cellIdx)
       {
           float [,] equalLatLonMap =new float[MAXLAT,MAXLON];
           int ibox =0;
           int ilat,ilon,jlon,lonSQ;
           int lonIdxBegin, lonIdxend;
           for(ilat=0;ilat<MAXLAT;ilat++)
           {
               for (ilon = 0; ilon < ICELLS[ilat]; ilon++)
               {
                   lonIdxBegin = lonIndexBegin[ilon];
                   lonIdxend = lonIndexEnd[ilon];
                   for (jlon = lonIdxBegin; jlon <= lonIdxend;jlon++)
                   {
                       lonSQ = jlon + MAXLON / 2;
                       if (lonSQ>MAXLON)
                       {
                           lonSQ -= MAXLON;
                       }
                       equalLatLonMap[lonSQ, ilat] = EQMAP[ibox];
                   }
                   ibox++;
               }           
           }
           return equalLatLonMap;
       }

       private void EQ2SQLUT(GCPRow[] EQMAP, out int[] cellIndex, out int[] rowIndex)
       {
           int[] equalLatLon = new int[MAXLAT* MAXLON];
           List<int> lstcellIndex = new List<int>();
           List<int> lstrowIndex = new List<int>();
           byte[][] rowdata=new byte[99][];
           int i,j,jlon, lonSQ;
           int lonIdxBegin, lonIdxend,latIdx;
           for (i = 0; i < EQMAP.Length; i++)                   //总共67行数据
           {
               GCPRow row = EQMAP[i];
               rowdata = row.GridCell;
               for(j = 0; j < rowdata.Length; j++)
               {
                   latIdx= rowdata[j][1];
                   lonIdxBegin = rowdata[j][2];
                   lonIdxend = rowdata[j][3];
                   for (jlon = lonIdxBegin; jlon <= lonIdxend; jlon++)
                   {
                       lonSQ = jlon + MAXLON / 2;
                       if (lonSQ > MAXLON)
                           lonSQ -= MAXLON;
                       lstcellIndex.Add(lonSQ);
                       lstrowIndex.Add(latIdx);
                   }
               }
           }
           cellIndex = lstcellIndex.ToArray();
           rowIndex = lstrowIndex.ToArray();
       }

    }
}
