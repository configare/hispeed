using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    public interface IFetClassImporter:IDisposable
    {
        void Import(ICatalogItem fetClassItem, ICatalogItem locationItem, IProgressTracker tracker,string tablename,string displayName,string description);
    }
}
