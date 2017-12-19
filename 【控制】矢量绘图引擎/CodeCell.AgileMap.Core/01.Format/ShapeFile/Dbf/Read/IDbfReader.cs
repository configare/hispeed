using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IDbfReader:IDisposable
    {
        int RecordCount { get; }
        string[] Fields { get; }
        string[] GetValues(int oid);
        string GetValue(int oid, string field);
    }
}
