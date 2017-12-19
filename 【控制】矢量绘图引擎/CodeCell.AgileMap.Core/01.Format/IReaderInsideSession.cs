using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace CodeCell.AgileMap.Core
{
    public interface IReaderInsideSession
    {
        Feature[] Read(Envelope envelope, IDbConnection dbConnection);
        Feature[] Read(Envelope envelope, string whereClause, IDbConnection dbConnection);
    }
}
