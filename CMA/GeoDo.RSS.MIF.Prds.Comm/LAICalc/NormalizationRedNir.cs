using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class NormalizationRedNir
    {
        public static UInt16 laiFillVlue = 0;

        #region 归一化红光、近红外
        /// <summary>
        /// 根据每个像元的波段值及角度值、覆盖类型计算输出归一后的可见光、近红外
        /// </summary>
        /// <param name="redAndNir"></param>红光、近红外波段的反射率值(red、nir)
        /// <param name="angles"></param>像元的观测角度值,csza,csaa,cvza,cvaa
        /// <param name="ccovert"></param>土地覆盖类型
        /// <param name="redAndNirZoom"></param>红光、近红外波段反射率的放大倍数
        /// <param name=""></param>输出，归一后的可见光、近红外
        public static UInt16[] NRedNir(UInt16[] redAndNir, float[] angles, byte ccovert, float[] redAndNirZoom)
        {
            float redBand, nirBand, csza, csaa, cvza, cvaa;// sisr;
            //byte  cbackgd=0;
            double sr, vza, sza, phi, NDVI, redBand_c, nirBand_c;
            int backgd = 0, covertp;
            covertp = LAICalc.NormalizedCoverType((int)ccovert);
            redBand = redAndNir[0] / redAndNirZoom[0];//red
            nirBand = redAndNir[1] / redAndNirZoom[1];//nir
            csza = angles[0];//太阳天顶角
            csaa = angles[1];//太阳方位角
            cvza = angles[2];//卫星天顶角
            cvaa = angles[3];//卫星方位角
            if (redBand < 0.001)
                redBand = 0.001f;// in case very small or negative red ref. value 
            #region 计算各个波段的地表反射率
            NDVI = (nirBand - redBand) / (nirBand + redBand);
            redBand_c = 0.022182356 + 0.950173706 * redBand + 0.001741533 * nirBand - 0.06009188 * NDVI + 0.039824286 * NDVI * NDVI;
            nirBand_c = -0.02199 + 0.111894 * redBand + 1.002059 * nirBand + 0.028802 * NDVI - 0.00598 * NDVI * NDVI;
            #endregion
            sr = (nirBand_c) / (redBand_c); //SRF correction for FY-3A/MERSI
            if (covertp == 0 || cvza > 180)//cvza=Viwer's zenith angle
            {
                return new UInt16[] { laiFillVlue, laiFillVlue };
            }
            vza = (double)cvza;//to get the physical value from the digital number of VGT's VZA
            sza = (double)csza;//to get the physical value from the digital number of VGT's SZA
            if (vza > 45)
                vza = 44;
            phi = (double)Math.Abs(cvaa - csaa);//to get phi fro digital number of VGT's VAA and SAA
            if (phi > 180)
                phi = phi - 180;
            double[] NormalizationValue = NormalizationCalculator(sr, vza, sza, phi, covertp, backgd);
            return new UInt16[] { (UInt16)(redBand / NormalizationValue[0]*redAndNirZoom[0]),
                                  (UInt16)(nirBand / NormalizationValue[1]*redAndNirZoom[1]) };
        }

        private static double[] NormalizationCalculator(double sr, double vza, double sza, double phi, int covertp, int backgd)
        {
            double lerate, le_p;
            double[] brdffuncArray = new double[2];//brdffunc计算得到的红光、近红外归一化比值 
            LUTDataInit init = new LUTDataInit();
            double[][] ka1a2co = init.CoverT[covertp - 1].Ka1a2co;
            double[][] Ska1a2co = init.CoverT[covertp - 1].SKa1a2co;
            double[][] cco = init.CoverT[covertp - 1].Cco;
            double[][] Scco = init.CoverT[covertp - 1].SCco;
            double[][] lesr = init.CoverT[covertp - 1].LeSr;
            double[][] lersr = init.CoverT[covertp - 1].LeRsr;
            int[] lines = init.CoverT[covertp - 1].Lines;
            int[] Slines = init.CoverT[covertp - 1].SLines;
            int a12lines = LUTData.a1a2lines[covertp - 1];
            double swirmx = LUTData.swirmax[covertp - 1];
            double swirmn = LUTData.swirmin[covertp - 1];
            lerate = LUTData.a1a2lerate[covertp - 1];
            #region 不同卫星天顶角
            if (sza < 10)
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[0], lesr[0], backgd);// first estimation of LE
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 0, 5, 0, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]);
                }
                else if (vza < 30)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[1], lesr[1], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[24], lesr[24], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 20, 5, 0, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]);
                }
                else if (vza < 45)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[2], lesr[2], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[25], lesr[25], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 40, 5, 0, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]);
                }
                else
                {
                    le_p = LAICalc.lefuncpre(sr, lines[3], lesr[3], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[26], lesr[26], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 50, 5, 0, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]);
                }
                #endregion
            }
            else if (sza < 20)
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[4], lesr[4], backgd);
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 0, 15, 0, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]);
                }
                else if (vza < 30)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[5], lesr[5], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[27], lesr[27], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 20, 15, 0, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]);
                }
                else if (vza < 45)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[6], lesr[6], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[28], lesr[28], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 40, 15, 0, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]);
                }
                else
                {
                    le_p = LAICalc.lefuncpre(sr, lines[7], lesr[7], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[29], lesr[29], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 50, 15, 0, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]);
                }
                #endregion
            }
            else if (sza < 30)
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[8], lesr[8], backgd);
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 0, 25, 0, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]);
                }
                else if (vza < 30)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[9], lesr[9], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[30], lesr[30], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 20, 25, 0, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]);
                }
                else if (vza < 45)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[10], lesr[10], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[31], lesr[31], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 40, 25, 0, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]);
                }
                else
                {
                    le_p = LAICalc.lefuncpre(sr, lines[11], lesr[11], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[32], lesr[32], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 50, 25, 0, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]);
                }
                #endregion
            }
            else if (sza < 40)
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[12], lesr[12], backgd);
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 0, 35, 0, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]);
                }
                else if (vza < 30)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[13], lesr[13], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[33], lesr[33], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 20, 35, 0, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]);
                }
                else if (vza < 45)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[14], lesr[14], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[34], lesr[34], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 40, 35, 0, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]);
                }
                else
                {
                    le_p = LAICalc.lefuncpre(sr, lines[15], lesr[15], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[35], lesr[35], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 50, 35, 0, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]);
                }
                #endregion
            }
            else if (sza < 50)
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[16], lesr[16], backgd);
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 0, 45, 0, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]);
                }
                else if (vza < 30)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[17], lesr[17], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[36], lesr[36], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 20, 45, 0, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]);
                }
                else if (vza < 45)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[18], lesr[18], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[37], lesr[37], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 40, 45, 0, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]);
                }
                else
                {
                    le_p = LAICalc.lefuncpre(sr, lines[19], lesr[19], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[38], lesr[38], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 50, 45, 0, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]);
                }
                #endregion
            }
            else
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[20], lesr[20], backgd);
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 0, 55, 0, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]);
                }
                else if (vza < 30)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[21], lesr[21], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[39], lesr[39], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 20, 55, 0, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]);
                }
                else if (vza < 45)
                {
                    le_p = LAICalc.lefuncpre(sr, lines[22], lesr[22], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[40], lesr[40], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 40, 55, 0, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]);
                }
                else
                {
                    le_p = LAICalc.lefuncpre(sr, lines[23], lesr[23], backgd) * (1 - phi / 180.0) + LAICalc.lefuncpre(sr, lines[41], lesr[41], backgd) * phi / 180.0;
                    brdffuncArray = LAICalc.GetBrdffunc(a12lines, le_p / lerate, 50, 55, 0, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]);
                }
                #endregion
            }
            #endregion

            return brdffuncArray;
        }
        #endregion

    }
}
