using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;

namespace GeoDo.FileProject
{
    public class PrjBandTable
    {
        public static PrjBand[] GetDefaultBandTable(string satelite, string sensor, string resolution)
        {
            if (sensor == "VIRR")
            {
                return OrbitBandDefCollection.GetOrbitBandDefCollection("FY3A", "VIRR", 0.01f);
            }
            else if (sensor == "MERSI"&&resolution =="0250M")
            {
                return OrbitBandDefCollection.GetOrbitBandDefCollection("FY3A", "MERSI", 0.0025f);
            }
            else if (sensor == "MERSI"&&resolution =="1000M")
            {
                return OrbitBandDefCollection.GetOrbitBandDefCollection("FY3A", "MERSI", 0.01f);
            }
            else if (satelite == "FY1D" && sensor == "AVHRR")
            {
                return OrbitBandDefCollection.GetOrbitBandDefCollection("FY1D", "AVHRR", 0.04f);
            }
            else if (satelite =="NOAA"||sensor == "AVHRR")
            {
                return OrbitBandDefCollection.GetOrbitBandDefCollection("NOAA", "AVHRR", 0.01f);
            }
            else if ((satelite == "EOS" || sensor == "MODIS") && resolution == "0250M")
            {
                return OrbitBandDefCollection.GetOrbitBandDefCollection("EOS", "MODIS", 0.0025f);
            }
            else if ((satelite == "EOS" || sensor == "MODIS") && resolution == "0500M")
            {
                return OrbitBandDefCollection.GetOrbitBandDefCollection("EOS", "MODIS", 0.005f);
            }
            else if (satelite == "EOS" || sensor == "MODIS")
            {
                return OrbitBandDefCollection.GetOrbitBandDefCollection("EOS", "MODIS", 0.01f);
            }
            else if (satelite == "FY2" || sensor == "VISSR")
            {
                return OrbitBandDefCollection.GetOrbitBandDefCollection("FY2", "VISSR", 0.05f);
            }
            return null;
        }

        public static PrjBand[] GetPrjBands(IRasterDataProvider rasterDataProvider)
        {
            PrjBand[] prjBands = null;
            string fileType = new FileChecker().GetFileType(rasterDataProvider);
            switch (fileType)
            {
                case "VIRR_L1":
                    prjBands = PrjBandTable.GetDefaultBandTable("FY3A", "VIRR", "1000M");
                    break;
                case "FY3C_VIRR_L1":
                    prjBands = PrjBandTable.GetDefaultBandTable("FY3C", "VIRR", "1000M");
                    break;
                case "MERSI_1KM_L1":
                case "MERSI_QKM_L1":
                    prjBands = PrjBandTable.GetDefaultBandTable("FY3A", "MERSI", "1000M");
                    break;
                case "FY3C_MERSI_1KM_L1":
                    prjBands = PrjBandTable.GetDefaultBandTable("FY3C", "MERSI", "1000M");
                    break;
                case "FY3C_MERSI_QKM_L1":
                    prjBands = PrjBandTable.GetDefaultBandTable("FY3C", "MERSI", "0250M");
                    break;
                case "MODIS_1KM_L1":
                    prjBands = PrjBandTable.GetDefaultBandTable("EOS", "MODIS", "1000M");
                    break;
                case "NOAA_1BD":
                    prjBands = PrjBandTable.GetDefaultBandTable("NOAA", "AVHRR", "1000M");
                    break;
                case "FY2NOM":
                    prjBands = PrjBandTable.GetDefaultBandTable("FY2", "VISSR", "5000M");
                    break;
                case "FY1X_1A5":
                    prjBands = PrjBandTable.GetDefaultBandTable("FY1D", "AVHRR", "4000M");
                    break;
                case "PROJECTED":
                    break;
                default:
                    break;
            }
            return prjBands;
        }
    }
}
