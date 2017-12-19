using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.RasterTools;

namespace GeoDo.RSS.Core.VectorDrawing
{
    internal class ContourDataStorager
    {
        public void Save(ContourLine[] contLines,  ContourLine.ContourEnvelope envelope, string spatialRef,string fname)
        {
            if (contLines == null || contLines.Length == 0)
                return;
            IContourPersisit persist = new ContourPersist();
            persist.Write(contLines, GeoDo.RSS.RasterTools.ContourPersist.enumCoordType.Prj,
                envelope, spatialRef, fname);
        }
    }
}
