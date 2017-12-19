using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    public class GRIB2ProductDefinitionSection:IGRIB2ProductDefinitionSection
    {
        private GRIB2SectionHeader _sectionHeader;
        private int _coordinates;
        private int _productDefinitionTemplateNo;
        private int _parameterCategory;
        private int _parameterNumber;

        private int _typeGenProcess;    //Type of generating process
        private int _backGenProcess;    //Background generating process identifier 
        private int _analysisGenProcess;//Analysis or forecast generating process identified
        private int _hoursAfter;        //Hours of observational data cutoff after reference time
        private int _minutesAfter;      //Minutes of observational data cutoff after reference time
        private int _timeRangeUnit;     //Indicator of unit of time range
        private int _forecastTime;      //Forecast time in units defined by octet 18
        private int _typeFirstFixedSurface;//Type of first fixed surface
        private float _firstFixedSurfaceValue;
        private int _typeSecondFixedSurface;
        private float _secondFixedSurfaceValue;
        private int _nb;                   //Number of contributing spectral bands(Template 30)

        public GRIB2ProductDefinitionSection(FileStream fs)
        {
            _sectionHeader = new GRIB2SectionHeader(fs);
            _coordinates = GribNumberHelper.Int2(fs);
            _productDefinitionTemplateNo = GribNumberHelper.Int2(fs);
            SetOtherAttributeByProductDefinition(fs);
        }

        public int ProductDefinitionTemplateNo
        {
            get { return _productDefinitionTemplateNo; }
        }

        public string ProductDefinitionTemplateName
        {
            get { return GetProductDefinitionTemplateName(); }
        }

        public string TimeRangeUnitString
        {
            get { return GetTimeRangeUnitString(); }
        }
            
        public int ParameterCategory
        {
            get { return _parameterCategory; }
        }

        public int ParameterNumber
        {
            get { return _parameterNumber; }
        }

        private string GetTimeRangeUnitString()
        {
            switch (_timeRangeUnit)
            {
                case 0: return "minutes";
                case 1: return "hours";
                case 2: return "days";
                case 3: return "months";
                case 4: return "years";
                case 5: return "decade";
                case 6: return "normal";
                case 7: return "century";
                case 10: return "3hours";
                case 11: return "6hours";
                case 12: return "12hours";
                case 13: return "secs";
            }
            return "unknown";
        }

        private string GetProductDefinitionTemplateName()
        {
            return GetProductDefinitionName(_productDefinitionTemplateNo);
        }

        private string GetProductDefinitionName(int productDefinition)
        {
            switch (productDefinition)
            {
                case 0: return "Analysis/forecast at horizontal level/layer";
                case 1: return "Individual ensemble forecast, control and perturbed";
                case 2: return "Derived forecast on all ensemble members";
                case 3: return "Derived forecasts on cluster of ensemble members over rectangular area";
                case 4: return "Derived forecasts on cluster of ensemble members over circular area";
                case 5: return "Probability forecasts at a horizontal level";
                case 6: return "Percentile forecasts at a horizontal level";
                case 7: return "Analysis or forecast error at a horizontal level";
                case 8: return "Average, accumulation, extreme values or other statistically processed value at a horizontal level";
                case 20: return "Radar product";
                case 30: return "Satellite product";
                case 254: return "CCITTIA5 character string";
                default: return "Unknown";
            }
        }

        private void SetOtherAttributeByProductDefinition(FileStream fs)
        {
            // octet 10
            _parameterCategory = fs.ReadByte();
            // octet 11
            _parameterNumber = fs.ReadByte();
            switch (_productDefinitionTemplateNo)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    {
                        // octet 12
                        _typeGenProcess = fs.ReadByte();
                        // octet 13
                        _backGenProcess = fs.ReadByte();
                        // octet 14
                        _analysisGenProcess = fs.ReadByte();
                        // octet 15-16
                        _hoursAfter = GribNumberHelper.Int2(fs);
                        // octet 17
                        _minutesAfter = fs.ReadByte();
                        // octet 18
                        _timeRangeUnit = fs.ReadByte();             
                        // octet 19-22
                        _forecastTime = GribNumberHelper.Int4(fs);            
                        // octet 23
                        _typeFirstFixedSurface = fs.ReadByte();              
                        // octet 24
                        int scaleFirstFixedSurface = fs.ReadByte();                    
                        // octet 25-28
                        int valueFirstFixedSurface = GribNumberHelper.Int4(fs);                       
                        _firstFixedSurfaceValue = (float)((scaleFirstFixedSurface == 0 || valueFirstFixedSurface == 0) ? valueFirstFixedSurface : System.Math.Pow(valueFirstFixedSurface, -scaleFirstFixedSurface));
                        // octet 29
                        _typeSecondFixedSurface = fs.ReadByte();  
                        // octet 30
                        int scaleSecondFixedSurface = fs.ReadByte();              
                        // octet 31-34
                        int valueSecondFixedSurface = GribNumberHelper.Int4(fs);             
                        _secondFixedSurfaceValue = (float)((scaleSecondFixedSurface == 0 || valueSecondFixedSurface == 0) ? valueSecondFixedSurface : System.Math.Pow(valueSecondFixedSurface, -scaleSecondFixedSurface));
                        if (_productDefinitionTemplateNo == 8)
                        {
                            //  35-41 bytes
                            int year = GribNumberHelper.Int2(fs);
                            int month = (fs.ReadByte()) - 1;
                            int day = fs.ReadByte();
                            int hour = fs.ReadByte();
                            int minute = fs.ReadByte();
                            int second = fs.ReadByte();             
                            // 42 - 46
                            int timeRanges = fs.ReadByte();            
                            int missingDataValues = GribNumberHelper.Int4(fs);                 
                            // 47 - 48
                            int outmostTimeRange = fs.ReadByte();                 
                            int missing = fs.ReadByte();               
                            // 49 - 53
                            int statisticalProcess = fs.ReadByte();
                            int timeIncrement = GribNumberHelper.Int4(fs);             
                            // 54 - 58
                            int indicatorTR = fs.ReadByte();
                            int lengthTR = GribNumberHelper.Int4(fs);                   
                        }
                        break;
                    }
                case 20:
                    {
                        _typeGenProcess = fs.ReadByte();
                        // octet 13
                        _backGenProcess = fs.ReadByte();
                        // octet 14
                        _analysisGenProcess = fs.ReadByte();
                        // octet 15-16
                        _hoursAfter = GribNumberHelper.Int2(fs);
                        // octet 17
                        _minutesAfter = fs.ReadByte();
                        // octet 18
                        _timeRangeUnit = fs.ReadByte();
                        // octet 19-22
                        _forecastTime = GribNumberHelper.Int4(fs);  
                        break;
                    }
                case 30:
                    {
                        // octet 12
                        _typeGenProcess = fs.ReadByte();
                        // octet 13
                        _backGenProcess = fs.ReadByte();
                        // octet 14
                        _nb = fs.ReadByte();
                        fs.Seek(10 * _nb, SeekOrigin.Current);
                        break;
                    }
                case 254:
                    {
                        break;
                    }
            }
        }
    }
}
