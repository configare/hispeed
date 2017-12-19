using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.WebDataServer
{
    internal class FeaturesReaderServiceDb:IFeaturesReaderService
    {
        private IVectorFeatureSpatialDbReader _reader = null;
        private string _connstring = null;
        private string _datatable = null;

        public FeaturesReaderServiceDb(string connString, string datatable)
        {
            _connstring = connString;
            _datatable = datatable;
            try
            {
                Init();
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
            }
        }

        private void Init()
        {
            using (IDbConnection dbConn = DbConnectionFactory.CreateDbConnection(_connstring))
            {
                if (dbConn == null)
                    return;
                dbConn.Open();
                _reader = new VectorFeatureSpatialDbReader(dbConn, _datatable);
            }
        }

        #region IFeaturesReaderService Members

        public void BeginRead()
        {
        }

        public void EndRead()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
        }

        public Feature[] Read(Envelope envelope)
        {
            if (_reader == null)
                Init();
            if (_reader == null)
                return null;
            try
            {
                using (IDbConnection dbConn = DbConnectionFactory.CreateDbConnection(_connstring))
                {
                    if (dbConn == null)
                        return null;
                    dbConn.Open();
                    return _reader.Read(envelope, dbConn);
                }
            }
            catch (Exception ex)
            {
                Log.WriterException(ex);
                return null;
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
        }

        #endregion
    }
}
