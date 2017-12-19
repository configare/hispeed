using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    public interface IGRIB2ProductDefinitionSection
    {
        int ProductDefinitionTemplateNo { get; }
        string ProductDefinitionTemplateName { get; }
        string TimeRangeUnitString { get; }
        int ParameterCategory { get; }
        int ParameterNumber { get; }
    }
}
