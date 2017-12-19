using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// 数据表示段  section 5：段长、段号、数据值表示法描述
    /// </summary>
    public class GRIB2DataRepresentationSection : IGRIB2DataRepresentationSection
    {
        private GRIB2SectionHeader _sectionHeader;
        /// <summary> Number of Data points.</summary>
        private int _dataPoints;
        /// <summary> Data representation template number.</summary>
        private int _dataTemplate;
        /// <summary> Reference value (R) (IEEE 32-bit floating-point value).</summary>
        private float _referenceValue;
        /// <summary> Binary scale factor (E).</summary>
        private int _binaryScaleFactor;
        /// <summary> Decimal scale factor (D).</summary>
        private int _decimalScaleFactor;
        /// <summary> Number of bits used for each packed value.</summary>
        private int _numberOfBits;
        /// <summary> data type of original field values.</summary>
        private int _originalType;
        /// <summary> Group splitting method used (see Code Table 5.4).</summary>
        private int _splittingMethod;
        /// <summary> Missing value management used (see Code Table 5.5).</summary>
        private int _missingValueManagement;
        /// <summary> Primary missing value substitute.</summary>
        private float _primaryMissingValue = -9999f;
        /// <summary> Secondary missing value substitute.</summary>
        private float _secondaryMissingValue = -9999f;
        /// <summary> NG - Number of groups of data values into which field is split.</summary>
        private int _numberOfGroups;
        /// <summary> Reference for group widths (see Note 12).</summary>
        private int _referenceGroupWidths;
        /// <summary> Number of bits used for the group widths (after the reference value.
        /// in octet 36 has been removed)
        /// </summary>
        private int _bitsGroupWidths;
        /// <summary> Reference for group lengths (see Note 13).</summary>
        private int _referenceGroupLength;
        /// <summary> Length increment for the group lengths (see Note 14).</summary>
        private int _lengthIncrement;
        /// <summary> Length increment for the group lengths (see Note 14).</summary>
        private int _lengthLastGroup;
        /// <summary> Number of bits used for the scaled group lengths (after subtraction of 
        /// the reference value given in octets 38-41 and division by the length 
        /// increment given in octet 42).
        /// </summary>
        private int _bitsScaledGroupLength;
        /// <summary> Order of spatial differencing (see Code Table 5.6).</summary>
        private int _orderSpatial;
        /// <summary> Number of octets required in the Data Section to specify extra
        /// descriptors needed for spatial differencing (octets 6-ww in Data
        /// Template 7.3) .
        /// </summary>
        private int _descriptorSpatial;
        /// <summary> Type compression method used (see Code Table 5.40000).</summary>
        private int _compressionMethod;
        /// <summary> Compression ratio used.</summary>
        private int _compressionRatio;


        public GRIB2DataRepresentationSection(FileStream fs)
        {
            long position = fs.Position;
            _sectionHeader = new GRIB2SectionHeader(fs);
            _dataPoints = GribNumberHelper.Int4(fs);
            _dataTemplate = (int)GribNumberHelper.Uint2(fs);
            SetOtherAttributeByDataTemplateNo(fs);
            fs.Seek(position + _sectionHeader.SectionLength, SeekOrigin.Begin);
        }

        public GRIB2SectionHeader SectionHeader
        {
            get { return _sectionHeader; }
        }

        public int DataTemplate
        {
            get { return _dataTemplate; }
        }

        public float ReferenceValue
        {
            get { return _referenceValue; }
        }

        public int BinaryScaleFactor
        {
            get { return _binaryScaleFactor; }
        }

        public int DecimalScaleFactor
        {
            get { return _decimalScaleFactor; }
        }

        public int DataPoints
        {
            get { return _dataPoints; }
        }

        public int NumberOfBits
        {
            get { return _numberOfBits; }
        }

        public float PrimaryMissingValue
        {
            get { return _primaryMissingValue; }
        }

        public int MissingValueManagement
        {
            get { return _missingValueManagement; }
        }

        public int NumberOfGroups
        {
            get { return _numberOfGroups; }
        }

        public int BitsGroupWidths
        {
            get { return _bitsGroupWidths; }
        }

        public int ReferenceGroupLength
        {
            get { return _referenceGroupLength; }
        }

        public int LengthIncrement
        {
            get { return _lengthIncrement; }
        }

        public int BitsScaledGroupLength
        {
            get { return _bitsScaledGroupLength; }
        }

        public int LengthLastGroup
        {
            get { return _lengthLastGroup; }
        }

        public int OrderSpatial
        {
            get { return _orderSpatial; }
        }

        public int DescriptorSpatial
        {
            get { return _descriptorSpatial; }
        }

        public int OriginalType
        {
            get { return _originalType; }
        }

        public float SecondaryMissingValue
        {
            get { return _secondaryMissingValue; }
        }

        private void SetOtherAttributeByDataTemplateNo(FileStream fs)
        {
            _referenceValue = GribNumberHelper.IEEEfloat4(fs);
            _binaryScaleFactor = GribNumberHelper.Int2(fs);
            _decimalScaleFactor = GribNumberHelper.Int2(fs);
            _numberOfBits = fs.ReadByte();
            _originalType = fs.ReadByte();
            switch (_dataTemplate)
            {
                case 0:    //0 - Grid point data - simple packing
                case 1:    //1 - Matrix values - simple packing
                    break;
                case 2:
                case 3:
                    _splittingMethod = fs.ReadByte();
                    // octet 23
                    _missingValueManagement = fs.ReadByte();
                    // octet 24 - 27
                    _primaryMissingValue = GribNumberHelper.IEEEfloat4(fs);
                    // octet 28 - 31
                    _secondaryMissingValue = GribNumberHelper.IEEEfloat4(fs);
                    // octet 32 - 35
                    _numberOfGroups = GribNumberHelper.Int4(fs);
                    // octet 36
                    _referenceGroupWidths = fs.ReadByte();
                    // octet 37
                    _bitsGroupWidths = fs.ReadByte();
                    // according to documentation subtract referenceGroupWidths
                    _bitsGroupWidths = _bitsGroupWidths - _referenceGroupWidths;
                    // octet 38 - 41
                    _referenceGroupLength = GribNumberHelper.Int4(fs);
                    // octet 42
                    _lengthIncrement = fs.ReadByte();
                    // octet 43 - 46
                    _lengthLastGroup = GribNumberHelper.Int4(fs);
                    // octet 47
                    _bitsScaledGroupLength = fs.ReadByte();
                    if (_dataTemplate != 2)
                    {
                        // case 3 // complex packing & spatial differencing
                        _orderSpatial = fs.ReadByte();
                        _descriptorSpatial = fs.ReadByte();
                    }
                    break;
                default:
                    if (_dataTemplate == 40 || _dataTemplate == 40000)
                    {
                        _compressionMethod = fs.ReadByte();
                        _compressionRatio = fs.ReadByte();
                    }
                    break;
            }
        }

        private float GetFloat4(FileStream fs)
        {
            byte[] bytes = new byte[4];
            fs.Read(bytes, 0, 4);

            int num = (int)bytes[1] << 16 | (int)bytes[2] << 8 | (int)bytes[3];
            if (num == 0)
            {
                return 0f;
            }
            int num2 = -(((bytes[0] & 128) >> 6) - 1);
            int num3 = (int)((bytes[0] & 127) - 64);
            return (float)((double)num2 * Math.Pow(16.0, (double)(num3 - 6)) * (double)num);
        }
    }
}
