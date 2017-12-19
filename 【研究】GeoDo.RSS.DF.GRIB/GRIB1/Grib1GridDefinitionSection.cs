using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace GeoDo.RSS.DF.GRIB
{
    /// <summary>
    /// Grib1网格定义段
    /// </summary>
    public class Grib1GridDefinitionSection
    {
        private int _sectionLength;//字节长度
        /// <summary>  P(V|L).
        /// PV - 垂直方向坐标参数序列
        /// PL - 每行点序号序列
        /// </summary>
        private int _vorL;
        private int _gridType;           //格网的类型
        private String _gridName;       //格网名称 
        private int _nx;                 //格网的列数 
        private int _ny;                 //格网的行数
        private double _latFirstPoint;   //起始格网点的纬度
        private double _lonFirstPoint;   //起始格网点的经度
        private double _latEndPoint;     //最后一个格网点的纬度
        private double _lonEndPoint;     //最后一个格网点的经度
        private double _lov;            //grid的方位
        private int _resolution;         //分辨率(See table 7)
        private double _dx;              //两格网点x坐标差
        private double _dy;              //两格网点y坐标差
        private int _np;                 //极点与赤道间平行线的数目
        protected internal int _scanMode;//扫描模式,(See table 8)
        private int _projetcenter;       //投影中心点标示
        /// <summary>
        /// Latin 1 - The first latitude from pole at which secant cone cuts the sperical earth.  
        /// </summary>
        private double _latin1;
        /// <summary> 
        /// Latin 2 - The second latitude from pole at which secant cone cuts the sperical earth.  
        /// </summary>
        private double _latin2;
        private double _latsp;    //南极点纬度
        private double _lonsp;    //南极点经度
        //是否为“瘦网格点” - 每行的格点数不同 
        private bool _thinnedGrid; 
        //每行的格点数
        private int[] _thinnedXNums; 
        //格点数的总和
        private int _thinnedGridNum;
        //该数据段的起始位置
        private long _position = 0;
        private bool _xReverse = false;
        private bool _yReverse = false;

        #region properties
        /// <summary>
        /// x方向上的点是否反转
        /// </summary>
        public bool XReverse
        {
            get { return _xReverse; }
        }

        /// <summary>
        /// y方向上的点是否反转
        /// </summary>
        public bool YReverse
        {
            get { return _yReverse; }
        }

        public bool ThinnedGrid
        {
            get { return _thinnedGrid; }
        }

        public int[] ThinnedXNums
        {
            get { return _thinnedXNums; }
        }

        public int ThinnedGridNum
        {
            get { return _thinnedGridNum; }
        }

        /// <summary>
        /// grid类型
        /// </summary>
        virtual public int GridType
        {
            get { return _gridType; }
        }

        /// <summary>
        /// 获取grid列数
        /// </summary>
        virtual public int Nx
        {
            get { return _nx; }
        }
        /// <summary>
        /// 获取grid行数
        /// </summary>
        virtual public int Ny
        {
            get { return _ny; }
        }
        /// <summary>
        /// 获取grid开始点y坐标/纬度的值
        /// </summary>
        virtual public double LatFirstPoint
        {
            get { return _latFirstPoint; }
        }
        /// <summary>
        /// 获取grid开始点x坐标/经度的值
        /// </summary>
        virtual public double LonFirstPoint
        {
            get { return _lonFirstPoint; }
        }
        /// <summary>
        /// 获取分辨率
        /// </summary>
        virtual public int Resolution
        {
            get { return _resolution; }
        }
        /// <summary>
        /// 圆或椭圆的格网
        /// </summary>
        virtual public int Shape
        {
            get
            {
                int res = _resolution >> 6;
                if (res == 1 || res == 3)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Grib的静态半径
        /// </summary>
        public static double ShapeRadius
        {
            get { return 6367.47; }
        }
        /// <summary>
        /// Grib的静态主轴
        /// </summary>
        public static double ShapeMajorAxis
        {
            get { return 6378.160; }
        }
        /// <summary>
        /// Grib的静态次轴
        /// </summary>
        public static double ShapeMinorAxis
        {
            get { return 6356.775; }
        }
        /// <summary>
        /// 获取grid终止点y坐标/纬度的值
        /// </summary>
        virtual public double LatEndPoint
        {
            get { return _latEndPoint; }
        }
        /// <summary>
        /// 获取grid终止点x坐标/经度的值
        /// </summary>
        virtual public double LonEndPoint
        {
            get { return _lonEndPoint; }
        }

        /// <summary>
        /// grid的方位
        /// </summary>
        virtual public double Lov
        {
            get { return _lov; }
        }
        /// <summary>
        /// 未定义 
        /// </summary>
        virtual public double Lad
        {
            get { return 0; }
        }
        /// <summary>
        /// 获取两个格网点间x增加量/距离的值
        /// </summary>
        /// <returns>x增加量</returns>
        virtual public double Dx
        {
            get { return _dx; }
        }
        /// <summary>
        /// 获取两个格网点间y增加量/距离的值
        /// </summary>
        /// <returns>y增加量</returns>
        virtual public double Dy
        {
            get { return _dy; }
        }
        /// <summary>
        /// 获取极点到赤道间的平行线
        /// </summary>
        virtual public double Np
        {
            get { return _np; }
        }
        /// <summary>
        /// 获取扫描模式
        /// </summary>
        virtual public int ScanMode
        {
            get { return _scanMode; }
        }
        /// <summary>
        /// 获取投影中心
        /// </summary>
        virtual public int ProjectionCenter
        {
            get { return _projetcenter; }
        }
        /// <summary> 
        /// Get first latitude from the pole at which cylinder cuts spherical earth 
        /// </summary>
        virtual public double Latin
        {
            get { return _latin1; }
        }
        /// <summary> 
        /// Get first latitude from the pole at which cone cuts spherical earth 
        /// </summary>
        virtual public double Latin1
        {
            get { return _latin1; }
        }
        /// <summary> 
        /// Get second latitude from the pole at which cone cuts spherical earth 
        /// </summary>
        virtual public double Latin2
        {
            get { return _latin2; }
        }
        /// <summary>
        /// 获取南极点纬度
        /// </summary>
        virtual public double SpLat
        {
            get { return _latsp; }
        }
        /// <summary>
        /// 获取南极点经度
        /// </summary>
        virtual public double SpLon
        {
            get { return _lonsp; }
        }
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public Grib1GridDefinitionSection()
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fs">文件读取参数对象</param>
        public Grib1GridDefinitionSection(FileStream fs)
        {
            _position = fs.Position;
            int reserved; // 用于读取空白
            _sectionLength = GribNumberHelper.Uint3(fs);// GDS的长度
            if (_sectionLength == 0)
            {
                // PDS与GDS间额外的一个字节
                fs.Seek(-2, SeekOrigin.Current);
                _sectionLength = (int)GribNumberHelper.Uint3(fs);
            }
            int NV = fs.ReadByte();
            // PL the location (octet number) of the list of numbers of points in each row
            _vorL = fs.ReadByte();
            _gridType = fs.ReadByte(); // grid类型
            _gridName = getGridName(_gridType);
            SetOtherAttribute(fs);
            //没有获取纬度方向上的分辨率
            //如果为高斯等经纬度的话，无法从数据中读取纬度方向上的分辨率
            if (_dy == 0)
                _dy = _ny / Math.Abs(_latEndPoint - _latFirstPoint);
            fs.Seek(_position + _sectionLength, SeekOrigin.Begin);
        }

        private void SetOtherAttribute(FileStream fs)
        {
            if (_gridType != 50)
            {
                // Nx - 沿x轴的点数量
                _nx = GribNumberHelper.Uint2(fs);// GribNumberHelper.Int2(fs);
                _nx = (_nx == -1) ? 1 : _nx;
                if (_nx >= 65535)
                {
                    _thinnedGrid = true;
                }
                // Ny - 沿y轴的点数量
                _ny = GribNumberHelper.Uint2(fs);// GribNumberHelper.Int2(fs);
                _ny = (_ny == -1) ? 1 : _ny;
                _latFirstPoint = GribNumberHelper.Int3(fs) / 1000.0;// La1 - 第一个格网点的纬度
                _lonFirstPoint = GribNumberHelper.Int3(fs) / 1000.0;// Lo1 - 第一个格网点的经度
                _resolution = fs.ReadByte();                        //分辨率和组件标识
            }
            switch (_gridType)
            {
                //  Latitude/Longitude  grids,Arakawa semi-staggered e-grid rotated
                //  Arakawa filled e-grid rotated

                case 0:                                           // Latitude/longitude grid
                case 4:                                           //Gaussian latitude/longitude grid
                case 40:
                case 201:
                case 202:
                    {
                        _latEndPoint = GribNumberHelper.Int3(fs) / 1000.0;   // La2 - 最后的格网点的纬度
                        _lonEndPoint = GribNumberHelper.Int3(fs) / 1000.0;   // Lo2 - 最后的格网点的经度
                        _dx = GribNumberHelper.Int2(fs) / 1000.0;           // Dx - 经度方向增加量
                        if (_gridType == 4)
                        {
                            _np = GribNumberHelper.Int2(fs);                // Np-极点与赤道间的纬度圈数                             
                        }
                        else
                        {
                            _dy = GribNumberHelper.Int2(fs) / 1000.0;      // Dy - 纬度方向增加量
                        }
                        _scanMode = fs.ReadByte();//扫描模式
                        //29-32 reserved
                        fs.Seek(4, SeekOrigin.Current);
                        if (_sectionLength > 32)
                        {
                            // Vertical coordinates (NV) and thinned grids (PL) not supported - skip this
                            fs.Seek(_sectionLength - 32, SeekOrigin.Current);
                        }
                        break;
                    }
                //Mercator projection
                case 1:
                    _latEndPoint = GribNumberHelper.Int3(fs) / 1000.0;// La2 - 最后格网点的纬度
                    _lonEndPoint = GribNumberHelper.Int3(fs) / 1000.0;// Lo2 - 最后格网点的经度
                    // octets 24-26 (Latin - latitude where cylinder intersects the earth
                    _latin1 = GribNumberHelper.Int3(fs) / 1000.0;//  Latin -圆柱体与地球相交处的高程
                    fs.ReadByte();
                    _scanMode = fs.ReadByte();//扫描模式
                    _dx = GribNumberHelper.Int3(fs); // Dx - 经度方向增加量
                    _dy = GribNumberHelper.Int3(fs);//  Dy - 纬度方向增加量
                    fs.Seek(4, SeekOrigin.Current);
                    if (_sectionLength > 42)
                    {
                        // Vertical coordinates (NV) and thinned grids (PL) not supported - skip this
                        fs.Seek(_sectionLength - 42, SeekOrigin.Current);
                    }
                    break;
                //Lambert conformal, secant or tangent, conic or bi-polar, projection
                case 3:
                    // Lov - Orientation of the grid - east lon parallel to y axis)
                    _lov = GribNumberHelper.Int3(fs) / 1000.0;
                    _dx = GribNumberHelper.Int3(fs);  // Dx - X方向格网长度
                    _dy = GribNumberHelper.Int3(fs);  // Dy - Y方向格网长度
                    _projetcenter = fs.ReadByte();   //投影中心标识
                    _scanMode = fs.ReadByte();      //扫描模式
                    // Latin1 - first lat where secant cone cuts spherical earth
                    _latin1 = GribNumberHelper.Int3(fs) / 1000.0;
                    // Latin2 - second lat where secant cone cuts spherical earth)
                    _latin2 = GribNumberHelper.Int3(fs) / 1000.0;
                    _latsp = GribNumberHelper.Int3(fs) / 1000.0;//南极点纬度
                    _lonsp = GribNumberHelper.Int3(fs) / 1000.0;//南极点经度
                    fs.Seek(2, SeekOrigin.Current);
                    if (_sectionLength > 42)
                    {
                        // Vertical coordinates (NV) and thinned grids (PL) not supported - skip this
                        fs.Seek(_sectionLength - 42, SeekOrigin.Current);
                    }

                    break;

                case 5:
                    // Lov - Orientation of the grid - east lon parallel to y axis)
                    _lov = GribNumberHelper.Int3(fs) / 1000.0;
                    _dx = GribNumberHelper.Int3(fs); // Dx - 经度方向增加量
                    _dy = GribNumberHelper.Int3(fs); // Dy - 纬度方向增加量
                    _projetcenter = fs.ReadByte();//投影中心标识
                    _scanMode = fs.ReadByte();//扫描模式
                    fs.Seek(4, SeekOrigin.Current);
                    if (_sectionLength > 32)
                    {
                        // Vertical coordinates (NV) and thinned grids (PL) not supported - skip this
                        fs.Seek(_sectionLength - 32, SeekOrigin.Current);
                    }
                    break;
                default:
                    break;
            }

            if ((_scanMode & 63) != 0)
                throw new Exception("GDS: This scanning mode (" + _scanMode + ") is not supported.");
            fs.Seek(_position, SeekOrigin.Begin);
            byte[] array = new byte[_sectionLength];
            fs.Read(array, 0, _sectionLength);
            AdjustAttributes(array);
        }

        private void AdjustAttributes(byte[] array)
        {
            if (this.Resolution == 128)
            {
                _dx = (double)GribNumberHelper.Uint2(array[23], array[24]) / 1000.0;
                if (this.GridType == 4)
                {
                    _np = GribNumberHelper.Uint2(array[25], array[26]);
                    _dy = (double)_np / 1000.0;
                }
                else
                {
                    _dy = (double)GribNumberHelper.Uint2(array[25], array[26]) / 1000.0;
                }
                _scanMode = Convert.ToInt32(array[27]);
            }
            else
            {
                _dx = (_lonEndPoint - _lonFirstPoint) / (double)(_nx - 1);
                _dy = (_latEndPoint - _latFirstPoint) / (double)(_ny - 1);
            }
            if ((_scanMode & 128) != 0)
            {
                _xReverse = true;
            }
            if (_latEndPoint < _latFirstPoint)
            {
                _yReverse = true;
            }
            if (_thinnedGrid)
            {
                _thinnedXNums = new int[_ny];
                _thinnedGridNum = 0;
                for (int i = 0; i < _ny; i++)
                {
                    _thinnedXNums[i] = GribNumberHelper.Int2(array[32 + i * 2], array[33 + i * 2]);
                    _thinnedGridNum += this.ThinnedXNums[i];
                    if (i == 0)
                    {
                        _nx = this.ThinnedXNums[i];
                    }
                    else
                    {
                        if (_nx < this.ThinnedXNums[i])
                        {
                            //取格点数最多那行的格点数为宽
                            _nx = this.ThinnedXNums[i];
                        }
                    }
                }
                _dx = Math.Abs(_lonEndPoint - _lonFirstPoint) / (double)(_nx - 1);
            }
        }


        /// <summary>
        /// 获取名称
        /// </summary>
        public String getGridName()
        {
            return _gridName;
        }

        /// <summary>
        /// 获取名称
        /// </summary>
        /// <param name="type">类型</param>
        static public String getGridName(int type)
        {
            switch (type)
            {
                case 0: return "Latitude/Longitude Grid";

                case 1: return "Mercator Projection Grid";

                case 2: return "Gnomonic Projection Grid";

                case 3: return "Lambert Conformal";

                case 4: return "Gaussian Latitude/Longitude";

                case 5: return "Polar Stereographic projection Grid";

                case 6: return "Universal Transverse Mercator";

                case 7: return "Simple polyconic projection";

                case 8: return "Albers equal-area, secant or tangent, conic or bi-polar, projection";

                case 9: return "Miller's cylindrical projection";

                case 10: return "Rotated latitude/longitude grid";

                case 13: return "Oblique Lambert conformal, secant or tangent, conical or bipolar, projection";

                case 14: return "Rotated Gaussian latitude/longitude grid";

                case 20: return "Stretched latitude/longitude grid";

                case 24: return "Stretched Gaussian latitude/longitude grid";

                case 30: return "Stretched and rotated latitude/longitude grids";

                case 34: return "Stretched and rotated Gaussian latitude/longitude grids";

                case 50: return "Spherical Harmonic Coefficients";

                case 60: return "Rotated spherical harmonic coefficients";

                case 70: return "Stretched spherical harmonics";

                case 80: return "Stretched and rotated spherical harmonic coefficients";

                case 90: return "Space view perspective or orthographic";

                case 201: return "Arakawa semi-staggered E-grid on rotated latitude/longitude grid-point array";

                case 202: return "Arakawa filled E-grid on rotated latitude/longitude grid-point array";
            }
            return "Unknown";
        }

        /// <summary>
        /// 获取格网形状
        /// </summary>
        public String getShapeName()
        {
            return getShapeName(Shape);
        }

        /// <summary>格网形状</summary>
        /// <param name="code">格网形状编码</param>
        /// <returns>格网形状名称</returns>
        static public String getShapeName(int code)
        {
            if (code == 1)
            {
                return "oblate spheroid";
            }
            else
            {
                return "spherical";
            }
        }
    }
}
