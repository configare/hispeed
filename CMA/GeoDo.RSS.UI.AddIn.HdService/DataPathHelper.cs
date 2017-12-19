using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace GeoDo.RSS.UI.AddIn.HdService
{
    public class DataPathHelper
    {
        //public static string MonitorRootPath
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["DataServerRootPath"];
        //    }
        //}

        public static string ProjectionRootPath
        {
            get
            {
                return ConfigurationManager.AppSettings["DataServerProjetionPath"];
            }
        }

        public static string DataServerMosaicPath
        {
            get
            {
                return ConfigurationManager.AppSettings["DataServerMosaicPath"];
            }
        }

        public static string DataServerBlockPath
        {
            get
            {
                return ConfigurationManager.AppSettings["DataServerBlockPath"];
            }
        }
    }
}
