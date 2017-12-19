using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    public interface IDataImporter:IDisposable
    {
        void Import(Feature[] features, MetaFeatureClassDef fetclassdef, IDbConnection dbConnection, IProgressTracker tracker, params object[] args);
    }
}
