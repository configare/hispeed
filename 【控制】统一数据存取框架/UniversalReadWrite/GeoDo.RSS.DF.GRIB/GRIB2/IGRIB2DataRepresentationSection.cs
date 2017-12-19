using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    public interface IGRIB2DataRepresentationSection
    {
        GRIB2SectionHeader SectionHeader { get; }
        int DataTemplate { get; }
        float ReferenceValue { get; }
        int BinaryScaleFactor { get; }
        int DecimalScaleFactor { get; }
        int DataPoints { get; }
        int NumberOfBits { get; }
        float PrimaryMissingValue { get; }
        int MissingValueManagement { get; }
        int NumberOfGroups { get; }
        int BitsGroupWidths { get; }
        int ReferenceGroupLength { get; }
        int LengthIncrement { get; }
        int BitsScaledGroupLength { get; }
        int LengthLastGroup { get; }
        int OrderSpatial { get; }
        int DescriptorSpatial { get; }
    }
}
