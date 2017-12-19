using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.Bricks.UIs;
using CodeCell.AgileMap.Core;

namespace CodeCell.AgileMap.Components
{
    internal class SpatialDatabaseConn
    {
        private string _name = null;
        private string _description = string.Empty;
        private string _connectionString = null;
        private ICatalogEntity[] _featureDatasets  = null ;
        private ICatalogEntity[] _featureClasses = null;
        private bool _isLoaded = false;

        public SpatialDatabaseConn(string name,string description, string connectionString)
        {
            _name = name;
            _description = description;
            _connectionString = connectionString;
        }

        public string Name
        {
            get { return _name != null ? _name : string.Empty; }
            set { _name = value; }
        }

        public string Description
        {
            get { return _description != null ? _description : string.Empty; }
            set { _description = value; }
        }

        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public bool TryConnection()
        {
            try
            {
                IDbConnection conn = DbConnectionFactory.CreateDbConnection(_connectionString);
                if (conn == null)
                    return false;
                try
                {
                    conn.Open();
                    return true;
                }
                finally
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
            catch (Exception ex)
            {
                MsgBox.ShowError(ex.Message);
            }
            return false;
        }

        public void Refresh()
        {
            _featureClasses = null;
            _featureDatasets = null;
            Load();
        }

        public ICatalogEntity[] GetSpatialFeatureDatasets()
        {
            if (!_isLoaded)
                Load();
            return _featureDatasets;
        }

        public ICatalogEntity[] GetSpatialFeatureClasses()
        {
            if (!_isLoaded)
                Load();
            return _featureClasses;
        }

        private void Load()
        {
            try
            {
                using (ICatalogEntityClass cec = new CatalogEntityClassFeatureDataset(_connectionString))
                {
                    _featureDatasets = cec.Query(new SpatialFeatureDataset());
                }
                using (ICatalogEntityClass cec = new CatalogEntityClassFeatureClass(_connectionString))
                {
                    _featureClasses = cec.Query("length(datasetid)=0");
                }
                _isLoaded = true;
            }
            catch (Exception ex)
            {
                _isLoaded = false;
                MsgBox.ShowError(ex.Message);
            }
        }
    }
}
