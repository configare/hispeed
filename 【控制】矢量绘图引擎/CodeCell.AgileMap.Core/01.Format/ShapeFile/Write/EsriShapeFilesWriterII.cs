using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeCell.Bricks.Runtime;

namespace CodeCell.AgileMap.Core
{
    public class EsriShapeFilesWriterII:IEsriShapeFilesWriter
    {
        private enumShapeType _shapeType;
        private string _mainFileName = null;
        private string _shxFileName = null;
        private string _dbfFileName = null;
        private Envelope _currentEnvelope = null;
        private int _currentShpRecondLength = 0;    
        private int _currentShxRecondLength = 0;
        private long _currentShpRecondPosition = 0;   //记录下次写时偏移位置
        private long _currentShxRecondPosition = 0;
        private int _featuresCount = 0;
        private int _offset = 50;
        private FileStream _fsMainFile = null;
        private BinaryWriter _bwMainFile = null;
        private FileStream _fsShxFile = null;
        private BinaryWriter _bwShxFile = null;
        private DbfWriterII _bwDbfFile = null;
        private bool _isFirstWrite = true;

        public EsriShapeFilesWriterII(string outFilename, enumShapeType shape)
        {
            try
            {
                if (Path.GetExtension(outFilename).ToUpper()!=".SHP")
                {
                    throw new Exception("请输入shp文件格式的文件名！");
                }
                _shapeType = shape;
                _mainFileName = outFilename;
                string dir = Path.GetDirectoryName(outFilename);
                string fname = Path.GetFileNameWithoutExtension(outFilename);
                _shxFileName = Path.Combine(dir, fname + ".shx");
                _dbfFileName = Path.Combine(dir, fname + ".dbf");
                if (File.Exists(_mainFileName))
                {
                    File.Delete(_mainFileName);
                }
                if (File.Exists(_shxFileName))
                {
                    File.Delete(_shxFileName);
                }
            }
            catch (Exception ex)
            {
                Log.WriterError(ex.Message);
            }
        }

        #region IEsriShapeFilesWriter Members

        public void BeginWrite()
        {
            if (string.IsNullOrEmpty(_mainFileName) || string.IsNullOrEmpty(_shxFileName) || string.IsNullOrEmpty(_dbfFileName))
                return;
            _fsMainFile = new FileStream(_mainFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _bwMainFile = new BinaryWriter(_fsMainFile);
            _fsShxFile = new FileStream(_shxFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            _bwShxFile = new BinaryWriter(_fsShxFile);
            _bwDbfFile = new DbfWriterII(_dbfFileName);
            _bwDbfFile.BeginWrite();
        }

        public void Write(Feature[] features)
        {
            Write(features, null);
        }

        public void Write(Feature[] features, Action<int, string> progress)
        {
            if (features == null || features.Length == 0)
                return;
            // write main file and shx file
            WriteVectorDataToFiles(features);
            _bwDbfFile.Write(features, progress);
            _featuresCount += features.Length;
        }

        public void EndWriter()
        {
            if (_fsMainFile != null)
                _fsMainFile.Close();
            if (_bwMainFile != null)
                _bwMainFile.Close();
            if (_fsShxFile != null)
                _fsShxFile.Close();
            if (_bwShxFile != null)
                _bwShxFile.Close();
            if (_bwDbfFile != null)
                _bwDbfFile.EndWrite();
        }

        #endregion

        private void WriteVectorDataToFiles(Feature[] features)
        {
            //write main file and shx file header
            WriteFileHeader(features);
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
            _currentShpRecondLength += TmpLength;
            return _currentShpRecondLength + 100;
        }

        private Int32 CalculateShxfileLenthInByte(Feature[] vf)
        {
            _currentShxRecondLength += vf.Length * 8;
            return _currentShxRecondLength + 100;
        }

        private void WriteShapePointRecords(Feature[] vfs)
        {
            //the fist record offset in 16-bit word
            byte[] byteArray;
            ShapePoint point;
            if (_currentShxRecondPosition != 0)
                _fsShxFile.Seek(_currentShxRecondPosition, SeekOrigin.Begin);
            if (_currentShpRecondPosition != 0)
                _fsMainFile.Seek(_currentShpRecondPosition, SeekOrigin.Begin);
            for (int i = 0; i < vfs.Length; i++)
            {
                point = vfs[i].Geometry as ShapePoint;

                byteArray = BitConverter.GetBytes(_featuresCount + i + 1);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                byteArray = BitConverter.GetBytes(10);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                byteArray = BitConverter.GetBytes(_offset);
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                _offset += 14;

                byteArray = BitConverter.GetBytes((int)_shapeType);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(point.X);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));

                byteArray = BitConverter.GetBytes(point.Y);
                _bwMainFile.Write(ToLocalEndian.ToInt64FromLittle(byteArray));
            }
            _currentShpRecondPosition = _fsMainFile.Position;
            _currentShxRecondPosition = _fsShxFile.Position;
        }

