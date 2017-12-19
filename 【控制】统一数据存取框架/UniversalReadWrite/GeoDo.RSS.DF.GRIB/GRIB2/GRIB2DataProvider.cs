using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeoDo.RSS.Core.DF;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    public class GRIB2DataProvider : RasterDataProvider,IGRIB2DataProvider
    {
        private GRIB_Definition _definition;
        private FileStream _fs;
        private int _discipline;   //学科
        private List<GRIB2Record> _records;
        private List<string> _parameterList;
        private DateTime _referenceTime;
        /// <summary> Buffer for one byte which will be processed bit by bit.</summary>
        private int _bitBuf = 0;
        /// <summary> Current bit position in <tt>bitBuf</tt>.</summary>
        private int _bitPos = 0;

        public GRIB2DataProvider(string fileName, byte[] header1024, IGeoDataDriver driver, enumDataProviderAccess access)
            : base(fileName, driver)
        {
            _records = new List<GRIB2Record>();
            _parameterList = new List<string>();
            ReadToDataProvider();
        }

        public GRIB_Definition Definition
        {
            get { return _definition; }
        }

        public List<string> ParameterList
        {
            get { return _parameterList; }
        }


        public DateTime ReferenceTime
        {
            get { return _referenceTime; }
        }

        private void ReadToDataProvider()
        {
            if (string.IsNullOrEmpty(_fileName) || !File.Exists(_fileName))
                return;
            long gdsOffset = 0; // GDS offset from start of file
            bool startAtHeader = true; // otherwise skip to GDS
            bool processGDS = true;
            _fs = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
            IGRIB2IndicatorSection iSection = null;
            GRIB2IdentificationSection idSection = null;
            GRIB2LocalUseSection lus = null;
            IGRIB2GridDefinitionSection gds = null;
            while (_fs.Position < _fs.Length)
            {
                if (startAtHeader)
                {
                    // begining of record
                    if (!SeekHeader(_fs, _fs.Length))
                        return;
                    // Read Section 0 Indicator Section
                    iSection = new GRIB2IndicatorSection(_fs); // section 0
                    _discipline = iSection.Displine;
                    // Read other Sections
                    idSection = new GRIB2IdentificationSection(_fs); // Section 1
                    _referenceTime = idSection.ReferenceTime;
                } // end startAtHeader
                if (processGDS)
                {
                    // check for Local Use Section 2
                    lus = new GRIB2LocalUseSection(_fs);
                    // obtain GDS offset in the file for this record
                    gdsOffset = _fs.Position;
                    // Section 3
                    gds = new GRIB2GridDefinitionSection(_fs);
                }
                // obtain PDS offset in the file for this record
                long pdsOffset = _fs.Position;
                IGRIB2ProductDefinitionSection pds = new GRIB2ProductDefinitionSection(_fs); // Section 4
                IGRIB2DataRepresentationSection drs = new GRIB2DataRepresentationSection(_fs); // Section 5
                IGribBitMapSection bms = new GRIB2BitMapSection(_fs, gds.PointsNumber); // Section 6
                long dataOffset = _fs.Position + 5;
                GRIB2DataSection ds = new GRIB2DataSection(_fs); //Section 7
                GRIB2Record record = new GRIB2Record(iSection.Displine, gds, pds, drs,bms, ds, gdsOffset, pdsOffset, dataOffset);
                _records.Add(record);
                _parameterList.Add(record.ParameterName);
                if (_fs.Position > _fs.Length)
                {
                    _fs.Seek(0, System.IO.SeekOrigin.Begin);
                    return ;
                }
                int ending = GribNumberHelper.Int4(_fs);
                if (ending == 926365495)
                {
                    // record ending string 7777 as a number
                    startAtHeader = true;
                    processGDS = true;
                }
                else
                {
                    int section = _fs.ReadByte(); // check if GDS or PDS section, 3 or 4
                    //reset back to begining of section
                    _fs.Seek(_fs.Position - 5, System.IO.SeekOrigin.Begin);
                    if (section == 3)
                    {
                        startAtHeader = false;
                        processGDS = true;
                    }
                    else if (section == 4)
                    {
                        startAtHeader = false;
                        processGDS = false;
                    }
                    else
                    {
                        GribEndSection es = new GribEndSection(_fs);
                        if (es.IsEndFound)
                        {
                            startAtHeader = true;
                            processGDS = true;
                        }
                        else
                            return;
                    }
                }
            } 
        }

        private bool SeekHeader(FileStream fs, long stop)
        {
            // 搜寻记录头
            StringBuilder hdr = new StringBuilder();
            int match = 0;
            while (fs.Position < stop)
            {
                // 代码必须是 "G" "R" "I" "B"
                sbyte c = (sbyte)fs.ReadByte();
                if (c < 0)
                    c = (sbyte)' ';

                hdr.Append((char)c);
                if (c == 'G')
                {
                    match = 1;
                }
                else if ((c == 'R') && (match == 1))
                {
                    match = 2;
                }
                else if ((c == 'I') && (match == 2))
                {
                    match = 3;
                }
                else if ((c == 'B') && (match == 3))
                {
                    return true;
                }
                else
                {
                    match = 0;
                }
            }
            return false;
        }

        public override void AddBand(enumDataType dataType)
        {
            throw new NotImplementedException();
        }

        public GRIB_Point[] Read(string parameter)
        {
            if (_records.Count < 1)
                return null;
            foreach (GRIB2Record record in _records)
            {
                if (record.ParameterName.ToUpper() == parameter.ToUpper())
                {
                    _fs.Seek(record.DataOffset, SeekOrigin.Begin);
                    switch (record.DRS.DataTemplate)
                    {
                        case 0:
                            {
                                // 0: Grid point data - simple packing
                                return SimpleUnpacking(_fs, record.GDS, record.DRS, record.BMS);
                            }
                        case 2:
                            {
                                // 2:Grid point data - complex packing
                                return ComplexUnpacking(_fs, record.GDS, record.DRS);
                            }
                        case 3:
                            {
                                // 3: complex packing with spatial differencing
                                return ComplexUnpackingWithSpatial(_fs, record.GDS, record.DRS);
                            }
                        case 40:
                        case 40000:
                            {
                                // JPEG 2000 Stream Format
                                return Jpeg2000Unpacking(_fs, record.GDS, record.DRS, record.BMS);
                            }
                    }
                }
            }
            return null;
        }

        private GRIB_Point[] Jpeg2000Unpacking(FileStream fs, IGRIB2GridDefinitionSection gds, IGRIB2DataRepresentationSection drs, IGribBitMapSection bms)
        {
            return null;
        }

        private GRIB_Point[] ComplexUnpackingWithSpatial(FileStream fs, IGRIB2GridDefinitionSection gds, IGRIB2DataRepresentationSection drs)
        {
            int mvm = drs.MissingValueManagement;
            float pmv = drs.PrimaryMissingValue;
            int NG = drs.NumberOfGroups;
            int g1 = 0, gMin = 0, h1 = 0, h2 = 0, hMin = 0;
            // [6-ww]   1st values of undifferenced scaled values and minimums
            int os = drs.OrderSpatial;
            int ds = drs.DescriptorSpatial;
            _bitPos = 0; _bitBuf = 0;
            int sign;
            // ds is number of bytes, convert to bits -1 for sign bit
            ds = ds * 8 - 1;
            if (os == 1)
            {
                // first order spatial differencing g1 and gMin
                sign = Bits2UInt(1, fs);
                g1 = Bits2UInt(ds, fs);
                if (sign == 1)
                {
                    g1 *= (-1);
                }
                sign = Bits2UInt(1, fs);
                gMin = Bits2UInt(ds, fs);
                if (sign == 1)
                {
                    gMin *= (-1);
                }
            }
            else if (os == 2)
            {
                //second order spatial differencing h1, h2, hMin
                sign = Bits2UInt(1, fs);
                h1 = Bits2UInt(ds, fs);
                if (sign == 1)
                {
                    h1 *= (-1);
                }
                sign = Bits2UInt(1, fs);
                h2 = Bits2UInt(ds, fs);
                if (sign == 1)
                {
                    h2 *= (-1);
                }
                sign = Bits2UInt(1, fs);
                hMin = Bits2UInt(ds, fs);
                if (sign == 1)
                {
                    hMin *= (-1);
                }
            }
            else
                return null;
            // [ww +1]-xx  Get reference values for groups (X1's)
            int[] X1 = new int[NG];
            int nb = drs.NumberOfBits;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                X1[i] = Bits2UInt(nb, fs);
            }
            // [xx +1 ]-yy Get number of bits used to encode each group
            int[] NB = new int[NG];
            nb = drs.BitsGroupWidths;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                NB[i] = Bits2UInt(nb, fs);
            }
            // [yy +1 ]-zz Get the scaled group lengths using formula
            //     Ln = ref + Kn * len_inc, where n = 1-NG,
            //          ref = referenceGroupLength, and  len_inc = lengthIncrement
            int[] L = new int[NG];
            int countL = 0;
            int ref_Renamed = drs.ReferenceGroupLength;
            int len_inc = drs.LengthIncrement;
            nb = drs.BitsScaledGroupLength;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                // NG
                L[i] = ref_Renamed + (Bits2UInt(nb, fs) * len_inc);
                countL += L[i];
            }
            // [zz +1 ]-nn get X2 values and add X1[ i ] + X2
            GRIB_Point[] data = new GRIB_Point[countL];
            // used to check missing values when X2 is packed with all 1's
            int[] bitsmv1 = new int[31];
            for (int i = 0; i < 31; i++)
            {
                bitsmv1[i] = (int)System.Math.Pow((double)2, (double)i) - 1;
            }
            int count = 0;
            int X2;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG - 1; i++)
            {
                for (int j = 0; j < L[i]; j++)
                {
                    data[count].Index = count;
                    if (NB[i] == 0)
                    {
                        if (mvm == 0)
                        {
                            // X2 = 0
                            data[count++].Value = X1[i];
                        }
                        else if (mvm == 1)
                        {
                            data[count++].Value = pmv;
                        }
                    }
                    else
                    {
                        X2 = Bits2UInt(NB[i], fs);
                        if (mvm == 0)
                        {
                            data[count++].Value = X1[i] + X2;
                        }
                        else if (mvm == 1)
                        {
                            // X2 is also set to missing value is all bits set to 1's
                            if (X2 == bitsmv1[NB[i]])
                            {
                                data[count++].Value = pmv;
                            }
                            else
                            {
                                data[count++].Value = X1[i] + X2;
                            }
                        }
                    }
                } 
            } 
            // process last group
            int last = drs.LengthLastGroup;
            for (int j = 0; j < last; j++)
            {
                data[count].Index = count;
                // last group
                if (NB[NG - 1] == 0)
                {
                    if (mvm == 0)
                    {
                        // X2 = 0
                        data[count++].Value = X1[NG - 1];
                    }
                    else if (mvm == 1)
                    {
                        data[count++].Value = pmv;
                    }
                }
                else
                {
                    X2 = Bits2UInt(NB[NG - 1], fs);
                    if (mvm == 0)
                    {
                        data[count++].Value = X1[NG - 1] + X2;
                    }
                    else if (mvm == 1)
                    {
                        // X2 is also set to missing value is all bits set to 1's
                        if (X2 == bitsmv1[NB[NG - 1]])
                        {
                            data[count++].Value = pmv;
                        }
                        else
                        {
                            data[count++].Value = X1[NG - 1] + X2;
                        }
                    }
                }
            } 
            if (os == 1)
            {
                // g1 and gMin this coding is a sort of guess, no doc
                float sum = 0;
                if (mvm == 0)
                {
                    // no missing values
                    for (int i = 1; i < data.Length; i++)
                    {
                        data[i].Value += gMin; // add minimum back
                    }
                    data[0].Value = g1;
                    for (int i = 1; i < data.Length; i++)
                    {
                        sum += data[i].Value;
                        data[i].Value = data[i - 1].Value + sum;
                    }
                }
                else
                {
                    // contains missing values
                    float lastOne = pmv;
                    // add the minimum back and set g1
                    int idx = 0;
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i].Value != pmv)
                        {
                            if (idx == 0)
                            {
                                // set g1
                                data[i].Value = g1;
                                lastOne = data[i].Value;
                                idx = i + 1;
                            }
                            else
                            {
                                data[i].Value += gMin;
                            }
                        }
                    }
                    if (lastOne == pmv)
                        return data;
                    for (int i = idx; i < data.Length; i++)
                    {
                        if (data[i].Value != pmv)
                        {
                            sum += data[i].Value;
                            data[i].Value = lastOne + sum;
                            lastOne = data[i].Value;
                        }
                    }
                }
            }
            else if (os == 2)
            {
                //h1, h2, hMin
                float hDiff = h2 - h1;
                float sum = 0;
                if (mvm == 0)
                {
                    // no missing values
                    for (int i = 2; i < data.Length; i++)
                    {
                        data[i].Value += hMin; // add minimum back
                    }
                    data[0].Value = h1;
                    data[1].Value = h2;
                    sum = hDiff;
                    for (int i = 2; i < data.Length; i++)
                    {
                        sum += data[i].Value;
                        data[i].Value = data[i - 1].Value + sum;
                    }
                }
                else
                {
                    // contains missing values
                    int idx = 0;
                    float lastOne = pmv;
                    // add the minimum back and set h1 and h2
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i].Value != pmv)
                        {
                            if (idx == 0)
                            {
                                // set h1
                                data[i].Value = h1;
                                sum = 0;
                                lastOne = data[i].Value;
                                idx++;
                            }
                            else if (idx == 1)
                            {
                                // set h2
                                data[i].Value = h1 + hDiff;
                                sum = hDiff;
                                lastOne = data[i].Value;
                                idx = i + 1;
                            }
                            else
                            {
                                data[i].Value += hMin;
                            }
                        }
                    }
                    if (lastOne == pmv)
                        return data;
                    for (int i = idx; i < data.Length; i++)
                    {
                        if (data[i].Value != pmv)
                        {
                            sum += data[i].Value;
                            data[i].Value = lastOne + sum;
                            lastOne = data[i].Value;
                        }
                    }
                }
            } // end h1, h2, hMin
            // formula used to create values,  Y * 10**D = R + (X1 + X2) * 2**E
            int D = drs.DecimalScaleFactor;
            float DD = (float)System.Math.Pow((double)10, (double)D);         
            float R = drs.ReferenceValue;       
            int E = drs.BinaryScaleFactor; 
            float EE = (float)System.Math.Pow((double)2.0, (double)E);
            if (mvm == 0)
            {
                // no missing values
                for (int i = 0; i < data.Length; i++)
                {
                    data[i].Value = (R + data[i].Value * EE) / DD;
                }
            }
            else
            {
                // missing value == 1
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Value != pmv)
                    {
                        data[i].Value = (R + data[i].Value * EE) / DD;
                    }
                }
            }
            ScanningModeCheck(data,gds.ScanMode,gds.Nx);
            return data;
        }

        private GRIB_Point[] ComplexUnpacking(FileStream fs, IGRIB2GridDefinitionSection gds, IGRIB2DataRepresentationSection drs)
        {
            int mvm = drs.MissingValueManagement;  
            float pmv = drs.PrimaryMissingValue;
            int NG = drs.NumberOfGroups;
            // 6-xx  Get reference values for groups (X1's)
            int[] X1 = new int[NG];
            int nb = drs.NumberOfBits;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                X1[i] = Bits2UInt(nb, fs);
            }
            // [xx +1 ]-yy Get number of bits used to encode each group
            int[] NB = new int[NG];
            nb = drs.BitsGroupWidths;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                NB[i] = Bits2UInt(nb, fs);
               
            }
            // [yy +1 ]-zz Get the scaled group lengths using formula
            //     Ln = ref + Kn * len_inc, where n = 1-NG,
            //          ref = referenceGroupLength, and  len_inc = lengthIncrement
            int[] L = new int[NG];
            int countL = 0;
            int ref_Renamed = drs.ReferenceGroupLength;
            int len_inc = drs.LengthIncrement;
            nb = drs.BitsScaledGroupLength;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG; i++)
            {
                // NG
                L[i] = ref_Renamed + (Bits2UInt(nb, fs) * len_inc);
                countL += L[i];
            }
            // [zz +1 ]-nn get X2 values and calculate the results Y using formula
            //                Y * 10**D = R + (X1 + X2) * 2**E

            int D = drs.DecimalScaleFactor;
            float DD = (float)System.Math.Pow((double)10, (double)D);
            float R = drs.ReferenceValue;
            int E = drs.BinaryScaleFactor;
            float EE = (float)System.Math.Pow((double)2.0, (double)E);
            GRIB_Point[] data = new GRIB_Point[countL];
            int count = 0;
            int[] bitsmv1 = new int[31];
            for (int i = 0; i < 31; i++)
            {
                bitsmv1[i] = (int)System.Math.Pow((double)2, (double)i) - 1;
            }
            int X2;
            _bitPos = 0;
            _bitBuf = 0;
            for (int i = 0; i < NG - 1; i++)
            {
                for (int j = 0; j < L[i]; j++)
                {
                    data[count].Index = count;
                    if (NB[i] == 0)
                    {
                        if (mvm == 0)
                        {
                            // X2 = 0
                            data[count++].Value = (R + X1[i] * EE) / DD;
                        }
                        else if (mvm == 1)
                        {
                            data[count++].Value = pmv;
                        }
                    }
                    else
                    {
                        X2 = Bits2UInt(NB[i], fs);
                        if (mvm == 0)
                        {
                            data[count++].Value = (R + (X1[i] + X2) * EE) / DD;
                        }
                        else if (mvm == 1)
                        {
                            // X2 is also set to missing value is all bits set to 1's
                            if (X2 == bitsmv1[NB[i]])
                            {
                                data[count++].Value = pmv;
                            }
                            else
                            {
                                data[count++].Value = (R + (X1[i] + X2) * EE) / DD;
                            }
                        }
                    }
                } 
            } 
            // process last group
            int last = drs.LengthLastGroup;
            for (int j = 0; j < last; j++)
            {
                data[count].Index = count;
                // last group
                if (NB[NG - 1] == 0)
                {
                    if (mvm == 0)
                    {
                        // X2 = 0
                        data[count++].Value = (R + X1[NG - 1] * EE) / DD;
                    }
                    else if (mvm == 1)
                    {
                        data[count++].Value = pmv;
                    }
                }
                else
                {
                    X2 = Bits2UInt(NB[NG - 1], fs);
                    if (mvm == 0)
                    {
                        data[count++].Value = (R + (X1[NG - 1] + X2) * EE) / DD;
                    }
                    else if (mvm == 1)
                    {
                        // X2 is also set to missing value is all bits set to 1's
                        if (X2 == bitsmv1[NB[NG - 1]])
                        {
                            data[count++].Value = pmv;
                        }
                        else
                        {
                            data[count++].Value = (R + (X1[NG - 1] + X2) * EE) / DD;
                        }
                    }
                }
            } // end for j
            ScanningModeCheck(data, gds.ScanMode, gds.Nx);
            return data;
        }

        private void ScanningModeCheck(GRIB_Point[] data, int scanMode,int nx)
        {
            // Mode  0 +x, -y, adjacent x, adjacent rows same dir
            // Mode  64 +x, +y, adjacent x, adjacent rows same dir
            if (scanMode == 0 || scanMode == 64)
                return;
            // Mode  128 -x, -y, adjacent x, adjacent rows same dir
            // Mode  192 -x, +y, adjacent x, adjacent rows same dir
            // change -x to +x ie east to west -> west to east
            else if (scanMode == 128 || scanMode == 192)
            {
                float tmp;
                int mid = (int)nx / 2;
                //System.out.println( "Xlength =" +Xlength +" mid ="+ mid );
                for (int index = 0; index < data.Length; index += nx)
                {
                    for (int idx = 0; idx < mid; idx++)
                    {
                        tmp = data[index + idx].Value;
                        data[index + idx].Value = data[index + nx - idx - 1].Value;
                        data[index + nx - idx - 1].Value = tmp;
                    }
                }
                return;
            }
            // scanMode == 16, 80, 144, 208 adjacent rows scan opposite dir
            float tmp2;
            int mid2 = (int)nx / 2;
            for (int index = 0; index < data.Length; index += nx)
            {
                int row = (int)index / nx;
                if (row % 2 == 1)
                {
                    // odd numbered row, calculate reverse index
                    for (int idx = 0; idx < mid2; idx++)
                    {
                        tmp2 = data[index + idx].Value;
                        data[index + idx].Value = data[index + nx - idx - 1].Value;
                        data[index + nx - idx - 1].Value = tmp2;
                    }
                }
            }
        }

        private GRIB_Point[] SimpleUnpacking(FileStream fs, IGRIB2GridDefinitionSection gds, IGRIB2DataRepresentationSection drs, IGribBitMapSection bms)
        {
            // dataPoints are number of points encoded, it could be less than the
            // numberPoints in the grid record if bitMap is used, otherwise equal
            _bitPos = 0;
            _bitBuf = 0;
            int dataPoints = drs.DataPoints;
            float pmv = drs.PrimaryMissingValue;
            int nb = drs.NumberOfBits;
            int D = drs.DecimalScaleFactor;
            float DD = (float)System.Math.Pow((double)10, (double)D);
            float R = drs.ReferenceValue;
            int E = drs.BinaryScaleFactor;
            float EE = (float)System.Math.Pow((double)2.0, (double)E);
            int numberPoints = gds.PointsNumber;
            GRIB_Point[] data = new GRIB_Point[numberPoints];
            bool[] bitmap = bms.Bitmap;
            //  Y * 10**D = R + (X1 + X2) * 2**E
            //   E = binary scale factor
            //   D = decimal scale factor
            //   R = reference value
            //   X1 = 0
            //   X2 = scaled encoded value
            //   data[ i ] = (R + ( X1 + X2) * EE)/DD ;
            if (bitmap == null)
            {
                for (int i = 0; i < numberPoints; i++)
                {
                    data[i].Index = i;
                    data[i].Value = (R + Bits2UInt(nb, fs) * EE) / DD;
                }
            }
            else
            {
                for (int i = 0; i < bitmap.Length; i++)
                {
                    data[i].Index = i;
                    if (bitmap[i])
                    {
                        //data[ i ] = (R + ( X1 + X2) * EE)/DD ;
                        data[i].Value = (R + Bits2UInt(nb, fs) * EE) / DD;
                    }
                    else
                    {
                        data[i].Value = pmv;
                    }
                }
            }
            return data;
        }

        private int Bits2UInt(int nb, FileStream raf)
        {
            int bitsLeft = nb;
            int result = 0;
            if (_bitPos == 0)
            {
                _bitBuf = raf.ReadByte();
                _bitPos = 8;
            }
            while (true)
            {
                int shift = bitsLeft - _bitPos;
                if (shift > 0)
                {
                    // Consume the entire buffer
                    result |= _bitBuf << shift;
                    bitsLeft -= _bitPos;

                    // Get the next byte from the RandomAccessFile
                    _bitBuf = raf.ReadByte();
                    _bitPos = 8;
                }
                else
                {
                    // Consume a portion of the buffer
                    result |= _bitBuf >> -shift;
                    _bitPos -= bitsLeft;
                    _bitBuf &= 0xff >> (8 - _bitPos); // mask off consumed bits
                    return result;
                }
            }
        } 

        public GRIB_Point[] Read()
        {
            throw new NotImplementedException();
        }

        public GRIB_Point[] Read(CoordEnvelope geoEnvelope)
        {
            throw new NotImplementedException();
        }

        public void Read(IntPtr buffer)
        {
        }

        public void Read(IntPtr buffer, CoordEnvelope geoEnvelope)
        {
        }

        public void StatMinMax(GRIB_Point[] pts, out GRIB_Point minPoint, out GRIB_Point maxPoint)
        {
            throw new NotImplementedException();
        }

        public IArrayRasterDataProvider ToArrayDataProvider(GRIB_Point[] points)
        {
            throw new NotImplementedException();
        }
    }
}
