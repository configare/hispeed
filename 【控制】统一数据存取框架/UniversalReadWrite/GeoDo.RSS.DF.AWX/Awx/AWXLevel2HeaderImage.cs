using System.IO;
using GeoDo.RSS.Core.DF;
using GeoDo.RSS.MIF.Core;

namespace GeoDo.RSS.DF.AWX
{
    public abstract class AWXLevel2HeaderImage : AWXLevel2Header
    {
        public override void Write(HdrFile hdr, RasterIdentify id)
        {

            char[] satchar = new char[8]; 
            char[] arr=id.Satellite.ToCharArray();
            if (arr.Length <=8)
            {
                for (int i = 0; i < arr.Length;i++ )
                    satchar[i] = arr[i];
            }
            else
            {
                for (int i = 0; i < 8; i++)
                    satchar[i] = arr[i];
            }
            Satellite = new string(satchar);//id.Satellite.ToCharArray()//string.Format("{0, -8}",id.Satellite.ToString())
            ImageWidth = (short)hdr.Samples;
            ImageHeight = (short)hdr.Lines;
            Reserve = (short)hdr.DataType;
        }
    }
}
