using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.EC_GRIB
{
    internal class BDSClass
    {
        internal int BDSLength { get; set; }
        internal float BinaryScale { get; set; }
        internal float ReferenceValue { get; set; }
        internal object OriginData { get; set; }

        internal BDSClass(FileStream fs, BinaryReader br, int lonNum, int latNum)
        {
            if (fs == null || br == null)
                return;
            //01-03	Length in octets of binary data section
            BDSLength = MathHelper.Bytes2Int(br.ReadBytes(3));
            //04	Bits 1 through 4: Flag 
            //Bits 5 through 8: Number of unused bits at end of Section 4.
            byte flag = br.ReadByte();


            //05-06	The binary scale factor (E). A negative value is indicated by setting the high order bit (bit No. 1) in octet 5 to 1 (on).
            byte[] bsBytes = br.ReadBytes(2);
            if ((bsBytes[0] & 128) == 128)//首位为1，为负
            {
                bsBytes[0] &= 127;
                BinaryScale = -MathHelper.Bytes2Int(bsBytes);
            }
            else //首位为0，为正
                BinaryScale = MathHelper.Bytes2Int(bsBytes);
            // BinaryScale = MathHelper.Bytes2Int(br.ReadBytes(2));
            //07-10	Reference value (minimum value); floating point representation of the number.
            byte[] rbytes = br.ReadBytes(4);
            ReferenceValue = ComputeReferenceValue(rbytes);
            //11	Number of bits into which a datum point is packed
            br.ReadByte();
            //12-nnn	Variable, depending on octet 4; zero filled to an even number of octets.
            //14	Optionally, may contain an extension of the flags in octet 4.

            int pointsNum = lonNum * latNum;
            if ((flag & 32) == 32) //int
            {
                OriginData = new int[pointsNum];
                for (int i = 0; i < pointsNum; i++)
                    (OriginData as int[])[i] = MathHelper.Bytes2Int(br.ReadBytes(4));
            }
            else    //float
            {
                int num = BDSLength - 11;
                string result = string.Empty;
                for (int i = 0; i < num; i++)
                {
                    int value = Convert.ToInt16(br.ReadByte());
                    string p = Convert.ToString(value, 2);
                    if (p.Length < 8)
                    {
                        char[] a = new char[8 - p.Length];//{'0'};
                        for (int j = 0; j < a.Length; j++)
                            a[j] = '0';
                        p = string.Join("", a) + p;
                    }
                    result += p;
                }
                //12bit读一个数
                int pointCount = (num * 8) / 12;
                string[] parts = new string[pointCount];
                for (int i = 0; i < num * 8; i += 12)
                {
                    if (i / 12 < pointCount)
                        parts[i / 12] = result.Substring(i, 12);
                }
                float[] values = new float[pointCount];
                for (int i = 0; i < pointCount; i++)
                {
                    char[] chars = parts[i].ToCharArray();
                    if (chars.Length != 12)
                        continue;
                    values[i] = Convert.ToInt32(string.Join("", chars), 2);
                    //values[i] =chars[0] * 2048 + chars[1] * 1024 + chars[2] * 512 + chars[3] * 256 + chars[4] * 128
                    //    + chars[5] * 64 + chars[6] * 32 + chars[7] * 16 + chars[8] * 8 + chars[9] * 4 + chars[10] * 2 + chars[11];

                }
                OriginData = values;
            }
        }

        //计算eference value (R)
        //The reference value (R) uses IBM single precision floating point format.
        //sAAAAAAA BBBBBBBB BBBBBBBB BBBBBBBB
        //•	s = sign bit, encoded as 0 means positive, 1 means negative
        //•	A...A = 7-bit binary integer representing the exponent/characteristic
        //•	B...B = 24-bit binary integer, the mantissa.
        //The appropriate formula to recover the value of R is:
        //R = (-1)s * 2(-24) * B * 16(A-64)
        private float ComputeReferenceValue(byte[] rbytes)
        {
            int s, a, b;
            if ((rbytes[0] & 128) == 128)
                s = 1;
            else
                s = 0;
            rbytes[0] &= 127;
            a = Convert.ToInt32(rbytes[0]);
            b = rbytes[1] * (int)Math.Pow(16, 4) + rbytes[2] * 16 * 16 + rbytes[3];
            return (float)(Math.Pow(-1, s) * Math.Pow(2, -24) * b * Math.Pow(16, (a - 64)));
        }
    }
}
