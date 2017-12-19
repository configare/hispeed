using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    public class GRIB2Record : IGRIB2Record
    {
        private IGRIB2GridDefinitionSection _gds;
        private IGRIB2ProductDefinitionSection _pds;
        private IGRIB2DataRepresentationSection _drs;
        private GRIB2DataSection _ds;
        private IGribBitMapSection _bms;
        private long _gdsOffset;
        private long _pdsOffset;
        private long _dataOffset;
        private Parameter _parameter;

        public GRIB2Record(int disciplineNo, IGRIB2GridDefinitionSection gds, IGRIB2ProductDefinitionSection pds,
                           IGRIB2DataRepresentationSection drs, IGribBitMapSection bms, GRIB2DataSection ds,
                           long gdsOffset, long pdsOffset, long dataOffset)
        {
            _gdsOffset = gdsOffset;
            _pdsOffset = pdsOffset;
            _dataOffset = dataOffset;
            _gds = gds;
            _pds = pds;
            _drs = drs;
            _bms = bms;
            _ds = ds;
            _parameter = ParametersFactory.GetParameter(disciplineNo, pds.ParameterCategory, pds.ParameterNumber);
        }

        public IGRIB2DataRepresentationSection DRS
        {
            get { return _drs; }
        }

        public IGRIB2GridDefinitionSection GDS
        {
            get { return _gds; }
        }

        public IGribBitMapSection BMS
        {
            get { return _bms; }
        }

        public long GetGdsOffset()
        {
            return _gdsOffset;
        }

        public long GetPdsOffset()
        {
            return _pdsOffset;
        }

        public long DataOffset
        {
            get { return _dataOffset; }
        }

        public string Header
        {
            get { throw new NotImplementedException(); }
        }

        public IGRIB2ProductDefinitionSection PDS
        {
            get { return _pds; }
        }

        public GRIB2DataSection DS
        {
            get { return _ds; }
        }

        public string ParameterName
        {
            get
            {
                if (_parameter == null)
                    return null;
                else
                    return _parameter.Name;
            }
        }
    }
}
