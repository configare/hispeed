using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public class QueryFilter
    {
        private Shape _geometry = null;
        private enumSpatialRelation _relation = enumSpatialRelation.Intersect;
        private string _whereclause = null;

        public QueryFilter()
        { 
        }

        public QueryFilter(Shape geometry)
            : this()
        {
            _geometry = geometry;
        }

        public QueryFilter(Shape geomtry, enumSpatialRelation rel)
            : this(geomtry)
        {
            _relation = rel;
        }

        public QueryFilter(string whereClause)
            : this()
        {
            _whereclause = whereClause;
        }

        public QueryFilter(Shape geometry, string whereClause)
        {
            _geometry = geometry;
            _whereclause = whereClause;
        }

        public QueryFilter(Shape geometry, enumSpatialRelation rel, string whereClause)
        {
            _geometry = geometry;
            _relation = rel;
            _whereclause = whereClause;
        }

        void Reset()
        {
            _geometry = null;
            _whereclause = null;
            _relation = enumSpatialRelation.Intersect;
        }

        public Shape Geometry
        {
            get { return _geometry; }
            set { _geometry = value; }
        }

        public enumSpatialRelation Relation
        {
            get { return _relation; }
            set { _relation = value; }
        }

        public string WhereClause
        {
            get { return _whereclause; }
            set { _whereclause = value; }
        }
    }
}
