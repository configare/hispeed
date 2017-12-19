using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    public abstract class CatalogEntityClassBase:ICatalogEntityClass,IDisposable
    {
        protected DiffDbAdapter _adapter = null;
        protected string _connString = null;
        protected IDbConnection _dbConnection = null;

        private class FieldAttAndValue
        {
            public AttToFieldMapAttribute FieldAtt = null;
            public object Value = null;

            public FieldAttAndValue(AttToFieldMapAttribute fieldAtt, object value)
            { 
                FieldAtt = fieldAtt ;
                Value = value ;
            }
        }

        public CatalogEntityClassBase(string connString)
        {
            _dbConnection = DbConnectionFactory.CreateDbConnection(connString);
            _dbConnection.Open();
            _connString = connString;
            _adapter = DiffDbAdapterFactory.GetDiffDbAdapter(_dbConnection);
        }

        protected abstract string GetTableName();
        protected abstract ICatalogEntity NewCatalogEntity();

        private FieldAttAndValue[] GetFieldAttAndValues(ICatalogEntity entity)
        {
            BindingFlags flags = BindingFlags.GetProperty;
            Type t = entity.GetType();
            PropertyInfo[] pInfos = t.GetProperties();
            List<FieldAttAndValue> values = new List<FieldAttAndValue>();
            foreach (PropertyInfo pInfo in pInfos)
            {
                object[] atts = pInfo.GetCustomAttributes(typeof(AttToFieldMapAttribute), true);
                if (atts == null || atts.Length == 0)
                    continue;
                AttToFieldMapAttribute fieldAtt = atts[0] as AttToFieldMapAttribute;
                object fieldValue = t.InvokeMember(pInfo.Name, flags, null, entity, null);
                values.Add(new FieldAttAndValue(fieldAtt, fieldValue));
            }
            return values.ToArray();
        }

        private string FieldValueToSqlExp(Type fieldType, object fieldValue)
        {
            if (fieldType.Equals(typeof(string)))
                return "'" + (fieldValue != null ? fieldValue.ToString() : string.Empty) + "'";
            else if (fieldType.Equals(typeof(DateTime)))
                return _adapter.DateTimeToSql((DateTime)fieldValue);
            else if (fieldType.Equals(typeof(Int16)) || fieldType.Equals(typeof(Int32)) || fieldType.Equals(typeof(Int64)) || fieldType.Equals(typeof(Byte)))
                return fieldValue.ToString();
            else if (fieldType.Equals(typeof(enumShapeType)))
                return "'"+fieldValue.ToString()+"'";
            else if (fieldType.Equals(typeof(Envelope)))
            {
                Envelope evp = fieldValue as Envelope;
                if(evp != null)
                    return _adapter.GeometryToSQL(evp.ToPolygon(),4326); //fdc+fixed
            }
            throw new NotSupportedException("不支持的字段类型\""+fieldType.ToString()+"\"。");
        }


        private object DataReaderToFieldValue(Type fieldType, IDataReader dr, int idx)
        {
            if (fieldType.Equals(typeof(string)))
                return dr.IsDBNull(idx) ? string.Empty : dr.GetString(idx);
            else if (fieldType.Equals(typeof(DateTime)))
                return dr.IsDBNull(idx) ? DateTime.MinValue : dr.GetDateTime(idx);
            else if (fieldType.Equals(typeof(Int16)))
                return dr.IsDBNull(idx) ? 0 : dr.GetInt16(idx);
            else if (fieldType.Equals(typeof(Int32)))
            {
                Type t = dr.GetFieldType(idx);
                if (t.Equals(typeof(decimal)))
                    return dr.IsDBNull(idx) ? 0 : (int)dr.GetDecimal(idx);
                else if (t.Equals(typeof(Int32)))
                    return dr.IsDBNull(idx) ? 0 : (int)dr.GetInt32(idx);
                else
                    throw new NotImplementedException();
            }
            else if (fieldType.Equals(typeof(Int64)))
                return dr.IsDBNull(idx) ? 0 : dr.GetInt64(idx);
            else if (fieldType.Equals(typeof(Byte)))
                return dr.IsDBNull(idx) ? 0 : dr.GetByte(idx);
            else if (fieldType.Equals(typeof(enumShapeType)))
                return DrToShapeType(dr, idx);
            else if (fieldType.Equals(typeof(Envelope)))
                return DrToEnvelope(dr, idx);
            throw new NotSupportedException("不支持的字段类型\"" + fieldType + "\"。");
        }

        private object DrToEnvelope(IDataReader dr, int idx)
        {
            ShapePolygon ply = _adapter.DrToShape(dr, idx) as ShapePolygon;
            if (ply != null)
            {
                ShapeRing ring = ply.Rings[0];
                return new Envelope(ring.Points[0].X, ring.Points[2].Y, ring.Points[1].X, ring.Points[0].Y);
            }
            return new Envelope();
        }

        private object DrToShapeType(IDataReader dr, int idx)
        {
            string str = _adapter.DrToString(dr, idx);
            if (string.IsNullOrEmpty(str))
                return enumShapeType.NullShape;
            foreach (enumShapeType st in Enum.GetValues(typeof(enumShapeType)))
            {
                if(st.ToString() == str)
                    return st;
            }
            return enumShapeType.NullShape;
        }

        #region ICatalogEntityClass Members

        public void Insert(ICatalogEntity entity)
        {
            FieldAttAndValue[] fvs = GetFieldAttAndValues(entity);
            StringBuilder sb = new StringBuilder();
            sb.Append("insert into " + GetTableName() + "(");
            //fields
            for (int i = 0; i < fvs.Length; i++)
            {
                if (i == fvs.Length - 1)
                    sb.Append(fvs[i].FieldAtt.FieldName+") ");
                else
                    sb.Append(fvs[i].FieldAtt.FieldName + ",");
            }
            //values
            sb.Append("values(");
            for (int i = 0; i < fvs.Length; i++)
            {
                if (i == fvs.Length - 1)
                    sb.Append(FieldValueToSqlExp(fvs[i].FieldAtt.FieldType, fvs[i].Value)+")");
                else
                    sb.Append(FieldValueToSqlExp(fvs[i].FieldAtt.FieldType, fvs[i].Value)+",");
            }
            //submit
            using (IDbCommand cmd = _dbConnection.CreateCommand())
            {
                try
                {
                    TransactionManager.BeginTransaction(_dbConnection, cmd);
                    cmd.CommandText = sb.ToString();
                    int ret = cmd.ExecuteNonQuery();
                    TransactionManager.Commit();
                }
                catch 
                {
                    TransactionManager.Rollback();
                    throw;
                }
            }
        }

        public void Update(ICatalogEntity entity)
        {
            FieldAttAndValue[] fvs = GetFieldAttAndValues(entity);
            StringBuilder sb = new StringBuilder();
            sb.Append("update " + GetTableName()+" set ");
            //fields
            for (int i = 0; i < fvs.Length; i++)
            {
                if (fvs[i].FieldAtt.FieldName.ToUpper() == "ID")
                    continue;
                if (i == fvs.Length - 1)
                    sb.Append( fvs[i].FieldAtt.FieldName + " = " + FieldValueToSqlExp(fvs[i].FieldAtt.FieldType, fvs[i].Value));
                else
                    sb.Append( fvs[i].FieldAtt.FieldName + " = " + FieldValueToSqlExp(fvs[i].FieldAtt.FieldType, fvs[i].Value) + ",");
            }
            string sql = sb.ToString();
            if (sql.EndsWith(","))
                sql = sql.Substring(0, sql.Length - 1);
            sql = sql + " where id='" + entity.Id+"'";
            using (IDbCommand cmd = _dbConnection.CreateCommand())
            {
                try
                {
                    TransactionManager.BeginTransaction(_dbConnection, cmd);
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                    TransactionManager.Commit();
                }
                catch
                {
                    TransactionManager.Rollback();
                    throw;
                }
            }
        }

        public virtual void Delete(ICatalogEntity entity)
        {
            using (IDbCommand cmd = _dbConnection.CreateCommand())
            {
                string sql = "delete from " + GetTableName() + " where id='" + entity.Id + "'";
                TransactionManager.BeginTransaction(_dbConnection, cmd);
                try
                {
                    cmd.CommandText = sql;
                    int ret = cmd.ExecuteNonQuery();
                    TransactionManager.Commit();
                }
                catch
                {
                    TransactionManager.Rollback();
                    throw;
                }
            }
        }

        public bool IsExist(ICatalogEntity entity)
        {
            using (IDbCommand cmd = _dbConnection.CreateCommand())
            {
                string sql = "select count(*) from " + GetTableName() + " where id='" + entity.Id + "'";
                cmd.CommandText = sql;
                object v = cmd.ExecuteScalar();
                int ret = int.Parse(v.ToString());
                return ret > 0;
            }
        }

        public ICatalogEntity[] Query(string where)
        {
            return Query(NewCatalogEntity(), where); 
        }

        public ICatalogEntity[] Query(ICatalogEntity templateEntity)
        {
            return Query(templateEntity, null);
        }

        public ICatalogEntity[] Query(ICatalogEntity templateEntity,string where)
        {
            if (templateEntity == null)
                return null;
            FieldAttAndValue[] fvs = GetFieldAttAndValues(templateEntity);
            StringBuilder sb = new StringBuilder();
            sb.Append("select ");
            //fields
            for (int i = 0; i < fvs.Length; i++)
            {
                string fname = fvs[i].FieldAtt.FieldName;
                if(fvs[i].FieldAtt.FieldType.Equals(typeof(Envelope)))
                    fname = _adapter.SQLToWktForShapeField(fname);
                if (i == fvs.Length - 1)
                    sb.Append(fname + " from " + GetTableName()+" t");
                else
                    sb.Append(fname + ",");
            }
            if (where != null)
            {
                sb.Append(" where " + where);
            }
            string sql = sb.ToString();
            if(sql.EndsWith(","))
                sql = sql.Substring(0, sql.Length - 1);
            //query
            using (IDbCommand cmd = _dbConnection.CreateCommand())
            {
                cmd.CommandText = sql;
                using (IDataReader dr = cmd.ExecuteReader())
                {
                    if (dr == null)
                        return null;
                    List<ICatalogEntity> retobjs = new List<ICatalogEntity>();
                    while (dr.Read())
                    {
                        ICatalogEntity c = NewCatalogEntity();
                        //
                        (c as CatalogEntityBase)._connString = _connString;
                        //fill value
                        Type t = c.GetType();
                        for (int i = 0; i < fvs.Length; i++)
                        {
                            t.InvokeMember(fvs[i].FieldAtt.FieldName, BindingFlags.SetProperty, null, c, new object[] { DataReaderToFieldValue(fvs[i].FieldAtt.FieldType,dr,i)});
                        }
                        //
                        (c as CatalogEntityBase)._connString = _connString;
                        //
                        retobjs.Add(c);
                    }
                    return retobjs.Count > 0 ? retobjs.ToArray() : null;
                }
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_dbConnection != null)
            {
                _dbConnection.Close();
                _dbConnection.Dispose();
                _dbConnection = null;
            }
        }

        #endregion
    }
}
