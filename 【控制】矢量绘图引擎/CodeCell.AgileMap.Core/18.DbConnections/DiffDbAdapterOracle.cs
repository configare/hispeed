using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CodeCell.AgileMap.Core
{
    internal class DiffDbAdapterOracle : DiffDbAdapter
    {
        public override string GetTableIsExistSql(string tablename, string datasource)
        {
            return "select count(*) from all_all_tables where table_name='" + tablename.ToUpper() + "'";
        }

        public override string DeleteSpatiaoIndexMetadata(string tablename)
        {
            return "delete from user_SDO_GEOM_METADATA where table_name ='"+tablename+"'";
        }

        protected override string GetFieldTypeFromINT64()
        {
            throw new NotImplementedException();
        }

        protected override string GetFieldTypeFromINT16()
        {
            throw new NotImplementedException();
        }

        protected override string GetFieldTypeFromINT32()
        {
            return "INT";
        }

        protected override string GetFieldTypeFromFloat()
        {
            return "FLOAT";
        }

        public override string GetGeometryFieldDef()
        {
            return "SHAPE MDSYS.SDO_GEOMETRY";
        }

        protected override string GetFieldTypeFromGeometry()
        {
            return "MDSYS.SDO_GEOMETRY";
        }

        public override string GetAutoIncrementFieldDef()
        {
            return "ID NUMBER(10,0)";
        }

        public override string GetAnnonIDFieldNameSQLString()
        {
            return "ID,";
        }

        public override string GetAnnonIDFieldValueSQLString(int id)
        {
            return id.ToString()+",";
        }

        public override string DateTimeToSql(DateTime dateTime)
        {
            return "to_date('" + dateTime.ToString("dd-MM-yyyy HH:mm:ss") + "', 'dd-mm-yyyy hh24:mi:ss')";
        }

        //public override string GeometryToSQL(Shape shape,int srid)
        //{
        //    return SdoGeometry.FromAgileMapShapeString(shape, srid);// "MDSYS.SDO_GEOMETRY('" + shape.ToWkt() + "'," + srid + ")";
        //}

        public override string GeometryToSQLByPara(Shape shape)
        {
            return ":shape";
        }

        //public override System.Data.IDbDataParameter CreateDataParameter()
        //{
        //    Oracle.DataAccess.Client.OracleParameter para = new Oracle.DataAccess.Client.OracleParameter("shape", Oracle.DataAccess.Client.OracleDbType.Object);
        //    para.UdtTypeName = "MDSYS.SDO_GEOMETRY";
        //    return para;
        //}

        //public override void ShapeToDbParameter(IDbDataParameter para, Shape shape,int srid)
        //{
        //    para.Value = SdoGeometry.FromAgileMapShape(shape, srid);
        //}

        public override string GetFieldTypeString(string metaFieldType)
        {
            if (metaFieldType.ToUpper() == "DATETIME")
                return "DATE";
            else if (metaFieldType.ToUpper() == "VARCHAR(4096)")
                return "VARCHAR2(4000)";
            else if (metaFieldType.ToUpper().Contains("TEXT"))
                return "CLOB";
            return base.GetFieldTypeString(metaFieldType);
        }

        public override string SQLToWktForShapeField(string fieldName)
        {
            //return "astext(" + fieldName + ")";
            //return "t."+fieldName + ".GET_WKT()";
            return "t." + fieldName;
        }

        public override string GetOIDFieldDef()
        {
            return "OID INT NOT NULL";
        }

        //public override string GeomeryToSqlWhere(string fieldname, Shape shape)
        //{
        //    return "MDSYS.SDO_FILTER(t." + fieldname + ","+SdoGeometry.FromAgileMapShapeString(shape,4326)+",'querytype=WINDOW') like '%TRUE%'";
        //}

        protected override string GetSpatialIndexDef(string tablename,string spatialIndexField)
        {
            return null;
        }

        public override string CreateSpatialIndex(string tablename, string spatialIndexField)
        {
            return "CREATE INDEX " + tablename + "_SIDX ON " + tablename + "(" + spatialIndexField + ") INDEXTYPE IS MDSYS.SPATIAL_INDEX";
        }

        public override string InsertMetadataForSpatialIndex(string tablename, string spatialIndexField)
        {
            Envelope mbr = new Envelope(-180,-90,180,90);
            double tolerance = 0.5;
            int srid = 4326;
            string sql = "INSERT INTO USER_SDO_GEOM_METADATA VALUES(" +
                   "'" + tablename + "'," +
                   "'" + spatialIndexField + "'," +
                   "MDSYS.SDO_DIM_ARRAY" +
                   "(" +
                       "MDSYS.SDO_DIM_ELEMENT" +
                        "(" +
                        "'X',{0},{1},{2})," +
                        "MDSYS.SDO_DIM_ELEMENT" +
                        "(" +
                        "'Y',{3},{4},{5})" +
                    ")," +
                    "{6}" +
                    ")";
            
            return string.Format(sql, mbr.MinX, mbr.MaxX, tolerance,
                                 mbr.MinY, mbr.MaxY, tolerance, srid);
        }

        public override int DrToInt32(System.Data.IDataReader dr, int index)
        {
            Type t = dr.GetFieldType(index);
            if (t.Equals(typeof(decimal)))
                return dr.IsDBNull(index) ? 0 : (int)dr.GetDecimal(index);
            else if (t.Equals(typeof(Int32)))
                return dr.IsDBNull(index) ? 0 : (int)dr.GetInt32(index);
            else
                throw new NotImplementedException();
        }

        public override float DrToFloat(System.Data.IDataReader dr, int index)
        {
            Type t = dr.GetFieldType(index);
            if (t.Equals(typeof(decimal)))
                return dr.IsDBNull(index) ? 0 : (float)dr.GetDecimal(index);
            else if (t.Equals(typeof(double)))
                return dr.IsDBNull(index) ? 0 : (float)dr.GetDouble(index);
            else if (t.Equals(typeof(float)))
                return dr.IsDBNull(index) ? 0 : dr.GetFloat(index);
            throw new NotImplementedException();
        }

        public override string DrToString(System.Data.IDataReader dr, int index)
        {
            Type t = dr.GetFieldType(index);
            return dr.IsDBNull(index) ? string.Empty : dr.GetString(index);
        }

        //public override Shape DrToShape(IDataReader dr, int index)
        //{
        //    //string wkt = dr.IsDBNull(index) ? string.Empty : dr.GetString(index);
        //    //if (string.IsNullOrEmpty(wkt))
        //    //    return null;
        //    //return Shape.FromWKT(wkt);
        //    SdoGeometry geo = dr.IsDBNull(index) ? null : (SdoGeometry)dr.GetValue(index);
        //    return geo.ToAgileMapShape();
        //}

        public override Dictionary<string, Type> GetFieldsOfTable(IDbConnection conn, string tablename, out string shapeField, out string oidField)
        {
            shapeField = null;
            oidField = null;
            string connstring = conn.ConnectionString;
            string sql = "select COLUMN_NAME,DATA_TYPE,DATA_LENGTH from all_tab_columns where TABLE_NAME='" + tablename.ToUpper() + "'";
            //string sql = "select column_name,data_type from information_schema.columns" +
            //                  " where table_schema = '" + GetDataSource(connstring) + "'and table_name = '" + tablename + "'";
            using (IDbCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    Dictionary<string, Type> flds = new Dictionary<string, Type>();
                    while (dr.Read())
                    {
                        string fldname = DrToString(dr, 0).ToUpper();
                        string fldType = DrToString(dr, 1).ToUpper();
                        int fldLen = DrToInt32(dr, 2);
                        if (fldname == "OID")
                        {
                            oidField = fldname;
                            continue;
                        }
                        switch (fldType)
                        {
                            case "VARCHAR":
                            case "VARCHAR2":
                            case "CLOB":
                                flds.Add(fldname, typeof(string));
                                break;
                            case "NUMBER":
                                flds.Add(fldname, typeof(Int32));
                                break;
                            case "MDSYS.SDO_GEOMETRY":
                            case "SDO_GEOMETRY":
                                shapeField = fldname;
                                break;
                            default:
                                throw new Exception("MySQL数据库的数据类型\"" + fldType + "\"对应.NET中的数据类型未定义。");
                        }
                    }
                    return flds;
                }
            }
        }
    }
}
