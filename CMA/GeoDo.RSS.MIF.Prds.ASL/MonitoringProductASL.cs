using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using GeoDo.RSS.MIF.Core;
using GeoDo.RSS.MIF.Prds.Comm;

namespace GeoDo.RSS.MIF.Prds.ASL
{
    [Export(typeof(IMonitoringProduct)), ExportMetadata("VERSION", "1")]
    public class MonitoringProductASL : MonitoringProduct
    {
        #region MODIS数据反演的陆地气溶胶产品LASL
        /*MODIS数据反演的气溶胶产品
        * 标识代码为MOD04,格式为标准的img格式，包含8个通道，数据格式为Floating-point (32 bits) 
         * MOD04.A2013014.0300.005.img
         * 产品代码MOD04，A代表AM,为Terra卫星标识(SatelliteID),2013014中的2013为年，014为从1月1日算起的天数，         *
        */        
         #region 产品的ENVI头文件
            /*
             ENVI
                description = {Direct Broadcast Aerosol Product}
                samples =  1354
                lines =  2030
                bands =   8
                data type =  4
                header offset =   0
                interleave = bil 
                byte order = 0
                band names = {
                band  1: Latitude,
                band  2: Longitude,
                band  3: Corrected_Optical_Depth_Land_.47micron,
                band  4: Corrected_Optical_Depth_Land_.55micron,
                band  5: Corrected_Optical_Depth_Land_.66micron,
                band  6: SDS_Dust_Weighting,
                band  7: Angstrom Coefficient,
                band  8: Mass Concentration
                }
                band units = {
                band  1: deg,
                band  2: deg,
                band  3: NA,
                band  4: NA,
                band  5: NA,
                band  6: NA,
                band  7: NA,
                band  8: ug/cm2,
                }
                bad value = -327.68
             * 
             */
         #endregion
        /*8个通道中包含经纬度和6个产品，
         * 分别是：
         * 纬度（单位： 度），
         * 精度（单位： 度），
         * 0.47微米通道的气溶胶光学厚度（无单位），
         * 0.55微米通道的气溶胶光学厚度（无单位），
         * 0.66微米通道的气溶胶光学厚度（无单位），
         * 科学数据集（SDS）中像元的沙尘（dust）权重（无单位），DSTW
         * MODIS Angstrom波长指数（无单位），等于ln(tau.47/tau.66)/ln(0.47/0.66),MAWC
         * 柱状质量浓度(columnar mass concentration)（单位： ug/cm2），0CMC
        */
        #endregion

        public MonitoringProductASL()
            : base()
        {

        }

        protected override ProductDef GetProductDef()
        {
            return MonitoringThemeFactory.GetProductDef("CMA", "ASL");
        }

        protected override Dictionary<string, Type> LoadSubProducts()
        {
            Dictionary<string, Type> subproducts = new Dictionary<string, Type>();
            ////0.47微米通道的气溶胶光学厚度（无单位），047T
            //subproducts.Add("L47T", typeof(SubProductL47TASL));
            // 0.55微米通道的气溶胶光学厚度（无单位），055T
            subproducts.Add("0IMG", typeof(SubProductU5TTASL));
            //// 0.66微米通道的气溶胶光学厚度（无单位），066T
            //subproducts.Add("L66T", typeof(SubProductL66TASL));
            //// 科学数据集（SDS）中像元的沙尘（dust）权重（无单位），DSTW
            //subproducts.Add("LDTW", typeof(SubProductLDTWASL));
            //// MODIS Angstrom波长指数（无单位），等于ln(tau.47/tau.66)/ln(0.47/0.66),MAWC
            //subproducts.Add("LMAC", typeof(SubProductLMACASL));
            //// 柱状质量浓度(columnar mass concentration)（单位： ug/cm2），0CMC
            //subproducts.Add("LCMC", typeof(SubProductLCMCASL));
            //面积统计
            subproducts.Add("STAT", typeof(SubProductSTATASL));
            return subproducts;
        }


    }
}
