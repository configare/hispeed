#region Version Info
/*========================================================================
* 功能概述：
* 
* 创建者：DongW     时间：2013/9/25 8:48:13
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
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// 类名：ArrayRasterDataProviderBuilder
    /// 属性描述：
    /// 创建者：DongW   创建日期：2013/9/25 8:48:13
    /// 修改者：             修改日期：
    /// 修改描述：
    /// 备注：
    /// </summary>
    public class ArrayRasterDataProviderBuilder
    {
        public IArrayRasterDataProvider Build(GRIB_Definition definition, GRIB_Point[] points)
        {
            float[][] bandValues = new float[1][];
            bandValues[0] = new float[definition.Width * definition.Height];
            InitBandValues(bandValues[0], definition.Width, definition.Height, points);
            //空间参考,以后根据格点场的投影类型修改
            ISpatialReference spatialRef = SpatialReference.GetDefault();
            return new ArrayRasterDataProvider<float>(definition.ValueName, bandValues, definition.Width, definition.Height,
                definition.GetCoordEnvelope(), spatialRef);
        }

        private unsafe void InitBandValues(float[] bandValues, int width, int height, GRIB_Point[] points)
        {
            fixed (GRIB_Point* ptr0 = points)
            {
                GRIB_Point* ptr = ptr0;
                fixed (float* vPtr0 = bandValues)
                {
                    float* vPtr = vPtr0;
                    //int nCount = width * height;
                    //for (int i = 0; i < nCount; i++,ptr++,vPtr++)
                    //{
                    //    *vPtr = ptr->Value;
                    //}
                    for (int col = 0; col < width; col++)
                    {
                        for (int row = height - 1; row >= 0; row--, ptr++)
                        {
                            vPtr = vPtr0 + row * width + col;
                            *vPtr = ptr->Value;
                        }
                    }
                }
            }
        }
    }
}
