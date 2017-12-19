using System;
using System.IO;

namespace GeoDo.RSS.DF.AWX
{
    public class AWXProduct
    {
        public byte[] Data { get; set; }

        public void Read(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                Data = br.ReadBytes(Convert.ToInt32(stream.Length));
            }
        }
    }
}
