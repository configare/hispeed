using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using GeoDo.Project;

namespace GeoDo.RSS.BlockOper
{
    public static class RasterMoasicChecker
    {
        public static bool CheckDataType(IRasterDataProvider[] srcRasters)
        {
            enumDataType type = srcRasters[0].DataType;
            for (int i = 1; i < srcRasters.Count(); i++)
            {
                if (type != srcRasters[i].DataType)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckBandCount(IRasterDataProvider[] srcRasters)
        {
            int count = srcRasters[0].BandCount;
            for (int i = 1; i < srcRasters.Count(); i++)
            {
                if (count != srcRasters[i].BandCount)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool CheckSpatialRef(IRasterDataProvider[] srcRasters)
        {
            for (int i = 1; i < srcRasters.Count(); i++)
            {
                if (!srcRasters[i].SpatialRef.IsSame(srcRasters[0].SpatialRef))
                {
                    return false;
                }
            }
            return true;
        }

    }
}
