using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Core
{
    internal class EsriShapeFilesFeatureReader : EsriShapeFilesReader
    {
        public EsriShapeFilesFeatureReader()
        {
        }

        protected override object ConstructFeature(BinaryReader br, int oid)
        {
            enumShapeType shapeType = (enumShapeType)ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            switch (shapeType)
            {
                case enumShapeType.NullShape:
                    break;
                case enumShapeType.Point:
                    return ConstructPoint(br, oid);
                case enumShapeType.MultiPoint:
                    return ConstructMultiPoint(br,oid);
                case enumShapeType.Polyline:
                    return ConstructPolyline(br, oid);
                case enumShapeType.PolylineM:
                    return ConstructPolylineM(br, oid);
                case enumShapeType.Polygon:
                    return ConstructPolygon(br, oid);
                default:
                    return null;
            }
            return null;
        }

        private string[] GetFieldNames()
        {
            if (_annReader == null)
                return _dbfReader.Fields;
            else
            {
                List<string> flds = new List<string>();
                if(_dbfReader.Fields != null)
                    flds.AddRange(_dbfReader.Fields);
                return flds.Count > 0 ? flds.ToArray() : null; 
            }
        }

        private string[] GetFieldValues(int oid)
        {
            if (_annReader == null)
                return _dbfReader.GetValues(oid);
            else
            {
                List<string> values = new List<string>();
                string[] vs = _dbfReader.GetValues(oid);
                if (vs != null && vs.Length > 0)
                    values.AddRange(vs);
                return values.Count > 0 ? values.ToArray() : null;
            }
        }

        private LabelLocation[] GetAnnotation(int oid)
        {
            if (_annReader == null)
                return null;
            return _annReader.GetLabelLocation(oid);
        }

        private object ConstructMultiPoint(BinaryReader br, int oid)
        {
            int pointCount = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            ShapePoint[] pts = new ShapePoint[pointCount];
            for (int i = 0; i < pts.Length; i++)
            {
                pts[i] = new ShapePoint(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)), ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)));
            }
            ShapeMultiPoint multiPoint = new ShapeMultiPoint(pts);
            Feature f = new Feature(oid, multiPoint, GetFieldNames(), GetFieldValues(oid), GetAnnotation(oid));
            return f;
        }

        private object ConstructPoint(BinaryReader br, int oid)
        {
            ShapePoint pt = new ShapePoint(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                                            ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8))
                                                             );
            Feature f = new Feature(oid, pt, GetFieldNames(), GetFieldValues(oid), GetAnnotation(oid));
            return f;
        }

        private object ConstructPolyline(BinaryReader br, int oid)
        {
            Envelope evp = new Envelope(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)));
            int nParts = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            int nPoints = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            int[] firstPoints = new int[nParts];
            for (int i = 0; i < nParts; i++)
                firstPoints[i] = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            ShapePoint[] pts = new ShapePoint[nPoints];
            for (int i = 0; i < nPoints; i++)
                pts[i] = new ShapePoint(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                     ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)));
            //ShapeLineString
            ShapeLineString[] Lines = new ShapeLineString[nParts];
            for (int i = 0; i < nParts; i++)
            {
                int bIdx = firstPoints[i];
                int eIdx = 0;
                if (nParts == 1 || i == nParts - 1)
                    eIdx = nPoints;
                else
                    eIdx = firstPoints[i + 1];
                ShapePoint[] rpts = new ShapePoint[eIdx - bIdx];
                for (int j = bIdx; j < eIdx; j++)
                {
                    rpts[j - bIdx] = pts[j];
                }
                Lines[i] = new ShapeLineString(rpts);
            }
            //
            ShapePolyline ply = new ShapePolyline(Lines, evp);
            Feature f = new Feature(oid, ply, GetFieldNames(), GetFieldValues(oid), GetAnnotation(oid));
            return f;
        }

        private object ConstructPolylineM(BinaryReader br, int oid)
        {
            Envelope evp = new Envelope(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)));
            int nParts = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            int nPoints = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            int[] firstPoints = new int[nParts];
            for (int i = 0; i < nParts; i++)
                firstPoints[i] = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            ShapePoint[] pts = new ShapePoint[nPoints];
            for (int i = 0; i < nPoints; i++)
                pts[i] = new ShapePoint(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                     ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)));
            //ShapeLineString
            ShapeLineString[] Lines = new ShapeLineString[nParts];
            for (int i = 0; i < nParts; i++)
            {
                int bIdx = firstPoints[i];
                int eIdx = 0;
                if (nParts == 1 || i == nParts - 1)
                    eIdx = nPoints;
                else
                    eIdx = firstPoints[i + 1];
                ShapePoint[] rpts = new ShapePoint[eIdx - bIdx];
                for (int j = bIdx; j < eIdx; j++)
                {
                    rpts[j - bIdx] = pts[j];
                }
                Lines[i] = new ShapeLineString(rpts);
            }
            //
            br.ReadBytes(2 * 8);//M Range
            br.ReadBytes(nPoints * 8);//M Array
            //
            ShapePolyline ply = new ShapePolyline(Lines, evp);
            Feature f = new Feature(oid, ply, GetFieldNames(), GetFieldValues(oid), GetAnnotation(oid));
            return f;
        }

        private object ConstructPolygon(BinaryReader br, int oid)
        {
            Envelope evp = new Envelope(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                        ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)));
            int nParts = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            int nPoints = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            int[] firstPoints = new int[nParts];
            for (int i = 0; i < nParts; i++)
                firstPoints[i] = ToLocalEndian.ToInt32FromLittle(br.ReadBytes(4));
            ShapePoint[] pts = new ShapePoint[nPoints];
            for (int i = 0; i < nPoints; i++)
                pts[i] = new ShapePoint(ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)),
                                     ToLocalEndian.ToDouble64FromLittle(br.ReadBytes(8)));
            //rings
            ShapeRing[] rings = new ShapeRing[nParts];
            for (int i = 0; i < nParts; i++)
            {
                int bIdx = firstPoints[i];
                int eIdx = 0;
                if (nParts == 1 || i == nParts - 1)
                    eIdx = nPoints;
                else
                    eIdx = firstPoints[i + 1];
                ShapePoint[] rpts = new ShapePoint[eIdx - bIdx];
                for (int j = bIdx; j < eIdx; j++)
                {
                    rpts[j - bIdx] = pts[j];
                }
                rings[i] = new ShapeRing(rpts);
            }
            //
            ShapePolygon ply = new ShapePolygon(rings, evp);
            Feature f = new Feature(oid, ply, GetFieldNames(), GetFieldValues(oid), GetAnnotation(oid));
            return f;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
