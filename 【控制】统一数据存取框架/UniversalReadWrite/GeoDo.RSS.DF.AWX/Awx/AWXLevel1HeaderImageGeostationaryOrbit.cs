using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.AWX
{
    public class AWXLevel1HeaderImageGeostationaryOrbit : AWXLevel1HeaderImage
    {
        public override void Write(HdrFile hdr)
        {
            base.Write(hdr);
            Level2HeaderLength = 64;
            PaddingLength = (short)(hdr.Samples - Level1HeaderLength - Level2HeaderLength);
            FileHeaderOccupyRecordCount = 1;
            ProductDataOccupyRecordCount = (short)hdr.Lines;            
            ProductType = 1;
        }
    }
}
