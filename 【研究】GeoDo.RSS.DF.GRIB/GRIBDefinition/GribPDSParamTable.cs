using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.DF.GRIB
{
    // 用于陈述参数的名称及描述信息，来自GRIB记录的字节代码的层和单位
    public class GribPDSParamTable
    {
        /*
         * There is 256 entries in a parameter table due to the nature of a byte
         */
        private const int m_NPARAMETERS = 256;

        protected int _centerId;//定义中心

        //  Identification of center defined sub-center - not fully implemented yet
        protected int _subcenterId;

        protected int _tableNumber;//定义参数集版本编号

        protected String _filename = null;//保存文件名

        protected List<Parameter> _parameters = null;// 参数集

        protected static List<GribPDSParamTable> _tables = null;//参数集序列

        /*
         *  static Array with parameter tables used by the GRIB file
         * (should only be one, but not actually limited to that - this allows
         *  GRIB files to be read that have more than one center's information in it)
         */
        private static List<GribPDSParamTable> _paramTables = null;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="subcenterId"></param>
        /// <param name="centerId">中心编号</param>
        /// <param name="tableNumber">参数集编号</param>
        /// <param name="parameTers">参数集</param>
        private GribPDSParamTable(string fileName, int subcenterId, int centerId, int tableNumber, List<Parameter> parameTers)
        {
            _filename = fileName;
            _centerId = centerId;
            _subcenterId = subcenterId;
            _tableNumber = tableNumber;
            _parameters = parameTers;
        }
       
        private static void InitDefaultTableEntries(List<GribPDSParamTable> aTables)
        {
            string[,] defaulttable_ncep_reanal2 = new string[256, 3]
            {
                           /*   0 */   {"var0", "undefined", "undefined"},
                           /*   1 */   {"pres", "Pressure", "Pa"},
                           /*   2 */   {"prmsl", "Pressure reduced to MSL", "Pa"},
                           /*   3 */   {"ptend", "Pressure tendency", "Pa/s"},
                           /*   4 */   {"var4", "undefined", "undefined"},
                           /*   5 */   {"var5", "undefined", "undefined"},
                           /*   6 */   {"gp", "Geopotential", "m^2/s^2"},
                           /*   7 */   {"hgt", "Geopotential height", "gpm"},
                           /*   8 */   {"dist", "Geometric height", "m"},
                           /*   9 */   {"hstdv", "Std dev of height", "m"},
                           /*  10 */   {"hvar", "Varianance of height", "m^2"},
                           /*  11 */   {"tmp", "Temperature", "K"},
                           /*  12 */   {"vtmp", "Virtual temperature", "K"},
                           /*  13 */   {"pot", "Potential temperature", "K"},
                           /*  14 */   {"epot", "Pseudo-adiabatic pot. temperature", "K"},
                           /*  15 */   {"tmax", "Max. temperature", "K"},
                           /*  16 */   {"tmin", "Min. temperature", "K"},
                           /*  17 */   {"dpt", "Dew point temperature", "K"},
                           /*  18 */   {"depr", "Dew point depression", "K"},
                           /*  19 */   {"lapr", "Lapse rate", "K/m"},
                           /*  20 */   {"visib", "Visibility", "m"},
                           /*  21 */   {"rdsp1", "Radar spectra (1)", ""},
                           /*  22 */   {"rdsp2", "Radar spectra (2)", ""},
                           /*  23 */   {"rdsp3", "Radar spectra (3)", ""},
                           /*  24 */   {"var24", "undefined", "undefined"},
                           /*  25 */   {"tmpa", "Temperature anomaly", "K"},
                           /*  26 */   {"presa", "Pressure anomaly", "Pa"},
                           /*  27 */   {"gpa", "Geopotential height anomaly", "gpm"},
                           /*  28 */   {"wvsp1", "Wave spectra (1)", ""},
                           /*  29 */   {"wvsp2", "Wave spectra (2)", ""},
                           /*  30 */   {"wvsp3", "Wave spectra (3)", ""},
                           /*  31 */   {"wdir", "Wind direction", "deg"},
                           /*  32 */   {"wind", "Wind speed", "m/s"},
                           /*  33 */   {"ugrd", "u wind", "m/s"},
                           /*  34 */   {"vgrd", "v wind", "m/s"},
                           /*  35 */   {"strm", "Stream function", "m^2/s"},
                           /*  36 */   {"vpot", "Velocity potential", "m^2/s"},
                           /*  37 */   {"mntsf", "Montgomery stream function", "m^2/s^2"},
                           /*  38 */   {"sgcvv", "Sigma coord. vertical velocity", "/s"},
                           /*  39 */   {"vvel", "Pressure vertical velocity", "Pa/s"},
                           /*  40 */   {"dzdt", "Geometric vertical velocity", "m/s"},
                           /*  41 */   {"absv", "Absolute vorticity", "/s"},
                           /*  42 */   {"absd", "Absolute divergence", "/s"},
                           /*  43 */   {"relv", "Relative vorticity", "/s"},
                           /*  44 */   {"reld", "Relative divergence", "/s"},
                           /*  45 */   {"vucsh", "Vertical u shear", "/s"},
                           /*  46 */   {"vvcsh", "Vertical v shear", "/s"},
                           /*  47 */   {"dirc", "Direction of current", "deg"},
                           /*  48 */   {"spc", "Speed of current", "m/s"},
                           /*  49 */   {"uogrd", "u of current", "m/s"},
                           /*  50 */   {"vogrd", "v of current", "m/s"},
                           /*  51 */   {"spfh", "Specific humidity", "kg/kg"},
                           /*  52 */   {"rh", "Relative humidity", "%"},
                           /*  53 */   {"mixr", "Humidity mixing ratio", "kg/kg"},
                           /*  54 */   {"pwat", "Precipitable water", "kg/m^2"},
                           /*  55 */   {"vapp", "Vapor pressure", "Pa"},
                           /*  56 */   {"satd", "Saturation deficit", "Pa"},
                           /*  57 */   {"evp", "Evaporation", "kg/m^2"},
                           /*  58 */   {"cice", "Cloud Ice", "kg/m^2"},
                           /*  59 */   {"prate", "Precipitation rate", "kg/m^2/s"},
                           /*  60 */   {"tstm", "Thunderstorm probability", "%"},
                           /*  61 */   {"apcp", "Total precipitation", "kg/m^2"},
                           /*  62 */   {"ncpcp", "Large scale precipitation", "kg/m^2"},
                           /*  63 */   {"acpcp", "Convective precipitation", "kg/m^2"},
                           /*  64 */   {"srweq", "Snowfall rate water equiv.", "kg/m^2/s"},
                           /*  65 */   {"weasd", "Accum. snow", "kg/m^2"},
                           /*  66 */   {"snod", "Snow depth", "m"},
                           /*  67 */   {"mixht", "Mixed layer depth", "m"},
                           /*  68 */   {"tthdp", "Transient thermocline depth", "m"},
                           /*  69 */   {"mthd", "Main thermocline depth", "m"},
                           /*  70 */   {"mtha", "Main thermocline anomaly", "m"},
                           /*  71 */   {"tcdc", "Total cloud cover", "%"},
                           /*  72 */   {"cdcon", "Convective cloud cover", "%"},
                           /*  73 */   {"lcdc", "Low level cloud cover", "%"},
                           /*  74 */   {"mcdc", "Mid level cloud cover", "%"},
                           /*  75 */   {"hcdc", "High level cloud cover", "%"},
                           /*  76 */   {"cwat", "Cloud water", "kg/m^2"},
                           /*  77 */   {"var77", "undefined", "undefined"},
                           /*  78 */   {"snoc", "Convective snow", "kg/m^2"},
                           /*  79 */   {"snol", "Large scale snow", "kg/m^2"},
                           /*  80 */   {"wtmp", "Water temperature", "K"},
                           /*  81 */   {"land", "Land cover (land=1;sea=0)", "fraction"},
                           /*  82 */   {"dslm", "Deviation of sea level from mean", "m"},
                           /*  83 */   {"sfcr", "Surface roughness", "m"},
                           /*  84 */   {"albdo", "Albedo", "%"},
                           /*  85 */   {"tsoil", "Soil temperature", "K"},
                           /*  86 */   {"soilm", "Soil moisture content", "kg/m^2"},
                           /*  87 */   {"veg", "Vegetation", "%"},
                           /*  88 */   {"salty", "Salinity", "kg/kg"},
                           /*  89 */   {"den", "Density", "kg/m^3"},
                           /*  90 */   {"runof", "Runoff", "kg/m^2"},
                           /*  91 */   {"icec", "Ice concentration (ice=1;no ice=0)", "fraction"},
                           /*  92 */   {"icetk", "Ice thickness", "m"},
                           /*  93 */   {"diced", "Direction of ice drift", "deg"},
                           /*  94 */   {"siced", "Speed of ice drift", "m/s"},
                           /*  95 */   {"uice", "u of ice drift", "m/s"},
                           /*  96 */   {"vice", "v of ice drift", "m/s"},
                           /*  97 */   {"iceg", "Ice growth rate", "m/s"},
                           /*  98 */   {"iced", "Ice divergence", "/s"},
                           /*  99 */   {"snom", "Snow melt", "kg/m^2"},
                           /* 100 */   {"htsgw", "Sig height of wind waves and swell", "m"},
                           /* 101 */   {"wvdir", "Direction of wind waves", "deg"},
                           /* 102 */   {"wvhgt", "Sig height of wind waves", "m"},
                           /* 103 */   {"wvper", "Mean period of wind waves", "s"},
                           /* 104 */   {"swdir", "Direction of swell waves", "deg"},
                           /* 105 */   {"swell", "Sig height of swell waves", "m"},
                           /* 106 */   {"swper", "Mean period of swell waves", "s"},
                           /* 107 */   {"dirpw", "Primary wave direction", "deg"},
                           /* 108 */   {"perpw", "Primary wave mean period", "s"},
                           /* 109 */   {"dirsw", "Secondary wave direction", "deg"},
                           /* 110 */   {"persw", "Secondary wave mean period", "s"},
                           /* 111 */   {"nswrs", "Net short wave (surface)", "W/m^2"},
                           /* 112 */   {"nlwrs", "Net long wave (surface)", "W/m^2"},
                           /* 113 */   {"nswrt", "Net short wave (top)", "W/m^2"},
                           /* 114 */   {"nlwrt", "Net long wave (top)", "W/m^2"},
                           /* 115 */   {"lwavr", "Long wave", "W/m^2"},
                           /* 116 */   {"swavr", "Short wave", "W/m^2"},
                           /* 117 */   {"grad", "Global radiation", "W/m^2"},
                           /* 118 */   {"var118", "undefined", "undefined"},
                           /* 119 */   {"var119", "undefined", "undefined"},
                           /* 120 */   {"var120", "undefined", "undefined"},
                           /* 121 */   {"lhtfl", "Latent heat flux", "W/m^2"},
                           /* 122 */   {"shtfl", "Sensible heat flux", "W/m^2"},
                           /* 123 */   {"blydp", "Boundary layer dissipation", "W/m^2"},
                           /* 124 */   {"uflx", "Zonal momentum flux", "N/m^2"},
                           /* 125 */   {"vflx", "Meridional momentum flux", "N/m^2"},
                           /* 126 */   {"wmixe", "Wind mixing energy", "J"},
                           /* 127 */   {"imgd", "Image data", ""},
                           /* 128 */   {"mslsa", "Mean sea level pressure (Std Atm)", "Pa"},
                           /* 129 */   {"mslma", "Mean sea level pressure (MAPS)", "Pa"},
                           /* 130 */   {"mslet", "Mean sea level pressure (ETA model)", "Pa"},
                           /* 131 */   {"lftx", "Surface lifted index", "K"},
                           /* 132 */   {"4lftx", "Best (4-layer) lifted index", "K"},
                           /* 133 */   {"kx", "K index", "K"},
                           /* 134 */   {"sx", "Sweat index", "K"},
                           /* 135 */   {"mconv", "Horizontal moisture divergence", "kg/kg/s"},
                           /* 136 */   {"vssh", "Vertical speed shear", "1/s"},
                           /* 137 */   {"tslsa", "3-hr pressure tendency (Std Atmos Red)", "Pa/s"},
                           /* 138 */   {"bvf2", "Brunt-Vaisala frequency^2", "1/s^2"},
                           /* 139 */   {"pvmw", "Potential vorticity (mass-weighted)", "1/s/m"},
                           /* 140 */   {"crain", "Categorical rain", "yes=1;no=0"},
                           /* 141 */   {"cfrzr", "Categorical freezing rain", "yes=1;no=0"},
                           /* 142 */   {"cicep", "Categorical ice pellets", "yes=1;no=0"},
                           /* 143 */   {"csnow", "Categorical snow", "yes=1;no=0"},
                           /* 144 */   {"soilw", "Volumetric soil moisture", "fraction"},
                           /* 145 */   {"pevpr", "Potential evaporation rate", "W/m^2"},
                           /* 146 */   {"cwork", "Cloud work function", "J/kg"},
                           /* 147 */   {"u-gwd", "Zonal gravity wave stress", "N/m^2"},
                           /* 148 */   {"v-gwd", "Meridional gravity wave stress", "N/m^2"},
                           /* 149 */   {"pvort", "Potential vorticity", "m^2/s/kg"},
                           /* 150 */   {"var150", "undefined", "undefined"},
                           /* 151 */   {"var151", "undefined", "undefined"},
                           /* 152 */   {"var152", "undefined", "undefined"},
                           /* 153 */   {"mfxdv", "Moisture flux divergence", "gr/gr*m/s/m"},
                           /* 154 */   {"vqr154", "undefined", "undefined"},
                           /* 155 */   {"gflux", "Ground heat flux", "W/m^2"},
                           /* 156 */   {"cin", "Convective inhibition", "J/kg"},
                           /* 157 */   {"cape", "Convective Avail. Pot. Energy", "J/kg"},
                           /* 158 */   {"tke", "Turbulent kinetic energy", "J/kg"},
                           /* 159 */   {"condp", "Lifted parcel condensation pressure", "Pa"},
                           /* 160 */   {"csusf", "Clear sky upward solar flux", "W/m^2"},
                           /* 161 */   {"csdsf", "Clear sky downward solar flux", "W/m^2"},
                           /* 162 */   {"csulf", "Clear sky upward long wave flux", "W/m^2"},
                           /* 163 */   {"csdlf", "Clear sky downward long wave flux", "W/m^2"},
                           /* 164 */   {"cfnsf", "Cloud forcing net solar flux", "W/m^2"},
                           /* 165 */   {"cfnlf", "Cloud forcing net long wave flux", "W/m^2"},
                           /* 166 */   {"vbdsf", "Visible beam downward solar flux", "W/m^2"},
                           /* 167 */   {"vddsf", "Visible diffuse downward solar flux", "W/m^2"},
                           /* 168 */   {"nbdsf", "Near IR beam downward solar flux", "W/m^2"},
                           /* 169 */   {"nddsf", "Near IR diffuse downward solar flux", "W/m^2"},
                           /* 170 */   {"ustr", "U wind stress", "N/m^2"},
                           /* 171 */   {"vstr", "V wind stress", "N/m^2"},
                           /* 172 */   {"mflx", "Momentum flux", "N/m^2"},
                           /* 173 */   {"lmh", "Mass point model surface", ""},
                           /* 174 */   {"lmv", "Velocity point model surface", ""},
                           /* 175 */   {"sglyr", "Neraby model level", ""},
                           /* 176 */   {"nlat", "Latitude", "deg"},
                           /* 177 */   {"nlon", "Longitude", "deg"},
                           /* 178 */   {"umas", "Mass weighted u", "gm/m*K*s"},
                           /* 179 */   {"vmas", "Mass weigtted v", "gm/m*K*s"},
                           /* 180 */   {"var180", "undefined", "undefined"},
                           /* 181 */   {"lpsx", "x-gradient of log pressure", "1/m"},
                           /* 182 */   {"lpsy", "y-gradient of log pressure", "1/m"},
                           /* 183 */   {"hgtx", "x-gradient of height", "m/m"},
                           /* 184 */   {"hgty", "y-gradient of height", "m/m"},
                           /* 185 */   {"stdz", "Standard deviation of Geop. hgt.", "m"},
                           /* 186 */   {"stdu", "Standard deviation of zonal wind", "m/s"},
                           /* 187 */   {"stdv", "Standard deviation of meridional wind", "m/s"},
                           /* 188 */   {"stdq", "Standard deviation of spec. hum.", "gm/gm"},
                           /* 189 */   {"stdt", "Standard deviation of temperature", "K"},
                           /* 190 */   {"cbuw", "Covariance between u and omega", "m/s*Pa/s"},
                           /* 191 */   {"cbvw", "Covariance between v and omega", "m/s*Pa/s"},
                           /* 192 */   {"cbuq", "Covariance between u and specific hum", "m/s*gm/gm"},
                           /* 193 */   {"cbvq", "Covariance between v and specific hum", "m/s*gm/gm"},
                           /* 194 */   {"cbtw", "Covariance between T and omega", "K*Pa/s"},
                           /* 195 */   {"cbqw", "Covariance between spec. hum and omeg", "gm/gm*Pa/s"},
                           /* 196 */   {"cbmzw", "Covariance between v and u", "m^2/si^2"},
                           /* 197 */   {"cbtzw", "Covariance between u and T", "K*m/s"},
                           /* 198 */   {"cbtmw", "Covariance between v and T", "K*m/s"},
                           /* 199 */   {"stdrh", "Standard deviation of Rel. Hum.", "%"},
                           /* 200 */   {"sdtz", "Std dev of time tend of geop. hgt", "m"},
                           /* 201 */   {"icwat", "Ice-free water surface", "%"},
                           /* 202 */   {"sdtu", "Std dev of time tend of zonal wind", "m/s"},
                           /* 203 */   {"sdtv", "Std dev of time tend of merid wind", "m/s"},
                           /* 204 */   {"dswrf", "Downward solar radiation flux", "W/m^2"},
                           /* 205 */   {"dlwrf", "Downward long wave radiation flux", "W/m^2"},
                           /* 206 */   {"sdtq", "Std dev of time tend of spec. hum", "gm/gm"},
                           /* 207 */   {"mstav", "Moisture availability", "%"},
                           /* 208 */   {"sfexc", "Exchange coefficient", "(kg/m^3)(m/s)"},
                           /* 209 */   {"mixly", "No. of mixed layers next to surface", "integer"},
                           /* 210 */   {"sdtt", "Std dev of time tend of temperature", "K"},
                           /* 211 */   {"uswrf", "Upward short wave flux", "W/m^2"},
                           /* 212 */   {"ulwrf", "Upward long wave flux", "W/m^2"},
                           /* 213 */   {"cdlyr", "Non-convective cloud", "%"},
                           /* 214 */   {"cprat", "Convective precip. rate", "kg/m^2/s"},
                           /* 215 */   {"ttdia", "Temperature tendency by all physics", "K/s"},
                           /* 216 */   {"ttrad", "Temperature tendency by all radiation", "K/s"},
                           /* 217 */   {"ttphy", "Temperature tendency by non-radiation physics", "K/s"},
                           /* 218 */   {"preix", "Precip index (0.0-1.00)", "fraction"},
                           /* 219 */   {"tsd1d", "Std. dev. of IR T over 1x1 deg area", "K"},
                           /* 220 */   {"nlgsp", "Natural log of surface pressure", "ln(kPa)"},
                           /* 221 */   {"sdtrh", "Std dev of time tend of rel humt", "%"},
                           /* 222 */   {"5wavh", "5-wave geopotential height", "gpm"},
                           /* 223 */   {"cwat", "Plant canopy surface water", "kg/m^2"},
                           /* 224 */   {"pltrs", "Maximum stomato plant resistance", "s/m"},
                           /* 225 */   {"rhcld", "RH-type cloud cover", "%"},
                           /* 226 */   {"bmixl", "Blackadar's mixing length scale", "m"},
                           /* 227 */   {"amixl", "Asymptotic mixing length scale", "m"},
                           /* 228 */   {"pevap", "Potential evaporation", "kg^2"},
                           /* 229 */   {"snohf", "Snow melt heat flux", "W/m^2"},
                           /* 230 */   {"snoev", "Snow sublimation heat flux", "W/m^2"},
                           /* 231 */   {"mflux", "Convective cloud mass flux", "Pa/s"},
                           /* 232 */   {"dtrf", "Downward total radiation flux", "W/m^2"},
                           /* 233 */   {"utrf", "Upward total radiation flux", "W/m^2"},
                           /* 234 */   {"bgrun", "Baseflow-groundwater runoff", "kg/m^2"},
                           /* 235 */   {"ssrun", "Storm surface runoff", "kg/m^2"},
                           /* 236 */   {"var236", "undefined", "undefined"},
                           /* 237 */   {"ozone", "Total column ozone concentration", "Dobson"},
                           /* 238 */   {"snoc", "Snow cover", "%"},
                           /* 239 */   {"snot", "Snow temperature", "K"},
                           /* 240 */   {"glcr", "Permanent snow points", "mask"},
                           /* 241 */   {"lrghr", "Large scale condensation heating rate", "K/s"},
                           /* 242 */   {"cnvhr", "Deep convective heating rate", "K/s"},
                           /* 243 */   {"cnvmr", "Deep convective moistening rate", "kg/kg/s"},
                           /* 244 */   {"shahr", "Shallow convective heating rate", "K/s"},
                           /* 245 */   {"shamr", "Shallow convective moistening rate", "kg/kg/s"},
                           /* 246 */   {"vdfhr", "Vertical diffusion heating rate", "K/s"},
                           /* 247 */   {"vdfua", "Vertical diffusion zonal accel", "m/s/s"},
                           /* 248 */   {"vdfva", "Vertical diffusion meridional accel", "m/s/s"},
                           /* 249 */   {"vdfmr", "Vertical diffusion moistening rate", "kg/kg/s"},
                           /* 250 */   {"swhr", "Solar radiative heating rate", "K/s"},
                           /* 251 */   {"lwhr", "Longwave radiative heating rate", "K/s"},
                           /* 252 */   {"cd", "Drag coefficient", ""},
                           /* 253 */   {"fricv", "Friction velocity", "m/s"},
                           /* 254 */   {"ri", "Richardson number", ""},
                           /* 255 */   {"var255", "undefined", "undefined"}
            };
            int npar = defaulttable_ncep_reanal2.GetLength(0);        
            List<Parameter> parameters = new List<Parameter>(npar);
            for (int n = 0; n < npar; ++n)
            {
                String pname = defaulttable_ncep_reanal2[n, 0];
                String pdesc = defaulttable_ncep_reanal2[n, 1];
                String punit = defaulttable_ncep_reanal2[n, 2];
                parameters.Add(new Parameter(n, pname, pdesc, punit));
            }
            aTables.Add(new GribPDSParamTable("ncep_reanal2.1", 7, -1, 1, parameters));
            aTables.Add(new GribPDSParamTable("ncep_reanal2.2", 7, -1, 2, parameters));
            aTables.Add(new GribPDSParamTable("ncep_reanal2.3", 7, -1, 3, parameters));
            aTables.Add(new GribPDSParamTable("ncep_reanal2.4", 81, -1, 3, parameters));
            aTables.Add(new GribPDSParamTable("ncep_reanal2.5", 88, -1, 2, parameters));
            aTables.Add(new GribPDSParamTable("ncep_reanal2.6", 88, -1, 128, parameters));
            //添加国家气象中心参数表
            AddNMCTable(aTables);
        }

        //添加国家气象中心tableVersion=0的参数表（针对T213数据）
        private static void AddNMCTable(List<GribPDSParamTable> aTables)
        {
            //写有//*或注释的项为根据节目广播表中t213数据某要素的取值及文档中说明定义的，其他为未涉及项
            string[,] defaulttable_ncep_reanal2 = new string[256, 3]
            {
                           /*   0 */   {"var0", "undefined", "undefined"},
                           /*   1 */   {"pres", "Pressure", "Pa"},                    //*
                           /*   2 */   {"prmsl", "Pressure reduced to MSL", "Pa"},    //*
                           /*   3 */   {"ptend", "Pressure tendency", "Pa/s"},
                           /*   4 */   {"var4", "undefined", "undefined"},
                           /*   5 */   {"var5", "undefined", "undefined"},
                           /*   6 */   {"gp", "Geopotential", "m^2/s^2"},
                           /*   7 */   {"hgt", "Geopotential height", "gpm"},  //*
                           /*   8 */   {"dist", "Geometric height", "m"},
                           /*   9 */   {"hstdv", "Std dev of height", "m"},
                           /*  10 */   {"hvar", "Varianance of height", "m^2"},
                           /*  11 */   {"tmp", "Temperature", "K"},           //*
                           /*  12 */   {"vtmp", "Virtual temperature", "K"},
                           /*  13 */   {"pot", "Potential temperature", "K"},
                           /*  14 */   {"epot", "Pseudo-adiabatic pot. temperature", "K"},
                           /*  15 */   {"tmax", "Max. temperature", "K"},
                           /*  16 */   {"tmin", "Min. temperature", "K"},
                           /*  17 */   {"dpt", "Dew point temperature", "K"},
                           /*  18 */   {"depr", "Dew point depression", "K"},
                           /*  19 */   {"lapr", "Lapse rate", "K/m"},
                           /*  20 */   {"visib", "Visibility", "m"},
                           /*  21 */   {"rdsp1", "Radar spectra (1)", ""},
                           /*  22 */   {"rdsp2", "Radar spectra (2)", ""},
                           /*  23 */   {"rdsp3", "Radar spectra (3)", ""},
                           /*  24 */   {"var24", "undefined", "undefined"},
                           /*  25 */   {"tmpa", "Temperature anomaly", "K"},
                           /*  26 */   {"presa", "Pressure anomaly", "Pa"},
                           /*  27 */   {"gpa", "Geopotential height anomaly", "gpm"},
                           /*  28 */   {"wvsp1", "Wave spectra (1)", ""},
                           /*  29 */   {"wvsp2", "Wave spectra (2)", ""},
                           /*  30 */   {"wvsp3", "Wave spectra (3)", ""},
                           /*  31 */   {"wdir", "Wind direction", "deg"},
                           /*  32 */   {"wind", "Wind speed", "m/s"},
                           /*  33 */   {"ugrd", "u wind", "m/s"},  //*
                           /*  34 */   {"vgrd", "v wind", "m/s"},  //*
                           /*  35 */   {"strm", "Stream function", "m^2/s"},
                           /*  36 */   {"vpot", "Velocity potential", "m^2/s"},
                           /*  37 */   {"mntsf", "Montgomery stream function", "m^2/s^2"},
                           /*  38 */   {"sgcvv", "Sigma coord. vertical velocity", "/s"},
                           /*  39 */   {"vvel", "Pressure vertical velocity", "Pa/s"},
                           /*  40 */   {"dzdt", "Geometric vertical velocity", "m/s"}, //*
                           /*  41 */   {"absv", "Absolute vorticity", "/s"},   //*
                           /*  42 */   {"absd", "Absolute divergence", "/s"},  //*
                           /*  43 */   {"relv", "Relative vorticity", "/s"},
                           /*  44 */   {"reld", "Relative divergence", "/s"},
                           /*  45 */   {"vucsh", "Vertical u shear", "/s"},
                           /*  46 */   {"vvcsh", "Vertical v shear", "/s"},
                           /*  47 */   {"dirc", "Direction of current", "deg"},
                           /*  48 */   {"spc", "Speed of current", "m/s"},
                           /*  49 */   {"uogrd", "u of current", "m/s"},
                           /*  50 */   {"vogrd", "v of current", "m/s"},
                           /*  51 */   {"spfh", "Specific humidity", "kg/kg"},       ////*
                           /*  52 */   {"rh", "Relative humidity", "%"},          ///*
                           /*  53 */   {"mixr", "Humidity mixing ratio", "kg/kg"},
                           /*  54 */   {"pwat", "Precipitable water", "kg/m^2"},
                           /*  55 */   {"vapp", "Vapor pressure", "Pa"},
                           /*  56 */   {"satd", "Saturation deficit", "Pa"},
                           /*  57 */   {"evp", "Evaporation", "kg/m^2"},
                           /*  58 */   {"cice", "Cloud Ice", "kg/m^2"},
                           /*  59 */   {"prate", "Precipitation rate", "kg/m^2/s"},
                           /*  60 */   {"tstm", "Thunderstorm probability", "%"},
                           /*  61 */   {"apcp", "Total precipitation", "kg/m^2"},
                           /*  62 */   {"ncpcp", "Large scale precipitation", "kg/m^2"},
                           /*  63 */   {"acpcp", "Convective precipitation", "kg/m^2"},
                           /*  64 */   {"srweq", "Snowfall rate water equiv.", "kg/m^2/s"},
                           /*  65 */   {"weasd", "Accum. snow", "kg/m^2"},
                           /*  66 */   {"snod", "Snow depth", "m"},
                           /*  67 */   {"mixht", "Mixed layer depth", "m"},
                           /*  68 */   {"tthdp", "Transient thermocline depth", "m"},
                           /*  69 */   {"mthd", "Main thermocline depth", "m"},
                           /*  70 */   {"mtha", "Main thermocline anomaly", "m"},
                           /*  71 */   {"tcdc", "Total cloud cover", "%"},
                           /*  72 */   {"cdcon", "Convective cloud cover", "%"},
                           /*  73 */   {"lcdc", "Low level cloud cover", "%"},
                           /*  74 */   {"mcdc", "Mid level cloud cover", "%"},
                           /*  75 */   {"hcdc", "High level cloud cover", "%"},
                           /*  76 */   {"cwat", "Cloud water", "kg/m^2"},
                           /*  77 */   {"var77", "undefined", "undefined"},
                           /*  78 */   {"snoc", "Convective snow", "kg/m^2"},
                           /*  79 */   {"snol", "Large scale snow", "kg/m^2"},
                           /*  80 */   {"wtmp", "Water temperature", "K"},
                           /*  81 */   {"land", "Land cover (land=1;sea=0)", "fraction"},
                           /*  82 */   {"dslm", "Deviation of sea level from mean", "m"},
                           /*  83 */   {"sfcr", "Surface roughness", "m"},
                           /*  84 */   {"albdo", "Albedo", "%"},
                           /*  85 */   {"tsoil", "Soil temperature", "K"},
                           /*  86 */   {"soilm", "Soil moisture content", "kg/m^2"},
                           /*  87 */   {"veg", "Vegetation", "%"},
                           /*  88 */   {"salty", "Salinity", "kg/kg"},
                           /*  89 */   {"den", "Density", "kg/m^3"},
                           /*  90 */   {"runof", "Runoff", "kg/m^2"},
                           /*  91 */   {"icec", "Ice concentration (ice=1;no ice=0)", "fraction"},
                           /*  92 */   {"icetk", "Ice thickness", "m"},
                           /*  93 */   {"diced", "Direction of ice drift", "deg"},
                           /*  94 */   {"siced", "Speed of ice drift", "m/s"},
                           /*  95 */   {"uice", "u of ice drift", "m/s"},
                           /*  96 */   {"vice", "v of ice drift", "m/s"},
                           /*  97 */   {"iceg", "Ice growth rate", "m/s"},
                           /*  98 */   {"iced", "Ice divergence", "/s"},
                           /*  99 */   {"snom", "Snow melt", "kg/m^2"},
                           /* 100 */   {"htsgw", "Sig height of wind waves and swell", "m"},
                           /* 101 */   {"wvdir", "Direction of wind waves", "deg"},
                           /* 102 */   {"wvhgt", "Sig height of wind waves", "m"},
                           /* 103 */   {"wvper", "Mean period of wind waves", "s"},
                           /* 104 */   {"swdir", "Direction of swell waves", "deg"},
                           /* 105 */   {"swell", "Sig height of swell waves", "m"},
                           /* 106 */   {"swper", "Mean period of swell waves", "s"},
                           /* 107 */   {"dirpw", "Primary wave direction", "deg"},
                           /* 108 */   {"perpw", "Primary wave mean period", "s"},
                           /* 109 */   {"dirsw", "Secondary wave direction", "deg"},
                           /* 110 */   {"persw", "Secondary wave mean period", "s"},
                           /* 111 */   {"nswrs", "Net short wave (surface)", "W/m^2"},
                           /* 112 */   {"nlwrs", "Net long wave (surface)", "W/m^2"},
                           /* 113 */   {"nswrt", "Net short wave (top)", "W/m^2"},
                           /* 114 */   {"nlwrt", "Net long wave (top)", "W/m^2"},
                           /* 115 */   {"lwavr", "Long wave", "W/m^2"},
                           /* 116 */   {"swavr", "Short wave", "W/m^2"},
                           /* 117 */   {"grad", "Global radiation", "W/m^2"},
                           /* 118 */   {"var118", "undefined", "undefined"},
                           /* 119 */   {"var119", "undefined", "undefined"},
                           /* 120 */   {"var120", "undefined", "undefined"},
                           /* 121 */   {"lhtfl", "Latent heat flux", "W/m^2"},
                           /* 122 */   {"shtfl", "Sensible heat flux", "W/m^2"},
                           /* 123 */   {"blydp", "Boundary layer dissipation", "W/m^2"},
                           /* 124 */   {"uflx", "Zonal momentum flux", "N/m^2"},
                           /* 125 */   {"vflx", "Meridional momentum flux", "N/m^2"},
                           /* 126 */   {"wmixe", "Wind mixing energy", "J"},
                           /* 127 */   {"imgd", "Image data", ""},
                           /* 128 */   {"kx", "K index", "K"},   //k指数分析 {"kx", "K index", "K"},133   //*
                           /* 129 */   {"mslma", "Mean sea level pressure (MAPS)", "Pa"},
                           /* 130 */   {"mslet", "Mean sea level pressure (ETA model)", "Pa"},
                           /* 131 */   {"lftx", "Surface lifted index", "K"},
                           /* 132 */   {"4lftx", "Best (4-layer) lifted index", "K"},
                           /* 133 */   {"kx", "K index", "K"},
                           /* 134 */   {"sx", "Sweat index", "K"},
                           /* 135 */   {"mconv", "Horizontal moisture divergence", "kg/kg/s"},
                           /* 136 */   {"vssh", "Vertical speed shear", "1/s"},
                           /* 137 */   {"tslsa", "3-hr pressure tendency (Std Atmos Red)", "Pa/s"},
                           /* 138 */   {"bvf2", "Brunt-Vaisala frequency^2", "1/s^2"},
                           /* 139 */   {"pvmw", "Potential vorticity (mass-weighted)", "1/s/m"},
                           /* 140 */   {"crain", "Categorical rain", "yes=1;no=0"},
                           /* 141 */   {"cfrzr", "Categorical freezing rain", "yes=1;no=0"},
                           /* 142 */   {"cicep", "Categorical ice pellets", "yes=1;no=0"},
                           /* 143 */   {"csnow", "Categorical snow", "yes=1;no=0"},
                           /* 144 */   {"soilw", "Volumetric soil moisture", "fraction"},
                           /* 145 */   {"pevpr", "Potential evaporation rate", "W/m^2"},
                           /* 146 */   {"cwork", "Cloud work function", "J/kg"},
                           /* 147 */   {"u-gwd", "Zonal gravity wave stress", "N/m^2"},
                           /* 148 */   {"v-gwd", "Meridional gravity wave stress", "N/m^2"},
                           /* 149 */   {"pvort", "Potential vorticity", "m^2/s/kg"},
                           /* 150 */   {"var150", "undefined", "undefined"},
                           /* 151 */   {"var151", "undefined", "undefined"},
                           /* 152 */   {"var152", "undefined", "undefined"},
                           /* 153 */   {"mfxdv", "Moisture flux divergence", "gr/gr*m/s/m"},
                           /* 154 */   {"var154", "undefined", "undefined"},
                           /* 155 */   {"mfx", "Moisture flux", "undefined"},              //水汽通量  moisture flux
                           /* 156 */   {"mfxdv", "Moisture flux divergence", "gr/gr*m/s/m"},            //*水汽通量散度分析 {"mfxdv", "Moisture flux divergence", "gr/gr*m/s/m"}153
                           /* 157 */   {"cape", "Convective Avail. Pot. Energy", "J/kg"},
                           /* 158 */   {"epot", "Pseudo-adiabatic pot. temperature", "K"},         //*假相当位温    /*  14 */   {"epot", "Pseudo-adiabatic pot. temperature", "K"},14
                           /* 159 */   {"condp", "Lifted parcel condensation pressure", "Pa"},
                           /* 160 */   {"csusf", "Clear sky upward solar flux", "W/m^2"},
                           /* 161 */   {"csdsf", "Clear sky downward solar flux", "W/m^2"},
                           /* 162 */   {"csulf", "Clear sky upward long wave flux", "W/m^2"},
                           /* 163 */   {"csdlf", "Clear sky downward long wave flux", "W/m^2"},
                           /* 164 */   {"cfnsf", "Cloud forcing net solar flux", "W/m^2"},
                           /* 165 */   {"cfnlf", "Cloud forcing net long wave flux", "W/m^2"},
                           /* 166 */   {"vbdsf", "Visible beam downward solar flux", "W/m^2"},
                           /* 167 */   {"vddsf", "Visible diffuse downward solar flux", "W/m^2"},
                           /* 168 */   {"nbdsf", "Near IR beam downward solar flux", "W/m^2"},
                           /* 169 */   {"nddsf", "Near IR diffuse downward solar flux", "W/m^2"},
                           /* 170 */   {"ustr", "U wind stress", "N/m^2"},
                           /* 171 */   {"vstr", "V wind stress", "N/m^2"},
                           /* 172 */   {"mflx", "Momentum flux", "N/m^2"},
                           /* 173 */   {"lmh", "Mass point model surface", ""},
                           /* 174 */   {"lmv", "Velocity point model surface", ""},
                           /* 175 */   {"sglyr", "Neraby model level", ""},
                           /* 176 */   {"nlat", "Latitude", "deg"},
                           /* 177 */   {"nlon", "Longitude", "deg"},
                           /* 178 */   {"umas", "Mass weighted u", "gm/m*K*s"},
                           /* 179 */   {"vmas", "Mass weigtted v", "gm/m*K*s"},
                           /* 180 */   {"var180", "undefined", "undefined"},
                           /* 181 */   {"lpsx", "x-gradient of log pressure", "1/m"},
                           /* 182 */   {"lpsy", "y-gradient of log pressure", "1/m"},
                           /* 183 */   {"hgtx", "x-gradient of height", "m/m"},
                           /* 184 */   {"hgty", "y-gradient of height", "m/m"},
                           /* 185 */   {"stdz", "Standard deviation of Geop. hgt.", "m"},
                           /* 186 */   {"stdu", "Standard deviation of zonal wind", "m/s"},
                           /* 187 */   {"stdv", "Standard deviation of meridional wind", "m/s"},
                           /* 188 */   {"stdq", "Standard deviation of spec. hum.", "gm/gm"},
                           /* 189 */   {"stdt", "Standard deviation of temperature", "K"},
                           /* 190 */   {"cbuw", "Covariance between u and omega", "m/s*Pa/s"},
                           /* 191 */   {"cbvw", "Covariance between v and omega", "m/s*Pa/s"},
                           /* 192 */   {"cbuq", "Covariance between u and specific hum", "m/s*gm/gm"},
                           /* 193 */   {"cbvq", "Covariance between v and specific hum", "m/s*gm/gm"},
                           /* 194 */   {"cbtw", "Covariance between T and omega", "K*Pa/s"},
                           /* 195 */   {"cbqw", "Covariance between spec. hum and omeg", "gm/gm*Pa/s"},
                           /* 196 */   {"cbmzw", "Covariance between v and u", "m^2/si^2"},
                           /* 197 */   {"cbtzw", "Covariance between u and T", "K*m/s"},
                           /* 198 */   {"cbtmw", "Covariance between v and T", "K*m/s"},
                           /* 199 */   {"stdrh", "Standard deviation of Rel. Hum.", "%"},
                           /* 200 */   {"sdtz", "Std dev of time tend of geop. hgt", "m"},
                           /* 201 */   {"icwat", "Sea level pressure error", "Pa"},            //海平面气压预报误差  
                           /* 202 */   {"sdtu", "Std dev of time tend of zonal wind", "m/s"},
                           /* 203 */   {"sdtv", "Std dev of time tend of merid wind", "m/s"},
                           /* 204 */   {"dswrf", "Downward solar radiation flux", "W/m^2"},
                           /* 205 */   {"dlwrf", "Downward long wave radiation flux", "W/m^2"},
                           /* 206 */   {"sdtq", "Std dev of time tend of spec. hum", "gm/gm"},
                           /* 207 */   {"mstav", "Moisture availability", "%"},
                           /* 208 */   {"hgte", "Height error", "m"},   //高度预报误差
                           /* 209 */   {"mixly", "No. of mixed layers next to surface", "integer"},
                           /* 210 */   {"sdtt", "Std dev of time tend of temperature", "K"},
                           /* 211 */   {"gtmp", "Ground Temperature", "K"},              //地面或水面特性层地面温度 
                           /* 212 */   {"teme", "Temperature error", "K"},               //温度预报误差
                           /* 213 */   {"cdlyr", "Non-convective cloud", "%"},
                           /* 214 */   {"cprat", "Convective precip. rate", "kg/m^2/s"},
                           /* 215 */   {"ttdia", "Temperature tendency by all physics", "K/s"},
                           /* 216 */   {"tadv", "Temperature advection", "undefined"},   //温度平流
                           /* 217 */   {"depr", "Dew point depression", "K"},   //温度露点差    {"depr", "Dew point depression", "K"}  18
                           /* 218 */   {"preix", "Precip index (0.0-1.00)", "fraction"},
                           /* 219 */   {"tsd1d", "Std. dev. of IR T over 1x1 deg area", "K"},
                           /* 220 */   {"nlgsp", "Natural log of surface pressure", "ln(kPa)"},
                           /* 221 */   {"sdtrh", "Std dev of time tend of rel humt", "%"},
                           /* 222 */   {"5wavh", "5-wave geopotential height", "gpm"},
                           /* 223 */   {"cwat", "Plant canopy surface water", "kg/m^2"},
                           /* 224 */   {"pltrs", "Maximum stomato plant resistance", "s/m"},
                           /* 225 */   {"rhcld", "RH-type cloud cover", "%"},
                           /* 226 */   {"bmixl", "Blackadar's mixing length scale", "m"},
                           /* 227 */   {"amixl", "Asymptotic mixing length scale", "m"},
                           /* 228 */   {"pevap", "Potential evaporation", "kg^2"},
                           /* 229 */   {"stmp", "Surface temperature", "K"},       //地面层温度
                           /* 230 */   {"rh", "Relative humidity", "%"}, //相对湿度  52
                           /* 231 */   {"mflux", "Convective cloud mass flux", "Pa/s"},
                           /* 232 */   {"dtrf", "Downward total radiation flux", "W/m^2"},
                           /* 233 */   {"utrf", "Upward total radiation flux", "W/m^2"},
                           /* 234 */   {"bgrun", "Baseflow-groundwater runoff", "kg/m^2"},
                           /* 235 */   {"ssrun", "Storm surface runoff", "kg/m^2"},
                           /* 236 */   {"var236", "undefined", "undefined"},
                           /* 237 */   {"ozone", "Total column ozone concentration", "Dobson"},
                           /* 238 */   {"snoc", "Snow cover", "%"},
                           /* 239 */   {"snot", "Snow temperature", "K"},
                           /* 240 */   {"glcr", "Permanent snow points", "mask"},
                           /* 241 */   {"vadv", "Vorticity advection", "K/s"},  //涡度平流分析 vorticity advection
                           /* 242 */   {"cnvhr", "Deep convective heating rate", "K/s"},
                           /* 243 */   {"cnvmr", "Deep convective moistening rate", "kg/kg/s"},
                           /* 244 */   {"shahr", "Shallow convective heating rate", "K/s"},
                           /* 245 */   {"shamr", "Shallow convective moistening rate", "kg/kg/s"},
                           /* 246 */   {"vdfhr", "Vertical diffusion heating rate", "K/s"},
                           /* 247 */   {"vdfua", "Vertical diffusion zonal accel", "m/s/s"},
                           /* 248 */   {"vdfva", "Vertical diffusion meridional accel", "m/s/s"},
                           /* 249 */   {"vdfmr", "Vertical diffusion moistening rate", "kg/kg/s"},
                           /* 250 */   {"swhr", "Solar radiative heating rate", "K/s"},
                           /* 251 */   {"lwhr", "Longwave radiative heating rate", "K/s"},
                           /* 252 */   {"cd", "Drag coefficient", ""},
                           /* 253 */   {"fricv", "Friction velocity", "m/s"},
                           /* 254 */   {"ri", "Richardson number", ""},
                           /* 255 */   {"var255", "undefined", "undefined"}
            };
            int npar = defaulttable_ncep_reanal2.GetLength(0);  
            List<Parameter> parameters = new List<Parameter>(npar);
            for (int n = 0; n < npar; ++n)
            {
                String pname = defaulttable_ncep_reanal2[n, 0];
                String pdesc = defaulttable_ncep_reanal2[n, 1];
                String punit = defaulttable_ncep_reanal2[n, 2];
                parameters.Add(new Parameter(n, pname, pdesc, punit));
            }
            aTables.Add(new GribPDSParamTable("ncep_reanal2.1", 0, 38, 0, parameters));
        }

        /**
         * Looks for the parameter table which matches the center, subcenter
         * and table version from the tables array.
         * If this is the first time asking for this table, then the parameters for
         * this table have not been read in yet, so this is done as well.
         * @param center - integer from PDS octet 5, representing Center.
         * @param subcenter - integer from PDS octet 26, representing Subcenter
         * @param number - integer from PDS octet 4, representing Parameter Table Version
         * @return GribPDSParamTable matching center, subcenter, and number
         * @throws NotSupportedException 
         */

        ////////////////////////

        public static GribPDSParamTable GetParameterTable(int center, int subcenter, int number)
        {
            try
            {
                GribPDSParamTable._tables = new List<GribPDSParamTable>();
                InitDefaultTableEntries(_tables);
                _paramTables = _tables;
            }
            catch (Exception e)
            {

            }
            /* 1) search excact match                   (center, table)
               2) if (1) failed, search matching table  ( - ,table(1..3))
            */
            for (int i = _paramTables.Count - 1; i >= 0; i--)
            {
                GribPDSParamTable table = _paramTables[i];
                if (table._centerId == -1)
                    continue;
                if (center == table._centerId)
                {
                    if (table._subcenterId == -1 || subcenter == table._subcenterId)
                    {
                        if (number == table._tableNumber)
                        {
                            // now that this table is being used, check to see if the
                            //   parameters for this table have been read in yet.
                            // If not, initialize table and read them in now.
                            //table.readParameterTable();
                            return table;
                        }
                    }
                }
            }
            //search matching table  ( - ,table(1..3))
            for (int i = _paramTables.Count - 1; i >= 0; i--)
            {
                GribPDSParamTable table = _paramTables[i];
                if (table._centerId == -1 && number == table._tableNumber)
                {
                    return table;
                }
            }
            return null;
        }

        public static GribPDSParamTable GetDefaultParameterTable()
        {
            if (_tables == null || _tables.Count == 0)
                InitDefaultTableEntries(_tables);
            if (_tables == null || _tables.Count == 0)
                return null;
            return _tables[0];
        }

        /// <summary>
        /// 获取参数及其信息
        /// </summary>
        /// <param name="id">参数编号</param>
        public Parameter GetParameter(int id)
        {
            if (id < 0 || id >= m_NPARAMETERS)
            {
                //throw new IllegalArgumentException("Bad id: " + id);
            }
            if (_parameters[id] == null)
            {
                return new Parameter(id, "undef_" + id, "undef", "undef");
            }
            return _parameters[id];
        }

        /// <summary>
        /// 获取参数的标识
        /// </summary>
        /// <param name="id">参数编号</param>
        public String GetParameterTag(int id)
        {
            return GetParameter(id).Name;
        }

        /// <summary>
        /// 获取参数的描述信息
        /// </summary>
        /// <param name="id">参数的编号</param>
        public String GetParameterDescription(int id)
        {
            return GetParameter(id).Description;
        }

        /// <summary>
        /// 获取对参数单位的描述
        /// </summary>
        /// <param name="id">参数编号</param>
        public String GetParameterUnit(int id)
        {
            return GetParameter(id).Unit;
        }

        public int GetCenterId()
        {
            return _centerId;
        }

        public int GetSubcenterId()
        {
            return _subcenterId;
        }

        public int GetTableNumber()
        {
            return _tableNumber;
        }

        public String getFilename()
        {
            return _filename;
        }

        public String ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("-1:" + _centerId + ":" + _subcenterId + ":" + _tableNumber + "\n");
            if (_parameters != null)
            {
                for (int i = 0; i < _parameters.Count; i++)
                {
                    if (_parameters[i] == null) continue;
                    str.Append(_parameters[i].ToString() + "\n");
                }
            }
            return str.ToString();
        }
    }
}
