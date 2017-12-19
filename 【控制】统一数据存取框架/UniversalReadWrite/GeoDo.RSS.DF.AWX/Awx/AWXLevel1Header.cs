using System.IO;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.RSS.DF.AWX
{
    public class AWXLevel1Header
    {
        public string Sat96Filename { get; set; }
        /// <summary>
        /// 整型数的字节顺序
        /// 0：按INTEL方式排列,即低字节在前，高字节在后。
        /// ~=0：按MOTOROLA方式排列，即高字节在前，低字节在后。
        /// </summary>
        public short Endian { get; set; }
        public short Level1HeaderLength { get; set; }
        public short Level2HeaderLength { get; set; }
        public short PaddingLength { get; set; }
        public short RecordLength { get; set; }
        public short FileHeaderOccupyRecordCount { get; set; }
        public short ProductDataOccupyRecordCount { get; set; }
        /// <summary>
        /// 产品类别
        /// 0：未定义类型的产品；1：静止气象卫星图象产品；2：极轨气象卫星图象产品；3：格点场定量产品；4：离散场定量产品；5：图形和分析产品；
        /// </summary>
        public short ProductType { get; set; }
        public short CompressType { get; set; }
        public string Format { get; set; }
        public short ProductDataQualityMark { get; set; }

        public void Read(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                Sat96Filename = Encoding.ASCII.GetString(br.ReadBytes(12));
                Endian = br.ReadInt16();
                Level1HeaderLength = br.ReadInt16();
                Level2HeaderLength = br.ReadInt16();
                PaddingLength = br.ReadInt16();
                RecordLength = br.ReadInt16();
                FileHeaderOccupyRecordCount = br.ReadInt16();
                ProductDataOccupyRecordCount = br.ReadInt16();
                ProductType = br.ReadInt16();
                CompressType = br.ReadInt16();
                Format = Encoding.ASCII.GetString(br.ReadBytes(8));
                ProductDataQualityMark = br.ReadInt16();
            }
        }

        public virtual void Write(HdrFile hdr)
        {
            Sat96Filename = new string(new char[12]{'1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c'}); // 
            Endian = (hdr.ByteOrder == enumHdrByteOder.Host_intel) ? (short)0 : (short)1;
            Level1HeaderLength = 40;
            Format = new string(new char[8]{'S', 'A', 'T', '2', '0', '0', '4', '\0'});//
        }

        public void WriteFile(string hdr)
        {
            using (FileStream stream = new FileStream(Path.ChangeExtension(hdr, "AWX"), FileMode.Append))
            {
                using (StreamWriter bw = new StreamWriter(stream))
                {
                    bw.Write(Sat96Filename);
                }
            }
            using (FileStream stream = new FileStream(Path.ChangeExtension(hdr, "AWX"), FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(Endian);
                    bw.Write(Level1HeaderLength);
                    bw.Write(Level2HeaderLength);
                    bw.Write(PaddingLength);
                    bw.Write(RecordLength);
                    bw.Write(FileHeaderOccupyRecordCount);
                    bw.Write(ProductDataOccupyRecordCount);
                    bw.Write(ProductType);
                    bw.Write(CompressType);
                }
            }
            using (FileStream stream = new FileStream(Path.ChangeExtension(hdr, "AWX"), FileMode.Append))
            {
                using (StreamWriter sw = new StreamWriter(stream))
                {
                    sw.Write(Format);
                }
            }

            using (FileStream stream = new FileStream(Path.ChangeExtension(hdr, "AWX"), FileMode.Append))
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    bw.Write(ProductDataQualityMark);
                }
            }
        }
    }
}
