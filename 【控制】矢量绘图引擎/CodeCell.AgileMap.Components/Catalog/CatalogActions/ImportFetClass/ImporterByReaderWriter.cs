using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    public class ImporterByReaderWriter:IFetClassImporter
    {
        #region IFetClassImporter Members

        public void Import(ICatalogItem fetClassItem, ICatalogItem locationItem, IProgressTracker tracker,string name,string displayName,string description)
        {
            using (IFeatureClassReader fetcReader = FeatureClassReaderFactory.GetFeatureClassReader(fetClassItem))
            {
                using (IFeatureClassWriter fetcWriter = FeatureClassWriterFactory.GetFetClassWriter(locationItem))
                {
                    fetcWriter.Write(fetcReader, tracker,name,displayName, description);
                }
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
