using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.HDF4.Cloudsat
{
    public class HDFConstants
    {
        //Limit Definitions
        public const int FIELDNAMELENMAX = 128;//Maximum length of a vdata field in bytes - 128 characters
        public const int H4_MAX_NC_ATTRS =3000 ;//Maximum number of file or variable attributes
        public const int H4_MAX_NC_DIMS= 5000 ;//Maximum number of dimensions per file
        public const int H4_MAX_NC_NAME= 256 ;//Maximum length of a name - NC interface
        public const int H4_MAX_NC_OPEN= MAX_FILE ;//Maximum number of files can be open at the same time
        public const int H4_MAX_NC_VARS =5000 ;//Maximum number of variables per file
        public const int H4_MAX_VAR_DIMS= 32 ;//Maximum number of dimensions per variable
        public const int MAXNVELT =64 ;//Maximum number of objects in a vgroup
        public const int MAX_FIELD_SIZE= 65535;// Maximum length of a field
        public const int MAX_FILE =32;// Maximum number of open files
        public const int MAX_ORDER= 65535 ;//Maximum order of a vdata field
        public const int MAX_PATH_LEN= 1024 ;//Maximum length of an external file name
        public const int MAX_GROUPS= 8;// Maximum number of groups
        public const int MAX_GR_NAME =256;// Maximum length of a name - GR interface
        public const int MAX_REF= 65535;// The largest number that will fit into a 16-bit word reference variable
        public const int MAX_BLOCK_SIZE= 65536;// Maximum size of blocks in linked blocks
        public const int VSNAMELENMAX =64;// Maximum length of a vdata name in bytes - 64 characters
        public const int VGNAMELENMAX =64 ;//Maximum length of a vgroup name in bytes - 64 characters
        public const int VSFIELDMAX= 256;// Maximum number of fields per vdata (64 for Macintosh)
        public const int VDEFAULTBLKSIZE =4096;// Default block size in a vdata
        public const int VDEFAULTNBLKS= 32;// Default number of blocks in a vdata

        public const int NLONE = 1000;

        public const int VGCLASSLENMAX = 256;
        /** FAIL */
        public const int FAIL = -1;
        public const int SUCCEED = 0;

        // file access code definitions
        public const int DFACC_READ = 1;
        public const int DFACC_WRITE = 2;
        public const int DFACC_RDWR = 3;
        public const int DFACC_CREATE = 4;
        public const int DFACC_RDONLY = DFACC_READ;
        public const int DFACC_DEFAULT = 000;
        public const int DFACC_SERIAL = 001;
        public const int DFACC_PARALLEL = 011;

        // annotation type in HDF
        public const int AN_DATA_LABEL = 0;
        public const int AN_DATA_DESC = AN_DATA_LABEL + 1;
        public const int AN_FILE_LABEL = AN_DATA_LABEL + 2;
        public const int AN_FILE_DESC = AN_DATA_LABEL + 3;

        // HDF Tag Definations

        public const int DFREF_WILDCARD = 0;
        public const int DFTAG_WILDCARD = 0;

        public const int DFREF_NONE = 0;  // used by mfhdf/libsrc/putget.c

        // tags and refs
        public const int DFTAG_NULL = 1;
        public const int DFTAG_LINKED = 20;  // linked-block special element
        public const int DFTAG_VERSION = 30;
        public const int DFTAG_COMPRESSED = 40;  // compressed special element
        public const int DFTAG_VLINKED = 50;  // variable-len linked-block header
        public const int DFTAG_VLINKED_DATA = 51;  // variable-len linked-block data
        public const int DFTAG_CHUNKED = 60;  // chunked special element header
        public const int DFTAG_CHUNK = 61;  // chunk element

        // utility set
        public const int DFTAG_FID = 100;   // File identifier
        public const int DFTAG_FD = 101;   // File description
        public const int DFTAG_TID = 102;   // Tag identifier
        public const int DFTAG_TD = 103;   // Tag descriptor
        public const int DFTAG_DIL = 104;   // data identifier label
        public const int DFTAG_DIA = 105;   // data identifier annotation
        public const int DFTAG_NT = 106;   // number type
        public const int DFTAG_MT = 107;   // machine type
        public const int DFTAG_FREE = 108;   // free space in the file

        // raster-8 set
        public const int DFTAG_ID8 = 200;   // 8-bit Image dimension
        public const int DFTAG_IP8 = 201;   // 8-bit Image palette
        public const int DFTAG_RI8 = 202;   // Raster-8 image
        public const int DFTAG_CI8 = 203;   // RLE compressed 8-bit image
        public const int DFTAG_II8 = 204;   // IMCOMP compressed 8-bit image

        // Raster Image set
        public const int DFTAG_ID = 300;   // Image DimRec
        public const int DFTAG_LUT = 301;   // Image Palette
        public const int DFTAG_RI = 302;   // Raster Image
        public const int DFTAG_CI = 303;   // Compressed Image
        public const int DFTAG_NRI = 304;   // New-format Raster Image

        public const int DFTAG_RIG = 306;   // Raster Image Group
        public const int DFTAG_LD = 307;   // Palette DimRec
        public const int DFTAG_MD = 308;   // Matte DimRec
        public const int DFTAG_MA = 309;   // Matte Data
        public const int DFTAG_CCN = 310;   // color correction
        public const int DFTAG_CFM = 311;   // color format
        public const int DFTAG_AR = 312;   // aspect ratio

        public const int DFTAG_DRAW = 400;   // Draw these images in sequence
        public const int DFTAG_RUN = 401;   // run this as a program/script

        public const int DFTAG_XYP = 500;   // x-y position
        public const int DFTAG_MTO = 501;   // machine-type override

        // Tektronix
        public const int DFTAG_T14 = 602;   // TEK 4014 data
        public const int DFTAG_T105 = 603;   // TEK 4105 data

        // Scientific Data set
        // Objects of tag 721 are never actually written to the file.  The tag is
        // needed to make things easier mixing DFSD and SD style objects in the
        // same file

        public const int DFTAG_SDG = 700;   // Scientific Data Group
        public const int DFTAG_SDD = 701;   // Scientific Data DimRec
        public const int DFTAG_SD = 702;   // Scientific Data
        public const int DFTAG_SDS = 703;   // Scales
        public const int DFTAG_SDL = 704;   // Labels
        public const int DFTAG_SDU = 705;   // Units
        public const int DFTAG_SDF = 706;   // Formats
        public const int DFTAG_SDM = 707;   // Max/Min
        public const int DFTAG_SDC = 708;   // Coord sys
        public const int DFTAG_SDT = 709;   // Transpose
        public const int DFTAG_SDLNK = 710;   // Links related to the dataset
        public const int DFTAG_NDG = 720;   // Numeric Data Group
        public const int DFTAG_CAL = 731;   // Calibration information
        public const int DFTAG_FV = 732;   // Fill Value information
        public const int DFTAG_BREQ = 799;   // Beginning of required tags
        public const int DFTAG_SDRAG = 781;   // List of ragged array line lengths
        public const int DFTAG_EREQ = 780;   // Current end of the range

        // VSets
        public const int DFTAG_VG = 1965;   // Vgroup
        public const int DFTAG_VH = 1962;   // Vdata Header
        public const int DFTAG_VS = 1963;   // Vdata Storage

        // compression schemes
        public const int DFTAG_RLE = 11;   // run length encoding
        public const int DFTAG_IMC = 12;   // IMCOMP compression alias
        public const int DFTAG_IMCOMP = 12;   // IMCOMP compression
        public const int DFTAG_JPEG = 13;   // JPEG compression (24-bit data)
        public const int DFTAG_GREYJPEG = 14;   // JPEG compression (8-bit data)
        public const int DFTAG_JPEG5 = 15;   // JPEG compression (24-bit data)
        public const int DFTAG_GREYJPEG5 = 16;   // JPEG compression (8-bit data)

        /** pixel interlacing scheme */
        public const int MFGR_INTERLACE_PIXEL = 0;

        /** line interlacing scheme */
        public const int MFGR_INTERLACE_LINE = MFGR_INTERLACE_PIXEL + 1;

        /** component interlacing scheme */
        public const int MFGR_INTERLACE_COMPONENT = MFGR_INTERLACE_PIXEL + 2;

        /** interlacing supported by the vset.*/
        public const int FULL_INTERLACE = 0;
        public const int NO_INTERLACE = 1;

        /** unsigned char */
        public const int DFNT_UCHAR8 = 3;
        public const int DFNT_UCHAR = 3;

        /** char */
        public const int DFNT_CHAR8 = 4;
        public const int DFNT_CHAR = 4;

        /** No supported by HDF */
        public const int DFNT_CHAR16 = 42;
        public const int DFNT_UCHAR16 = 43;


        /** float */
        public const int DFNT_FLOAT32 = 5;
        public const int DFNT_FLOAT = 5;

        //** double */
        public const int DFNT_FLOAT64 = 6;
        public const int DFNT_FLOAT128 = 7;
        public const int DFNT_DOUBLE = 6;

        /** 8-bit integer */
        public const int DFNT_INT8 = 20;

        /** unsigned 8-bit interger */
        public const int DFNT_UINT8 = 21;

        /** short */
        public const int DFNT_INT16 = 22;

        /** unsigned interger */
        public const int DFNT_UINT16 = 23;

        /** interger */
        public const int DFNT_INT32 = 24;

        /** unsigned interger */
        public const int DFNT_UINT32 = 25;

        /** No supported */
        public const int DFNT_INT64 = 26;
        public const int DFNT_UINT64 = 27;
        public const int DFNT_INT128 = 28;
        public const int DFNT_UINT128 = 30;
        public const int DFNT_LITEND = 0x00004000;

        public const int DF_FORWARD = 1;
        public const int DFS_MAXLEN = 255;

        public const int COMP_NONE = 0;
        public const int COMP_JPEG = 2;
        public const int COMP_RLE = 11;
        public const int COMP_IMCOMP = 12;
        public const int COMP_CODE_NONE = 0;
        public const int COMP_CODE_RLE = 1;
        public const int COMP_CODE_NBIT = 2;
        public const int COMP_CODE_SKPHUFF = 3;
        public const int COMP_CODE_DEFLATE = 4;
        public const int COMP_CODE_SZIP = 5;
        public const int COMP_CODE_INVALID = 6;
        public const int COMP_CODE_JPEG = 7;

        // Interlace schemes
        public const int DFIL_PIXEL = 0;  /* Pixel Interlacing */
        public const int DFIL_LINE = 1;  /* Scan Line Interlacing */
        public const int DFIL_PLANE = 2;  /* Scan Plane Interlacing */

        public const int SD_UNLIMITED = 0;
        public const int SD_FILL = 0;
        public const int SD_NOFILL = 0x100;
        public const int SD_DIMVAL_BW_COMP = 1;
        public const int SD_DIMVAL_BW_INCOMP = 0;

        public const int HDF_NONE = 0x0;
        public const int HDF_CHUNK = 0x1;
        public const int HDF_COMP = 0x3;
        public const int HDF_NBIT = 0x5;
        public const int MAX_VAR_DIMS = 32;

        //the names of the Vgroups created by the GR interface
        public const String GR_NAME = "RIG0.0";
        public const String RI_NAME = "RI0.0";
        public const String RIGATTRNAME = "RIATTR0.0N";
        public const String RIGATTRCLASS = "RIATTR0.0C";

        // names of classes of the Vdatas/Vgroups created by the SD interface
        public const String HDF_ATTRIBUTE = "Attr0.0";
        public const String HDF_VARIABLE = "Var0.0";
        public const String HDF_DIMENSION = "Dim0.0";
        public const String HDF_UDIMENSION = "UDim0.0";
        public const String DIM_VALS = "DimVal0.0";
        public const String DIM_VALS01 = "DimVal0.1";
        public const String HDF_CHK_TBL = "_HDF_CHK_TBL_";
        public const String HDF_SDSVAR = "SDSVar";
        public const String HDF_CRDVAR = "CoordVar";

        public const String HDF_CDF = "CDF0.0";

        // names of data object types
        public const String ANNOTATION = "HDF_ANNOTATION";
        public const String RI8 = "HDF_RI8";
        public const String RI24 = "HDF_RI24";
        public const String GR = "HDF_GR";
        public const String SDS = "HDF_SDS";
        public const String VDATA = "HDF_VDATA";
        public const String VGROUP = "HDF_GROUP";

        // data types represented by Strings
        public const String UCHAR8 = "UCHAR8";
        public const String CHAR8 = "CHAR8";
        public const String UCHAR16 = "UCHAR16";
        public const String CHAR16 = "CHAR16";
        public const String FLOAT32 = "FLOAT32";
        public const String FLOAT64 = "FLOAT64";
        public const String FLOAT128 = "FLOAT128";
        public const String INT8 = "INT8";
        public const String UINT8 = "UINT8";
        public const String INT16 = "INT16";
        public const String UINT16 = "UINT16";
        public const String INT32 = "INT32";
        public const String UINT32 = "UINT32";
        public const String INT64 = "INT64";
        public const String UINT64 = "UINT64";
        public const String INT128 = "INT128";
        public const String UINT128 = "UINT128";

        /**
         *  convert number type to string type
         *  params type  the number representing the data type
         *  return the string representing the data type
         */
        public static String getType(int type)
        {
            if (type == HDFConstants.DFNT_UCHAR8)
            {
                return HDFConstants.UCHAR8;
            }
            else if (type == HDFConstants.DFNT_CHAR8)
            {
                return HDFConstants.CHAR8;
            }
            else if (type == HDFConstants.DFNT_UCHAR16)
            {
                return HDFConstants.UCHAR16;
            }
            else if (type == HDFConstants.DFNT_CHAR16)
            {
                return HDFConstants.CHAR16;
            }
            else if (type == HDFConstants.DFNT_FLOAT32)
            {
                return HDFConstants.FLOAT32;
            }
            else if (type == HDFConstants.DFNT_FLOAT64)
            {
                return HDFConstants.FLOAT64;
            }
            else if (type == HDFConstants.DFNT_FLOAT128)
            {
                return HDFConstants.FLOAT128;
            }
            else if (type == HDFConstants.DFNT_INT8)
            {
                return HDFConstants.INT8;
            }
            else if (type == HDFConstants.DFNT_UINT8)
            {
                return HDFConstants.UINT8;
            }
            else if (type == HDFConstants.DFNT_INT16)
            {
                return HDFConstants.INT16;
            }
            else if (type == HDFConstants.DFNT_UINT16)
            {
                return HDFConstants.UINT16;
            }
            else if (type == HDFConstants.DFNT_INT32)
            {
                return HDFConstants.INT32;
            }
            else if (type == HDFConstants.DFNT_UINT32)
            {
                return HDFConstants.UINT32;
            }
            else if (type == HDFConstants.DFNT_INT64)
            {
                return HDFConstants.INT64;
            }
            else if (type == HDFConstants.DFNT_UINT64)
            {
                return HDFConstants.UINT64;
            }
            else if (type == HDFConstants.DFNT_INT128)
            {
                return HDFConstants.INT128;
            }
            else if (type == HDFConstants.DFNT_UINT128)
            {
                return HDFConstants.UINT128;
            }
            else
            {
                return "Undefined Data Type";
            }
        }

        /**
         *  convert string type to number type
         *  params type  the string representing the data type
         *  return the integer representing the data type
         */
        public static int getType(String type)
        {
            if (type.Equals(HDFConstants.UCHAR8, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_UCHAR8;
            }
            else if (type.Equals(HDFConstants.CHAR8, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_CHAR8;
            }
            else if (type.Equals(HDFConstants.UCHAR16, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_UCHAR16;
            }
            else if (type.Equals(HDFConstants.CHAR16, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_CHAR16;
            }
            else if (type.Equals(HDFConstants.FLOAT32, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_FLOAT32;
            }
            else if (type.Equals(HDFConstants.FLOAT64, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_FLOAT64;
            }
            else if (type.Equals(HDFConstants.FLOAT128, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_FLOAT128;
            }
            else if (type.Equals(HDFConstants.INT8, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_INT8;
            }
            else if (type.Equals(HDFConstants.UINT8, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_UINT8;
            }
            else if (type.Equals(HDFConstants.INT16, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_INT16;
            }
            else if (type.Equals(HDFConstants.UINT16, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_UINT16;
            }
            else if (type.Equals(HDFConstants.INT32, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_INT32;
            }
            else if (type.Equals(HDFConstants.UINT32, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_UINT32;
            }
            else if (type.Equals(HDFConstants.INT64, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_INT64;
            }
            else if (type.Equals(HDFConstants.UINT64, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_UINT64;
            }
            else if (type.Equals(HDFConstants.INT128, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_INT128;
            }
            else if (type.Equals(HDFConstants.UINT128, StringComparison.OrdinalIgnoreCase))
            {
                return HDFConstants.DFNT_UINT128;
            }
            else
            {
                return -1;
            }
        }

        /**
         *  gets the size of the data type in bytes,
         *  e.g size of DFNT_FLOAT32 = 4
         *
         *  the size of the data type
         */
        public static int getTypeSize(int type)
        {
            int size = 0;

            switch (type)
            {
                case HDFConstants.DFNT_UCHAR16:
                case HDFConstants.DFNT_CHAR16:
                case HDFConstants.DFNT_INT16:
                case HDFConstants.DFNT_UINT16:
                    size = 2;
                    break;
                case HDFConstants.DFNT_FLOAT32:
                case HDFConstants.DFNT_INT32:
                case HDFConstants.DFNT_UINT32:
                    size = 4;
                    break;
                case HDFConstants.DFNT_FLOAT64:
                case HDFConstants.DFNT_INT64:
                case HDFConstants.DFNT_UINT64:
                    size = 8;
                    break;
                case HDFConstants.DFNT_FLOAT128:
                case HDFConstants.DFNT_INT128:
                case HDFConstants.DFNT_UINT128:
                    size = 16;
                    break;
                default:
                    size = 1;
                    break;
            }

            return size;
        }

    }
}
