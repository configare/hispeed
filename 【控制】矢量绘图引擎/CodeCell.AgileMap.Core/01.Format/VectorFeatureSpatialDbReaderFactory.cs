using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CodeCell.AgileMap.Core
{
    public static class VectorFeatureSpatialDbReaderFactory
    {
        public static IVectorFeatureSpatialDbReader GetVectorFeatureSpatialDbReader(IDbConnection dbConn, string fetclass,bool allowReadAllAtFirst,ArgOfLeveling argOfLeveling)
        {
            return new VectorFeatureSpatialDbReader(dbConn, fetclass, allowReadAllAtFirst, argOfLeveling);
        }
    }
}
