using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
    public class MemoryEstimateOfShapeFile
    {
        public float Estimate(string filename,out int featureCount,out int fieldCount )
        {
            featureCount = 0;
            fieldCount = 0;
            if (!filename.ToUpper().EndsWith(".SHP"))
                return 0;
            if (string.IsNullOrEmpty(filename))
                throw new NullReferenceException("new ShapeFilesReader(null)");
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);
            string mainFile = filename;
            string dir = Path.GetDirectoryName(filename);
            string shxFile = Path.Combine(dir, Path.GetFileNameWithoutExtension(filename) + ".shx");
            if (!File.Exists(shxFile))
                throw new FileNotFoundException(shxFile);
            string dbffile = Path.Combine(dir, Path.GetFileNameWithoutExtension(filename) + ".dbf");
            if (!File.Exists(dbffile))
                throw new FileNotFoundException(dbffile);
            string[] fields = ParseDbfToDataTable.GetFields(dbffile);
            fieldCount = fields != null ? fields.Length : 0;
            float memorySize = ComputeSizeAndCount(mainFile, shxFile,out featureCount);
            memorySize += fieldCount * 100; //100 bytes per fieldvalue + fieldName
            return memorySize / 1024f / 1024f;
        }

        private float ComputeSizeAndCount(string mainFile, string shxFile,out int featureCount)
        {
            featureCount = 0;
            int pointCount = 0;
            FileStream fsMainFile = null, fsShxFile = null;
            BinaryReader brMainFile = null, brShxFile = null;
            try
            {
                fsMainFile = new FileStream(mainFile, FileMode.Open, FileAccess.Read);
                brMainFile = new BinaryReader(fsMainFile);
                fsShxFile = new FileStream(shxFile, FileMode.Open, FileAccess.Read);
                brShxFile = new BinaryReader(fsShxFile);
                //
                int fileLength =0;
                long headerSize = GetHeaderSizeAndFileLength(fsMainFile,brMainFile,out fileLength);
                //
                featureCount = 0;
                fsMainFile.Seek(headerSize, SeekOrigin.Begin);
                while (fsMainFile.Position < fileLength)
                {
                    //Record header
                    int oid = ToLocalEndian.ToInt32FromBig(brMainFile.ReadBytes(4));
                    int contentSize = ToLocalEndian.ToInt32FromBig(brMainFile.ReadBytes(4));//16bit 字为单位
                    byte[] contentBytes = brMainFile.ReadBytes(contentSize * 2);
                    pointCount += GetPointCount(contentBytes, oid,fsMainFile);
                    //
                    featureCount++;
                }
                //
                float memorySize = pointCount * 8f * 2f; //x double, y double
                memorySize += featureCount * 100; //100 bytes per feature
                memorySize += 1024;//1024 bytes  per feature layer 
                return memorySize;
            }
            finally 
            {
                if (brMainFile != null)
                    brMainFile.Dispose();
                if (fsMainFile != null)
                    fsMainFile.Dispose();
                if (fsShxFile != null)
                    fsShxFile.Dispose();
                if (brShxFile != null)
                    brShxFile.Dispose();
            }
        }

        private int GetPointCount(byte[] contentBytes, int oid,FileStream fs)
        {
            using (MemoryStream ms = new MemoryStream(contentBytes))
            {
                using (BinaryReader br = new BinaryReader(ms))
                {
                    enumShapeType shapeType = (enumShapeType)ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
                    switch (shapeType)
                    {
                        case enumShapeType.NullShape:
                            break;
                        case enumShapeType.Point:
                            return GetPointCountByPoint(fs, br, oid);
                        case enumShapeType.MultiPoint:
                            return GetPointCountByMultiPoint(fs, br, oid);
                        case enumShapeType.Polyline:
                            return GetPointCountByPolyline(fs, br, oid);
                        case enumShapeType.PolylineM:
                            return GetPointCoungByPolylineM(fs, br, oid);
                        case enumShapeType.Polygon:
                            return GetPointCountByPolygon(fs, br, oid);
                        default:
                            return 0;
                    }
                    return 0;
                }
            }
        }

        private long GetHeaderSizeAndFileLength(FileStream fsMainFile, BinaryReader brMainFile,out int fileLength)
        {
            //FileCode
            int fileCode = ToLocalEndian.ToInt32FromBig(brMainFile.ReadBytes(4));
            if (fileCode != 9994)
                throw new Exception("文件不是ESRI Shape Files文件头！");
            //Skip 20 bytes unused (5 integer)
            brMainFile.ReadBytes(20);
            //File length
            fileLength = ToLocalEndian.ToInt32FromBig(brMainFile.ReadBytes(4)) * 2;
            if (fileLength != fsMainFile.Length)
                throw new Exception("ESRI Shape Files文件未正确结束！");
            //Version
            int Version = ToLocalEndian.ToInt32FromLittle(brMainFile.ReadBytes(4));
            //Shape Type
            int ShapeType = ToLocalEndian.ToInt32FromLittle(brMainFile.ReadBytes(4));
            //
            Envelope evp = new Envelope();
            evp.MinX = ToLocalEndian.ToDouble64FromLittle(brMainFile.ReadBytes(8));
            evp.MinY = ToLocalEndian.ToDouble64FromLittle(brMainFile.ReadBytes(8));
            evp.MaxX = ToLocalEndian.ToDouble64FromLittle(brMainFile.ReadBytes(8));
            evp.MaxY = ToLocalEndian.ToDouble64FromLittle(brMainFile.ReadBytes(8));
            //Skip minZ,maxZ,minM,maxM
            brMainFile.ReadBytes(8 * 4);
            return fsMainFile.Position;
        }

        private int GetPointCountByMultiPoint(FileStream fs, BinaryReader br, int oid)
        {
            int pointCount = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            //fs.Seek(pointCount * 2 * 8, SeekOrigin.Current);
            return pointCount;
        }

        private int GetPointCountByPoint(FileStream fs,BinaryReader br, int oid)
        {
            //fs.Seek(2 * 8, SeekOrigin.Current);
            return 1;
        }

        private int GetPointCountByPolyline(FileStream fs,BinaryReader br, int oid)
        {
            Envelope evp = new Envelope(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)));
            int nParts = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            int nPoints = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            //fs.Seek(nParts * 4, SeekOrigin.Current);
            //fs.Seek(nPoints * 2 * 8, SeekOrigin.Current);
            return nPoints;
        }

        private int GetPointCoungByPolylineM(FileStream fs,BinaryReader br, int oid)
        {
            Envelope evp = new Envelope(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)));
            int nParts = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            int nPoints = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            //fs.Seek(nParts * 4, SeekOrigin.Current);
            //fs.Seek(nPoints * 2 * 8, SeekOrigin.Current);
            //fs.Seek(2 * 8, SeekOrigin.Current);//M Range
            //fs.Seek(nPoints * 8, SeekOrigin.Current);//M Array
            return nPoints;
        }

        private int GetPointCountByPolygon(FileStream fs,BinaryReader br, int oid)
        {
            Envelope evp = new Envelope(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)));
            int nParts = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            int nPoints = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            //fs.Seek(nParts * 4, SeekOrigin.Current);
            //fs.Seek(nPoints * 2 * 8, SeekOrigin.Current);
            return nPoints;
        }
    }
}
