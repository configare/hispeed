using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.AgileMap.Core;
using CodeCell.AgileMap.Components;

namespace CodeCell.AgileMap.WebDataServer
{
    internal class CatalogProviderSpatialDb:ICatalogProvider
    {
        private string _connectionString = null;

        public CatalogProviderSpatialDb(string connectionString)
        {
            _connectionString = connectionString;
        }

        #region ICatalogProvider Members

        public FetDatasetIdentify[] GetFetDatasetIdentify()
        {
            using (CatalogEntityClassFeatureDataset c = new CatalogEntityClassFeatureDataset(_connectionString))
            {
                SpatialFeatureDataset ds = new SpatialFeatureDataset();
                ICatalogEntity[] dses = c.Query(ds);
                if (dses != null && dses.Length > 0)
                {
                    FetDatasetIdentify[] dsIds = new FetDatasetIdentify[dses.Length];
                    int i = 0;
                    foreach (SpatialFeatureDataset d in dses)
                    {
                        dsIds[i] = new FetDatasetIdentify(d.Id, d.Name, d.Description, GetFetClassIdentify(d.SpatialFeatureClasses));
                        i++;
                    }
                    return dsIds;
                }
            }
            return null;
        }

        private FetClassIdentify[] GetFetClassIdentify(ICatalogEntity[] fetclasses)
        {
            if (fetclasses == null || fetclasses.Length == 0)
                return null;
            FetClassIdentify[] fetcIds = new FetClassIdentify[fetclasses.Length];
            for (int i = 0; i < fetclasses.Length; i++)
                fetcIds[i] = new FetClassIdentify(fetclasses[i].Id, fetclasses[i].Name, fetclasses[i].Description);
            return fetcIds;
        }

        public FetClassIdentify[] GetFetClassIdentify()
        {
            using (CatalogEntityClassFeatureClass c = new CatalogEntityClassFeatureClass(_connectionString))
            {
                SpatialFeatureClass temp = new SpatialFeatureClass();
                return GetFetClassIdentify(c.Query("length(datasetid)=0"));
            }
        }

        public FetClassProperty GetFetClassProperty(string fetclassId)
        {
            using (CatalogEntityClassFeatureClass c = new CatalogEntityClassFeatureClass(_connectionString))
            {
                ICatalogEntity[] fetcs = c.Query("id='" + fetclassId + "'");
                if (fetcs == null || fetcs.Length == 0)
                    return null;
                return SpatialFeatureClassToProperty(fetcs[0] as SpatialFeatureClass);
            }
        }

        private FetClassProperty SpatialFeatureClassToProperty(SpatialFeatureClass spatialFeatureClass)
        {
            FetClassProperty p = new FetClassProperty();
            ISpatialReference spref = null;
            p.CoordinateType = GetCoordinateType(spatialFeatureClass,out spref);
            p.Fields = GetFieldsBySpatialFetClass(spatialFeatureClass);
            p.FullEnvelope = spatialFeatureClass.Envelope;
            p.Name = spatialFeatureClass.Name;
            p.ShapeType = spatialFeatureClass.ShapeType;
            p.SpatialReference = spref;
            p.FeatureCount = spatialFeatureClass.FeatureCount;
            return p;
        }

        private enumCoordinateType GetCoordinateType(SpatialFeatureClass spatialFeatureClass,out ISpatialReference spref)
        {
            spref = null;
            try
            {
                ISpatialReference rf = SpatialReferenceFactory.GetSpatialReferenceByPrjFile(spatialFeatureClass.SpatialRef) as ISpatialReference;
                spref = rf ;
                if (rf == null)
                    goto ensureByEnvelopeLine;
                if (rf.ProjectionCoordSystem == null)
                    return enumCoordinateType.Geographic;
                else
                    return enumCoordinateType.Projection;
            }
            catch
            {
                goto ensureByEnvelopeLine;
            }
        ensureByEnvelopeLine:
            return spatialFeatureClass.Envelope.IsGeoRange() ? enumCoordinateType.Geographic : enumCoordinateType.Projection;
        }

        private string[] GetFieldsBySpatialFetClass(SpatialFeatureClass spatialFeatureClass)
        {
            using (IDbConnection dbConn = DbConnectionFactory.CreateDbConnection(_connectionString))
            {
                dbConn.Open();
                DiffDbAdapter _adapter = DiffDbAdapterFactory.GetDiffDbAdapter(dbConn);
                string shapeField = null;
                string oidfield = null ;
                Dictionary<string, Type> fields = _adapter.GetFieldsOfTable(dbConn, spatialFeatureClass.DataTable, out shapeField, out oidfield);
                return fields.Keys.ToArray();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

    }
}
