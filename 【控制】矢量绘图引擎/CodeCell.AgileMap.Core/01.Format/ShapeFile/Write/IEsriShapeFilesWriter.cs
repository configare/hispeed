using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeCell.AgileMap.Core
{
    public interface IEsriShapeFilesWriter
    {
        void BeginWrite();
        void Write(Feature[] features);
        void Write(Feature[] features, Action<int, string> progress);
        void EndWriter();
    }
}
