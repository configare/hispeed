using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GeoDo.RSS.RasterTools
{
    public class ContourPersist : IContourPersisit
    {
        public enum enumCoordType : byte
        {
            Raster, //0
            Geo,    //1
            Prj     //2
        }

        public static char[] FILE_IDENTIFY = new char[] { 'G', 'E', 'O', 'D', 'O', 'C', 'O', 'N', 'T', 'O', 'U', 'R' }; //GeoDoContour
        private const int SIZE_OF_DOUBLE = 8;
        private const int SIZE_OF_POINTF = 8;
        private const int SIZE_OF_INT = 4;
        private const int SIZE_OF_FLOAT = 4;
        private const int SIZE_OF_ENVELOPE = 4 * SIZE_OF_FLOAT;

        public ContourPersist()
        { 
        }

        public static bool FileIdentifyIsOK(char[] fileIdentify)
        {
            if (fileIdentify == null || fileIdentify.Length == 0 || fileIdentify.Length != FILE_IDENTIFY.Length)
                return false;
            for (int i = 0; i < fileIdentify.Length; i++)
                if (fileIdentify[i] != FILE_IDENTIFY[i])
                    return false;
            return true;
        }

        public unsafe void Write(ContourLine[] cntLines, ContourPersist.enumCoordType coordType, ContourLine.ContourEnvelope envelope, string spatialRef, string fname)
        {
            if (cntLines == null || cntLines.Length == 0)
                return;
            int cntCount = cntLines.Length;
            byte version = 0;
            using (FileStream fs = new FileStream(fname, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs, Encoding.Unicode))
                {
                    bw.Write(FILE_IDENTIFY);
                    bw.Write(version);
                    bw.Write(cntCount);
                    bw.Write((byte)coordType);
                    //envelope
                    WriteEnvelope(bw, envelope);
                    //spatial ref
                    char[] spRefChars;
                    int spLen = GetSpatialRefLen(spatialRef, out spRefChars);
                    bw.Write(spLen);
                    if (spRefChars != null && spRefChars.Length > 0)
                        bw.Write(spRefChars);
                    //contourValue
                    long offset = fs.Position;
                    for (int i = 0; i < cntCount; i++,offset += SIZE_OF_DOUBLE)
                        bw.Write(cntLines[i].ContourValue);
                    //index
                    int cntLineHeader = SIZE_OF_ENVELOPE + SIZE_OF_INT + SIZE_OF_INT;//外包矩形+ 分类序号 + 点数
                    for (int i = 0; i < cntCount; i++)
                    {
                        bw.Write(offset);
                        offset += (cntLineHeader + cntLines[i].Count * SIZE_OF_POINTF);
                    }
                    //contour lines
                    ContourLine crtLine;
                    for (int i = 0; i < cntCount; i++)
                    {
                        crtLine = cntLines[i];
                        WriteEnvelope(bw, crtLine.Envelope);
                        bw.Write(crtLine.ClassIndex);
                        bw.Write(crtLine.Count);
                        fixed (PointF* ptr0 = crtLine.Points)
                        {
                            PointF* ptr = ptr0;
                            int ptCount = crtLine.Count;
                            for (int k = 0; k < ptCount; k++,ptr ++)
                            {
                                bw.Write(ptr->X);
                                bw.Write(ptr->Y);
                            }
                        }
                    }
                }
            }
        }

        private void WriteEnvelope(BinaryWriter bw, ContourLine.ContourEnvelope envelope)
        {
            if (envelope == null)
                envelope = new ContourLine.ContourEnvelope();
            bw.Write(envelope.MinX);
            bw.Write(envelope.MinY);
            bw.Write(envelope.MaxX);
            bw.Write(envelope.MaxY);
        }

        private int GetSpatialRefLen(string spatialRef, out char[] spRefChars)
        {
            spRefChars = null;
            if (spatialRef == null)
                return 0;
            spRefChars = spatialRef.ToCharArray();
            return spRefChars.Length;
        }

        public ContourLine[] Read(string fname, out ContourPersist.enumCoordType coordType, out ContourLine.ContourEnvelope envelope, out string spatialRef)
        {
            coordType = 0;
            envelope = null;
            spatialRef = null;
            if (string.IsNullOrEmpty(fname) || !File.Exists(fname)) 
                return null;
            using (FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs, Encoding.Unicode))
                {
                    char[] fileId = br.ReadChars(FILE_IDENTIFY.Length);
                    FileIdentifyIsError(fileId);
                    byte version = br.ReadByte();
                    if (version == 0)
                        return ReadVersion0(fs, br,out coordType, out envelope, out spatialRef);
                }
            }
            throw new NotSupportedException("等值线文件版本号错误！");
        }

        private ContourLine[] ReadVersion0(FileStream fs, BinaryReader br, out ContourPersist.enumCoordType coordType, out ContourLine.ContourEnvelope envelope, out string spatialRef)
        {
            int cntCount = br.ReadInt32();
            coordType = (enumCoordType)br.ReadByte();
            envelope = ReadEnvelope(br);
            int spLen = br.ReadInt32();
            spatialRef = ReadSpatialRef(br, spLen);
            double[] contourValues = ReadContourValues(br, cntCount);
            long[] offsets = ReadOffsets(br, cntCount);
            List<ContourLine> cntLines = new List<ContourLine>(cntCount);
            for (int i = 0; i < cntCount; i++)
            {
                ContourLine cntLine = ReadCntLine(br,contourValues[i]);
                cntLines.Add(cntLine);
            }
            return cntLines.Count > 0 ? cntLines.ToArray() : null;
        }

        private unsafe ContourLine ReadCntLine(BinaryReader br,double contourValue)
        {
            ContourLine.ContourEnvelope evp = ReadEnvelope(br);
            ContourLine cntLine = new ContourLine(contourValue);
            cntLine.ClassIndex = br.ReadInt32();
            int ptCount = br.ReadInt32();
            PointF[] pts = new PointF[ptCount];
            fixed (PointF* ptr0 = pts)
            {
                PointF* ptr = ptr0;
                for (int i = 0; i < ptCount; i++, ptr++)
                {
                    ptr->X = br.ReadSingle();
                    ptr->Y = br.ReadSingle();
                }
            }
            cntLine.AddPoints(pts);
            return cntLine;
        }

        private long[] ReadOffsets(BinaryReader br, int cntCount)
        {
            long[] offsets = new long[cntCount];
            for (int i = 0; i < cntCount; i++)
                offsets[i] = br.ReadInt64();
            return offsets;
        }

        private double[] ReadContourValues(BinaryReader br, int cntCount)
        {
            double[] values = new double[cntCount];
            for (int i = 0; i < cntCount; i++)
                values[i] = br.ReadDouble();
            return values;
        }

        private string ReadSpatialRef(BinaryReader br, int spLen)
        {
            if (spLen == 0)
                return null;
            char[] chars = br.ReadChars(spLen);
            string str = null;
            for (int i = 0; i < spLen; i++)
                str += chars[i];
            return str;
        }

        private ContourLine.ContourEnvelope ReadEnvelope(BinaryReader br)
        {
            float minX = br.ReadSingle();
            float minY = br.ReadSingle();
            float maxX = br.ReadSingle();
            float maxY = br.ReadSingle();
            return new ContourLine.ContourEnvelope(minX, maxX, minY, maxY);
        }

        private static void FileIdentifyIsError(char[] fileId)
        {
            for (int i = 0; i < fileId.Length; i++)
                if (fileId[i] != FILE_IDENTIFY[i])
                    throw new ArgumentException("无效的等值线文件格式!");
        }
    }
}
