using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace GeoDo.RSS.UI.AddIn.HdService
{
   public  class DataPathHelper
    {
        public static string MonitorRootPath = ConfigurationManager.AppSettings.Get("DataServerRootPath");
        public static string ProjectionRootPath = ConfigurationManager.AppSettings.Get("DataServerProjetionPath");
        public static string DataServerMosaicPath = ConfigurationManager.AppSettings.Get("DataServerMosaicPath");
        public static string DataServerBlockPath = ConfigurationManager.AppSettings.Get("DataServerBlockPath");
    }
}