        private void WriteShapeMultiPointRecords(Feature[] vfs)
        {
            //the fist record offset in 16-bit word
            byte[] byteArray;
            ShapeMultiPoint mulPoint;
            Int32 contentLength;
            if (_currentShxRecondPosition != 0)
                _fsShxFile.Seek(_currentShxRecondPosition, SeekOrigin.Begin);
            if (_currentShpRecondPosition != 0)
                _fsMainFile.Seek(_currentShpRecondPosition, SeekOrigin.Begin);
            for (int i = 0; i < vfs.Length; i++)
            {
                mulPoint = vfs[i].Geometry as ShapeMultiPoint;
                contentLength = (mulPoint.Points.Length * 16 + 48) / 2;

                byteArray = BitConverter.GetBytes(_featuresCount + i + 1);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                byteArray = BitConverter.GetBytes(contentLength);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                byteArray = BitConverter.GetBytes(_offset);
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                _offset += contentLength;

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
            _currentShpRecondPosition = _fsMainFile.Position;
            _currentShxRecondPosition = _fsShxFile.Position;
        }

        private void WriteShapePolylineRecords(Feature[] vfs)
        {
            byte[] byteArray;
            ShapePolyline polyLine;
            Int32 contentLength;
            if(_currentShxRecondPosition!=0)
                _fsShxFile.Seek(_currentShxRecondPosition, SeekOrigin.Begin);
            if (_currentShpRecondPosition != 0)
                _fsMainFile.Seek(_currentShpRecondPosition, SeekOrigin.Begin);
            for (int i = 0; i < vfs.Length; i++)
            {
                polyLine = vfs[i].Geometry as ShapePolyline;
                contentLength = (44 + polyLine.Parts.Length * 4) / 2;
                int pointsNum = 0;
                int index = 0;
                foreach (ShapeLineString part in polyLine.Parts)
                {
                    contentLength += part.Points.Length * 8;
                    pointsNum += part.Points.Length;
                }
                byteArray = BitConverter.GetBytes(_featuresCount + i + 1);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                byteArray = BitConverter.GetBytes(_offset);
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                _offset += contentLength + 4;

                byteArray = BitConverter.GetBytes(contentLength);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                _bwMainFile.Write((int)_shapeType);

                _bwMainFile.Write(polyLine.Envelope.MinX);

                _bwMainFile.Write(polyLine.Envelope.MinY);

                _bwMainFile.Write(polyLine.Envelope.MaxX);

                _bwMainFile.Write(polyLine.Envelope.MaxY);

                _bwMainFile.Write(polyLine.Parts.Length);

                _bwMainFile.Write(pointsNum);

                foreach (ShapeLineString part in polyLine.Parts)
                {
                    _bwMainFile.Write(index);
                    index += part.Points.Length;
                }
                foreach (ShapeLineString part in polyLine.Parts)
                {
                    foreach (ShapePoint sp in part.Points)
                    {
                        _bwMainFile.Write(sp.X);
                        _bwMainFile.Write(sp.Y);
                    }
                }
            }
            _currentShpRecondPosition = _fsMainFile.Position;
            _currentShxRecondPosition = _fsShxFile.Position;
        }

        private void WriteShapePolygonRecords(Feature[] vfs)
        {
            //the fist record offset in 16-bit word
            byte[] byteArray;
            ShapePolygon polygon;
            Int32 contentLength;
            Int32 pointsNum = 0;
            Int32 index = 0;
            if (_currentShxRecondPosition != 0)
                _fsShxFile.Seek(_currentShxRecondPosition, SeekOrigin.Begin);
            if (_currentShpRecondPosition != 0)
                _fsMainFile.Seek(_currentShpRecondPosition, SeekOrigin.Begin);
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
                byteArray = BitConverter.GetBytes(_featuresCount + i + 1);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                byteArray = BitConverter.GetBytes(_offset);
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                _offset += contentLength + 4;

                byteArray = BitConverter.GetBytes(contentLength);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));

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
            _currentShpRecondPosition = _fsMainFile.Position;
            _currentShxRecondPosition = _fsShxFile.Position;
        }

