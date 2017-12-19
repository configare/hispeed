using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
    public class EsriShapeFilesWriter
    {
        private FileStream _fsMainFile = null;
        private BinaryWriter _bwMainFile = null;
        private FileStream _fsShxFile = null;
        private BinaryWriter _bwShxFile = null;
        private DbfWriter _bwDbfFile = null;
        private enumShapeType _shapeType;

        public EsriShapeFilesWriter(string outFilename, Feature[] features, enumShapeType shape)
        {
            //检查输入是否合法
            string dir = Path.GetDirectoryName(outFilename);
            string mainFilename = outFilename;
            string shxFilename = Path.Combine(dir, Path.GetFileNameWithoutExtension(outFilename) + ".shx");
            string dbfFilename = Path.Combine(dir, Path.GetFileNameWithoutExtension(outFilename) + ".dbf");
            try
            {
                if (!outFilename.Contains(".shp"))
                {
                    throw new Exception("请输入shp文件格式的文件名！");
                }
                if (File.Exists(mainFilename))
                {
                    File.Delete(mainFilename);
                }
                if (File.Exists(shxFilename))
                {
                    File.Delete(shxFilename);
                }
                _shapeType = shape;
                //创建文件
                _fsMainFile = new FileStream(mainFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                _bwMainFile = new BinaryWriter(_fsMainFile);
                _fsShxFile = new FileStream(shxFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                _bwShxFile = new BinaryWriter(_fsShxFile);
                /*
                 * River.shp
                 * River.shx
                 * River.dbf
                 * 1、shp文件，几何形状的文件
                 * 2、shx文件，索引文件
                 * 3、dbf文件，属性数据文件
                 * 4、prj文件，坐标系统
                 */
                //write dbf file
                _bwDbfFile = new DbfWriter(dbfFilename, features);
                // write main file and shx file
                WriteVectorDataToFiles(features);
                WriteDbfEncode(dbfFilename);
            }
            catch (Exception ex)
            {
                Log.WriterError(ex.Message);
            }
        }

        private void WriteDbfEncode(string dbfFilename)
        {
            using (FileStream fs = new FileStream(dbfFilename, FileMode.Open, FileAccess.ReadWrite))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    fs.Seek(29, SeekOrigin.Begin);
                    fs.WriteByte(77);
                }
            }
        }


        private void WriteVectorDataToFiles(Feature[] features)
        {
            byte[] byteArray;

            Envelope env = new Envelope(double.MaxValue, double.MaxValue, double.MinValue, double.MinValue);

            foreach (Feature vf in features)
            {
                env.UnionWith(vf.Geometry.Envelope);
            }

            ////write main file
            //write main file and shx file header
            byteArray = BitConverter.GetBytes(9994);
            _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
            _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

            byteArray = new byte[20];
            _bwMainFile.Write(byteArray);
            _bwShxFile.Write(byteArray);

            ////
            byteArray = BitConverter.GetBytes(CalculateMainfileLengthInByte(features) / 2);
            _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
            byteArray = BitConverter.GetBytes(CalculateShxfileLenthInByte(features) / 2);
            _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
            ////

            byteArray = BitConverter.GetBytes(1000);
            _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));
            _bwShxFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

            byteArray = BitConverter.GetBytes((int)_shapeType);
            _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));
            _bwShxFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));


            byteArray = BitConverter.GetBytes(env.MinX);
            _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));
            _bwShxFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

            byteArray = BitConverter.GetBytes(env.MinY);
            _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));
            _bwShxFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

            byteArray = BitConverter.GetBytes(env.MaxX);
            _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));
            _bwShxFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

            byteArray = BitConverter.GetBytes(env.MaxY);
            _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));
            _bwShxFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

            byteArray = new byte[32];
            _bwMainFile.Write(byteArray);
            _bwShxFile.Write(byteArray);

            //write main file and Shx file records

            switch (_shapeType)
            {
                case enumShapeType.Point:
                    WriteShapePointRecords(features);
                    break;
                case enumShapeType.MultiPoint:
                    WriteShapeMultiPointRecords(features);
                    break;
                case enumShapeType.Polyline:
                    WriteShapePolylineRecords(features);
                    break;
                case enumShapeType.Polygon:
                    WriteShapePolygonRecords(features);
                    break;
                default:
                    break;

            }


            ////write prj file,leave it now,no spec

            CloseVectorDataFiles();
        }

        private void WriteShapePointRecords(Feature[] vfs)
        {
            //the fist record offset in 16-bit word
            Int32 Offset = 50;
            byte[] byteArray;
            ShapePoint point;


            for (int i = 0; i < vfs.Length; i++)
            {
                point = vfs[i].Geometry as ShapePoint;

                byteArray = BitConverter.GetBytes(i + 1);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                byteArray = BitConverter.GetBytes(14);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                ////
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                byteArray = BitConverter.GetBytes(Offset);
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                Offset += 14;
                ////
                byteArray = BitConverter.GetBytes((int)_shapeType);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(point.X);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(point.Y);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

            }

        }
        private void WriteShapeMultiPointRecords(Feature[] vfs)
        {
            //the fist record offset in 16-bit word
            Int32 Offset = 50;
            byte[] byteArray;
            ShapeMultiPoint mulPoint;
            Int32 contentLength;

            for (int i = 0; i < vfs.Length; i++)
            {

                mulPoint = vfs[i].Geometry as ShapeMultiPoint;
                contentLength = (mulPoint.Points.Length * 16 + 48) / 2;

                byteArray = BitConverter.GetBytes(i + 1);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                byteArray = BitConverter.GetBytes(contentLength);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                ////
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                byteArray = BitConverter.GetBytes(Offset);
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                Offset += contentLength;
                ////

                byteArray = BitConverter.GetBytes((int)_shapeType);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(mulPoint.Envelope.MinX);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(mulPoint.Envelope.MinY);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(mulPoint.Envelope.MaxX);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(mulPoint.Envelope.MaxY);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(mulPoint.Points.Length);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

                foreach (ShapePoint sp in mulPoint.Points)
                {
                    byteArray = BitConverter.GetBytes(sp.X);
                    _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                    byteArray = BitConverter.GetBytes(sp.Y);
                    _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));
                }

            }

        }
        private void WriteShapePolylineRecords(Feature[] vfs)
        {
            //the fist record offset in 16-bit word
            Int32 Offset = 50;
            byte[] byteArray;
            ShapePolyline polyLine;
            Int32 contentLength;
            Int32 pointsNum = 0;
            Int32 index = 0;

            for (int i = 0; i < vfs.Length; i++)
            {
                polyLine = vfs[i].Geometry as ShapePolyline;

                contentLength = (44 + polyLine.Parts.Length * 4) / 2;
                pointsNum = 0;
                index = 0;
                foreach (ShapeLineString part in polyLine.Parts)
                {
                    contentLength += part.Points.Length * 8;
                    pointsNum += part.Points.Length;
                }

                byteArray = BitConverter.GetBytes(i + 1);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                ////
                byteArray = BitConverter.GetBytes(Offset);
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                Offset += contentLength + 4;
                ////

                byteArray = BitConverter.GetBytes(contentLength);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                ////
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                ////

                byteArray = BitConverter.GetBytes((int)_shapeType);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(polyLine.Envelope.MinX);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(polyLine.Envelope.MinY);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(polyLine.Envelope.MaxX);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(polyLine.Envelope.MaxY);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(polyLine.Parts.Length);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(pointsNum);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

                foreach (ShapeLineString part in polyLine.Parts)
                {
                    byteArray = BitConverter.GetBytes(index);
                    _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));
                    index += part.Points.Length;
                }
                foreach (ShapeLineString part in polyLine.Parts)
                {
                    foreach (ShapePoint sp in part.Points)
                    {
                        byteArray = BitConverter.GetBytes(sp.X);
                        _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                        byteArray = BitConverter.GetBytes(sp.Y);
                        _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));
                    }
                }



            }

        }
        private void WriteShapePolygonRecords(Feature[] vfs)
        {
            //the fist record offset in 16-bit word
            Int32 Offset = 50;
            byte[] byteArray;
            ShapePolygon polygon;
            Int32 contentLength;
            Int32 pointsNum = 0;
            Int32 index = 0;

            for (int i = 0; i < vfs.Length; i++)
            {
                polygon = vfs[i].Geometry as ShapePolygon;

                pointsNum = 0;
                index = 0;
                contentLength = (44 + polygon.Rings.Length * 4) / 2;
                foreach (ShapeRing part in polygon.Rings)
                {
                    contentLength += part.Points.Length * 8;
                    pointsNum += part.Points.Length;
                }



                byteArray = BitConverter.GetBytes(i + 1);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                ////
                byteArray = BitConverter.GetBytes(Offset);
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                Offset += contentLength + 4;
                ////


                byteArray = BitConverter.GetBytes(contentLength);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                ////
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                ////

                byteArray = BitConverter.GetBytes((int)_shapeType);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(polygon.Envelope.MinX);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(polygon.Envelope.MinY);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(polygon.Envelope.MaxX);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(polygon.Envelope.MaxY);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(polygon.Rings.Length);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(pointsNum);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

                foreach (ShapeRing sr in polygon.Rings)
                {
                    byteArray = BitConverter.GetBytes(index);
                    _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));
                    index += sr.Points.Length;
                }
                foreach (ShapeRing sr in polygon.Rings)
                {
                    foreach (ShapePoint sp in sr.Points)
                    {
                        byteArray = BitConverter.GetBytes(sp.X);
                        _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                        byteArray = BitConverter.GetBytes(sp.Y);
                        _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));
                    }
                }

            }

        }

        private Int32 CalculateMainfileLengthInByte(Feature[] vf)
        {
            Int32 TmpLength = 0;

            if (vf[0].Geometry is ShapePoint)
            {
                TmpLength += 28 * vf.Length;

            }
            else if (vf[0].Geometry is ShapeMultiPoint)
            {

                foreach (Feature tvf in vf)
                {
                    TmpLength += 48 + (tvf.Geometry as ShapeMultiPoint).Points.Length * 16;
                }

            }
            else if (vf[0].Geometry is ShapePolyline)
            {
                //points?
                ShapePolyline polyLine = null;
                foreach (Feature tvf in vf)
                {
                    polyLine = tvf.Geometry as ShapePolyline;
                    TmpLength += 52 + polyLine.Parts.Length * 4;
                    foreach (ShapeLineString pt in polyLine.Parts)
                    {
                        TmpLength += pt.Points.Length * 16;
                    }
                }
            }
            else if (vf[0].Geometry is ShapePolygon)
            {
                ShapePolygon polygon = null;
                foreach (Feature tvf in vf)
                {
                    polygon = tvf.Geometry as ShapePolygon;
                    TmpLength += 52 + polygon.Rings.Length * 4;
                    foreach (ShapeRing sr in polygon.Rings)
                    {
                        TmpLength += sr.Points.Length * 16;
                    }
                }
            }

            return TmpLength + 100;

        }

        private Int32 CalculateShxfileLenthInByte(Feature[] vf)
        {
            return vf.Length * 8 + 100;
        }

        private bool CloseVectorDataFiles()
        {
            if (_fsMainFile != null)
                _fsMainFile.Close();
            if (_bwMainFile != null)
                _bwMainFile.Close();
            if (_fsShxFile != null)
                _fsShxFile.Close();
            if (_bwShxFile != null)
                _bwShxFile.Close();
            return true;
        }

        private bool CheckInputValidity()
        {
            return true;
        }


    }
}
