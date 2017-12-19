using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CodeCell.AgileMap.Core
{
    internal class DiffDbAdapterMySQL : DiffDbAdapter
    {
        protected override string GetFieldTypeFromINT64()
        {
            return "BIGINT";
        }

        protected override string GetFieldTypeFromINT16()
        {
            return "SMALLINT";
        }

        protected override string GetFieldTypeFromINT32()
        {
            return "INT";
        }

        public override string GetGeometryFieldDef()
        {
            return "SHAPE GEOMETRY NOT NULL";
        }

        protected override string GetFieldTypeFromGeometry()
        {
            return "GEOMETRY";
        }

        public override string GeomeryToSqlWhere(string fieldname,Shape shape)
        {
            //return "MBRContains(GeomFromText('"+shape.ToWkt()+"'),"+fieldname+")";
            return "MBRIntersects(GeomFromText('" + shape.ToWkt() + "')," + fieldname + ")";
        }

        public override string GetAutoIncrementFieldDef()
        {
            return "ID INT AUTO_INCREMENT";
        }

        public override IDbDataParameter CreateDataParameter()
        {
            IDbDataParameter p = new MySql.Data.MySqlClient.MySqlParameter(null,MySql.Data.MySqlClient.MySqlDbType.Text);
            return p;
        }

        public override string GeometryToSQL(Shape shape, int srid)
        {
            return "GeomFromText('" + shape.ToWkt() + "')";
        }

        public override void ShapeToDbParameter(IDbDataParameter para, Shape shape, int srid)
        {
            para.Value = shape.ToWkt();
        }

        protected override string GetFieldTypeFromVARCHAR(int size)
        {
            return "VARCHAR(" + size.ToString() + ") CHARACTER SET utf8 COLLATE utf8_bin";
        }

        protected override string GetFieldTypeFromTEXT(int size)
        {
            return "TEXT(" + size.ToString() + ") CHARACTER SET utf8 COLLATE utf8_bin";
        }

        protected override string GetFieldTypeFromFloat()
        {
            return "FLOAT";
        }

        protected override string GetTableOptions(bool isSpatial)
        {
            if (isSpatial)
                return "CHARACTER SET utf8 COLLATE utf8_bin,engine=MyISAM";
            else
                return "CHARACTER SET utf8 COLLATE utf8_bin,engine=InnoDB";
        }

        public override string DateTimeToSql(DateTime dateTime)
        {
            return "'" + dateTime.ToString("yyyy-MM-dd HH:mm:ss") + "'";
        }

        public override string GetTableIsExistSql(string tablename, string datasource)
        {
            return "select count(*) from information_schema.tables where table_schema = '" + datasource + "' and table_name='" + tablename+"'";
        }

        public override string DrToString(IDataReader dr, int index)
        {
            return dr.IsDBNull(index) ? string.Empty : dr.GetString(index);
        }

        public override Shape DrToShape(IDataReader dr, int index)
        {
            string wkt = dr.IsDBNull(index) ? string.Empty : dr.GetString(index);
            if (string.IsNullOrEmpty(wkt))
                return null;
            return Shape.FromWKT(wkt);
        }

        public override int DrToInt32(IDataReader dr, int index)
        {
            return dr.IsDBNull(0) ? 0 : dr.GetInt32(index);
        }

        public override float DrToFloat(IDataReader dr, int index)
        {
            return dr.IsDBNull(index) ? 0 : dr.GetFloat(index);
        }

        public override string SQLToWktForShapeField(string fieldName)
        {
            return "astext("+fieldName+")";
        }

        public override Dictionary<string, Type> GetFieldsOfTable(IDbConnection conn, string tablename, out string shapeField,out string oidField)
        {
            shapeField = null;
            oidField = null;
            string connstring = conn.ConnectionString;
            string sql = "select column_name,data_type from information_schema.columns"+
                              " where table_schema = '"+GetDataSource(connstring)+"'and table_name = '"+tablename+"'";
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
                        if (fldname == "OID")
                        {
                            oidField = fldname;
                            continue;
                        }
                        switch (fldType)
                        { 
                            case "VARCHAR":
                                flds.Add(fldname, typeof(string));
                                break;
                            case "INT":
                                flds.Add(fldname, typeof(int));
                                break;
                            case "GEOMETRY":
                                shapeField = fldname;
                                break;
                            default:
                                throw new Exception("MySQL数据库的数据类型\"" + fldType+"\"对应.NET中的数据类型未定义。");
                        }
                    }
                    return flds;
                }
            }
        }

        public override string CreateSpatialIndex(string tablename, string spatialIndexField)
        {
            return null;
        }

        public override string GetFieldTypeString(string metaFieldType)
        {
            return metaFieldType;
        }

        public override string GeometryToSQLByPara(Shape shape)
        {
            return "GeomFromText(?shape)";
        }
    }
}
