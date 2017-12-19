using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CodeCell.AgileMap.Core;
using CodeCell.Bricks.Runtime;


namespace CodeCell.AgileMap.Components
{
    internal class FetClassReaderForShpFile:IFeatureClassReader,IDisposable
    {
        private IVectorFeatureDataReader _vdr = null;
        private string _annoTable = null;

        public FetClassReaderForShpFile(string shpfilename)
        {
            _vdr = VectorDataReaderFactory.GetUniversalDataReader(shpfilename) as IVectorFeatureDataReader;
            string annf = shpfilename.ToUpper().Replace(".SHP", "_注记.DBF");
            if (File.Exists(annf))
            {
                _annoTable = Path.GetFileNameWithoutExtension(shpfilename) + "_注记";
            }
        }

        #region IFeatureClassReader Members

        public Envelope Envelope
        {
            get { return _vdr.Envelope; }
        }

        public enumShapeType ShapeType
        {
            get { return _vdr.ShapeType; }
        }

        public string AnnoTable
        {
            get { return _annoTable; }
        }

        public ISpatialReference SpatialReference
        {
            get { return _vdr.SpatialReference; }
        }

        public string[] FieldNames
        {
            get { return _vdr.Fields; }
        }

        public Feature[] Read(IProgressTracker tracker)
        {
            return _vdr.Features;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            if (_vdr != null)
            {
                _vdr.Dispose();
                //_vdr = null;
            }
        }

        #endregion
    }
}