        private void WriteFileHeader(Feature[] features)
        {
            if (_isFirstWrite)
            {
                byte[] byteArray;
                Envelope env = features[0].Geometry.Envelope;
                for (int i = 1; i < features.Length; i++)
                    env.UnionWith(features[i].Geometry.Envelope);
                _currentEnvelope = env;

                byteArray = BitConverter.GetBytes(9994);
                int byteArrayValue = ToLocalEndian.ToInt32FromBig(byteArray);
                _bwMainFile.Write(byteArrayValue);
                _bwShxFile.Write(byteArrayValue);

                byteArray = new byte[20];
                _bwMainFile.Write(byteArray);
                _bwShxFile.Write(byteArray);
                //
                byteArray = BitConverter.GetBytes(CalculateMainfileLengthInByte(features) / 2);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                byteArray = BitConverter.GetBytes(CalculateShxfileLenthInByte(features) / 2);
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                //
                _bwMainFile.Write(1000);
                _bwShxFile.Write(1000);

                _bwMainFile.Write((int)_shapeType);
                _bwShxFile.Write((int)_shapeType);

                byteArray = BitConverter.GetBytes(env.MinX);
                _bwMainFile.Write(byteArray);
                _bwShxFile.Write(byteArray);

                byteArray = BitConverter.GetBytes(env.MinY);
                _bwMainFile.Write(byteArray);
                _bwShxFile.Write(byteArray);

                byteArray = BitConverter.GetBytes(env.MaxX);
                _bwMainFile.Write(byteArray);
                _bwShxFile.Write(byteArray);

                byteArray = BitConverter.GetBytes(env.MaxY);
                _bwMainFile.Write(byteArray);
                _bwShxFile.Write(byteArray);

                byteArray = new byte[32];
                _bwMainFile.Write(byteArray);
                _bwShxFile.Write(byteArray);
                _isFirstWrite = false;
            }
            else
            {
                byte[] byteArray;
                Envelope env = features[0].Geometry.Envelope;
                for (int i = 1; i < features.Length; i++)
                    env.UnionWith(features[i].Geometry.Envelope);
                if (_currentEnvelope != null)
                    env.UnionWith(_currentEnvelope);
                _currentEnvelope = env;
                _fsMainFile.Seek(24, SeekOrigin.Begin);
                _fsShxFile.Seek(24, SeekOrigin.Begin);

                byteArray = BitConverter.GetBytes(CalculateMainfileLengthInByte(features) / 2);
                _bwMainFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                byteArray = BitConverter.GetBytes(CalculateShxfileLenthInByte(features) / 2);
                _bwShxFile.Write(ToLocalEndian.ToInt32FromBig(byteArray));
                ////
                _fsMainFile.Seek(8, SeekOrigin.Current);
                _fsShxFile.Seek(8, SeekOrigin.Current);

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
            }
        }
    }
}
