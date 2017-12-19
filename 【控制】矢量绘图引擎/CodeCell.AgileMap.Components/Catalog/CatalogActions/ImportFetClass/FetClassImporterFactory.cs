using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Components
{
    public static class FetClassImporterFactory
    {
        public static IFetClassImporter GetImporter(ICatalogItem fetclassItem, ICatalogItem locationItem)
        {
            if (fetclassItem is CatalogFile && locationItem is CatalogLocal)
                return new ImporterByCopyFile();
            return new ImporterByReaderWriter();
        }
    }
}
