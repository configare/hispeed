using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    internal class FetClassWriterDb:IFeatureClassWriter,IDisposable
    {
        private IDbConnection _dbConn = null;
        private DiffDbAdapter _adapter = null;
        private string _datasetId = string.Empty;
        private ISpatialReference _dstSpatialRef = null;

        public FetClassWriterDb(ICatalogItem locationItem)
        {
            if (locationItem is CatalogDatabaseConn)
            {
                _dbConn = DbConnectionFactory.CreateDbConnection(((locationItem as CatalogDatabaseConn).Tag as SpatialDatabaseConn).ConnectionString);
            }
            else if (locationItem is CatalogFeatureDataset)
            {
                CatalogFeatureDataset cfd = locationItem as CatalogFeatureDataset;
                SpatialFeatureDataset sfd = cfd.Tag as SpatialFeatureDataset;
                _dbConn = DbConnectionFactory.CreateDbConnection((sfd as CatalogEntityBase)._connString);
                _datasetId = sfd.Id;
                _dstSpatialRef = SpatialReferenceFactory.GetSpatialReferenceByWKT(sfd.SpatialRef,enumWKTSource.EsriPrjFile);
            }
            else
                throw new NotSupportedException("类型为\""+locationItem+"\"的位置暂不支持写。");
            _adapter = DiffDbAdapterFactory.GetDiffDbAdapter(_dbConn);
            _dbConn.Open();
        }

        #region IFeatureClassWriter Members

        public ISpatialReference SpatialReference
        {
            get { return _dstSpatialRef; }
        }

        public int SpatialRefToSRID()
        {
            return _dstSpatialRef == null ? CodeCell.AgileMap.Core.SpatialReference.WGS84SRID : _dstSpatialRef.OracleSpatialSRID;
        }

        public void Write(IFeatureClassReader reader, IProgressTracker tracker,string tableName,string displayName,string description)
        {
            if (tracker != null)
            {
                tracker.StartTracking("正在读取要素...", 100);
                tracker.Tracking("正在读取要素...", 30);
            }
            Feature[] features = reader.Read(tracker );
            if (tracker != null)
            {
                tracker.Tracking("读取要素结束。", 100);
            }
            //
            try
            {
                using (IDbCommand cmd = _dbConn.CreateCommand())
                {
                    TransactionManager.BeginTransaction(_dbConn, cmd);
                    //Create FeatureClass Table
                    string[] fields = GetFieldTypes(reader.FieldNames, features);
                    _adapter.CreateTable(_dbConn, true, "OID","SHAPE", tableName, fields, cmd);
                    //Create Annotation Table
                    string[] annFields = null;
                    if (reader.AnnoTable != null)
                    {
                        annFields = GetAnnoTableFields();
                        _adapter.CreateTable(_dbConn, true, _adapter.GetAnnonKeyField(), "SHAPE",reader.AnnoTable, annFields, cmd);
                    }
                    //Insert Row Into BGIS_FeatureClass
                    int span = Math.Min(100, features.Length);
                    if (tracker != null)
                        tracker.StartTracking("正在导入要素集\"" + displayName + "\"...", features.Length / span + 1);
                    // 
                    InsertFeatureClassRow(cmd, tableName, displayName,description,
                                                     (reader.SpatialReference != null ? reader.SpatialReference.ToString() : string.Empty),
                                                     0,
                                                     string.Empty,
                                                     reader.AnnoTable,
                                                     reader.Envelope,
                                                     reader.ShapeType,
                                                     features.Length
                                                     );
                    //Insert Features
                    InsertFeatures(cmd, features, fields, tableName, tracker, span, reader.AnnoTable, annFields);
                    //Submit
                    TransactionManager.Commit();
                }
            }
            catch (Exception ex)
            {
                TransactionManager.Rollback();
                throw ex;
            }
        }

        private string[] GetAnnoTableFields()
        {
            return new string[] 
                            {
                                _adapter.GetAutoIncrementFieldDef(), 
                                "OID "+_adapter.GetSpecialFieldType("INT32") + " NOT NULL",
                                "ANGLE " + _adapter.GetSpecialFieldType("FLOAT"),
                                _adapter.GetGeometryFieldDef()
                            };
        }

        int annId = 0;
        private void InsertFeatures(IDbCommand cmd, Feature[] features, string[] fieldTypes, string tablename, IProgressTracker tracker, int span, string anntable, string[] annfields)
        {
            string flds = string.Empty;
            for (int i = 0; i < fieldTypes.Length; i++)
                flds += (fieldTypes[i].Split(' ')[0] + ",");
            flds = flds.Substring(0, flds.Length - 1);
            int ret = 0;
            int idx = -1;
            foreach (Feature fet in features)
            {
                idx++;
                int n = (idx / span);
                if (idx % span == 0 && tracker != null)
                {
                    tracker.Tracking("正在导入要素(" + (n * span).ToString() + "/" + features.Length.ToString() + ")...", n);
                }
                if (fet.Geometry == null)
                    continue;
                string sql = "insert into " + tablename + "(" + flds + ") values(" +
                         fet.OID.ToString() + ",";
                for (int i = 1; i < fieldTypes.Length - 1; i++)
                    sql += ("'" + fet.GetFieldValue(i - 1) + "',");
                //sql += _adapter.GeometryToSQL(fet.Geometry) + ")";
                sql += _adapter.GeometryToSQLByPara(fet.Geometry) + ")";
                IDbDataParameter ps = GetShapePara(fet.Geometry);
                cmd.Parameters.Add(ps);
                cmd.CommandText = sql;
                ret = cmd.ExecuteNonQuery();
                //insert anno
                if (anntable != null && fet.Annotations != null && fet.Annotations.Length > 0)
                {
                    for (int annI = 0; annI < fet.Annotations.Length; annI++)
                    {
                        sql = "insert into " + anntable + "("+_adapter.GetAnnonIDFieldNameSQLString()+"OID,ANGLE,SHAPE) values(" +
                                _adapter.GetAnnonIDFieldValueSQLString(annId) +
                                fet.OID.ToString() + "," + fet.Annotations[annI].Angle.ToString() + "," + _adapter.GeometryToSQL(fet.Annotations[annI].Location, SpatialRefToSRID()) + ")";
                        cmd.Parameters.Clear();
                        cmd.CommandText = sql;
                        ret = cmd.ExecuteNonQuery();
                        annId++;
                    }
                }
                else
                    cmd.Parameters.Clear();
            }
        }

        private IDbDataParameter GetShapePara(Shape shape)
        {
            IDbDataParameter p = _adapter.CreateDataParameter();
            p.ParameterName = "shape";
            //p.Value = shape.ToWkt();
            //p.Value = SdoGeometry.FromAgileMapShape(shape, SpatialRefToSRID());
            _adapter.ShapeToDbParameter(p, shape, SpatialRefToSRID());
            return p;
        }

        private void InsertFeatureClassRow(IDbCommand cmd,string name,string displayName,string description,string spatialRef,
                                                            int mapscale,string source,string annotable,
                                                            Envelope envelope,enumShapeType shapeType,int fetCount)
        {
            string sql = "insert into BGIS_FeatureClass(ID,DATASETID,NAME,DATATABLE,SPATIALREF,CREATETIME,"+
                              "MAPSCALE,SOURCE,DESCRIPTION,ANNOTABLE,ENVELOPE,SHAPETYPE,FEATURECOUNT) " +
                             "values('{0}','{1}','{2}','{3}','{4}',{5},{6},'{7}','{8}','{9}',{10},'{11}',{12})";
            if (envelope == null)
                envelope = new Envelope();//empty 
            cmd.CommandText = string.Format(sql,
                                                                 Guid.NewGuid().ToString(),
                                                                 _datasetId,
                                                                 displayName,
                                                                 name,
                                                                 spatialRef,
                                                                 _adapter.DateTimeToSql(DateTime.Now),
                                                                 mapscale.ToString(),
                                                                 source,
                                                                 description,
                                                                 (string.IsNullOrEmpty(annotable)?string.Empty:annotable),
                                                                 _adapter.GeometryToSQL(envelope.ToPolygon(), SpatialRefToSRID()),
                                                                 shapeType.ToString(),
                                                                 fetCount.ToString()
                                                                 );
            cmd.ExecuteNonQuery();
        }

        private string[] GetFieldTypes(string[] fields, Feature[] features)
        {
            string[] fieldTypes = new string[fields.Length + 2];
            fieldTypes[0] = _adapter.GetOIDFieldDef();//
            for (int i = 0; i < fields.Length; i++)
            {
                int len = fields[i].Length * 2;//unicode
                for (int ifea = 0; ifea < features.Length; ifea++)
                {
                    string v = features[ifea].GetFieldValue(fields[i]);
                    if (v == null)
                        continue;
                    int vLen = v.Length * 2;
                    if (len < vLen)
                        len = vLen;
                }
                fieldTypes[i + 1] = fields[i].ToUpper() + " VARCHAR(" + len.ToString() + ")";
            }
            fieldTypes[fieldTypes.Length - 1] = _adapter.GetGeometryFieldDef();
            return fieldTypes;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_dbConn != null)
            {
                _dbConn.Dispose();
                _dbConn = null;
            }
        }

        #endregion
    }
}
