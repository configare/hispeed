using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Core
{
    public static class BandRefTableHelper
    {
        static BandnameRefTable[] _bandRefTables;

        static BandRefTableHelper()
        {
            _bandRefTables = (new BandnameRefTableParser()).Parse();
        }

        public static BandnameRefTable GetBandRefTable(string satellite, string sensor)
        {
            if (_bandRefTables == null || _bandRefTables.Length == 0)
                return null;
            foreach (BandnameRefTable table in _bandRefTables)
                if (table.Satellite != null && table.Sensor != null && table.Satellite.ToUpper() == satellite && table.Sensor.ToUpper() == sensor)
                    return table;
            return null;
        }
    }
}
