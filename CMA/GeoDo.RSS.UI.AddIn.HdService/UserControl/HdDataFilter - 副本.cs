using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class HdDataFilter
    {
        public HdDataFilter(string satellite, string sensor, string text)
        {
            Satellite = satellite;
            Sensor = sensor;
            Text = text;
        }

        public string Satellite;
        public string Sensor;
        public string Text;

        public static HdDataFilter[] FilterColl()
        {
            return new HdDataFilter[]
            { 
                new HdDataFilter("FY3A", "VIRRX", "FY-3A VIRRX"),
                new HdDataFilter("FY3A", "MERSI", "FY-3A MERSI"),
                new HdDataFilter("FY3B", "VIRRX", "FY-3B VIRRX"),
                new HdDataFilter("FY3B", "MERSI", "FY-3B MERSI"),
                new HdDataFilter("FY3C", "VIRRX", "FY-3C VIRRX"),
                new HdDataFilter("FY3C", "MERSI", "FY-3C MERSI"),
                new HdDataFilter("NA18", "AVHRR", "NOAA18 AVHRR"),
                new HdDataFilter("EOSA", "MODIS", "EOSA MODIS"),
                new HdDataFilter("EOST", "MODIS", "EOST MODIS")
            };
        }
    }
}
