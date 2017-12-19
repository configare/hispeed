using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// GRIB2 网格定义段 Section3：段长、段号、网格面、面内数据的几何形状定义
    /// </summary>
    public class GRIB2GridDefinitionSection:IGRIB2GridDefinitionSection
    {
        /// <summary>  scale factor for Lat/Lon variables in degrees.</summary>
        private static float TenToNegSix = (float)((1 / 1000000.0));
        private static float TenToNegThree = (float)((1 / 1000.0));

        private GRIB2SectionHeader _sectionHeader;
        private int _source;
        private int _pointsNumber;
        private int _olon;
        private int _iolon;
        private int _gridTemplateNo;  //网格定义模板号
        private string _gridName;
        private int _shape;   //shape of the Earth;
        private int _nx;      //x方向点个数
        private int _ny;      //y方向点个数
        private int _angle;   //Basic angle of the initial production domain
        //Subdivisions of basic angle used to define extreme 
        //longitudes and latitudes, and direction increments
        private int _subDivisionsAngle;
        private double _latFirstPoint;
        private double _lonFirstPoint;
        private double _latEndPoint;
        private double _lonEndPoint;
        private int _resolutionFlag;
        private float _dx;
        private float _dy;
        private int _scanMode;
        private float _spLon;   //Longitude of the southern pole of projection
        private float _spLat;   //Latitude of the southern pole of projection
        private float _rotationAngle; //Angle of rotation of projection
        private float _poleLat;     //Latitude of the pole of stretching
        private float _poleLon;    //Longitude of the pole of stretching
        private float _factor;     //Stretching factor
        private float _latD;      //latitude(s) at which the Mercator projection intersects the Earth
        private float _lov;       //orientation of the grid
        private int _projectionCenter;  //Projection centre flag
        private float _latin1;    //first latitude from the pole at which the secant cone cuts the sphere
        private float _latin2;    //second latitude from the pole at which the secant cone cuts the sphere
        private int _paralellNumber;           //number of paralells between a pole and the equator
        private float _lap;       //Lap ― latitude of sub-satellite point 
        private float _lop;       //Lop ― longitude of sub-satellite point
        private float _xp;        //x-coordinate of sub-satellite point 
        private float _yp;        //y-coordinate of sub-satellite point
        private float _altitude;  //altitude of the camera from the Earth's centre
        private int _xo;          //x-coordinate of origin of sector image
        private int _yo;          //y-coordinate of origin of sector image
        private float _j;         //J ― pentagonal resolution parameter
        private float _k;         //K ― pentagonal resolution parameter
        private float _m;         //M ― pentagonal resolution parameter
        private int _method;      //Representation type indicating the method used to define the norm
        private int _mode;        //Representation mode indicating the order of the coefficients
        private int _n2, _n3;     //exponent of 2/3 for the number of intervals on main triangle sides
        private int _ni;          //ni ― number of intervals on main triangle sides of the icosahedron
        private int _nd;          //nd ― Number of diamonds
        private int _lonofcenter; //Longitude of the centre line of the first diamond of the icosahedron on the sphere
        private int _position;    //Grid point position
        private int _order;       //Numbering order of diamonds
        private int _nt;          //nt ― total number of great points
        private int _nb;          //number of data bins along radials 
        private int _nr;          //number of radials
        private float _dstart;     //Dstart ― offset from origin to inner bound


        public GRIB2GridDefinitionSection(FileStream fs)
        {
            long position = fs.Position;
            _sectionHeader = new GRIB2SectionHeader(fs);
            _source = fs.ReadByte();
            _pointsNumber = GribNumberHelper.Int4(fs);
            _olon = fs.ReadByte();
            _iolon = fs.ReadByte();
            _gridTemplateNo = GribNumberHelper.Int2(fs);
            _gridName = GetGridNameByGridNo();
            SetAttributeByGridTemplateNo(fs);
            fs.Seek(position + _sectionHeader.SectionLength, SeekOrigin.Begin);
        }

        public int GridTemplateNo
        {
            get { return _gridTemplateNo; }
        }

        public string GridName
        {
            get { return _gridName; }
        }

        public int PointsNumber
        {
            get { return _pointsNumber; }
        }

        public int Nx
        {
            get { return _nx; }
        }

        public int Ny
        {
            get { return _ny; }
        }

        public double LatFirstPoint
        {
            get { return _latFirstPoint; }
        }

        public double LonFirstPoint
        {
            get { return _lonFirstPoint; }
        }

        public double LatEndPoint
        {
            get { return _latEndPoint; }
        }

        public double LonEndPoint
        {
            get { return _lonEndPoint; }
        }

        public float Dx
        {
            get { return _dx; }
        }

        public float Dy
        {
            get { return _dy; }
        }

        public int ScanMode
        {
            get { return _scanMode; }
        }

        private void SetAttributeByGridTemplateNo(FileStream fs)
        {
            int scaleFactorRadius,scaleDvalueRadius,scaleFactorMajor,scaleDvalueMajor,
                scaleFactorMinor,scaleDvalueMinor;
            float ratio;
            if ((_gridTemplateNo >= 50 && _gridTemplateNo <= 53) || _gridTemplateNo == 100 || _gridTemplateNo == 120)
            {
                if (_gridTemplateNo >= 50 && _gridTemplateNo <= 53)
                {
                    _j = GribNumberHelper.IEEEfloat4(fs);
                    _k = GribNumberHelper.IEEEfloat4(fs);
                    _m = GribNumberHelper.IEEEfloat4(fs);
                    _method = fs.ReadByte();
                    _mode = fs.ReadByte();
                    if (_gridTemplateNo == 51)
                    {
                        _spLat = GribNumberHelper.Int4(fs) * TenToNegSix;
                        _spLon = GribNumberHelper.Int4(fs) * TenToNegSix;
                        _rotationAngle = GribNumberHelper.IEEEfloat4(fs);
                    }
                    else if (_gridTemplateNo == 52)
                    {
                        _poleLat = GribNumberHelper.Int4(fs) * TenToNegSix;
                        _poleLon = GribNumberHelper.Int4(fs) * TenToNegSix;
                        _factor = GribNumberHelper.Int4(fs);
                    }
                    else if (_gridTemplateNo == 53)
                    {
                        _spLat = GribNumberHelper.Int4(fs) * TenToNegSix;
                        _spLon = GribNumberHelper.Int4(fs) * TenToNegSix;
                        _rotationAngle = GribNumberHelper.IEEEfloat4(fs);
                        _poleLat = GribNumberHelper.Int4(fs) * TenToNegSix;
                        _poleLon = GribNumberHelper.Int4(fs) * TenToNegSix;
                        _factor = GribNumberHelper.Int4(fs);
                    }
                }
                else if (_gridTemplateNo == 100)
                {
                    _n2 = fs.ReadByte();
                    _n3 = fs.ReadByte();
                    _ni = GribNumberHelper.Int2(fs);
                    _nd = fs.ReadByte();
                    _poleLat = GribNumberHelper.Int4(fs) * TenToNegSix;
                    _poleLon = GribNumberHelper.Int4(fs) * TenToNegSix;
                    _lonofcenter = GribNumberHelper.Int4(fs);
                    _position = fs.ReadByte();
                    _order = fs.ReadByte();
                    _scanMode = fs.ReadByte();
                    _nt = GribNumberHelper.Int4(fs);
                }
                else
                {
                    _nb = GribNumberHelper.Int4(fs);
                    _nr = GribNumberHelper.Int4(fs);
                    _latFirstPoint = GribNumberHelper.Int4(fs);
                    _lonFirstPoint = GribNumberHelper.Int4(fs);
                    _dx = GribNumberHelper.Int4(fs);
                    _dstart = GribNumberHelper.IEEEfloat4(fs);
                    _scanMode = fs.ReadByte();
                }
            }
            else
            {
                _shape = fs.ReadByte();
                scaleFactorRadius = fs.ReadByte();
                scaleDvalueRadius = GribNumberHelper.Int4(fs);
                scaleFactorMajor = fs.ReadByte();
                scaleDvalueMajor = GribNumberHelper.Int4(fs);
                scaleFactorMinor = fs.ReadByte();
                scaleDvalueMinor = GribNumberHelper.Int4(fs);
                _nx = GribNumberHelper.Int4(fs);
                _ny = GribNumberHelper.Int4(fs);
                switch (_gridTemplateNo)
                {
                    // Latitude/Longitude Grid
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                        {
                            _angle = GribNumberHelper.Int4(fs);
                            _subDivisionsAngle = GribNumberHelper.Int4(fs);
                            if (_angle == 0)
                                ratio = TenToNegSix;
                            else
                                ratio = _angle / _subDivisionsAngle;
                            _latFirstPoint = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _lonFirstPoint = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _resolutionFlag = fs.ReadByte();
                            _latEndPoint = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _lonEndPoint = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _dx = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _dy = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _scanMode = fs.ReadByte();
                            //  1, 2, and 3 needs checked
                            if (_gridTemplateNo == 1)
                            {
                                //Rotated Latitude/longitude
                                _spLat = GribNumberHelper.Int4(fs) * TenToNegSix;
                                _spLon = GribNumberHelper.Int4(fs) * TenToNegSix;
                                _rotationAngle = GribNumberHelper.IEEEfloat4(fs);
                            }
                            else if (_gridTemplateNo == 2)
                            {
                                //Stretched Latitude/longitude
                                _poleLat = GribNumberHelper.Int4(fs) * TenToNegSix;
                                _poleLon = GribNumberHelper.Int4(fs) * TenToNegSix;
                                _factor = GribNumberHelper.Int4(fs);
                            }
                            else if (_gridTemplateNo == 3)
                            {
                                //Stretched and Rotated Latitude/longitude
                                _spLat = GribNumberHelper.Int4(fs) * TenToNegSix;
                                _spLon = GribNumberHelper.Int4(fs) * TenToNegSix;
                                _rotationAngle = GribNumberHelper.IEEEfloat4(fs);
                                _poleLat = GribNumberHelper.Int4(fs) * TenToNegSix;
                                _poleLon = GribNumberHelper.Int4(fs) * TenToNegSix;
                                _factor = GribNumberHelper.Int4(fs);
                            }
                            break;
                        }
                    case 10:  // Mercator
                        {
                            _latFirstPoint = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _lonFirstPoint = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _resolutionFlag = fs.ReadByte();
                            _latD = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _latEndPoint = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _lonEndPoint = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _scanMode = fs.ReadByte();
                            _angle = GribNumberHelper.Int4(fs);
                            _dx = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _dy = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            break;
                        }
                    case 20:  // Polar stereographic projection
                        {
                            _latFirstPoint = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _lonFirstPoint = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _resolutionFlag = fs.ReadByte();
                            _latD = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _lov = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _dx = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _dy = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _projectionCenter = fs.ReadByte();
                            _scanMode = fs.ReadByte();
                            break;
                        }
                    case 30:  // Lambert Conformal
                        {
                            _latFirstPoint = (float)(GribNumberHelper.Int4(fs) * TenToNegSix);
                            _lonFirstPoint = (float)(GribNumberHelper.Int4(fs) * TenToNegSix);
                            _resolutionFlag = fs.ReadByte();
                            _latD = (float)(GribNumberHelper.Int4(fs) * TenToNegSix);
                            _lov = (float)(GribNumberHelper.Int4(fs) * TenToNegSix);
                            _dx = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _dy = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _projectionCenter = fs.ReadByte();
                            _scanMode = fs.ReadByte();
                            _latin1 = (float)(GribNumberHelper.Int4(fs) * TenToNegSix);
                            _latin2 = (float)(GribNumberHelper.Int4(fs) * TenToNegSix);
                            _spLat = (float)(GribNumberHelper.Int4(fs) * TenToNegSix);
                            _spLon = (float)(GribNumberHelper.Int4(fs) * TenToNegSix);
                            break;
                        }
                    case 31:  // Albers Equal Area
                        {
                            _latFirstPoint = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _lonFirstPoint = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _resolutionFlag = fs.ReadByte();
                            _latD = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _lov = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _dx = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _dy = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _projectionCenter = fs.ReadByte();
                            _scanMode = fs.ReadByte();
                            _latin1 = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _latin2 = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _spLat = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _spLon = GribNumberHelper.Int4(fs) * TenToNegSix;
                            break;
                        }
                    case 40:
                    case 41:
                    case 42:
                    case 43:  // Gaussian latitude/longitude
                        {
                            _angle = GribNumberHelper.Int4(fs);
                            _subDivisionsAngle = GribNumberHelper.Int4(fs);
                            if (_angle == 0)
                                ratio = TenToNegSix;
                            else
                                ratio = _angle / _subDivisionsAngle;
                            _latFirstPoint = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _lonFirstPoint = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _resolutionFlag = fs.ReadByte();
                            _latEndPoint = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _lonEndPoint = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _dx = (float)(GribNumberHelper.Int4(fs) * ratio);
                            _paralellNumber = fs.ReadByte();
                            _scanMode = fs.ReadByte();
                            if (_gridTemplateNo == 41)
                            {
                                _spLat = GribNumberHelper.Int4(fs) * ratio;
                                _spLon = GribNumberHelper.Int4(fs) * ratio;
                                _rotationAngle = GribNumberHelper.IEEEfloat4(fs);
                            }
                            else if (_gridTemplateNo == 42)
                            {
                                _poleLat = GribNumberHelper.Int4(fs) * ratio;
                                _poleLon = GribNumberHelper.Int4(fs) * ratio;
                                _factor = GribNumberHelper.Int4(fs);
                            }
                            else if (_gridTemplateNo == 43)
                            {
                                _spLat = GribNumberHelper.Int4(fs) * ratio;
                                _spLon = GribNumberHelper.Int4(fs) * ratio;
                                _rotationAngle = GribNumberHelper.IEEEfloat4(fs);
                                _poleLat = GribNumberHelper.Int4(fs) * ratio;
                                _poleLon = GribNumberHelper.Int4(fs) * ratio;
                                _factor = GribNumberHelper.Int4(fs);
                            }
                            break;
                        }
                    case 90:  // Space view perspective or orthographic
                        {
                            _lap = GribNumberHelper.Int4(fs);
                            _lop = GribNumberHelper.Int4(fs);
                            _resolutionFlag = fs.ReadByte();
                            _dx = GribNumberHelper.Int4(fs);
                            _dy = GribNumberHelper.Int4(fs);
                            _xp = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _yp = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _scanMode = fs.ReadByte();
                            _angle = GribNumberHelper.Int4(fs);
                            _altitude = GribNumberHelper.Int4(fs) * 1000000;
                            _xo = GribNumberHelper.Int4(fs);
                            _yo = GribNumberHelper.Int4(fs);
                            break;
                        }
                    case 110:  // Equatorial azimuthal equidistant projection
                        {
                            _latFirstPoint = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _lonFirstPoint = GribNumberHelper.Int4(fs) * TenToNegSix;
                            _resolutionFlag = fs.ReadByte();
                            _dx = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _dy = (float)(GribNumberHelper.Int4(fs) * TenToNegThree);
                            _projectionCenter = fs.ReadByte();
                            _scanMode = fs.ReadByte();
                            break;
                        }
                }
            }

        }

        private string GetGridNameByGridNo()
        {
            switch (_gridTemplateNo)
            {
                case 0: return "Latitude/Longitude";
                case 1: return "Rotated Latitude/Longitude";
                case 2: return "Stretched Latitude/Longitude";
                case 3: return "Rotated and Stretched Latitude/Longitude";
                case 4: return "Variable Resolution Latitude/Longitude";
                case 5: return "Variable Resolution Rotated Latitude/Longitude";
                case 10: return "Mercator";
                case 12: return "Transverse Mercator";
                case 20: return "Polar Stereographic Projection";
                case 30: return "Lambert Conformal";
                case 31: return "Albers Equal Area";
                case 40: return "Gaussian Latitude/Longitude";
                case 41: return "Rotated Gaussian Latitude/Longitude";
                case 42: return "Stretched Gaussian Latitude/Longitude";
                case 43: return "Stretched and Rotated Gaussian Latitude/Longitude";
                case 50: return "Spherical Harmonic Coefficients";
                case 51: return "Rotated Spherical Harmonic Coefficients";
                case 52: return "Stretched Spherical Harmonic Coefficients";
                case 53: return "Stretched and Rotated Spherical Harmonic Coefficients";
                case 90: return "Space View Perspective or Orthographic";
                case 100: return "Triangular Grid Based on an Icosahedron";
                case 101: return "General Unstructured Grid";
                case 110: return "Equatorial Azimuthal Equidistant Projection";
                case 120: return "Azimuth-Range";
                case 140: return "Lambert Azimuthal Equal Area Projection";
                case 204: return "Curvilinear Orthogonal Grids";
                default: return "Unknown projection" + _gridTemplateNo;
            }
        }
    }
}
