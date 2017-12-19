using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.AWX
{
    public class AWXLevel1HeaderImage : AWXLevel1Header
    {
        public override void Write(HdrFile hdr)
        {
            base.Write(hdr);
            RecordLength = (short)hdr.Samples;
        }
    }
}
