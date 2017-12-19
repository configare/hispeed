using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.UI.AddIn.ReportService
{
    class MonitorProductInfo
    {
        public string productType;
        public string productDataType;                                        //VectorProduct、
        public string productIdentify;                                        //DBLV,0CSR,"PLST";
        public string productIdentifyName;                                    //"火点二值图";
        public string satellite;
        public string sensor;
        public string subProductType = "";                                    //通常为空
        public DateTime orbitDateTime = DateTime.Now;
        public string productArea;
        public string productFileName;
        public string productFilePath;
    }
}
