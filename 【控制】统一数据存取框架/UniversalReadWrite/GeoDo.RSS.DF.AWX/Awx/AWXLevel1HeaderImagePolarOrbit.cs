using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.AWX
{
    public class AWXLevel1HeaderImagePolarOrbit : AWXLevel1HeaderImage
    {
        public override void Write(HdrFile hdr)
        {
            base.Write(hdr);
            Level2HeaderLength = 88;
            PaddingLength = (short)(hdr.Samples - Level1HeaderLength - Level2HeaderLength);
            FileHeaderOccupyRecordCount = 1;
            ProductType = 2;
        }
    }
}
