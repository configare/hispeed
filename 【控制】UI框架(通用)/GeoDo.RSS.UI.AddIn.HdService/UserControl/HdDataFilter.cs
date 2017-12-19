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
                new HdDataFilter("FY3A", "VIRR", "FY-3A VIRR"),
                new HdDataFilter("FY3A", "MERSI", "FY-3A MERSI"),
                new HdDataFilter("FY3B", "VIRR", "FY-3B VIRR"),
                new HdDataFilter("FY3B", "MERSI", "FY-3B MERSI")
            };
        }
    }
}
