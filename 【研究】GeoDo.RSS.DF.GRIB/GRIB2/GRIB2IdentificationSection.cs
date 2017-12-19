using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// GRIB2标识段 (Section 1)：段长、段号
    /// </summary>
    public class GRIB2IdentificationSection
    {
        private GRIB2SectionHeader _sectionHeader;
        private int _centerId;
        private int _subcenterId;
        private int _masterTableVersionNo;
        private int _locationTableVersion;
        private int _significanceOfRT;
        private DateTime _referenceTime;
        private int _productStatus;
        private int _productType;

        public GRIB2IdentificationSection(FileStream fs)
        {
            //段长度，段号
            long position = fs.Position;
            _sectionHeader = new GRIB2SectionHeader(fs);
            if (_sectionHeader.SectionNo != 1)
                return;
            _centerId = GribNumberHelper.Int2(fs);
            _subcenterId = GribNumberHelper.Int2(fs);
            _masterTableVersionNo = fs.ReadByte();
            _locationTableVersion = fs.ReadByte();
            _significanceOfRT = fs.ReadByte();
            int year = GribNumberHelper.Int2(fs);
            int month = fs.ReadByte();
            int day = fs.ReadByte();
            int hour = fs.ReadByte();
            int minute = fs.ReadByte();
            int second = fs.ReadByte();
            _referenceTime = new DateTime(year, month, day, hour, minute, second, DateTimeKind.Utc);
            _productStatus = fs.ReadByte();
            _productType = fs.ReadByte();
            fs.Seek(position + _sectionHeader.SectionLength, SeekOrigin.Begin);
        }

        public int SectionNo
        {
            get { return _sectionHeader.SectionNo; }
        }

        public string CenterName
        {
            get { return GetCenterNameById(); }
        }

        public string SignificanceOfRTName
        {
            get { return GetSignificanceOfRTName(); }
        }
 
        public DateTime ReferenceTime
        {
            get { return _referenceTime; }
        }

        public string ProductStatusName
        {
            get { return GetProductStatusName(); }
        }

        public string ProductTypeName
        {
            get { return GetProductTypeName(); }
        }

        private string GetCenterNameById()
        {
            switch (_centerId)
            {
                case 0: return "WMO Secretariat";
                case 1:
                case 2: return "Melbourne";
                case 4:
                case 5: return "Moscow";
                case 7: return "US National Weather Service (NCEP)";
                case 8: return "US National Weather Service (NWSTG)";
                case 9: return "US National Weather Service (other)";
                case 10: return "Cairo (RSMC/RAFC)";
                case 12: return "Dakar (RSMC/RAFC)";
                case 14: return "Nairobi (RSMC/RAFC)";
                case 18: return "Tunis Casablanca (RSMC)";
                case 20: return "Las Palmas (RAFC)";
                case 21: return "Algiers (RSMC)";
                case 24: return "Pretoria (RSMC)";
                case 25: return "La R?union (RSMC)";
                case 26: return "Khabarovsk (RSMC)";
                case 28: return "New Delhi (RSMC/RAFC)";
                case 30: return "Novosibirsk (RSMC)";
                case 32: return "Tashkent (RSMC)";
                case 33: return "eddah (RSMC)";
                case 34: return "Tokyo (RSMC), Japan Meteorological Agency";
                case 36: return "Bangkok";
                case 37: return "Ulan Bator";
                case 38: return "Beijing (RSMC)";
                case 40: return "Seoul";
                case 41: return "Buenos Aires (RSMC/RAFC)";
                case 43: return "Brasilia (RSMC/RAFC)";
                case 45: return "Santiago";
                case 46: return "Brazilian Space Agency ? INPE";
                case 51: return "Miami (RSMC/RAFC)";
                case 52: return "Miami RSMC, National Hurricane Center";
                case 53: return "Montreal (RSMC)";
                case 55: return "San Francisco";
                case 57: return "Air Force Weather Agency";
                case 58: return "Fleet Numerical Meteorology and Oceanography Center";
                case 59: return "The NOAA Forecast Systems Laboratory";
                case 60: return "United States National Centre for Atmospheric Research (NCAR)";
                case 64: return "Honolulu";
                case 65: return "Darwin (RSMC)";
                case 67: return "Melbourne (RSMC)";
                case 69: return "Wellington (RSMC/RAFC)";
                case 71: return "Nadi (RSMC)";
                case 74: return "UK Meteorological Office Bracknell (RSMC)";
                case 76: return "Moscow (RSMC/RAFC)";
                case 78: return "Offenbach (RSMC)";
                case 80: return "Rome (RSMC)";
                case 82: return "Norrk?ping";
                case 85: return "Toulouse (RSMC)";
                case 86: return "Helsinki";
                case 87: return "Belgrade";
                case 88: return "Oslo";
                case 89: return "Prague";
                case 90: return "Episkopi";
                case 91: return "Ankara";
                case 92: return "Frankfurt/Main (RAFC)";
                case 93: return "London (WAFC)";
                case 94: return "Copenhagen";
                case 95: return "Rota";
                case 96: return "Athens";
                case 97: return "European Space Agency (ESA)";
                case 98: return "ECMWF, RSMC";
                case 99: return "De Bilt";
                case 110: return "Hong-Kong";
                case 210: return "Frascati (ESA/ESRIN)";
                case 211: return "Lanion";
                case 212: return "Lisboa";
                case 213: return "Reykjavik";
                case 254: return "EUMETSAT Operation Centre";
                default: return "Unknown";
            }
        }

        private string GetSignificanceOfRTName()
        {
            switch (_significanceOfRT)
            {
                case 0: return "Analysis";
                case 1: return "Start of Forecast";
                case 2: return "Verifying time of Forecast";
                case 3: return "Observation Time";
                default: return "Unknown";            
            }
        }

        private string GetProductStatusName()
        {
            switch (_productStatus)
            {
                case 0: return "Operational Products";
                case 1: return "Operational test Products";
                case 2: return "Research Products";
                case 3: return "Re-analysis Products";
                case 4: return "THORPEX Interactive Grand Global Ensemble(TIGGE)";
                case 5: return "THORPEX Interactive Grand Global Ensemble(TIGGE) test";
                case 6: return "S25 Operational Products";
                case 7: return "S25 Test Products";
                default: return "Unknown";
            }
        }

        private string GetProductTypeName()
        {
            switch (_productType)
            {
                case 0: return "Analysis Products";
                case 1: return "Forecast Products";
                case 2: return "Analysis and Forecast Products";
                case 3: return "Control Forecast Products";
                case 4: return "Perturbed Forecast Products";
                case 5: return "Control and Perturbed Forecast Products";
                case 6: return "Processed Satellite Observations";
                case 7: return "Processed Radar Observations";
                case 8: return "Event Probability";
                default: return "Unknown";
            }
        }

    }
}
