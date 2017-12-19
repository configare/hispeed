using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;

namespace CodeCell.AgileMap.Core
{
    public abstract class DiffDbAdapter
    {
        public virtual void CreateTable(IDbConnection connection,
            bool isSpatial,string keyField,
            string spatialIndexField,string tablename, 
            string[] fieldTypes, IDbCommand cmd)
        {
            string sql = "create table " + tablename + " (";
            foreach (string fld in fieldTypes)
            {
                string[] vs = fld.Split(' ');
                string flddef = vs[0] + " " + GetSpecialFieldType(vs[1]);
                if (vs.Length > 2)
                {
                    for (int i = 2; i < vs.Length;i++ )
                        flddef = flddef + " " + vs[i];
                }
                sql += (flddef + ",");
            }
            if (sql.EndsWith(","))
                sql = sql.Substring(0, sql.Length - 1);
            if(keyField != null)
                sql += ",PRIMARY KEY("+keyField+")";
            if (spatialIndexField != null)
            {
                string sidx = GetSpatialIndexDef(tablename, spatialIndexField);
                if(sidx != null)
                    sql += ("," + sidx);//
            }
            sql += ") " + GetTableOptions(isSpatial);
            //
            cmd.CommandText = sql;
            int ret = cmd.ExecuteNonQuery();
            //
            if (isSpatial)
            {
                string createSIDX = CreateSpatialIndex(tablename, spatialIndexField);
                if (createSIDX != null)
                {
                    string insertMetadata = InsertMetadataForSpatialIndex(tablename, spatialIndexField);
                    cmd.CommandText = insertMetadata;
                    cmd.ExecuteNonQuery();
                    //
                    cmd.CommandText = createSIDX;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected virtual string GetSpatialIndexDef(string tablename, string spatialIndexField)
        {
            return "SPATIAL INDEX(" + spatialIndexField + ")";
        }

        public virtual string InsertMetadataForSpatialIndex(string tablename, string spatialIndexField)
        {
            return null;
        }

        public virtual string CreateSpatialIndex(string tablename, string spatialIndexField)
        {
            return null;
        }

        protected virtual string GetTableOptions(bool isSpatial)
        {
            return string.Empty;
        }

        public virtual string DeleteSpatiaoIndexMetadata(string tablename)
        {
            return null;
        }

        public virtual string GetSpecialFieldType(string fieldType)
        {
            Match m = Regex.Match(fieldType, @"(?<ftype>\S+)(?<size>\(\d+\))?");
            if (m.Success)
            {
                int size = 0;
                string sizestring = m.Groups["size"].Value;
                if (sizestring.Trim() != string.Empty)
                    size = int.Parse(sizestring);
                string ftype = m.Groups["ftype"].Value;
                switch (ftype)
                {
                    case "INT32":
                        return GetFieldTypeFromINT32();
                    case "INT16":
                        return GetFieldTypeFromINT16();
                    case "INT64":
                        return GetFieldTypeFromINT64();
                    case "FLOAT":
                        return GetFieldTypeFromFloat();
                    case "VARCHAR":
                        return GetFieldTypeFromVARCHAR(size);
                    case "TEXT":
                        return GetFieldTypeFromTEXT(size);
                    case "GEOMETRY":
                        return GetFieldTypeFromGeometry();
                    default:
                        return fieldType;
                }
            }
            else
            {
                return fieldType;
            }
        }

        protected virtual string GetFieldTypeFromGeometry()
        {
            throw new NotImplementedException();
        }

        protected virtual string GetFieldTypeFromFloat()
        {
            throw new NotImplementedException();
        }

        protected virtual string GetFieldTypeFromVARCHAR(int size)
        {
            return "VARCHAR(" + size.ToString() + ")";
        }

        protected virtual string GetFieldTypeFromTEXT(int size)
        {
            return "TEXT(" + size.ToString() + ")";
        }

        public virtual string SQLToWktForShapeField(string fieldName)
        {
            throw new NotImplementedException();
        }

        public virtual string GeomeryToSqlWhere(string fieldname, Shape shape)
        {
            throw new NotImplementedException();
        }

        protected abstract string GetFieldTypeFromINT64();

        protected abstract string GetFieldTypeFromINT16();

        protected abstract string GetFieldTypeFromINT32();

        public abstract string GetGeometryFieldDef();

        public virtual void ShapeToDbParameter(IDbDataParameter para, Shape shape, int srid)
        {
            throw new NotImplementedException();
        }

        public virtual string GeometryToSQL(Shape shape, int srid)
        {
            throw new NotImplementedException();
        }

        public virtual string GeometryToSQLByPara(Shape shape)
        {
            throw new NotImplementedException();
        }

        public virtual string GetAutoIncrementFieldDef()
        {
            throw new NotImplementedException();
        }

        public virtual string GetAnnonIDFieldNameSQLString()
        {
            return string.Empty;
        }

        public virtual string GetAnnonIDFieldValueSQLString(int id)
        {
            return string.Empty;
        }

        public virtual string GetAnnonKeyField()
        {
            return "ID";
        }

        public virtual IDbDataParameter CreateDataParameter()
        {
            throw new NotImplementedException();
        }

        public virtual string DrToString(IDataReader dr, int index)
        {
            throw new NotImplementedException();
        }

        public virtual byte DrToByte(IDataReader dr, int index)
        {
            throw new NotImplementedException();
        }

        public virtual Int16 DrToInt16(IDataReader dr, int index)
        {
            throw new NotImplementedException();
        }

        public virtual Int32 DrToInt32(IDataReader dr, int index)
        {
            throw new NotImplementedException();
        }

        public virtual Int64 DrToInt64(IDataReader dr, int index)
        {
            throw new NotImplementedException();
        }

        public virtual DateTime DrToDateTime(IDataReader dr, int index)
        {
            throw new NotImplementedException();
        }

        public virtual bool DrToBool(IDataReader dr, int index)
        {
            throw new NotImplementedException();
        }

        public virtual Shape DrToShape(IDataReader dr, int index)
        {
            throw new NotImplementedException();
        }

        public virtual float DrToFloat(IDataReader dr, int index)
        {
            throw new NotImplementedException();
        }

        public abstract string DateTimeToSql(DateTime dateTime);

        public virtual string GetTableIsExistSql(string tablename, string datasource)
        {
            throw new NotImplementedException();
        }

        public virtual string GetDataSource(string connstring)
        {
            string[] parts = connstring.Split(';');
            if (parts == null || parts.Length == 0)
                return null;
            foreach (string p in parts)
            {
                string up = p.ToUpper();
                if (up.Contains("Database".ToUpper()))
                {
                    parts = p.Split('=');
                    return parts[1];
                }
            }
            return null;
        }

        public virtual Dictionary<string, Type> GetFieldsOfTable(IDbConnection conn, string tablename, out string shapeField, out string oidField)
        {
            throw new NotImplementedException();
        }

        public virtual string GetFieldTypeString(string metaFieldType)
        {
            return metaFieldType;
        }

        public virtual string GetOIDFieldDef()
        {
            return "OID INT32 NOT NULL";
        }
    }
}
