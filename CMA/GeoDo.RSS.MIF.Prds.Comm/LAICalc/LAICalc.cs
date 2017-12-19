using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeoDo.RSS.MIF.Prds.Comm
{
    public class LAICalc
    {
        public static UInt16 laiFillVlue = 32767;

        #region 计算每个点的LAI
        /// <summary>
        /// 根据每个像元的波段值及角度值、覆盖类型计算植被指数
        /// </summary>
        /// <param name="b346"></param>3、4、6波段的反射率值(red、nir、swir)
        /// <param name="angles"></param>像元的观测角度值,csza,csaa,cvza,cvaa
        /// <param name="ccovert"></param>土地覆盖类型
        /// <param name="lelai"></param>输出，le,le2,lai,lai2
        public static UInt16[] CalLAI(float[] b346, float[] angles, byte ccovert/*, out UInt16[] lelai*/)
        {
            float b2, b3, swir, csza, csaa, cvza, cvaa;// sisr;
            //byte  cbackgd=0;
            double sr, vza, sza, phi, ros, NDVI, b2_c, b3_c, swir_c;
            int backgd = 0, covertp;
            covertp = NormalizedCoverType((int)ccovert);
            b2 = b346[0];//red
            b3 = b346[1];//nir
            swir = b346[2];//swir
            csza = angles[0];//太阳天顶角
            csaa = angles[1];//太阳方位角
            cvza = angles[2];//卫星天顶角
            cvaa = angles[3];//卫星方位角
            if (b2 < 0.001)
                b2 = 0.001f;// in case very small or negative red ref. value 
            // if(b2>0.01275)
            // b2=0.01275;// in case very large red ref. value 
            #region 计算各个波段的地表反射率
            NDVI = (b3 - b2) / (b3 + b2);
            b2_c = 0.022182356 + 0.950173706 * b2 + 0.001741533 * b3 - 0.06009188 * NDVI + 0.039824286 * NDVI * NDVI;
            b3_c = -0.02199 + 0.111894 * b2 + 1.002059 * b3 + 0.028802 * NDVI - 0.00598 * NDVI * NDVI;
            swir_c = 1.007 * swir + 0.001;
            #endregion
            sr = (b3_c) / (b2_c); //SRF correction for FY-3A/MERSI
            //sisr = (UInt16)(sr * 10);
            if (covertp == 0 || cvza > 180)//cvza=Viwer's zenith angle
            {
                return new UInt16[] { laiFillVlue, laiFillVlue };
            }
            ros = (double)swir_c;//to get the physical value from the digital number of VGT's MIR
            vza = (double)cvza;//to get the physical value from the digital number of VGT's VZA
            sza = (double)csza;//to get the physical value from the digital number of VGT's SZA
            if (vza > 45)
                vza = 44;
            phi = (double)Math.Abs(cvaa - csaa);//to get phi fro digital number of VGT's VAA and SAA
            if (phi > 180)
                phi = phi - 180;
            return LaiCalculator(sr, ros, vza, sza, phi, covertp, backgd); ;
        }

        private static UInt16[] LaiCalculator(double sr, double ros, double vza, double sza, double phi, int covertp, int backgd/*, out  UInt16[] lelai*/)
        {
            double lai, laifromrsr, le = 0, lefromrsr = 0, srmodif, rosmodif, lerate, le_p;
            //UInt16 lai1, lai2;//, le2;
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
                    le_p = lefuncpre(sr, lines[0], lesr[0], backgd);// first estimation of LE
                    //modified simple ratio based on formula (7)
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 0, 5, 0, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]);
                    le = lefunc(srmodif, lines[0], lesr[0], backgd, 0, le_p);//formula (2)
                    if (covertp > 5 && covertp < 18)
                        lefromrsr = le;
                    else
                    {
                        //formula (8)
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate / lerate, 0, 5, 0, Ska1a2co[0], Ska1a2co[1], Scco[0]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[0], Ska1a2co[1], Scco[0]);
                        //formula (3)
                        lefromrsr = lersrf(srmodif, rosmodif, lines[0], Slines[0], lersr[0], backgd, 0, swirmx, swirmn);
                    }
                }
                else if (vza < 30)
                {
                    //first estimation of le, linear interpolating between phi=0 and 180.			
                    le_p = lefuncpre(sr, lines[1], lesr[1], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[24], lesr[24], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 20, 5, 0, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]);
                    le = lefunc(srmodif, lines[1], lesr[1], backgd, 20, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 20, 5, 0, Ska1a2co[0], Ska1a2co[1], Scco[0]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[0], Ska1a2co[1], Scco[0]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[1], Slines[1], lersr[1], backgd, 20, swirmx, swirmn);
                    }
                }
                else if (vza < 45)
                {
                    le_p = lefuncpre(sr, lines[2], lesr[2], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[25], lesr[25], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 40, 5, 0, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]);
                    le = lefunc(srmodif, lines[2], lesr[2], backgd, 40, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 40, 5, 0, Ska1a2co[0], Ska1a2co[1], Scco[0]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[0], Ska1a2co[1], Scco[0]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[2], Slines[2], lersr[2], backgd, 40, swirmx, swirmn);
                    }
                }
                else
                {
                    le_p = lefuncpre(sr, lines[3], lesr[3], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[26], lesr[26], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 50, 5, 0, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[0], ka1a2co[1], ka1a2co[2], ka1a2co[3], cco[0], cco[1]);
                    le = lefunc(srmodif, lines[3], lesr[3], backgd, 50, le_p);
                    if ((covertp > 5) && (covertp < 18))
                    {
                        lefromrsr = le;
                    }
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 50, 5, 0, Ska1a2co[0], Ska1a2co[1], Scco[0]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[0], Ska1a2co[1], Scco[0]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[3], Slines[3], lersr[3], backgd, 50, swirmx, swirmn);
                    }
                }
                #endregion
            }
            else if (sza < 20)
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = lefuncpre(sr, lines[4], lesr[4], backgd);
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 0, 15, 0, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]);
                    le = lefunc(srmodif, lines[4], lesr[4], backgd, 0, le_p);//formula (2)
                    if (covertp > 5 && covertp < 18)
                        lefromrsr = le;
                    else
                    {
                        //formula (8)
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate / lerate, 0, 15, 0, Ska1a2co[2], Ska1a2co[3], Scco[1]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[2], Ska1a2co[3], Scco[1]);
                        //formula (3)
                        lefromrsr = lersrf(srmodif, rosmodif, lines[4], Slines[4], lersr[4], backgd, 0, swirmx, swirmn);
                    }
                }
                else if (vza < 30)
                {
                    //first estimation of le, linear interpolating between phi=0 and 180.			
                    le_p = lefuncpre(sr, lines[5], lesr[5], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[27], lesr[27], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 20, 15, 0, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]);
                    le = lefunc(srmodif, lines[5], lesr[5], backgd, 20, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 20, 15, 0, Ska1a2co[2], Ska1a2co[3], Scco[1]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[2], Ska1a2co[3], Scco[1]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[5], Slines[5], lersr[5], backgd, 20, swirmx, swirmn);
                    }
                }
                else if (vza < 45)
                {
                    le_p = lefuncpre(sr, lines[6], lesr[6], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[28], lesr[28], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 40, 15, 0, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]);
                    le = lefunc(srmodif, lines[6], lesr[6], backgd, 40, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 40, 15, 0, Ska1a2co[2], Ska1a2co[3], Scco[1]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[2], Ska1a2co[3], Scco[1]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[6], Slines[6], lersr[6], backgd, 40, swirmx, swirmn);
                    }
                }
                else
                {
                    le_p = lefuncpre(sr, lines[7], lesr[7], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[29], lesr[29], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 50, 15, 0, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[4], ka1a2co[5], ka1a2co[6], ka1a2co[7], cco[2], cco[3]);
                    le = lefunc(srmodif, lines[7], lesr[7], backgd, 50, le_p);
                    if ((covertp > 5) && (covertp < 18))
                    {
                        lefromrsr = le;
                    }
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 50, 15, 0, Ska1a2co[2], Ska1a2co[3], Scco[1]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[2], Ska1a2co[3], Scco[1]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[7], Slines[7], lersr[7], backgd, 50, swirmx, swirmn);
                    }
                }
                #endregion
            }
            else if (sza < 30)
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = lefuncpre(sr, lines[8], lesr[8], backgd);
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 0, 25, 0, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]);
                    le = lefunc(srmodif, lines[8], lesr[8], backgd, 0, le_p);//formula (2)
                    if (covertp > 5 && covertp < 18)
                        lefromrsr = le;
                    else
                    {
                        //formula (8)
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate / lerate, 0, 25, 0, Ska1a2co[4], Ska1a2co[5], Scco[2]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[4], Ska1a2co[5], Scco[2]);
                        //formula (3)
                        lefromrsr = lersrf(srmodif, rosmodif, lines[8], Slines[8], lersr[8], backgd, 0, swirmx, swirmn);
                    }
                }
                else if (vza < 30)
                {
                    //first estimation of le, linear interpolating between phi=0 and 180.			
                    le_p = lefuncpre(sr, lines[9], lesr[9], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[30], lesr[30], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 20, 25, 0, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]);
                    le = lefunc(srmodif, lines[9], lesr[9], backgd, 20, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 20, 25, 0, Ska1a2co[4], Ska1a2co[5], Scco[2]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[4], Ska1a2co[5], Scco[2]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[9], Slines[9], lersr[9], backgd, 20, swirmx, swirmn);
                    }
                }
                else if (vza < 45)
                {
                    le_p = lefuncpre(sr, lines[10], lesr[10], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[31], lesr[31], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 40, 25, 0, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]);
                    le = lefunc(srmodif, lines[10], lesr[10], backgd, 40, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 40, 25, 0, Ska1a2co[4], Ska1a2co[5], Scco[2]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[4], Ska1a2co[5], Scco[2]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[10], Slines[10], lersr[10], backgd, 40, swirmx, swirmn);
                    }
                }
                else
                {
                    le_p = lefuncpre(sr, lines[11], lesr[11], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[32], lesr[32], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 50, 25, 0, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[8], ka1a2co[9], ka1a2co[10], ka1a2co[11], cco[4], cco[5]);
                    le = lefunc(srmodif, lines[11], lesr[11], backgd, 50, le_p);
                    if ((covertp > 5) && (covertp < 18))
                    {
                        lefromrsr = le;
                    }
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 50, 25, 0, Ska1a2co[4], Ska1a2co[5], Scco[2]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[4], Ska1a2co[5], Scco[2]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[11], Slines[11], lersr[11], backgd, 50, swirmx, swirmn);
                    }
                }
                #endregion
            }
            else if (sza < 40)
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = lefuncpre(sr, lines[12], lesr[12], backgd);
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 0, 35, 0, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]);
                    le = lefunc(srmodif, lines[12], lesr[12], backgd, 0, le_p);//formula (2)
                    if (covertp > 5 && covertp < 18)
                        lefromrsr = le;
                    else
                    {
                        //formula (8)
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate / lerate, 0, 35, 0, Ska1a2co[6], Ska1a2co[7], Scco[3]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[6], Ska1a2co[7], Scco[3]);
                        //formula (3)
                        lefromrsr = lersrf(srmodif, rosmodif, lines[12], Slines[12], lersr[12], backgd, 0, swirmx, swirmn);
                    }
                }
                else if (vza < 30)
                {
                    //first estimation of le, linear interpolating between phi=0 and 180.			
                    le_p = lefuncpre(sr, lines[13], lesr[13], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[33], lesr[33], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 20, 35, 0, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]);
                    le = lefunc(srmodif, lines[13], lesr[13], backgd, 20, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 20, 35, 0, Ska1a2co[6], Ska1a2co[7], Scco[3]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[6], Ska1a2co[7], Scco[3]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[13], Slines[13], lersr[13], backgd, 20, swirmx, swirmn);
                    }
                }
                else if (vza < 45)
                {
                    le_p = lefuncpre(sr, lines[14], lesr[14], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[34], lesr[34], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 40, 35, 0, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]);
                    le = lefunc(srmodif, lines[14], lesr[14], backgd, 40, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 40, 35, 0, Ska1a2co[6], Ska1a2co[7], Scco[3]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[6], Ska1a2co[7], Scco[3]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[14], Slines[14], lersr[14], backgd, 14, swirmx, swirmn);
                    }
                }
                else
                {
                    le_p = lefuncpre(sr, lines[15], lesr[15], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[35], lesr[35], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 50, 35, 0, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[12], ka1a2co[13], ka1a2co[14], ka1a2co[15], cco[6], cco[7]);
                    le = lefunc(srmodif, lines[15], lesr[15], backgd, 50, le_p);
                    if ((covertp > 5) && (covertp < 18))
                    {
                        lefromrsr = le;
                    }
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 50, 35, 0, Ska1a2co[6], Ska1a2co[7], Scco[3]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[6], Ska1a2co[7], Scco[3]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[15], Slines[15], lersr[15], backgd, 50, swirmx, swirmn);
                    }
                }
                #endregion
            }
            else if (sza < 50)
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = lefuncpre(sr, lines[16], lesr[16], backgd);
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 0, 45, 0, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]);
                    le = lefunc(srmodif, lines[16], lesr[16], backgd, 0, le_p);//formula (2)
                    if (covertp > 5 && covertp < 18)
                        lefromrsr = le;
                    else
                    {
                        //formula (8)
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate / lerate, 0, 45, 0, Ska1a2co[8], Ska1a2co[9], Scco[4]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[8], Ska1a2co[9], Scco[4]);
                        //formula (3)
                        lefromrsr = lersrf(srmodif, rosmodif, lines[16], Slines[16], lersr[16], backgd, 0, swirmx, swirmn);
                    }
                }
                else if (vza < 30)
                {
                    //first estimation of le, linear interpolating between phi=0 and 180.			
                    le_p = lefuncpre(sr, lines[17], lesr[17], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[36], lesr[36], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 20, 45, 0, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]);
                    le = lefunc(srmodif, lines[17], lesr[17], backgd, 20, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 20, 45, 0, Ska1a2co[8], Ska1a2co[9], Scco[4]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[8], Ska1a2co[9], Scco[4]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[17], Slines[17], lersr[17], backgd, 20, swirmx, swirmn);
                    }
                }
                else if (vza < 45)
                {
                    le_p = lefuncpre(sr, lines[18], lesr[18], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[37], lesr[37], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 40, 45, 0, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]);
                    le = lefunc(srmodif, lines[18], lesr[18], backgd, 40, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 40, 45, 0, Ska1a2co[8], Ska1a2co[9], Scco[4]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[8], Ska1a2co[9], Scco[4]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[18], Slines[18], lersr[18], backgd, 40, swirmx, swirmn);
                    }
                }
                else
                {
                    le_p = lefuncpre(sr, lines[19], lesr[19], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[38], lesr[38], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 50, 45, 0, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[16], ka1a2co[17], ka1a2co[18], ka1a2co[19], cco[8], cco[9]);
                    le = lefunc(srmodif, lines[19], lesr[19], backgd, 50, le_p);
                    if ((covertp > 5) && (covertp < 18))
                    {
                        lefromrsr = le;
                    }
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 50, 45, 0, Ska1a2co[8], Ska1a2co[9], Scco[4]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[8], Ska1a2co[9], Scco[4]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[19], Slines[19], lersr[19], backgd, 50, swirmx, swirmn);
                    }
                }
                #endregion
            }
            else
            {
                #region 不同观测天顶角
                if (vza < 10)
                {
                    le_p = lefuncpre(sr, lines[20], lesr[20], backgd);
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 0, 55, 0, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]);
                    le = lefunc(srmodif, lines[20], lesr[20], backgd, 0, le_p);//formula (2)
                    if (covertp > 5 && covertp < 18)
                        lefromrsr = le;
                    else
                    {
                        //formula (8)
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate / lerate, 0, 55, 0, Ska1a2co[10], Ska1a2co[11], Scco[5]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[10], Ska1a2co[11], Scco[5]);
                        //formula (3)
                        lefromrsr = lersrf(srmodif, rosmodif, lines[20], Slines[20], lersr[20], backgd, 0, swirmx, swirmn);
                    }
                }
                else if (vza < 30)
                {
                    //first estimation of le, linear interpolating between phi=0 and 180.			
                    le_p = lefuncpre(sr, lines[21], lesr[21], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[39], lesr[39], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 20, 55, 0, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]);
                    le = lefunc(srmodif, lines[21], lesr[21], backgd, 20, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 20, 55, 0, Ska1a2co[10], Ska1a2co[11], Scco[5]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[10], Ska1a2co[11], Scco[5]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[21], Slines[21], lersr[21], backgd, 20, swirmx, swirmn);
                    }
                }
                else if (vza < 45)
                {
                    le_p = lefuncpre(sr, lines[22], lesr[22], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[40], lesr[40], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 40, 55, 0, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]);
                    le = lefunc(srmodif, lines[22], lesr[22], backgd, 40, le_p);
                    if ((covertp > 5) && (covertp < 18))
                        lefromrsr = le;
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 40, 55, 0, Ska1a2co[10], Ska1a2co[11], Scco[5]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[10], Ska1a2co[11], Scco[5]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[22], Slines[22], lersr[22], backgd, 40, swirmx, swirmn);
                    }
                }
                else
                {
                    le_p = lefuncpre(sr, lines[23], lesr[23], backgd) * (1 - phi / 180.0) + lefuncpre(sr, lines[41], lesr[41], backgd) * phi / 180.0;
                    srmodif = sr * brdffunc(a12lines, le_p / lerate, 50, 55, 0, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]) / brdffunc(a12lines, le_p / lerate, vza, sza, phi, ka1a2co[20], ka1a2co[21], ka1a2co[22], ka1a2co[23], cco[10], cco[11]);
                    le = lefunc(srmodif, lines[23], lesr[23], backgd, 50, le_p);
                    if ((covertp > 5) && (covertp < 18))
                    {
                        lefromrsr = le;
                    }
                    else
                    {
                        rosmodif = ros * swirfunc(a12lines, le_p / lerate, 50, 55, 0, Ska1a2co[10], Ska1a2co[11], Scco[5]) / swirfunc(a12lines, le_p / lerate, vza, sza, phi, Ska1a2co[10], Ska1a2co[11], Scco[5]);
                        lefromrsr = lersrf(srmodif, rosmodif, lines[23], Slines[23], lersr[23], backgd, 50, swirmx, swirmn);
                    }
                }
                #endregion
            }
            #endregion
            if ((le <= 0.2) || (lefromrsr < 0))
                lefromrsr = le;//if the calculating LE(from RSR) is negative or LE(SR) less than 0.2, adopt the LE value from the SR approach.
            ////if (covertp > 5 && covertp < 18)//非森林区域
            ////{
            ////le from SR
            //le = (le < LUTData.maxlai[covertp - 1] * LUTData.clump[covertp - 1]) ? le : LUTData.maxlai[covertp - 1] * LUTData.clump[covertp - 1];
            ////lai from SR
            //lai = le * 100 / LUTData.clump[covertp - 1];
            ////le1=(unsigned short)le;//from SR
            ////lai1 = (UInt16)lai;//from SR
            //return new ushort[] { (UInt16)lai };
            ////} 
            ////else
            {
                //lefromrsr from RSR
                lefromrsr = (lefromrsr < LUTData.maxlai[covertp - 1] * LUTData.clump[covertp - 1]) ? lefromrsr : LUTData.maxlai[covertp - 1] * LUTData.clump[covertp - 1];
                //laifromrsr from RSR
                laifromrsr = lefromrsr * 100 / LUTData.clump[covertp - 1];
                //le2 = (UInt16)lefromrsr;//from RSR and SR
                //lai2 = (UInt16)laifromrsr;//from RSR and SR
                return new ushort[] { (UInt16)laifromrsr };
            }
            //return new ushort[] { lai1, lai2 };
        }

        /// <summary>
        /// 按照既有标准统一覆盖类型
        /// </summary>
        /// <param name="covertpin"></param>
        /// <returns></returns>
        public static int NormalizedCoverType(int covertpin)
        {
            int covertp;
            #region 对输入的covertype进行分类、对应
            switch (covertpin)  //from input GLCC to Deng's class IGBP
            {
                case 1:  //GLCC=Tree cover, broadleaved, evergreen,常绿阔叶林
                    {
                        covertp = 2;//IGBP=evergreen broadlaef forest
                        break;
                    }

                case 2:	//GLCC=Tree cover, broadleaved, deciduous, closed，封闭落叶阔叶林
                    {
                        covertp = 2; //IGBP=evergreen broadlaef forest
                        break;
                    }

                case 3: //GLCC=Tree cover, broadleaved, deciduous, open，开放落叶阔叶林
                    {
                        covertp = 2; //IGBP=evergreen broadlaef forest
                        break;
                    }

                case 4: //GLCC=Tree cover, needle-leaved, evergreen，常绿针叶林
                    {
                        covertp = 1; //IGBP=evergreen needleleaf
                        break;
                    }

                case 5: //GLCC=Tree cover, needle-leaved, deciduous,落叶针叶林
                    {
                        covertp = 3;//deciduous needleleaf
                        break;
                    }

                case 6://Tree cover, mixed leaf type，混交林
                    {
                        covertp = 5;//mixed forest
                        break;
                    }

                case 7://GLCC=Tree cover, regular flooded, fresh water，淡水洪泛区森林
                    {
                        covertp = 5;//mixed forest
                        break;
                    }

                case 8: //GLCC=Tree cover, regular flooded, saline water,咸水洪泛区森林
                    {
                        covertp = 5; //mixed forest
                        break;
                    }

                case 9: //GLCC=Mosaic: Tree cover/other natural vegetation，自然植被
                    {
                        covertp = 8; //woody savannas,多树草原
                        break;
                    }

                case 10://GLCC=Tree cover, burnt，过火森林
                    {
                        covertp = 5; //mixed forest，
                        break;
                    }

                case 11: //GLCC=Shrub Cover, closed-open, evergreen,常绿灌丛
                    {
                        covertp = 6;//IGBP=closed shrubland
                        break;
                    }

                case 12://GLCC=Shrub Cover, closed-open, deciduous，落叶灌丛
                    {
                        covertp = 7; //IGBP=open shrub
                        break;
                    }

                case 13://GLCC=Herbaceous cover, closed-open,草皮覆被
                    {
                        covertp = 10;//IGBP=grassland
                        break;
                    }

                case 14: //GLCC=Sparse herbaceous or sparse shrub cover,稀疏灌丛/草皮
                    {
                        covertp = 0; //
                        break;
                    }

                case 15: //GLCC=Regular flooded shrub and/or herbaceous cover,洪泛区灌丛/草皮
                    {
                        covertp = 11; //IGBP=permanent wetlands，永久湿地
                        break;
                    }

                case 16: //GLCC=Cultivated and managed areas,农田区域
                    {
                        covertp = 12; //IGBP=crop land
                        break;
                    }

                case 17://GLCC=Mosaic: Cropland/ Shrub and / or grass cover，农田/灌丛/草地覆盖
                    {
                        covertp = 14; //Cropland Mosaic
                        break;
                    }

                case 18: //GLCC=Mosaic: Cropland/ Shrub and / or grass cover
                    {
                        covertp = 0;
                        break;
                    }
                case 19:  //GLCC=Bare Areas，裸地
                    {
                        covertp = 0;
                        break;
                    }
                case 20://GLCC=water
                    {
                        covertp = 0;
                        break;
                    }
                case 21://GLCC=snow or ice
                    {
                        covertp = 0;
                        break;
                    }
                case 22:
                    {
                        covertp = 13;
                        break;
                    }
                case 0:
                    {
                        covertp = 0;
                        break;
                    }
                default:
                    covertp = 0;
                    break;
            }
            #endregion
            return covertp;
        }

        //LESRfunc.h
        public static double lefuncpre(double sr, int lines, double[] lesrco, int backgd)
        {
            double maxsr = ((double)(lines + 23)) / 10.0;
            if (sr >= maxsr)
                sr = maxsr;
            sr = (2.4 - LUTData.BkgdSR[backgd]) / (maxsr - LUTData.BkgdSR[backgd]) * (maxsr - sr) + sr;//formula (11)
            if (sr <= 2.4)
                return 0;
            sr = sr * 10;
            double lep = chebyshev(chterm, sr, lines, lesrco);
            //a linear interpolation designed to adjust very small LE value 
            //to prevent negative calculating value.
            if (sr <= 30.0)
            {
                lep = chebyshev(chterm, 30.0, lines, lesrco) / 6.0 * (sr - 24.0);
                if (lep <= 0.0)
                    lep = chebyshev(chterm, 35.0, lines, lesrco) / 11.0 * (sr - 24.0);
            }
            return lep;
        }

        //LESRfunc.h
        public static double lefunc(double sr, int lines, double[] lesrco, int backgd, double vza, double le_p)
        {
            double maxsr = ((double)(lines + 23)) / 10.0;
            if (sr >= maxsr)
                sr = maxsr;
            sr = (2.4 - LUTData.BkgdSR[backgd]) * Math.Exp(-0.5 * le_p / Math.Cos(vza * Math.PI / 180)) / (maxsr - LUTData.BkgdSR[backgd]) * (maxsr - sr) + sr;//formula (11)
            //sr=(2.4-BkgdSR[backgd])/(maxsr-BkgdSR[backgd])*(maxsr-sr)+sr;//formula (11)
            if (sr <= 2.4)
                return 0;
            sr = sr * 10;
            double le = chebyshev(chterm, sr, lines, lesrco);
            //a linear interpolation designed to adjust very small LE value 
            //to prevent negative calculating value.
            if (sr <= 30.0)
            {
                le = chebyshev(chterm, 30.0, lines, lesrco) / 6.0 * (sr - 24.0);
                if (le <= 0.0)
                    le = chebyshev(chterm, 35.0, lines, lesrco) / 11.0 * (sr - 24.0);
            }

            return le;
        }

        //BRDFfunc.h
        public static double brdffunc(int a12lines, double le, double vza, double sza, double phi, double[] reda1co, double[] reda2co, double[] nira1co, double[] nira2co, double[] redc, double[] nirc)
        {
            vza = vza * Math.PI / 180;
            sza = sza * Math.PI / 180;
            phi = phi * Math.PI / 180;
            double le10 = le * 10;
            double smllgv = smllg(vza, sza, phi);
            double reda1 = chebyshev(chibterm, le10, a12lines, reda1co);
            double reda2 = chebyshev(chibterm, le10, a12lines, reda2co);
            double redfunc = (1 + reda1 * f1(vza, sza, phi) + reda2 * f2(vza, sza, phi)) * (1 + redc[0] * Math.Exp(-redc[1] * smllgv / Math.PI));
            if (redfunc <= 0) redfunc = 0.00001;
            double nira1 = chebyshev(chibterm, le10, a12lines, nira1co);
            double nira2 = chebyshev(chibterm, le10, a12lines, nira2co);
            double nirfunc = (1 + nira1 * f1(vza, sza, phi) + nira2 * f2(vza, sza, phi)) * (1 + nirc[0] * Math.Exp((-nirc[1] * smllgv / Math.PI)));
            if (nirfunc <= 0) nirfunc = 0.00001;
            double brdffunc = nirfunc / redfunc;
            return brdffunc;
        }

        public static double[] GetBrdffunc(int a12lines, double le, double vza, double sza, double phi, double[] reda1co, double[] reda2co, double[] nira1co, double[] nira2co, double[] redc, double[] nirc)
        {
            vza = vza * Math.PI / 180;
            sza = sza * Math.PI / 180;
            phi = phi * Math.PI / 180;
            double le10 = le * 10;
            double smllgv = smllg(vza, sza, phi);
            double reda1 = chebyshev(chibterm, le10, a12lines, reda1co);
            double reda2 = chebyshev(chibterm, le10, a12lines, reda2co);
            double redfunc = (1 + reda1 * f1(vza, sza, phi) + reda2 * f2(vza, sza, phi)) * (1 + redc[0] * Math.Exp(-redc[1] * smllgv / Math.PI));
            if (redfunc <= 0) redfunc = 0.00001;
            double nira1 = chebyshev(chibterm, le10, a12lines, nira1co);
            double nira2 = chebyshev(chibterm, le10, a12lines, nira2co);
            double nirfunc = (1 + nira1 * f1(vza, sza, phi) + nira2 * f2(vza, sza, phi)) * (1 + nirc[0] * Math.Exp((-nirc[1] * smllgv / Math.PI)));
            if (nirfunc <= 0) nirfunc = 0.00001;
            return new double[] { redfunc, nirfunc };
        }

        /// <summary>
        ///BRDFsmllg.h
        ///function to calculate angle difference between the sun and the viewer
        ///based on the formula in the Nomenclature section
        /// </summary>
        /// <param name="vza"></param>
        /// <param name="sza"></param>
        /// <param name="phi"></param>
        /// <returns></returns>
        private static double smllg(double vza, double sza, double phi)
        {

            double coss = Math.Cos(sza);
            double cosv = Math.Cos(vza);
            double sins = Math.Sqrt(1.0 - coss * coss);
            double sinv = Math.Sqrt(1.0 - cosv * cosv);
            double cosp = Math.Cos(phi);
            double cosmllg = coss * cosv + sins * sinv * cosp;
            double smllg = Math.Acos(cosmllg);
            return smllg;
        }

        /// <summary>
        /// BRDFf1.h,f1 function based on formula (5)
        /// </summary>
        /// <param name="vza"></param>
        /// <param name="sza"></param>
        /// <param name="phi"></param>
        /// <returns></returns>
        private static double f1(double vza, double sza, double phi)
        {
            double coss = Math.Cos(sza);
            double cosv = Math.Cos(vza);
            double sins = Math.Sqrt(1.0 - coss * coss);
            double sinv = Math.Sqrt(1.0 - cosv * cosv);
            double cosp = Math.Cos(phi);
            double sinp = Math.Sqrt(1.0 - cosp * cosp);
            double tans = sins / coss;
            double tanv = sinv / cosv;
            double D = Math.Sqrt(tans * tans + tanv * tanv - 2.0 * tans * tanv * cosp);
            double f1 = ((Math.PI - phi) * cosp + sinp) * tans * tanv / 2 / Math.PI - (tans + tanv + D) / Math.PI;
            return f1;
        }

        /// <summary>
        /// BRDFf2.h,f2 function based on formula (6)
        /// </summary>
        /// <param name="vza"></param>
        /// <param name="sza"></param>
        /// <param name="phi"></param>
        /// <returns></returns>
        private static double f2(double vza, double sza, double phi)
        {
            double coss = Math.Cos(sza);
            double cosv = Math.Cos(vza);
            double sins = Math.Sqrt(1.0 - coss * coss);
            double sinv = Math.Sqrt(1.0 - cosv * cosv);
            double cosp = Math.Cos(phi);
            double cosmllg = coss * cosv + sins * sinv * cosp;
            double smllg = Math.Acos(cosmllg);
            double f2 = (((Math.PI / 2 - smllg) * cosmllg + Math.Sin(smllg)) / (coss + cosv) - Math.PI / 4) * 4 / (3 * Math.PI);
            return f2;
        }

        /// <summary>
        /// BRDFswir.h,BRDF function for SWIR based on formula (10)
        /// </summary>
        /// <param name="a12lines"></param>
        /// <param name="le"></param>
        /// <param name="vza"></param>
        /// <param name="sza"></param>
        /// <param name="phi"></param>
        /// <param name="swira1co"></param>
        /// <param name="swira2co"></param>
        /// <param name="swirc"></param>
        /// <returns></returns>
        private static double swirfunc(int a12lines, double le, double vza, double sza, double phi, double[] swira1co, double[] swira2co, double[] swirc)
        {
            vza = vza * Math.PI / 180;
            sza = sza * Math.PI / 180;
            phi = phi * Math.PI / 180;
            double le10 = le * 10;
            double smllgv = smllg(vza, sza, phi);
            double swira1 = chebyshev(chibterm, le10, a12lines, swira1co);
            double swira2 = chebyshev(chibterm, le10, a12lines, swira2co);
            double swirf = (1 + swira1 * f1(vza, sza, phi) + swira2 * f2(vza, sza, phi)) * (1 + swirc[0] * Math.Exp(-swirc[1] * smllgv / Math.PI));
            if (swirf <= 0) swirf = 0.00001;
            return swirf;
        }

        /// <summary>
        ///LERSRfunc.h,to calculate effective LAI from the RSR, based on formula (17)
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="rosmodif"></param>
        /// <param name="lines"></param>
        /// <param name="Slines"></param>
        /// <param name="lersrco"></param>
        /// <param name="backgd"></param>
        /// <param name="vza"></param>
        /// <param name="swmax"></param>
        /// <param name="swmin"></param>
        /// <returns></returns>
        private static double lersrf(double sr, double rosmodif, int lines, int Slines, double[] lersrco, int backgd, double vza, double swmax, double swmin)
        {
            double maxsr = ((double)(lines + 23)) / 10.0;
            if (sr > maxsr) sr = maxsr;
            sr = (2.4 - LUTData.BkgdSR[backgd]) * Math.Cos(vza * Math.PI / 180) / (maxsr - LUTData.BkgdSR[backgd]) * (maxsr - sr) + sr;//formula (11)
            if (sr <= 2.4) return 0;
            if (rosmodif > swmax) rosmodif = swmax;
            if (rosmodif < swmin) rosmodif = swmin;
            double rsr = sr * (swmax - rosmodif) / (swmax - swmin);
            /*
            if (rosmodif>0.4) rosmodif=0.4;
	        if (rosmodif<0.05) rosmodif=0.05;
	        double rsr=sr*(0.4-rosmodif)/0.35;
            */
            double maxrsr = ((double)(Slines - 1)) / 10.0;
            if (rsr > maxrsr) rsr = maxrsr;
            rsr = rsr * 10.0;
            double le = chebyshev(RSRchterm, rsr, Slines, lersrco);
            return le;
        }

        /// <summary>
        ///chebytermforLERSR.h
        ///chebyshev polynomials of the second kind, based on the formula (15) and (16)
        ///for calculation of LE from RSR
        /// </summary>
        /// <param name="total"></param>
        /// <param name="vari"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static double RSRchterm(int total, double vari, int n)
        {
            double c = 1.9998 / (total - 1);
            double x = -0.9999 + (vari - 0) * c;//formula (17)
            if (n == 0)
                return 1;
            else if (n == 1)
                return 2 * x;
            else
                return 2 * x * RSRchterm(total, vari, n - 1) - RSRchterm(total, vari, n - 2);
        }

        private static double chterm(int total, double vari, int n)
        {
            double c = 1.9998 / (total - 1);
            double x = -0.9999 + (vari - 24) * c;//formula (16)
            if (n == 0)
                return 1;
            else if (n == 1)
                return 2 * x;
            else return
                2 * x * chterm(total, vari, n - 1) - chterm(total, vari, n - 2);

        }

        /// <summary>
        ///chebytermfora1a2.h
        ///Chebyshev polynimials of the second kind based on (15) and (16)
        ///for the calculation of a1 and a2 
        /// </summary>
        /// <param name="total"></param>
        /// <param name="vari"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static double chibterm(int total, double vari, int n)
        {
            double c = 1.9998 / (total - 1);
            double x = -0.9999 + (vari - 1) * c;//formula (17)
            if (n == 0)
                return 1;
            else if (n == 1)
                return 2 * x;
            else
                return 2 * x * chibterm(total, vari, n - 1) - chibterm(total, vari, n - 2);
            /*double x=-0.9988+(vari-1)*0.0454;
            if (n==0) return 1;
            else if(n==1) return 2*x;
            else return 2*x*chibterm(total,vari,n-1)-chibterm(total,vari,n-2);*/
        }

        private static double chebyshev(Func<int, double, int, double> term, double x, int t, double[] c)
        {
            double a = 0.0;
            for (int i = 0; i < 10; i++)
            {
                a += c[i] * term(t, x, i);
            }
            return a;
        }
        #endregion
    }
}
