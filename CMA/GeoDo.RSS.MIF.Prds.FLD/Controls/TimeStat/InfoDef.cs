using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.FLD
{
    public class SearchConditionClass
    {
        public DateTime BeginDate = DateTime.Now.AddYears(-5);
        public DateTime EndDate = DateTime.Now;
        public string RegionName;
        public string RegionIdentify;
        public int Resolution;
        public TimeFrameClass TimeQuantum = null;
        public TimeRegionClass TimeRegion = null;

        public SearchConditionClass()
        {

        }
    }

    public class TimeFrameClass
    {
        public int BeginMonth;
        public int EndMonth;
        public int BeginDay;
        public int EndDay;

        public TimeFrameClass()
        { }
    }

    public class TimeRegionClass
    {
        public int BeginMonth;
        public int BeginDay;
        public int EndMonth;
        public int EndDay;

        public TimeRegionClass()
        { }
    }

    public class StatDimClass
    {
        public enumStatDimType DimType = enumStatDimType.不区分;
        public enumStatCompoundType CompoundType = enumStatCompoundType.全部;
        public enumStatDayMosaicType DayMosaicType = enumStatDayMosaicType.面积;

        public StatDimClass()
        { }

        public StatDimClass(enumStatDimType dimType, enumStatCompoundType compoundType, enumStatDayMosaicType dayMosaicType)
        {
            DimType = dimType;
            CompoundType = compoundType;
            DayMosaicType = dayMosaicType;
        }
    }

    public enum enumStatDimType
    {
        不区分,
        年,
        季,
        月,
        旬,
        日
    }

    public enum enumStatCompoundType
    {
        全部,
        最大,
        最小
    }

    public enum enumStatDayMosaicType
    {
        面积,
        空间合成
    }
}
